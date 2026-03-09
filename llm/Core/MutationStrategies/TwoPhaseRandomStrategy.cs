using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NLog;
using Peach.Core;
using Peach.Core.Dom;
using Peach.Pro.Core;
using Peach.Pro.Core.Mutators.Utility;
using Peach.Pro.Core.Runtime;
using Peach.LLM.Core.Fixups;
using Peach.Pro.Core.MutationStrategies;
using Action = Peach.Core.Dom.Action;
using Logger = NLog.Logger;
using Random = Peach.Core.Random;

namespace Peach.LLM.Core.MutationStrategies
{
	/// <summary>
	/// Random mutation strategy with two-phase mutation support.
	/// Extends RandomStrategy with TwoPhaseMutation and MultipleMutationsPerElement capabilities.
	/// </summary>
	[MutationStrategy("TwoPhaseRandom")]
	[Alias("TwoPhaseRandomStrategy")]
	[Parameter("SwitchCount", typeof(int), "Number of iterations to perform per-mutator befor switching.", "200")]
	[Parameter("MaxFieldsToMutate", typeof(int), "Maximum fields to mutate at once.", "6")]
	[Parameter("StateMutation", typeof(bool), "Enable state mutations.", "false")]
	[Parameter("Weighting", typeof(int), "Controls mutation weight evaulation.", "10")]
	[Parameter("MultipleMutationsPerElement", typeof(int), "Maximum number of times to mutate a single data element. The actual count is randomly chosen between 1 and this value, with higher probability for lower values (especially 1).", "1")]
	[Parameter("TwoPhaseMutation", typeof(bool), "Enable two-phase mutation within a single iteration. When enabled, each element is first mutated with LLM mutators (random count between 1 and MultipleMutationsPerElement), then with non-LLM mutators (random count between 1 and MultipleMutationsPerElement). Each phase independently selects its mutation count.", "false")]
	[Parameter("MutationPhase", typeof(string), "Mutation phase: None, LLMOnly, NonLLMOnly. None uses all mutators, LLMOnly uses only LLM mutators, NonLLMOnly uses only non-LLM mutators.", "None")]
	[Parameter("PhaseSwitchIteration", typeof(int), "Iteration at which to switch from LLMOnly phase to NonLLMOnly phase. 0 means use SwitchCount as the switch point. Setting this parameter enables phased mutation across iterations (first LLM, then non-LLM).", "0")]
	public class TwoPhaseRandomStrategy : RandomStrategy
	{
		static Logger logger = LogManager.GetCurrentClassLogger();

		/// <summary>
		/// Static variable to track the current mutation phase for fixup logging.
		/// Historically this allowed LLM-specific fixups to identify which phase triggered the fixup.
		/// Prefer toggling fixup execution via LLMFixup.ShouldFixup instead of relying on this value.
		/// Using ThreadStatic to ensure thread safety.
		/// </summary>
		[ThreadStatic]
		private static string _currentPhaseName;

		/// <summary>
		/// Key used in RunContext.stateStore to mark that two-phase messages have been sent.
		/// This prevents Action.Run() from sending a third message.
		/// </summary>
		private const string TwoPhaseMessagesSentKey = "TwoPhaseRandomStrategy.MessagesSent";

		/// <summary>
		/// Maximum number of times to mutate a single data element before keeping the final result.
		/// The actual count is randomly chosen between 1 and this value, with higher probability for lower values (especially 1).
		/// If set to 1, each element is mutated only once (default behavior).
		/// </summary>
		public int MultipleMutationsPerElement { get; set; }

		/// <summary>
		/// Whether to use two-phase mutation within a single iteration.
		/// When enabled, each element is first mutated with LLM mutators (random count between 1 and MultipleMutationsPerElement),
		/// then with non-LLM mutators (random count between 1 and MultipleMutationsPerElement).
		/// Each phase independently selects its mutation count, weighted toward 1.
		/// </summary>
		public bool TwoPhaseMutation { get; set; }

		/// <summary>
		/// Mutation phase: None, LLMOnly, NonLLMOnly
		/// None: Use all mutators (default)
		/// LLMOnly: Only use LLM mutators
		/// NonLLMOnly: Only use non-LLM mutators
		/// </summary>
		public string MutationPhase
		{
			get;
			set;
		}

