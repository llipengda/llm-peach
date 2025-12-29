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
    [Mutator("DtlsMutateAlertLevel")]
    [CMutator("mutate_alert_level")]
    [Description("Mutates Alert Level.")]
    public class DtlsMutateAlertLevel : LLMMutator
    {
        public DtlsMutateAlertLevel(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj is Number && obj.Name == "level" && obj.IsIn("alert_record");
        }

        public override int count => 1;
        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }
        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            var num = obj as Number;
            byte val = (byte)((DtlsUtils.RndU32(2) == 0) ? 1 : 2);
            num.MutatedValue = new Variant((ulong)val);
        }
    }

    [Mutator("DtlsMutateAlertDescription")]
    [CMutator("mutate_alert_description")]
    [Description("Mutates Alert Description.")]
    public class DtlsMutateAlertDescription : LLMMutator
    {
        public DtlsMutateAlertDescription(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj is Number && obj.Name == "description" && obj.IsIn("alert_record");
        }

        public override int count => 1;
        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }
        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            var num = obj as Number;
            byte val = (byte)DtlsUtils.RndU32(256);
            num.MutatedValue = new Variant((ulong)val);
        }
    }
}