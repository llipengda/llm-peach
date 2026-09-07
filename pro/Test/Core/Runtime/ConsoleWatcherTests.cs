using System;
using System.IO;
using NUnit.Framework;
using Peach.Core;
using Peach.Core.Test;
using Peach.Pro.Core.Runtime;

namespace Peach.Pro.Test.Core.Runtime
{
	[TestFixture]
	[Peach]
	[Quick]
	[NonParallelizable]
	class ConsoleWatcherTests
	{
		const string xml = @"
<Peach>
	<DataModel name='TheDataModel'>
		<String name='value' value='Hello World!' />
	</DataModel>

	<StateModel name='TheState' initialState='Initial'>
		<State name='Initial'>
			<Action type='output'>
				<DataModel ref='TheDataModel' />
			</Action>
		</State>
	</StateModel>

	<Test name='Default'>
		<StateModel ref='TheState' />
		<Publisher class='Null' />
		<Strategy class='Sequential' />
		<Mutators mode='include'>
			<Mutator class='DataElementRemove' />
			<Mutator class='StringCaseUpper' />
		</Mutators>
	</Test>
</Peach>";

		[Test]
		public void ReportsMutationBeforeAndAfterValuesOnSameLine()
		{
			var originalOut = Console.Out;
			var output = new StringWriter();

			try
			{
				Console.SetOut(output);

				var dom = DataModelCollector.ParsePit(xml);
				var engine = new Engine(new ConsoleWatcher());
				engine.startFuzzing(dom, new RunConfiguration());
			}
			finally
			{
				Console.SetOut(originalOut);
			}

			StringAssert.Contains(
				"[*] ***,TheDataModel.value,StringCaseUpper,Hello World!,HELLO WORLD!",
				output.ToString());
			StringAssert.Contains(
				"[*] ***,TheDataModel.value,DataElementRemove,Hello World!,<removed>",
				output.ToString());
		}
	}
}
