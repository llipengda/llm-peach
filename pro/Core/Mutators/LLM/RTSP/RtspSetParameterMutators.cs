using System;
using System.ComponentModel;
using System.Text;
using Peach.Core;
using Peach.Core.Dom;

namespace Peach.Pro.Core.Mutators.LLM.RTSP
{
    // ==================== Connection ====================
    
    // C Function: delete_connection
    [Mutator("DeleteSetParameterConnection")]
    [CMutator("delete_connection")]
    [Description("Deletes Connection header for SET_PARAMETER")]
    public class RtspDeleteSetParameterConnection : LLMMutator
    {
        public RtspDeleteSetParameterConnection(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("set_parameter") &&
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
    [Mutator("RepeatSetParameterConnection")]
    [CMutator("repeat_connection")]
    [Description("Repeats Connection header for SET_PARAMETER")]
    public class RtspRepeatSetParameterConnection : LLMMutator
    {
        public RtspRepeatSetParameterConnection(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("set_parameter") &&
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
    [Mutator("MutateSetParameterConnection")]
    [CMutator("mutate_connection")]
    [Description("Mutates Connection header for SET_PARAMETER")]
    public class RtspMutateSetParameterConnection : LLMMutator
    {
        public RtspMutateSetParameterConnection(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("set_parameter") &&
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
    [Mutator("DeleteSetParameterDate")]
    [CMutator("delete_date")]
    [Description("Deletes Date header for SET_PARAMETER")]
    public class RtspDeleteSetParameterDate : LLMMutator
    {
        public RtspDeleteSetParameterDate(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("set_parameter") &&
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
                obj.MutatedValue = new Variant("");
            }
        }
    }

    // C Function: repeat_date
    [Mutator("RepeatSetParameterDate")]
    [CMutator("repeat_date")]
    [Description("Repeats Date header for SET_PARAMETER")]
    public class RtspRepeatSetParameterDate : LLMMutator
    {
        public RtspRepeatSetParameterDate(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("set_parameter") &&
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
    [Mutator("MutateSetParameterDate")]
    [CMutator("mutate_date")]
    [Description("Mutates Date header for SET_PARAMETER")]
    public class RtspMutateSetParameterDate : LLMMutator
    {
        public RtspMutateSetParameterDate(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("set_parameter") &&
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
    [Mutator("DeleteSetParameterVia")]
    [CMutator("delete_via")]
    [Description("Deletes Via header for SET_PARAMETER")]
    public class RtspDeleteSetParameterVia : LLMMutator
    {
        public RtspDeleteSetParameterVia(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("set_parameter") &&
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
    [Mutator("RepeatSetParameterVia")]
    [CMutator("repeat_via")]
    [Description("Repeats Via header for SET_PARAMETER")]
    public class RtspRepeatSetParameterVia : LLMMutator
    {
        public RtspRepeatSetParameterVia(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("set_parameter") &&
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
    [Mutator("MutateSetParameterVia")]
    [CMutator("mutate_via")]
    [Description("Mutates Via header for SET_PARAMETER")]
    public class RtspMutateSetParameterVia : LLMMutator
    {
        public RtspMutateSetParameterVia(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("set_parameter") &&
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

    // ==================== Accept-Language ====================
    
    // C Function: delete_accept_language
    [Mutator("DeleteSetParameterAcceptLanguage")]
    [CMutator("delete_accept_language")]
    [Description("Deletes Accept-Language header for SET_PARAMETER")]
    public class RtspDeleteSetParameterAcceptLanguage : LLMMutator
    {
        public RtspDeleteSetParameterAcceptLanguage(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("set_parameter") &&
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
    [Mutator("RepeatSetParameterAcceptLanguage")]
    [CMutator("repeat_accept_language")]
    [Description("Repeats Accept-Language header for SET_PARAMETER")]
    public class RtspRepeatSetParameterAcceptLanguage : LLMMutator
    {
        public RtspRepeatSetParameterAcceptLanguage(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("set_parameter") &&
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
    [Mutator("MutateSetParameterAcceptLanguage")]
    [CMutator("mutate_accept_language")]
    [Description("Mutates Accept-Language header for SET_PARAMETER")]
    public class RtspMutateSetParameterAcceptLanguage : LLMMutator
    {
        public RtspMutateSetParameterAcceptLanguage(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("set_parameter") &&
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


    // ==================== Content-Encoding ====================
    
    // C Function: delete_content_encoding
    [Mutator("DeleteSetParameterContentEncoding")]
    [CMutator("delete_content_encoding")]
    [Description("Deletes Content-Encoding header for SET_PARAMETER")]
    public class RtspDeleteSetParameterContentEncoding : LLMMutator
    {
        public RtspDeleteSetParameterContentEncoding(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("set_parameter") &&
                   (obj.Name == "content_encoding" || obj.Name.Contains("Content-Encoding"));
        }

        public override int count => 1;

        public override uint mutation { get; set; }

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.Name == "content_encoding" || obj.Name.Contains("Content-Encoding"))
            {
                obj.MutatedValue = new Variant("");
            }
        }
    }

    // C Function: repeat_content_encoding
    [Mutator("RepeatSetParameterContentEncoding")]
    [CMutator("repeat_content_encoding")]
    [Description("Repeats Content-Encoding header for SET_PARAMETER")]
    public class RtspRepeatSetParameterContentEncoding : LLMMutator
    {
        public RtspRepeatSetParameterContentEncoding(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("set_parameter") &&
                   (obj.Name == "content_encoding" || obj.Name.Contains("Content-Encoding"));
        }

        public override int count => 1;

        public override uint mutation { get; set; }

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.Name == "content_encoding" || obj.Name.Contains("Content-Encoding"))
            {
                obj.MutatedValue = new Variant("gzip, deflate, br");
            }
        }
    }

    // C Function: mutate_content_encoding
    [Mutator("MutateSetParameterContentEncoding")]
    [CMutator("mutate_content_encoding")]
    [Description("Mutates Content-Encoding header for SET_PARAMETER")]
    public class RtspMutateSetParameterContentEncoding : LLMMutator
    {
        public RtspMutateSetParameterContentEncoding(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("set_parameter") &&
                   (obj.Name == "content_encoding" || obj.Name.Contains("Content-Encoding"));
        }

        public override int count => 1;

        public override uint mutation { get; set; }

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.Name == "content_encoding" || obj.Name.Contains("Content-Encoding"))
            {
                var random = RtspUtils.GetRandom();
                int[] weights = { 100, 100, 100, 100, 0, 0, 0, 100, 0, 0, 0, 0 };
                
                int opIdx = RtspUtils.WeightedPickIdx(weights, weights.Length, random);
                string mutatedValue = "";

                switch (opIdx)
                {
                    case 0: // ce_gzip
                        mutatedValue = "gzip";
                        break;
                    case 1: // ce_deflate
                        mutatedValue = "deflate";
                        break;
                    case 2: // ce_identity
                        mutatedValue = "identity";
                        break;
                    case 3: // ce_unknown_token
                        mutatedValue = "x-zstd";
                        break;
                    case 4: // ce_multi
                        mutatedValue = "gzip,br";
                        break;
                    case 7: // ce_ws_fold
                        mutatedValue = "\tdeflate";
                        break;
                    default:
                        mutatedValue = "gzip";
                        break;
                }

                obj.MutatedValue = new Variant(mutatedValue);
            }
        }
    }

    // ==================== Content-Length ====================
    
    // C Function: delete_content_length
    [Mutator("DeleteSetParameterContentLength")]
    [CMutator("delete_content_length")]
    [Description("Deletes Content-Length header for SET_PARAMETER")]
    public class RtspDeleteSetParameterContentLength : LLMMutator
    {
        public RtspDeleteSetParameterContentLength(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("set_parameter") &&
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
    [Mutator("RepeatSetParameterContentLength")]
    [CMutator("repeat_content_length")]
    [Description("Repeats Content-Length header for SET_PARAMETER")]
    public class RtspRepeatSetParameterContentLength : LLMMutator
    {
        public RtspRepeatSetParameterContentLength(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("set_parameter") &&
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
    [Mutator("MutateSetParameterContentLength")]
    [CMutator("mutate_content_length")]
    [Description("Mutates Content-Length header for SET_PARAMETER")]
    public class RtspMutateSetParameterContentLength : LLMMutator
    {
        public RtspMutateSetParameterContentLength(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("set_parameter") &&
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

    // ==================== Content-Type ====================
    
    // C Function: delete_content_type
    [Mutator("DeleteSetParameterContentType")]
    [CMutator("delete_content_type")]
    [Description("Deletes Content-Type header for SET_PARAMETER")]
    public class RtspDeleteSetParameterContentType : LLMMutator
    {
        public RtspDeleteSetParameterContentType(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("set_parameter") &&
                   (obj.Name == "content_type" || obj.Name.Contains("Content-Type"));
        }

        public override int count => 1;

        public override uint mutation { get; set; }

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.Name == "content_type" || obj.Name.Contains("Content-Type"))
            {
                obj.MutatedValue = new Variant("");
            }
        }
    }

    // C Function: repeat_content_type
    [Mutator("RepeatSetParameterContentType")]
    [CMutator("repeat_content_type")]
    [Description("Repeats Content-Type header for SET_PARAMETER")]
    public class RtspRepeatSetParameterContentType : LLMMutator
    {
        public RtspRepeatSetParameterContentType(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("set_parameter") &&
                   (obj.Name == "content_type" || obj.Name.Contains("Content-Type"));
        }

        public override int count => 1;

        public override uint mutation { get; set; }

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.Name == "content_type" || obj.Name.Contains("Content-Type"))
            {
                obj.MutatedValue = new Variant("application/sdp, text/plain");
            }
        }
    }

    // C Function: mutate_content_type
    [Mutator("MutateSetParameterContentType")]
    [CMutator("mutate_content_type")]
    [Description("Mutates Content-Type header for SET_PARAMETER")]
    public class RtspMutateSetParameterContentType : LLMMutator
    {
        public RtspMutateSetParameterContentType(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("set_parameter") &&
                   (obj.Name == "content_type" || obj.Name.Contains("Content-Type"));
        }

        public override int count => 1;

        public override uint mutation { get; set; }

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.Name == "content_type" || obj.Name.Contains("Content-Type"))
            {
                var random = RtspUtils.GetRandom();
                int[] weights = { 100, 100, 100, 0, 100, 100, 0, 0, 0, 100, 100, 0, 0, 0 };
                
                int opIdx = RtspUtils.WeightedPickIdx(weights, weights.Length, random);
                string mutatedValue = "";

                switch (opIdx)
                {
                    case 0: // ct_sdp
                        mutatedValue = "application/sdp";
                        break;
                    case 1: // ct_text_plain
                        mutatedValue = "text/plain";
                        break;
                    case 2: // ct_json
                        mutatedValue = "application/json";
                        break;
                    case 4: // ct_param_charset
                        mutatedValue = "application/sdp; charset=UTF-8";
                        break;
                    case 5: // ct_upper_lower
                        mutatedValue = "APPLICATION/SDP";
                        break;
                    case 9: // ct_long_tokens
                        byte[] mt = new byte[64];
                        byte[] st = new byte[64];
                        RtspUtils.MakeRepeatedChar(mt, 63, (byte)'A', 63);
                        RtspUtils.MakeRepeatedChar(st, 63, (byte)'B', 63);
                        mutatedValue = System.Text.Encoding.UTF8.GetString(mt).TrimEnd('\0') + "/" + System.Text.Encoding.UTF8.GetString(st).TrimEnd('\0');
                        break;
                    case 10: // ct_param_semicolon_chain
                        mutatedValue = "application/sdp;level=3;profile=cb;boundary=xyz";
                        break;
                    default:
                        mutatedValue = "application/sdp";
                        break;
                }

                obj.MutatedValue = new Variant(mutatedValue);
            }
        }
    }


    // ==================== Session ====================
    
    // C Function: delete_session
    [Mutator("DeleteSetParameterSession")]
    [CMutator("delete_session")]
    [Description("Deletes Session header for SET_PARAMETER")]
    public class RtspDeleteSetParameterSession : LLMMutator
    {
        public RtspDeleteSetParameterSession(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            // 严格检查：必须是set_parameter上下文，名称必须是session，且必须是String类型
            return obj.IsIn("set_parameter") && 
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
    [Mutator("RepeatSetParameterSession")]
    [CMutator("repeat_session")]
    [Description("Repeats Session header for SET_PARAMETER")]
    public class RtspRepeatSetParameterSession : LLMMutator
    {
        public RtspRepeatSetParameterSession(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            // 严格检查：必须是set_parameter上下文，名称必须是session，且必须是String类型
            return obj.IsIn("set_parameter") && 
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
    [Mutator("MutateSetParameterSession")]
    [CMutator("mutate_session")]
    [Description("Mutates Session header for SET_PARAMETER")]
    public class RtspMutateSetParameterSession : LLMMutator
    {
        public RtspMutateSetParameterSession(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            // 严格检查：必须是set_parameter上下文，名称必须是session，且必须是String类型
            if (obj == null)
                return false;
            
            if (!obj.IsIn("set_parameter"))
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
            
            // 确保在set_parameter上下文中
            if (!obj.IsIn("set_parameter"))
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

