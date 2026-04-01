using System;
using System.Collections.Generic;
using System.Xml;
using Peach.Core;
using Peach.Core.Analyzers;
using Peach.Core.Cracker;
using Peach.Core.Dom;
using Peach.Core.IO;
using NLog;

namespace Peach.LLM.Core.Dom
{
	/// <summary>
	/// Optional data element - Conditional element wrapper
	/// Includes or excludes wrapped content based on expression evaluation
	/// ref element's value is available as 'value' in the expression
	/// </summary>
	[DataElement("Optional")]
	[PitParsable("Optional")]
	[Parameter("name", typeof(string), "Element name", "")]
	[Parameter("fieldId", typeof(string), "Element field ID", "")]
	[Parameter("ref", typeof(string), "Reference to element to use in expression", "")]
	[Parameter("expression", typeof(string), "Scripting expression for conditional inclusion (ref value available as 'value'; use '&gt;' for '>' and '&lt;' for '<')", "")]
	[Parameter("length", typeof(uint?), "Length in data element", "")]
	[Parameter("lengthType", typeof(LengthType), "Units of the length attribute", "bytes")]
	[Parameter("mutable", typeof(bool), "Is element mutable", "true")]
	[Parameter("minOccurs", typeof(int), "Minimum occurrences", "0")]
	[Parameter("maxOccurs", typeof(int), "Maximum occurrences", "1")]
	[Parameter("occurs", typeof(int), "Actual occurrences", "0")]
	[Serializable]
	public class Optional : Block
	{
		private static readonly NLog.Logger logger = LogManager.GetCurrentClassLogger();

		/// <summary>
		/// Reference to the element that should be optionally included
		/// </summary>
		public string Ref { get; set; }

		/// <summary>
		/// Expression that determines if the element should be included
		/// </summary>
		public string Expression { get; set; }

		/// <summary>
		/// Cached reference to the actual element
		/// </summary>
		private DataElement _refElement;

		public Optional()
			: base()
		{
		}

		public Optional(string name)
			: base(name)
		{
		}

		/// <summary>
		/// Get the referenced element
		/// </summary>
		public DataElement GetReferencedElement()
		{
			if (_refElement != null)
				return _refElement;

			if (string.IsNullOrWhiteSpace(Ref))
				return null;

			DataElement elem = null;

			var p = parent;
			while (p != null)
			{
				elem = p.find(Ref);

				if (elem != null)
					break;

				p = p.parent;
			}

			_refElement = elem;
			return elem;
		}

		/// <summary>
		/// Crack the optional element and its children based on expression condition
		/// Follows the same pattern as DataElementContainer.Crack()
		/// </summary>
		public override void Crack(DataCracker context, BitStream data, long? size)
		{
			BitStream sizedData = ReadSizedData(data, size);
			long startPosition = data.PositionBits;

			try
			{
				// Evaluate the conditional expression
				if (!EvaluateCondition())
				{
					logger.Trace("Optional '{0}': Condition `{1}` evaluated to false, skipping cracking", debugName, Expression);
					return;
				}

				logger.Trace("Optional '{0}': Condition `{1}` evaluated to true, proceeding with cracking", debugName, Expression);
				
				// Process children similar to DataElementContainer.Crack
				var prevCount = Count;

				// Handle children, iterate since cracking can modify the list
				for (var i = 0; i < Count; )
				{
					var child = this[i];
					context.CrackData(child, sizedData);

					// If we are unsized, cracking a child can cause our size
					// to be available. If so, update and keep going.
					if (!size.HasValue)
					{
						size = context.GetElementSize(this);

						if (size.HasValue)
						{
							long read = data.PositionBits - startPosition;
							sizedData = ReadSizedData(data, size, read);
						}
					}

					// if Count < prevCount, the child was placed in a different
					// spot in the dom, so don't increment our index
					if (Count == prevCount)
					{
						// if the child's index is previous to our current index
						// the child was placed behind the current position.
						// it will get cracked on a subsequent pass so just
						// increment the index to go to the next element.
						// if the child index is ahead of the index, this means
						// it was placed and not cracked so keep the index the same
						var idx = IndexOf(child);

						System.Diagnostics.Debug.Assert(idx >= 0);

						if (idx <= i)
							++i;
					}
					else if (Count > prevCount)
					{
						// Cracking child caused new elements to be added to
						// this container, so set our next index to be one
						// after the index of the child
						i = IndexOf(child) + 1;
					}

					prevCount = Count;
				}

				if (size.HasValue && sizedData == data)
					data.SeekBits(startPosition + size.Value, System.IO.SeekOrigin.Begin);
			}
			catch (CrackingFailure)
			{
				logger.Trace("Optional '{0}': Cracking failed", debugName);
				throw;
			}
			catch (Exception ex)
			{
				logger.Trace("Optional '{0}': Exception during cracking: {1}", debugName, ex.Message);
				throw new CrackingFailure(string.Format(
					"Optional '{0}' cracking failed: {1}", debugName, ex.Message), this, data, ex);
			}
		}

