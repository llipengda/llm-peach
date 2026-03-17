using System;
using System.Linq;
using Peach.Core;
using Peach.Core.Dom;
using Peach.Core.Cracker;
using Peach.Core.IO;
using Peach.Pro.Core;
using System.Collections.Generic;
using System.IO;
using Peach.LLM.Core.Mutators;

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
        }

        enum TestResult
        {
            Pass,
            Fail
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

        static Dictionary<TestInfo, List<TestResult>> testResults = new Dictionary<TestInfo, List<TestResult>>();

        static readonly List<TestInfo> tests = new List<TestInfo>();

        static void Main(string[] args)
        {
            if (args.Length != 3)
            {
                Console.WriteLine("Usage: MutatorsTest <pitFilePath> <dataFilePath> <dataModelName>");
                return;
            }

            string pitFilePath = args[0];
            string targetFilePath = args[1];
            string dataModelName = args[2];

            FindMutators();

            DataElement root = null;

            try
            {
                root = ParseData(pitFilePath, targetFilePath, dataModelName);
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
                        tests.Add(new TestInfo()
                        {
                            Element = elem,
                            MutatorType = mutatorType
                        });
                    }
                }
            }

            Console.WriteLine($"Found {tests.Count} tests to run.");

            Test(root);

            ShowResults();
        }

        static readonly int testIterations = 100;

        static void Test(DataElement root)
        {
            int t = 0;
            foreach (var test in tests)
            {
                t++;
                testResults.Add(test, new List<TestResult>());
                for (int i = 0; i < testIterations; i++)
                {
                    if (i % 10 == 0)
                        WriteColored($"TESTING [{t:000}/{tests.Count:000}][{i + 1:000}/{testIterations}] {test.MutatorType.Name} on {test.Element.fullName}\n", ConsoleColor.Yellow);
                    var rootClone = ObjectCopier.Clone(root) as DM;
                    var elem = rootClone.find(test.Element.fullName);
                    if (!(Activator.CreateInstance(test.MutatorType, new object[] { elem }) is LLMMutator mutator))
                    {
                        Console.WriteLine($"Failed to create mutator of type {test.MutatorType.FullName} for element {elem.fullName}");
                        testResults[test].Add(TestResult.Fail);
                        continue;
                    }
                    mutator.context = new MockStrategy();
                    mutator.randomMutation(elem);
                    var v = rootClone.Value;
                    var dataCracker = new DataCracker();
                    try
                    {
                        var dataModelClone = ObjectCopier.Clone(originalDataModel);
                        dataModelClone.dom = dom;
                        dataCracker.CrackData(dataModelClone, new BitStream(v));
                        testResults[test].Add(TestResult.Pass);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Mutator {test.MutatorType.Name} failed to re-crack data for element {elem.fullName}: {ex.Message}");
                        testResults[test].Add(TestResult.Fail);
                    }
                }
            }
        }

        static void ShowResults()
        {
            Console.WriteLine("Mutator Test Results:");
            var res = testResults.GroupBy(kvp => kvp.Key.MutatorType.Name).Select(g => new
            {
                Mutator = g.Key,
                Pass = g.Sum(kvp => kvp.Value.Count(r => r == TestResult.Pass)),
                Fail = g.Sum(kvp => kvp.Value.Count(r => r == TestResult.Fail)),
                Total = g.Sum(kvp => kvp.Value.Count()),
                Status = g.Any(kvp => kvp.Value.Any(r => r == TestResult.Fail)) ? "FAIL" : "PASS"
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
                    else
                    {
                        WriteColored($"[FAIL][{r.Pass:000}/{r.Total:000}]", ConsoleColor.Red);
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

        static void WriteColored(string message, ConsoleColor color)
        {
            var originalColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.Write(message);
            Console.ForegroundColor = originalColor;
        }

        static void FindMutators()
        {
            var m = typeof(LLMMutator).Assembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(LLMMutator)));
            mutators.AddRange(m);
            mutators.Sort((a, b) => a.Name.CompareTo(b.Name));
        }

        static DataElement ParseData(string pitFile, string dataFile, string modelName)
        {
            ClassLoader.Initialize();

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