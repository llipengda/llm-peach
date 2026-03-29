using System;
using System.ComponentModel;
using System.Text;
using Peach.Core;
using Peach.Core.Dom;

namespace Peach.LLM.Core.Mutators.RTSP
{
    // ==================== Connection ====================
    
    // C Function: delete_connection
    [Mutator("DeleteRedirectConnection")]
    [CMutator("delete_connection")]
    [Description("Deletes Connection header for REDIRECT")]
    public class RtspDeleteRedirectConnection : LLMMutator
    {
        public RtspDeleteRedirectConnection(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("redirect") &&
                   (obj.Name == "connection" || obj.Name.Contains("Connection"));
        }

        public override int count => 1;

        public override uint mutation { get; set; }

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
    [Mutator("RepeatRedirectConnection")]
    [CMutator("repeat_connection")]
    [Description("Repeats Connection header for REDIRECT")]
    public class RtspRepeatRedirectConnection : LLMMutator
    {
        public RtspRepeatRedirectConnection(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("redirect") &&
                   (obj.Name == "connection" || obj.Name.Contains("Connection"));
        }

        public override int count => 1;

        public override uint mutation { get; set; }

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
    [Mutator("MutateRedirectConnection")]
    [CMutator("mutate_connection")]
    [Description("Mutates Connection header for REDIRECT")]
    public class RtspMutateRedirectConnection : LLMMutator
    {
        public RtspMutateRedirectConnection(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("redirect") &&
                   (obj.Name == "connection" || obj.Name.Contains("Connection"));
        }

        public override int count => 1;

        public override uint mutation { get; set; }

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
    [Mutator("DeleteRedirectDate")]
    [CMutator("delete_date")]
    [Description("Deletes Date header for REDIRECT")]
    public class RtspDeleteRedirectDate : LLMMutator
    {
        public RtspDeleteRedirectDate(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("redirect") &&
                   (obj.Name == "date" || obj.Name.Contains("Date"));
        }

        public override int count => 1;

        public override uint mutation { get; set; }

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
    [Mutator("RepeatRedirectDate")]
    [CMutator("repeat_date")]
    [Description("Repeats Date header for REDIRECT")]
    public class RtspRepeatRedirectDate : LLMMutator
    {
        public RtspRepeatRedirectDate(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("redirect") &&
                   (obj.Name == "date" || obj.Name.Contains("Date"));
        }

        public override int count => 1;

        public override uint mutation { get; set; }

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
    [Mutator("MutateRedirectDate")]
    [CMutator("mutate_date")]
    [Description("Mutates Date header for REDIRECT")]
    public class RtspMutateRedirectDate : LLMMutator
    {
        public RtspMutateRedirectDate(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("redirect") &&
                   (obj.Name == "date" || obj.Name.Contains("Date"));
        }

        public override int count => 1;

        public override uint mutation { get; set; }

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
    [Mutator("DeleteRedirectVia")]
    [CMutator("delete_via")]
    [Description("Deletes Via header for REDIRECT")]
    public class RtspDeleteRedirectVia : LLMMutator
    {
        public RtspDeleteRedirectVia(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("redirect") &&
                   (obj.Name == "via" || obj.Name.Contains("Via"));
        }

        public override int count => 1;

        public override uint mutation { get; set; }

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
    [Mutator("RepeatRedirectVia")]
    [CMutator("repeat_via")]
    [Description("Repeats Via header for REDIRECT")]
    public class RtspRepeatRedirectVia : LLMMutator
    {
        public RtspRepeatRedirectVia(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("redirect") &&
                   (obj.Name == "via" || obj.Name.Contains("Via"));
        }

        public override int count => 1;

        public override uint mutation { get; set; }

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
    [Mutator("MutateRedirectVia")]
    [CMutator("mutate_via")]
    [Description("Mutates Via header for REDIRECT")]
    public class RtspMutateRedirectVia : LLMMutator
    {
        public RtspMutateRedirectVia(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("redirect") &&
                   (obj.Name == "via" || obj.Name.Contains("Via"));
        }

        public override int count => 1;

        public override uint mutation { get; set; }

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
    [Mutator("DeleteRedirectAcceptLanguage")]
    [CMutator("delete_accept_language")]
    [Description("Deletes Accept-Language header for REDIRECT")]
    public class RtspDeleteRedirectAcceptLanguage : LLMMutator
    {
        public RtspDeleteRedirectAcceptLanguage(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("redirect") &&
                   (obj.Name == "accept_language" || obj.Name.Contains("language"));
        }

        public override int count => 1;

        public override uint mutation { get; set; }

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
    [Mutator("RepeatRedirectAcceptLanguage")]
    [CMutator("repeat_accept_language")]
    [Description("Repeats Accept-Language header for REDIRECT")]
    public class RtspRepeatRedirectAcceptLanguage : LLMMutator
    {
        public RtspRepeatRedirectAcceptLanguage(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("redirect") &&
                   (obj.Name == "accept_language" || obj.Name.Contains("language"));
        }

        public override int count => 1;

        public override uint mutation { get; set; }

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
    [Mutator("MutateRedirectAcceptLanguage")]
    [CMutator("mutate_accept_language")]
    [Description("Mutates Accept-Language header for REDIRECT")]
    public class RtspMutateRedirectAcceptLanguage : LLMMutator
    {
        public RtspMutateRedirectAcceptLanguage(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("redirect") &&
                   (obj.Name == "accept_language" || obj.Name.Contains("language"));
        }

