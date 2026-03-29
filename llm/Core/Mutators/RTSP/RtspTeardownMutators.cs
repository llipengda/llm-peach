using System;
using System.ComponentModel;
using System.Text;
using Peach.Core;
using Peach.Core.Dom;

namespace Peach.LLM.Core.Mutators.RTSP
{
    // ==================== Connection ====================
    
    // C Function: delete_connection
    [Mutator("DeleteTeardownConnection")]
    [CMutator("delete_connection")]
    [Description("Deletes Connection header for TEARDOWN")]
    public class RtspDeleteTeardownConnection : LLMMutator
    {
        public RtspDeleteTeardownConnection(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("teardown") &&
                   (obj.Name == "connection" || obj.Name.Contains("Connection"));
        }
        public override uint mutation { get; set; }
        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        protected override void PerformMutation(DataElement obj)
        {
            if (obj.Name == "connection" || obj.Name.Contains("Connection"))
            {
                obj.MutatedValue = new Variant("");
            }
        }
    }

    // C Function: repeat_connection
    [Mutator("RepeatTeardownConnection")]
    [CMutator("repeat_connection")]
    [Description("Repeats Connection header for TEARDOWN")]
    public class RtspRepeatTeardownConnection : LLMMutator
    {
        public RtspRepeatTeardownConnection(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("teardown") &&
                   (obj.Name == "connection" || obj.Name.Contains("Connection"));
        }

        public override uint mutation { get; set; }
        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        protected override void PerformMutation(DataElement obj)
        {
            if (obj.Name == "connection" || obj.Name.Contains("Connection"))
            {
                obj.MutatedValue = new Variant("keep-alive, close");
            }
        }
    }

    // C Function: mutate_connection
    [Mutator("MutateTeardownConnection")]
    [CMutator("mutate_connection")]
    [Description("Mutates Connection header for TEARDOWN")]
    public class RtspMutateTeardownConnection : LLMMutator
    {
        public RtspMutateTeardownConnection(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("teardown") &&
                   (obj.Name == "connection" || obj.Name.Contains("Connection"));
        }

        public override uint mutation { get; set; }
        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        protected override void PerformMutation(DataElement obj)
        {
            if (obj.Name == "connection" || obj.Name.Contains("Connection"))
            {
                var random = RtspUtils.GetRandom();
                string[] values = { "keep-alive", "close", "keep-alive, foo, bar" };
                obj.MutatedValue = new Variant(values[random.Next(values.Length)]);
            }
        }
    }

    // ==================== Date ====================
    
    // C Function: delete_date
    [Mutator("DeleteTeardownDate")]
    [CMutator("delete_date")]
    [Description("Deletes Date header for TEARDOWN")]
    public class RtspDeleteTeardownDate : LLMMutator
    {
        public RtspDeleteTeardownDate(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("teardown") &&
                   (obj.Name == "date" || obj.Name.Contains("Date"));
        }

        public override uint mutation { get; set; }
        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        protected override void PerformMutation(DataElement obj)
        {
            if (obj.Name == "date" || obj.Name.Contains("Date"))
            {
                obj.MutatedValue = new Variant("");
            }
        }
    }

    // C Function: repeat_date
    [Mutator("RepeatTeardownDate")]
    [CMutator("repeat_date")]
    [Description("Repeats Date header for TEARDOWN")]
    public class RtspRepeatTeardownDate : LLMMutator
    {
        public RtspRepeatTeardownDate(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("teardown") &&
                   (obj.Name == "date" || obj.Name.Contains("Date"));
        }

        public override uint mutation { get; set; }
        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        protected override void PerformMutation(DataElement obj)
        {
            if (obj.Name == "date" || obj.Name.Contains("Date"))
            {
                obj.MutatedValue = new Variant("Tue, 15 Nov 1994 08:12:31 GMT, Wed, 16 Nov 1994 09:13:32 GMT");
            }
        }
    }

    // C Function: mutate_date
    [Mutator("MutateTeardownDate")]
    [CMutator("mutate_date")]
    [Description("Mutates Date header for TEARDOWN")]
    public class RtspMutateTeardownDate : LLMMutator
    {
        public RtspMutateTeardownDate(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("teardown") &&
                   (obj.Name == "date" || obj.Name.Contains("Date"));
        }

