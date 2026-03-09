using System;
using System.ComponentModel;
using System.Text;
using Peach.Core;
using Peach.Core.Dom;

namespace Peach.LLM.Core.Mutators.RTSP
{
    // C Function: add_accept
    [Mutator("AddAccept")]
    [CMutator("add_accept")]
    [Description("Adds Accept header with application/sdp")]
    public class RtspAddDescribeAccept : LLMMutator
    {
        public RtspAddDescribeAccept(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("describe") && (obj.Name == "accept" || obj.Name.Contains("Accept"));
        }

        public override int count => 1;

        public override uint mutation { get; set; }

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.Name == "accept" || obj.Name.Contains("Accept"))
            {
                obj.MutatedValue = new Variant("application/sdp");
            }
        }
    }

    // C Function: delete_accept
    [Mutator("DeleteAccept")]
    [CMutator("delete_accept")]
    [Description("Deletes Accept header")]
    public class RtspDeleteDescribeAccept : LLMMutator
    {
        public RtspDeleteDescribeAccept(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("describe") && (obj.Name == "accept" || obj.Name.Contains("Accept"));
        }

        public override int count => 1;

        public override uint mutation { get; set; }

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.Name == "accept" || obj.Name.Contains("Accept"))
            {
                obj.MutatedValue = new Variant("");
            }
        }
    }

    // C Function: repeat_accept
    [Mutator("RepeatAccept")]
    [CMutator("repeat_accept")]
    [Description("Repeats Accept header with multiple values")]
    public class RtspRepeatDescribeAccept : LLMMutator
    {
        public RtspRepeatDescribeAccept(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("describe") && (obj.Name == "accept" || obj.Name.Contains("Accept"));
        }

        public override int count => 1;

        public override uint mutation { get; set; }

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.Name == "accept" || obj.Name.Contains("Accept"))
            {
                obj.MutatedValue = new Variant("application/sdp, */*;q=0.1, text/plain");
            }
        }
    }

    // C Function: mutate_accept
    [Mutator("MutateAccept")]
    [CMutator("mutate_accept")]
    [Description("Mutates Accept header with various valid and invalid forms")]
    public class RtspMutateDescribeAccept : LLMMutator
    {
        public RtspMutateDescribeAccept(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("describe") && (obj.Name == "accept" || obj.Name.Contains("Accept"));
        }

        public override int count => 1;

        public override uint mutation { get; set; }

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.Name == "accept" || obj.Name.Contains("Accept"))
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
    }

    // C Function: add_accept_encoding
    [Mutator("AddAcceptEncoding")]
    [CMutator("add_accept_encoding")]
    [Description("Adds Accept-Encoding header")]
    public class RtspAddDescribeAcceptEncoding : LLMMutator
    {
        public RtspAddDescribeAcceptEncoding(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("describe") && (obj.Name == "accept_encoding" || obj.Name.Contains("encoding"));
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

    // C Function: delete_accept_encoding
    [Mutator("DeleteAcceptEncoding")]
    [CMutator("delete_accept_encoding")]
    [Description("Deletes Accept-Encoding header")]
    public class RtspDeleteDescribeAcceptEncoding : LLMMutator
    {
        public RtspDeleteDescribeAcceptEncoding(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("describe") && (obj.Name == "accept_encoding" || obj.Name.Contains("encoding"));
        }

        public override int count => 1;

        public override uint mutation { get; set; }

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
    [Mutator("RepeatAcceptEncoding")]
    [CMutator("repeat_accept_encoding")]
    [Description("Repeats Accept-Encoding header")]
    public class RtspRepeatDescribeAcceptEncoding : LLMMutator
    {
        public RtspRepeatDescribeAcceptEncoding(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("describe") && (obj.Name == "accept_encoding" || obj.Name.Contains("encoding"));
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
    [Mutator("MutateAcceptEncoding")]
    [CMutator("mutate_accept_encoding")]
    [Description("Mutates Accept-Encoding header")]
    public class RtspMutateDescribeAcceptEncoding : LLMMutator
    {
        public RtspMutateDescribeAcceptEncoding(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("describe") && (obj.Name == "accept_encoding" || obj.Name.Contains("encoding"));
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

    // C Function: add_accept_language
    [Mutator("AddAcceptLanguage")]
    [CMutator("add_accept_language")]
    [Description("Adds Accept-Language header")]
    public class RtspAddDescribeAcceptLanguage : LLMMutator
    {
        public RtspAddDescribeAcceptLanguage(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("describe") && (obj.Name == "accept_language" || obj.Name.Contains("language"));
        }

        public override int count => 1;

        public override uint mutation { get; set; }

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if (obj.Name == "accept_language" || obj.Name.Contains("language"))
            {
                obj.MutatedValue = new Variant("en-US");
            }
        }
    }

    // C Function: delete_accept_language
    [Mutator("DeleteAcceptLanguage")]
    [CMutator("delete_accept_language")]
    [Description("Deletes Accept-Language header")]
    public class RtspDeleteDescribeAcceptLanguage : LLMMutator
    {
        public RtspDeleteDescribeAcceptLanguage(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("describe") && (obj.Name == "accept_language" || obj.Name.Contains("language"));
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
    [Mutator("RepeatAcceptLanguage")]
    [CMutator("repeat_accept_language")]
    [Description("Repeats Accept-Language header")]
    public class RtspRepeatDescribeAcceptLanguage : LLMMutator
    {
        public RtspRepeatDescribeAcceptLanguage(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("describe") && (obj.Name == "accept_language" || obj.Name.Contains("language"));
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
    [Mutator("MutateAcceptLanguage")]
    [CMutator("mutate_accept_language")]
    [Description("Mutates Accept-Language header")]
    public class RtspMutateDescribeAcceptLanguage : LLMMutator
    {
        public RtspMutateDescribeAcceptLanguage(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("describe") && (obj.Name == "accept_language" || obj.Name.Contains("language"));    
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
}

