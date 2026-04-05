using System;
using System.Linq;
using Peach.Core;
using Peach.Core.Dom;
using Peach.Core.Cracker;
using Peach.Core.IO;
using Peach.Pro.Core;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Peach.LLM.Core.Mutators;
using NLog;

using DM = Peach.Core.Dom.DataModel;

namespace Peach.LLM.Validations.Mutator
{
    class Program
    {
        static readonly List<Type> mutators = new List<Type>();

        static DM originalDataModel = null;

        static Dom dom = null;

        class TestInfo
        {
            public DataElement Element;
            public Type MutatorType;
            public string DataFile;
        }

        enum TestResult
        {
            Pass,
            Fail,
            Error
        }

        class TestLogInfo
        {
            public string DataFile;
            public string ElementFullName;
            public int Iteration;
            public string Message;
            public string StackTrace;
        }

        class MockStrategy : MutationStrategy
        {
            public MockStrategy() : base(new Dictionary<string, Variant>())
            {
                var context = new RunContext();
                context.config = new RunConfiguration();
                Initialize(context, null);
                SeedRandom();
            }

            public override bool UsesRandomSeed => throw new NotImplementedException();

            public override bool IsDeterministic => throw new NotImplementedException();

            public override uint Count => throw new NotImplementedException();

            public override uint Iteration { get; set; } = 0;
        }

        static ConcurrentDictionary<TestInfo, List<TestResult>> testResults = new ConcurrentDictionary<TestInfo, List<TestResult>>();

        static ConcurrentDictionary<string, ConcurrentBag<TestLogInfo>> failLogs = new ConcurrentDictionary<string, ConcurrentBag<TestLogInfo>>();

        static ConcurrentDictionary<string, ConcurrentBag<TestLogInfo>> errorLogs = new ConcurrentDictionary<string, ConcurrentBag<TestLogInfo>>();

        static Dictionary<string, int> mutatorTestCounts = new Dictionary<string, int>();

        static readonly List<TestInfo> tests = new List<TestInfo>();

        static readonly NLog.Logger NLogger = LogManager.GetCurrentClassLogger();

        static string pitFilePath;

        static string dataModelName;

        static string dataFileFolder;

        static void Main(string[] args)
        {
            if (args.Length != 3 && args.Length != 4)
            {
                Console.WriteLine("Usage: Peach.LLM.Validations.Mutator <pitFilePath> <dataFileFolder> <dataModelName> [<testIterations>]");
                return;
            }

            ClassLoader.Initialize("./Plugins");

            pitFilePath = args[0];
            dataFileFolder = args[1];
            dataModelName = args[2];

            if (args.Length == 4)
            {
                if (!int.TryParse(args[3], out int iterations))
                {
                    Console.WriteLine($"Invalid test iterations value: {args[3]}");
                    return;
                }
                testIterations = iterations;
            }

            FindMutators();

            Console.WriteLine($"Found {mutators.Count} mutators to test.");

            if (!Directory.Exists(dataFileFolder))
            {
                Console.WriteLine($"Data file folder does not exist: {dataFileFolder}");
                return;
            }
            var dataFiles = Directory.GetFiles(dataFileFolder);
            Console.WriteLine($"Found {dataFiles.Length} data files to test.");

            Console.WriteLine($"Testing with {Environment.ProcessorCount} cpu cores");

            foreach (var dataFile in dataFiles)
            {
                Console.WriteLine($"Testing data file: {dataFile}");
                TestSingleFile(dataFile);
                if (mutators.Count <= mutatorTestCounts.Count)
                {
                    Console.WriteLine("All mutators have reached the test limit. Ending tests.");
                    break;
                }
            }

            ShowResults();
        }

