using System;
using System.ComponentModel;
using System.Text;
using Peach.Core;
using Peach.Core.Dom;

namespace Peach.LLM.Core.Mutators.RTSP
{
    // PAUSE message type 主要使用通用字段的 mutator
    // 这些 mutator 已经在 RtspCommonFieldMutators.cs 中定义
    // 通过 supportedDataElement 方法中的 IsIn("pause") 检查来支持 PAUSE message type
    
    // PAUSE 特有的字段：Range (与 PLAY 类似)
    // C Function: add_range (for PAUSE)
    [Mutator("AddPauseRange")]
    [CMutator("add_range")]
    [Description("Adds Range header for PAUSE request")]
    public class RtspAddPauseRange : LLMMutator
    {
        public RtspAddPauseRange(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("pause") && obj.Name == "range" && obj is Peach.Core.Dom.String;
        }

        public override int count => 1;

        public override uint mutation { get; set; }

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            obj.MutatedValue = new Variant("npt=0-");
        }
    }

    // C Function: delete_range (for PAUSE)
    [Mutator("DeletePauseRange")]
    [CMutator("delete_range")]
    [Description("Deletes Range header for PAUSE request")]
    public class RtspDeletePauseRange : LLMMutator
    {
        public RtspDeletePauseRange(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("pause") && obj.Name == "range" && obj is Peach.Core.Dom.String;
        }

        public override int count => 1;

        public override uint mutation { get; set; }

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            obj.MutatedValue = new Variant("");
        }
    }

    // C Function: repeat_range (for PAUSE)
    [Mutator("RepeatPauseRange")]
    [CMutator("repeat_range")]
    [Description("Repeats Range header for PAUSE request")]
    public class RtspRepeatPauseRange : LLMMutator
    {
        public RtspRepeatPauseRange(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("pause") && obj.Name == "range" && obj is Peach.Core.Dom.String;
        }

        public override int count => 1;

        public override uint mutation { get; set; }

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            obj.MutatedValue = new Variant("npt=0-, npt=10-20");
        }
    }

    // C Function: mutate_range (for PAUSE)
    [Mutator("MutatePauseRange")]
    [CMutator("mutate_range")]
    [Description("Mutates Range header for PAUSE request")]
    public class RtspMutatePauseRange : LLMMutator
    {
        public RtspMutatePauseRange(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("pause") && obj.Name == "range" && obj is Peach.Core.Dom.String;
        }

        public override int count => 1;

        public override uint mutation { get; set; }

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
}