		/// <summary>
		/// Evaluate the conditional expression
		/// ref element's value is available as 'value' in the expression
		/// </summary>
		public bool EvaluateCondition()
		{
			if (string.IsNullOrWhiteSpace(Expression))
				return true; // No expression means always include

			try
			{
				var refElement = GetReferencedElement();
				if (refElement == null)
					throw new PeachException(string.Format(
						"Optional '{0}': Referenced element '{1}' not found", debugName, Ref));

				// Get the value from the referenced element
				var refValue = refElement.DefaultValue;

				// Create state dictionary similar to SizeRelation
				var state = new Dictionary<string, object>
				{
					{ "self", this }
				};

                switch (refValue.GetVariantType())
                {
                    case Variant.VariantType.Int:
                        state["value"] = (int)refValue;
                        break;
                    case Variant.VariantType.Long:
                        state["value"] = (long)refValue;
                        break;
                    case Variant.VariantType.ULong:
                        state["value"] = (ulong)refValue;
                        break;
                    case Variant.VariantType.String:
                        state["value"] = (string)refValue;
                        break;
                    case Variant.VariantType.Double:
                        state["value"] = (double)refValue;
                        break;
                    case Variant.VariantType.ByteString:
                        state["value"] = (byte[])refValue;
                        break;
                    case Variant.VariantType.BitStream:
                        state["value"] = (BitwiseStream)refValue;
                        break;
                    default:
                        logger.Warn("Optional '{0}': Unsupported ref value type {1}, expression may not evaluate correctly", debugName, refValue.GetVariantType());
                        state["value"] = refValue;
                        break;
                }

				// Evaluate the expression using DataElement's built-in scripting
				object result = EvalExpression(Expression, state);

                logger.Trace("Optional '{0}': Expression `{1}` evaluated to {2} with value = {3}", debugName, Expression, result, refValue);
				
				// Convert result to bool
				if (result is bool)
					return (bool)result;

				if (result is int)
					return (int)result != 0;

				if (result is long)
					return (long)result != 0;

				if (result is ulong)
					return (ulong)result != 0;

				if (result is string)
					return !string.IsNullOrEmpty((string)result);

				return result != null;
			}
			catch (Exception ex)
			{
				throw new PeachException(string.Format(
					"Error evaluating Optional '{0}' expression '{1}': {2}",
					debugName, Expression, ex.Message), ex);
			}
		}

		/// <summary>
		/// Parse the Optional element from PIT XML
		/// </summary>
		public static new DataElement PitParser(PitParser context, XmlNode node, DataElementContainer parent)
		{
			if (node.Name != "Optional")
				return null;

            Optional optional;

            if (node.hasAttr("ref"))
			{
				var name = node.getAttr("name", null);
				var refName = node.getAttrString("ref");
				var dom = ((DataModel)parent.root).dom;
				var refObj = dom.getRef(refName, parent);

				if (refObj == null)
					throw new PeachException("Error, Optional {0}could not resolve ref '{1}'. XML:\n{2}".Fmt(
						name == null ? "" : "'" + name + "' ", refName, node.OuterXml));

				if (!(refObj is DataElement))
					throw new PeachException("Error, Optional {0}resolved ref '{1}' to unsupported element {2}. XML:\n{3}".Fmt(
						name == null ? "" : "'" + name + "' ", refName, refObj.debugName, node.OuterXml));
				
				if (string.IsNullOrEmpty(name))
					name = new Optional().Name;

                optional = new Optional(name)
                {
                    parent = parent,
                    isReference = true,
                    referenceName = refName,
                    Ref = refName
                };
            }
			else
			{
				optional = Generate<Optional>(node, parent);
				optional.parent = parent;
			}

			// Parse 'expression' attribute
			var exprAttr = node.Attributes["expression"];
			if (exprAttr != null)
				optional.Expression = exprAttr.Value;

			context.handleCommonDataElementAttributes(node, optional);
			context.handleCommonDataElementChildren(node, optional);
			context.handleDataElementContainer(node, optional);

			return optional;
		}

		/// <summary>
		/// Write the Optional element to PIT XML
		/// </summary>
		public override void WritePit(XmlWriter pit)
		{
			pit.WriteStartElement("Optional");

			if (referenceName != null)
				pit.WriteAttributeString("ref", referenceName);

			if (!string.IsNullOrEmpty(Expression))
				pit.WriteAttributeString("expression", Expression);

			WritePitCommonAttributes(pit);
			WritePitCommonChildren(pit);

			foreach (var obj in this)
				obj.WritePit(pit);

			pit.WriteEndElement();
		}
	}
}
