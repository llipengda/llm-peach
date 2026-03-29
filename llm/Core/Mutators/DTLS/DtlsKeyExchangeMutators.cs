using System;
using System.IO;
using System.Linq;
using Peach.Core.Dom;
using Peach.Core;
using Peach.Pro.Core.Mutators;
using Peach.LLM.Core.Mutators.DTLS;
using System.ComponentModel;

namespace Peach.LLM.Core.Mutators.DTLS
{
    [Mutator("DtlsMutateServerKeyExchange")]
    [CMutator("mutate_server_key_exchange")]
    [Description("Mutates Server Key Exchange body.")]
    public class DtlsMutateServerKeyExchange : LLMMutator
    {
        public DtlsMutateServerKeyExchange(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj is Block && obj.Name == "body" && obj.IsIn("server_key_exchange");
        }

        public override int count => 1;
        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }
        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        protected override void PerformMutation(DataElement obj)
        {
            // Traverse down to find blobs like 'dh_p', 'ec_point' etc. and mutate them.
            // C code logic dispatches based on structure. Here we rely on recursive
            // finding of blobs or mutating the selected choice.
            var blk = obj as Block;
            var u = blk.find("u") as Choice;
            if(u != null && u.SelectedElement != null)
            {
                // Mutate all blobs found in the selected element
                foreach(var blob in u.SelectedElement.EnumerateAllElements().OfType<Blob>())
                {
                    byte[] val = blob.Bytes();
                    if(val.Length > 0) NextBytes(val);
                    blob.MutatedValue = new Variant(val);
                }
            }
        }
    }

    [Mutator("DtlsMutateClientKeyExchange")]
    [CMutator("mutate_client_key_exchange")]
    [Description("Mutates Client Key Exchange body.")]
    public class DtlsMutateClientKeyExchange : LLMMutator
    {
        public DtlsMutateClientKeyExchange(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj is Block && obj.Name == "body" && obj.IsIn("client_key_exchange");
        }

        public override int count => 1;
        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }
        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        protected override void PerformMutation(DataElement obj)
        {
            var blk = obj as Block;
            var u = blk.find("u") as Choice;
            if (u == null || u.SelectedElement == null) return;

            var selected = u.SelectedElement as Block;
            if (selected == null) return;

            string selectedName = selected.Name;

            // Mutate based on selected key exchange type
            if (selectedName == "rsa")
            {
                // RSA: mutate encrypted_premaster_secret
                var encPmsLen = selected.find("enc_pms_len") as Number;
                var encPms = selected.find("enc_pms") as Blob;
                if (encPmsLen != null && encPms != null)
                {
                    MutateEncryptedPms(encPmsLen, encPms);
                }
            }
            else if (selectedName == "dh")
            {
                // DH: mutate dh_Yc
                var dhYcLen = selected.find("dh_Yc_len") as Number;
                var dhYc = selected.find("dh_Yc") as Blob;
                if (dhYcLen != null && dhYc != null)
                {
                    MutateDhPublic(dhYcLen, dhYc);
                }
            }
            else if (selectedName == "ecdh")
            {
                // ECDH: mutate ec_point
                var ecPointLen = selected.find("ec_point_len") as Number;
                var ecPoint = selected.find("ec_point") as Blob;
                if (ecPointLen != null && ecPoint != null)
                {
                    MutateEcdhClientPublic(ecPointLen, ecPoint);
                }
            }
            else if (selectedName == "psk")
            {
                // PSK: mutate identity
                var identityLen = selected.find("identity_len") as Number;
                var identity = selected.find("identity") as Blob;
                if (identityLen != null && identity != null)
                {
                    MutatePskIdentity(identityLen, identity);
                }
            }
            else if (selectedName == "dhe_psk")
            {
                // DHE_PSK: mutate both identity and dh_Yc
                var identityLen = selected.find("identity_len") as Number;
                var identity = selected.find("identity") as Blob;
                if (identityLen != null && identity != null)
                {
                    MutatePskIdentity(identityLen, identity);
                }
                var dhYcLen = selected.find("dh_Yc_len") as Number;
                var dhYc = selected.find("dh_Yc") as Blob;
                if (dhYcLen != null && dhYc != null)
                {
                    MutateDhPublic(dhYcLen, dhYc);
                }
            }
            else if (selectedName == "rsa_psk")
            {
                // RSA_PSK: mutate both identity and enc_pms
                var identityLen = selected.find("identity_len") as Number;
                var identity = selected.find("identity") as Blob;
                if (identityLen != null && identity != null)
                {
                    MutatePskIdentity(identityLen, identity);
                }
                var encPmsLen = selected.find("enc_pms_len") as Number;
                var encPms = selected.find("enc_pms") as Blob;
                if (encPmsLen != null && encPms != null)
                {
                    MutateEncryptedPms(encPmsLen, encPms);
                }
            }
            else if (selectedName == "ecdhe_psk")
            {
                // ECDHE_PSK: mutate both identity and ec_point
                var identityLen = selected.find("identity_len") as Number;
                var identity = selected.find("identity") as Blob;
                if (identityLen != null && identity != null)
                {
                    MutatePskIdentity(identityLen, identity);
                }
                var ecPointLen = selected.find("ec_point_len") as Number;
                var ecPoint = selected.find("ec_point") as Blob;
                if (ecPointLen != null && ecPoint != null)
                {
                    MutateEcdhClientPublic(ecPointLen, ecPoint);
                }
            }
        }

        private void MutateDhPublic(Number lenElem, Blob blob)
        {
            ushort curLen = (ushort)(ulong)lenElem.InternalValue;
            if (curLen > DtlsUtils.DTLS_MAX_DH_Y_LEN) curLen = (ushort)DtlsUtils.DTLS_MAX_DH_Y_LEN;

            byte[] cur = blob.Bytes();
            if (cur == null || cur.Length != curLen) cur = new byte[curLen];

            uint cat = DtlsUtils.RndU32(8);
            ushort L = curLen;
            byte[] out_buf;

            switch (cat)
            {
                case 0: // A. Canonical form
                    if (L == 0) { L = 64; lenElem.MutatedValue = new Variant((ulong)L); }
                    out_buf = new byte[L];
                    DtlsUtils.FillRand(out_buf, L);
                    DtlsUtils.MixShallowDeep(out_buf, L);
                    blob.MutatedValue = new Variant(out_buf);
                    break;

                case 1: // B. Boundaries
                    {
                        ushort[] b = { 0, 1, 2, 3, 7, 8, 15, 16, 31, 32, 63, 64, 127, 128,
                            (ushort)(DtlsUtils.DTLS_MAX_DH_Y_LEN - 1), (ushort)DtlsUtils.DTLS_MAX_DH_Y_LEN };
                        ushort pick = b[DtlsUtils.RndU32((uint)b.Length)];
                        if (pick > DtlsUtils.DTLS_MAX_DH_Y_LEN) pick = (ushort)DtlsUtils.DTLS_MAX_DH_Y_LEN;
                        L = pick;
                        lenElem.MutatedValue = new Variant((ulong)L);
                        out_buf = new byte[L];
                        switch (DtlsUtils.RndU32(4))
                        {
                            case 0: for (int i = 0; i < L; i++) out_buf[i] = 0x00; break;
                            case 1: for (int i = 0; i < L; i++) out_buf[i] = 0xFF; break;
                            case 2: for (int i = 0; i < L; i++) out_buf[i] = (byte)i; break;
                            default: for (int i = 0; i < L; i++) out_buf[i] = (byte)((i & 1) != 0 ? 0xAA : 0x55); break;
                        }
                        DtlsUtils.MixShallowDeep(out_buf, L);
                        blob.MutatedValue = new Variant(out_buf);
                    }
                    break;

                default:
                    // Other categories: use similar patterns
                    if (L == 0) L = 64;
                    lenElem.MutatedValue = new Variant((ulong)L);
                    out_buf = new byte[L];
                    DtlsUtils.FillRand(out_buf, L);
                    DtlsUtils.MixShallowDeep(out_buf, L);
                    blob.MutatedValue = new Variant(out_buf);
                    break;
            }
        }

        private void MutateEcdhClientPublic(Number lenElem, Blob blob)
        {
            byte curLen = (byte)(ulong)lenElem.InternalValue;
            if (curLen > 255) curLen = 255;

            byte[] cur = blob.Bytes();
            if (cur == null || cur.Length != curLen) cur = new byte[curLen];

            uint cat = DtlsUtils.RndU32(8);
            ushort L = curLen;
            byte[] out_buf;

            switch (cat)
            {
                case 0: // A. Canonical form
                    if (L < 1) { L = 65; lenElem.MutatedValue = new Variant((ulong)L); }
                    out_buf = new byte[L];
                    out_buf[0] = 0x04; // uncompressed
                    if (L > 1)
                    {
                        for (int i = 1; i < L; i++)
                        {
                            out_buf[i] = (byte)DtlsUtils.UrandCert(256);
                        }
                    }
                    DtlsUtils.MixShallowDeep(out_buf, L);
                    blob.MutatedValue = new Variant(out_buf);
                    break;

                case 1: // B. Boundaries
                    {
                        byte[] b = { 0, 1, 2, 3, 5, 7, 8, 15, 16, 31, 32, 33, 64, 65, 97, 129, 255 };
                        byte pick = b[DtlsUtils.RndU32((uint)b.Length)];
                        L = pick;
                        lenElem.MutatedValue = new Variant((ulong)L);
                        out_buf = new byte[L];
                        switch (DtlsUtils.RndU32(4))
                        {
                            case 0: for (int i = 0; i < L; i++) out_buf[i] = 0x00; break;
                            case 1: for (int i = 0; i < L; i++) out_buf[i] = 0xFF; break;
                            case 2: for (int i = 0; i < L; i++) out_buf[i] = (byte)i; break;
                            default: for (int i = 0; i < L; i++) out_buf[i] = (byte)((i & 1) != 0 ? 0xAA : 0x55); break;
                        }
                        if (L >= 1 && DtlsUtils.RndU32(2) == 0) out_buf[0] = 0x04;
                        DtlsUtils.MixShallowDeep(out_buf, L);
                        blob.MutatedValue = new Variant(out_buf);
                    }
                    break;

                default:
                    if (L < 1) L = 33;
                    lenElem.MutatedValue = new Variant((ulong)L);
                    out_buf = new byte[L];
                    byte[] tags = { 0x02, 0x03, 0x04, 0x06, 0x07 };
                    out_buf[0] = tags[DtlsUtils.RndU32((uint)tags.Length)];
                    for (int i = 1; i < L; i++)
                    {
                        out_buf[i] = (byte)DtlsUtils.UrandCert(256);
                    }
                    DtlsUtils.MixShallowDeep(out_buf, L);
                    blob.MutatedValue = new Variant(out_buf);
                    break;
            }
        }

        private void MutatePskIdentity(Number lenElem, Blob blob)
        {
            ushort curLen = (ushort)(ulong)lenElem.InternalValue;
            if (curLen > DtlsUtils.DTLS_MAX_PSK_IDENTITY_LEN) curLen = (ushort)DtlsUtils.DTLS_MAX_PSK_IDENTITY_LEN;

            byte[] cur = blob.Bytes();
            if (cur == null || cur.Length != curLen) cur = new byte[curLen];

            uint cat = DtlsUtils.RndU32(8);
            ushort L = curLen;
            byte[] out_buf;

            switch (cat)
            {
                case 0: // A. Canonical form
                    if (L == 0) { L = 8; lenElem.MutatedValue = new Variant((ulong)L); }
                    out_buf = new byte[L];
                    for (int i = 0; i < L; i++)
                    {
                        byte c = (byte)('a' + DtlsUtils.RndU32(26));
                        if (DtlsUtils.RndU32(2) == 0) c = (byte)('0' + DtlsUtils.RndU32(10));
                        out_buf[i] = c;
                    }
                    DtlsUtils.MixShallowDeep(out_buf, L);
                    blob.MutatedValue = new Variant(out_buf);
                    break;

                case 1: // B. Boundaries
                    {
                        ushort[] b = { 0, 1, 2, 3, 4, 7, 8, 15, 16, 31, 32, 63, 64, 127, 128, 255, 256 };
                        ushort pick = b[DtlsUtils.RndU32((uint)b.Length)];
                        if (pick > DtlsUtils.DTLS_MAX_PSK_IDENTITY_LEN) pick = (ushort)DtlsUtils.DTLS_MAX_PSK_IDENTITY_LEN;
                        L = pick;
                        lenElem.MutatedValue = new Variant((ulong)L);
                        out_buf = new byte[L];
                        switch (DtlsUtils.RndU32(4))
                        {
                            case 0: for (int i = 0; i < L; i++) out_buf[i] = (byte)'A'; break;
                            case 1: for (int i = 0; i < L; i++) out_buf[i] = (byte)'0'; break;
                            case 2: for (int i = 0; i < L; i++) out_buf[i] = (byte)('a' + (i % 26)); break;
                            default: for (int i = 0; i < L; i++) out_buf[i] = (byte)((i & 1) != 0 ? '_' : '-'); break;
                        }
                        DtlsUtils.MixShallowDeep(out_buf, L);
                        blob.MutatedValue = new Variant(out_buf);
                    }
                    break;

                default:
                    if (L == 0) L = 8;
                    lenElem.MutatedValue = new Variant((ulong)L);
                    out_buf = new byte[L];
                    DtlsUtils.FillRand(out_buf, L);
                    DtlsUtils.MixShallowDeep(out_buf, L);
                    blob.MutatedValue = new Variant(out_buf);
                    break;
            }
        }

        private void MutateEncryptedPms(Number lenElem, Blob blob)
        {
            ushort curLen = (ushort)(ulong)lenElem.InternalValue;
            if (curLen > DtlsUtils.DTLS_MAX_RSA_ENC_PMS_LEN) curLen = (ushort)DtlsUtils.DTLS_MAX_RSA_ENC_PMS_LEN;

            byte[] cur = blob.Bytes();
            if (cur == null || cur.Length != curLen) cur = new byte[curLen];

            uint cat = DtlsUtils.RndU32(8);
            ushort L = curLen;
            byte[] out_buf;

            switch (cat)
            {
                case 0: // A. Canonical form
                    if (L == 0) { L = 128; lenElem.MutatedValue = new Variant((ulong)L); }
                    out_buf = new byte[L];
                    DtlsUtils.FillRand(out_buf, L);
                    DtlsUtils.MixShallowDeep(out_buf, L);
                    blob.MutatedValue = new Variant(out_buf);
                    break;

                case 1: // B. Boundaries
                    {
                        ushort[] b = { 0, 1, 2, 3, 7, 8, 15, 16, 31, 32, 47, 48, 63, 64,
                            95, 96, 127, 128, 191, 192, 255, 256,
                            (ushort)(DtlsUtils.DTLS_MAX_RSA_ENC_PMS_LEN - 1), (ushort)DtlsUtils.DTLS_MAX_RSA_ENC_PMS_LEN };
                        ushort pick = b[DtlsUtils.RndU32((uint)b.Length)];
                        if (pick > DtlsUtils.DTLS_MAX_RSA_ENC_PMS_LEN) pick = (ushort)DtlsUtils.DTLS_MAX_RSA_ENC_PMS_LEN;
                        L = pick;
                        lenElem.MutatedValue = new Variant((ulong)L);
                        out_buf = new byte[L];
                        switch (DtlsUtils.RndU32(4))
                        {
                            case 0: for (int i = 0; i < L; i++) out_buf[i] = 0x00; break;
                            case 1: for (int i = 0; i < L; i++) out_buf[i] = 0xFF; break;
                            case 2: for (int i = 0; i < L; i++) out_buf[i] = (byte)i; break;
                            default: for (int i = 0; i < L; i++) out_buf[i] = (byte)((i & 1) != 0 ? 0xAA : 0x55); break;
                        }
                        DtlsUtils.MixShallowDeep(out_buf, L);
                        blob.MutatedValue = new Variant(out_buf);
                    }
                    break;

                default:
                    if (L == 0) L = 128;
                    lenElem.MutatedValue = new Variant((ulong)L);
                    out_buf = new byte[L];
                    DtlsUtils.FillRand(out_buf, L);
                    DtlsUtils.MixShallowDeep(out_buf, L);
                    blob.MutatedValue = new Variant(out_buf);
                    break;
            }
        }
    }
}