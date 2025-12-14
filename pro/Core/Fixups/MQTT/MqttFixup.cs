using System;
using System.Collections.Generic;
using System.ComponentModel;
using NLog;
using Peach.Core;
using Peach.Core.Dom;

namespace Peach.Pro.Core.Fixups.MQTT
{
    [Description("Mqtt Fixup.")]
    [Fixup("MqttFixup", true)]
    [Parameter("ref", typeof(DataElement), "Reference to data element")]
    [Serializable]
    public class MqttFixup : Fixup
    {
        public DataElement _ref { get; protected set; }

        private static readonly NLog.Logger _logger = LogManager.GetCurrentClassLogger();

        public MqttFixup(DataElement parent, Dictionary<string, Variant> args) : base(parent, args, "ref")
        {
            ParameterParser.Parse(this, args);
        }

        protected override Variant fixupImpl()
        {
            var elem = elements["ref"];
            var packets = elem.find("packets") as Peach.Core.Dom.Array;

            var before = elem.Bytes();

            MqttFixers.ResetPacketIdTracking();

            try {
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
            catch (NullReferenceException)
            {
                _logger.Error("MQTT Fixup failed due to missing expected elements. Skipping fixup.");
                return new Variant(before);
            }

            // before.DumpDiff(elem.Bytes());

            return elem.InternalValue;
        }
    }
}