        public override uint mutation { get; set; }
        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        protected override void PerformMutation(DataElement obj)
        {
            if (obj.Name == "date" || obj.Name.Contains("Date"))
            {
                var random = RtspUtils.GetRandom();
                string[] values = {
                    "Tue, 15 Nov 1994 08:12:31 GMT",
                    "Mon, 15 Nov 1994 08:12:31 GMT",
                    "Tue, 15 Foo 1994 08:12:31 GMT",
                    "Tue, 15 Nov 94 08:12:31 GMT",
                    "tue, 15 nov 1994 08:12:31 gmt"
                };
                obj.MutatedValue = new Variant(values[random.Next(values.Length)]);
            }
        }
    }

    // ==================== Via ====================
    
    // C Function: delete_via
    [Mutator("DeleteTeardownVia")]
    [CMutator("delete_via")]
    [Description("Deletes Via header for TEARDOWN")]
    public class RtspDeleteTeardownVia : LLMMutator
    {
        public RtspDeleteTeardownVia(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("teardown") &&
                   (obj.Name == "via" || obj.Name.Contains("Via"));
        }

        public override uint mutation { get; set; }
        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        protected override void PerformMutation(DataElement obj)
        {
            if (obj.Name == "via" || obj.Name.Contains("Via"))
            {
                obj.MutatedValue = new Variant("");
            }
        }
    }

    // C Function: repeat_via
    [Mutator("RepeatTeardownVia")]
    [CMutator("repeat_via")]
    [Description("Repeats Via header for TEARDOWN")]
    public class RtspRepeatTeardownVia : LLMMutator
    {
        public RtspRepeatTeardownVia(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("teardown") &&
                   (obj.Name == "via" || obj.Name.Contains("Via"));
        }

        public override uint mutation { get; set; }
        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        protected override void PerformMutation(DataElement obj)
        {
            if (obj.Name == "via" || obj.Name.Contains("Via"))
            {
                obj.MutatedValue = new Variant("RTSP/1.0 example.com, RTSP/1.0 proxy.com");
            }
        }
    }

    // C Function: mutate_via
    [Mutator("MutateTeardownVia")]
    [CMutator("mutate_via")]
    [Description("Mutates Via header for TEARDOWN")]
    public class RtspMutateTeardownVia : LLMMutator
    {
        public RtspMutateTeardownVia(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("teardown") &&
                   (obj.Name == "via" || obj.Name.Contains("Via"));
        }

