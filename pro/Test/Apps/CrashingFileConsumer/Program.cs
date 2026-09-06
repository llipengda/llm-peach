using System;
using System.IO;

namespace Peach.TestApps.CrashingFileConsumer
{
	internal static class Program
	{
		public static int Main(string[] args)
		{
			if (args.Length < 1)
			{
				Console.WriteLine("Error, please supply a filename to load.");
				return -1;
			}

			Console.WriteLine("Loading file \"{0}\"...", args[0]);

			try
			{
				using (var stream = new FileStream(args[0], FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
				using (var reader = new MemoryStream())
				{
					stream.CopyTo(reader);
					Console.WriteLine("Length of file is {0}.", reader.Length);
				}
			}
			catch (Exception)
			{
				Console.WriteLine("Error, unable to open file \"{0}\".", args[0]);
			}

			return 0;
		}
	}
}
