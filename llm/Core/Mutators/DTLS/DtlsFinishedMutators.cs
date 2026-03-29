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
    [Mutator("DtlsMutateFinishedVerifyData")]
    [CMutator("mutate_finished_verify_data")]
    [Description("Mutates Finished verify_data.")]
    public class DtlsMutateFinishedVerifyData : LLMMutator
    {
        public DtlsMutateFinishedVerifyData(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj is Blob && obj.Name == "verify_data" && obj.IsIn("finished");
        }

        public override int count => 1;
        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }
        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        protected override void PerformMutation(DataElement obj)
        {
            var blob = obj as Blob;
            byte[] val = blob.Bytes();
            if(val.Length == 12) NextBytes(val);
            blob.MutatedValue = new Variant(val);
        }
    }
}