        public override uint mutation { get; set; }
        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        protected override void PerformMutation(DataElement obj)
        {
            if (obj.Name == "via" || obj.Name.Contains("Via"))
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

    // ==================== Accept-Language ====================
    
    // C Function: delete_accept_language
    [Mutator("DeleteTeardownAcceptLanguage")]
    [CMutator("delete_accept_language")]
    [Description("Deletes Accept-Language header for TEARDOWN")]
    public class RtspDeleteTeardownAcceptLanguage : LLMMutator
    {
        public RtspDeleteTeardownAcceptLanguage(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("teardown") &&
                   (obj.Name == "accept_language" || obj.Name.Contains("language"));
        }

        public override uint mutation { get; set; }
        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        protected override void PerformMutation(DataElement obj)
        {
            if (obj.Name == "accept_language" || obj.Name.Contains("language"))
            {
                obj.MutatedValue = new Variant("");
            }
        }
    }

    // C Function: repeat_accept_language
    [Mutator("RepeatTeardownAcceptLanguage")]
    [CMutator("repeat_accept_language")]
    [Description("Repeats Accept-Language header for TEARDOWN")]
    public class RtspRepeatTeardownAcceptLanguage : LLMMutator
    {
        public RtspRepeatTeardownAcceptLanguage(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("teardown") &&
                   (obj.Name == "accept_language" || obj.Name.Contains("language"));
        }

        public override uint mutation { get; set; }
        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        protected override void PerformMutation(DataElement obj)
        {
            if (obj.Name == "accept_language" || obj.Name.Contains("language"))
            {
                obj.MutatedValue = new Variant("en-US, fr-FR, de-DE");
            }
        }
    }

    // C Function: mutate_accept_language
    [Mutator("MutateTeardownAcceptLanguage")]
    [CMutator("mutate_accept_language")]
    [Description("Mutates Accept-Language header for TEARDOWN")]
    public class RtspMutateTeardownAcceptLanguage : LLMMutator
    {
        public RtspMutateTeardownAcceptLanguage(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("teardown") &&
                   (obj.Name == "accept_language" || obj.Name.Contains("Accept-Language"));
        }

        public override uint mutation { get; set; }
        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        protected override void PerformMutation(DataElement obj)
        {
            if (obj.Name == "accept_language" || obj.Name.Contains("Accept-Language"))
            {
                var random = RtspUtils.GetRandom();
                int[] weights = { 100, 100, 100, 100, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                
                int opIdx = RtspUtils.WeightedPickIdx(weights, weights.Length, random);
                string mutatedValue = "";

                switch (opIdx)
                {
                    case 0: // al_valid_simple
                        mutatedValue = "en-US";
                        break;
                    case 1: // al_valid_multi_ordered
                        mutatedValue = "en;q=1.0, fr;q=0.7, de;q=0.3";
                        break;
                    case 2: // al_with_wildcard
                        mutatedValue = "*;q=0.1";
                        break;
                    case 3: // al_duplicate_tags
                        mutatedValue = "en-US, en-US, fr-FR";
                        break;
                    default:
                        mutatedValue = "en-US";
                        break;
                }

                obj.MutatedValue = new Variant(mutatedValue);
            }
        }
    }


    // ==================== Session ====================
    
    // C Function: delete_session
    [Mutator("DeleteTeardownSession")]
    [CMutator("delete_session")]
    [Description("Deletes Session header for TEARDOWN")]
    public class RtspDeleteTeardownSession : LLMMutator
    {
        public RtspDeleteTeardownSession(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            // 严格检查：必须是teardown上下文，名称必须是session，且必须是String类型
            return obj.IsIn("teardown") && 
                   obj.Name == "session" && 
                   obj is Peach.Core.Dom.String;
        }

        public override uint mutation { get; set; }
        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        protected override void PerformMutation(DataElement obj)
        {
            // 双重检查：确保是String类型且名称正确
            if (obj.Name == "session" && obj is Peach.Core.Dom.String)
            {
                obj.MutatedValue = new Variant("");
            }
        }
    }

    // C Function: repeat_session
    [Mutator("RepeatTeardownSession")]
    [CMutator("repeat_session")]
    [Description("Repeats Session header for TEARDOWN")]
    public class RtspRepeatTeardownSession : LLMMutator
    {
        public RtspRepeatTeardownSession(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            // 严格检查：必须是teardown上下文，名称必须是session，且必须是String类型
            return obj.IsIn("teardown") && 
                   obj.Name == "session" && 
                   obj is Peach.Core.Dom.String;
        }

        public override uint mutation { get; set; }
        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        protected override void PerformMutation(DataElement obj)
        {
            // 双重检查：确保是String类型且名称正确
            if (obj.Name == "session" && obj is Peach.Core.Dom.String)
            {
                obj.MutatedValue = new Variant("ABCDEF, 1234");
            }
        }
    }

    // C Function: mutate_session
    [Mutator("MutateTeardownSession")]
    [CMutator("mutate_session")]
    [Description("Mutates Session header for TEARDOWN")]
    public class RtspMutateTeardownSession : LLMMutator
    {
        public RtspMutateTeardownSession(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            // 严格检查：必须是teardown上下文，名称必须是session，且必须是String类型
            if (obj == null)
                return false;
            
            if (!obj.IsIn("teardown"))
                return false;
            
            if (obj.Name != "session")
                return false;
            
            if (!(obj is Peach.Core.Dom.String))
                return false;
            
            return true;
        }

        public override uint mutation { get; set; }
        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        protected override void PerformMutation(DataElement obj)
        {
            // 多重检查：确保类型和名称都正确
            if (obj == null)
                return;
            
            if (obj.Name != "session")
                return;
            
            if (!(obj is Peach.Core.Dom.String))
                return;
            
            // 确保在teardown上下文中
            if (!obj.IsIn("teardown"))
                return;

            var random = RtspUtils.GetRandom();
            int[] weights = { 100, 100, 0, 0, 100, 100, 0, 0, 0, 0, 0, 0 };
            
            int opIdx = RtspUtils.WeightedPickIdx(weights, weights.Length, random);
            string mutatedValue = "";

            switch (opIdx)
            {
                case 0: // ss_ok_id_timeout
                    mutatedValue = "12345678;timeout=60";
                    break;
                case 1: // ss_ok_id_no_timeout
                    mutatedValue = "12345678";
                    break;
                case 4: // ss_long_id
                    byte[] longId = new byte[200];
                    RtspUtils.MakeRepeatedChar(longId, 200, (byte)'A', 199);
                    mutatedValue = System.Text.Encoding.UTF8.GetString(longId).TrimEnd('\0') + ";timeout=60";
                    break;
                case 5: // ss_zero_timeout
                    mutatedValue = "12345678;timeout=0";
                    break;
                default:
                    mutatedValue = "12345678;timeout=60";
                    break;
            }

            obj.MutatedValue = new Variant(mutatedValue);
        }
    }

}

