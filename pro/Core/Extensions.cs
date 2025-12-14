using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Peach.Core;
using Peach.Core.Dom;
using Peach.Core.IO;
using Peach.Pro.Core.Dom;
using Peach.Pro.Core.Mutators.Utility;
using Random = Peach.Core.Random;

// This assembly contains Peach plugins
[assembly: PluginAssembly]

namespace Peach.Pro.Core
{
    public static class Extensions
    {
        public static T GetStaticField<T>(this Type type, string name)
        {
            var bindingAttr = BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy;
            var field = type.GetField(name, bindingAttr);
            var ret = (T)field.GetValue(null);
            return ret;
        }

        public static T WeightedChoice<T>(this Random rng, WeightedList<T> list) where T : IWeighted
        {
            // returns between 0 <= x < UpperBound
            var val = rng.Next(list.Max);

            // Finds first element with sum-weight greater than value
            var ret = list.UpperBound(val);

            return ret.Item;
        }

        public static T[] WeightedSample<T>(this Random rng, WeightedList<T> list, int count) where T : IWeighted
        {
            // Shrink count so that we return the list
            // in a weighted order.
            if (count > list.Count)
                count = list.Count;

            var max = list.Max;
            var ret = new List<T>();
            var conversions = new SortedList<long, Func<long, long>>();

            for (int i = 0; i < count; ++i)
            {
                var rand = rng.Next(max);

                foreach (var c in conversions)
                    rand = c.Value(rand);

                var item = list.UpperBound(rand);
                var weight = item.UpperBound - item.LowerBound;

                conversions.Add(item.UpperBound, (c) =>
                {
                    if (c >= item.LowerBound)
                        return c + weight;
                    else
                        return c;
                });

                System.Diagnostics.Debug.Assert(!ret.Contains(item.Item));
                System.Diagnostics.Debug.Assert(max >= weight);

                ret.Add(item.Item);
                max -= weight;
            }

            return ret.ToArray();
        }

        public static IEnumerable<TSource> DistinctBy<TSource, TKey>
            (this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }

