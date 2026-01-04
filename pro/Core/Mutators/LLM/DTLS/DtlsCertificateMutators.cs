using System;
using System.IO;
using System.Linq;
using Peach.Core.Dom;
using Peach.Core;
using Peach.Pro.Core.Mutators;
using Peach.Pro.Core.Mutators.LLM.DTLS;
using System.ComponentModel;

using Array = System.Array;

namespace Peach.Pro.Core.Mutators.LLM.DTLS
{
    [Mutator("DtlsMutateCertificateCertBlob")]
    [CMutator("mutate_certificate_cert_blob")]
    [Description("Mutates Certificate Blob.")]
    public class DtlsMutateCertificateCertBlob : LLMMutator
    {
        public DtlsMutateCertificateCertBlob(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj is Blob && obj.Name == "cert_blob";
        }

        public override int count => 1;
        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }
        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            var blob = obj as Blob;
            byte[] blob_bytes = blob.Bytes();
            uint len = (uint)blob_bytes.Length;

            // Get parent to update cert_blob_len
            var parent = obj.parent;
            Number lenElem = null;
            if (parent != null)
            {
                lenElem = parent.find("cert_blob_len") as Number;
            }

            // pick semantic category A..H
            uint cat = DtlsUtils.UrandCert(8);

            switch (cat)
            {
                case 0: // A. Canonical form
                    // keep as-is; if empty, synthesize a minimal DER-like prefix
                    if (len == 0)
                    {
                        uint target = 8;
                        if (target > DtlsUtils.DTLS_MAX_CERT_BLOB_LEN) target = DtlsUtils.DTLS_MAX_CERT_BLOB_LEN;
                        len = target;
                        blob_bytes = new byte[target];
                        // 0x30 SEQUENCE, length, then some bytes
                        blob_bytes[0] = 0x30;
                        blob_bytes[1] = (byte)(target - 2);
                        for (uint k = 2; k < target; k++) blob_bytes[k] = (byte)(0xA0u + k);
                    }
                    break;

                case 1: // B. Boundaries
                    // 0, 1, small, max-1, max
                    switch (DtlsUtils.UrandCert(5))
                    {
                        case 0:
                            len = 0;
                            blob_bytes = new byte[0];
                            break;
                        case 1:
                            len = 1;
                            blob_bytes = new byte[1];
                            blob_bytes[0] = (byte)DtlsUtils.UrandCert(256);
                            break;
                        case 2:
                            {
                                uint target = 32;
                                if (target > DtlsUtils.DTLS_MAX_CERT_BLOB_LEN) target = DtlsUtils.DTLS_MAX_CERT_BLOB_LEN;
                                len = target;
                                blob_bytes = new byte[target];
                                DtlsUtils.FillRand(blob_bytes, (int)target);
                            }
                            break;
                        case 3:
                            {
                                uint target = (DtlsUtils.DTLS_MAX_CERT_BLOB_LEN > 0) ? (DtlsUtils.DTLS_MAX_CERT_BLOB_LEN - 1u) : 0u;
                                len = target;
                                blob_bytes = new byte[target];
                                DtlsUtils.FillRand(blob_bytes, (int)target);
                            }
                            break;
                        default:
                            len = DtlsUtils.DTLS_MAX_CERT_BLOB_LEN;
                            blob_bytes = new byte[len];
                            DtlsUtils.FillRand(blob_bytes, (int)len);
                            break;
                    }
                    break;

                case 2: // C. Equivalence-class alternatives
                    // classes: all-zero, all-0xFF, ASCII PEM-like, DER-like header + random
                    {
                        uint target;
                        switch (DtlsUtils.UrandCert(4))
                        {
                            case 0: target = 64; break;
                            case 1: target = 128; break;
                            case 2: target = 256; break;
                            default: target = 96; break;
                        }
                        if (target > DtlsUtils.DTLS_MAX_CERT_BLOB_LEN) target = DtlsUtils.DTLS_MAX_CERT_BLOB_LEN;
                        len = target;
                        blob_bytes = new byte[target];

                        switch (DtlsUtils.UrandCert(4))
                        {
                            case 0:
                                for (int i = 0; i < target; i++) blob_bytes[i] = 0x00;
                                break;
                            case 1:
                                for (int i = 0; i < target; i++) blob_bytes[i] = 0xFF;
                                break;
                            case 2:
                                {
                                    // "-----BEGIN" style (not a full PEM, just class)
                                    string hdr = "-----BEGIN CERT-----\n";
                                    int hlen = hdr.Length;
                                    int off = 0;
                                    while (off < target)
                                    {
                                        int take = (hlen <= (target - off)) ? hlen : (int)(target - off);
                                        System.Text.Encoding.ASCII.GetBytes(hdr, 0, take, blob_bytes, off);
                                        off += take;
                                    }
                                }
                                break;
                            default:
                                // DER-ish: 0x30 len ...
                                DtlsUtils.FillRand(blob_bytes, (int)target);
                                if (target >= 2)
                                {
                                    blob_bytes[0] = 0x30;
                                    blob_bytes[1] = (byte)(target - 2);
                                }
                                break;
                        }
                    }
                    break;

                case 3: // D. Allowed bitfield/enum/range
                    // legal range: 0..DTLS_MAX_CERT_BLOB_LEN
                    {
                        uint target = DtlsUtils.UrandCert(DtlsUtils.DTLS_MAX_CERT_BLOB_LEN + 1u);
                        // bias toward typical sizes
                        if (DtlsUtils.UrandCert(100) < 60)
                        {
                            target = 256u + DtlsUtils.UrandCert(1024u); // 256..1279
                            if (target > DtlsUtils.DTLS_MAX_CERT_BLOB_LEN) target = DtlsUtils.DTLS_MAX_CERT_BLOB_LEN;
                        }
                        len = target;
                        blob_bytes = new byte[target];
                        if (len > 0) DtlsUtils.FillRand(blob_bytes, (int)len);
                    }
                    break;

                case 4: // E. Encoding-shape variant
                    // keep length, transform bytes to alter "shape"
                    if (len == 0)
                    {
                        uint target = 64;
                        if (target > DtlsUtils.DTLS_MAX_CERT_BLOB_LEN) target = DtlsUtils.DTLS_MAX_CERT_BLOB_LEN;
                        len = target;
                        blob_bytes = new byte[target];
                        DtlsUtils.FillRand(blob_bytes, (int)target);
                    }
                    switch (DtlsUtils.UrandCert(4))
                    {
                        case 0:
                            DtlsUtils.MemRev(blob_bytes, (int)len);
                            break;
                        case 1:
                            DtlsUtils.MemXor(blob_bytes, (int)len, 0x5A);
                            break;
                        case 2:
                            DtlsUtils.MemRotl(blob_bytes, (int)len, 1 + (int)DtlsUtils.UrandCert(7));
                            break;
                        default:
                            // nibble swap
                            for (uint k = 0; k < len; k++)
                                blob_bytes[k] = (byte)((blob_bytes[k] << 4) | (blob_bytes[k] >> 4));
                            break;
                    }
                    break;

                case 5: // F. Padding/alignment
                    // pad/trim to 4 or 16 alignment; pad with last byte
                    {
                        uint target = len;
                        if (target == 0) target = 16;
                        uint align = (DtlsUtils.UrandCert(2) == 0) ? 4u : 16u;

                        if (DtlsUtils.UrandCert(2) == 0)
                        {
                            // round up
                            uint r = target % align;
                            if (r != 0) target += (align - r);
                            if (target > DtlsUtils.DTLS_MAX_CERT_BLOB_LEN) target = DtlsUtils.DTLS_MAX_CERT_BLOB_LEN;
                        }
                        else
                        {
                            // round down
                            target -= (target % align);
                            if (target == 0) target = align;
                        }

                        if (target > DtlsUtils.DTLS_MAX_CERT_BLOB_LEN) target = DtlsUtils.DTLS_MAX_CERT_BLOB_LEN;

                        byte[] new_blob = new byte[target];
                        if (target > len)
                        {
                            byte pad = (len > 0) ? blob_bytes[len - 1] : (byte)0x00;
                            Array.Copy(blob_bytes, new_blob, len);
                            for (uint k = len; k < target; k++) new_blob[k] = pad;
                        }
                        else if (target < len)
                        {
                            Array.Copy(blob_bytes, new_blob, target);
                        }
                        else
                        {
                            Array.Copy(blob_bytes, new_blob, len);
                            // same length: tweak pad marker
                            if (target > 0) new_blob[target - 1] ^= 0x01;
                        }

                        len = target;
                        blob_bytes = new_blob;
                    }
                    break;

                case 6: // G. In-range sweep
                    // deterministic-ish sweep over sizes
                    {
                        uint[] sweep = { 0u, 1u, 8u, 32u, 128u, 256u, 512u, 1024u, 2048u };
                        uint idx = DtlsUtils.UrandCert((uint)sweep.Length);
                        uint target = sweep[idx];
                        if (target > DtlsUtils.DTLS_MAX_CERT_BLOB_LEN) target = DtlsUtils.DTLS_MAX_CERT_BLOB_LEN;
                        len = target;
                        blob_bytes = new byte[target];
                        for (uint k = 0; k < len; k++) blob_bytes[k] = (byte)(k + (byte)(DtlsUtils.UrandCert(256) * 13u));
                        // keep a DER-ish start when length permits
                        if (len >= 2) { blob_bytes[0] = 0x30; blob_bytes[1] = (byte)(len - 2); }
                    }
                    break;

                case 7: // H. Random valid mix
                default:
                    {
                        uint r = DtlsUtils.UrandCert(100);
                        uint target;
                        if (r < 10) target = 0;
                        else if (r < 55) target = 128u + DtlsUtils.UrandCert(512u);      // 128..639
                        else if (r < 85) target = 512u + DtlsUtils.UrandCert(2048u);     // 512..2559
                        else target = DtlsUtils.DTLS_MAX_CERT_BLOB_LEN;

                        if (target > DtlsUtils.DTLS_MAX_CERT_BLOB_LEN) target = DtlsUtils.DTLS_MAX_CERT_BLOB_LEN;
                        len = target;
                        blob_bytes = new byte[target];
                        if (len > 0) DtlsUtils.FillRand(blob_bytes, (int)len);

                        // sometimes impose DER-like header
                        if (len >= 2 && DtlsUtils.UrandCert(100) < 50)
                        {
                            blob_bytes[0] = 0x30;
                            blob_bytes[1] = (byte)((len > 2) ? (len - 2) : 0);
                        }
                    }
                    break;
            }

