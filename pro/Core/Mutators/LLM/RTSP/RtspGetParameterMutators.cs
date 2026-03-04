using System;
using System.ComponentModel;
using System.Text;
using Peach.Core;
using Peach.Core.Dom;

namespace Peach.Pro.Core.Mutators.LLM.RTSP
{
    // ==================== Connection ====================
    
    // C Function: delete_connection
    [Mutator("DeleteGetParameterConnection")]
    [CMutator("delete_connection")]
    [Description("Deletes Connection header for GET_PARAMETER")]
    public class RtspDeleteGetParameterConnection : LLMMutator
    {
        public RtspDeleteGetParameterConnection(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("get_parameter") &&
                   (obj.Name == "connection" || obj.Name.Contains("Connection"));
        }

        public override int count => 1;

        public override uint mutation { get; set; }

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.Name == "connection" || obj.Name.Contains("Connection"))
            {
                obj.MutatedValue = new Variant("");
            }
        }
    }

    // C Function: repeat_connection
    [Mutator("RepeatGetParameterConnection")]
    [CMutator("repeat_connection")]
    [Description("Repeats Connection header for GET_PARAMETER")]
    public class RtspRepeatGetParameterConnection : LLMMutator
    {
        public RtspRepeatGetParameterConnection(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("get_parameter") &&
                   (obj.Name == "connection" || obj.Name.Contains("Connection"));
        }

        public override int count => 1;
        public override uint mutation { get; set; }
        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.Name == "connection" || obj.Name.Contains("Connection"))
            {
                obj.MutatedValue = new Variant("keep-alive, close");
            }
        }
    }

    // C Function: mutate_connection
    [Mutator("MutateGetParameterConnection")]
    [CMutator("mutate_connection")]
    [Description("Mutates Connection header for GET_PARAMETER")]
    public class RtspMutateGetParameterConnection : LLMMutator
    {
        public RtspMutateGetParameterConnection(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("get_parameter") &&
                   (obj.Name == "connection" || obj.Name.Contains("Connection"));
        }

        public override int count => 1;

        public override uint mutation { get; set; }

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
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
    [Mutator("DeleteGetParameterDate")]
    [CMutator("delete_date")]
    [Description("Deletes Date header for GET_PARAMETER")]
    public class RtspDeleteGetParameterDate : LLMMutator
    {
        public RtspDeleteGetParameterDate(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("get_parameter") &&
                   (obj.Name == "date" || obj.Name.Contains("Date"));
        }
        public override uint mutation { get; set; } 
        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.Name == "date" || obj.Name.Contains("Date"))
            {
                obj.MutatedValue = new Variant("");
            }
        }
    }

    // C Function: repeat_date
    [Mutator("RepeatGetParameterDate")]
    [CMutator("repeat_date")]
    [Description("Repeats Date header for GET_PARAMETER")]
    public class RtspRepeatGetParameterDate : LLMMutator
    {
        public RtspRepeatGetParameterDate(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("get_parameter") &&
                   (obj.Name == "date" || obj.Name.Contains("Date"));
        }

        public override int count => 1;

        public override uint mutation { get; set; }

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.Name == "date" || obj.Name.Contains("Date"))
            {
                obj.MutatedValue = new Variant("Tue, 15 Nov 1994 08:12:31 GMT, Wed, 16 Nov 1994 09:13:32 GMT");
            }
        }
    }

    // C Function: mutate_date
    [Mutator("MutateGetParameterDate")]
    [CMutator("mutate_date")]
    [Description("Mutates Date header for GET_PARAMETER")]
    public class RtspMutateGetParameterDate : LLMMutator
    {
        public RtspMutateGetParameterDate(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("get_parameter") &&
                   (obj.Name == "date" || obj.Name.Contains("Date"));
        }

        public override int count => 1;

        public override uint mutation { get; set; }

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.Name == "date" || obj.Name.Contains("Date"))
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

    // ==================== Via ====================
    
    // C Function: delete_via
    [Mutator("DeleteGetParameterVia")]
    [CMutator("delete_via")]
    [Description("Deletes Via header for GET_PARAMETER")]
    public class RtspDeleteGetParameterVia : LLMMutator
    {
        public RtspDeleteGetParameterVia(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("get_parameter") &&
                   (obj.Name == "via" || obj.Name.Contains("Via"));
        }

        public override int count => 1;

        public override uint mutation { get; set; }

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.Name == "via" || obj.Name.Contains("Via"))
            {
                obj.MutatedValue = new Variant("");
            }
        }
    }

    // C Function: repeat_via
    [Mutator("RepeatGetParameterVia")]
    [CMutator("repeat_via")]
    [Description("Repeats Via header for GET_PARAMETER")]
    public class RtspRepeatGetParameterVia : LLMMutator
    {
        public RtspRepeatGetParameterVia(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("get_parameter") &&
                   (obj.Name == "via" || obj.Name.Contains("Via"));
        }

        public override int count => 1;

        public override uint mutation { get; set; }

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.Name == "via" || obj.Name.Contains("Via"))
            {
                obj.MutatedValue = new Variant("RTSP/1.0 example.com, RTSP/1.0 proxy.com");
            }
        }
    }

    // C Function: mutate_via
    [Mutator("MutateGetParameterVia")]
    [CMutator("mutate_via")]
    [Description("Mutates Via header for GET_PARAMETER")]
    public class RtspMutateGetParameterVia : LLMMutator
    {
        public RtspMutateGetParameterVia(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("get_parameter") &&
                   (obj.Name == "via" || obj.Name.Contains("Via"));
        }

        public override int count => 1;

        public override uint mutation { get; set; }

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
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

    // ==================== Accept ====================
    
    // C Function: delete_accept
    [Mutator("DeleteGetParameterAccept")]
    [CMutator("delete_accept")]
    [Description("Deletes Accept header for GET_PARAMETER")]
    public class RtspDeleteGetParameterAccept : LLMMutator
    {
        public RtspDeleteGetParameterAccept(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            // 严格检查：必须是get_parameter上下文，名称必须是accept，且必须是String类型
            return obj.IsIn("get_parameter") && 
                   obj.Name == "accept" && 
                   obj is Peach.Core.Dom.String;
        }

        public override uint mutation { get; set; }
        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            // 双重检查：确保是String类型且名称正确
            if (obj.Name == "accept" && obj is Peach.Core.Dom.String)
            {
                obj.MutatedValue = new Variant("");
            }
        }
    }

    // C Function: repeat_accept
    [Mutator("RepeatGetParameterAccept")]
    [CMutator("repeat_accept")]
    [Description("Repeats Accept header for GET_PARAMETER")]
    public class RtspRepeatGetParameterAccept : LLMMutator
    {
        public RtspRepeatGetParameterAccept(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            // 严格检查：必须是get_parameter上下文，名称必须是accept，且必须是String类型
            return obj.IsIn("get_parameter") && 
                   obj.Name == "accept" && 
                   obj is Peach.Core.Dom.String;
        }

        public override uint mutation { get; set; }
        public override int count => 1;


        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            // 双重检查：确保是String类型且名称正确
            if (obj.Name == "accept" && obj is Peach.Core.Dom.String)
            {
                obj.MutatedValue = new Variant("application/sdp, */*;q=0.1, text/plain");
            }
        }
    }

    // C Function: mutate_accept
    [Mutator("MutateGetParameterAccept")]
    [CMutator("mutate_accept")]
    [Description("Mutates Accept header for GET_PARAMETER")]
    public class RtspMutateGetParameterAccept : LLMMutator
    {
        public RtspMutateGetParameterAccept(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            // 严格检查：必须是get_parameter上下文，名称必须是accept，且必须是String类型
            if (obj == null)
                return false;
            
            if (!obj.IsIn("get_parameter"))
                return false;
            
            if (obj.Name != "accept")
                return false;
            
            if (!(obj is Peach.Core.Dom.String))
                return false;
            
            return true;
        }

        public override uint mutation { get; set; }
        public override int count => 1;


        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            // 多重检查：确保类型和名称都正确
            if (obj == null)
                return;
            
            if (obj.Name != "accept")
                return;
            
            if (!(obj is Peach.Core.Dom.String))
                return;
            
            // 确保在get_parameter上下文中
            if (!obj.IsIn("get_parameter"))
                return;

            var random = RtspUtils.GetRandom();
            int[] weights = { 100, 100, 100, 0, 0, 0, 0, 0, 0, 100, 0, 0, 0, 0 };
            
            int opIdx = RtspUtils.WeightedPickIdxAccept(weights, weights.Length, random);
            string mutatedValue = "";

            switch (opIdx)
            {
                case 0: // acc_set_valid_sdp
                    mutatedValue = "application/sdp";
                    break;
                case 1: // acc_set_wildcard_any
                    mutatedValue = "*/*";
                    break;
                case 2: // acc_set_with_params
                    mutatedValue = "application/sdp;level=1;q=1.0;charset=utf-8";
                    break;
                case 9: // acc_multi_values_in_one
                    mutatedValue = "application/sdp, */*;q=0.1, text/plain";
                    break;
                default:
                    mutatedValue = "application/sdp";
                    break;
            }

            obj.MutatedValue = new Variant(mutatedValue);
        }
    }

    // ==================== Accept-Encoding ====================
    
    // C Function: delete_accept_encoding
    [Mutator("DeleteGetParameterAcceptEncoding")]
    [CMutator("delete_accept_encoding")]
    [Description("Deletes Accept-Encoding header for GET_PARAMETER")]
    public class RtspDeleteGetParameterAcceptEncoding : LLMMutator
    {
        public RtspDeleteGetParameterAcceptEncoding(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("get_parameter") &&
                   (obj.Name == "accept_encoding" || obj.Name.Contains("encoding"));
        }

        public override uint mutation { get; set; }
        public override int count => 1;


        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.Name == "accept_encoding" || obj.Name.Contains("encoding"))
            {
                obj.MutatedValue = new Variant("");
            }
        }
    }

    // C Function: repeat_accept_encoding
    [Mutator("RepeatGetParameterAcceptEncoding")]
    [CMutator("repeat_accept_encoding")]
    [Description("Repeats Accept-Encoding header for GET_PARAMETER")]
    public class RtspRepeatGetParameterAcceptEncoding : LLMMutator
    {
        public RtspRepeatGetParameterAcceptEncoding(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("get_parameter") &&
                   (obj.Name == "accept_encoding" || obj.Name.Contains("encoding"));
        }

        public override int count => 1;
        public override uint mutation { get; set; } 
        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.Name == "accept_encoding" || obj.Name.Contains("encoding"))
            {
                obj.MutatedValue = new Variant("gzip, deflate, br");
            }
        }
    }

    // C Function: mutate_accept_encoding
    [Mutator("MutateGetParameterAcceptEncoding")]
    [CMutator("mutate_accept_encoding")]
    [Description("Mutates Accept-Encoding header for GET_PARAMETER")]
    public class RtspMutateGetParameterAcceptEncoding : LLMMutator
    {
        public RtspMutateGetParameterAcceptEncoding(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("get_parameter") &&
                   (obj.Name == "accept_encoding" || obj.Name.Contains("encoding"));
        }

        public override int count => 1;
        public override uint mutation { get; set; }     
        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.Name == "accept_encoding" || obj.Name.Contains("encoding"))
            {
                var random = RtspUtils.GetRandom();
                int[] weights = { 100, 100, 100, 100, 0, 0, 0, 0, 0, 0, 0, 100, 0, 100 };
                
                int opIdx = RtspUtils.WeightedPickIdx(weights, weights.Length, random);
                string mutatedValue = "";

                switch (opIdx)
                {
                    case 0: // ae_set_gzip
                        mutatedValue = "gzip";
                        break;
                    case 1: // ae_set_identity_only
                        mutatedValue = "identity";
                        break;
                    case 2: // ae_set_all_wildcard
                        mutatedValue = "*";
                        break;
                    case 3: // ae_set_with_qparams
                        mutatedValue = "gzip;q=1.0, deflate;q=0.5, br;q=0.0";
                        break;
                    case 11: // ae_duplicates_and_order
                        mutatedValue = "gzip, gzip, deflate, br";
                        break;
                    case 13: // ae_unknown_and_wildcard
                        mutatedValue = "unknown, *";
                        break;
                    default:
                        mutatedValue = "gzip";
                        break;
                }

                obj.MutatedValue = new Variant(mutatedValue);
            }
        }
    }

    // ==================== Accept-Language ====================
    
    // C Function: delete_accept_language
    [Mutator("DeleteGetParameterAcceptLanguage")]
    [CMutator("delete_accept_language")]
    [Description("Deletes Accept-Language header for GET_PARAMETER")]
    public class RtspDeleteGetParameterAcceptLanguage : LLMMutator
    {
        public RtspDeleteGetParameterAcceptLanguage(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("get_parameter") &&
                   (obj.Name == "accept_language" || obj.Name.Contains("language"));
        }

        public override int count => 1;
        public override uint mutation { get; set; }     
        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.Name == "accept_language" || obj.Name.Contains("language"))
            {
                obj.MutatedValue = new Variant("");
            }
        }
    }

    // C Function: repeat_accept_language
    [Mutator("RepeatGetParameterAcceptLanguage")]
    [CMutator("repeat_accept_language")]
    [Description("Repeats Accept-Language header for GET_PARAMETER")]
    public class RtspRepeatGetParameterAcceptLanguage : LLMMutator
    {
        public RtspRepeatGetParameterAcceptLanguage(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("get_parameter") &&
                   (obj.Name == "accept_language" || obj.Name.Contains("language"));
        }

        public override int count => 1;
        public override uint mutation { get; set; }     
        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.Name == "accept_language" || obj.Name.Contains("language"))
            {
                obj.MutatedValue = new Variant("en-US, fr-FR, de-DE");
            }
        }
    }

    // C Function: mutate_accept_language
    [Mutator("MutateGetParameterAcceptLanguage")]
    [CMutator("mutate_accept_language")]
    [Description("Mutates Accept-Language header for GET_PARAMETER")]
    public class RtspMutateGetParameterAcceptLanguage : LLMMutator
    {
        public RtspMutateGetParameterAcceptLanguage(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("get_parameter") &&
                   (obj.Name == "accept_language" || obj.Name.Contains("language"));
        }

        public override int count => 1;
        public override uint mutation { get; set; }         
        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
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



    // ==================== Session ====================
    
    // C Function: delete_session
    [Mutator("DeleteGetParameterSession")]
    [CMutator("delete_session")]
    [Description("Deletes Session header for GET_PARAMETER")]
    public class RtspDeleteGetParameterSession : LLMMutator
    {
        public RtspDeleteGetParameterSession(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            // 严格检查：必须是get_parameter上下文，名称必须是session，且必须是String类型
            return obj.IsIn("get_parameter") && 
                   obj.Name == "session" && 
                   obj is Peach.Core.Dom.String;
        }

        public override int count => 1;
        public override uint mutation { get; set; }         
        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            // 双重检查：确保是String类型且名称正确
            if (obj.Name == "session" && obj is Peach.Core.Dom.String)
            {
                obj.MutatedValue = new Variant("");
            }
        }
    }

    // C Function: repeat_session
    [Mutator("RepeatGetParameterSession")]
    [CMutator("repeat_session")]
    [Description("Repeats Session header for GET_PARAMETER")]
    public class RtspRepeatGetParameterSession : LLMMutator
    {
        public RtspRepeatGetParameterSession(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            // 严格检查：必须是get_parameter上下文，名称必须是session，且必须是String类型
            return obj.IsIn("get_parameter") && 
                   obj.Name == "session" && 
                   obj is Peach.Core.Dom.String;
        }

        public override int count => 1;
        public override uint mutation { get; set; }         
        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            // 双重检查：确保是String类型且名称正确
            if (obj.Name == "session" && obj is Peach.Core.Dom.String)
            {
                obj.MutatedValue = new Variant("ABCDEF, 1234");
            }
        }
    }

    // C Function: mutate_session
    [Mutator("MutateGetParameterSession")]
    [CMutator("mutate_session")]
    [Description("Mutates Session header for GET_PARAMETER")]
    public class RtspMutateGetParameterSession : LLMMutator
    {
        public RtspMutateGetParameterSession(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            // 严格检查：必须是get_parameter上下文，名称必须是session，且必须是String类型
            if (obj == null)
                return false;
            
            if (!obj.IsIn("get_parameter"))
                return false;
            
            if (obj.Name != "session")
                return false;
            
            if (!(obj is Peach.Core.Dom.String))
                return false;
            
            return true;
        }

        public override int count => 1;
        public override uint mutation { get; set; }         
        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            // 多重检查：确保类型和名称都正确
            if (obj == null)
                return;
            
            if (obj.Name != "session")
                return;
            
            if (!(obj is Peach.Core.Dom.String))
                return;
            
            // 确保在get_parameter上下文中
            if (!obj.IsIn("get_parameter"))
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


    // ==================== Content-Base ====================
    
    // C Function: delete_content_base
    [Mutator("DeleteGetParameterContentBase")]
    [CMutator("delete_content_base")]
    [Description("Deletes Content-Base header for GET_PARAMETER")]
    public class RtspDeleteGetParameterContentBase : LLMMutator
    {
        public RtspDeleteGetParameterContentBase(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("get_parameter") &&
                   (obj.Name == "content_base" || obj.Name.Contains("Content-Base"));
        }

        public override int count => 1;

        public override uint mutation { get; set; }

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.Name == "content_base" || obj.Name.Contains("Content-Base"))
            {
                obj.MutatedValue = new Variant("");
            }
        }
    }

    // C Function: repeat_content_base
    [Mutator("RepeatGetParameterContentBase")]
    [CMutator("repeat_content_base")]
    [Description("Repeats Content-Base header for GET_PARAMETER")]
    public class RtspRepeatGetParameterContentBase : LLMMutator
    {
        public RtspRepeatGetParameterContentBase(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("get_parameter") &&
                   (obj.Name == "content_base" || obj.Name.Contains("Content-Base"));
        }

        public override int count => 1;
        public override uint mutation { get; set; }             
        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.Name == "content_base" || obj.Name.Contains("Content-Base"))
            {
                obj.MutatedValue = new Variant("rtsp://a/ , rtsp://b/");
            }
        }
    }

    // C Function: mutate_content_base
    [Mutator("MutateGetParameterContentBase")]
    [CMutator("mutate_content_base")]
    [Description("Mutates Content-Base header for GET_PARAMETER")]
    public class RtspMutateGetParameterContentBase : LLMMutator
    {
        public RtspMutateGetParameterContentBase(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("get_parameter") &&
                   (obj.Name == "content_base" || obj.Name.Contains("Content-Base"));
        }

        public override int count => 1;
        public override uint mutation { get; set; }             
        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.Name == "content_base" || obj.Name.Contains("Content-Base"))
            {
                var random = RtspUtils.GetRandom();
                int[] weights = { 100, 100, 100, 100, 100, 0, 0, 0, 0, 0, 0, 0 };
                
                int opIdx = RtspUtils.WeightedPickIdx(weights, weights.Length, random);
                string mutatedValue = "";

                switch (opIdx)
                {
                    case 0: // cb_valid_abs
                        mutatedValue = "rtsp://host/app/";
                        break;
                    case 1: // cb_http_scheme
                        mutatedValue = "http://host/app/";
                        break;
                    case 2: // cb_no_trailing_slash
                        mutatedValue = "rtsp://host/app";
                        break;
                    case 3: // cb_ipv6
                        mutatedValue = "rtsp://[2001:db8::1]/app/";
                        break;
                    case 4: // cb_userinfo
                        mutatedValue = "rtsp://u:p@host/app/";
                        break;
                    default:
                        mutatedValue = "rtsp://host/app/";
                        break;
                }

                obj.MutatedValue = new Variant(mutatedValue);
            }
        }
    }

    // ==================== Content-Length ====================
    
    // C Function: delete_content_length
    [Mutator("DeleteGetParameterContentLength")]
    [CMutator("delete_content_length")]
    [Description("Deletes Content-Length header for GET_PARAMETER")]
    public class RtspDeleteGetParameterContentLength : LLMMutator
    {
        public RtspDeleteGetParameterContentLength(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("get_parameter") &&
                   (obj.Name == "content_length" || obj.Name.Contains("Content-Length"));
        }

        public override int count => 1;
        public override uint mutation { get; set; }                   
        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.Name == "content_length" || obj.Name.Contains("Content-Length"))
            {
                obj.MutatedValue = new Variant("");
            }
        }
    }

    // C Function: repeat_content_length
    [Mutator("RepeatGetParameterContentLength")]
    [CMutator("repeat_content_length")]
    [Description("Repeats Content-Length header for GET_PARAMETER")]
    public class RtspRepeatGetParameterContentLength : LLMMutator
    {
        public RtspRepeatGetParameterContentLength(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("get_parameter") &&
                   (obj.Name == "content_length" || obj.Name.Contains("Content-Length"));
        }

        public override int count => 1;
        public override uint mutation { get; set; }                   
        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.Name == "content_length" || obj.Name.Contains("Content-Length"))
            {
                obj.MutatedValue = new Variant("1234");
            }
        }
    }

    // C Function: mutate_content_length
    [Mutator("MutateGetParameterContentLength")]
    [CMutator("mutate_content_length")]
    [Description("Mutates Content-Length header for GET_PARAMETER")]
    public class RtspMutateGetParameterContentLength : LLMMutator
    {
        public RtspMutateGetParameterContentLength(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("get_parameter") &&
                   (obj.Name == "content_length" || obj.Name.Contains("Content-Length"));
        }

        public override int count => 1;
        public override uint mutation { get; set; }                   
        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.Name == "content_length" || obj.Name.Contains("Content-Length"))
            {
                var random = RtspUtils.GetRandom();
                int[] weights = { 100, 100, 0, 0, 0, 0, 100, 100, 0, 0, 0, 0 };
                
                int opIdx = RtspUtils.WeightedPickIdx(weights, weights.Length, random);
                string mutatedValue = "";

                switch (opIdx)
                {
                    case 0: // clen_ok_small
                        mutatedValue = "0";
                        break;
                    case 1: // clen_ok_typical
                        mutatedValue = "128";
                        break;
                    case 6: // clen_off_by_one_low
                        mutatedValue = "1";
                        break;
                    case 7: // clen_off_by_one_high
                        mutatedValue = "1025";
                        break;
                    default:
                        mutatedValue = "128";
                        break;
                }

                obj.MutatedValue = new Variant(mutatedValue);
            }
        }
    }

    // ==================== Content-Location ====================
    
    // C Function: delete_content_location
    [Mutator("DeleteGetParameterContentLocation")]
    [CMutator("delete_content_location")]
    [Description("Deletes Content-Location header for GET_PARAMETER")]
    public class RtspDeleteGetParameterContentLocation : LLMMutator
    {
        public RtspDeleteGetParameterContentLocation(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("get_parameter") &&
                   (obj.Name == "content_location" || obj.Name.Contains("Content-Location"));
        }

        public override int count => 1;
        public override uint mutation { get; set; }                       
        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.Name == "content_location" || obj.Name.Contains("Content-Location"))
            {
                obj.MutatedValue = new Variant("");
            }
        }
    }

    // C Function: repeat_content_location
    [Mutator("RepeatGetParameterContentLocation")]
    [CMutator("repeat_content_location")]
    [Description("Repeats Content-Location header for GET_PARAMETER")]
    public class RtspRepeatGetParameterContentLocation : LLMMutator
    {
        public RtspRepeatGetParameterContentLocation(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("get_parameter") &&
                   (obj.Name == "content_location" || obj.Name.Contains("Content-Location"));
        }

        public override int count => 1;
        public override uint mutation { get; set; }                       
        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.Name == "content_location" || obj.Name.Contains("Content-Location"))
            {
                obj.MutatedValue = new Variant("rtsp://a/s.sdp, rtsp://b/s.sdp");
            }
        }
    }

    // C Function: mutate_content_location
    [Mutator("MutateGetParameterContentLocation")]
    [CMutator("mutate_content_location")]
    [Description("Mutates Content-Location header for GET_PARAMETER")]
    public class RtspMutateGetParameterContentLocation : LLMMutator
    {
        public RtspMutateGetParameterContentLocation(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("get_parameter") &&
                   (obj.Name == "content_location" || obj.Name.Contains("Content-Location"));
        }

        public override int count => 1;
        public override uint mutation { get; set; }                       
        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.Name == "content_location" || obj.Name.Contains("Content-Location"))
            {
                var random = RtspUtils.GetRandom();
                int[] weights = { 100, 100, 100, 100, 100, 0, 0, 0, 0, 0, 0, 0, 0 };
                
                int opIdx = RtspUtils.WeightedPickIdx(weights, weights.Length, random);
                string mutatedValue = "";

                switch (opIdx)
                {
                    case 0: // cloc_abs_rtsp
                        mutatedValue = "rtsp://host/path/file.sdp";
                        break;
                    case 1: // cloc_relative
                        mutatedValue = "../file.sdp";
                        break;
                    case 2: // cloc_http_scheme
                        mutatedValue = "http://host/file.sdp";
                        break;
                    case 3: // cloc_ipv6
                        mutatedValue = "rtsp://[2001:db8::2]/a/b.sdp";
                        break;
                    case 4: // cloc_userinfo
                        mutatedValue = "rtsp://u:p@h/app.sdp";
                        break;
                    default:
                        mutatedValue = "rtsp://host/path/file.sdp";
                        break;
                }

                obj.MutatedValue = new Variant(mutatedValue);
            }
        }
    }

    // ==================== Last-Modified ====================
    
    // C Function: delete_last_modified
    [Mutator("DeleteGetParameterLastModified")]
    [CMutator("delete_last_modified")]
    [Description("Deletes Last-Modified header for GET_PARAMETER")]
    public class RtspDeleteGetParameterLastModified : LLMMutator
    {
        public RtspDeleteGetParameterLastModified(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("get_parameter") &&
                   (obj.Name == "last_modified" || obj.Name.Contains("Last-Modified"));
        }

        public override int count => 1;
        public override uint mutation { get; set; }                         
        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.Name == "last_modified" || obj.Name.Contains("Last-Modified"))
            {
                obj.MutatedValue = new Variant("");
            }
        }
    }

    // C Function: repeat_last_modified
    [Mutator("RepeatGetParameterLastModified")]
    [CMutator("repeat_last_modified")]
    [Description("Repeats Last-Modified header for GET_PARAMETER")]
    public class RtspRepeatGetParameterLastModified : LLMMutator
    {
        public RtspRepeatGetParameterLastModified(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("get_parameter") &&
                   (obj.Name == "last_modified" || obj.Name.Contains("Last-Modified"));
        }

        public override int count => 1;
        public override uint mutation { get; set; }                         
        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.Name == "last_modified" || obj.Name.Contains("Last-Modified"))
            {
                obj.MutatedValue = new Variant("Tue, 15 Nov 1994 08:12:31 GMT, Wed, 16 Nov 1994 09:13:32 GMT");
            }
        }
    }

    // C Function: mutate_last_modified
    [Mutator("MutateGetParameterLastModified")]
    [CMutator("mutate_last_modified")]
    [Description("Mutates Last-Modified header for GET_PARAMETER")]
    public class RtspMutateGetParameterLastModified : LLMMutator
    {
        public RtspMutateGetParameterLastModified(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("get_parameter") &&
                   (obj.Name == "last_modified" || obj.Name.Contains("Last-Modified"));
        }

        public override int count => 1;
        public override uint mutation { get; set; }                           
        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.Name == "last_modified" || obj.Name.Contains("Last-Modified"))
            {
                var random = RtspUtils.GetRandom();
                int[] weights = { 100, 100, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                
                int opIdx = RtspUtils.WeightedPickIdx(weights, weights.Length, random);
                string mutatedValue = "";

                switch (opIdx)
                {
                    case 0: // lm_ok_rfc1123
                        mutatedValue = "Tue, 15 Nov 1994 08:12:31 GMT";
                        break;
                    case 1: // lm_ok_rfc850
                        mutatedValue = "Tuesday, 15-Nov-94 08:12:31 GMT";
                        break;
                    default:
                        mutatedValue = "Tue, 15 Nov 1994 08:12:31 GMT";
                        break;
                }

                obj.MutatedValue = new Variant(mutatedValue);
            }
        }
    }
}

