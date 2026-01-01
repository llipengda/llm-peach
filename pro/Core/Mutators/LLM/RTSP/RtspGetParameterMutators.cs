using System;
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
            return obj.IsIn("get_parameter");
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
    [Mutator("RepeatGetParameterConnection")]
    [CMutator("repeat_connection")]
    [Description("Repeats Connection header for GET_PARAMETER")]
    public class RtspRepeatGetParameterConnection : LLMMutator
    {
        public RtspRepeatGetParameterConnection(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("get_parameter");
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
    [Mutator("MutateGetParameterConnection")]
    [CMutator("mutate_connection")]
    [Description("Mutates Connection header for GET_PARAMETER")]
    public class RtspMutateGetParameterConnection : LLMMutator
    {
        public RtspMutateGetParameterConnection(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("get_parameter");
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
            return obj.IsIn("get_parameter");
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
    [Mutator("RepeatGetParameterDate")]
    [CMutator("repeat_date")]
    [Description("Repeats Date header for GET_PARAMETER")]
    public class RtspRepeatGetParameterDate : LLMMutator
    {
        public RtspRepeatGetParameterDate(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("get_parameter");
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
    [Mutator("MutateGetParameterDate")]
    [CMutator("mutate_date")]
    [Description("Mutates Date header for GET_PARAMETER")]
    public class RtspMutateGetParameterDate : LLMMutator
    {
        public RtspMutateGetParameterDate(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("get_parameter");
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
            return obj.IsIn("get_parameter");
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
    [Mutator("RepeatGetParameterVia")]
    [CMutator("repeat_via")]
    [Description("Repeats Via header for GET_PARAMETER")]
    public class RtspRepeatGetParameterVia : LLMMutator
    {
        public RtspRepeatGetParameterVia(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("get_parameter");
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
    [Mutator("MutateGetParameterVia")]
    [CMutator("mutate_via")]
    [Description("Mutates Via header for GET_PARAMETER")]
    public class RtspMutateGetParameterVia : LLMMutator
    {
        public RtspMutateGetParameterVia(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("get_parameter");
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
            return obj.IsIn("get_parameter") && obj.name == "accept";
        }

        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            obj.MutatedValue = new Variant("");
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
            return obj.IsIn("get_parameter") && obj.name == "accept";
        }

        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            obj.MutatedValue = new Variant("application/sdp, */*;q=0.1, text/plain");
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
            return obj.IsIn("get_parameter") && obj.name == "accept";
        }

        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
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
            return obj.IsIn("get_parameter");
        }

        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.name == "accept_encoding" || obj.name.Contains("encoding"))
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
            return obj.IsIn("get_parameter");
        }

        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.name == "accept_encoding" || obj.name.Contains("encoding"))
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
            return obj.IsIn("get_parameter");
        }

        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.name == "accept_encoding" || obj.name.Contains("encoding"))
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
            return obj.IsIn("get_parameter");
        }

        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.name == "accept_language" || obj.name.Contains("language"))
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
            return obj.IsIn("get_parameter");
        }

        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.name == "accept_language" || obj.name.Contains("language"))
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
            return obj.IsIn("get_parameter");
        }

        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.name == "accept_language" || obj.name.Contains("language"))
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
    [Mutator("DeleteGetParameterAuthorization")]
    [CMutator("delete_authorization")]
    [Description("Deletes Authorization header for GET_PARAMETER")]
    public class RtspDeleteGetParameterAuthorization : LLMMutator
    {
        public RtspDeleteGetParameterAuthorization(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("get_parameter");
        }

        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.name == "authorization" || obj.name.Contains("Authorization"))
            {
                obj.MutatedValue = new Variant("");
            }
        }
    }

    // C Function: repeat_authorization
    [Mutator("RepeatGetParameterAuthorization")]
    [CMutator("repeat_authorization")]
    [Description("Repeats Authorization header for GET_PARAMETER")]
    public class RtspRepeatGetParameterAuthorization : LLMMutator
    {
        public RtspRepeatGetParameterAuthorization(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("get_parameter");
        }

        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.name == "authorization" || obj.name.Contains("Authorization"))
            {
                obj.MutatedValue = new Variant("Basic dXNlcjpwYXNz, Basic Zm9vOmJhcg==, Basic Og==");
            }
        }
    }

    // C Function: mutate_authorization
    [Mutator("MutateGetParameterAuthorization")]
    [CMutator("mutate_authorization")]
    [Description("Mutates Authorization header for GET_PARAMETER")]
    public class RtspMutateGetParameterAuthorization : LLMMutator
    {
        public RtspMutateGetParameterAuthorization(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("get_parameter");
        }

        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.name == "authorization" || obj.name.Contains("Authorization"))
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

    // ==================== Bandwidth ====================
    
    // C Function: delete_bandwidth
    [Mutator("DeleteGetParameterBandwidth")]
    [CMutator("delete_bandwidth")]
    [Description("Deletes Bandwidth header for GET_PARAMETER")]
    public class RtspDeleteGetParameterBandwidth : LLMMutator
    {
        public RtspDeleteGetParameterBandwidth(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("get_parameter");
        }

        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.name == "bandwidth" || obj.name.Contains("Bandwidth"))
            {
                obj.MutatedValue = new Variant("");
            }
        }
    }

    // C Function: repeat_bandwidth
    [Mutator("RepeatGetParameterBandwidth")]
    [CMutator("repeat_bandwidth")]
    [Description("Repeats Bandwidth header for GET_PARAMETER")]
    public class RtspRepeatGetParameterBandwidth : LLMMutator
    {
        public RtspRepeatGetParameterBandwidth(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("get_parameter");
        }

        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.name == "bandwidth" || obj.name.Contains("Bandwidth"))
            {
                obj.MutatedValue = new Variant("1000");
            }
        }
    }

    // C Function: mutate_bandwidth
    [Mutator("MutateGetParameterBandwidth")]
    [CMutator("mutate_bandwidth")]
    [Description("Mutates Bandwidth header for GET_PARAMETER")]
    public class RtspMutateGetParameterBandwidth : LLMMutator
    {
        public RtspMutateGetParameterBandwidth(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("get_parameter");
        }

        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.name == "bandwidth" || obj.name.Contains("Bandwidth"))
            {
                var random = RtspUtils.GetRandom();
                int[] weights = { 100, 100, 0, 0, 0, 0, 100, 100, 100, 0, 0, 0, 0 };
                
                int opIdx = RtspUtils.WeightedPickIdx(weights, weights.Length, random);
                string mutatedValue = "";

                // Get current value
                string currentValue = obj.DefaultValue?.ToString() ?? "5000";
                int currentNum = 5000;
                int.TryParse(currentValue, out currentNum);

                switch (opIdx)
                {
                    case 0: // bw_valid_typical
                        mutatedValue = "5000";
                        break;
                    case 1: // bw_zero
                        mutatedValue = "0";
                        break;
                    case 6: // bw_small_random
                        mutatedValue = random.Next(1024).ToString();
                        break;
                    case 7: // bw_scale_up
                        long scaledUp = (long)currentNum * (1 + random.Next(8));
                        if (scaledUp > int.MaxValue) scaledUp = int.MaxValue;
                        mutatedValue = scaledUp.ToString();
                        break;
                    case 8: // bw_scale_down
                        int divisor = 1 + random.Next(8);
                        mutatedValue = (currentNum / divisor).ToString();
                        break;
                    default:
                        mutatedValue = "5000";
                        break;
                }

                obj.MutatedValue = new Variant(mutatedValue);
            }
        }
    }

    // ==================== Blocksize ====================
    
    // C Function: delete_blocksize
    [Mutator("DeleteGetParameterBlocksize")]
    [CMutator("delete_blocksize")]
    [Description("Deletes Blocksize header for GET_PARAMETER")]
    public class RtspDeleteGetParameterBlocksize : LLMMutator
    {
        public RtspDeleteGetParameterBlocksize(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("get_parameter");
        }

        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.name == "blocksize" || obj.name.Contains("Blocksize"))
            {
                obj.MutatedValue = new Variant("");
            }
        }
    }

    // C Function: repeat_blocksize
    [Mutator("RepeatGetParameterBlocksize")]
    [CMutator("repeat_blocksize")]
    [Description("Repeats Blocksize header for GET_PARAMETER")]
    public class RtspRepeatGetParameterBlocksize : LLMMutator
    {
        public RtspRepeatGetParameterBlocksize(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("get_parameter");
        }

        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.name == "blocksize" || obj.name.Contains("Blocksize"))
            {
                obj.MutatedValue = new Variant("1024");
            }
        }
    }

    // C Function: mutate_blocksize
    [Mutator("MutateGetParameterBlocksize")]
    [CMutator("mutate_blocksize")]
    [Description("Mutates Blocksize header for GET_PARAMETER")]
    public class RtspMutateGetParameterBlocksize : LLMMutator
    {
        public RtspMutateGetParameterBlocksize(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("get_parameter");
        }

        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.name == "blocksize" || obj.name.Contains("Blocksize"))
            {
                var random = RtspUtils.GetRandom();
                int[] weights = { 100, 100, 100, 0, 0, 0, 100, 100, 100, 100, 0, 100, 100, 100, 0, 0, 0, 0 };
                
                int opIdx = RtspUtils.WeightedPickIdx(weights, weights.Length, random);
                string mutatedValue = "";

                // Get current value
                string currentValue = obj.DefaultValue?.ToString() ?? "4096";
                int currentNum = 4096;
                int.TryParse(currentValue, out currentNum);

                switch (opIdx)
                {
                    case 0: // bs_valid_typical
                        mutatedValue = "4096";
                        break;
                    case 1: // bs_zero
                        mutatedValue = "0";
                        break;
                    case 2: // bs_one
                        mutatedValue = "1";
                        break;
                    case 6: // bs_power_of_two
                        int[] powers = { 512, 1024, 2048, 4096, 8192, 16384, 32768, 65536 };
                        mutatedValue = powers[random.Next(powers.Length)].ToString();
                        break;
                    case 7: // bs_odd_unaligned
                        mutatedValue = ((random.Next(8191) * 2) + 1).ToString();
                        break;
                    case 8: // bs_mtu_edge
                        int[] mtus = { 1460, 1472, 1500, 9000 };
                        mutatedValue = mtus[random.Next(mtus.Length)].ToString();
                        break;
                    case 9: // bs_ts_like
                        int[] ts = { 188, 376, 564, 752 };
                        mutatedValue = ts[random.Next(ts.Length)].ToString();
                        break;
                    case 11: // bs_small_random
                        mutatedValue = (2 + random.Next(8192)).ToString();
                        break;
                    case 12: // bs_scale_up
                        long scaledUp = (long)currentNum * (2 + random.Next(8));
                        if (scaledUp > int.MaxValue) scaledUp = int.MaxValue;
                        mutatedValue = scaledUp.ToString();
                        break;
                    case 13: // bs_scale_down
                        int divisor = 1 + random.Next(8);
                        mutatedValue = (currentNum / divisor).ToString();
                        break;
                    default:
                        mutatedValue = "4096";
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
            return obj.IsIn("get_parameter");
        }

        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.name == "session" || obj.name.Contains("Session"))
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
            return obj.IsIn("get_parameter");
        }

        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.name == "session" || obj.name.Contains("Session"))
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
            return obj.IsIn("get_parameter");
        }

        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.name == "session" || obj.name.Contains("Session"))
            {
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
                        mutatedValue = Encoding.UTF8.GetString(longId).TrimEnd('\0') + ";timeout=60";
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

    // ==================== User-Agent ====================
    
    // C Function: delete_user_agent
    [Mutator("DeleteGetParameterUserAgent")]
    [CMutator("delete_user_agent")]
    [Description("Deletes User-Agent header for GET_PARAMETER")]
    public class RtspDeleteGetParameterUserAgent : LLMMutator
    {
        public RtspDeleteGetParameterUserAgent(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("get_parameter");
        }

        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.name == "user_agent" || obj.name.Contains("User-Agent"))
            {
                obj.MutatedValue = new Variant("");
            }
        }
    }

    // C Function: repeat_user_agent
    [Mutator("RepeatGetParameterUserAgent")]
    [CMutator("repeat_user_agent")]
    [Description("Repeats User-Agent header for GET_PARAMETER")]
    public class RtspRepeatGetParameterUserAgent : LLMMutator
    {
        public RtspRepeatGetParameterUserAgent(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("get_parameter");
        }

        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.name == "user_agent" || obj.name.Contains("User-Agent"))
            {
                obj.MutatedValue = new Variant("Foo/1.0, Bar/2.0");
            }
        }
    }

    // C Function: mutate_user_agent
    [Mutator("MutateGetParameterUserAgent")]
    [CMutator("mutate_user_agent")]
    [Description("Mutates User-Agent header for GET_PARAMETER")]
    public class RtspMutateGetParameterUserAgent : LLMMutator
    {
        public RtspMutateGetParameterUserAgent(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("get_parameter");
        }

        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.name == "user_agent" || obj.name.Contains("User-Agent"))
            {
                var random = RtspUtils.GetRandom();
                int[] weights = { 100, 0, 100, 0, 100, 0, 100, 0, 0, 0 };
                
                int opIdx = RtspUtils.WeightedPickIdx(weights, weights.Length, random);
                string mutatedValue = "";

                switch (opIdx)
                {
                    case 0: // ua_ok
                        mutatedValue = "VLC/3.0.11";
                        break;
                    case 2: // ua_long
                        byte[] longUa = new byte[400];
                        RtspUtils.MakeRepeatedChar(longUa, 400, (byte)'A', 399);
                        mutatedValue = Encoding.UTF8.GetString(longUa).TrimEnd('\0');
                        break;
                    case 4: // ua_tabs
                        mutatedValue = "App\t/1.2\t(arm64)";
                        break;
                    case 6: // ua_many_products
                        mutatedValue = "A/1 B/2 C/3 D/4";
                        break;
                    default:
                        mutatedValue = "VLC/3.0.11";
                        break;
                }

                obj.MutatedValue = new Variant(mutatedValue);
            }
        }
    }

    // ==================== From ====================
    
    // C Function: delete_from
    [Mutator("DeleteGetParameterFrom")]
    [CMutator("delete_from")]
    [Description("Deletes From header for GET_PARAMETER")]
    public class RtspDeleteGetParameterFrom : LLMMutator
    {
        public RtspDeleteGetParameterFrom(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("get_parameter");
        }

        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.name == "from" || obj.name.Contains("From"))
            {
                obj.MutatedValue = new Variant("");
            }
        }
    }

    // C Function: repeat_from
    [Mutator("RepeatGetParameterFrom")]
    [CMutator("repeat_from")]
    [Description("Repeats From header for GET_PARAMETER")]
    public class RtspRepeatGetParameterFrom : LLMMutator
    {
        public RtspRepeatGetParameterFrom(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("get_parameter");
        }

        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.name == "from" || obj.name.Contains("From"))
            {
                obj.MutatedValue = new Variant("<sip:a@b>, <sip:c@d>");
            }
        }
    }

    // C Function: mutate_from
    [Mutator("MutateGetParameterFrom")]
    [CMutator("mutate_from")]
    [Description("Mutates From header for GET_PARAMETER")]
    public class RtspMutateGetParameterFrom : LLMMutator
    {
        public RtspMutateGetParameterFrom(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("get_parameter");
        }

        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.name == "from" || obj.name.Contains("From"))
            {
                var random = RtspUtils.GetRandom();
                int[] weights = { 100, 100, 100, 0, 0, 0, 0, 100, 100, 0 };
                
                int opIdx = RtspUtils.WeightedPickIdx(weights, weights.Length, random);
                string mutatedValue = "";

                switch (opIdx)
                {
                    case 0: // fr_ok_sip
                        mutatedValue = "<sip:user@example.com>";
                        break;
                    case 1: // fr_ok_mailto
                        mutatedValue = "<mailto:user@example.com>";
                        break;
                    case 2: // fr_no_angle
                        mutatedValue = "sip:user@example.com";
                        break;
                    case 7: // fr_long_uri
                        byte[] longUri = new byte[256];
                        longUri[0] = (byte)'<';
                        RtspUtils.MakeRepeatedChar(longUri, 254, (byte)'A', 252);
                        longUri[253] = (byte)'>';
                        mutatedValue = Encoding.UTF8.GetString(longUri).TrimEnd('\0');
                        break;
                    case 8: // fr_inject_comma_list
                        mutatedValue = "<sip:a@x>, <sip:b@y>";
                        break;
                    default:
                        mutatedValue = "<sip:user@example.com>";
                        break;
                }

                obj.MutatedValue = new Variant(mutatedValue);
            }
        }
    }

    // ==================== Proxy-Require ====================
    
    // C Function: delete_proxy_require
    [Mutator("DeleteGetParameterProxyRequire")]
    [CMutator("delete_proxy_require")]
    [Description("Deletes Proxy-Require header for GET_PARAMETER")]
    public class RtspDeleteGetParameterProxyRequire : LLMMutator
    {
        public RtspDeleteGetParameterProxyRequire(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("get_parameter");
        }

        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.name == "proxy_require" || obj.name.Contains("Proxy-Require"))
            {
                obj.MutatedValue = new Variant("");
            }
        }
    }

    // C Function: repeat_proxy_require
    [Mutator("RepeatGetParameterProxyRequire")]
    [CMutator("repeat_proxy_require")]
    [Description("Repeats Proxy-Require header for GET_PARAMETER")]
    public class RtspRepeatGetParameterProxyRequire : LLMMutator
    {
        public RtspRepeatGetParameterProxyRequire(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("get_parameter");
        }

        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.name == "proxy_require" || obj.name.Contains("Proxy-Require"))
            {
                obj.MutatedValue = new Variant("play.basic, funky.ext, foo");
            }
        }
    }

    // C Function: mutate_proxy_require
    [Mutator("MutateGetParameterProxyRequire")]
    [CMutator("mutate_proxy_require")]
    [Description("Mutates Proxy-Require header for GET_PARAMETER")]
    public class RtspMutateGetParameterProxyRequire : LLMMutator
    {
        public RtspMutateGetParameterProxyRequire(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("get_parameter");
        }

        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.name == "proxy_require" || obj.name.Contains("Proxy-Require"))
            {
                var random = RtspUtils.GetRandom();
                int[] weights = { 100, 0, 0, 0, 100, 0, 100, 0 };
                
                int opIdx = RtspUtils.WeightedPickIdx(weights, weights.Length, random);
                string mutatedValue = "";

                switch (opIdx)
                {
                    case 0: // pr_ok_multi
                        mutatedValue = "play.basic, com.vendor.feature";
                        break;
                    case 4: // pr_long_tag
                        byte[] longTag = new byte[256];
                        RtspUtils.MakeRepeatedChar(longTag, 256, (byte)'A', 255);
                        mutatedValue = Encoding.UTF8.GetString(longTag).TrimEnd('\0');
                        break;
                    case 6: // pr_space_list
                        mutatedValue = "  a  ,   b  ,c ";
                        break;
                    default:
                        mutatedValue = "play.basic, com.vendor.feature";
                        break;
                }

                obj.MutatedValue = new Variant(mutatedValue);
            }
        }
    }

    // ==================== Referer ====================
    
    // C Function: delete_referer
    [Mutator("DeleteGetParameterReferer")]
    [CMutator("delete_referer")]
    [Description("Deletes Referer header for GET_PARAMETER")]
    public class RtspDeleteGetParameterReferer : LLMMutator
    {
        public RtspDeleteGetParameterReferer(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("get_parameter");
        }

        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.name == "referer" || obj.name.Contains("Referer"))
            {
                obj.MutatedValue = new Variant("");
            }
        }
    }

    // C Function: repeat_referer
    [Mutator("RepeatGetParameterReferer")]
    [CMutator("repeat_referer")]
    [Description("Repeats Referer header for GET_PARAMETER")]
    public class RtspRepeatGetParameterReferer : LLMMutator
    {
        public RtspRepeatGetParameterReferer(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("get_parameter");
        }

        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.name == "referer" || obj.name.Contains("Referer"))
            {
                obj.MutatedValue = new Variant("rtsp://a/1, rtsp://b/2");
            }
        }
    }

    // C Function: mutate_referer
    [Mutator("MutateGetParameterReferer")]
    [CMutator("mutate_referer")]
    [Description("Mutates Referer header for GET_PARAMETER")]
    public class RtspMutateGetParameterReferer : LLMMutator
    {
        public RtspMutateGetParameterReferer(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("get_parameter");
        }

        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.name == "referer" || obj.name.Contains("Referer"))
            {
                var random = RtspUtils.GetRandom();
                int[] weights = { 100, 100, 100, 0, 0, 0, 0, 100, 100, 100, 0 };
                
                int opIdx = RtspUtils.WeightedPickIdx(weights, weights.Length, random);
                string mutatedValue = "";

                switch (opIdx)
                {
                    case 0: // rf_ok_rtsp
                        mutatedValue = "rtsp://host/prev";
                        break;
                    case 1: // rf_ok_http
                        mutatedValue = "http://host/page";
                        break;
                    case 2: // rf_no_schema
                        mutatedValue = "//host/path";
                        break;
                    case 7: // rf_long_uri
                        byte[] longUri = new byte[300];
                        longUri[0] = (byte)'r'; longUri[1] = (byte)'t'; longUri[2] = (byte)'s'; 
                        longUri[3] = (byte)'p'; longUri[4] = (byte)':'; longUri[5] = (byte)'/'; 
                        longUri[6] = (byte)'/';
                        RtspUtils.MakeRepeatedChar(longUri, 293, (byte)'A', 293);
                        mutatedValue = Encoding.UTF8.GetString(longUri).TrimEnd('\0');
                        break;
                    case 8: // rf_quoted
                        mutatedValue = "\"rtsp://host/with space\"";
                        break;
                    case 9: // rf_two_values
                        mutatedValue = "rtsp://a, http://b";
                        break;
                    default:
                        mutatedValue = "rtsp://host/prev";
                        break;
                }

                obj.MutatedValue = new Variant(mutatedValue);
            }
        }
    }

    // ==================== Require ====================
    
    // C Function: delete_require
    [Mutator("DeleteGetParameterRequire")]
    [CMutator("delete_require")]
    [Description("Deletes Require header for GET_PARAMETER")]
    public class RtspDeleteGetParameterRequire : LLMMutator
    {
        public RtspDeleteGetParameterRequire(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("get_parameter");
        }

        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.name == "require" || obj.name.Contains("Require"))
            {
                obj.MutatedValue = new Variant("");
            }
        }
    }

    // C Function: repeat_require
    [Mutator("RepeatGetParameterRequire")]
    [CMutator("repeat_require")]
    [Description("Repeats Require header for GET_PARAMETER")]
    public class RtspRepeatGetParameterRequire : LLMMutator
    {
        public RtspRepeatGetParameterRequire(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("get_parameter");
        }

        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.name == "require" || obj.name.Contains("Require"))
            {
                obj.MutatedValue = new Variant("implicit-play, com.foo.bar, x");
            }
        }
    }

    // C Function: mutate_require
    [Mutator("MutateGetParameterRequire")]
    [CMutator("mutate_require")]
    [Description("Mutates Require header for GET_PARAMETER")]
    public class RtspMutateGetParameterRequire : LLMMutator
    {
        public RtspMutateGetParameterRequire(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("get_parameter");
        }

        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.name == "require" || obj.name.Contains("Require"))
            {
                var random = RtspUtils.GetRandom();
                int[] weights = { 100, 100, 0, 0, 0, 0, 100, 0, 100, 0 };
                
                int opIdx = RtspUtils.WeightedPickIdx(weights, weights.Length, random);
                string mutatedValue = "";

                switch (opIdx)
                {
                    case 0: // rq_ok_one
                        mutatedValue = "implicit-play";
                        break;
                    case 1: // rq_ok_multi
                        mutatedValue = "com.vendor.feature,play.basic";
                        break;
                    case 6: // rq_long_tag
                        byte[] longTag = new byte[256];
                        RtspUtils.MakeRepeatedChar(longTag, 256, (byte)'R', 255);
                        mutatedValue = Encoding.UTF8.GetString(longTag).TrimEnd('\0');
                        break;
                    case 8: // rq_spaces_list
                        mutatedValue = "  a ,   b, c  ";
                        break;
                    default:
                        mutatedValue = "implicit-play";
                        break;
                }

                obj.MutatedValue = new Variant(mutatedValue);
            }
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
            return obj.IsIn("get_parameter");
        }

        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.name == "content_base" || obj.name.Contains("Content-Base"))
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
            return obj.IsIn("get_parameter");
        }

        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.name == "content_base" || obj.name.Contains("Content-Base"))
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
            return obj.IsIn("get_parameter");
        }

        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.name == "content_base" || obj.name.Contains("Content-Base"))
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
            return obj.IsIn("get_parameter");
        }

        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.name == "content_length" || obj.name.Contains("Content-Length"))
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
            return obj.IsIn("get_parameter");
        }

        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.name == "content_length" || obj.name.Contains("Content-Length"))
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
            return obj.IsIn("get_parameter");
        }

        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.name == "content_length" || obj.name.Contains("Content-Length"))
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
            return obj.IsIn("get_parameter");
        }

        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.name == "content_location" || obj.name.Contains("Content-Location"))
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
            return obj.IsIn("get_parameter");
        }

        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.name == "content_location" || obj.name.Contains("Content-Location"))
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
            return obj.IsIn("get_parameter");
        }

        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.name == "content_location" || obj.name.Contains("Content-Location"))
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
            return obj.IsIn("get_parameter");
        }

        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.name == "last_modified" || obj.name.Contains("Last-Modified"))
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
            return obj.IsIn("get_parameter");
        }

        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.name == "last_modified" || obj.name.Contains("Last-Modified"))
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
            return obj.IsIn("get_parameter");
        }

        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.name == "last_modified" || obj.name.Contains("Last-Modified"))
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