            // Randomized perturbations: mix shallow and deep
            if (len > 0)
            {
                uint r = DtlsUtils.UrandCert(100);

                if (r < 18)
                {
                    // shallow: flip one bit in one byte
                    uint pos = DtlsUtils.UrandCert(len);
                    byte bit = (byte)(1u << (int)DtlsUtils.UrandCert(8));
                    blob_bytes[pos] ^= bit;
                }
                else if (r < 28)
                {
                    // shallow: swap two bytes
                    if (len > 1)
                    {
                        uint a = DtlsUtils.UrandCert(len);
                        uint b = DtlsUtils.UrandCert(len);
                        byte t = blob_bytes[a];
                        blob_bytes[a] = blob_bytes[b];
                        blob_bytes[b] = t;
                    }
                }
                else if (r < 36)
                {
                    // deep: regenerate content with same length
                    DtlsUtils.FillRand(blob_bytes, (int)len);
                }
                else if (r < 44)
                {
                    // deep: change length to another valid value and fill
                    uint target = DtlsUtils.UrandCert(DtlsUtils.DTLS_MAX_CERT_BLOB_LEN + 1u);
                    len = target;
                    blob_bytes = new byte[target];
                    if (len > 0) DtlsUtils.FillRand(blob_bytes, (int)len);
                }
                else if (r < 50)
                {
                    // deep: force a consistent-looking prefix (DER-ish)
                    if (len < 16)
                    {
                        uint target = 64;
                        if (target > DtlsUtils.DTLS_MAX_CERT_BLOB_LEN) target = DtlsUtils.DTLS_MAX_CERT_BLOB_LEN;
                        len = target;
                        blob_bytes = new byte[target];
                        DtlsUtils.FillRand(blob_bytes, (int)len);
                    }
                    if (len >= 2) { blob_bytes[0] = 0x30; blob_bytes[1] = (byte)((len > 2) ? (len - 2) : 0); }
                }
            }

