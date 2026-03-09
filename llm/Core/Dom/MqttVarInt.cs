//
// Copyright (c) Peach Fuzzer, LLC
//
// MQTT VarInt - Custom data element type for MQTT Variable Byte Integer
// MQTT VarInt encoding: 1-4 bytes, where bit 7 of each byte indicates continuation
// Each byte encodes 7 bits, bit 7=1 means more bytes follow
// Maximum value: 268,435,455 (0x0FFFFFFF)

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Peach.Core;
using Peach.Core.Analyzers;
using Peach.Core.Cracker;
using Peach.Core.Dom;
using Peach.Core.IO;

namespace Peach.LLM.Core.Dom
{
	/// <summary>
	/// MQTT VarInt data element - Variable Byte Integer for MQTT protocol
	/// Encodes/decodes values using MQTT VarInt format (1-4 bytes)
	/// Supports Relation for dynamic length fields
	/// </summary>
	[DataElement("VarInt", DataElementTypes.NonDataElements)]
	[PitParsable("VarInt")]
	[Parameter("name", typeof(string), "Element name", "")]
	[Parameter("fieldId", typeof(string), "Element field ID", "")]
	[Parameter("value", typeof(string), "Default value", "")]
	[Parameter("valueType", typeof(Peach.Core.Dom.ValueType), "Format of value attribute", "string")]
	[Parameter("token", typeof(bool), "Is element a token", "false")]
	[Parameter("mutable", typeof(bool), "Is element mutable", "true")]
	[Parameter("constraint", typeof(string), "Scripting expression that evaluates to true or false", "")]
	[Parameter("minOccurs", typeof(int), "Minimum occurances", "1")]
	[Parameter("maxOccurs", typeof(int), "Maximum occurances", "1")]
	[Parameter("occurs", typeof(int), "Actual occurances", "1")]
	[Serializable]
	public class MqttVarInt : Number
	{
		// MQTT VarInt maximum value: 268,435,455 (0x0FFFFFFF)
		private const ulong MAX_VARINT_VALUE = 0x0FFFFFFFUL;

		public MqttVarInt()
			: base()
		{
			lengthType = LengthType.Bits;
			// VarInt is variable length (1-4 bytes = 8-32 bits)
			// We use 32 bits as the internal representation
			length = 32;
			Signed = false;
			LittleEndian = false; // MQTT uses big endian
			DefaultValue = new Variant(0UL);
		}

		public MqttVarInt(string name)
			: base(name)
		{
			lengthType = LengthType.Bits;
			length = 32;
			Signed = false;
			LittleEndian = false;
			DefaultValue = new Variant(0UL);
		}

		/// <summary>
		/// VarInt has variable length (1-4 bytes), so hasLength returns false
		/// </summary>
		public override bool hasLength { get { return false; } }

		/// <summary>
		/// VarInt is deterministic (can be calculated from value)
		/// </summary>
		public override bool isDeterministic { get { return true; } }

		/// <summary>
		/// Override length property to prevent setting invalid values
		/// VarInt always uses 32 bits internally for value representation
		/// </summary>
		public override long length
		{
			get
			{
				// Return the actual encoded length in bits
				return GetEncodedLengthBits();
			}
			set
			{
				// VarInt length is determined by the value, not set directly
				// We keep internal representation as 32 bits
				base.length = 32;
			}
		}

		/// <summary>
		/// Get the actual encoded length in bits based on current value
		/// </summary>
		private long GetEncodedLengthBits()
		{
			var value = GetValue();
			if (value == 0)
				return 8; // 1 byte

			// Calculate number of bytes needed
			int bytes = 1;
			ulong val = value;
			while (val > 127 && bytes < 4)
			{
				val >>= 7;
				bytes++;
			}

			return bytes * 8;
		}

		/// <summary>
		/// Get the current value as ulong
		/// </summary>
		private ulong GetValue()
		{
			if (InternalValue == null)
				return 0;

			var variant = InternalValue;
			if (variant.GetVariantType() == Variant.VariantType.ULong)
				return (ulong)variant;
			if (variant.GetVariantType() == Variant.VariantType.Long)
				return (ulong)(long)variant;
			if (variant.GetVariantType() == Variant.VariantType.Int)
				return (ulong)(int)variant;

			return 0;
		}

		/// <summary>
		/// Encode value to MQTT VarInt format (1-4 bytes)
		/// </summary>
		protected override BitwiseStream InternalValueToBitStream()
		{
			var value = GetValue();

			// Validate value range
			if (value > MAX_VARINT_VALUE)
				value = MAX_VARINT_VALUE;

			var ret = new BitStream();

			// MQTT VarInt encoding: each byte encodes 7 bits, bit 7 indicates continuation
			// Encode from least significant to most significant
			var bytes = new System.Collections.Generic.List<byte>();
			var x = value;

			do
			{
				byte encodedByte = (byte)(x & 0x7F);
				x >>= 7;
				if (x > 0)
					encodedByte |= 0x80; // Set continuation bit

				bytes.Add(encodedByte);
			} while (x > 0 && bytes.Count < 4);

			if (x > 0)
				throw new PeachException(string.Format("Error, {0} value '{1}' requires more than 4 bytes for VarInt encoding.", debugName, value));

			// Write bytes in order (first byte is least significant)
			foreach (var b in bytes)
			{
				ret.WriteByte(b);
			}

			ret.Seek(0, SeekOrigin.Begin);
			return ret;
		}