        static void TestSingleFile(string dataFilePath)
        {
            DataElement root = null;

            try
            {
                root = ParseData(pitFilePath, dataFilePath, dataModelName);
                Console.WriteLine($"Root Element Name: {root?.Name}");
                Console.WriteLine($"Data Length: {root?.Value.Length}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unable to parse data: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                return;
            }

            foreach (var elem in root.PreOrderTraverse())
            {
                foreach (var mutatorType in mutators)
                {
                    var supportedDataElementFn = mutatorType.GetMethod("supportedDataElement", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
                    if (supportedDataElementFn == null)
                    {
                        Console.WriteLine($"Mutator {mutatorType.FullName} is missing static method 'supportedDataElement'");
                        continue;
                    }
                    bool isSupported = (bool)supportedDataElementFn.Invoke(null, new object[] { elem });
                    if (isSupported)
                    {
                        if (!mutatorTestCounts.ContainsKey(mutatorType.FullName))
                            mutatorTestCounts[mutatorType.FullName] = 0;
                        if (mutatorTestCounts[mutatorType.FullName] >= 1)
                            continue;
                        mutatorTestCounts[mutatorType.FullName]++;
                        tests.Add(new TestInfo()
                        {
                            Element = elem,
                            MutatorType = mutatorType,
                            DataFile = dataFilePath
                        });
                    }
                }
            }

            Console.WriteLine($"Found {tests.Count} tests to run in file {dataFilePath}.");

            Test(root);

            tests.Clear();
        }

        static readonly object consoleLock = new object();

        static int testIterations = 100;

        static void Test(DataElement root)
        {
            // Initialize test results for all tests first
            foreach (var test in tests)
            {
                testResults.TryAdd(test, new List<TestResult>());
            }

            // Run tests in parallel
            Parallel.ForEach(tests, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, (test, loopState) =>
            {
                var testIndex = tests.IndexOf(test) + 1;
                var testResultList = testResults[test];

                for (int i = 0; i < testIterations; i++)
                {
                    if (i % 10 == 0)
                    {
                        lock (consoleLock)
                        {
                            WriteColored($"TESTING [{testIndex:000}/{tests.Count:000}][{i + 1:000}/{testIterations}] {test.MutatorType.Name} on {test.Element.fullName}\n", ConsoleColor.Yellow);
                        }
                    }

                    var rootClone = ObjectCopier.Clone(root) as DM;
                    rootClone.dom = dom;
                    var elem = rootClone.find(test.Element.fullName);
                    if (!(Activator.CreateInstance(test.MutatorType, new object[] { elem }) is LLMMutator mutator))
                    {
                        lock (consoleLock)
                        {
                            Console.WriteLine($"Failed to create mutator of type {test.MutatorType.FullName} for element {elem.fullName}");
                        }
                        lock (testResultList)
                        {
                            testResultList.Add(TestResult.Error);
                        }
                        continue;
                    }
                    mutator.context = new MockStrategy();
                    BitwiseStream v;
                    try
                    {
                        mutator.randomMutation(elem);
                        v = rootClone.Value;
                    }
                    catch (Exception ex)
                    {
                        lock (consoleLock)
                        {
                            Console.WriteLine($"Mutator {test.MutatorType.Name} threw an exception during mutation for element {elem.fullName}: {ex.Message}");
                        }
                        var mutatorName = test.MutatorType.Name;
                        var logs = errorLogs.GetOrAdd(mutatorName, _ => new ConcurrentBag<TestLogInfo>());
                        logs.Add(new TestLogInfo
                        {
                            DataFile = test.DataFile,
                            ElementFullName = elem.fullName,
                            Iteration = i + 1,
                            Message = ex.Message,
                            StackTrace = ex.StackTrace
                        });
                        lock (testResultList)
                        {
                            testResultList.Add(TestResult.Error);
                        }
                        continue;
                    }
                    var dataCracker = new DataCracker();
                    try
                    {
                        var dataModelClone = ObjectCopier.Clone(originalDataModel);
                        dataModelClone.dom = dom;
                        dataCracker.CrackData(dataModelClone, new BitStream(v));
                        lock (testResultList)
                        {
                            testResultList.Add(TestResult.Pass);
                        }
                    }
                    catch (Exception ex)
                    {
                        lock (consoleLock)
                        {
                            Console.WriteLine($"Mutator {test.MutatorType.Name} failed to re-crack data for element {elem.fullName}: {ex.Message}");
                        }
                        var mutatorName = test.MutatorType.Name;
                        var logs = failLogs.GetOrAdd(mutatorName, _ => new ConcurrentBag<TestLogInfo>());
                        logs.Add(new TestLogInfo
                        {
                            DataFile = test.DataFile,
                            ElementFullName = elem.fullName,
                            Iteration = i + 1,
                            Message = ex.Message,
                            StackTrace = ex.StackTrace
                        });
                        lock (testResultList)
                        {
                            testResultList.Add(TestResult.Fail);
                        }
                    }
                }
            });
        }

        static void ShowResults()
        {
            WriteFailureAndErrorLogs();

            Console.WriteLine("Mutator Test Results:");
            var res = testResults.GroupBy(kvp => kvp.Key.MutatorType.Name).Select(g => new
            {
                Mutator = g.Key,
                Pass = g.Sum(kvp => kvp.Value.Count(r => r == TestResult.Pass)),
                Fail = g.Sum(kvp => kvp.Value.Count(r => r == TestResult.Fail)),
                Error = g.Sum(kvp => kvp.Value.Count(r => r == TestResult.Error)),
                Total = g.Sum(kvp => kvp.Value.Count()),
                Status = g.Any(kvp => kvp.Value.Any(r => r == TestResult.Error)) ? "ERROR" : (g.All(kvp => kvp.Value.All(r => r == TestResult.Pass)) ? "PASS" : "FAIL")
            }).ToDictionary(x => x.Mutator, x => x);

            foreach (var t in mutators)
            {
                if (res.TryGetValue(t.Name, out var r))
                {
                    if (r.Status == "PASS")
                    {
                        WriteColored($"[PASS][{r.Pass:000}/{r.Total:000}]", ConsoleColor.Green);
                        Console.WriteLine($" {t.Name}");
                    }
                    else if (r.Status == "FAIL")
                    {
                        WriteColored($"[FAIL][{r.Pass:000}/{r.Total:000}]", ConsoleColor.Red);
                        Console.WriteLine($" {t.Name}");
                    }
                    else
                    {
                        WriteColored($"[ERROR]", ConsoleColor.Magenta);
                        Console.WriteLine($" {t.Name}");
                    }
                }
                else
                {
                    WriteColored($"[SKIP][000/000]", ConsoleColor.Blue);
                    Console.WriteLine($" {t.Name}");
                }
            }
        }

        static void WriteFailureAndErrorLogs()
        {
            foreach (var mutator in mutators)
            {
                var mutatorName = mutator.Name;
                var safeMutatorName = SanitizeFileName(mutatorName);
                var failFilePath = Path.Combine("fail", $"{safeMutatorName}.log");
                var errorFilePath = Path.Combine("error", $"{safeMutatorName}.log");

                if (failLogs.TryGetValue(mutatorName, out var failEntries) && failEntries.Count > 0)
                {
                    WriteLogFileWithNLog(failFilePath, FormatLogContent(mutatorName, "FAIL", failEntries));
                }
                else if (File.Exists(failFilePath))
                {
                    File.Delete(failFilePath);
                }

                if (errorLogs.TryGetValue(mutatorName, out var errorEntries) && errorEntries.Count > 0)
                {
                    WriteLogFileWithNLog(errorFilePath, FormatLogContent(mutatorName, "ERROR", errorEntries));
                }
                else if (File.Exists(errorFilePath))
                {
                    File.Delete(errorFilePath);
                }
            }

            LogManager.Flush();
        }

        static void WriteLogFileWithNLog(string filePath, string content)
        {
            LogManager.Configuration.Variables["logFileName"] = filePath;
            LogManager.ReconfigExistingLoggers();

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            NLogger.Info(content);
        }

        static string FormatLogContent(string mutatorName, string status, IEnumerable<TestLogInfo> entries)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Mutator: {mutatorName}");
            sb.AppendLine($"Status: {status}");
            sb.AppendLine($"Count: {entries.Count()}");
            sb.AppendLine();

            foreach (var entry in entries
                .OrderBy(e => e.DataFile, StringComparer.Ordinal)
                .ThenBy(e => e.ElementFullName, StringComparer.Ordinal)
                .ThenBy(e => e.Iteration))
            {
                sb.AppendLine($"DataFile: {entry.DataFile}");
                sb.AppendLine($"Element: {entry.ElementFullName}");
                sb.AppendLine($"Iteration: {entry.Iteration}");
                sb.AppendLine($"Message: {entry.Message}");
                if (!string.IsNullOrEmpty(entry.StackTrace))
                {
                    sb.AppendLine("StackTrace:");
                    sb.AppendLine(entry.StackTrace);
                }
                sb.AppendLine(new string('-', 80));
            }

            return sb.ToString();
        }

        static string SanitizeFileName(string name)
        {
            var invalidChars = Path.GetInvalidFileNameChars();
            var chars = name.Select(ch => invalidChars.Contains(ch) ? '_' : ch).ToArray();
            return new string(chars);
        }

        static void WriteColored(string message, ConsoleColor color)
        {
            var originalColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.Write(message);
            Console.ForegroundColor = originalColor;
        }

        static void FindMutators()
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

            var m = allTypes
                .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(LLMMutator)));

