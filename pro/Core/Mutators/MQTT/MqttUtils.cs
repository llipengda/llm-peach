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

namespace Peach.Pro.Core.Mutators.MQTT
{
    static class MqttUtils
    {
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
}