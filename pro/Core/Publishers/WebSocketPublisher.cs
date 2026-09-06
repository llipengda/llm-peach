using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Newtonsoft.Json.Linq;
using NLog;
using Peach.Core;
using Peach.Core.IO;

namespace Peach.Pro.Core.Publishers
{
	[Publisher("WebSocket")]
	[Description("WebSocket Publisher")]
	[Parameter("Port", typeof(int), "Port to listen for connections on", "8080")]
	[Parameter("Template", typeof(string), "Data template for publishing")]
	[Parameter("Publish", typeof(string), "How to publish data, base64 or url.", "base64")]
	[Parameter("DataToken", typeof(string), "Token to replace with data in template", "##DATA##")]
	[Parameter("Timeout", typeof(int), "Time in milliseconds to wait for client response", "60000")]
	public class WebSocketPublisher : Publisher
	{
		private static readonly NLog.Logger logger = LogManager.GetCurrentClassLogger();
		protected override NLog.Logger Logger { get { return logger; } }

		readonly HttpListener _socketServer = new HttpListener();
		readonly BufferBlock<string> _msgQueue = new BufferBlock<string>();
		readonly AutoResetEvent _evaluated = new AutoResetEvent(false);
		readonly ManualResetEvent _clientReady = new ManualResetEvent(false);
		readonly CancellationTokenSource _cancelAccept = new CancellationTokenSource();

		public int Port { get; protected set; }
		public string Template { get; protected set; }
		public string Publish { get; protected set; }
		public string DataToken { get; protected set; }
		public int Timeout { get; protected set; }

		readonly string _template;
		readonly JObject _jsonTemplateMessage = new JObject();

		public WebSocketPublisher(Dictionary<string, Variant> args)
			: base(args)
		{
			_template = File.ReadAllText(Template);
			_jsonTemplateMessage["type"] = "template";
		}

		static async Task AcceptWebSocketClientAsync(HttpListener server, CancellationToken token,
			BufferBlock<string> queue, EventWaitHandle clientReady, EventWaitHandle evaluated)
		{
			CancellationTokenSource cancelConnection = null;
			Task reader = null;
			Task writer = null;

			try
			{
				while (!token.IsCancellationRequested)
				{
					var context = await server.GetContextAsync().WaitAsync(token).ConfigureAwait(false);
					if (!context.Request.IsWebSocketRequest)
					{
						context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
						context.Response.Close();
						continue;
					}

					var accepted = await context.AcceptWebSocketAsync(null).ConfigureAwait(false);
					var ws = accepted.WebSocket;

					if (cancelConnection != null)
					{
						logger.Debug("New web socket connection. Closing down existing connection.");
						cancelConnection.Cancel();
						try
						{
							await Task.WhenAll(reader, writer)
								.WaitAsync(TimeSpan.FromSeconds(1)).ConfigureAwait(false);
						}
						catch { }
					}
					else
					{
						logger.Debug("New web socket connection");
					}

					cancelConnection = CancellationTokenSource.CreateLinkedTokenSource(token);
					reader = HandleConnectionAsync(ws, cancelConnection.Token, clientReady, evaluated);
					writer = HandleSendQueueAsync(ws, cancelConnection.Token, queue);
				}
			}
			catch (OperationCanceledException) when (token.IsCancellationRequested) { }
			catch (Exception ex)
			{
				logger.Debug("Error accepting clients: {0}", ex.GetBaseException().Message);
			}
			finally
			{
				if (cancelConnection != null)
					cancelConnection.Cancel();
				logger.Debug("Server stopped accepting clients");
			}
		}

		static async Task HandleConnectionAsync(WebSocket ws, CancellationToken cancellation,
			EventWaitHandle clientReady, EventWaitHandle evaluated)
		{
			var buffer = new byte[4096];
			try
			{
				while (ws.State == WebSocketState.Open && !cancellation.IsCancellationRequested)
				{
					using var message = new MemoryStream();
					WebSocketReceiveResult result;
					do
					{
						result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), cancellation).ConfigureAwait(false);
						if (result.MessageType == WebSocketMessageType.Close)
							return;
						message.Write(buffer, 0, result.Count);
					} while (!result.EndOfMessage);

					if (result.MessageType != WebSocketMessageType.Text)
						continue;

					var text = System.Text.Encoding.UTF8.GetString(message.ToArray());
					logger.Trace("NewMessageReceived: {0}", text);
					var json = JObject.Parse(text);
					if ((string)json["msg"] == "Client ready")
						clientReady.Set();
					else if ((string)json["msg"] == "Evaluation complete")
						evaluated.Set();
					else
						logger.Debug("Unknown message received: {0}", text);
				}
			}
			catch (OperationCanceledException) when (cancellation.IsCancellationRequested) { }
			catch (Exception ex)
			{
				logger.Debug("Error handling connection: {0}", ex.GetBaseException().Message);
			}
			finally
			{
				clientReady.Reset();
				ws.Dispose();
			}
		}

		static async Task HandleSendQueueAsync(WebSocket ws, CancellationToken cancellation,
			BufferBlock<string> queue)
		{
			try
			{
				while (ws.State == WebSocketState.Open && !cancellation.IsCancellationRequested)
				{
					var message = await queue.ReceiveAsync(cancellation).ConfigureAwait(false);
					var bytes = System.Text.Encoding.UTF8.GetBytes(message);
					await ws.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text,
						true, cancellation).ConfigureAwait(false);
				}
			}
			catch (OperationCanceledException) when (cancellation.IsCancellationRequested) { }
			catch (Exception ex)
			{
				logger.Debug("Error handling send queue: {0}", ex.GetBaseException().Message);
			}
			finally
			{
				ws.Dispose();
			}
		}

		protected override void OnStart()
		{
			base.OnStart();
			_socketServer.Prefixes.Add("http://*:" + Port + "/");
			_socketServer.Start();
			_ = AcceptWebSocketClientAsync(_socketServer, _cancelAccept.Token,
				_msgQueue, _clientReady, _evaluated);
		}

		protected override void OnStop()
		{
			_cancelAccept.Cancel();
			_socketServer.Close();
			base.OnStop();
		}

		protected override void OnOpen()
		{
			base.OnOpen();
			_msgQueue.TryReceiveAll(out IList<string> _);
			_evaluated.Reset();
			if (!_clientReady.WaitOne(Timeout))
				throw new SoftException("Timeout waiting for web socket connection.");
		}

		protected override void OnOutput(BitwiseStream data)
		{
			_jsonTemplateMessage["content"] = BuildTemplate(data);
			_msgQueue.Post(_jsonTemplateMessage.ToString(Newtonsoft.Json.Formatting.None) + "\n");

			var sw = Stopwatch.StartNew();
			while (sw.ElapsedMilliseconds < Timeout)
			{
				if (!_clientReady.WaitOne(0))
					throw new SoftException("Web socket connection lost.");
				if (_evaluated.WaitOne(200))
					return;
			}
			throw new SoftException("Timeout waiting for WebSocket evaluated.");
		}

		protected string BuildTemplate(BitwiseStream data)
		{
			var value = Publish;
			if (Publish == "base64")
			{
				data.Seek(0, SeekOrigin.Begin);
				value = Convert.ToBase64String(new BitReader(data).ReadBytes((int)data.Length));
			}
			return _template.Replace(DataToken, value);
		}
	}
}
