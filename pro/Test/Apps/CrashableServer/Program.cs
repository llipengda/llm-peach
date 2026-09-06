using System.Net;
using System.Net.Sockets;

if (args.Length < 1)
{
	Console.Error.WriteLine("usage: CrashableServer <server-address> [server-port] [timeout]");
	return 1;
}

var address = IPAddress.Parse(args[0]);
var port = args.Length >= 2 ? int.Parse(args[1]) : 4242;
var timeout = args.Length >= 3 ? int.Parse(args[2]) : -1;
var listener = new TcpListener(address, port);
listener.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
listener.Start(1);

Console.WriteLine("CrashableServer starting...");
Console.WriteLine("Waiting for a connection...");

while (true)
{
	TcpClient client;
	if (timeout >= 0)
	{
		var pending = listener.AcceptTcpClientAsync();
		if (await Task.WhenAny(pending, Task.Delay(TimeSpan.FromSeconds(timeout))) != pending)
		{
			Console.Error.WriteLine("Timed out waiting for connection");
			return 3;
		}
		client = await pending;
	}
	else
	{
		client = await listener.AcceptTcpClientAsync();
	}

	using (client)
	using (var stream = client.GetStream())
	{
		var buffer = new byte[1024];
		int count;
		while ((count = await stream.ReadAsync(buffer)) != 0)
		{
			Console.WriteLine("Received {0} bytes from client.", count);
			if (count >= buffer.Length)
				Environment.FailFast("CrashableServer received its crash payload.");
		}
	}

	Console.WriteLine("Connection is down.");
}
