using System;
using System.Collections.Generic;
using System.ComponentModel;
using NLog;
using Peach.Core;
using Peach.Core.Dom;
using Peach.Pro.Core.MutationStrategies;

namespace Peach.Pro.Core.Fixups.LLM.DTLS
{
    [Description("Dtls Fixup.")]
    [Fixup("DtlsFixup", true)]
    [Parameter("ref", typeof(DataElement), "Reference to data element")]
    [Serializable]
    public class DtlsFixup : LLMFixup
    {
        public DataElement _ref { get; protected set; }

        [NonSerialized]
        private static readonly NLog.Logger _logger = LogManager.GetCurrentClassLogger();

        // [NonSerialized]
        // private static readonly System.Random _rand = new System.Random();

        public DtlsFixup(DataElement parent, Dictionary<string, Variant> args) : base(parent, args, "ref")
        {
            ParameterParser.Parse(this, args);
        }

        protected override Variant fixupImpl()
        {
            if (!ShouldFixup)
                return elements["ref"].InternalValue;

            var elem = elements["ref"].Clone();

            // if (_rand.Next(2) == 0)
            //     return elem.InternalValue;

            _logger.Debug("Applying DTLS Fixup to element: {0}", elem.fullName);

            var before = elem.Bytes();

            try
            {
                DtlsFixers.FixDtls(elem);
            }
            catch (NullReferenceException ex)
            {
                _logger.Error(ex, "DTLS Fixup failed due to missing expected elements. Skipping fixup.");
                return new Variant(before);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "DTLS Fixup failed. Skipping fixup.");
                return new Variant(before);
            }

            // before.DumpDiff(elem.Bytes());

            return elem.InternalValue;
        }
    }
}