            mutators.AddRange(m);
            var unique = mutators.Distinct().ToList();
            mutators.Clear();
            mutators.AddRange(unique);
            mutators.Sort((a, b) => a.Name.CompareTo(b.Name));
        }

        static DataElement ParseData(string pitFile, string dataFile, string modelName)
        {
            var parser = new ProPitParser(null, null, pitFile);

            var parserArgs = new Dictionary<string, object>();

            dom = parser.asParser(parserArgs, pitFile);

            Console.WriteLine("DOM parsed successfully");

            if (!dom.dataModels.Contains(modelName))
            {
                throw new Exception($"Failed to find DataModel '{modelName}' in Pit File.");
            }
            DM dataModel = dom.dataModels[modelName];

            if (!File.Exists(dataFile))
            {
                throw new FileNotFoundException($"Failed to find file : {dataFile}");
            }

            byte[] fileBytes = File.ReadAllBytes(dataFile);
            var dataStream = new BitStream(fileBytes);

            var crackedModel = ObjectCopier.Clone(dataModel);
            crackedModel.dom = dom;

            originalDataModel = dataModel;
            originalDataModel.dom = dom;

            var cracker = new DataCracker();

            cracker.CrackData(crackedModel, dataStream);

            return crackedModel;
        }

        // static string GetType(DataElement obj)
        // {
        //     var ans = new List<string>();
        //     if (obj is Peach.Core.Dom.Blob) ans.Add("Blob");
        //     if (obj is Peach.Core.Dom.String) ans.Add("String");
        //     if (obj is Peach.Core.Dom.Number) ans.Add("Number");
        //     if (obj is Peach.Core.Dom.Block) ans.Add("Block");
        //     if (obj is Peach.Core.Dom.Choice) ans.Add("Choice");
        //     if (obj is Peach.Core.Dom.Flags) ans.Add("Flags");
        //     if (obj is Peach.Core.Dom.Padding) ans.Add("Padding");
        //     if (obj is Peach.Core.Dom.Array) ans.Add("Array");
        //     if (ans.Count == 0) ans.Add("Unknown (" + obj.GetType().Name + ")");
        //     return string.Join("&", ans);
        // }
    }
}