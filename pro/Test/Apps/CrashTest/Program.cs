using System.Diagnostics;
using System.Runtime.InteropServices;

if (args.Length == 0)
{
	Console.WriteLine("Missing args");
	return 1;
}

switch (args[0])
{
	case "exit":
		return int.Parse(args[1]);
	case "timeout":
		Thread.Sleep(TimeSpan.FromSeconds(int.Parse(args[1])));
		break;
	case "regex":
		Console.Out.WriteLine(args[1]);
		Console.Error.WriteLine(args[2]);
		break;
	case "when":
		File.WriteAllText(args[1], args[2]);
		break;
	case "fork":
		if (OperatingSystem.IsWindows())
		{
			Console.WriteLine("Not supported");
			break;
		}
		var child = Process.Start(new ProcessStartInfo(Environment.ProcessPath, "child") {
			UseShellExecute = false,
		});
		Console.WriteLine("fork()");
		Console.WriteLine("child: {0}", child.Id);
		Console.WriteLine("parent: {0}", Environment.ProcessId);
		child.WaitForExit();
		break;
	case "child":
		Thread.Sleep(Timeout.Infinite);
		break;
	case "nosigterm":
		if (OperatingSystem.IsWindows())
		{
			Console.WriteLine("Not supported");
			break;
		}
		using (PosixSignalRegistration.Create(PosixSignal.SIGTERM, context => context.Cancel = true))
		{
			Console.WriteLine("Ignoring SIGTERM and pausing...");
			Thread.Sleep(Timeout.Infinite);
		}
		break;
}

return 0;
