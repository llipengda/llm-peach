using System;
using System.ComponentModel;
using System.Text;
using Peach.Core;
using Peach.Core.Dom;

namespace Peach.LLM.Core.Mutators.RTSP
{
    // C Function: add_range
    [Mutator("AddRange")]
    [CMutator("add_range")]
    [Description("Adds Range header")]
    public class RtspAddPlayRange : LLMMutator
    {
        public RtspAddPlayRange(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("play") && obj.Name == "range" && obj is Peach.Core.Dom.String;
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

    // C Function: delete_range
    [Mutator("DeleteRange")]
    [CMutator("delete_range")]
    [Description("Deletes Range header")]
    public class RtspDeletePlayRange : LLMMutator
    {
        public RtspDeletePlayRange(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("play") && obj.Name == "range" && obj is Peach.Core.Dom.String;
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

    // C Function: repeat_range
    [Mutator("RepeatRange")]
    [CMutator("repeat_range")]
    [Description("Repeats Range header")]
    public class RtspRepeatPlayRange : LLMMutator
    {
        public RtspRepeatPlayRange(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("play") && obj.Name == "range" && obj is Peach.Core.Dom.String;
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

    // C Function: mutate_range
    [Mutator("MutateRange")]
    [CMutator("mutate_range")]
    [Description("Mutates Range header with various valid and invalid forms")]
    public class RtspMutatePlayRange : LLMMutator
    {
        public RtspMutatePlayRange(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("play") && obj.Name == "range" && obj is Peach.Core.Dom.String;
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
                "smpte=10:20:30-11:21:31",
                "clock=19960213T143205Z-",
                "npt=now-",
                "npt=-10",
                "npt=abc-def", // invalid
                "npt=" // empty
            };
            obj.MutatedValue = new Variant(values[random.Next(values.Length)]);
        }
    }

    // C Function: add_scale
    [Mutator("AddScale")]
    [CMutator("add_scale")]
    [Description("Adds Scale header")]
    public class RtspAddPlayScale : LLMMutator
    {
        public RtspAddPlayScale(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("play") && (obj.Name == "scale" || obj.Name.Contains("Scale"));
        }

        public override int count => 1;

        public override uint mutation { get; set; }

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.Name == "scale" || obj.Name.Contains("Scale"))
            {
                obj.MutatedValue = new Variant("1.0");
            }
        }
    }

    // C Function: delete_scale
    [Mutator("DeleteScale")]
    [CMutator("delete_scale")]
    [Description("Deletes Scale header")]
    public class RtspDeletePlayScale : LLMMutator
    {
        public RtspDeletePlayScale(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("play") && (obj.Name == "scale" || obj.Name.Contains("Scale"));
        }

        public override int count => 1;

        public override uint mutation { get; set; }

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.Name == "scale" || obj.Name.Contains("Scale"))
            {
                obj.MutatedValue = new Variant("");
            }
        }
    }

    // C Function: repeat_scale
    [Mutator("RepeatScale")]
    [CMutator("repeat_scale")]
    [Description("Repeats Scale header")]
    public class RtspRepeatPlayScale : LLMMutator
    {
        public RtspRepeatPlayScale(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("play") && (obj.Name == "scale" || obj.Name.Contains("Scale"));
        }

        public override int count => 1;

        public override uint mutation { get; set; }

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.Name == "scale" || obj.Name.Contains("Scale"))
            {
                obj.MutatedValue = new Variant("1.0, 2.0");
            }
        }
    }

    // C Function: mutate_scale
    [Mutator("MutateScale")]
    [CMutator("mutate_scale")]
    [Description("Mutates Scale header")]
    public class RtspMutatePlayScale : LLMMutator
    {
        public RtspMutatePlayScale(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("play") && (obj.Name == "speed" || obj.Name.Contains("Speed"));
        }

        public override int count => 1;

        public override uint mutation { get; set; }

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.Name == "scale" || obj.Name.Contains("Scale"))
            {
                var random = RtspUtils.GetRandom();
                string[] values = { "1.0", "2.0", "0.5", "-1.0", "0.0", "999.9" };
                obj.MutatedValue = new Variant(values[random.Next(values.Length)]);
            }
        }
    }

    // C Function: add_speed
    [Mutator("AddSpeed")]
    [CMutator("add_speed")]
    [Description("Adds Speed header")]
    public class RtspAddPlaySpeed : LLMMutator
    {
        public RtspAddPlaySpeed(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("play") && (obj.Name == "speed" || obj.Name.Contains("Speed"));
        }

        public override int count => 1;

        public override uint mutation { get; set; }

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.Name == "speed" || obj.Name.Contains("Speed"))
            {
                obj.MutatedValue = new Variant("1.0");
            }
        }
    }

    // C Function: delete_speed
    [Mutator("DeleteSpeed")]
    [CMutator("delete_speed")]
    [Description("Deletes Speed header")]
    public class RtspDeletePlaySpeed : LLMMutator
    {
        public RtspDeletePlaySpeed(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("play") && (obj.Name == "speed" || obj.Name.Contains("Speed"));
        }

        public override int count => 1;

        public override uint mutation { get; set; }

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.Name == "speed" || obj.Name.Contains("Speed"))
            {
                obj.MutatedValue = new Variant("");
            }
        }
    }

    // C Function: repeat_speed
    [Mutator("RepeatSpeed")]
    [CMutator("repeat_speed")]
    [Description("Repeats Speed header")]
    public class RtspRepeatPlaySpeed : LLMMutator
    {
        public RtspRepeatPlaySpeed(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("play") && (obj.Name == "speed" || obj.Name.Contains("Speed"));
        }

        public override int count => 1;

        public override uint mutation { get; set; }

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.Name == "speed" || obj.Name.Contains("Speed"))
            {
                obj.MutatedValue = new Variant("1.0, 2.0");
            }
        }
    }

    // C Function: mutate_speed
    [Mutator("MutateSpeed")]
    [CMutator("mutate_speed")]
    [Description("Mutates Speed header")]
    public class RtspMutatePlaySpeed : LLMMutator
    {
        public RtspMutatePlaySpeed(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("play") && (obj.Name == "speed" || obj.Name.Contains("Speed"));
        }

        public override int count => 1;

        public override uint mutation { get; set; }

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.Name == "speed" || obj.Name.Contains("Speed"))
            {
                var random = RtspUtils.GetRandom();
                string[] values = { "1.0", "2.0", "0.5", "-1.0", "0.0", "999.9" };
                obj.MutatedValue = new Variant(values[random.Next(values.Length)]);
            }
        }
    }

    // Note: Additional mutators for Connection, Date, Via, Accept-Language, Authorization,
    // Bandwidth, Blocksize, From, Proxy-Require, Referer, Require, Session, User-Agent
    // follow the same pattern and can be generated similarly.
}

