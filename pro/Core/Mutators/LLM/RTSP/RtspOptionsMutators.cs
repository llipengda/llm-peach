using System;
using System.Text;
using Peach.Core;
using Peach.Core.Dom;

namespace Peach.Pro.Core.Mutators.LLM.RTSP
{
    // C Function: add_connection
    [Mutator("AddConnection")]
    [CMutator("add_connection")]
    [Description("Adds Connection header with keep-alive")]
    public class RtspAddOptionsConnection : LLMMutator
    {
        public RtspAddOptionsConnection(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("options");
        }

        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.name == "connection" || obj.name.Contains("Connection"))
            {
                obj.MutatedValue = new Variant("keep-alive");
            }
        }
    }

    // C Function: delete_connection
    [Mutator("DeleteConnection")]
    [CMutator("delete_connection")]
    [Description("Deletes Connection header")]
    public class RtspDeleteOptionsConnection : LLMMutator
    {
        public RtspDeleteOptionsConnection(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("options");
        }

        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.name == "connection" || obj.name.Contains("Connection"))
            {
                obj.MutatedValue = new Variant("");
            }
        }
    }

    // C Function: repeat_connection
    [Mutator("RepeatConnection")]
    [CMutator("repeat_connection")]
    [Description("Repeats Connection header")]
    public class RtspRepeatOptionsConnection : LLMMutator
    {
        public RtspRepeatOptionsConnection(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("options");
        }

        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.name == "connection" || obj.name.Contains("Connection"))
            {
                obj.MutatedValue = new Variant("keep-alive, close");
            }
        }
    }

    // C Function: mutate_connection
    [Mutator("MutateConnection")]
    [CMutator("mutate_connection")]
    [Description("Mutates Connection header")]
    public class RtspMutateOptionsConnection : LLMMutator
    {
        public RtspMutateOptionsConnection(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("options");
        }

        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.name == "connection" || obj.name.Contains("Connection"))
            {
                var random = RtspUtils.GetRandom();
                string[] values = { "keep-alive", "close", "keep-alive, foo, bar" };
                obj.MutatedValue = new Variant(values[random.Next(values.Length)]);
            }
        }
    }

    // C Function: add_date
    [Mutator("AddDate")]
    [CMutator("add_date")]
    [Description("Adds Date header")]
    public class RtspAddOptionsDate : LLMMutator
    {
        public RtspAddOptionsDate(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("options");
        }

        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.name == "date" || obj.name.Contains("Date"))
            {
                obj.MutatedValue = new Variant("Tue, 15 Nov 1994 08:12:31 GMT");
            }
        }
    }

    // C Function: delete_date
    [Mutator("DeleteDate")]
    [CMutator("delete_date")]
    [Description("Deletes Date header")]
    public class RtspDeleteOptionsDate : LLMMutator
    {
        public RtspDeleteOptionsDate(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("options");
        }

        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.name == "date" || obj.name.Contains("Date"))
            {
                obj.MutatedValue = new Variant("");
            }
        }
    }

    // C Function: repeat_date
    [Mutator("RepeatDate")]
    [CMutator("repeat_date")]
    [Description("Repeats Date header")]
    public class RtspRepeatOptionsDate : LLMMutator
    {
        public RtspRepeatOptionsDate(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("options");
        }

        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.name == "date" || obj.name.Contains("Date"))
            {
                obj.MutatedValue = new Variant("Tue, 15 Nov 1994 08:12:31 GMT, Wed, 16 Nov 1994 09:13:32 GMT");
            }
        }
    }

    // C Function: mutate_date
    [Mutator("MutateDate")]
    [CMutator("mutate_date")]
    [Description("Mutates Date header")]
    public class RtspMutateOptionsDate : LLMMutator
    {
        public RtspMutateOptionsDate(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("options");
        }

        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.name == "date" || obj.name.Contains("Date"))
            {
                var random = RtspUtils.GetRandom();
                string[] values = {
                    "Tue, 15 Nov 1994 08:12:31 GMT",
                    "Mon, 15 Nov 1994 08:12:31 GMT", // wrong weekday
                    "Tue, 15 Foo 1994 08:12:31 GMT", // bad month
                    "Tue, 15 Nov 94 08:12:31 GMT", // 2-digit year
                    "tue, 15 nov 1994 08:12:31 gmt" // lowercase
                };
                obj.MutatedValue = new Variant(values[random.Next(values.Length)]);
            }
        }
    }

    // C Function: add_via
    [Mutator("AddVia")]
    [CMutator("add_via")]
    [Description("Adds Via header")]
    public class RtspAddOptionsVia : LLMMutator
    {
        public RtspAddOptionsVia(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("options");
        }

        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.name == "via" || obj.name.Contains("Via"))
            {
                obj.MutatedValue = new Variant("RTSP/1.0 example.com");
            }
        }
    }

    // C Function: delete_via
    [Mutator("DeleteVia")]
    [CMutator("delete_via")]
    [Description("Deletes Via header")]
    public class RtspDeleteOptionsVia : LLMMutator
    {
        public RtspDeleteOptionsVia(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("options");
        }

        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.name == "via" || obj.name.Contains("Via"))
            {
                obj.MutatedValue = new Variant("");
            }
        }
    }

    // C Function: repeat_via
    [Mutator("RepeatVia")]
    [CMutator("repeat_via")]
    [Description("Repeats Via header")]
    public class RtspRepeatOptionsVia : LLMMutator
    {
        public RtspRepeatOptionsVia(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("options");
        }

        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.name == "via" || obj.name.Contains("Via"))
            {
                obj.MutatedValue = new Variant("RTSP/1.0 example.com, RTSP/1.0 proxy.com");
            }
        }
    }

    // C Function: mutate_via
    [Mutator("MutateVia")]
    [CMutator("mutate_via")]
    [Description("Mutates Via header")]
    public class RtspMutateOptionsVia : LLMMutator
    {
        public RtspMutateOptionsVia(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("options");
        }

        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.name == "via" || obj.name.Contains("Via"))
            {
                var random = RtspUtils.GetRandom();
                string[] values = {
                    "RTSP/1.0 example.com",
                    "RTSP/1.0 example.com:554",
                    "RTSP/1.0 [2001:db8::1]",
                    "RTSP/1.0 example.com (comment)"
                };
                obj.MutatedValue = new Variant(values[random.Next(values.Length)]);
            }
        }
    }

    // Note: Additional mutators for Accept-Language, Authorization, Bandwidth, From, 
    // Proxy-Require, Referer, Require, User-Agent follow the same pattern.
    // They are omitted here for brevity but should be generated similarly.
}

