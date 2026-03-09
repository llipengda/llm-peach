using System;
using System.Collections.Generic;
using System.Linq;
using Peach.Core;
using Peach.Core.Dom;
using Peach.Core.IO;

using SArray = System.Array;

namespace Peach.LLM.Core.Fixups.DTLS
{
    public class DtlsFixers
    {
        #region Helpers
        // =======================================================================
        // Helpers
        // =======================================================================

        private static DataElement FindHandshakeBody(DataElement packet, out string msgType)
        {
            msgType = null;
            var record = packet.find("dtls_record");
            if (record == null) return null;

            // Check for handshake_record
            var handshakeRecord = record.find("handshake_record");
            if (handshakeRecord == null) return null;

            // Navigate: handshake_data -> handshake_messages -> non_encrypted
            var hsData = handshakeRecord.find("handshake_data");
            var hsMsgs = hsData?.find("handshake_messages");
            var nonEncrypted = hsMsgs?.find("non_encrypted") as Choice;

            if (nonEncrypted == null) return null;

            // non_encrypted is a Choice; get the selected element (ClientHello, ServerHello, etc.)
            // In Peach DOM, for a Choice, we usually look at elements. Since this is a fixer running on a generated model,
            // we assume one child is active/selected.
            var selected = nonEncrypted.SelectedElement;
            if (selected == null) return null;
            msgType = selected.Name; // e.g., "client_hello", "server_hello", etc.
            return selected;
        }

        private static DataElement FindBlock(DataElement parent, string name)
        {
            return parent?.find(name);
        }

        private static void SetBlob(DataElement blobElem, byte[] data)
        {
            if (blobElem == null) return;
            blobElem.SetValue(new Variant(data));
        }
        #endregion
        #region C2S Fixers

        // =======================================================================
        // Client-to-Server Fixers (Single Packet Only)
        // =======================================================================

        // SHOT-0: ClientHello MUST be first client handshake message.
        // skipped

        // SHOT-2: CipherSuite list length >=2 and multiple of 2.
        public static void FixC2sShot2ClienthelloCipherSuitesLen(DataElement elem)
        {
            string msgType;
            var bodyBlock = FindHandshakeBody(elem, out msgType);
            if (bodyBlock == null || msgType != "client_hello") return;

            var body = bodyBlock.find("body"); // dtls_client_hello_t
            var suitesBlob = body?.find("cipher_suites");

            if (suitesBlob == null) return;

            byte[] data = suitesBlob.Bytes() ?? (new byte[0]);
            bool changed = false;

            // Ensure even length
            if ((data.Length & 1) != 0)
            {
                SArray.Resize(ref data, data.Length - 1);
                changed = true;
            }

            // Ensure min length 2
            if (data.Length < 2)
            {
                // TLS_RSA_WITH_AES_128_CBC_SHA (0x002F)
                data = new byte[] { 0x00, 0x2F };
                changed = true;
            }

            // Max length check (DTLS_MAX_CIPHER_SUITES_BYTES) is implicit in Peach Relation if handled, 
            // but we can enforce a safe limit if needed. Assuming 65534 based on u16.
            // C code uses a define, we'll skip arbitrary clamping unless it exceeds u16.

            if (changed)
                SetBlob(suitesBlob, data);
        }

        // SHOT-3: CompressionMethods list must contain at least one method.
        public static void FixC2sShot3ClienthelloCompressionMethods(DataElement elem)
        {
            string msgType;
            var bodyBlock = FindHandshakeBody(elem, out msgType);
            if (bodyBlock == null || msgType != "client_hello") return;

            var body = bodyBlock.find("body");
            var compBlob = body?.find("compression_methods");

            if (compBlob == null) return;

            byte[] data = compBlob.Bytes();
            if (data == null || data.Length == 0)
            {
                // null compression (0x00)
                SetBlob(compBlob, new byte[] { 0x00 });
            }
        }

        // SHOT-4: If extensions present, length-prefixed correctly.
        // skipped - usually handled by Peach relations/structure.

        // SHOT-6: SessionID length 0..32.
        public static void FixC2sShot6ClienthelloSessionIdLen(DataElement elem)
        {
            string msgType;
            var bodyBlock = FindHandshakeBody(elem, out msgType);
            if (bodyBlock == null || msgType != "client_hello") return;

            var body = bodyBlock.find("body");
            var sessionIdBlock = body?.find("session_id");
            var idBlob = sessionIdBlock?.find("id");

            if (idBlob == null) return;

            byte[] data = idBlob.Bytes();
            if (data != null && data.Length > 32)
            {
                SArray.Resize(ref data, 32);
                SetBlob(idBlob, data);
            }
        }