		/// <summary>
		/// Initialize the two-phase random strategy with support for MultipleMutationsPerElement and TwoPhaseMutation.
		/// </summary>
		public TwoPhaseRandomStrategy(Dictionary<string, Variant> args)
			: base(args)
		{
			MultipleMutationsPerElement = 1;
			if (args.ContainsKey("MultipleMutationsPerElement"))
				MultipleMutationsPerElement = int.Parse((string)args["MultipleMutationsPerElement"]);

			TwoPhaseMutation = false;
			if (args.ContainsKey("TwoPhaseMutation"))
			{
				TwoPhaseMutation = bool.Parse((string)args["TwoPhaseMutation"]);
				if (TwoPhaseMutation)
					logger.Info("TwoPhaseRandomStrategy: TwoPhaseMutation enabled (LLM mutators first, then non-LLM mutators)");
			}
		}

		/// <summary>
		/// Initialize the strategy and register event handlers.
		/// </summary>
		public override void Initialize(RunContext context, Engine engine)
		{
			base.Initialize(context, engine);

			// Register ActionFinished handler to clean up the two-phase messages sent flag
			if (TwoPhaseMutation)
			{
				context.ActionFinished += ActionFinished;
			}
		}

		/// <summary>
		/// Finalize the strategy and unregister event handlers.
		/// </summary>
		public override void Finalize(RunContext context, Engine engine)
		{
			// Unregister ActionFinished handler
			if (TwoPhaseMutation)
			{
				context.ActionFinished -= ActionFinished;
			}

			base.Finalize(context, engine);
		}

		/// <summary>
		/// Clean up the two-phase messages sent flag after action completes.
		/// </summary>
		private void ActionFinished(RunContext context, Action action)
		{
			// Clean up the flag to ensure it doesn't affect the next iteration
			// if (context.stateStore.ContainsKey(TwoPhaseMessagesSentKey))
			// {
			// 	context.stateStore.Remove(TwoPhaseMessagesSentKey);
			// }
		}

		/// <summary>
		/// Get a random mutation count between 1 and MultipleMutationsPerElement.
		/// Higher probability for lower values (especially 1).
		/// Uses a weighted random selection where weights decrease as count increases.
		/// </summary>
		protected int GetRandomMutationCountPerElement()
		{
			if (MultipleMutationsPerElement <= 1)
				return 1;

			// Create weights that favor lower numbers (especially 1)
			// Weight decreases exponentially: weight[i] = maxWeight / (i + 1)
			// This gives: 1 gets highest weight, 2 gets half, 3 gets third, etc.
			var weights = new int[MultipleMutationsPerElement];
			int maxWeight = 100; // Base weight for count=1

			for (int i = 0; i < MultipleMutationsPerElement; i++)
			{
				weights[i] = maxWeight / (i + 1);
			}

			// Calculate total weight
			int totalWeight = weights.Sum();

			// Random selection based on weights
			int randomValue = Random.Next(totalWeight);
			int cumulativeWeight = 0;

			for (int i = 0; i < MultipleMutationsPerElement; i++)
			{
				cumulativeWeight += weights[i];
				if (randomValue < cumulativeWeight)
				{
					return i + 1; // Return count (1-based)
				}
			}

			// Fallback (shouldn't reach here)
			return 1;
		}

		/// <summary>
		/// Check if a mutator type belongs to LLM mutators namespace
		/// </summary>
		protected bool IsLLMMutator(Type mutatorType)
		{
			return mutatorType != null && mutatorType.Namespace != null && mutatorType.Namespace.StartsWith("Peach.LLM.Core.Mutators");
		}