            // Update cert_blob_len if parent exists
            if (lenElem != null)
            {
                lenElem.MutatedValue = new Variant((ulong)len);
            }

            blob.MutatedValue = new Variant(blob_bytes);
        }
    }

    // Stub classes for Add/Delete/Repeat types (C logic was empty, but classes requested)
    [Mutator("DtlsAddCertificateRequestCertTypes")]
    [CMutator("add_certificate_request_cert_types")]
    [Description("Add Cert Types (Stub).")]
    public class DtlsAddCertificateRequestCertTypes : LLMMutator
    {
        public DtlsAddCertificateRequestCertTypes(DataElement obj) : base(obj) { }
        public new static bool supportedDataElement(DataElement obj) => false;
        public override int count => 0;
        public override void sequentialMutation(DataElement obj) { }
        public override void randomMutation(DataElement obj) { }
    }

    [Mutator("DtlsDeleteCertificateRequestCertTypes")]
    [CMutator("delete_certificate_request_cert_types")]
    [Description("Delete Cert Types (Stub).")]
    public class DtlsDeleteCertificateRequestCertTypes : LLMMutator
    {
        public DtlsDeleteCertificateRequestCertTypes(DataElement obj) : base(obj) { }
        public new static bool supportedDataElement(DataElement obj) => false;
        public override int count => 0;
        public override void sequentialMutation(DataElement obj) { }
        public override void randomMutation(DataElement obj) { }
    }

    [Mutator("DtlsRepeatCertificateRequestCertTypes")]
    [CMutator("repeat_certificate_request_cert_types")]
    [Description("Repeat Cert Types (Stub).")]
    public class DtlsRepeatCertificateRequestCertTypes : LLMMutator
    {
        public DtlsRepeatCertificateRequestCertTypes(DataElement obj) : base(obj) { }
        public new static bool supportedDataElement(DataElement obj) => false;
        public override int count => 0;
        public override void sequentialMutation(DataElement obj) { }
        public override void randomMutation(DataElement obj) { }
    }

    [Mutator("DtlsMutateCertificateRequestCertTypes")]
    [CMutator("mutate_certificate_request_cert_types")]
    [Description("Mutates CertificateRequest Cert Types.")]
    public class DtlsMutateCertificateRequestCertTypes : LLMMutator
    {
        public DtlsMutateCertificateRequestCertTypes(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj is Blob && obj.Name == "cert_types" && obj.IsIn("certificate_request");
        }

        public override int count => 1;
        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }
        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            var blob = obj as Blob;
            byte[] val = blob.Bytes();
            if (val.Length > 0) NextBytes(val);
            blob.MutatedValue = new Variant(val);
        }
    }

    [Mutator("DtlsAddCertificateRequestCaDnBlob")]
    [CMutator("add_certificate_request_ca_dn_blob")]
    [Description("Add CA DN Blob.")]
    public class DtlsAddCertificateRequestCaDnBlob : LLMMutator
    {
        public DtlsAddCertificateRequestCaDnBlob(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj is Blob && obj.Name == "ca_dn_blob" && obj.IsIn("certificate_request");
        }

        public override int count => 1;
        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }
        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            var blob = obj as Blob;
            if (blob.Value.Length == 0)
                blob.MutatedValue = new Variant(new byte[] { 0x30, 0x00 }); // Empty SEQ
        }
    }

    [Mutator("DtlsDeleteCertificateRequestCaDnBlob")]
    [CMutator("delete_certificate_request_ca_dn_blob")]
    [Description("Delete CA DN Blob.")]
    public class DtlsDeleteCertificateRequestCaDnBlob : LLMMutator
    {
        public DtlsDeleteCertificateRequestCaDnBlob(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj is Blob && obj.Name == "ca_dn_blob" && obj.IsIn("certificate_request");
        }

        public override int count => 1;
        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }
        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            var blob = obj as Blob;
            blob.MutatedValue = new Variant(new byte[0]);
        }
    }

    [Mutator("DtlsRepeatCertificateRequestCaDnBlob")]
    [CMutator("repeat_certificate_request_ca_dn_blob")]
    [Description("Repeats CA DN Blob.")]
    public class DtlsRepeatCertificateRequestCaDnBlob : LLMMutator
    {
        public DtlsRepeatCertificateRequestCaDnBlob(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj is Blob && obj.Name == "ca_dn_blob" && obj.IsIn("certificate_request");
        }

        public override int count => 1;
        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }
        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            var blob = obj as Blob;
            byte[] cur = blob.Bytes();
            if (cur.Length > 0)
            {
                byte[] newVal = new byte[cur.Length * 2];
                cur.CopyTo(newVal, 0);
                cur.CopyTo(newVal, cur.Length);
                blob.MutatedValue = new Variant(newVal);
            }
        }
    }

    [Mutator("DtlsMutateCertificateRequestCaDnBlob")]
    [CMutator("mutate_certificate_request_ca_dn_blob")]
    [Description("Mutates CA DN Blob.")]
    public class DtlsMutateCertificateRequestCaDnBlob : LLMMutator
    {
        public DtlsMutateCertificateRequestCaDnBlob(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj is Blob && obj.Name == "ca_dn_blob" && obj.IsIn("certificate_request");
        }

        public override int count => 1;
        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }
        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            var blob = obj as Blob;
            byte[] val = blob.Bytes();
            if (val.Length > 0) NextBytes(val);
            blob.MutatedValue = new Variant(val);
        }
    }

    [Mutator("DtlsMutateCertificateRequestSigAlgs")]
    [CMutator("mutate_certificate_request_sig_algs")]
    [Description("Mutates Sig Algs.")]
    public class DtlsMutateCertificateRequestSigAlgs : LLMMutator
    {
        public DtlsMutateCertificateRequestSigAlgs(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj is Blob && obj.Name == "sig_algs" && obj.IsIn("certificate_request");
        }

        public override int count => 1;
        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }
        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            var blob = obj as Blob;
            byte[] val = blob.Bytes();
            if (val.Length > 0) NextBytes(val);
            blob.MutatedValue = new Variant(val);
        }
    }

    [Mutator("DtlsMutateCertificateVerifyAlg")]
    [CMutator("mutate_certificate_verify_alg")]
    [Description("Mutates CertificateVerify Alg.")]
    public class DtlsMutateCertificateVerifyAlg : LLMMutator
    {
        public DtlsMutateCertificateVerifyAlg(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj is Block && obj.Name == "alg" && obj.IsIn("certificate_verify");
        }

        public override int count => 1;
        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }
        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            var blk = obj as Block;
            var hElem = blk.find("hash_algorithm") as Number;
            var sElem = blk.find("signature_algorithm") as Number;
            hElem.MutatedValue = new Variant((ulong)(1 + DtlsUtils.RndU32(6)));
            sElem.MutatedValue = new Variant((ulong)(1 + DtlsUtils.RndU32(3)));
        }
    }

    [Mutator("DtlsMutateCertificateVerifySignature")]
    [CMutator("mutate_certificate_verify_signature")]
    [Description("Mutates CertificateVerify Signature.")]
    public class DtlsMutateCertificateVerifySignature : LLMMutator
    {
        public DtlsMutateCertificateVerifySignature(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj is Blob && obj.Name == "signature" && obj.IsIn("certificate_verify");
        }

        public override int count => 1;
        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }
        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            var blob = obj as Blob;
            byte[] val = blob.Bytes();
            if (val.Length > 0) NextBytes(val);
            blob.MutatedValue = new Variant(val);
        }
    }
}