        // SHOT-11: cookie length 0..255.
        public static void FixC2sShot11ClienthelloCookieLen(DataElement elem)
        {
            string msgType;
            var bodyBlock = FindHandshakeBody(elem, out msgType);
            if (bodyBlock == null || msgType != "client_hello") return;

            var body = bodyBlock.find("body");
            var cookieBlob = body?.find("cookie");

            if (cookieBlob == null) return;

            byte[] data = cookieBlob.Bytes();
            // DTLS_MAX_COOKIE_LEN is typically 32 or 255 depending on implementation/version. 
            // RFC 6347 4.2.1 says < 2^8. So 255.
            if (data != null && data.Length > 255)
            {
                SArray.Resize(ref data, 255);
                SetBlob(cookieBlob, data);
            }
        }

        // SHOT-23: Client MUST send CCS immediately before Finished.
        // skipped

        // SHOT-24: CCS must be single byte 0x01.
        public static void FixC2sShot24CcsValue(DataElement elem)
        {
            var record = elem.find("dtls_record");
            var ccsRecord = record?.find("ccs_record");
            if (ccsRecord == null) return;

            var ccsBody = ccsRecord.find("ccs_body");
            var valElem = ccsBody?.find("value");

            if (valElem != null)
            {
                valElem.SetValue(new Variant(1));
            }
        }

        // SHOT-25: Finished immediately after CCS.
        // skipped

        // SHOT-28/29: handshake ordering;
        // skipped

        // SHOT-30/34: Application Data MUST NOT be sent before handshake complete.
        // skipped

        // SHOT-31: monotonically increasing message_seq.
        public static void FixC2sShot31MonotonicMessageSeq(DataElement elem)
        {
            var packets = elem.root.find("packets") as Peach.Core.Dom.Array;
            if (packets == null) return;
            int packetIndex = packets.IndexOf(elem);
            if (packetIndex < 0) return;
            var record = elem.find("dtls_record");
            if (record == null) return;
            var handshakeRecord = record.find("handshake_record");
            if (handshakeRecord == null) return;
            var msgSeqElem = handshakeRecord.find("message_seq");
            if (msgSeqElem == null) return;
            msgSeqElem.SetValue(new Variant((ushort)packetIndex));
        }

        // SHOT-32: fragment_length must not exceed total length. We'll canonicalize frag fields.
        // Sets offset=0, frag_len=len.
        public static void FixC2sShot32FragmentLengthOk(DataElement elem)
        {
            string msgType;
            var msgBlock = FindHandshakeBody(elem, out msgType);
            if (msgBlock == null) return;

            // Fields in the message block: length, fragment_offset, fragment_length
            var lenElem = msgBlock.find("length");
            var fragOffElem = msgBlock.find("fragment_offset");
            var fragLenElem = msgBlock.find("fragment_length");

            if (lenElem != null && fragOffElem != null && fragLenElem != null)
            {
                // Read the actual length (which might be calculated by relation from Body)
                // Note: In Peach, if 'length' has a relation, getting its value returns the calculated value 
                // if the relation has updated.
                long lenVal = (long)lenElem.InternalValue;

                fragOffElem.SetValue(new Variant(0));
                fragLenElem.SetValue(new Variant(lenVal));
            }
        }
        #endregion
        #region S2C Fixers

        // =======================================================================
        // Server-to-Client Fixers (Single Packet Only)
        // =======================================================================

        // SHOT-0: ServerHello MUST be first server handshake message.
        // skipped

        // SHOT-1: server_version <= client_version.
        // skipped

        // SHOT-2/3: Random fixed size; session_id len 0..32.
        public static void FixS2cShot3ServerhelloSessionIdLen(DataElement elem)
        {
            string msgType;
            var bodyBlock = FindHandshakeBody(elem, out msgType);
            if (bodyBlock == null || msgType != "server_hello") return;

            var body = bodyBlock.find("body");
            var sessionIdBlock = body?.find("session_id");
            var idBlob = sessionIdBlock?.find("id");

            if (idBlob == null) return;

            byte[] data = idBlob.Bytes();
            if (data != null && data.Length > 32)
            {
                SArray.Resize(ref data, 32);
                SetBlob(idBlob, data);
            }
        }

        // SHOT-4: server cipher_suite selected from client-offered list.
        // skipped

        // SHOT-5: server compression_method from client list.
        // skipped

        // SHOT-6: ServerHello extensions length-prefixed.
        // skipped - usually handled by Peach relations/structure.

