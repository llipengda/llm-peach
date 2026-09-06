using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Peach.Core;
using Peach.Core.IO;
using Peach.Core.Test;
using Peach.Pro.Core.Publishers;

namespace Peach.Pro.Test.Core.Publishers
{
	[TestFixture]
	[Quick]
	[Peach]
	class WebSocketPublisherTests
	{
		[Test]
		public async Task TestMessageRoundTrip()
		{
			var listener = new TcpListener(IPAddress.Loopback, 0);
			listener.Start();
			var port = ((IPEndPoint)listener.LocalEndpoint).Port;
			listener.Stop();

			var template = Path.GetTempFileName();
			File.WriteAllText(template, "payload=##DATA##");
			var publisher = new WebSocketPublisher(new System.Collections.Generic.Dictionary<string, Variant>
			{
				{ "Port", new Variant(port.ToString()) },
				{ "Template", new Variant(template) },
				{ "Publish", new Variant("base64") },
				{ "Timeout", new Variant("5000") },
			});

			using var client = new ClientWebSocket();
			try
			{
				publisher.start();
				await client.ConnectAsync(new System.Uri("ws://127.0.0.1:" + port + "/"), CancellationToken.None);
				await SendAsync(client, "{\"msg\":\"Client ready\"}");
				publisher.open();

				var output = Task.Run(() => publisher.output(new BitStream(System.Text.Encoding.ASCII.GetBytes("Hello"))));
				var buffer = new byte[4096];
				var result = await client.ReceiveAsync(new System.ArraySegment<byte>(buffer), CancellationToken.None);
				var json = JObject.Parse(System.Text.Encoding.UTF8.GetString(buffer, 0, result.Count));

				Assert.AreEqual("template", (string)json["type"]);
				Assert.AreEqual("payload=SGVsbG8=", (string)json["content"]);

				await SendAsync(client, "{\"msg\":\"Evaluation complete\"}");
				await output;
			}
			finally
			{
				publisher.close();
				publisher.stop();
				File.Delete(template);
			}
		}

		static Task SendAsync(ClientWebSocket client, string text)
		{
			var bytes = System.Text.Encoding.UTF8.GetBytes(text);
			return client.SendAsync(new System.ArraySegment<byte>(bytes), WebSocketMessageType.Text,
				true, CancellationToken.None);
		}

		[Test]
		public void TestCreate()
		{
			var tmp = Path.GetTempFileName();

			string xml = @"
<Peach>
	<DataModel name='DM'>
		<String value='Hello World' />
	</DataModel>

	<StateModel name='SM' initialState='Initial'>
		<State name='Initial'>
			<!-- No actions, just want to construct/desctuct the publisher -->
		</State>
	</StateModel>

	<Test name='Default'>
		<StateModel ref='SM' />
		<Publisher class='WebSocket'>
			<Param name='Template' value='{0}' />
		</Publisher>
	</Test>
</Peach>
";

			var template =
@"<html>
<body>
</body>
</html>";

			File.WriteAllBytes(tmp, Encoding.ASCII.GetBytes(template));
			xml = xml.Fmt(tmp);

			try
			{
				var dom = DataModelCollector.ParsePit(xml);

				var config = new RunConfiguration()
				{
					singleIteration = true,
				};

				var e = new Engine(null);
				e.startFuzzing(dom, config);
			}
			finally
			{
				File.Delete(tmp);
			}
		}
	}
}
