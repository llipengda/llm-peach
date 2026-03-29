# LLM-Peach SDK

## Extension Methods

- `Bytes(this DataElement obj)`: Converts a data element value to a raw byte array, with special handling for numeric and string variants.
- `IsIn(this DataElement obj, string name)`: Checks whether an element belongs to a parent/reference chain matching the given name pattern.
- `Dump(this byte[] bytes, TextWriter writer = null)`: Prints bytes in a readable hex + ASCII hexdump format.
- `DumpDiff(this byte[] left, byte[] right, TextWriter writer = null)`: Prints two hexdumps and highlights byte-level differences.
- `SetUniqChild(this Array arr, Variant v)`: Ensures a single child exists in an array and sets its value.
- `SetValue(this DataElement elem, Variant v)`: Sets a data element mutated value, auto-converting string variants to ASCII bytes for non-string elements.
- `GetUint8(this Number num)`: Reads a number as an unsigned 8-bit value, falling back to raw bytes when needed.
- `GetUint16(this Number num)`: Reads a number as an unsigned 16-bit value, falling back to raw bytes when needed.
- `GetVarInt(this MqttVarInt varInt)`: Decodes an MQTT variable-byte integer from either numeric or encoded byte form.
- `ToMqttString(this string data)`: Encodes a string as an MQTT UTF-8 string (2-byte length prefix + UTF-8 payload).

## LLMMutator

`LLMMutator` is an abstract mutator base class for LLM-driven mutations.
You should derive from this class and implement the `PerformMutation` method to define your mutation logic,
and `supportedDataElement` to specify which data element types your mutator can handle.

- `PickWeighted(int[] weights)`: Selects an index using weighted random choice.
- `Next(int max)`: Returns a random integer in `[0, max)` from Peach context RNG.
- `NextBytes(byte[] buffer)`: Fills a buffer with random bytes.
- `PerformMutation(DataElement obj)`: Abstract hook implemented by derived mutators.
- `supportedDataElement(DataElement obj)`: Returns true if this mutator can handle the given data element type.

## LLMFixup

`LLMFixup` is an abstract fixup base class for LLM-driven fixups.
You should derive from this class and implement the `fixupImpl` method to define your fixup logic.

- `Variant fixupImpl()`: Implemented by derived fixups to compute the fixup value.

