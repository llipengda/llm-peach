using System;
using System.Linq;

using uint8 = System.Byte;
using int8 = System.SByte;
using int16 = System.Int16;
using int32 = System.Int32;
using uint16 = System.UInt16;
using uint32 = System.UInt32;
using uint64 = System.UInt64;
using int64 = System.Int64;
using Peach.Core.Dom;
using Peach.Core.IO;
using System.IO;
using Microsoft.Scripting.Hosting.Shell.Remote;

namespace Peach.Pro.Core.Mutators.MQTT
{
    static class MqttUtils
    {
        static readonly Random rng = new Random();

        static public int PickWeighted(int[] weights)
        {
            int sum = 0;
            foreach (int w in weights) sum += w;
            if (sum <= 0) return 0;
            int r = rng.Next(sum);
            for (int i = 0; i < weights.Length; i++)
            {
                if (r < weights[i]) return i;
                r -= weights[i];
            }
            return weights.Length - 1;
        }

        static public string FromUtf8(byte[] data)
        {
            return System.Text.Encoding.UTF8.GetString(data);
        }

        static public byte[] ToUtf8(string data)
        {
            return System.Text.Encoding.UTF8.GetBytes(data);
        }

        static public byte[] ToMqttString(string data)
        {
            var utf8Bytes = ToUtf8(data);
            var length = (uint16)utf8Bytes.Length;
            return new uint8[] { (uint8)(length >> 8), (uint8)(length & 0xFF) }.Concat(utf8Bytes).ToArray();
        }

        static public string FromMqttString(byte[] data)
        {
            if (data == null || data.Length < 2) return "";
            var length = (uint16)(data[0] << 8 | data[1]);
            return FromUtf8(data.Skip(2).Take(length).ToArray());
        }
    }

    static class DataElementExtensions
    {
        static public byte[] Bytes(this DataElement obj)
        {
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
            catch { }
            return new byte[0];
        }

        static public bool IsIn(this DataElement obj, string name)
        {
            // try
            // {
            var o = obj;
            while (o != null)
            {
                if (o.Name == name || o.Name.Contains("_" + name) || (o.referenceName != null && o.referenceName.Contains("_" + name)))
                    return true;
                o = o.parent;
            }
            return false;
            // }
            // catch (Exception e)
            // {
            //     Console.WriteLine($"Error in IsIn: {e.Message}");
            //     Console.WriteLine(e.StackTrace);
            //     return false;
            // }
        }
    }
}