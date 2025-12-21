
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NLog;
using Peach.Core;
using Peach.Core.Dom;
using Peach.Pro.Core.Mutators.Utility;
using Action = Peach.Core.Dom.Action;
using Logger = NLog.Logger;
using Random = Peach.Core.Random;

namespace Peach.Pro.Core.MutationStrategies
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
	[Parameter("TwoPhaseMutation", typeof(bool), "Enable two-phase mutation within a single iteration. When enabled, each element is first mutated with MQTT mutators (random count between 1 and MultipleMutationsPerElement), then with non-MQTT mutators (random count between 1 and MultipleMutationsPerElement). Each phase independently selects its mutation count.", "false")]
	[Parameter("MutationPhase", typeof(string), "Mutation phase: None, MQTTOnly, NonMQTTOnly. None uses all mutators, MQTTOnly uses only MQTT mutators, NonMQTTOnly uses only non-MQTT mutators.", "None")]
	[Parameter("PhaseSwitchIteration", typeof(int), "Iteration at which to switch from MQTTOnly phase to NonMQTTOnly phase. 0 means use SwitchCount as the switch point. Setting this parameter enables phased mutation across iterations (first MQTT, then non-MQTT).", "0")]
	public class TwoPhaseRandomStrategy : RandomStrategy
	{
		static Logger logger = LogManager.GetCurrentClassLogger();

		/// <summary>
		/// Static variable to track the current mutation phase for fixup logging.
		/// This allows MqttFixup to identify which phase triggered the fixup.
		/// Using ThreadStatic to ensure thread safety.
		/// </summary>
		[ThreadStatic]
		private static string _currentPhaseName;

		/// <summary>
		/// Maximum number of times to mutate a single data element before keeping the final result.
		/// The actual count is randomly chosen between 1 and this value, with higher probability for lower values (especially 1).
		/// If set to 1, each element is mutated only once (default behavior).
		/// </summary>
		public int MultipleMutationsPerElement { get; set; }

		/// <summary>
		/// Whether to use two-phase mutation within a single iteration.
		/// When enabled, each element is first mutated with MQTT mutators (random count between 1 and MultipleMutationsPerElement),
		/// then with non-MQTT mutators (random count between 1 and MultipleMutationsPerElement).
		/// Each phase independently selects its mutation count, weighted toward 1.
		/// </summary>
		public bool TwoPhaseMutation { get; set; }

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
					logger.Info("TwoPhaseRandomStrategy: TwoPhaseMutation enabled (MQTT mutators first, then non-MQTT mutators)");
			}
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
		/// Override MutateDataModel to support two-phase mutation.
		/// </summary>
		protected override void MutateDataModel(Action action)
		{
			// MutateDataModel should only be called after ParseDataModel
			Debug.Assert(Iteration > 0);

			logger.Info("MutateDataModel: Starting mutation for action {0} (TwoPhaseMutation={1}, MultipleMutationsPerElement={2})", 
				action.Name, TwoPhaseMutation, MultipleMutationsPerElement);

			if (TwoPhaseMutation)
			{
				// Two-phase mutation: first apply MQTT mutators to all elements, then non-MQTT mutators
				// Send message after each phase completes for all elements
				
				// Phase 1: Apply MQTT mutators to all elements
				logger.Info("MutateDataModel: Phase 1 (MQTT) - Starting mutation for all elements");
				bool phase1HasMutations = false;
				foreach (var item in action.outputData)
				{
					if (ApplyPhaseMutation(item, action, true)) // true = MQTT phase
						phase1HasMutations = true;
				}
				
			// Send message after Phase 1 completes (if there were any mutations)
			if (phase1HasMutations)
			{
				// Use the first outputData item to get the data model for sending
				var firstData = action.outputData.FirstOrDefault();
				if (firstData != null)
				{
					// Set phase name for fixup logging
					_currentPhaseName = "Phase 1 (MQTT)";
					
					// Force reapplication of relations and fixups after Phase 1 mutations
					// This ensures that fixups and relations are applied to the mutated data
					firstData.dataModel.Invalidate();
					
					// Accessing Value will trigger GenerateValue() which applies relations and fixups
					// This happens automatically when publisher.output() is called, but we can do it explicitly here
					var _ = firstData.dataModel.Value;
					
					SendOutputMessage(action, firstData, "Phase 1 (MQTT)");
					
					// Clear phase name after sending
					_currentPhaseName = null;
				}
			}

				// Phase 2: Apply non-MQTT mutators to all elements (mutating already-mutated elements)
				logger.Info("MutateDataModel: Phase 2 (Non-MQTT) - Starting mutation for all elements");
				bool phase2HasMutations = false;
				foreach (var item in action.outputData)
				{
					if (ApplyPhaseMutation(item, action, false)) // false = non-MQTT phase
						phase2HasMutations = true;
				}
				
			// Send message after Phase 2 completes (if there were any mutations)
			if (phase2HasMutations)
			{
				// Use the first outputData item to get the data model for sending
				var firstData = action.outputData.FirstOrDefault();
				if (firstData != null)
				{
					// Set phase name for fixup logging
					_currentPhaseName = "Phase 2 (Non-MQTT)";
					
					// Force reapplication of relations and fixups after Phase 2 mutations
					// This ensures that fixups and relations are applied to the final mutated data
					firstData.dataModel.Invalidate();
					
					// Accessing Value will trigger GenerateValue() which applies relations and fixups
					var _ = firstData.dataModel.Value;
					
					SendOutputMessage(action, firstData, "Phase 2 (Non-MQTT)");
					
					// Clear phase name after sending
					_currentPhaseName = null;
				}
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
		protected override void ApplyMutation(ActionData data, Action action)
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
		/// Apply mutations for a single phase (MQTT or non-MQTT) to a single element.
		/// Returns true if any mutations were applied.
		/// </summary>
		private bool ApplyPhaseMutation(ActionData data, Action action, bool isMqttPhase)
		{
			var instanceName = data.instanceName;
			bool hasMutations = false;

			foreach (var item in mutations)
			{
				if (item.InstanceName != instanceName)
					continue;

				var elem = data.dataModel.find(item.ElementName);
				if (elem == null)
					continue;

				// Get random mutation count for this element (1 to MultipleMutationsPerElement, weighted toward 1)
				int mutationCount = GetRandomMutationCountPerElement();
				
				// Filter mutators based on phase
				var phaseMutators = isMqttPhase 
					? item.Mutators.Where(m => IsMQTTMutator(m.GetType())).ToList()
					: item.Mutators.Where(m => !IsMQTTMutator(m.GetType())).ToList();
				
				if (phaseMutators.Count == 0)
				{
					logger.Info("Action_Starting: Phase {0} - No {1} mutators available for element {2}", 
						isMqttPhase ? "1 (MQTT)" : "2 (Non-MQTT)", 
						isMqttPhase ? "MQTT" : "non-MQTT",
						item.ElementName);
					continue;
				}

				var mutatorList = new WeightedList<Mutator>(phaseMutators);
				string phaseName = isMqttPhase ? "Phase 1 (MQTT)" : "Phase 2 (Non-MQTT)";
				
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
		/// This allows MqttFixup to identify which phase triggered the fixup.
		/// </summary>
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
