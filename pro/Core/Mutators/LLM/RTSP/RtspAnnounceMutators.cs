using System;
using System.ComponentModel;
using System.Text;
using Peach.Core;
using Peach.Core.Dom;

namespace Peach.Pro.Core.Mutators.LLM.RTSP
{
    // ==================== Connection ====================
    
    // C Function: delete_connection
    [Mutator("DeleteAnnounceConnection")]
    [CMutator("delete_connection")]
    [Description("Deletes Connection header for ANNOUNCE")]
    public class RtspDeleteAnnounceConnection : LLMMutator
    {
        public RtspDeleteAnnounceConnection(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("announce") && (obj.Name == "connection" || obj.Name.Contains("Connection"));   
        }

        public override int count => 1;

        public override uint mutation { get; set; }

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if ( obj.Name == "connection" ||  obj.Name.Contains("Connection"))
            {
                obj.MutatedValue = new Variant("");
            }
        }
    }

    // C Function: repeat_connection
    [Mutator("RepeatAnnounceConnection")]
    [CMutator("repeat_connection")]
    [Description("Repeats Connection header for ANNOUNCE")]
    public class RtspRepeatAnnounceConnection : LLMMutator
    {
        public RtspRepeatAnnounceConnection(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("announce") && (obj.Name == "connection" || obj.Name.Contains("Connection"));           
        }

        public override int count => 1;

        public override uint mutation { get; set; }

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if ( obj.Name == "connection" ||  obj.Name.Contains("Connection"))
            {
                obj.MutatedValue = new Variant("keep-alive, close");
            }
        }
    }

    // C Function: mutate_connection
    [Mutator("MutateAnnounceConnection")]
    [CMutator("mutate_connection")]
    [Description("Mutates Connection header for ANNOUNCE")]
    public class RtspMutateAnnounceConnection : LLMMutator
    {
        public RtspMutateAnnounceConnection(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("announce") && (obj.Name == "connection" || obj.Name.Contains("Connection"));   
        }

        public override int count => 1;

        public override uint mutation { get; set; }

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if ( obj.Name == "connection" ||  obj.Name.Contains("Connection"))
            {
                var random = RtspUtils.GetRandom();
                string[] values = { "keep-alive", "close", "keep-alive, foo, bar" };
                obj.MutatedValue = new Variant(values[random.Next(values.Length)]);
            }
        }
    }

    // ==================== Date ====================
    
    // C Function: delete_date
    [Mutator("DeleteAnnounceDate")]
    [CMutator("delete_date")]
    [Description("Deletes Date header for ANNOUNCE")]
    public class RtspDeleteAnnounceDate : LLMMutator
    {
        public RtspDeleteAnnounceDate(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("announce") && (obj.Name == "date" || obj.Name.Contains("Date"));   
        }

        public override int count => 1;

        public override uint mutation { get; set; }

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if ( obj.Name == "date" ||  obj.Name.Contains("Date"))
            {
                obj.MutatedValue = new Variant("");
            }
        }
    }

    // C Function: repeat_date
    [Mutator("RepeatAnnounceDate")]
    [CMutator("repeat_date")]
    [Description("Repeats Date header for ANNOUNCE")]
    public class RtspRepeatAnnounceDate : LLMMutator
    {
        public RtspRepeatAnnounceDate(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("announce") && (obj.Name == "date" || obj.Name.Contains("Date"));   
        }

        public override int count => 1;

        public override uint mutation { get; set; }

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if ( obj.Name == "date" ||  obj.Name.Contains("Date"))
            {
                obj.MutatedValue = new Variant("Tue, 15 Nov 1994 08:12:31 GMT, Wed, 16 Nov 1994 09:13:32 GMT");
            }
        }
    }

    // C Function: mutate_date
    [Mutator("MutateAnnounceDate")]
    [CMutator("mutate_date")]
    [Description("Mutates Date header for ANNOUNCE")]
    public class RtspMutateAnnounceDate : LLMMutator
    {
        public RtspMutateAnnounceDate(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("announce") && (obj.Name == "date" || obj.Name.Contains("Date"));   
        }

        public override int count => 1;

        public override uint mutation { get; set; }

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if ( obj.Name == "date" ||  obj.Name.Contains("Date"))
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
    [Mutator("DeleteAnnounceVia")]
    [CMutator("delete_via")]
    [Description("Deletes Via header for ANNOUNCE")]
    public class RtspDeleteAnnounceVia : LLMMutator
    {
        public RtspDeleteAnnounceVia(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("announce") && (obj.Name == "via" || obj.Name.Contains("Via"));   
        }

        public override int count => 1;

        public override uint mutation { get; set; }

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if ( obj.Name == "via" ||  obj.Name.Contains("Via"))
            {
                obj.MutatedValue = new Variant("");
            }
        }
    }

    // C Function: repeat_via
    [Mutator("RepeatAnnounceVia")]
    [CMutator("repeat_via")]
    [Description("Repeats Via header for ANNOUNCE")]
    public class RtspRepeatAnnounceVia : LLMMutator
    {
        public RtspRepeatAnnounceVia(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("announce") && (obj.Name == "via" || obj.Name.Contains("Via"));   
        }

        public override int count => 1;

        public override uint mutation { get; set; }

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if ( obj.Name == "via" ||  obj.Name.Contains("Via"))
            {
                obj.MutatedValue = new Variant("RTSP/1.0 example.com, RTSP/1.0 proxy.com");
            }
        }
    }

    // C Function: mutate_via
    [Mutator("MutateAnnounceVia")]
    [CMutator("mutate_via")]
    [Description("Mutates Via header for ANNOUNCE")]
    public class RtspMutateAnnounceVia : LLMMutator
    {
        public RtspMutateAnnounceVia(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("announce") && (obj.Name == "via" || obj.Name.Contains("Via"));   
        }

        public override int count => 1;

        public override uint mutation { get; set; }

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if ( obj.Name == "via" ||  obj.Name.Contains("Via"))
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
    [Mutator("DeleteAnnounceAcceptLanguage")]
    [CMutator("delete_accept_language")]
    [Description("Deletes Accept-Language header for ANNOUNCE")]
    public class RtspDeleteAnnounceAcceptLanguage : LLMMutator
    {
        public RtspDeleteAnnounceAcceptLanguage(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("announce") && (obj.Name == "accept_language" || obj.Name.Contains("language"));   
        }

        public override int count => 1;

        public override uint mutation { get; set; }

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if ( obj.Name == "accept_language" ||  obj.Name.Contains("language"))
            {
                obj.MutatedValue = new Variant("");
            }
        }
    }

    // C Function: repeat_accept_language
    [Mutator("RepeatAnnounceAcceptLanguage")]
    [CMutator("repeat_accept_language")]
    [Description("Repeats Accept-Language header for ANNOUNCE")]
    public class RtspRepeatAnnounceAcceptLanguage : LLMMutator
    {
        public RtspRepeatAnnounceAcceptLanguage(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("announce") && (obj.Name == "accept_language" || obj.Name.Contains("language"));   
        }

        public override int count => 1;

        public override uint mutation { get; set; }

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if ( obj.Name == "accept_language" ||  obj.Name.Contains("language"))
            {
                obj.MutatedValue = new Variant("en-US, fr-FR, de-DE");
            }
        }
    }

    // C Function: mutate_accept_language
    [Mutator("MutateAnnounceAcceptLanguage")]
    [CMutator("mutate_accept_language")]
    [Description("Mutates Accept-Language header for ANNOUNCE")]
    public class RtspMutateAnnounceAcceptLanguage : LLMMutator
    {
        public RtspMutateAnnounceAcceptLanguage(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("announce") && (obj.Name == "accept_language" || obj.Name.Contains("language"));   
        }

        public override int count => 1;

        public override uint mutation { get; set; }

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if ( obj.Name == "accept_language" ||  obj.Name.Contains("language"))
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
    [Mutator("DeleteAnnounceContentEncoding")]
    [CMutator("delete_content_encoding")]
    [Description("Deletes Content-Encoding header for ANNOUNCE")]
    public class RtspDeleteAnnounceContentEncoding : LLMMutator
    {
        public RtspDeleteAnnounceContentEncoding(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("announce") && (obj.Name == "content_encoding" || obj.Name.Contains("Content-Encoding"));   
        }

        public override int count => 1;

        public override uint mutation { get; set; }

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if ( obj.Name == "content_encoding" ||  obj.Name.Contains("Content-Encoding"))
            {
                obj.MutatedValue = new Variant("");
            }
        }
    }

    // C Function: repeat_content_encoding
    [Mutator("RepeatAnnounceContentEncoding")]
    [CMutator("repeat_content_encoding")]
    [Description("Repeats Content-Encoding header for ANNOUNCE")]
    public class RtspRepeatAnnounceContentEncoding : LLMMutator
    {
        public RtspRepeatAnnounceContentEncoding(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("announce") && (obj.Name == "content_encoding" || obj.Name.Contains("Content-Encoding"));   
        }

        public override int count => 1;

        public override uint mutation { get; set; }

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if ( obj.Name == "content_encoding" ||  obj.Name.Contains("Content-Encoding"))
            {
                obj.MutatedValue = new Variant("gzip, deflate, br");
            }
        }
    }

    // C Function: mutate_content_encoding
    [Mutator("MutateAnnounceContentEncoding")]
    [CMutator("mutate_content_encoding")]
    [Description("Mutates Content-Encoding header for ANNOUNCE")]
    public class RtspMutateAnnounceContentEncoding : LLMMutator
    {
        public RtspMutateAnnounceContentEncoding(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("announce") && (obj.Name == "content_encoding" || obj.Name.Contains("Content-Encoding"));
        }

        public override int count => 1;

        public override uint mutation { get; set; }

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if ( obj.Name == "content_encoding" ||  obj.Name.Contains("Content-Encoding"))
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

    // ==================== Content-Language ====================
    
    // C Function: delete_content_language
    [Mutator("DeleteAnnounceContentLanguage")]
    [CMutator("delete_content_language")]
    [Description("Deletes Content-Language header for ANNOUNCE")]
    public class RtspDeleteAnnounceContentLanguage : LLMMutator
    {
        public RtspDeleteAnnounceContentLanguage(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("announce") && (obj.Name == "content_language" || obj.Name.Contains("Content-Language"));       
        }

        public override int count => 1;

        public override uint mutation { get; set; }

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if ( obj.Name == "content_language" ||  obj.Name.Contains("Content-Language"))
            {
                obj.MutatedValue = new Variant("");
            }
        }
    }

    // C Function: repeat_content_language
    [Mutator("RepeatAnnounceContentLanguage")]
    [CMutator("repeat_content_language")]
    [Description("Repeats Content-Language header for ANNOUNCE")]
    public class RtspRepeatAnnounceContentLanguage : LLMMutator
    {
        public RtspRepeatAnnounceContentLanguage(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("announce") && (obj.Name == "content_language" || obj.Name.Contains("Content-Language"));       
        }

        public override int count => 1;

        public override uint mutation { get; set; }

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if ( obj.Name == "content_language" ||  obj.Name.Contains("Content-Language"))
            {
                obj.MutatedValue = new Variant("en-US, fr-FR, de-DE");
            }
        }
    }

    // C Function: mutate_content_language
    [Mutator("MutateAnnounceContentLanguage")]
    [CMutator("mutate_content_language")]
    [Description("Mutates Content-Language header for ANNOUNCE")]
    public class RtspMutateAnnounceContentLanguage : LLMMutator
    {
        public RtspMutateAnnounceContentLanguage(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("announce") && (obj.Name == "content_language" || obj.Name.Contains("Content-Language"));       
        }

        public override int count => 1;

        public override uint mutation { get; set; }

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if ( obj.Name == "content_language" ||  obj.Name.Contains("Content-Language"))
            {
                var random = RtspUtils.GetRandom();
                int[] weights = { 100, 100, 100, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                
                int opIdx = RtspUtils.WeightedPickIdx(weights, weights.Length, random);
                string mutatedValue = "";

                switch (opIdx)
                {
                    case 0: // cl_ok_simple
                        mutatedValue = "en-US";
                        break;
                    case 1: // cl_ok_multi
                        mutatedValue = "en-US, fr-FR, de-DE";
                        break;
                    case 2: // cl_duplicate
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

    // ==================== Content-Length ====================
    
    // C Function: delete_content_length
    [Mutator("DeleteAnnounceContentLength")]
    [CMutator("delete_content_length")]
    [Description("Deletes Content-Length header for ANNOUNCE")]
    public class RtspDeleteAnnounceContentLength : LLMMutator
    {
        public RtspDeleteAnnounceContentLength(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("announce") && (obj.Name == "content_length" || obj.Name.Contains("Content-Length"));           
        }

        public override int count => 1;

        public override uint mutation { get; set; }

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if ( obj.Name == "content_length" ||  obj.Name.Contains("Content-Length"))
            {
                obj.MutatedValue = new Variant("");
            }
        }
    }

    // C Function: repeat_content_length
    [Mutator("RepeatAnnounceContentLength")]
    [CMutator("repeat_content_length")]
    [Description("Repeats Content-Length header for ANNOUNCE")]
    public class RtspRepeatAnnounceContentLength : LLMMutator
    {
        public RtspRepeatAnnounceContentLength(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("announce") && (obj.Name == "content_length" || obj.Name.Contains("Content-Length"));           
        }

        public override int count => 1;

        public override uint mutation { get; set; }

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if ( obj.Name == "content_length" ||  obj.Name.Contains("Content-Length"))
            {
                obj.MutatedValue = new Variant("1234");
            }
        }
    }

    // C Function: mutate_content_length
    [Mutator("MutateAnnounceContentLength")]
    [CMutator("mutate_content_length")]
    [Description("Mutates Content-Length header for ANNOUNCE")]
    public class RtspMutateAnnounceContentLength : LLMMutator
    {
        public RtspMutateAnnounceContentLength(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("announce") && (obj.Name == "content_length" || obj.Name.Contains("Content-Length"));           
        }

        public override int count => 1;

        public override uint mutation { get; set; }

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if ( obj.Name == "content_length" ||  obj.Name.Contains("Content-Length"))
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
    [Mutator("DeleteAnnounceContentType")]
    [CMutator("delete_content_type")]
    [Description("Deletes Content-Type header for ANNOUNCE")]
    public class RtspDeleteAnnounceContentType : LLMMutator
    {
        public RtspDeleteAnnounceContentType(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("announce") && (obj.Name == "content_type" || obj.Name.Contains("Content-Type"));               
        }

        public override int count => 1;

        public override uint mutation { get; set; }

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if ( obj.Name == "content_type" ||  obj.Name.Contains("Content-Type"))
            {
                obj.MutatedValue = new Variant("");
            }
        }
    }

    // C Function: repeat_content_type
    [Mutator("RepeatAnnounceContentType")]
    [CMutator("repeat_content_type")]
    [Description("Repeats Content-Type header for ANNOUNCE")]
    public class RtspRepeatAnnounceContentType : LLMMutator
    {
        public RtspRepeatAnnounceContentType(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("announce") && (obj.Name == "content_type" || obj.Name.Contains("Content-Type"));               
        }

        public override int count => 1;

        public override uint mutation { get; set; }

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if ( obj.Name == "content_type" ||  obj.Name.Contains("Content-Type"))
            {
                obj.MutatedValue = new Variant("application/sdp, text/plain");
            }
        }
    }

    // C Function: mutate_content_type
    [Mutator("MutateAnnounceContentType")]
    [CMutator("mutate_content_type")]
    [Description("Mutates Content-Type header for ANNOUNCE")]
    public class RtspMutateAnnounceContentType : LLMMutator
    {
        public RtspMutateAnnounceContentType(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("announce") && (obj.Name == "content_type" || obj.Name.Contains("Content-Type"));               
        }

        public override int count => 1;

        public override uint mutation { get; set; }

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if ( obj.Name == "content_type" ||  obj.Name.Contains("Content-Type"))
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

    // ==================== Expires ====================
    
    // C Function: delete_expires
    [Mutator("DeleteAnnounceExpires")]
    [CMutator("delete_expires")]
    [Description("Deletes Expires header for ANNOUNCE")]
    public class RtspDeleteAnnounceExpires : LLMMutator
    {
        public RtspDeleteAnnounceExpires(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("announce") && (obj.Name == "expires" || obj.Name.Contains("Expires"));             
        }

        public override int count => 1;

        public override uint mutation { get; set; }

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if ( obj.Name == "expires" ||  obj.Name.Contains("Expires"))
            {
                obj.MutatedValue = new Variant("");
            }
        }
    }

    // C Function: repeat_expires
    [Mutator("RepeatAnnounceExpires")]
    [CMutator("repeat_expires")]
    [Description("Repeats Expires header for ANNOUNCE")]
    public class RtspRepeatAnnounceExpires : LLMMutator
    {
        public RtspRepeatAnnounceExpires(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("announce") && (obj.Name == "expires" || obj.Name.Contains("Expires"));             
        }

        public override int count => 1;

        public override uint mutation { get; set; }

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if ( obj.Name == "expires" ||  obj.Name.Contains("Expires"))
            {
                obj.MutatedValue = new Variant("Wed, 01 Jan 2099 00:00:00 GMT, Thu, 02 Jan 2099 00:00:00 GMT");
            }
        }
    }

    // C Function: mutate_expires
    [Mutator("MutateAnnounceExpires")]
    [CMutator("mutate_expires")]
    [Description("Mutates Expires header for ANNOUNCE")]
    public class RtspMutateAnnounceExpires : LLMMutator
    {
        public RtspMutateAnnounceExpires(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.IsIn("announce") && (obj.Name == "expires" || obj.Name.Contains("Expires"));             
        }

        public override int count => 1;

        public override uint mutation { get; set; }

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            if ( obj.Name == "expires" ||  obj.Name.Contains("Expires"))
            {
                var random = RtspUtils.GetRandom();
                int[] weights = { 100, 100, 100, 0, 0, 0, 0, 0, 100, 0, 0 };
                
                int opIdx = RtspUtils.WeightedPickIdx(weights, weights.Length, random);
                string mutatedValue = "";

                switch (opIdx)
                {
                    case 0: // ex_ok_future
                        mutatedValue = "Wed, 01 Jan 2099 00:00:00 GMT";
                        break;
                    case 1: // ex_past
                        mutatedValue = "Wed, 01 Jan 1990 00:00:00 GMT";
                        break;
                    case 2: // ex_now
                        mutatedValue = "Wed, 01 Jan 2099 23:59:59 GMT";
                        break;
                    case 8: // ex_weekday_mismatch
                        mutatedValue = "Sun, 01 Jan 2099 00:00:00 GMT";
                        break;
                    default:
                        mutatedValue = "Wed, 01 Jan 2099 00:00:00 GMT";
                        break;
                }

                obj.MutatedValue = new Variant(mutatedValue);
            }
        }
    }


    // ==================== Session ====================
    
    // C Function: delete_session
    [Mutator("DeleteAnnounceSession")]
    [CMutator("delete_session")]
    [Description("Deletes Session header for ANNOUNCE")]
    public class RtspDeleteAnnounceSession : LLMMutator
    {
        public RtspDeleteAnnounceSession(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            // 严格检查：必须是announce上下文，名称必须是session，且必须是String类型
            return obj.IsIn("announce") && (obj.Name == "session" || obj.Name.Contains("Session")) && obj is Peach.Core.Dom.String;           
        }

        public override int count => 1;

        public override uint mutation { get; set; }

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            // 双重检查：确保是String类型且名称正确
            if (obj.Name == "session" || obj.Name.Contains("Session") && obj is Peach.Core.Dom.String)
            {
                obj.MutatedValue = new Variant("");
            }
        }
    }

    // C Function: repeat_session
    [Mutator("RepeatAnnounceSession")]
    [CMutator("repeat_session")]
    [Description("Repeats Session header for ANNOUNCE")]
    public class RtspRepeatAnnounceSession : LLMMutator
    {
        public RtspRepeatAnnounceSession(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            // 严格检查：必须是announce上下文，名称必须是session，且必须是String类型
            return obj.IsIn("announce") && (obj.Name == "session" || obj.Name.Contains("Session")) && obj is Peach.Core.Dom.String;
        }

        public override int count => 1;

        public override uint mutation { get; set; }

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            // 双重检查：确保是String类型且名称正确
            if (obj.Name == "session" || obj.Name.Contains("Session") && obj is Peach.Core.Dom.String)
            {
                obj.MutatedValue = new Variant("ABCDEF, 1234");
            }
        }
    }

    // C Function: mutate_session
    [Mutator("MutateAnnounceSession")]
    [CMutator("mutate_session")]
    [Description("Mutates Session header for ANNOUNCE with strict type checking")]
    public class RtspMutateAnnounceSession : LLMMutator
    {
        public RtspMutateAnnounceSession(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            // 严格检查：必须是announce上下文，名称必须是session，且必须是String类型
            return obj.IsIn("announce") && (obj.Name == "session" || obj.Name.Contains("Session")) && obj is Peach.Core.Dom.String;
        }

        public override int count => 1;

        public override uint mutation { get; set; }

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }

        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            // 多重检查：确保类型和名称都正确
            if (obj.Name == "session" || obj.Name.Contains("Session") && obj is Peach.Core.Dom.String)  
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

