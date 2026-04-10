using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Peach.Core;
using Peach.LLM.Validations.Common;
using NLog;

namespace Peach.LLM.Validations.Fixer
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

	public class Program
	{
		private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

		static void RunDataTest(string pitFile, string dataModelName, byte[] data)
		{
			var parser = new DataParser(pitFile, dataModelName);
			try
			{
				parser.Parse(data);
			} 
			catch (Exception ex)
			{
				Console.Error.WriteLine($"[FAIL] Data test failed: {ex.Message}");
				Environment.Exit(1);
			}
			Console.Error.WriteLine("[PASS] Data test passed.");
			Environment.Exit(0);
		}

		public static void Main(string[] args)
		{
            ClassLoader.Initialize("./Plugins");

			InitializeLogging("fixer.log");

			if (args.Length == 4 && args[0] == "-d")
			{
				var pitFile = args[1];
				var dataModelName = args[2];
				var hexData = args[3];
				byte[] data = new byte[hexData.Length / 2];
				for (int i = 0; i < data.Length; i++)
				{
					data[i] = Convert.ToByte(hexData.Substring(i * 2, 2), 16);
				}
				RunDataTest(pitFile, dataModelName, data);
				return;
			} 
			else if (args.Length != 0)
			{
				Console.WriteLine("Usage:");
				Console.WriteLine("  Fixer.exe                                         - Run all fixer tests");
				Console.WriteLine("  Fixer.exe -d <pitFile> <dataModelName> <hexData>  - Run a single data test");
				Environment.Exit(1);
			}

			Logger.Info("Starting fixer tests.");

			var results = RunTests();

			SetLogFileName("fixer.log");

			if (results.Count == 0)
			{
				Console.WriteLine("No fixer tests found.");
				Logger.Warn("No fixer tests found.");
				return;
			}

			foreach (var result in results)
			{
				if (result.Passed)
				{
					Console.WriteLine($"[PASS] {result.Name}");
					continue;
				}

				Console.WriteLine($"[FAIL] {result.Name}: {result.Error?.Message}");
			}

			var passedCount = results.Count(r => r.Passed);
			Console.WriteLine($"[{(passedCount == results.Count ? "PASS" : "FAIL")}] {passedCount:000}/{results.Count:000} tests passed.");
			Logger.Info($"[{(passedCount == results.Count ? "PASS" : "FAIL")}] {passedCount:000}/{results.Count:000} tests passed.");
			Environment.ExitCode = passedCount == results.Count ? 0 : 1;
		}

		public static IList<FixerTestResult> RunTests()
		{
			var assemblies = AppDomain.CurrentDomain.GetAssemblies();
			var allTypes = new List<Type>();

			foreach (var assembly in assemblies)
			{
				try
				{
					allTypes.AddRange(assembly.GetTypes());
				}
				catch (ReflectionTypeLoadException ex)
				{
					allTypes.AddRange(ex.Types.Where(t => t != null));
				}
				catch
				{
					// Ignore assemblies that cannot be reflected.
				}
			}

			var methods = allTypes.SelectMany(t =>
					t.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
					.Where(m => m.GetCustomAttribute<FixerTestAttribute>() != null)
					.OrderBy(m => m.Name)
				).ToArray();

			if (methods.Length == 0)
			{
				Logger.Warn("No fixer test methods were discovered.");
				return new List<FixerTestResult>();
			}

			Logger.Info($"Discovered {methods.Length} fixer test methods.");

			var results = new List<FixerTestResult>();
			foreach (var method in methods)
			{
				var attr = method.GetCustomAttribute<FixerTestAttribute>();
				var name = attr == null ? method.Name : attr.Name;
				var logFileName = SanitizeFileName(name) + ".log";
				var logFilePath = Path.Combine("/logs", logFileName);

				SetLogFileName(logFileName);
				Logger.Info($"Running fixer test: {name}");

				try
				{
					InvokeFixerTest(method);
					Logger.Info($"[PASS] {name}");
					LogManager.Flush();
					TryDeleteLogFile(logFilePath);
					results.Add(FixerTestResult.Success(name));
				}
				catch (Exception ex)
				{
					var unwrapped = UnwrapException(ex);
					Logger.Error(unwrapped, $"[FAIL] {name}: {unwrapped.Message}");
					LogManager.Flush();
					results.Add(FixerTestResult.Failure(name, unwrapped));
				}
			}

			var passedCount = results.Count(r => r.Passed);
			Logger.Info($"[{(passedCount == results.Count ? "PASS" : "FAIL")}] {passedCount:000}/{results.Count:000} tests passed.");

			return results
				.OrderBy(r => r.Name)
				.ToList();
		}

		private static void InitializeLogging(string logFileName)
		{
			var configuration = LogManager.Configuration;
			if (configuration == null)
				return;

			if (configuration.Variables.ContainsKey("logFileName"))
			{
				configuration.Variables["logFileName"] = logFileName;
				LogManager.ReconfigExistingLoggers();
			}
		}

		private static void SetLogFileName(string logFileName)
		{
			var configuration = LogManager.Configuration;
			if (configuration == null)
				return;

			if (configuration.Variables.ContainsKey("logFileName"))
			{
				configuration.Variables["logFileName"] = logFileName;
				LogManager.ReconfigExistingLoggers();
			}
		}

		private static void TryDeleteLogFile(string logFilePath)
		{
			if (File.Exists(logFilePath))
				File.Delete(logFilePath);
		}

		private static string SanitizeFileName(string name)
		{
			var invalidChars = Path.GetInvalidFileNameChars();
			var chars = name.Select(ch => invalidChars.Contains(ch) ? '_' : ch).ToArray();
			return new string(chars);
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

		private static void InvokeFixerTest(MethodInfo method)
		{
			if (method.GetParameters().Length != 0)
				throw new InvalidOperationException(string.Format("Fixer test method '{0}' cannot have parameters.", method.Name));

			var result = method.Invoke(null, null);

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
