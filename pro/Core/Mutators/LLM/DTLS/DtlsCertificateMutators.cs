using System;
using System.IO;
using System.Linq;
using Peach.Core.Dom;
using Peach.Core;
using Peach.Pro.Core.Mutators;
using Peach.Pro.Core.Mutators.LLM.DTLS;
using System.ComponentModel;

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
            byte[] val = blob.Bytes();
            if (val.Length > 0) NextBytes(val);
            blob.MutatedValue = new Variant(val);
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
            if (blob.Value.Length == 0) blob.MutatedValue = new Variant(new byte[] { 0x30, 0x00 }); // Empty SEQ
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