// Copyright (c) Peach Fuzzer, LLC
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NLog;
using NLog.Config;
using NLog.Targets;
using Peach.Core;
using LogLevel = NLog.LogLevel;
using TextEncoding = System.Text.Encoding;

namespace Peach.Pro.Core.Agent.Channels.Rest
{
	internal sealed class WireLogEvent
	{
		public long Id { get; set; }
		public string Level { get; set; }
		public string LoggerName { get; set; }
		public string Message { get; set; }
	}

	internal sealed class NLogLevelConverter : JsonConverter
	{
		public static readonly NLogLevelConverter Instance = new NLogLevelConverter();
		public override bool CanConvert(Type objectType) { return objectType == typeof(LogLevel); }
		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			var name = serializer.Deserialize<string>(reader);
			return name == null ? null : LogLevel.FromString(name);
		}
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			serializer.Serialize(writer, value == null ? null : ((LogLevel)value).Name);
		}
	}

	internal sealed class LogHandler : IDisposable
	{
		readonly List<LogResponse> _responses = new List<LogResponse>();
		readonly LogTarget _target = new LogTarget { Name = "RestLogTarget" };
		readonly LoggingRule _rule;
		readonly RouteHandler _routes;

		public LogHandler(RouteHandler routes)
		{
			_routes = routes;
			_rule = new LoggingRule("*", LogLevel.Trace, _target);
			var config = LogManager.Configuration;
			config.AddTarget(_target.Name, _target);
			config.LoggingRules.Add(_rule);
			LogManager.Configuration = config;
			_routes.Add(Server.LogPath, "GET", OnSubscribe);
		}

		public void Dispose()
		{
			var config = LogManager.Configuration;
			config.LoggingRules.Remove(_rule);
			config.RemoveTarget(_target.Name);
			LogManager.Configuration = config;
			lock (_responses)
			{
				foreach (var response in _responses.ToArray()) response.Dispose();
				_responses.Clear();
			}
			_target.Dispose();
			_routes.Remove(Server.LogPath);
		}

		RouteResponse OnSubscribe(HttpListenerRequest request)
		{
			if (!request.IsWebSocketRequest) return RouteResponse.Success();
			var name = request.QueryString.Get("level");
			var level = string.IsNullOrEmpty(name) ? LogLevel.Info : LogLevel.FromString(name);
			LogResponse response = null;
			response = new LogResponse(_target, level, () => { lock (_responses) _responses.Remove(response); });
			lock (_responses) _responses.Add(response);
			return response;
		}
	}

	internal sealed class LogResponse : RouteResponse, IDisposable
	{
		readonly LogTarget _target;
		readonly LogLevel _level;
		readonly Action _closed;
		readonly object _sendLock = new object();
		WebSocket _socket;
		long _counter;

		public LogResponse(LogTarget target, LogLevel level, Action closed)
		{
			_target = target; _level = level; _closed = closed;
		}

		public override void Complete(HttpListenerContext context)
		{
			_socket = context.AcceptWebSocketAsync("log").GetAwaiter().GetResult().WebSocket;
			_target.Add(this);
			try
			{
				var buffer = new byte[1024];
				while (_socket.State == WebSocketState.Open)
				{
					var result = _socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None).GetAwaiter().GetResult();
					if (result.MessageType == WebSocketMessageType.Close) break;
					SendFlush();
				}
			}
			catch (WebSocketException)
			{
				// A client process can disappear without completing the close handshake.
			}
			catch (ObjectDisposedException)
			{
			}
			finally { _target.Remove(this); _closed(); Dispose(); }
		}

		public void Log(LogEventInfo item)
		{
			if (item.Level < _level) return;
			Send(JsonConvert.SerializeObject(new WireLogEvent
			{
				Id = Interlocked.Increment(ref _counter) - 1,
				Level = item.Level.Name,
				LoggerName = item.LoggerName,
				Message = item.FormattedMessage,
			}));
		}

		void SendFlush()
		{
			Send(JsonConvert.SerializeObject(new WireLogEvent { Id = -1 }));
		}

		void Send(string value)
		{
			lock (_sendLock)
			{
				if (_socket == null || _socket.State != WebSocketState.Open) return;
				var bytes = TextEncoding.UTF8.GetBytes(value);
				_socket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None).GetAwaiter().GetResult();
			}
		}

		public void Dispose()
		{
			lock (_sendLock)
			{
				if (_socket == null) return;
				try { _socket.Abort(); } catch { }
				_socket.Dispose(); _socket = null;
			}
		}
	}

	internal sealed class LogTarget : TargetWithLayout
	{
		readonly List<LogResponse> _responses = new List<LogResponse>();
		public void Add(LogResponse response) { lock (_responses) _responses.Add(response); }
		public void Remove(LogResponse response) { lock (_responses) _responses.Remove(response); }
		protected override void Write(LogEventInfo item)
		{
			if (item.Properties.ContainsKey("PreventLoop")) return;
			LogResponse[] copy;
			lock (_responses) copy = _responses.ToArray();
			foreach (var response in copy) response.Log(item);
		}
	}

	internal sealed class LogSink : IDisposable
	{
		static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();
		readonly string _name;
		readonly NLog.Logger _chain;
		readonly SortedDictionary<long, LogEventInfo> _pending = new SortedDictionary<long, LogEventInfo>();
		readonly ManualResetEventSlim _flushed = new ManualResetEventSlim(false);
		ClientWebSocket _socket;
		CancellationTokenSource _cancel;
		Task _receiver;
		long _expectId;

		public LogSink(string name) { _name = name; _chain = LogManager.GetLogger("Agent." + name); }

		public void Start(Uri baseUri)
		{
			Stop(); _expectId = 0; _pending.Clear(); _flushed.Reset();
			_cancel = new CancellationTokenSource();
			_socket = new ClientWebSocket();
			_socket.Options.AddSubProtocol("log");
			var uri = new Uri("ws://{0}:{1}{2}?level={3}".Fmt(baseUri.Host, baseUri.Port, Server.LogPath, Configuration.LogLevel));
			_socket.ConnectAsync(uri, _cancel.Token).GetAwaiter().GetResult();
			_receiver = Task.Run(() => Receive(_cancel.Token));
		}

		public void Stop()
		{
			if (_socket == null) return;
			try
			{
				if (_socket.State == WebSocketState.Open)
				{
					var bytes = TextEncoding.UTF8.GetBytes("Flush");
					_socket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None).GetAwaiter().GetResult();
					if (!_flushed.Wait(TimeSpan.FromSeconds(10))) Logger.Warn("Timeout waiting for remote logging service to flush");
					_socket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "Done", CancellationToken.None).GetAwaiter().GetResult();
				}
			}
			catch (Exception ex) { Logger.Debug(ex.Message); }
			finally { DisposeSocket(); }
		}

		async Task Receive(CancellationToken token)
		{
			var buffer = new byte[8192];
			try
			{
				while (!token.IsCancellationRequested && _socket.State == WebSocketState.Open)
				{
					using (var stream = new MemoryStream())
					{
						WebSocketReceiveResult result;
						do
						{
							result = await _socket.ReceiveAsync(new ArraySegment<byte>(buffer), token).ConfigureAwait(false);
							if (result.MessageType == WebSocketMessageType.Close) return;
							stream.Write(buffer, 0, result.Count);
						} while (!result.EndOfMessage);
						Process(TextEncoding.UTF8.GetString(stream.ToArray()));
					}
				}
			}
			catch (OperationCanceledException) { }
			catch (Exception ex) { Logger.Debug(ex.Message); }
		}

		void Process(string json)
		{
			var wire = JsonConvert.DeserializeObject<WireLogEvent>(json);
			lock (_pending)
			{
				var id = wire.Id;
				if (id == -1)
				{
					foreach (var entry in _pending) Forward(entry.Value);
					_pending.Clear(); _flushed.Set(); return;
				}
				var item = new LogEventInfo(LogLevel.FromString(wire.Level), wire.LoggerName, wire.Message);
				if (!_pending.ContainsKey(id)) _pending.Add(id, item);
				while (_pending.TryGetValue(_expectId, out item))
				{
					_pending.Remove(_expectId++); Forward(item);
				}
			}
		}

		void Forward(LogEventInfo item)
		{
			item.LoggerName = "[{0}] {1}".Fmt(_name, item.LoggerName);
			item.Properties["PreventLoop"] = true; _chain.Log(item);
		}

		public void Dispose() { Stop(); _flushed.Dispose(); }
		void DisposeSocket()
		{
			_cancel?.Cancel();
			try { _receiver?.Wait(TimeSpan.FromSeconds(1)); } catch { }
			_socket?.Dispose(); _cancel?.Dispose();
			_socket = null; _cancel = null; _receiver = null;
		}
	}
}
