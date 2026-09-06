using System;
using Peach.Pro.Core.Runtime;

namespace Peach
{
	/// <summary>
	/// Command line interface for Peach 3.
	/// Mostly backwards compatable with Peach 2.3.
	/// </summary>
	public class PeachMain
	{
		static int Main(string[] args)
		{
			try
			{
				//System.Diagnostics.Debugger.Launch();

				using (var program = new ConsoleProgram())
				{
					return program.Run(args);
				}
			}
			catch (Exception)
			{
				if (System.Diagnostics.Debugger.IsAttached)
					System.Diagnostics.Debugger.Break();
				throw;
			}
			finally
			{
				if (System.Diagnostics.Debugger.IsAttached)
					System.Diagnostics.Debugger.Break();
			}
		}
	}
}