        // SHOT-10: HVR cookie len 0..255.
        public static void FixS2cShot10HvrCookieLen(DataElement elem)
        {
            string msgType;
            var bodyBlock = FindHandshakeBody(elem, out msgType);
            if (bodyBlock == null || msgType != "hello_verify_request") return;

            var body = bodyBlock.find("body");
            var cookieBlob = body?.find("cookie");

            if (cookieBlob == null) return;

            byte[] data = cookieBlob.Bytes();
            if (data != null && data.Length > 255)
            {
                SArray.Resize(ref data, 255);
                SetBlob(cookieBlob, data);
            }
        }

        // SHOT-11: HVR server_version <= client_version.
        // skipped

        // SHOT-22: If CertificateRequest is sent, it MUST appear before ServerHelloDone.
        // skipped

        // SHOT-23: CertificateRequest.cert_types list MUST NOT be empty.
        public static void FixS2cShot23CertreqCertTypesNonempty(DataElement elem)
        {
            string msgType;
            var bodyBlock = FindHandshakeBody(elem, out msgType);
            if (bodyBlock == null || msgType != "certificate_request") return;

            var body = bodyBlock.find("body");
            var typesBlob = body?.find("cert_types");

            if (typesBlob == null) return;

            byte[] data = typesBlob.Bytes();
            if (data == null || data.Length == 0)
            {
                // rsa_sign (1)
                SetBlob(typesBlob, new byte[] { 0x01 });
            }
            // Max length check skipped (implicit relation or not critical for structure)
        }

        // SHOT-24: sig_algs length must be even.
        public static void FixS2cShot24CertreqSigAlgsEven(DataElement elem)
        {
            string msgType;
            var bodyBlock = FindHandshakeBody(elem, out msgType);
            if (bodyBlock == null || msgType != "certificate_request") return;

            var body = bodyBlock.find("body");
            var sigsBlob = body?.find("sig_algs");

            if (sigsBlob == null) return;

            byte[] data = sigsBlob.Bytes();
            if (data != null && (data.Length & 1) != 0)
            {
                SArray.Resize(ref data, data.Length - 1);
                SetBlob(sigsBlob, data);
            }
        }

        // SHOT-25: ca_dn_list may be empty but length-prefixed. 
        // We only store ca_dn_len+blob; clamp max length.
        public static void FixS2cShot25CertreqCaDnLen(DataElement elem)
        {
            string msgType;
            var bodyBlock = FindHandshakeBody(elem, out msgType);
            if (bodyBlock == null || msgType != "certificate_request") return;

            var body = bodyBlock.find("body");
            var dnBlob = body?.find("ca_dn_blob");

            if (dnBlob == null) return;

            byte[] data = dnBlob.Bytes();
            // DTLS_MAX_CA_DN_LEN (usually 2^16-1). 
            // If it exceeds USHRT_MAX, the 16-bit length field will wrap or error.
            if (data != null && data.Length > 65535)
            {
                SArray.Resize(ref data, 65535);
                SetBlob(dnBlob, data);
            }
        }

        // SHOT-28/29: Server CCS before its Finished;
        // skipped

        // SHOT-30/31: Server Finished after its CCS;
        // skipped

        // SHOT-33: Server handshake ordering;
        // skipped

        // SHOT-35/36/37: DTLS server sequencing/retransmit;
        // skipped

        // SHOT-38: alert exactly two bytes (struct).
        public static void FixS2cShot38AlertTwoBytes(DataElement elem)
        {
            var record = elem.find("dtls_record");
            var alertRecord = record?.find("alert_record");
            if (alertRecord == null) return;

            var alertBody = alertRecord.find("alert_body");
            if (alertBody == null) return;

            // In the PIT, alert_body has 'level' (8) and 'description' (8).
            // No Blob to resize here, the structure enforces 2 bytes. 
            // If this fixer is meant to fix specific values, we leave it.
            // But the C code essentially did nothing but ensure type=21.
            var typeElem = alertRecord.find("type");
            if (typeElem != null) typeElem.SetValue(new Variant(21));
        }

        // SHOT-35/36: Canonicalize fragments (Same as C2S Shot 32)
        // public static void FixS2cShot3536DtlsSeqAndFrag(DataElement elem)
        // {
        //     FixC2sShot32FragmentLengthOk(elem);
        // }
        #endregion
        #region Common Fixers

        // =======================================================================
        // Common / General Fixers
        // =======================================================================

        public static void FixAppdataLen(DataElement elem)
        {
            var record = elem.find("dtls_record");
            var appRecord = record?.find("app_data_record");
            if (appRecord == null) return;

            var appData = appRecord.find("app_data");
            if (appData == null) return;

            byte[] data = appData.Bytes();
            // DTLS_MAX_APPDATA_LEN approx 16384 or similar. Using a safe default.
            const int MAX_APP_DATA = 16384;
            if (data != null && data.Length > MAX_APP_DATA)
            {
                SArray.Resize(ref data, MAX_APP_DATA);
                SetBlob(appData, data);
            }
        }

