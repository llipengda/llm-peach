using System;
using System.Linq;
using System.Reflection;
using Mono.Unix;
using Mono.Unix.Native;

namespace PeachTrampoline
{
	public class TrampolineMain
	{
		static int Main(string[] args)
		{
			if (args.Length == 3 && args[0] == "--ipc")
				return DoRemote(args[1], args[2]);

			if (args.Length > 0 && args[0] != "--ipc")
				return DoExec(args);

			var asm = Assembly.GetExecutingAssembly();
			var asmName = asm.GetName();
			var copywright = asm.GetCustomAttributes(false)
				.OfType<AssemblyCopyrightAttribute>()
				.Select(a => a.Copyright)
				.FirstOrDefault();

			Console.WriteLine(@"{0} v{1}

{2}

Ipc Proxy Usage:
{0}.exe --ipc <channel> <type>

Exec Usage:
{0}.exe <file> [<args>]
", asmName.Name, asmName.Version, copywright ?? "");

			return -1;
		}

		static int DoExec(string[] args)
		{
			var ret = Syscall.fcntl(3, FcntlCommand.F_SETFD, 1);
			UnixMarshal.ThrowExceptionForLastErrorIf(ret);
	
			ret = Syscall.execvp(args[0], args);
			UnixMarshal.ThrowExceptionForLastErrorIf(ret);

			return 0;
		}

		static int DoRemote(string channelName, string typeName)
		{
			Console.Error.WriteLine("The --ipc mode depended on .NET Remoting and is not available on .NET 8.");
			return 2;
		}
	}
}
