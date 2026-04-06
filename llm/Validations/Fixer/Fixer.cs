using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Peach.Core;
using Peach.Core.Cracker;
using Peach.Core.Dom;
using Peach.Core.IO;
using Peach.Pro.Core;

namespace Peach.LLM.Validations.Fixer
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public sealed class FixerTestAttribute : Attribute
	{
		public string Name { get; private set; }

		public FixerTestAttribute(string name)
		{
			if (string.IsNullOrWhiteSpace(name))
				throw new ArgumentException("Test name is required.", "name");

			Name = name;
		}
	}

	public class FixerValidator
	{
		public sealed class FixerTestResult
		{
			public string Name { get; private set; }
			public bool Passed { get; private set; }
			public Exception Error { get; private set; }

			private FixerTestResult(string name, bool passed, Exception error)
			{
				Name = name;
				Passed = passed;
				Error = error;
			}

			public static FixerTestResult Success(string name)
			{
				return new FixerTestResult(name, true, null);
			}

			public static FixerTestResult Failure(string name, Exception error)
			{
				return new FixerTestResult(name, false, error);
			}
		}

		private readonly string _pitFile;
		private readonly string _dataModelName;
		private readonly Dom _dom;
		private readonly Peach.Core.Dom.DataModel _dataModel;

		public FixerValidator(string pitFile, string dataModelName)
		{
			if (string.IsNullOrWhiteSpace(pitFile))
				throw new ArgumentException("PIT file path is required.", "pitFile");

			if (string.IsNullOrWhiteSpace(dataModelName))
				throw new ArgumentException("DataModel name is required.", "dataModelName");

			ClassLoader.Initialize("./Plugins");

			_pitFile = pitFile;
			_dataModelName = dataModelName;

			var parser = new ProPitParser(null, null, _pitFile);
			var parserArgs = new Dictionary<string, object>();
			_dom = parser.asParser(parserArgs, _pitFile);

			if (!_dom.dataModels.Contains(_dataModelName))
				throw new ArgumentException(string.Format("Failed to find DataModel '{0}' in Pit File.", _dataModelName), "dataModelName");

			_dataModel = _dom.dataModels[_dataModelName];
		}

		public DataElement Parse(byte[] data)
		{
			if (data == null)
				throw new ArgumentNullException("data");

			var dataStream = new BitStream(data);
			var crackedModel = ObjectCopier.Clone(_dataModel);
			crackedModel.dom = _dom;

			var cracker = new DataCracker();
			cracker.CrackData(crackedModel, dataStream);

			return crackedModel;
		}

		public IList<FixerTestResult> RunTests()
		{
			var methods = GetType()
				.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
				.Where(m => m.GetCustomAttribute<FixerTestAttribute>() != null)
				.OrderBy(m => m.Name)
				.ToArray();

			if (methods.Length == 0)
				return new List<FixerTestResult>();

			var results = new ConcurrentBag<FixerTestResult>();
			var tasks = methods.Select(method => Task.Run(() =>
			{
				var attr = method.GetCustomAttribute<FixerTestAttribute>();
				var name = attr == null ? method.Name : attr.Name;

				try
				{
					InvokeFixerTest(method);
					results.Add(FixerTestResult.Success(name));
				}
				catch (Exception ex)
				{
					results.Add(FixerTestResult.Failure(name, UnwrapException(ex)));
				}
			})).ToArray();

			Task.WaitAll(tasks);

			return results
				.OrderBy(r => r.Name)
				.ToList();
		}

		private static Exception UnwrapException(Exception ex)
		{
			var targetException = ex as TargetInvocationException;
			if (targetException != null && targetException.InnerException != null)
				return targetException.InnerException;

			var aggregateException = ex as AggregateException;
			if (aggregateException != null)
			{
				var flattened = aggregateException.Flatten();
				if (flattened.InnerExceptions.Count == 1)
					return UnwrapException(flattened.InnerExceptions[0]);
			}

			return ex;
		}

		private void InvokeFixerTest(MethodInfo method)
		{
			if (method.GetParameters().Length != 0)
				throw new InvalidOperationException(string.Format("Fixer test method '{0}' cannot have parameters.", method.Name));

			var result = method.Invoke(this, null);

			if (result == null)
				return;

			var task = result as Task;
			if (task != null)
			{
				task.GetAwaiter().GetResult();
				return;
			}

			throw new InvalidOperationException(string.Format("Fixer test method '{0}' must return void or Task.", method.Name));
		}
	}
}
