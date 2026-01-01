using System;
using System.Text;
using Peach.Core;
using Peach.Core.Dom;

namespace Peach.Pro.Core.Mutators.LLM.RTSP
{
    // RECORD message type 主要使用通用字段的 mutator
    // 这些 mutator 已经在 RtspCommonFieldMutators.cs 和 RtspPlayMutators.cs 中定义
    // 通过 supportedDataElement 方法中的 IsIn("record") 检查来支持 RECORD message type
    
    // RECORD 特有的字段：Range, Scale (与 PLAY 类似)
    // C Function: add_range (for RECORD)
    [Mutator("AddRecordRange")]
    [CMutator("add_range")]
    [Description("Adds Range header for RECORD request")]
    public class RtspAddRecordRange : LLMMutator
    {
        public RtspAddRecordRange(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("record") && obj.name == "range";
        }

        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            obj.MutatedValue = new Variant("npt=0-");
        }
    }

    // C Function: delete_range (for RECORD)
    [Mutator("DeleteRecordRange")]
    [CMutator("delete_range")]
    [Description("Deletes Range header for RECORD request")]
    public class RtspDeleteRecordRange : LLMMutator
    {
        public RtspDeleteRecordRange(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("record") && obj.name == "range";
        }

        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            obj.MutatedValue = new Variant("");
        }
    }

    // C Function: repeat_range (for RECORD)
    [Mutator("RepeatRecordRange")]
    [CMutator("repeat_range")]
    [Description("Repeats Range header for RECORD request")]
    public class RtspRepeatRecordRange : LLMMutator
    {
        public RtspRepeatRecordRange(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("record") && obj.name == "range";
        }

        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            obj.MutatedValue = new Variant("npt=0-, npt=10-20");
        }
    }

    // C Function: mutate_range (for RECORD)
    [Mutator("MutateRecordRange")]
    [CMutator("mutate_range")]
    [Description("Mutates Range header for RECORD request")]
    public class RtspMutateRecordRange : LLMMutator
    {
        public RtspMutateRecordRange(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("record") && obj.name == "range";
        }

        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            var random = RtspUtils.GetRandom();
            string[] values = {
                "npt=0-",
                "npt=10-20",
                "npt=0-100.5",
                "smpte=0:0:0-",
                "smpte=10:20:30-11:21:31"
            };
            obj.MutatedValue = new Variant(values[random.Next(values.Length)]);
        }
    }

    // C Function: add_scale (for RECORD)
    [Mutator("AddRecordScale")]
    [CMutator("add_scale")]
    [Description("Adds Scale header for RECORD request")]
    public class RtspAddRecordScale : LLMMutator
    {
        public RtspAddRecordScale(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("record");
        }

        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.name == "scale" || obj.name.Contains("Scale"))
            {
                obj.MutatedValue = new Variant("1.0");
            }
        }
    }

    // C Function: delete_scale (for RECORD)
    [Mutator("DeleteRecordScale")]
    [CMutator("delete_scale")]
    [Description("Deletes Scale header for RECORD request")]
    public class RtspDeleteRecordScale : LLMMutator
    {
        public RtspDeleteRecordScale(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("record");
        }

        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.name == "scale" || obj.name.Contains("Scale"))
            {
                obj.MutatedValue = new Variant("");
            }
        }
    }

    // C Function: repeat_scale (for RECORD)
    [Mutator("RepeatRecordScale")]
    [CMutator("repeat_scale")]
    [Description("Repeats Scale header for RECORD request")]
    public class RtspRepeatRecordScale : LLMMutator
    {
        public RtspRepeatRecordScale(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("record");
        }

        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.name == "scale" || obj.name.Contains("Scale"))
            {
                obj.MutatedValue = new Variant("1.0, 2.0");
            }
        }
    }

    // C Function: mutate_scale (for RECORD)
    [Mutator("MutateRecordScale")]
    [CMutator("mutate_scale")]
    [Description("Mutates Scale header for RECORD request")]
    public class RtspMutateRecordScale : LLMMutator
    {
        public RtspMutateRecordScale(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("record");
        }

        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.name == "scale" || obj.name.Contains("Scale"))
            {
                var random = RtspUtils.GetRandom();
                string[] values = { "1.0", "2.0", "0.5", "-1.0", "0.0", "999.9" };
                obj.MutatedValue = new Variant(values[random.Next(values.Length)]);
            }
        }
    }
}

