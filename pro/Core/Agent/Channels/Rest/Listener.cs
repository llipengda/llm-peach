//
// Copyright (c) Peach Fuzzer, LLC
//

using System;
using System.Linq;
using System.Net;
using System.Threading;
using NLog;
using Peach.Core;
using Logger = NLog.Logger;

namespace Peach.Pro.Core.Agent.Channels.Rest
{
	internal class Listener : IDisposable
	{
		private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

		private HttpListener _listener;
		private ManualResetEvent _event;

		public RouteHandler Routes { get; private set; }
		public Uri Uri { get; private set; }

		public static Listener Create(string prefix)
		{
			return new Listener(MakeListener(prefix));
		}

		public void Dispose()
		{
			if (_listener != null)
			{
				_listener.Close();
				_listener = null;
			}

			if (_event != null)
			{
				_event.Dispose();
				_event = null;
			}
		}

		public void Start()
		{
			_listener.Start();
			while (!_event.WaitOne(0))
			{
				HttpListenerContext context;
				try
				{
					context = _listener.GetContext();
				}
				catch (HttpListenerException) when (_event.WaitOne(0))
				{
					break;
				}
				catch (ObjectDisposedException) when (_event.WaitOne(0))
				{
					break;
				}

				ThreadPool.QueueUserWorkItem(_ => ProcessContext(context));
			}
		}

		public void Stop()
		{
			_event.Set();
			if (_listener != null && _listener.IsListening)
				_listener.Stop();
		}

		private Listener(HttpListener listener)
		{
			_listener = listener;
			_event = new ManualResetEvent(false);

			Uri = new Uri(_listener.Prefixes.First().Replace("+", Environment.MachineName));
			Routes = new RouteHandler();
		}

		private void ProcessContext(HttpListenerContext ctx)
		{
			Logger.Trace(">>> {0} {1}", ctx.Request.HttpMethod, ctx.Request.RawUrl);

			var response = Routes.Dispatch(ctx.Request);

			try
			{
				response.Complete(ctx);
			}
			finally
			{
				Logger.Trace("<<< {0} {1}", (int)response.StatusCode, response.StatusCode);
			}
		}

		private static HttpListener MakeListener(string prefix)
		{
			// If the listener fails to start it is disposed so we
			// need to make a new one each time.
			var ret = new HttpListener
			{
				IgnoreWriteExceptions = true
			};

			ret.Prefixes.Add(prefix);

			try
			{
				ret.Start();

				return ret;
			}
			catch (System.Net.HttpListenerException ex)
			{
				throw new PeachException("An error occurred starting the HTTP listener.", ex);
			}
		}
	}
}
