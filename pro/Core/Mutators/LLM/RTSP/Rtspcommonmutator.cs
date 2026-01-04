using System;
using System.ComponentModel;
using System.Text;
using Peach.Core;
using Peach.Core.Dom;
using Peach.Pro.Core;
using Peach.Pro.Core.Mutators.LLM;

namespace Peach.Pro.Core.Mutators.LLM.RTSP
{
    // C Function: mutate_request_uri
    // 变异请求 URI，支持多种合法和非法形式
    [Mutator("MutateRequestUri")]
    [CMutator("mutate_request_uri")]
    [Description("Mutates the RTSP request URI with various valid and invalid forms")]
    public class RtspMutateRequestUri : LLMMutator
    {
        public RtspMutateRequestUri(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("request_line") && obj is Peach.Core.Dom.String && obj.Name == "uri";
        }

        public override int count => 1;

        public override uint mutation { get; set; }

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            var random = RtspUtils.GetRandom();
            int[] weights = { 100, 100, 0, 0, 0, 0, 100, 100, 0, 0, 0, 0 };
            
            // Check if this is OPTIONS request (asterisk URI only valid for OPTIONS)
            bool isOptions = obj.IsIn("options");
            if (!isOptions)
            {
                weights[1] = 0; // mut_op_asterisk
            }

            int opIdx = RtspUtils.WeightedPickIdx(weights, weights.Length, random);
            string mutatedValue = "";

            switch (opIdx)
            {
                case 0: // mut_op_absolute_valid
                    mutatedValue = "rtsp://127.0.0.1:8554/test.sdp";
                    break;
                case 1: // mut_op_asterisk
                    mutatedValue = "*";
                    break;
                case 2: // mut_op_empty
                    mutatedValue = "";
                    break;
                case 3: // mut_op_very_long_path
                    byte[] pathBuf = new byte[256];
                    RtspUtils.MakeRepeatedChar(pathBuf, 256, (byte)'A', 246);
                    mutatedValue = "rtsp://host/" + System.Text.Encoding.UTF8.GetString(pathBuf).TrimEnd('\0');
                    break;
                case 4: // mut_op_traversal
                    mutatedValue = "rtsp://host/../../../../../../etc/passwd";
                    break;
                case 5: // mut_op_percent_encoding
                    mutatedValue = "rtsp://host/stream%2Esdp?x=%00%2F..%2F&y=%FF";
                    break;
                case 6: // mut_op_utf8
                    mutatedValue = "rtsp://host/%E6%91%84%E5%83%8F%E5%A4%B4/%E9%80%9A%E9%81%93%E4%B8%80.sdp";
                    break;
                case 7: // mut_op_ipv6_edge_port
                    mutatedValue = "rtsp://[2001:db8::1]:65535/stream";
                    break;
                case 8: // mut_op_userinfo
                    mutatedValue = "rtsp://user:pa%3Ass@host:0/hidden";
                    break;
                case 9: // mut_op_scheme_variants
                    mutatedValue = "RTSPu://HOST/UPCASE";
                    break;
                case 10: // mut_op_query_fragment
                    mutatedValue = "rtsp://host/stream.sdp?track=video&rate=1.0#frag";
                    break;
                case 11: // mut_op_illegal_chars_inject
                    mutatedValue = "rtsp://host/evil\r\nInjected: yes";
                    break;
                default:
                    mutatedValue = "rtsp://127.0.0.1:8554/test.sdp";
                    break;
            }

            obj.MutatedValue = new Variant(mutatedValue);
        }
    }

    // C Function: mutate_cseq
    // 变异 CSeq header，支持合法和非法值
    [Mutator("MutateCseq")]
    [CMutator("mutate_cseq")]
    [Description("Mutates the RTSP CSeq header with various valid and invalid values")]
    public class RtspMutateCseq : LLMMutator
    {
        public RtspMutateCseq(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("common_headers") && obj is Peach.Core.Dom.String && obj.Name == "cseq";
        }

        public override int count => 1;

        public override uint mutation { get; set; }

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            var random = RtspUtils.GetRandom();
            int[] weights = { 100, 0, 0, 100, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            
            int opIdx = RtspUtils.WeightedPickIdx(weights, weights.Length, random);
            string mutatedValue = "";

            // Get current value
            string currentValue = obj.DefaultValue?.ToString() ?? "1";
            int currentNum = 1;
            int.TryParse(currentValue, out currentNum);

            switch (opIdx)
            {
                case 0: // op_valid_increment
                    mutatedValue = (currentNum < int.MaxValue ? currentNum + 1 : currentNum).ToString();
                    break;
                case 1: // op_zero
                    mutatedValue = "0";
                    break;
                case 2: // op_negative
                    mutatedValue = (-1 * (1 + random.Next(1000))).ToString();
                    break;
                case 3: // op_int_max
                    mutatedValue = int.MaxValue.ToString();
                    break;
                case 4: // op_int_min
                    mutatedValue = int.MinValue.ToString();
                    break;
                case 5: // op_large_jump_overflow
                    mutatedValue = (currentNum + (1 << 30)).ToString();
                    break;
                case 6: // op_random_32
                    mutatedValue = ((int)((uint)random.Next() ^ ((uint)random.Next() << 1))).ToString();
                    break;
                case 7: // op_flip_lowbit
                    mutatedValue = (currentNum ^ 1).ToString();
                    break;
                case 8: // op_off_by_one_zero
                    mutatedValue = (currentNum == 1 ? 0 : 1).ToString();
                    break;
                default:
                    mutatedValue = (currentNum < int.MaxValue ? currentNum + 1 : currentNum).ToString();
                    break;
            }

            obj.MutatedValue = new Variant(mutatedValue);
        }
    }
}