        static public byte[] Bytes(this DataElement obj)
        {
            if (obj is Number &&
                (obj.InternalValue.GetVariantType() == Variant.VariantType.Int ||
                 obj.InternalValue.GetVariantType() == Variant.VariantType.Long ||
                 obj.InternalValue.GetVariantType() == Variant.VariantType.ULong))
            {
                var num = obj as Number;
                var size = (num.lengthAsBits < 8 ? 8 : num.lengthAsBits) / 8;
                var val = (ulong)num.InternalValue;
                var bytes = new byte[size];
                for (int i = 0; i < size; i++)
                {
                    bytes[size - i - 1] = (byte)(val & 0xFF);
                    val >>= 8;
                }
                return bytes;
            }
            if (obj is Peach.Core.Dom.String && obj.InternalValue.GetVariantType() == Variant.VariantType.String)
            {
                var str = obj as Peach.Core.Dom.String;
                return System.Text.Encoding.ASCII.GetBytes((string)str.InternalValue);
            }
            try
            {
                var bs = (BitwiseStream)obj.InternalValue;
                if (bs != null)
                {
                    long pos = bs.PositionBits;
                    bs.SeekBits(0, SeekOrigin.Begin);
                    var ms = new MemoryStream();
                    bs.CopyTo(ms);
                    bs.SeekBits(pos, SeekOrigin.Begin);
                    return ms.ToArray();
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("Exception in Bytes(): " + e.ToString());
                Console.Error.WriteLine(e.StackTrace);
            }
            return new byte[0];
        }

        static public bool IsIn(this DataElement obj, string name)
        {
            var o = obj;
            while (o != null)
            {
                if (o.Name == name || o.Name.Contains("_" + name) || (o.referenceName != null && o.referenceName.Contains("_" + name)))
                    return true;
                o = o.parent;
            }
            return false;
        }

        public static void Dump(this byte[] bytes, TextWriter writer = null)
        {
            if (writer == null)
                writer = Console.Out;
            if (bytes == null)
                return;
            var len = bytes.Length;
            if (len == 0)
                return;

            var sb = new StringBuilder();

            for (int i = 0; i < len; i += 16)
            {
                sb.AppendFormat("{0:X8}   ", i);

                for (int j = 0; j < 16; j++)
                {
                    if (i + j < len)
                    {
                        sb.AppendFormat("{0:X2} ", bytes[i + j]);
                    }
                    else
                    {
                        sb.Append("   ");
                    }

                    if (j == 7) sb.Append(" ");
                }

                sb.Append("  ");

                for (int j = 0; j < 16; j++)
                {
                    if (i + j < len)
                    {
                        byte b = bytes[i + j];
                        char c = (b > 31 && b < 127) ? (char)b : '.';
                        sb.Append(c);
                    }
                }

                sb.AppendLine();
            }

            writer.WriteLine(sb.ToString());
        }

        public static void DumpDiff(this byte[] left, byte[] right, TextWriter writer = null)
        {
            if (writer == null)
                writer = Console.Out;

            DumpWithHighlight(left, right);
            writer.WriteLine("==============================================================================");
            DumpWithHighlight(right, left);
        }

        private static void DumpWithHighlight(byte[] source, byte[] reference, TextWriter writer = null)
        {
            if (writer == null)
                writer = Console.Out;

            if (source == null) return;

            var len = source.Length;
            var originalColor = Console.ForegroundColor;

            for (int i = 0; i < len; i += 16)
            {
                Console.ForegroundColor = originalColor;
                writer.Write("{0:X8}   ", i);

                for (int j = 0; j < 16; j++)
                {
                    int currentPos = i + j;
                    if (currentPos < len)
                    {
                        bool isDiff = (reference == null) || (currentPos >= reference.Length) || (source[currentPos] != reference[currentPos]);

                        if (isDiff) Console.ForegroundColor = ConsoleColor.Red;
                        else Console.ForegroundColor = originalColor;

                        writer.Write("{0:X2} ", source[currentPos]);
                    }
                    else
                    {
                        writer.Write("   ");
                    }

                    if (j == 7) writer.Write(" ");
                }

                writer.Write("  ");

                for (int j = 0; j < 16; j++)
                {
                    int currentPos = i + j;
                    if (currentPos < len)
                    {
                        byte b = source[currentPos];
                        char c = (b > 31 && b < 127) ? (char)b : '.';

                        bool isDiff = (reference == null) || (currentPos >= reference.Length) || (source[currentPos] != reference[currentPos]);

                        if (isDiff) Console.ForegroundColor = ConsoleColor.Red;
                        else Console.ForegroundColor = originalColor;

                        writer.Write(c);
                    }
                }

                writer.WriteLine();
            }

            Console.ForegroundColor = originalColor;
        }

        public static void SetUniqChild(this Peach.Core.Dom.Array arr, Variant v)
        {
            if (arr.Count > 1)
                throw new PeachException("SetUniqChild can only be called on an Array with zero or one child elements.");
            if (arr.Count == 0)
            {
                var elem = arr.OriginalElement.ShallowClone(arr, arr.Name + "_0");
                elem.SetValue(v);
                arr.Add(elem);
            }
            else
            {
                arr[0].SetValue(v);
            }
        }

        public static void SetValue(this Peach.Core.Dom.DataElement elem, Variant v)
        {
            if (v.GetVariantType() == Variant.VariantType.String && !(elem is Peach.Core.Dom.String))
            {
                v = new Variant(System.Text.Encoding.ASCII.GetBytes((string)v));
            }
            elem.MutatedValue = v;
            elem.mutationFlags = MutateOverride.Transformer | MutateOverride.Fixup;
        }

        public static byte GetUint8(this Number num)
        {
            var inter = num.InternalValue;
            if (inter.GetVariantType() != Variant.VariantType.Int &&
                inter.GetVariantType() != Variant.VariantType.Long &&
                inter.GetVariantType() != Variant.VariantType.ULong)
            {
                var bytes = Bytes(num);
                return bytes.Length > 0 ? bytes[0] : (byte)0;
            }
            return (byte)(ulong)inter;
        }

        public static ushort GetUint16(this Number num)
        {
            var inter = num.InternalValue;
            if (inter.GetVariantType() != Variant.VariantType.Int &&
                inter.GetVariantType() != Variant.VariantType.Long &&
                inter.GetVariantType() != Variant.VariantType.ULong)
            {
                var bytes = Bytes(num);
                return BitConverter.ToUInt16(bytes, bytes.Length - Math.Min(2, bytes.Length));
            }
            return (ushort)(ulong)inter;
        }

        public static uint GetVarInt(this MqttVarInt varInt)
        {
            var inter = varInt.InternalValue;
            if (inter.GetVariantType() != Variant.VariantType.Int &&
                inter.GetVariantType() != Variant.VariantType.Long &&
                inter.GetVariantType() != Variant.VariantType.ULong)
            {
                var bytes = Bytes(varInt);
                uint value = 0;
                int multiplier = 1;
                for (int i = 0; i < bytes.Length; i++)
                {
                    byte encodedByte = bytes[i];
                    value += (uint)(encodedByte & 127) * (uint)multiplier;
                    if ((encodedByte & 128) == 0)
                        break;
                    multiplier *= 128;
                }
                return value;
            }
            return (uint)(ulong)inter;
        }

        public static byte[] ToMqttString(this string data)
        {
            var utf8Bytes = System.Text.Encoding.UTF8.GetBytes(data);
            var length = (UInt16)utf8Bytes.Length;
            return new byte[] { (byte)(length >> 8), (byte)(length & 0xFF) }.Concat(utf8Bytes).ToArray();
        }
    }
}