		/// <summary>
		/// Filter mutators based on current mutation phase (LLMOnly / NonLLMOnly).
		/// Overrides base behavior to implement phase-aware filtering for TwoPhaseRandomStrategy.
		/// </summary>
		protected override bool ShouldIncludeMutator(Type mutatorType)
		{
			if (MutationPhase == null || MutationPhase == "None")
				return true;

			bool isLLM = IsLLMMutator(mutatorType);

			if (MutationPhase == "LLMOnly")
				return isLLM;

			if (MutationPhase == "NonLLMOnly")
				return !isLLM;

			return true;
		}
		protected override void MutateDataModel(Action action)
		{
			// MutateDataModel should only be called after ParseDataModel
			Debug.Assert(Iteration > 0);

			logger.Info("MutateDataModel: Starting mutation for action {0} (TwoPhaseMutation={1}, MultipleMutationsPerElement={2})",
				action.Name, TwoPhaseMutation, MultipleMutationsPerElement);

			if (TwoPhaseMutation)
			{
				bool phase1HasMutations = false;
				bool phase2HasMutations = false;

				// Prepare Phase 1 scope and source
				var phase1Scope = new MutationScope("Phase1_LLM");
				IEnumerable<MutableItem> phase1Source = null;
				if (mutationScopeGlobal != null && mutationScopeGlobal.Count > 0)
				{
					phase1Source = mutationScopeGlobal;
					logger.Debug("MutateDataModel: Phase 1 (LLM) - Using mutationScopeGlobal ({0} elements)", mutationScopeGlobal.Count);
				}
				else if (mutations != null && mutations.Length > 0)
				{
					phase1Source = mutations;
					logger.Debug("MutateDataModel: Phase 1 (LLM) - mutationScopeGlobal is empty, using current mutations list ({0} elements)", mutations.Length);
				}
				else
				{
					phase1Source = Enumerable.Empty<MutableItem>();
					logger.Warn("MutateDataModel: Phase 1 (LLM) - No mutable elements available (mutationScopeGlobal and mutations are both empty)");
				}

				// 2. Optimize: collect LLM-capable mutable items without duplication
				Action<IEnumerable<MutableItem>> AddLLMMutableItems = items =>
				{
					if (items == null) return;
					foreach (var item in items)
					{
						var llmMutators = item.Mutators.Where(m => IsLLMMutator(m.GetType())).ToList();
						if (llmMutators.Count == 0) continue;
						if (!phase1Scope.Any(p => p.InstanceName == item.InstanceName && p.ElementName == item.ElementName))
							phase1Scope.Add(new MutableItem(item.InstanceName, item.ElementName, llmMutators));
					}
				};

				// Collect LLM-capable items from all sources (no short-circuiting): phase1Source → mutations → mutableItems
				AddLLMMutableItems(phase1Source);
				if (mutations != null && mutations.Length > 0)
					AddLLMMutableItems(mutations);
				if (mutableItems != null)
				{
					foreach (var scope in mutableItems)
						AddLLMMutableItems(scope);
				}

				// 3. Phase 1 核心逻辑（修复日志与空值问题）
				if (phase1Scope.Count == 0)
				{
					logger.Info("MutateDataModel: Phase 1 (LLM) - No elements found with LLM mutators. Skipping Phase 1.");
				}
				else
				{
					// 修复：日志打印真实的筛选来源数量
					logger.Info("MutateDataModel: Phase 1 (LLM) - Found {0} unique elements with LLM mutators.", phase1Scope.Count);

					// 选择需要变异的项
					var fieldsToMutate = GetMutationCount();
					fieldsToMutate = Math.Max(1, Math.Min(fieldsToMutate, phase1Scope.Count));
					var phase1Mutations = Random.WeightedSample(phase1Scope, fieldsToMutate);

					logger.Info("MutateDataModel: Phase 1 (LLM) - Selected {0} elements to mutate", phase1Mutations.Length);

					// 优化：先将phase1Mutations的InstanceName存入HashSet，提升查询效率
					HashSet<string> phase1InstanceNames = new HashSet<string>(phase1Mutations.Select(m => m.InstanceName));
					SetLLMFixupsShouldFixup(action, true); // Enable LLM fixups for LLM phase
					foreach (var item in action.outputData)
					{
						// O(1) 时间复杂度查询，提升性能
						if (phase1InstanceNames.Contains(item.instanceName))
						{
							if (ApplyPhaseMutation(item, action, true, phase1Mutations))
								phase1HasMutations = true;
						}
					}
				}

				// 4. 发送消息（修复dataModel空值风险）
				if (phase1HasMutations)
				{
					var firstData = action.outputData.FirstOrDefault();
					if (firstData != null && firstData.dataModel != null) // 双重空值判断
					{
						_currentPhaseName = "Phase 1 (LLM)";
						try
						{
							firstData.dataModel.Invalidate();
							var _ = firstData.dataModel.Value;
							SendOutputMessage(action, firstData, "Phase 1 (LLM)");
						}
						catch (Exception ex)
						{
							// 异常捕获：避免单个数据模型异常导致整体流程中断
							logger.Error(ex, "MutateDataModel: Phase 1 (LLM) - Failed to process data model for action {0}", action.Name);
						}
						finally
						{
							_currentPhaseName = null; // finally确保无论是否异常，都清空阶段名称
						}
					}
				}
				try
				{
					SetLLMFixupsShouldFixup(action, false);

					// Prepare Phase 2
					foreach (var item in action.outputData)
					{
						if (ApplyPhaseMutation(item, action, false))
							phase2HasMutations = true;
					}


					// If Phase 2 made changes, send Phase 2 message (similar to Phase 1)
					if (phase2HasMutations)
					{
						// var firstData = action.outputData.FirstOrDefault();
						// if (firstData != null && firstData.dataModel != null)
						// {
						// 	_currentPhaseName = "Phase 2 (Non-LLM)";
						// 	try
						// 	{
						// 		firstData.dataModel.Invalidate();
						// 		var _ = firstData.dataModel.Value;
						// 		SendOutputMessage(action, firstData, "Phase 2 (Non-LLM)");
						// 	}
						// 	catch (Exception ex)
						// 	{
						// 		logger.Error(ex, "MutateDataModel: Phase 2 (Non-LLM) - Failed to process data model for action {0}", action.Name);
						// 	}
						// 	finally
						// 	{
						// 		_currentPhaseName = null;
						// 	}
						// }
					}
				}
				finally
				{
					// Always restore LLM fixups to true so later iterations are correct
					// SetLLMFixupsShouldFixup(action, true);
				}
			}
			else
			{
				// Original behavior: Apply mutations to each element independently
				// Use the base class implementation but with MultipleMutationsPerElement support
				base.MutateDataModel(action);
			}
		}
		/// <summary>
		/// Override ApplyMutation to support MultipleMutationsPerElement.
		/// </summary>
		protected void ApplyMutation(ActionData data, Action action)
		{
			var instanceName = data.instanceName;

			foreach (var item in mutations)
			{
				if (item.InstanceName != instanceName)
					continue;

				var elem = data.dataModel.find(item.ElementName);
				if (elem != null && elem.mutationFlags == MutateOverride.None)
				{
					// Apply multiple mutations to the same element
					// Only the final mutation result will be kept
					// Get random mutation count (1 to MultipleMutationsPerElement, weighted toward 1)
					int mutationCount = GetRandomMutationCountPerElement();

					// Filter mutators based on phase if needed
					WeightedList<Mutator> availableMutators;
					if (MutationPhase != null && MutationPhase != "None")
					{
						// Create a new WeightedList with filtered mutators
						var filteredMutators = item.Mutators.Where(m => ShouldIncludeMutator(m.GetType())).ToList();
						if (filteredMutators.Count == 0)
						{
							logger.Debug("Action_Starting: No available mutators for phase {0} on element {1}",
								MutationPhase, item.ElementName);
							break;
						}
						availableMutators = new WeightedList<Mutator>(filteredMutators);
					}
					else
					{
						availableMutators = item.Mutators;
					}

					if (availableMutators.Count == 0)
					{
						logger.Debug("Action_Starting: No available mutators for phase {0} on element {1}",
							MutationPhase, item.ElementName);
						break;
					}

					for (int i = 0; i < mutationCount; i++)
					{
						// Check if element still exists (might have been removed by previous mutator)
						var currentElem = data.dataModel.find(item.ElementName);
						if (currentElem == null)
						{
							logger.Debug("Action_Starting: Element {0} was removed after mutation {1}/{2}, stopping",
								item.ElementName, i, mutationCount);
							break;
						}

						// Randomly select a mutator from the full list (may select the same mutator multiple times)
						var mutator = Random.WeightedChoice(availableMutators);

						// Output phase information for non-two-phase mode (if MutationPhase is set)
						string phaseInfo = "";
						if (MutationPhase != null && MutationPhase != "None")
						{
							phaseInfo = $" [{MutationPhase}]";
							ConsoleWatcher.WriteInfoMark();
							Console.WriteLine("Fuzzing: {0}{1}", currentElem.fullName, phaseInfo);
							ConsoleWatcher.WriteInfoMark();
							Console.WriteLine("Mutator: {0}{1}", mutator.Name, phaseInfo);
						}

						Context.OnDataMutating(data, currentElem, mutator);
						logger.Debug("Action_Starting: Fuzzing: {0} (mutation {1}/{2})",
							item.ElementName, i + 1, mutationCount);
						logger.Debug("Action_Starting: Mutator: {0}", mutator.Name);

						// Apply the mutation
						try
						{
							mutator.randomMutation(currentElem);
							RecordMutation(instanceName, item.ElementName, mutator.Name);
						}
						catch (Exception ex)
						{
							// If mutation fails (e.g., type mismatch after previous mutation),
							// log warning and continue with next mutation attempt
							logger.Warn("Action_Starting: Mutation failed for element {0} with mutator {1} at mutation {2}/{3}: {4}",
								item.ElementName, mutator.Name, i + 1, mutationCount, ex.Message);
							logger.Trace(ex);
							// Break out of loop since this element may no longer be compatible with mutators
							break;
						}

						// For the last mutation, we keep the result
						// For intermediate mutations, we continue mutating the already-mutated element
					}
				}
				else
				{
					logger.Debug("Action_Starting: Skipping Fuzzing: {0}", item.ElementName);
				}
			}
		}

