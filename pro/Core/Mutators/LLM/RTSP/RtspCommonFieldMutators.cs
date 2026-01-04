using System;
using System.ComponentModel;
using System.Text;
using Peach.Core;
using Peach.Core.Dom;

namespace Peach.Pro.Core.Mutators.LLM.RTSP
{
    // ==================== Authorization ====================
    
    // C Function: add_authorization
    [Mutator("AddAuthorization")]
    [CMutator("add_authorization")]
    [Description("Adds Authorization header with Basic authentication")]
    public class RtspAddAuthorization : LLMMutator
    {
        public RtspAddAuthorization(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return (obj.IsIn("options") || obj.IsIn("setup") || obj.IsIn("describe") || 
                   obj.IsIn("play") || obj.IsIn("pause") || obj.IsIn("teardown") || 
                   obj.IsIn("get_parameter") || obj.IsIn("set_parameter")) &&
                   (obj.Name == "authorization" || obj.Name.Contains("Authorization"));
        }

        public override int count => 1;

        public override uint mutation { get; set; }

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.Name == "authorization" || obj.Name.Contains("Authorization"))
            {
                obj.MutatedValue = new Variant("Basic dXNlcjpwYXNz");
            }
        }
    }

    // C Function: delete_authorization
    [Mutator("DeleteAuthorization")]
    [CMutator("delete_authorization")]
    [Description("Deletes Authorization header")]
    public class RtspDeleteAuthorization : LLMMutator
    {
        public RtspDeleteAuthorization(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return (obj.IsIn("options") || obj.IsIn("setup") || obj.IsIn("describe") || 
                   obj.IsIn("play") || obj.IsIn("pause") || obj.IsIn("teardown") || 
                   obj.IsIn("get_parameter") || obj.IsIn("set_parameter")) &&
                   (obj.Name == "authorization" || obj.Name.Contains("Authorization"));
        }

        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        public override uint mutation { get; set; }

        private void PerformMutation(DataElement obj)
        {
            if (obj.Name == "authorization" || obj.Name.Contains("Authorization"))
            {
                obj.MutatedValue = new Variant("");
            }
        }
    }

    // C Function: repeat_authorization
    [Mutator("RepeatAuthorization")]
    [CMutator("repeat_authorization")]
    [Description("Repeats Authorization header")]
    public class RtspRepeatAuthorization : LLMMutator
    {
        public RtspRepeatAuthorization(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return (obj.IsIn("options") || obj.IsIn("setup") || obj.IsIn("describe") || 
                   obj.IsIn("play") || obj.IsIn("pause") || obj.IsIn("teardown") || 
                   obj.IsIn("get_parameter") || obj.IsIn("set_parameter")) &&
                   (obj.Name == "authorization" || obj.Name.Contains("Authorization"));
        }
        public override uint mutation { get; set; }
        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.Name == "authorization" || obj.Name.Contains("Authorization"))
            {
                obj.MutatedValue = new Variant("Basic dXNlcjpwYXNz, Basic Zm9vOmJhcg==, Basic Og==");
            }
        }
    }

    // C Function: mutate_authorization
    [Mutator("MutateAuthorization")]
    [CMutator("mutate_authorization")]
    [Description("Mutates Authorization header with various valid and invalid forms")]
    public class RtspMutateAuthorization : LLMMutator
    {
        public RtspMutateAuthorization(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return (obj.IsIn("options") || obj.IsIn("setup") || obj.IsIn("describe") || 
                   obj.IsIn("play") || obj.IsIn("pause") || obj.IsIn("teardown") || 
                   obj.IsIn("get_parameter") || obj.IsIn("set_parameter")) &&
                   (obj.Name == "authorization" || obj.Name.Contains("Authorization"));
        }
        public override uint mutation { get; set; }
        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
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

    // ==================== Bandwidth ====================
    
    // C Function: add_bandwidth
    [Mutator("AddBandwidth")]
    [CMutator("add_bandwidth")]
    [Description("Adds Bandwidth header")]
    public class RtspAddBandwidth : LLMMutator
    {
        public RtspAddBandwidth(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return (obj.IsIn("options") || obj.IsIn("setup") || obj.IsIn("describe") || 
                   obj.IsIn("play") || obj.IsIn("pause") || obj.IsIn("teardown") || 
                   obj.IsIn("get_parameter") || obj.IsIn("set_parameter")) &&
                   (obj.Name == "bandwidth" || obj.Name.Contains("Bandwidth"));
        }
        public override uint mutation { get; set; }
        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.Name == "bandwidth" || obj.Name.Contains("Bandwidth"))
            {
                obj.MutatedValue = new Variant("64000");
            }
        }
    }

    // C Function: delete_bandwidth
    [Mutator("DeleteBandwidth")]
    [CMutator("delete_bandwidth")]
    [Description("Deletes Bandwidth header")]
    public class RtspDeleteBandwidth : LLMMutator
    {
        public RtspDeleteBandwidth(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return (obj.IsIn("options") || obj.IsIn("setup") || obj.IsIn("describe") || 
                   obj.IsIn("play") || obj.IsIn("pause") || obj.IsIn("teardown") || 
                   obj.IsIn("get_parameter") || obj.IsIn("set_parameter")) &&
                   (obj.Name == "bandwidth" || obj.Name.Contains("Bandwidth"));
        }
        public override uint mutation { get; set; }
        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.Name == "bandwidth" || obj.Name.Contains("Bandwidth"))
            {
                obj.MutatedValue = new Variant("");
            }
        }
    }

    // C Function: repeat_bandwidth
    [Mutator("RepeatBandwidth")]
    [CMutator("repeat_bandwidth")]
    [Description("Repeats Bandwidth header")]
    public class RtspRepeatBandwidth : LLMMutator
    {
        public RtspRepeatBandwidth(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return (obj.IsIn("options") || obj.IsIn("setup") || obj.IsIn("describe") || 
                   obj.IsIn("play") || obj.IsIn("pause") || obj.IsIn("teardown") || 
                   obj.IsIn("get_parameter") || obj.IsIn("set_parameter")) &&
                   (obj.Name == "bandwidth" || obj.Name.Contains("Bandwidth"));
        }
        public override uint mutation { get; set; }
        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.Name == "bandwidth" || obj.Name.Contains("Bandwidth"))
            {
                obj.MutatedValue = new Variant("1000");
            }
        }
    }

    // C Function: mutate_bandwidth
    [Mutator("MutateBandwidth")]
    [CMutator("mutate_bandwidth")]
    [Description("Mutates Bandwidth header")]
    public class RtspMutateBandwidth : LLMMutator
    {
        public RtspMutateBandwidth(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return (obj.IsIn("options") || obj.IsIn("setup") || obj.IsIn("describe") || 
                   obj.IsIn("play") || obj.IsIn("pause") || obj.IsIn("teardown") || 
                   obj.IsIn("get_parameter") || obj.IsIn("set_parameter")) &&
                   (obj.Name == "bandwidth" || obj.Name.Contains("Bandwidth"));
        }
        public override uint mutation { get; set; }
        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.Name == "bandwidth" || obj.Name.Contains("Bandwidth"))
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
    
    // C Function: add_blocksize
    [Mutator("AddBlocksize")]
    [CMutator("add_blocksize")]
    [Description("Adds Blocksize header")]
    public class RtspAddBlocksize : LLMMutator
    {
        public RtspAddBlocksize(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return (obj.IsIn("setup") || obj.IsIn("describe") || obj.IsIn("play") || 
                   obj.IsIn("pause") || obj.IsIn("get_parameter") || obj.IsIn("set_parameter") ||
                   obj.IsIn("redirect")) &&
                   (obj.Name == "blocksize" || obj.Name.Contains("Blocksize"));
        }
        public override uint mutation { get; set; }
        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.Name == "blocksize" || obj.Name.Contains("Blocksize"))
            {
                obj.MutatedValue = new Variant("4096");
            }
        }
    }

    // C Function: delete_blocksize
    [Mutator("DeleteBlocksize")]
    [CMutator("delete_blocksize")]
    [Description("Deletes Blocksize header")]
    public class RtspDeleteBlocksize : LLMMutator
    {
        public RtspDeleteBlocksize(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return (obj.IsIn("setup") || obj.IsIn("describe") || obj.IsIn("play") || 
                   obj.IsIn("pause") || obj.IsIn("get_parameter") || obj.IsIn("set_parameter") ||
                   obj.IsIn("redirect")) &&
                   (obj.Name == "blocksize" || obj.Name.Contains("Blocksize"));
        }
        public override uint mutation { get; set; }
        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.Name == "blocksize" || obj.Name.Contains("Blocksize"))
            {
                obj.MutatedValue = new Variant("");
            }
        }
    }

    // C Function: repeat_blocksize
    [Mutator("RepeatBlocksize")]
    [CMutator("repeat_blocksize")]
    [Description("Repeats Blocksize header")]
    public class RtspRepeatBlocksize : LLMMutator
    {
        public RtspRepeatBlocksize(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return (obj.IsIn("setup") || obj.IsIn("describe") || obj.IsIn("play") || 
                   obj.IsIn("pause") || obj.IsIn("get_parameter") || obj.IsIn("set_parameter") ||
                   obj.IsIn("redirect")) &&
                   (obj.Name == "blocksize" || obj.Name.Contains("Blocksize"));
        }
        public override uint mutation { get; set; }
        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.Name == "blocksize" || obj.Name.Contains("Blocksize"))
            {
                obj.MutatedValue = new Variant("1024");
            }
        }
    }

    // C Function: mutate_blocksize
    [Mutator("MutateBlocksize")]
    [CMutator("mutate_blocksize")]
    [Description("Mutates Blocksize header")]
    public class RtspMutateBlocksize : LLMMutator
    {
        public RtspMutateBlocksize(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return (obj.IsIn("setup") || obj.IsIn("describe") || obj.IsIn("play") || 
                   obj.IsIn("pause") || obj.IsIn("get_parameter") || obj.IsIn("set_parameter") ||
                   obj.IsIn("redirect")) &&
                   (obj.Name == "blocksize" || obj.Name.Contains("Blocksize"));
        }
        public override uint mutation { get; set; }
        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.Name == "blocksize" || obj.Name.Contains("Blocksize"))
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
    
    // C Function: add_session
    [Mutator("AddSession")]
    [CMutator("add_session")]
    [Description("Adds Session header")]
    public class RtspAddSession : LLMMutator
    {
        public RtspAddSession(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            // 严格检查：必须是支持的上下文，名称必须是session，且必须是String类型
            return (obj.IsIn("describe") || obj.IsIn("play") || obj.IsIn("pause") || 
                   obj.IsIn("teardown") || obj.IsIn("get_parameter") || obj.IsIn("set_parameter")) &&
                   obj.Name == "session" &&
                   obj is Peach.Core.Dom.String;
        }
        public override uint mutation { get; set; }
        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            // 双重检查：确保是String类型且名称正确
            if (obj.Name == "session" && obj is Peach.Core.Dom.String)
            {
                obj.MutatedValue = new Variant("12345678;timeout=60");
            }
        }
    }

    // C Function: delete_session
    [Mutator("DeleteSession")]
    [CMutator("delete_session")]
    [Description("Deletes Session header")]
    public class RtspDeleteSession : LLMMutator
    {
        public RtspDeleteSession(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            // 严格检查：必须是支持的上下文，名称必须是session，且必须是String类型
            return (obj.IsIn("describe") || obj.IsIn("play") || obj.IsIn("pause") || 
                   obj.IsIn("teardown") || obj.IsIn("get_parameter") || obj.IsIn("set_parameter")) &&
                   obj.Name == "session" &&
                   obj is Peach.Core.Dom.String;
        }
        public override uint mutation { get; set; }
        public override int count => 1;

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
    [Mutator("RepeatSession")]
    [CMutator("repeat_session")]
    [Description("Repeats Session header")]
    public class RtspRepeatSession : LLMMutator
    {
        public RtspRepeatSession(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            // 严格检查：必须是支持的上下文，名称必须是session，且必须是String类型
            return (obj.IsIn("describe") || obj.IsIn("play") || obj.IsIn("pause") || 
                   obj.IsIn("teardown") || obj.IsIn("get_parameter") || obj.IsIn("set_parameter")) &&
                   obj.Name == "session" &&
                   obj is Peach.Core.Dom.String;
        }
        public override uint mutation { get; set; }
        public override int count => 1;

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
    [Mutator("MutateSession")]
    [CMutator("mutate_session")]
    [Description("Mutates Session header")]
    public class RtspMutateSession : LLMMutator
    {
        public RtspMutateSession(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            // 严格检查：必须是支持的上下文，名称必须是session，且必须是String类型
            if (obj == null)
                return false;
            
            if (!(obj.IsIn("describe") || obj.IsIn("play") || obj.IsIn("pause") || 
                  obj.IsIn("teardown") || obj.IsIn("get_parameter") || obj.IsIn("set_parameter")))
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

        private void PerformMutation(DataElement obj)
        {
            // 多重检查：确保类型和名称都正确
            if (obj == null)
                return;
            
            if (obj.Name != "session")
                return;
            
            if (!(obj is Peach.Core.Dom.String))
                return;
            
            // 确保在支持的上下文中
            if (!(obj.IsIn("describe") || obj.IsIn("play") || obj.IsIn("pause") || 
                  obj.IsIn("teardown") || obj.IsIn("get_parameter") || obj.IsIn("set_parameter")))
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

    // ==================== User-Agent ====================
    
    // C Function: add_user_agent
    [Mutator("AddUserAgent")]
    [CMutator("add_user_agent")]
    [Description("Adds User-Agent header")]
    public class RtspAddUserAgent : LLMMutator
    {
        public RtspAddUserAgent(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return (obj.IsIn("options") || obj.IsIn("setup") || obj.IsIn("describe") || 
                   obj.IsIn("play") || obj.IsIn("pause") || obj.IsIn("teardown") || 
                   obj.IsIn("get_parameter") || obj.IsIn("set_parameter")) &&
                   (obj.Name == "user_agent" || obj.Name.Contains("User-Agent"));
        }
        public override uint mutation { get; set; }
        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.Name == "user_agent" || obj.Name.Contains("User-Agent"))
            {
                obj.MutatedValue = new Variant("Live555/0.92");
            }
        }
    }

    // C Function: delete_user_agent
    [Mutator("DeleteUserAgent")]
    [CMutator("delete_user_agent")]
    [Description("Deletes User-Agent header")]
    public class RtspDeleteUserAgent : LLMMutator
    {
        public RtspDeleteUserAgent(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return (obj.IsIn("options") || obj.IsIn("setup") || obj.IsIn("describe") || 
                   obj.IsIn("play") || obj.IsIn("pause") || obj.IsIn("teardown") || 
                   obj.IsIn("get_parameter") || obj.IsIn("set_parameter")) &&
                   (obj.Name == "user_agent" || obj.Name.Contains("User-Agent"));
        }
        public override uint mutation { get; set; }
        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.Name == "user_agent" || obj.Name.Contains("User-Agent"))
            {
                obj.MutatedValue = new Variant("");
            }
        }
    }

    // C Function: repeat_user_agent
    [Mutator("RepeatUserAgent")]
    [CMutator("repeat_user_agent")]
    [Description("Repeats User-Agent header")]
    public class RtspRepeatUserAgent : LLMMutator
    {
        public RtspRepeatUserAgent(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return (obj.IsIn("options") || obj.IsIn("setup") || obj.IsIn("describe") || 
                   obj.IsIn("play") || obj.IsIn("pause") || obj.IsIn("teardown") || 
                   obj.IsIn("get_parameter") || obj.IsIn("set_parameter")) &&
                   (obj.Name == "user_agent" || obj.Name.Contains("User-Agent"));
        }
        public override uint mutation { get; set; }
        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.Name == "user_agent" || obj.Name.Contains("User-Agent"))
            {
                obj.MutatedValue = new Variant("Foo/1.0, Bar/2.0");
            }
        }
    }

    // C Function: mutate_user_agent
    [Mutator("MutateUserAgent")]
    [CMutator("mutate_user_agent")]
    [Description("Mutates User-Agent header")]
    public class RtspMutateUserAgent : LLMMutator
    {
        public RtspMutateUserAgent(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return (obj.IsIn("options") || obj.IsIn("setup") || obj.IsIn("describe") || 
                   obj.IsIn("play") || obj.IsIn("pause") || obj.IsIn("teardown") || 
                   obj.IsIn("get_parameter") || obj.IsIn("set_parameter")) &&
                   (obj.Name == "user_agent" || obj.Name.Contains("User-Agent"));
        }
        public override uint mutation { get; set; }
        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.Name == "user_agent" || obj.Name.Contains("User-Agent"))
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
                        mutatedValue = System.Text.Encoding.UTF8.GetString(longUa).TrimEnd('\0');
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
    
    // C Function: add_from
    [Mutator("AddFrom")]
    [CMutator("add_from")]
    [Description("Adds From header")]
    public class RtspAddFrom : LLMMutator
    {
        public RtspAddFrom(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return (obj.IsIn("options") || obj.IsIn("setup") || obj.IsIn("describe") || 
                   obj.IsIn("play") || obj.IsIn("pause") || obj.IsIn("teardown") || 
                   obj.IsIn("get_parameter") || obj.IsIn("set_parameter") || obj.IsIn("redirect") ||
                   obj.IsIn("announce")) &&
                   (obj.Name == "from" || obj.Name.Contains("From"));
        }
        public override uint mutation { get; set; }
        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.Name == "from" || obj.Name.Contains("From"))
            {
                obj.MutatedValue = new Variant("<sip:user@example.com>");
            }
        }
    }

    // C Function: delete_from
    [Mutator("DeleteFrom")]
    [CMutator("delete_from")]
    [Description("Deletes From header")]
    public class RtspDeleteFrom : LLMMutator
    {
        public RtspDeleteFrom(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return (obj.IsIn("options") || obj.IsIn("setup") || obj.IsIn("describe") || 
                   obj.IsIn("play") || obj.IsIn("pause") || obj.IsIn("teardown") || 
                   obj.IsIn("get_parameter") || obj.IsIn("set_parameter") || obj.IsIn("redirect") ||
                   obj.IsIn("announce")) &&
                   (obj.Name == "from" || obj.Name.Contains("From"));
        }
        public override uint mutation { get; set; }
        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.Name == "from" || obj.Name.Contains("From"))
            {
                obj.MutatedValue = new Variant("");
            }
        }
    }

    // C Function: repeat_from
    [Mutator("RepeatFrom")]
    [CMutator("repeat_from")]
    [Description("Repeats From header")]
    public class RtspRepeatFrom : LLMMutator
    {
        public RtspRepeatFrom(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return (obj.IsIn("options") || obj.IsIn("setup") || obj.IsIn("describe") || 
                   obj.IsIn("play") || obj.IsIn("pause") || obj.IsIn("teardown") || 
                   obj.IsIn("get_parameter") || obj.IsIn("set_parameter") || obj.IsIn("redirect") ||
                   obj.IsIn("announce")) &&
                   (obj.Name == "from" || obj.Name.Contains("From"));
        }
        public override uint mutation { get; set; }
        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.Name == "from" || obj.Name.Contains("From"))
            {
                obj.MutatedValue = new Variant("<sip:a@b>, <sip:c@d>");
            }
        }
    }

    // C Function: mutate_from
    [Mutator("MutateFrom")]
    [CMutator("mutate_from")]
    [Description("Mutates From header")]
    public class RtspMutateFrom : LLMMutator
    {
        public RtspMutateFrom(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return (obj.IsIn("options") || obj.IsIn("setup") || obj.IsIn("describe") || 
                   obj.IsIn("play") || obj.IsIn("pause") || obj.IsIn("teardown") || 
                   obj.IsIn("get_parameter") || obj.IsIn("set_parameter") || obj.IsIn("redirect") ||
                   obj.IsIn("announce")) &&
                   (obj.Name == "from" || obj.Name.Contains("From"));
        }
        public override uint mutation { get; set; }
        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.Name == "from" || obj.Name.Contains("From"))
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
                        mutatedValue = System.Text.Encoding.UTF8.GetString(longUri).TrimEnd('\0');
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
    
    // C Function: add_proxy_require
    [Mutator("AddProxyRequire")]
    [CMutator("add_proxy_require")]
    [Description("Adds Proxy-Require header")]
    public class RtspAddProxyRequire : LLMMutator
    {
        public RtspAddProxyRequire(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return (obj.IsIn("options") || obj.IsIn("setup") || obj.IsIn("describe") || 
                   obj.IsIn("play") || obj.IsIn("pause") || obj.IsIn("teardown") || 
                   obj.IsIn("get_parameter") || obj.IsIn("set_parameter") || obj.IsIn("redirect") ||
                   obj.IsIn("announce")) &&
                   (obj.Name == "proxy_require" || obj.Name.Contains("Proxy-Require"));
        }   
        public override uint mutation { get; set; }
        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.Name == "proxy_require" || obj.Name.Contains("Proxy-Require"))
            {
                obj.MutatedValue = new Variant("play.basic");
            }
        }
    }

    // C Function: delete_proxy_require
    [Mutator("DeleteProxyRequire")]
    [CMutator("delete_proxy_require")]
    [Description("Deletes Proxy-Require header")]
    public class RtspDeleteProxyRequire : LLMMutator
    {
        public RtspDeleteProxyRequire(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return (obj.IsIn("options") || obj.IsIn("setup") || obj.IsIn("describe") || 
                   obj.IsIn("play") || obj.IsIn("pause") || obj.IsIn("teardown") || 
                   obj.IsIn("get_parameter") || obj.IsIn("set_parameter") || obj.IsIn("redirect") ||
                   obj.IsIn("announce")) &&
                   (obj.Name == "proxy_require" || obj.Name.Contains("Proxy-Require"));
        }   
        public override uint mutation { get; set; }
        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.Name == "proxy_require" || obj.Name.Contains("Proxy-Require"))
            {
                obj.MutatedValue = new Variant("");
            }
        }
    }

    // C Function: repeat_proxy_require
    [Mutator("RepeatProxyRequire")]
    [CMutator("repeat_proxy_require")]
    [Description("Repeats Proxy-Require header")]
    public class RtspRepeatProxyRequire : LLMMutator
    {
        public RtspRepeatProxyRequire(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return (obj.IsIn("options") || obj.IsIn("setup") || obj.IsIn("describe") || 
                   obj.IsIn("play") || obj.IsIn("pause") || obj.IsIn("teardown") || 
                   obj.IsIn("get_parameter") || obj.IsIn("set_parameter") || obj.IsIn("redirect") ||
                   obj.IsIn("announce")) &&
                   (obj.Name == "proxy_require" || obj.Name.Contains("Proxy-Require"));
        }
        public override uint mutation { get; set; }
        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.Name == "proxy_require" || obj.Name.Contains("Proxy-Require"))
            {
                obj.MutatedValue = new Variant("play.basic, funky.ext, foo");
            }
        }
    }

    // C Function: mutate_proxy_require
    [Mutator("MutateProxyRequire")]
    [CMutator("mutate_proxy_require")]
    [Description("Mutates Proxy-Require header")]
    public class RtspMutateProxyRequire : LLMMutator
    {
        public RtspMutateProxyRequire(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return (obj.IsIn("options") || obj.IsIn("setup") || obj.IsIn("describe") || 
                   obj.IsIn("play") || obj.IsIn("pause") || obj.IsIn("teardown") || 
                   obj.IsIn("get_parameter") || obj.IsIn("set_parameter") || obj.IsIn("redirect") ||
                   obj.IsIn("announce")) &&
                   (obj.Name == "proxy_require" || obj.Name.Contains("Proxy-Require"));
        }
        public override uint mutation { get; set; }
        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.Name == "proxy_require" || obj.Name.Contains("Proxy-Require"))
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
                        mutatedValue = System.Text.Encoding.UTF8.GetString(longTag).TrimEnd('\0');
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
    
    // C Function: add_referer
    [Mutator("AddReferer")]
    [CMutator("add_referer")]
    [Description("Adds Referer header")]
    public class RtspAddReferer : LLMMutator
    {
        public RtspAddReferer(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return (obj.IsIn("options") || obj.IsIn("setup") || obj.IsIn("describe") || 
                   obj.IsIn("play") || obj.IsIn("pause") || obj.IsIn("teardown") || 
                   obj.IsIn("get_parameter") || obj.IsIn("set_parameter") || obj.IsIn("redirect") ||
                   obj.IsIn("announce")) &&
                   (obj.Name == "referer" || obj.Name.Contains("Referer"));
        }
        public override uint mutation { get; set; }
        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.Name == "referer" || obj.Name.Contains("Referer"))
            {
                obj.MutatedValue = new Variant("rtsp://example.com/prev");
            }
        }
    }

    // C Function: delete_referer
    [Mutator("DeleteReferer")]
    [CMutator("delete_referer")]
    [Description("Deletes Referer header")]
    public class RtspDeleteReferer : LLMMutator
    {
        public RtspDeleteReferer(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return (obj.IsIn("options") || obj.IsIn("setup") || obj.IsIn("describe") || 
                   obj.IsIn("play") || obj.IsIn("pause") || obj.IsIn("teardown") || 
                   obj.IsIn("get_parameter") || obj.IsIn("set_parameter") || obj.IsIn("redirect") ||
                   obj.IsIn("announce")) &&
                   (obj.Name == "referer" || obj.Name.Contains("Referer"));
        }
        public override uint mutation { get; set; }
        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.Name == "referer" || obj.Name.Contains("Referer"))
            {
                obj.MutatedValue = new Variant("");
            }
        }
    }

    // C Function: repeat_referer
    [Mutator("RepeatReferer")]
    [CMutator("repeat_referer")]
    [Description("Repeats Referer header")]
    public class RtspRepeatReferer : LLMMutator
    {
        public RtspRepeatReferer(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return (obj.IsIn("options") || obj.IsIn("setup") || obj.IsIn("describe") || 
                   obj.IsIn("play") || obj.IsIn("pause") || obj.IsIn("teardown") || 
                   obj.IsIn("get_parameter") || obj.IsIn("set_parameter") || obj.IsIn("redirect") ||
                   obj.IsIn("announce")) &&
                   (obj.Name == "referer" || obj.Name.Contains("Referer"));
        }
        public override uint mutation { get; set; }
        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.Name == "referer" || obj.Name.Contains("Referer"))
            {
                obj.MutatedValue = new Variant("rtsp://a/1, rtsp://b/2");
            }
        }
    }

    // C Function: mutate_referer
    [Mutator("MutateReferer")]
    [CMutator("mutate_referer")]
    [Description("Mutates Referer header")]
    public class RtspMutateReferer : LLMMutator
    {
        public RtspMutateReferer(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return (obj.IsIn("options") || obj.IsIn("setup") || obj.IsIn("describe") || 
                   obj.IsIn("play") || obj.IsIn("pause") || obj.IsIn("teardown") || 
                   obj.IsIn("get_parameter") || obj.IsIn("set_parameter") || obj.IsIn("redirect") ||
                   obj.IsIn("announce")) &&
                   (obj.Name == "referer" || obj.Name.Contains("Referer"));
        }
        public override uint mutation { get; set; }
        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.Name == "referer" || obj.Name.Contains("Referer"))
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
                        mutatedValue = System.Text.Encoding.UTF8.GetString(longUri).TrimEnd('\0');
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
    
    // C Function: add_require
    [Mutator("AddRequire")]
    [CMutator("add_require")]
    [Description("Adds Require header")]
    public class RtspAddRequire : LLMMutator
    {
        public RtspAddRequire(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return (obj.IsIn("options") || obj.IsIn("setup") || obj.IsIn("describe") || 
                   obj.IsIn("play") || obj.IsIn("pause") || obj.IsIn("teardown") || 
                   obj.IsIn("get_parameter") || obj.IsIn("set_parameter") || obj.IsIn("redirect") ||
                   obj.IsIn("announce")) &&
                   (obj.Name == "require" || obj.Name.Contains("Require"));
        }
        public override uint mutation { get; set; }
        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.Name == "require" || obj.Name.Contains("Require"))
            {
                obj.MutatedValue = new Variant("implicit-play");
            }
        }
    }

    // C Function: delete_require
    [Mutator("DeleteRequire")]
    [CMutator("delete_require")]
    [Description("Deletes Require header")]
    public class RtspDeleteRequire : LLMMutator
    {
        public RtspDeleteRequire(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return (obj.IsIn("options") || obj.IsIn("setup") || obj.IsIn("describe") || 
                   obj.IsIn("play") || obj.IsIn("pause") || obj.IsIn("teardown") || 
                   obj.IsIn("get_parameter") || obj.IsIn("set_parameter") || obj.IsIn("redirect") ||
                   obj.IsIn("announce")) &&
                   (obj.Name == "require" || obj.Name.Contains("Require"));
        }
        public override uint mutation { get; set; }
        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.Name == "require" || obj.Name.Contains("Require"))
            {
                obj.MutatedValue = new Variant("");
            }
        }
    }

    // C Function: repeat_require
    [Mutator("RepeatRequire")]
    [CMutator("repeat_require")]
    [Description("Repeats Require header")]
    public class RtspRepeatRequire : LLMMutator
    {
        public RtspRepeatRequire(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return (obj.IsIn("options") || obj.IsIn("setup") || obj.IsIn("describe") || 
                   obj.IsIn("play") || obj.IsIn("pause") || obj.IsIn("teardown") || 
                   obj.IsIn("get_parameter") || obj.IsIn("set_parameter") || obj.IsIn("redirect") ||
                   obj.IsIn("announce")) &&
                   (obj.Name == "require" || obj.Name.Contains("Require"));
        }
        public override uint mutation { get; set; }
        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.Name == "require" || obj.Name.Contains("Require"))
            {
                obj.MutatedValue = new Variant("implicit-play, com.foo.bar, x");
            }
        }
    }

    // C Function: mutate_require
    [Mutator("MutateRequire")]
    [CMutator("mutate_require")]
    [Description("Mutates Require header")]
    public class RtspMutateRequire : LLMMutator
    {
        public RtspMutateRequire(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return (obj.IsIn("options") || obj.IsIn("setup") || obj.IsIn("describe") || 
                   obj.IsIn("play") || obj.IsIn("pause") || obj.IsIn("teardown") || 
                   obj.IsIn("get_parameter") || obj.IsIn("set_parameter") || obj.IsIn("redirect") ||
                   obj.IsIn("announce")) &&
                   (obj.Name == "require" || obj.Name.Contains("Require"));
        }
        public override uint mutation { get; set; }
        public override int count => 1;

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.Name == "require" || obj.Name.Contains("Require"))
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
                        mutatedValue = System.Text.Encoding.UTF8.GetString(longTag).TrimEnd('\0');
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
}

