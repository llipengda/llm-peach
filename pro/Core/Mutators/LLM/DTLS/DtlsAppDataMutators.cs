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
    [Mutator("DtlsAddApplicationDataData")]
    [CMutator("add_application_data_data")]
    [Description("Adds Application Data payload.")]
    public class DtlsAddApplicationDataData : LLMMutator
    {
        public DtlsAddApplicationDataData(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj is Blob && obj.Name == "app_data" && obj.IsIn("app_data_record");
        }

        public override int count => 1;
        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }
        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            var blob = obj as Blob;
            if (blob.Value.Length == 0)
            {
                byte[] val = new byte[16];
                NextBytes(val);
                blob.MutatedValue = new Variant(val);
            }
        }
    }

    [Mutator("DtlsDeleteApplicationDataData")]
    [CMutator("delete_application_data_data")]
    [Description("Deletes Application Data payload.")]
    public class DtlsDeleteApplicationDataData : LLMMutator
    {
        public DtlsDeleteApplicationDataData(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj is Blob && obj.Name == "app_data" && obj.IsIn("app_data_record");
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

    [Mutator("DtlsRepeatApplicationDataData")]
    [CMutator("repeat_application_data_data")]
    [Description("Repeats Application Data payload.")]
    public class DtlsRepeatApplicationDataData : LLMMutator
    {
        public DtlsRepeatApplicationDataData(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj is Blob && obj.Name == "app_data" && obj.IsIn("app_data_record");
        }

        public override int count => 1;
        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }
        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            var blob = obj as Blob;
            byte[] cur = blob.Bytes();
            if (cur.Length > 0 && cur.Length * 2 <= 2048)
            {
                byte[] val = new byte[cur.Length * 2];
                cur.CopyTo(val, 0);
                cur.CopyTo(val, cur.Length);
                blob.MutatedValue = new Variant(val);
            }
        }
    }

    [Mutator("DtlsMutateApplicationDataData")]
    [CMutator("mutate_application_data_data")]
    [Description("Mutates Application Data payload.")]
    public class DtlsMutateApplicationDataData : LLMMutator
    {
        public DtlsMutateApplicationDataData(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj is Blob && obj.Name == "app_data" && obj.IsIn("app_data_record");
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