		/// <summary>
		/// Apply mutations for a single phase (LLM or non-LLM) to a single element.
		/// Returns true if any mutations were applied.
		/// </summary>
		/// <param name="data">The action data containing the data model</param>
		/// <param name="action">The action being executed</param>
		/// <param name="isLlmPhase">True for LLM phase, false for non-LLM phase</param>
		/// <param name="filteredMutations">Optional filtered mutations list. If null, uses the default mutations list.</param>
		private bool ApplyPhaseMutation(ActionData data, Action action, bool isLlmPhase, MutableItem[] filteredMutations = null)
		{
			var instanceName = data.instanceName;
			bool hasMutations = false;
			int skippedCount = 0;  // 记录跳过的元素数量
								   // SetLLMFixupsShouldFixup(action, isLlmPhase); // 设置LLM fixup的ShouldFixup属性
								   // Use filtered mutations if provided, otherwise use the default mutations list
			var mutationsToUse = filteredMutations ?? mutations;

			// 首先尝试从已选择的 mutations 列表中选择元素
			foreach (var item in mutationsToUse)
			{
				if (item.InstanceName != instanceName)
					continue;

				var elem = data.dataModel.find(item.ElementName);
				if (elem == null)
					continue;

				// Get random mutation count for this element (1 to MultipleMutationsPerElement, weighted toward 1)
				int mutationCount = GetRandomMutationCountPerElement();

				// Filter mutators based on phase
				var phaseMutators = isLlmPhase
					? item.Mutators.Where(m => IsLLMMutator(m.GetType())).ToList()
					: item.Mutators.Where(m => !IsLLMMutator(m.GetType())).ToList();

				if (phaseMutators.Count == 0)
				{
					skippedCount++;
					logger.Debug("Action_Starting: Phase {0} - No {1} mutators available for element {2}",
						isLlmPhase ? "1 (LLM)" : "2 (Non-LLM)",
						isLlmPhase ? "LLM" : "non-LLM",
						item.ElementName);
					continue;
				}

				var mutatorList = new WeightedList<Mutator>(phaseMutators);
				string phaseName = isLlmPhase ? "Phase 1 (LLM)" : "Phase 2 (Non-LLM)";
				logger.Info("Action_Starting: {0} - Fuzzing element {1}, will apply {2} mutations",
					phaseName, item.ElementName, mutationCount);

				for (int i = 0; i < mutationCount; i++)
				{
					// Check if element still exists (might have been removed by previous mutator)
					var currentElem = data.dataModel.find(item.ElementName);
					if (currentElem == null)
					{
						logger.Info("Action_Starting: {0} - Element {1} was removed after mutation {2}/{3}, stopping",
							phaseName, item.ElementName, i, mutationCount);
						break;
					}

					// Randomly select a mutator from the full list (may select the same mutator multiple times)
					var mutator = Random.WeightedChoice(mutatorList);

					// OnDataMutating will output phase information automatically via ConsoleWatcher
					Context.OnDataMutating(data, currentElem, mutator);
					logger.Info("Action_Starting: {0} - Fuzzing: {1} (mutation {2}/{3})",
						phaseName, item.ElementName, i + 1, mutationCount);
					logger.Info("Action_Starting: {0} - Mutator: {1}", phaseName, mutator.Name);

					try
					{
						mutator.randomMutation(currentElem);
						RecordMutation(instanceName, item.ElementName, mutator.Name);
						hasMutations = true;
					}
					catch (Exception ex)
					{
						// If mutation fails (e.g., type mismatch after previous mutation),
						// log warning and break out of loop
						logger.Warn("Action_Starting: {0} - Mutation failed for element {1} with mutator {2} at mutation {3}/{4}: {5}",
							phaseName, item.ElementName, mutator.Name, i + 1, mutationCount, ex.Message);
						logger.Trace(ex);
						// Break out of loop since this element may no longer be compatible with mutators
						break;
					}
				}
			}

			return hasMutations;
		}

