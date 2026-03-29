using System;
using System.Collections.Generic;
using System.ComponentModel;
using NLog;
using Peach.Core;
using Peach.Core.Dom;
using Peach.Pro.Core.MutationStrategies;

namespace Peach.LLM.Core.Fixups.MQTT
{
    [Description("Mqtt Fixup.")]
    [Fixup("MqttFixup", true)]
    [Parameter("ref", typeof(DataElement), "Reference to data element")]
    [Serializable]
    public class MqttFixup : LLMFixup
    {
        public DataElement _ref { get; protected set; }

        [NonSerialized]
        private static readonly NLog.Logger _logger = LogManager.GetCurrentClassLogger();

        // [NonSerialized]
        // private static readonly System.Random _rand = new System.Random();

        public MqttFixup(DataElement parent, Dictionary<string, Variant> args) : base(parent, args, "ref")
        {
            ParameterParser.Parse(this, args);
        }

        protected override Variant fixupImpl()
        {
            if (!ShouldFixup)
                return elements["ref"].InternalValue;

            var elem = elements["ref"].Clone();
            var packets = elem.find("packets") as Peach.Core.Dom.Array;

            // Phase-aware skipping is now controlled by LLMFixup.ShouldFixup; the strategy will
            // toggle ShouldFixup on LLMFixup instances before Phase 2 to skip LLM-specific fixups.

            // if (_rand.Next(2) == 0)
            //     return elem.InternalValue;

            _logger.Debug("Applying MQTT Fixup to element: {0}", elem.fullName);

            var before = elem.Bytes();

            MqttFixers.ResetPacketIdTracking();

            try
            {
                for (int i = 0; i < packets.Count; i++)
                {
                    var p = (packets[i].find("packet_union") as Choice).SelectedElement;

                    if (p.Name == "connect")
                    {
                        MqttFixers.FixConnect(p);
                    }
                    else if (p.Name == "publish")
                    {
                        MqttFixers.FixPublish(p);
                    }
                    else if (p.Name == "subscribe")
                    {
                        MqttFixers.FixSubscribe(p);
                    }
                    else if (p.Name == "unsubscribe")
                    {
                        MqttFixers.FixUnsubscribe(p);
                    }
                }
            }
            catch (NullReferenceException ex)
            {
                _logger.Error(ex, "MQTT Fixup failed due to missing expected elements. Skipping fixup.");
                return new Variant(before);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "MQTT Fixup failed. Skipping fixup.");
                return new Variant(before);
            }

            // before.DumpDiff(elem.Bytes());

            return elem.InternalValue;
        }
    }
}