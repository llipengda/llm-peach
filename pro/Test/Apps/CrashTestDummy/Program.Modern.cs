using System;
using System.Threading;
using Peach.Pro.Core.OS;

namespace Peach.CrashTestDummy
{
	public static class Program
	{
		public static int Main(string[] args)
		{
			IDisposable canary = null;
			if (args.Length > 0 && Guid.TryParse(args[0], out var guid))
				canary = Pal.GetCanary(guid);

			try
			{
				using var mutex = Pal.SingleInstance("CrashTestDummy");
				for (var i = 0; i < 20 && !mutex.TryLock(); ++i)
					Thread.Sleep(1000);
			}
			finally
			{
				canary?.Dispose();
			}

			return 0;
		}
	}
}
