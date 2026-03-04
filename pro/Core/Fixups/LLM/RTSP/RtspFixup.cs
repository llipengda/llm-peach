using System;
using System.Collections.Generic;
using System.ComponentModel;
using NLog;
using Peach.Core;
using Peach.Core.Dom;

namespace Peach.Pro.Core.Fixups.LLM.RTSP
{
    [Description("RTSP Fixup.")]
    [Fixup("RtspFixup", true)]
    [Parameter("ref", typeof(DataElement), "Reference to data element")]
    [Serializable]
    public class RtspFixup : LLMFixup
    {
        public DataElement _ref { get; protected set; }

        [NonSerialized]
        private static readonly NLog.Logger _logger = LogManager.GetCurrentClassLogger();

        public RtspFixup(DataElement parent, Dictionary<string, Variant> args) : base(parent, args, "ref")
        {
            ParameterParser.Parse(this, args);
        }

        protected override Variant fixupImpl()
        {
            if (!ShouldFixup)
                return elements["ref"].InternalValue;

            var elem = elements["ref"].Clone();
            // var requests = elem.find("requests") as Peach.Core.Dom.Array;

            _logger.Debug("Applying RTSP Fixup to element: {0}", elem.fullName);

            var before = elem.Bytes();

            try
            {
                RtspFixers.FixRtsp(elem);
            }
            catch (NullReferenceException ex)
            {
                _logger.Error(ex, "RTSP Fixup failed due to missing expected elements. Skipping fixup.");
                return new Variant(before);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "RTSP Fixup failed. Skipping fixup.");
                return new Variant(before);
            }

            return elem.InternalValue;
        }
    }
}
