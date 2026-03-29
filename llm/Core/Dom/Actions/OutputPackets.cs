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
            logger.Debug("Output each packet in data model.");

            publisher.start();
            publisher.open();

            foreach (var packet in data.dataModel.find("packets")?.Children() ?? Enumerable.Empty<DataElement>())
            {
                publisher.output(packet.Value);
            }
        }
    }
}