        public override int count => 1;

        public override uint mutation { get; set; }

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        protected override void PerformMutation(DataElement obj)
        {
            if (obj.Name == "accept_language" || obj.Name.Contains("language"))
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

    // ==================== Authorization ====================
    
    // C Function: delete_authorization
    [Mutator("DeleteRedirectAuthorization")]
    [CMutator("delete_authorization")]
    [Description("Deletes Authorization header for REDIRECT")]
    public class RtspDeleteRedirectAuthorization : LLMMutator
    {
        public RtspDeleteRedirectAuthorization(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("redirect") &&
                   (obj.Name == "authorization" || obj.Name.Contains("Authorization"));
        }

        public override int count => 1;

        public override uint mutation { get; set; }

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        protected override void PerformMutation(DataElement obj)
        {
            if (obj.Name == "authorization" || obj.Name.Contains("Authorization"))
            {
                obj.MutatedValue = new Variant("");
            }
        }
    }

    // C Function: repeat_authorization
    [Mutator("RepeatRedirectAuthorization")]
    [CMutator("repeat_authorization")]
    [Description("Repeats Authorization header for REDIRECT")]
    public class RtspRepeatRedirectAuthorization : LLMMutator
    {
        public RtspRepeatRedirectAuthorization(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("redirect") &&
                   (obj.Name == "authorization" || obj.Name.Contains("Authorization"));
        }

        public override int count => 1;

        public override uint mutation { get; set; }

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        protected override void PerformMutation(DataElement obj)
        {
            if (obj.Name == "authorization" || obj.Name.Contains("Authorization"))
            {
                obj.MutatedValue = new Variant("Basic dXNlcjpwYXNz, Basic Zm9vOmJhcg==, Basic Og==");
            }
        }
    }

    // C Function: mutate_authorization
    [Mutator("MutateRedirectAuthorization")]
    [CMutator("mutate_authorization")]
    [Description("Mutates Authorization header for REDIRECT")]
    public class RtspMutateRedirectAuthorization : LLMMutator
    {
        public RtspMutateRedirectAuthorization(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("redirect") &&
                   (obj.Name == "authorization" || obj.Name.Contains("Authorization"));
        }

        public override int count => 1;

        public override uint mutation { get; set; }

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        protected override void PerformMutation(DataElement obj)
        {
            if (obj.Name == "authorization" || obj.Name.Contains("Authorization"))
            {
                var random = RtspUtils.GetRandom();
                int[] weights = { 100, 0, 0, 0, 0, 0, 100, 0, 0, 0, 100, 0, 100, 0, 0, 0 };
                
                int opIdx = RtspUtils.WeightedPickIdx(weights, weights.Length, random);
                string mutatedValue = "";

                switch (opIdx)
                {
                    case 0: // auth_basic_valid
                        mutatedValue = "Basic Z3Vlc3Q6Z3Vlc3Q=";
                        break;
                    case 6: // auth_digest_valid
                        mutatedValue = "Digest username=\"user\", realm=\"live\", nonce=\"abc\", uri=\"rtsp://x\", response=\"0123456789abcdef\", qop=auth, nc=00000001, cnonce=\"xyz\"";
                        break;
                    case 10: // auth_digest_dup_params
                        mutatedValue = "Digest username=\"u\", username=\"u2\", realm=\"r\", nonce=\"n\", uri=\"/\", response=\"r\"";
                        break;
                    case 12: // auth_unknown_scheme
                        mutatedValue = "Bearer tok_tok_tok";
                        break;
                    default:
                        mutatedValue = "Basic Z3Vlc3Q6Z3Vlc3Q=";
                        break;
                }

                obj.MutatedValue = new Variant(mutatedValue);
            }
        }
    }



    // ==================== Session ====================
    
    // C Function: delete_session
    [Mutator("DeleteRedirectSession")]
    [CMutator("delete_session")]
    [Description("Deletes Session header for REDIRECT")]
    public class RtspDeleteRedirectSession : LLMMutator
    {
        public RtspDeleteRedirectSession(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            // 严格检查：必须是redirect上下文，名称必须是session，且必须是String类型
            return obj.IsIn("redirect") && 
                   obj.Name == "session" && 
                   obj is Peach.Core.Dom.String;
        }

        public override int count => 1;

        public override uint mutation { get; set; }

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
    [Mutator("RepeatRedirectSession")]
    [CMutator("repeat_session")]
    [Description("Repeats Session header for REDIRECT")]
    public class RtspRepeatRedirectSession : LLMMutator
    {
        public RtspRepeatRedirectSession(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            // 严格检查：必须是redirect上下文，名称必须是session，且必须是String类型
            return obj.IsIn("redirect") && 
                   obj.Name == "session" && 
                   obj is Peach.Core.Dom.String;
        }

        public override int count => 1;

        public override uint mutation { get; set; }

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
    [Mutator("MutateRedirectSession")]
    [CMutator("mutate_session")]
    [Description("Mutates Session header for REDIRECT")]
    public class RtspMutateRedirectSession : LLMMutator
    {
        public RtspMutateRedirectSession(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("redirect") && 
                   obj.Name == "session" && 
                   obj is Peach.Core.Dom.String;
        }

        public override int count => 1;

        public override uint mutation { get; set; }

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        protected override void PerformMutation(DataElement obj)
        {
            // 多重检查：确保类型和名称都正确

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

