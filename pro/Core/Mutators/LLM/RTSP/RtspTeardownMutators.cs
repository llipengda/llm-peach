using System;
using System.Text;
using Peach.Core;
using Peach.Core.Dom;

namespace Peach.Pro.Core.Mutators.LLM.RTSP
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
            return obj.IsIn("teardown");
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
    [Mutator("RepeatTeardownConnection")]
    [CMutator("repeat_connection")]
    [Description("Repeats Connection header for TEARDOWN")]
    public class RtspRepeatTeardownConnection : LLMMutator
    {
        public RtspRepeatTeardownConnection(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("teardown");
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
    [Mutator("MutateTeardownConnection")]
    [CMutator("mutate_connection")]
    [Description("Mutates Connection header for TEARDOWN")]
    public class RtspMutateTeardownConnection : LLMMutator
    {
        public RtspMutateTeardownConnection(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("teardown");
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
    [Mutator("DeleteTeardownDate")]
    [CMutator("delete_date")]
    [Description("Deletes Date header for TEARDOWN")]
    public class RtspDeleteTeardownDate : LLMMutator
    {
        public RtspDeleteTeardownDate(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("teardown");
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
    [Mutator("RepeatTeardownDate")]
    [CMutator("repeat_date")]
    [Description("Repeats Date header for TEARDOWN")]
    public class RtspRepeatTeardownDate : LLMMutator
    {
        public RtspRepeatTeardownDate(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("teardown");
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
    [Mutator("MutateTeardownDate")]
    [CMutator("mutate_date")]
    [Description("Mutates Date header for TEARDOWN")]
    public class RtspMutateTeardownDate : LLMMutator
    {
        public RtspMutateTeardownDate(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("teardown");
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
    [Mutator("DeleteTeardownVia")]
    [CMutator("delete_via")]
    [Description("Deletes Via header for TEARDOWN")]
    public class RtspDeleteTeardownVia : LLMMutator
    {
        public RtspDeleteTeardownVia(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("teardown");
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
    [Mutator("RepeatTeardownVia")]
    [CMutator("repeat_via")]
    [Description("Repeats Via header for TEARDOWN")]
    public class RtspRepeatTeardownVia : LLMMutator
    {
        public RtspRepeatTeardownVia(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("teardown");
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
    [Mutator("MutateTeardownVia")]
    [CMutator("mutate_via")]
    [Description("Mutates Via header for TEARDOWN")]
    public class RtspMutateTeardownVia : LLMMutator
    {
        public RtspMutateTeardownVia(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("teardown");
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
    [Mutator("DeleteTeardownAcceptLanguage")]
    [CMutator("delete_accept_language")]
    [Description("Deletes Accept-Language header for TEARDOWN")]
    public class RtspDeleteTeardownAcceptLanguage : LLMMutator
    {
        public RtspDeleteTeardownAcceptLanguage(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("teardown");
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
    [Mutator("RepeatTeardownAcceptLanguage")]
    [CMutator("repeat_accept_language")]
    [Description("Repeats Accept-Language header for TEARDOWN")]
    public class RtspRepeatTeardownAcceptLanguage : LLMMutator
    {
        public RtspRepeatTeardownAcceptLanguage(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("teardown");
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
    [Mutator("MutateTeardownAcceptLanguage")]
    [CMutator("mutate_accept_language")]
    [Description("Mutates Accept-Language header for TEARDOWN")]
    public class RtspMutateTeardownAcceptLanguage : LLMMutator
    {
        public RtspMutateTeardownAcceptLanguage(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("teardown");
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
    [Mutator("DeleteTeardownAuthorization")]
    [CMutator("delete_authorization")]
    [Description("Deletes Authorization header for TEARDOWN")]
    public class RtspDeleteTeardownAuthorization : LLMMutator
    {
        public RtspDeleteTeardownAuthorization(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("teardown");
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
    [Mutator("RepeatTeardownAuthorization")]
    [CMutator("repeat_authorization")]
    [Description("Repeats Authorization header for TEARDOWN")]
    public class RtspRepeatTeardownAuthorization : LLMMutator
    {
        public RtspRepeatTeardownAuthorization(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("teardown");
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
    [Mutator("MutateTeardownAuthorization")]
    [CMutator("mutate_authorization")]
    [Description("Mutates Authorization header for TEARDOWN")]
    public class RtspMutateTeardownAuthorization : LLMMutator
    {
        public RtspMutateTeardownAuthorization(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("teardown");
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
    [Mutator("DeleteTeardownBandwidth")]
    [CMutator("delete_bandwidth")]
    [Description("Deletes Bandwidth header for TEARDOWN")]
    public class RtspDeleteTeardownBandwidth : LLMMutator
    {
        public RtspDeleteTeardownBandwidth(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("teardown");
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
    [Mutator("RepeatTeardownBandwidth")]
    [CMutator("repeat_bandwidth")]
    [Description("Repeats Bandwidth header for TEARDOWN")]
    public class RtspRepeatTeardownBandwidth : LLMMutator
    {
        public RtspRepeatTeardownBandwidth(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("teardown");
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
    [Mutator("MutateTeardownBandwidth")]
    [CMutator("mutate_bandwidth")]
    [Description("Mutates Bandwidth header for TEARDOWN")]
    public class RtspMutateTeardownBandwidth : LLMMutator
    {
        public RtspMutateTeardownBandwidth(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("teardown");
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

    // ==================== From ====================
    
    // C Function: delete_from
    [Mutator("DeleteTeardownFrom")]
    [CMutator("delete_from")]
    [Description("Deletes From header for TEARDOWN")]
    public class RtspDeleteTeardownFrom : LLMMutator
    {
        public RtspDeleteTeardownFrom(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("teardown");
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
    [Mutator("RepeatTeardownFrom")]
    [CMutator("repeat_from")]
    [Description("Repeats From header for TEARDOWN")]
    public class RtspRepeatTeardownFrom : LLMMutator
    {
        public RtspRepeatTeardownFrom(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("teardown");
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
    [Mutator("MutateTeardownFrom")]
    [CMutator("mutate_from")]
    [Description("Mutates From header for TEARDOWN")]
    public class RtspMutateTeardownFrom : LLMMutator
    {
        public RtspMutateTeardownFrom(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("teardown");
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
    [Mutator("DeleteTeardownProxyRequire")]
    [CMutator("delete_proxy_require")]
    [Description("Deletes Proxy-Require header for TEARDOWN")]
    public class RtspDeleteTeardownProxyRequire : LLMMutator
    {
        public RtspDeleteTeardownProxyRequire(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("teardown");
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
    [Mutator("RepeatTeardownProxyRequire")]
    [CMutator("repeat_proxy_require")]
    [Description("Repeats Proxy-Require header for TEARDOWN")]
    public class RtspRepeatTeardownProxyRequire : LLMMutator
    {
        public RtspRepeatTeardownProxyRequire(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("teardown");
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
    [Mutator("MutateTeardownProxyRequire")]
    [CMutator("mutate_proxy_require")]
    [Description("Mutates Proxy-Require header for TEARDOWN")]
    public class RtspMutateTeardownProxyRequire : LLMMutator
    {
        public RtspMutateTeardownProxyRequire(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("teardown");
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
    [Mutator("DeleteTeardownReferer")]
    [CMutator("delete_referer")]
    [Description("Deletes Referer header for TEARDOWN")]
    public class RtspDeleteTeardownReferer : LLMMutator
    {
        public RtspDeleteTeardownReferer(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("teardown");
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
    [Mutator("RepeatTeardownReferer")]
    [CMutator("repeat_referer")]
    [Description("Repeats Referer header for TEARDOWN")]
    public class RtspRepeatTeardownReferer : LLMMutator
    {
        public RtspRepeatTeardownReferer(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("teardown");
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
    [Mutator("MutateTeardownReferer")]
    [CMutator("mutate_referer")]
    [Description("Mutates Referer header for TEARDOWN")]
    public class RtspMutateTeardownReferer : LLMMutator
    {
        public RtspMutateTeardownReferer(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("teardown");
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
    [Mutator("DeleteTeardownRequire")]
    [CMutator("delete_require")]
    [Description("Deletes Require header for TEARDOWN")]
    public class RtspDeleteTeardownRequire : LLMMutator
    {
        public RtspDeleteTeardownRequire(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("teardown");
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
    [Mutator("RepeatTeardownRequire")]
    [CMutator("repeat_require")]
    [Description("Repeats Require header for TEARDOWN")]
    public class RtspRepeatTeardownRequire : LLMMutator
    {
        public RtspRepeatTeardownRequire(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("teardown");
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
    [Mutator("MutateTeardownRequire")]
    [CMutator("mutate_require")]
    [Description("Mutates Require header for TEARDOWN")]
    public class RtspMutateTeardownRequire : LLMMutator
    {
        public RtspMutateTeardownRequire(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("teardown");
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
    [Mutator("DeleteTeardownSession")]
    [CMutator("delete_session")]
    [Description("Deletes Session header for TEARDOWN")]
    public class RtspDeleteTeardownSession : LLMMutator
    {
        public RtspDeleteTeardownSession(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("teardown");
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
    [Mutator("RepeatTeardownSession")]
    [CMutator("repeat_session")]
    [Description("Repeats Session header for TEARDOWN")]
    public class RtspRepeatTeardownSession : LLMMutator
    {
        public RtspRepeatTeardownSession(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("teardown");
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
    [Mutator("MutateTeardownSession")]
    [CMutator("mutate_session")]
    [Description("Mutates Session header for TEARDOWN")]
    public class RtspMutateTeardownSession : LLMMutator
    {
        public RtspMutateTeardownSession(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("teardown");
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
    [Mutator("DeleteTeardownUserAgent")]
    [CMutator("delete_user_agent")]
    [Description("Deletes User-Agent header for TEARDOWN")]
    public class RtspDeleteTeardownUserAgent : LLMMutator
    {
        public RtspDeleteTeardownUserAgent(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("teardown");
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
    [Mutator("RepeatTeardownUserAgent")]
    [CMutator("repeat_user_agent")]
    [Description("Repeats User-Agent header for TEARDOWN")]
    public class RtspRepeatTeardownUserAgent : LLMMutator
    {
        public RtspRepeatTeardownUserAgent(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("teardown");
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
    [Mutator("MutateTeardownUserAgent")]
    [CMutator("mutate_user_agent")]
    [Description("Mutates User-Agent header for TEARDOWN")]
    public class RtspMutateTeardownUserAgent : LLMMutator
    {
        public RtspMutateTeardownUserAgent(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("teardown");
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