		/// <summary>
		/// Get the current phase name for fixup logging.
		/// Historically this allowed LLM fixups to identify which phase triggered the fixup; prefer using LLMFixup.ShouldFixup.
		/// </summary>
		/// <summary>
		/// Enable or disable LLM fixups (sets LLMFixup.ShouldFixup) for all output datamodel elements
		/// </summary>
		private void SetLLMFixupsShouldFixup(Action action, bool value)
		{
			foreach (var item in action.outputData)
			{
				if (item?.dataModel == null)
					continue;

				foreach (var elem in item.dataModel.PreOrderTraverse())
				{
					var fix = elem.fixup as LLMFixup;
					if (fix != null)
						fix.ShouldFixup = value;
				}
			}
		}

		public static string GetCurrentPhaseName()
		{
			return _currentPhaseName;
		}

		/// <summary>
		/// Send output message using publisher for the given action and data.
		/// This method opens the publisher and sends data before OnRun() is called.
		/// </summary>
		private void SendOutputMessage(Action action, ActionData data, string phaseName)
		{
			Publisher publisher = null;
			try
			{
				// Ensure phase name is set for fixup (in case it was cleared)
				_currentPhaseName = phaseName;

				// Get publisher (same logic as Action.Run())
				if (action.publisher != null && action.publisher != "Peach.Agent")
				{
					if (!Context.test.publishers.ContainsKey(action.publisher))
					{
						logger.Warn("SendOutputMessage: Publisher '{0}' not found for {1}, skipping output",
							action.publisher, phaseName);
						return;
					}
					publisher = Context.test.publishers[action.publisher];
				}
				else
				{
					publisher = Context.test.publishers[0];
				}

				// Ensure publisher is started (this is safe to call multiple times)
				publisher.start();

				// Open publisher - this will initialize the connection if needed
				// Note: open() checks _isOpen internally, so it's safe to call even if already open
				publisher.open();

				// Send the mutated data
				logger.Info("SendOutputMessage: Sending output message after {0} mutations", phaseName);
				publisher.output(data.dataModel);
				logger.Info("SendOutputMessage: Successfully sent output message after {0} mutations", phaseName);

				// Mark that we've sent messages via two-phase strategy
				// Only set the flag after Phase 2 completes, to prevent Action.Run() from sending a third message
				// This ensures both Phase 1 and Phase 2 messages have been sent before blocking Action.Run()
				// if (phaseName.Contains("Phase 2") || phaseName.Contains("Non-LLM"))
				// {
				// 	Context.stateStore[TwoPhaseMessagesSentKey] = true;
				// 	logger.Debug("SendOutputMessage: Set flag to prevent Action.Run() from sending third message");
				// }
				publisher.close();
			}
			catch (Exception ex)
			{
				// Check if the error is due to client not being initialized (connection closed by async operation)
				// This can happen if async read operation detected connection close between Phase 1 and Phase 2
				if (ex.Message != null && ex.Message.Contains("client is not initalized") && publisher != null)
				{
					logger.Info("SendOutputMessage: Connection was closed, attempting to reconnect and retry for {0}", phaseName);
					try
					{
						// Close publisher to reset _isOpen flag
						// This allows open() to actually reconnect instead of returning early
						publisher.close();

						// Reopen the connection
						publisher.open();

						// Retry sending
						logger.Info("SendOutputMessage: Retrying send after reconnection for {0}", phaseName);
						publisher.output(data.dataModel);
						logger.Info("SendOutputMessage: Successfully sent output message after {0} mutations (retry)", phaseName);

						// Mark that we've sent messages via two-phase strategy (after retry)
						// Only set the flag after Phase 2 completes
						if (phaseName.Contains("Phase 2") || phaseName.Contains("Non-LLM"))
						{
							Context.stateStore[TwoPhaseMessagesSentKey] = true;
							logger.Debug("SendOutputMessage: Set flag to prevent Action.Run() from sending third message (after retry)");
						}
						return;
					}
					catch (Exception retryEx)
					{
						// If retry also fails, log and continue (final data will still be sent by normal Action output)
						logger.Warn("SendOutputMessage: Retry also failed for {0}: {1}. The final data will still be sent by the normal Action output.",
							phaseName, retryEx.Message);
						logger.Trace(retryEx);
					}
				}
				else
				{
					// For other errors, just log and continue
					// The normal Action output will still send the final mutated data
					logger.Warn("SendOutputMessage: Failed to send output message after {0}: {1}. The final data will still be sent by the normal Action output.",
						phaseName, ex.Message);
					logger.Trace(ex);
				}
			}
		}
	}
}