		/// <summary>
		/// Decode MQTT VarInt format from bitstream
		/// </summary>
		protected override Variant Sanitize(Variant variant)
		{
			ulong value = 0;

			if (variant.GetVariantType() == Variant.VariantType.String)
			{
				// Parse string value
				string str = (string)variant;
				if (str.StartsWith("0x", StringComparison.InvariantCultureIgnoreCase))
				{
					value = Convert.ToUInt64(str.Substring(2), 16);
				}
				else
				{
					value = Convert.ToUInt64(str);
				}
			}
			else if (variant.GetVariantType() == Variant.VariantType.BitStream ||
					 variant.GetVariantType() == Variant.VariantType.ByteString)
			{
				// Decode from VarInt format
				BitwiseStream bs;
				if (variant.GetVariantType() == Variant.VariantType.ByteString)
					bs = new BitStream((byte[])variant);
				else
					bs = (BitwiseStream)variant;

				var pos = bs.PositionBits;
				try
				{
					value = DecodeVarInt(bs);
				}
				finally
				{
					bs.PositionBits = pos;
				}
			}
			else if (variant.GetVariantType() == Variant.VariantType.ULong)
			{
				value = (ulong)variant;
			}
			else if (variant.GetVariantType() == Variant.VariantType.Long)
			{
				var longVal = (long)variant;
				if (longVal < 0)
					throw new PeachException(string.Format("Error, {0} value '{1}' is negative. VarInt must be unsigned.", debugName, longVal));
				value = (ulong)longVal;
			}
			else if (variant.GetVariantType() == Variant.VariantType.Int)
			{
				var intVal = (int)variant;
				if (intVal < 0)
					throw new PeachException(string.Format("Error, {0} value '{1}' is negative. VarInt must be unsigned.", debugName, intVal));
				value = (ulong)intVal;
			}
			else
			{
				throw new PeachException(string.Format("Error, {0} unsupported variant type '{1}'.", debugName, variant.GetVariantType()));
			}

			// Validate value range
			if (value > MAX_VARINT_VALUE)
				value = MAX_VARINT_VALUE;

			return new Variant(value);
		}

		/// <summary>
		/// Decode MQTT VarInt from bitstream
		/// </summary>
		private ulong DecodeVarInt(BitwiseStream bs)
		{
			ulong value = 0;
			int multiplier = 1;
			int byteCount = 0;

			while (byteCount < 4)
			{
				if (bs.PositionBits >= bs.LengthBits)
					throw new PeachException(string.Format("Error, {0} incomplete VarInt encoding (reached end of stream).", debugName));

				byte b = (byte)bs.ReadByte();
				byteCount++;

				// Extract 7 bits of data
				value += (ulong)(b & 0x7F) * (ulong)multiplier;
				multiplier *= 128;

				// Check continuation bit
				if ((b & 0x80) == 0)
				{
					// Last byte
					return value;
				}
			}

			// More than 4 bytes - invalid
			throw new PeachException(string.Format("Error, {0} VarInt encoding exceeds 4 bytes (malformed).", debugName));
		}

		/// <summary>
		/// Override Crack to handle variable-length VarInt parsing
		/// </summary>
		public override void Crack(DataCracker context, BitStream data, long? size)
		{
			// VarInt is variable length (1-4 bytes)
			// We need to read bytes until we find one with continuation bit = 0
			var startPos = data.PositionBits;
			var bytes = new System.Collections.Generic.List<byte>();
			int byteCount = 0;

			while (byteCount < 4)
			{
				if (data.PositionBits >= data.LengthBits)
					throw new CrackingFailure(
						string.Format("Error, {0} incomplete VarInt encoding (reached end of stream).", debugName),
						this, data);

				byte b = (byte)data.ReadByte();
				bytes.Add(b);
				byteCount++;

				// Check continuation bit (bit 7)
				if ((b & 0x80) == 0)
				{
					// Last byte found
					break;
				}
			}

			if (byteCount >= 4 && (bytes[bytes.Count - 1] & 0x80) != 0)
			{
				throw new CrackingFailure(
					string.Format("Error, {0} VarInt encoding exceeds 4 bytes (malformed).", debugName),
					this, data);
			}

			// Create a BitStream with the VarInt bytes
			var varIntStream = new BitStream(bytes.ToArray());
			varIntStream.Seek(0, SeekOrigin.Begin);

			// Decode the VarInt value
			ulong value = DecodeVarInt(varIntStream);

			// Validate value range
			if (value > MAX_VARINT_VALUE)
				throw new CrackingFailure(
					string.Format("Error, {0} value '{1}' exceeds maximum VarInt value {2}.", debugName, value, MAX_VARINT_VALUE),
					this, data);

			// Set the DefaultValue
			DefaultValue = new Variant(value);

			if (context.IsLogEnabled)
			{
				context.Log("Value: {0} (0x{1:X}), Length: {2} bytes", value, value, byteCount);
			}
		}

		/// <summary>
		/// Parse VarInt from XML
		/// </summary>
		public new static DataElement PitParser(PitParser context, XmlNode node, DataElementContainer parent)
		{
			if (node.Name != "VarInt")
				return null;

			var varInt = DataElement.Generate<MqttVarInt>(node, parent);

			context.handleCommonDataElementAttributes(node, varInt);
			context.handleCommonDataElementChildren(node, varInt);
			context.handleCommonDataElementValue(node, varInt);

			return varInt;
		}

		/// <summary>
		/// Write VarInt to XML
		/// </summary>
		public override void WritePit(XmlWriter pit)
		{
			pit.WriteStartElement("VarInt");
			WritePitCommonAttributes(pit);
			WritePitCommonChildren(pit);
			WritePitCommonValue(pit);
			pit.WriteEndElement();
		}
	}
}


