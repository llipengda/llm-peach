using System;
using System.Text;
using Peach.Core;
using Peach.Core.Dom;

namespace Peach.Pro.Core.Mutators.LLM.RTSP
{
    // C Function: add_transport
    [Mutator("AddTransport")]
    [CMutator("add_transport")]
    [Description("Adds Transport header")]
    public class RtspAddSetupTransport : LLMMutator
    {
        public RtspAddSetupTransport(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("setup") && obj.name == "transport";
        }

        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            obj.MutatedValue = new Variant("RTP/AVP;unicast;client_port=8000-8001;mode=play");
        }
    }

    // C Function: delete_transport
    [Mutator("DeleteTransport")]
    [CMutator("delete_transport")]
    [Description("Deletes Transport header (illegal for SETUP)")]
    public class RtspDeleteSetupTransport : LLMMutator
    {
        public RtspDeleteSetupTransport(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("setup") && obj.name == "transport";
        }

        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            obj.MutatedValue = new Variant("");
        }
    }

    // C Function: repeat_transport
    [Mutator("RepeatTransport")]
    [CMutator("repeat_transport")]
    [Description("Repeats Transport header")]
    public class RtspRepeatSetupTransport : LLMMutator
    {
        public RtspRepeatSetupTransport(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("setup") && obj.name == "transport";
        }

        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            obj.MutatedValue = new Variant("RTP/AVP;unicast;client_port=8000-8001, RTP/AVP;unicast;client_port=9000-9001");
        }
    }

    // C Function: mutate_transport
    [Mutator("MutateTransport")]
    [CMutator("mutate_transport")]
    [Description("Mutates Transport header with various valid and invalid forms")]
    public class RtspMutateSetupTransport : LLMMutator
    {
        public RtspMutateSetupTransport(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("setup") && obj.name == "transport";
        }

        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            var random = RtspUtils.GetRandom();
            string[] values = {
                "RTP/AVP;unicast;client_port=8000-8001;mode=play",
                "RTP/AVP;multicast;client_port=8000-8001;mode=play",
                "RTP/AVP/TCP;unicast;client_port=8000-8001;mode=play",
                "RTP/AVP;unicast;client_port=8000;mode=play",
                "RTP/AVP;unicast;client_port=8001-8000;mode=play",
                "RTP/AVP;unicast;clientport=8000-8001;mode=play",
                "RTP/AVP unicast;client_port=8000-8001;mode=play",
                "RTP/AVP;unicast client_port=8000-8001;mode=play"
            };
            obj.MutatedValue = new Variant(values[random.Next(values.Length)]);
        }
    }

    // Note: Additional mutators for Connection, Date, Via, Accept-Language, Authorization,
    // Bandwidth, Blocksize, Cache-Control, Conference, From, If-Modified-Since,
    // Proxy-Require, Referer, Require, User-Agent follow the same pattern.
    // They can be generated similarly to the ones in RtspOptionsMutators.cs
}

