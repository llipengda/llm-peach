using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Peach.Pro.Core;
using Peach.Core;
using Peach.Core.Dom.Actions;
using Peach.Core.Dom;

namespace Peach.LLM.Core.Dom.Actions
{
	[Action("OutputPackets")]
	[Serializable]
	public class OutputPackets : Output
    {
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        protected override void OnRun(Publisher publisher, RunContext context)
        {
            var packets = data.dataModel.find("packets")?.Children() ?? Enumerable.Empty<DataElement>();

            var packetCount = (context.stateStore.ContainsKey("packetCount") ? (int)context.stateStore["packetCount"] : 0) + packets.Count();
            var packetSequenceCount = (context.stateStore.ContainsKey("packetSequenceCount") ? (int)context.stateStore["packetSequenceCount"] : 0) + 1;

            context.stateStore["packetCount"] = packetCount;
            context.stateStore["packetSequenceCount"] = packetSequenceCount;

            logger.Debug("Outputting {0} packets (Total: {1}, Sequences: {2})", packets.Count(), packetCount, packetSequenceCount);

            publisher.start();
            publisher.open();

            foreach (var packet in packets)
            {
                publisher.output(packet.Value);
            }
        }
    }
}