        // public static void FixCommonLengthsAndKinds(DataElement elem)
        // {
        //     // Apply common fixes
        //     FixC2sShot32FragmentLengthOk(elem); // Canonicalize handshake
        //     FixAppdataLen(elem);
        //     FixC2sShot24CcsValue(elem);
        //     FixS2cShot38AlertTwoBytes(elem);

        //     // Fix Encrypted length if applicable
        //     var record = elem.find("dtls_record");
        //     var encrypted = record?.find("encrypted"); // Within handshake_all choice?
        //                                                // In PIT, 'encrypted' is a choice sibling of 'non_encrypted' inside 'handshake_messages'
        //                                                // OR 'encrypted' is a choice sibling of 'handshake_record' in 'dtls_packet_t'??
        //                                                // Looking at PIT: dtls_handshake_all -> Choice -> encrypted (Blob data)

        //     // But wait, dtls_packet_t -> Choice dtls_record -> 
        //     // handshake_record -> handshake_data (dtls_handshake_all) -> Choice handshake_messages -> encrypted

        //     if (record != null)
        //     {
        //         var hsRec = record.find("handshake_record");
        //         var hsData = hsRec?.find("handshake_data");
        //         var hsMsgs = hsData?.find("handshake_messages");
        //         var encBlock = hsMsgs?.find("encrypted");
        //         var encData = encBlock?.find("data");

        //         if (encData != null)
        //         {
        //             byte[] data = encData.Bytes();
        //             if (data != null && data.Length > 16384) // DTLS_MAX_CIPHERTEXT_LEN
        //             {
        //                 SArray.Resize(ref data, 16384);
        //                 SetBlob(encData, data);
        //             }
        //         }
        //     }
        // }
        #endregion

        // =======================================================================
        // Skipped Multi-Packet Fixers (Comments)
        // =======================================================================
        /*
         * Skipped fixers requiring Packet Array context:
         * - fix_c2s_shot_0_clienthello_first: Requires reordering packets.
         * - fix_c2s_shot_23_ccs_before_finished: Requires scanning previous packets.
         * - fix_c2s_shot_25_finished_after_ccs: Requires scanning previous packets.
         * - fix_c2s_shot_28_29_handshake_order: Requires reordering.
         * - fix_c2s_shot_30_34_no_appdata_before_done: Requires reordering.
         * - fix_c2s_shot_31_monotonic_message_seq: Requires global counter state.
         * - fix_s2c_shot_0_serverhello_first: Requires reordering.
         * - fix_s2c_shot_1_server_version_le_client: Requires ClientHello from different packet.
         * - fix_s2c_shot_4_serverhello_cipher_suite_from_client: Requires ClientHello.
         * - fix_s2c_shot_5_serverhello_compression_from_client: Requires ClientHello.
         * - fix_s2c_shot_11_hvr_version_le_client: Requires ClientHello.
         * - fix_s2c_shot_22_certreq_before_shd: Requires reordering.
         * - fix_s2c_shot_28_29_server_ccs_before_finished: Requires reordering/scanning.
         * - fix_s2c_shot_30_31_server_finished_after_ccs: Requires scanning.
         * - fix_s2c_shot_33_34_server_order_and_no_appdata: Requires reordering.
         * - fix_s2c_shot_40_no_appdata_before_done: Requires reordering.
         */

        public static void FixDtls(DataElement elem)
        {
            if (elem == null) return;


            // ---- Client-to-Server ----
            FixC2sShot2ClienthelloCipherSuitesLen(elem);
            FixC2sShot3ClienthelloCompressionMethods(elem);
            // FixC2sShot4ClienthelloExtensionsLen(elem); // Impl: extensions usually structure, length handled by relation.
            FixC2sShot6ClienthelloSessionIdLen(elem);
            FixC2sShot11ClienthelloCookieLen(elem);
            FixC2sShot31MonotonicMessageSeq(elem);
            FixC2sShot24CcsValue(elem);
            FixC2sShot32FragmentLengthOk(elem);

            // // ---- Server-to-Client ----
            // FixS2cShot3ServerhelloSessionIdLen(elem);
            // // FixS2cShot6ServerhelloExtensionsLen(elem);
            // FixS2cShot10HvrCookieLen(elem);
            // FixS2cShot23CertreqCertTypesNonempty(elem);
            // FixS2cShot24CertreqSigAlgsEven(elem);
            // FixS2cShot25CertreqCaDnLen(elem);
            // FixS2cShot38AlertTwoBytes(elem);
        }
    }
}