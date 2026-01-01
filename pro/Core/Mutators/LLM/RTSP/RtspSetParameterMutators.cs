using System;
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
            return obj.IsIn("set_parameter");
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
    [Mutator("RepeatSetParameterConnection")]
    [CMutator("repeat_connection")]
    [Description("Repeats Connection header for SET_PARAMETER")]
    public class RtspRepeatSetParameterConnection : LLMMutator
    {
        public RtspRepeatSetParameterConnection(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("set_parameter");
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
    [Mutator("MutateSetParameterConnection")]
    [CMutator("mutate_connection")]
    [Description("Mutates Connection header for SET_PARAMETER")]
    public class RtspMutateSetParameterConnection : LLMMutator
    {
        public RtspMutateSetParameterConnection(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("set_parameter");
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
    [Mutator("DeleteSetParameterDate")]
    [CMutator("delete_date")]
    [Description("Deletes Date header for SET_PARAMETER")]
    public class RtspDeleteSetParameterDate : LLMMutator
    {
        public RtspDeleteSetParameterDate(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("set_parameter");
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
    [Mutator("RepeatSetParameterDate")]
    [CMutator("repeat_date")]
    [Description("Repeats Date header for SET_PARAMETER")]
    public class RtspRepeatSetParameterDate : LLMMutator
    {
        public RtspRepeatSetParameterDate(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("set_parameter");
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
    [Mutator("MutateSetParameterDate")]
    [CMutator("mutate_date")]
    [Description("Mutates Date header for SET_PARAMETER")]
    public class RtspMutateSetParameterDate : LLMMutator
    {
        public RtspMutateSetParameterDate(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("set_parameter");
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
            return obj.IsIn("set_parameter");
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
    [Mutator("RepeatSetParameterVia")]
    [CMutator("repeat_via")]
    [Description("Repeats Via header for SET_PARAMETER")]
    public class RtspRepeatSetParameterVia : LLMMutator
    {
        public RtspRepeatSetParameterVia(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("set_parameter");
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
    [Mutator("MutateSetParameterVia")]
    [CMutator("mutate_via")]
    [Description("Mutates Via header for SET_PARAMETER")]
    public class RtspMutateSetParameterVia : LLMMutator
    {
        public RtspMutateSetParameterVia(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("set_parameter");
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
            return obj.IsIn("set_parameter");
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
    [Mutator("RepeatSetParameterAcceptLanguage")]
    [CMutator("repeat_accept_language")]
    [Description("Repeats Accept-Language header for SET_PARAMETER")]
    public class RtspRepeatSetParameterAcceptLanguage : LLMMutator
    {
        public RtspRepeatSetParameterAcceptLanguage(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("set_parameter");
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
    [Mutator("MutateSetParameterAcceptLanguage")]
    [CMutator("mutate_accept_language")]
    [Description("Mutates Accept-Language header for SET_PARAMETER")]
    public class RtspMutateSetParameterAcceptLanguage : LLMMutator
    {
        public RtspMutateSetParameterAcceptLanguage(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("set_parameter");
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
    [Mutator("DeleteSetParameterAuthorization")]
    [CMutator("delete_authorization")]
    [Description("Deletes Authorization header for SET_PARAMETER")]
    public class RtspDeleteSetParameterAuthorization : LLMMutator
    {
        public RtspDeleteSetParameterAuthorization(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("set_parameter");
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
    [Mutator("RepeatSetParameterAuthorization")]
    [CMutator("repeat_authorization")]
    [Description("Repeats Authorization header for SET_PARAMETER")]
    public class RtspRepeatSetParameterAuthorization : LLMMutator
    {
        public RtspRepeatSetParameterAuthorization(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("set_parameter");
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
    [Mutator("MutateSetParameterAuthorization")]
    [CMutator("mutate_authorization")]
    [Description("Mutates Authorization header for SET_PARAMETER")]
    public class RtspMutateSetParameterAuthorization : LLMMutator
    {
        public RtspMutateSetParameterAuthorization(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("set_parameter");
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
    [Mutator("DeleteSetParameterBandwidth")]
    [CMutator("delete_bandwidth")]
    [Description("Deletes Bandwidth header for SET_PARAMETER")]
    public class RtspDeleteSetParameterBandwidth : LLMMutator
    {
        public RtspDeleteSetParameterBandwidth(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("set_parameter");
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
    [Mutator("RepeatSetParameterBandwidth")]
    [CMutator("repeat_bandwidth")]
    [Description("Repeats Bandwidth header for SET_PARAMETER")]
    public class RtspRepeatSetParameterBandwidth : LLMMutator
    {
        public RtspRepeatSetParameterBandwidth(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("set_parameter");
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
    [Mutator("MutateSetParameterBandwidth")]
    [CMutator("mutate_bandwidth")]
    [Description("Mutates Bandwidth header for SET_PARAMETER")]
    public class RtspMutateSetParameterBandwidth : LLMMutator
    {
        public RtspMutateSetParameterBandwidth(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("set_parameter");
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
    [Mutator("DeleteSetParameterBlocksize")]
    [CMutator("delete_blocksize")]
    [Description("Deletes Blocksize header for SET_PARAMETER")]
    public class RtspDeleteSetParameterBlocksize : LLMMutator
    {
        public RtspDeleteSetParameterBlocksize(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("set_parameter");
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
    [Mutator("RepeatSetParameterBlocksize")]
    [CMutator("repeat_blocksize")]
    [Description("Repeats Blocksize header for SET_PARAMETER")]
    public class RtspRepeatSetParameterBlocksize : LLMMutator
    {
        public RtspRepeatSetParameterBlocksize(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("set_parameter");
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
    [Mutator("MutateSetParameterBlocksize")]
    [CMutator("mutate_blocksize")]
    [Description("Mutates Blocksize header for SET_PARAMETER")]
    public class RtspMutateSetParameterBlocksize : LLMMutator
    {
        public RtspMutateSetParameterBlocksize(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("set_parameter");
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
            return obj.IsIn("set_parameter");
        }

        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.name == "content_encoding" || obj.name.Contains("Content-Encoding"))
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
            return obj.IsIn("set_parameter");
        }

        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.name == "content_encoding" || obj.name.Contains("Content-Encoding"))
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
            return obj.IsIn("set_parameter");
        }

        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.name == "content_encoding" || obj.name.Contains("Content-Encoding"))
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
            return obj.IsIn("set_parameter");
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
    [Mutator("RepeatSetParameterContentLength")]
    [CMutator("repeat_content_length")]
    [Description("Repeats Content-Length header for SET_PARAMETER")]
    public class RtspRepeatSetParameterContentLength : LLMMutator
    {
        public RtspRepeatSetParameterContentLength(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("set_parameter");
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
    [Mutator("MutateSetParameterContentLength")]
    [CMutator("mutate_content_length")]
    [Description("Mutates Content-Length header for SET_PARAMETER")]
    public class RtspMutateSetParameterContentLength : LLMMutator
    {
        public RtspMutateSetParameterContentLength(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("set_parameter");
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
            return obj.IsIn("set_parameter");
        }

        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.name == "content_type" || obj.name.Contains("Content-Type"))
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
            return obj.IsIn("set_parameter");
        }

        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.name == "content_type" || obj.name.Contains("Content-Type"))
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
            return obj.IsIn("set_parameter");
        }

        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.name == "content_type" || obj.name.Contains("Content-Type"))
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
                        mutatedValue = Encoding.UTF8.GetString(mt).TrimEnd('\0') + "/" + Encoding.UTF8.GetString(st).TrimEnd('\0');
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

    // ==================== From ====================
    
    // C Function: delete_from
    [Mutator("DeleteSetParameterFrom")]
    [CMutator("delete_from")]
    [Description("Deletes From header for SET_PARAMETER")]
    public class RtspDeleteSetParameterFrom : LLMMutator
    {
        public RtspDeleteSetParameterFrom(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("set_parameter");
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
    [Mutator("RepeatSetParameterFrom")]
    [CMutator("repeat_from")]
    [Description("Repeats From header for SET_PARAMETER")]
    public class RtspRepeatSetParameterFrom : LLMMutator
    {
        public RtspRepeatSetParameterFrom(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("set_parameter");
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
    [Mutator("MutateSetParameterFrom")]
    [CMutator("mutate_from")]
    [Description("Mutates From header for SET_PARAMETER")]
    public class RtspMutateSetParameterFrom : LLMMutator
    {
        public RtspMutateSetParameterFrom(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("set_parameter");
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
    [Mutator("DeleteSetParameterProxyRequire")]
    [CMutator("delete_proxy_require")]
    [Description("Deletes Proxy-Require header for SET_PARAMETER")]
    public class RtspDeleteSetParameterProxyRequire : LLMMutator
    {
        public RtspDeleteSetParameterProxyRequire(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("set_parameter");
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
    [Mutator("RepeatSetParameterProxyRequire")]
    [CMutator("repeat_proxy_require")]
    [Description("Repeats Proxy-Require header for SET_PARAMETER")]
    public class RtspRepeatSetParameterProxyRequire : LLMMutator
    {
        public RtspRepeatSetParameterProxyRequire(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("set_parameter");
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
    [Mutator("MutateSetParameterProxyRequire")]
    [CMutator("mutate_proxy_require")]
    [Description("Mutates Proxy-Require header for SET_PARAMETER")]
    public class RtspMutateSetParameterProxyRequire : LLMMutator
    {
        public RtspMutateSetParameterProxyRequire(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("set_parameter");
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
    [Mutator("DeleteSetParameterReferer")]
    [CMutator("delete_referer")]
    [Description("Deletes Referer header for SET_PARAMETER")]
    public class RtspDeleteSetParameterReferer : LLMMutator
    {
        public RtspDeleteSetParameterReferer(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("set_parameter");
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
    [Mutator("RepeatSetParameterReferer")]
    [CMutator("repeat_referer")]
    [Description("Repeats Referer header for SET_PARAMETER")]
    public class RtspRepeatSetParameterReferer : LLMMutator
    {
        public RtspRepeatSetParameterReferer(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("set_parameter");
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
    [Mutator("MutateSetParameterReferer")]
    [CMutator("mutate_referer")]
    [Description("Mutates Referer header for SET_PARAMETER")]
    public class RtspMutateSetParameterReferer : LLMMutator
    {
        public RtspMutateSetParameterReferer(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("set_parameter");
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
    [Mutator("DeleteSetParameterRequire")]
    [CMutator("delete_require")]
    [Description("Deletes Require header for SET_PARAMETER")]
    public class RtspDeleteSetParameterRequire : LLMMutator
    {
        public RtspDeleteSetParameterRequire(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("set_parameter");
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
    [Mutator("RepeatSetParameterRequire")]
    [CMutator("repeat_require")]
    [Description("Repeats Require header for SET_PARAMETER")]
    public class RtspRepeatSetParameterRequire : LLMMutator
    {
        public RtspRepeatSetParameterRequire(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("set_parameter");
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
    [Mutator("MutateSetParameterRequire")]
    [CMutator("mutate_require")]
    [Description("Mutates Require header for SET_PARAMETER")]
    public class RtspMutateSetParameterRequire : LLMMutator
    {
        public RtspMutateSetParameterRequire(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("set_parameter");
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
            return obj.IsIn("set_parameter");
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
    [Mutator("RepeatSetParameterSession")]
    [CMutator("repeat_session")]
    [Description("Repeats Session header for SET_PARAMETER")]
    public class RtspRepeatSetParameterSession : LLMMutator
    {
        public RtspRepeatSetParameterSession(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("set_parameter");
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
    [Mutator("MutateSetParameterSession")]
    [CMutator("mutate_session")]
    [Description("Mutates Session header for SET_PARAMETER")]
    public class RtspMutateSetParameterSession : LLMMutator
    {
        public RtspMutateSetParameterSession(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("set_parameter");
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
    [Mutator("DeleteSetParameterUserAgent")]
    [CMutator("delete_user_agent")]
    [Description("Deletes User-Agent header for SET_PARAMETER")]
    public class RtspDeleteSetParameterUserAgent : LLMMutator
    {
        public RtspDeleteSetParameterUserAgent(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("set_parameter");
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
    [Mutator("RepeatSetParameterUserAgent")]
    [CMutator("repeat_user_agent")]
    [Description("Repeats User-Agent header for SET_PARAMETER")]
    public class RtspRepeatSetParameterUserAgent : LLMMutator
    {
        public RtspRepeatSetParameterUserAgent(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("set_parameter");
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
    [Mutator("MutateSetParameterUserAgent")]
    [CMutator("mutate_user_agent")]
    [Description("Mutates User-Agent header for SET_PARAMETER")]
    public class RtspMutateSetParameterUserAgent : LLMMutator
    {
        public RtspMutateSetParameterUserAgent(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("set_parameter");
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
}

