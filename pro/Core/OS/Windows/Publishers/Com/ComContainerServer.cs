using System;
using System.Runtime.Versioning;
using Peach.Core;

namespace Peach.Pro.Core.OS.Windows.Publishers.Com
{
	[SupportedOSPlatform("windows")]
	public class ComContainerServer : MarshalByRefObject
	{
		public IComContainer GetComContainer(int logLevel, string clsid)
		{
			Utilities.ConfigureLogging(logLevel);

			return new ComContainer(clsid);
		}
	}
}
