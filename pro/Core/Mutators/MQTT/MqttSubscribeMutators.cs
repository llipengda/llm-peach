using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.IO;
using Peach.Core;
using Peach.Core.Dom;
using Peach.Pro.Core.Mutators;
using Peach.Core.IO;

using SysRandom = System.Random;
using SysEncoding = System.Text.Encoding;

using static Peach.Pro.Core.Mutators.MQTT.MqttUtils;

namespace Peach.Pro.Core.Mutators.MQTT
{
    [Mutator("MqttSubscribeMutatePacketIdentifier")]
    [CMutator("mutate_subscribe_packet_identifier")]
    [Description("Mutates MQTT Subscribe Packet Identifier")]
    public class MqttSubscribeMutatePacketIdentifier : MqttMutator
    {
        public MqttSubscribeMutatePacketIdentifier(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj is Peach.Core.Dom.Number && obj.Name == "packet_identifier" &&
                   obj.IsIn("subscribe");
        }

        public override int count => 10;
        public override uint mutation { get; set; }

        public override void sequentialMutation(DataElement obj)
        {
            PerformMutation(obj, (int)mutation);
            obj.mutationFlags = MutateOverride.Default;
        }

        public override void randomMutation(DataElement obj)
        {
            int[] weights = { 0, 40, 40, 0, 0, 0, 40, 40, 40, 40 };
            PerformMutation(obj, PickWeighted(weights));
            obj.mutationFlags = MutateOverride.Default;
        }

        private void PerformMutation(DataElement obj, int strategy)
        {
            uint orig = (uint)((Peach.Core.Dom.Number)obj).InternalValue;
            uint mutated = orig;

            switch (strategy)
            {
                case 0: mutated = 0; break; // 设置为0（非法）
                case 1: mutated = 65535; break; // 设置为最大合法值
                case 2: mutated = 1; break; // 设置为最小合法值
                case 3: mutated = (uint)Next(65536); break; // 生成完全随机值
                case 4: mutated = 0xFF00 | (uint)(Next(256)); break; // 高位全1，低位随机
                case 5: mutated = orig ^ (1u << Next(16)); break; // 翻转一位
                case 6: mutated = orig + 1; break; // 加1
                case 7: mutated = orig - 1; break; // 减1
                case 8: mutated = orig; break; // 设置为前一个包的 ID (Simulated as same)
                case 9: mutated = 0x8000; break; // 设置为常见边界值
            }
            obj.MutatedValue = new Variant(mutated & 0xFFFF);
        }
    }

    [Mutator("MqttSubscribeMutateProperties")]
    [CMutator("mutate_subscribe_properties")]
    [Description("Mutates MQTT Subscribe Properties (Rebuild)")]
    public class MqttSubscribeMutateProperties : MqttMutator
    {
        public MqttSubscribeMutateProperties(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj is Blob && obj.Name == "properties" &&
                   obj.IsIn("subscribe");
        }

        public override int count => 1;
        public override uint mutation { get; set; }

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); obj.mutationFlags = MutateOverride.Default; }
        public override void randomMutation(DataElement obj) { PerformMutation(obj); obj.mutationFlags = MutateOverride.Default; }

        private void PerformMutation(DataElement obj)
        {
            obj.MutatedValue = new Variant(GenerateProperties());
        }

        private byte[] GenerateProperties()
        {
            using (var ms = new MemoryStream())
            using (var writer = new BinaryWriter(ms))
            {
                // 50% 是否带 Subscription Identifier
                if (Next(2) != 0)
                {
                    writer.Write((byte)0x0B);
                    WriteVarInt(writer, 1 + Next(16383));
                }
                // 追加 0..3 个 User Property
                string[] keys = { "source", "priority", "note", "device" };
                string[] vals = { "sensor1", "high", "ok", "edge" };
                int upn = Next(4);
                for (int t = 0; t < upn; ++t)
                {
                    writer.Write((byte)0x26);
                    WriteUtf8(writer, keys[Next(4)]);
                    WriteUtf8(writer, vals[Next(4)]);
                }
                return ms.ToArray();
            }
        }

        private void WriteVarInt(BinaryWriter writer, int val)
        {
            do
            {
                byte b = (byte)(val % 128);
                val /= 128;
                if (val > 0) b |= 0x80;
                writer.Write(b);
            } while (val > 0);
        }

        private void WriteUtf8(BinaryWriter writer, string str)
        {
            byte[] b = SysEncoding.UTF8.GetBytes(str);
            writer.Write((byte)((b.Length >> 8) & 0xFF));
            writer.Write((byte)(b.Length & 0xFF));
            writer.Write(b);
        }
    }

    [Mutator("MqttSubscribeAddProperties")]
    [CMutator("add_subscribe_properties")]
    [Description("Adds MQTT Subscribe Properties")]
    public class MqttSubscribeAddProperties : MqttMutator
    {
        public MqttSubscribeAddProperties(DataElement obj) : base(obj) { }
        public new static bool supportedDataElement(DataElement obj)
        {
            return obj is Blob && obj.Name == "properties" &&
                   obj.IsIn("subscribe");
        }
        public override int count => 1;
        public override uint mutation { get; set; }
        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); obj.mutationFlags = MutateOverride.Default; }
        public override void randomMutation(DataElement obj) { PerformMutation(obj); obj.mutationFlags = MutateOverride.Default; }

        private void PerformMutation(DataElement obj)
        {
            byte[] original = GetOriginalBytes(obj);
            if (original == null) original = new byte[0];

            // Logic matches add_subscribe_properties in C:
            // 1. Check if Sub ID exists. If not, add it.
            // 2. Else append User Property.
            // Need to parse original props.
            bool has_sid = false;
            int pos = 0;
            while (pos < original.Length)
            {
                byte id = original[pos];
                if (id == 0x0B) { has_sid = true; break; }
                else if (id == 0x26)
                {
                    // skip user prop
                    pos++; if (pos + 2 > original.Length) break;
                    int k = (original[pos] << 8) | original[pos + 1]; pos += 2 + k;
                    if (pos + 2 > original.Length) break;
                    int v = (original[pos] << 8) | original[pos + 1]; pos += 2 + v;
                }
                else { has_sid = true; break; } // Unknown, treat as has_sid to avoid mess
            }

            using (var ms = new MemoryStream())
            using (var writer = new BinaryWriter(ms))
            {
                ms.Write(original, 0, original.Length);
                if (!has_sid)
                {
                    writer.Write((byte)0x0B);
                    WriteVarInt(writer, 1 + Next(16383));
                }
                else
                {
                    writer.Write((byte)0x26);
                    WriteUtf8(writer, "foo");
                    WriteUtf8(writer, "bar");
                }
                obj.MutatedValue = new Variant(ms.ToArray());
            }
        }
        private void WriteVarInt(BinaryWriter writer, int val)
        {
            do
            {
                byte b = (byte)(val % 128);
                val /= 128;
                if (val > 0) b |= 0x80;
                writer.Write(b);
            } while (val > 0);
        }
        private void WriteUtf8(BinaryWriter writer, string str)
        {
            byte[] b = SysEncoding.UTF8.GetBytes(str);
            writer.Write((byte)((b.Length >> 8) & 0xFF));
            writer.Write((byte)(b.Length & 0xFF));
            writer.Write(b);
        }

        private byte[] GetOriginalBytes(DataElement obj)
        {
            try { return (byte[])obj.InternalValue; } catch { }
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
            return null;
        }
    }

    [Mutator("MqttSubscribeDeleteProperties")]
    [CMutator("delete_subscribe_properties")]
    [Description("Deletes MQTT Subscribe Properties")]
    public class MqttSubscribeDeleteProperties : MqttMutator
    {
        public MqttSubscribeDeleteProperties(DataElement obj) : base(obj) { }
        public new static bool supportedDataElement(DataElement obj)
        {
            return obj is Blob && obj.Name == "properties" &&
                   obj.IsIn("subscribe");
        }
        public override int count => 1;
        public override uint mutation { get; set; }
        public override void sequentialMutation(DataElement obj) { obj.parent.Remove(obj); }
        public override void randomMutation(DataElement obj) { obj.parent.Remove(obj); }
    }

    [Mutator("MqttSubscribeRepeatProperties")]
    [CMutator("repeat_subscribe_properties")]
    [Description("Repeats MQTT Subscribe User Property")]
    public class MqttSubscribeRepeatProperties : MqttMutator
    {
        public MqttSubscribeRepeatProperties(DataElement obj) : base(obj) { }
        public new static bool supportedDataElement(DataElement obj)
        {
            return obj is Blob && obj.Name == "properties" &&
                   obj.IsIn("subscribe");
        }
        public override int count => 1;
        public override uint mutation { get; set; }
        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); obj.mutationFlags = MutateOverride.Default; }
        public override void randomMutation(DataElement obj) { PerformMutation(obj); obj.mutationFlags = MutateOverride.Default; }

        private void PerformMutation(DataElement obj)
        {
            byte[] original = GetOriginalBytes(obj);
            if (original == null || original.Length == 0) return;

            int pos = 0;
            while (pos < original.Length)
            {
                byte id = original[pos];
                if (id == 0x26)
                {
                    int start = pos;
                    pos++;
                    if (pos + 2 > original.Length) break;
                    int klen = (original[pos] << 8) | original[pos + 1];
                    pos += 2 + klen;
                    if (pos + 2 > original.Length) break;
                    int vlen = (original[pos] << 8) | original[pos + 1];
                    pos += 2 + vlen;
                    if (pos <= original.Length)
                    {
                        int len = pos - start;
                        using (var ms = new MemoryStream())
                        {
                            ms.Write(original, 0, original.Length);
                            ms.Write(original, start, len);
                            obj.MutatedValue = new Variant(ms.ToArray());
                        }
                    }
                    break;
                }
                else if (id == 0x0B)
                {
                    pos++;
                    int count = 0;
                    while (pos < original.Length && count < 4)
                    {
                        if ((original[pos++] & 0x80) == 0) break;
                        count++;
                    }
                }
                else break;
            }
        }

        private byte[] GetOriginalBytes(DataElement obj)
        {
            try { return (byte[])obj.InternalValue; } catch { }
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
            return null;
        }
    }

    [Mutator("MqttSubscribeMutateTopicFilter")]
    [CMutator("mutate_subscribe_topic_filter")]
    [Description("Mutates MQTT Subscribe Topic Filter")]
    public class MqttSubscribeMutateTopicFilter : MqttMutator
    {
        public MqttSubscribeMutateTopicFilter(DataElement obj) : base(obj) { }
        public new static bool supportedDataElement(DataElement obj)
        {
            return obj is Peach.Core.Dom.String && obj.Name == "value" &&
                   obj.parent != null && obj.parent.Name == "topic_filter" ;
        }
        public override int count => 6;
        public override uint mutation { get; set; }

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj, (int)mutation); obj.mutationFlags = MutateOverride.Default; }
        public override void randomMutation(DataElement obj)
        {
            int[] weights = { 20, 20, 20, 20, 10, 10 };
            PerformMutation(obj, PickWeighted(weights));
            obj.mutationFlags = MutateOverride.Default;
        }

        private void PerformMutation(DataElement obj, int strategy)
        {
            string val = "topic";
            string[] legalWildcards = { "#", "+", "+/+", "devices/+/status", "sensor/#" };
            string legalChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_-";

            switch (strategy)
            {
                case 0: // 直接使用一条“已知合法”的模板
                    val = legalWildcards[Next(legalWildcards.Length)];
                    break;
                case 1: // 纯静态合法路径
                    {
                        int levels = 1 + Next(4);
                        var sb = new StringBuilder();
                        for (int i = 0; i < levels; i++)
                        {
                            if (i > 0) sb.Append("/");
                            int len = 1 + Next(8);
                            for (int c = 0; c < len; c++) sb.Append(legalChars[Next(legalChars.Length)]);
                        }
                        val = sb.ToString();
                    }
                    break;
                case 2: // 含 '+' 的合法过滤器
                    {
                        int levels = 2 + Next(3); // 2..4
                        var sb = new StringBuilder();
                        int plus_cnt = 1 + Next(2);
                        int[] plus_at = { -1, -1 };
                        for (int p = 0; p < plus_cnt; p++)
                        {
                            int idx;
                            do { idx = Next(levels); } while (p == 1 && idx == plus_at[0]);
                            plus_at[p] = idx;
                        }
                        for (int l = 0; l < levels; ++l)
                        {
                            if (l > 0) sb.Append("/");
                            if (l == plus_at[0] || l == plus_at[1]) sb.Append("+");
                            else
                            {
                                int len = 1 + Next(8);
                                for (int c = 0; c < len; c++) sb.Append(legalChars[Next(legalChars.Length)]);
                            }
                        }
                        val = sb.ToString();
                    }
                    break;
                case 3: // 末尾 '#'
                    {
                        int levels = Next(4); // 0..3
                        var sb = new StringBuilder();
                        if (levels == 0) sb.Append("#");
                        else
                        {
                            for (int l = 0; l < levels; ++l)
                            {
                                if (l > 0) sb.Append("/");
                                int len = 1 + Next(8);
                                for (int c = 0; c < len; c++) sb.Append(legalChars[Next(legalChars.Length)]);
                            }
                            sb.Append("/#");
                        }
                        val = sb.ToString();
                    }
                    break;
                case 4: // 构造“比较长但合法”的过滤器
                    {
                        var sb = new StringBuilder();
                        while (sb.Length < 65535)
                        { // MAX_TOPIC_LEN 65535
                            int left = 65535 - sb.Length;
                            if (left <= 5) break;
                            if (sb.Length > 0) sb.Append("/");
                            int seg = 4 + Next(8);
                            for (int s = 0; s < seg && sb.Length < 65535; ++s) sb.Append((char)('a' + Next(26)));
                        }
                        val = sb.ToString();
                    }
                    break;
                case 5: // 拷贝前一个合法 filter (simulated)
                    val = "sensor/#";
                    break;
            }
            obj.MutatedValue = new Variant(val);
        }
    }

    [Mutator("MqttSubscribeRepeatTopicFilter")]
    [CMutator("repeat_subscribe_topic_filter")]
    [Description("Repeats (Duplicates) MQTT Subscribe Topic Filter")]
    public class MqttSubscribeRepeatTopicFilter : MqttMutator
    {
        public MqttSubscribeRepeatTopicFilter(DataElement obj) : base(obj) { }
        public new static bool supportedDataElement(DataElement obj)
        {
            return obj is Peach.Core.Dom.Block && obj.Name == "topic_filters";
        }
        public override int count => 1;
        public override uint mutation { get; set; }
        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); obj.mutationFlags = MutateOverride.Default; }
        public override void randomMutation(DataElement obj) { PerformMutation(obj); obj.mutationFlags = MutateOverride.Default; }

        private void PerformMutation(DataElement obj)
        {
            // Placeholder: Peach Mutators can't easily add children to Block unless using ArrayMutator strategies
            // Attempt to act as ArrayVariance
        }
    }

    [Mutator("MqttSubscribeMutateQoS")]
    [CMutator("mutate_subscribe_qos")]
    [Description("Mutates MQTT Subscribe QoS")]
    public class MqttSubscribeMutateQoS : MqttMutator
    {
        public MqttSubscribeMutateQoS(DataElement obj) : base(obj) { }
        public new static bool supportedDataElement(DataElement obj)
        {
            return obj is Peach.Core.Dom.Number && obj.Name == "qos" &&
                   obj.IsIn("topic_filters");
        }
        public override int count => 10;
        public override uint mutation { get; set; }

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj, (int)mutation); obj.mutationFlags = MutateOverride.Default; }
        public override void randomMutation(DataElement obj)
        {
            int[] weights = { 40, 40, 40, 0, 0, 0, 0, 0, 0, 40 };
            PerformMutation(obj, PickWeighted(weights));
            obj.mutationFlags = MutateOverride.Default;
        }

        private void PerformMutation(DataElement obj, int strategy)
        {
            uint val = 0;
            switch (strategy)
            {
                case 0: val = 0; break;
                case 1: val = 1; break;
                case 2: val = 2; break;
                case 3: val = 3; break; // 非法值
                case 4: val = 255; break;
                case 5: val = (uint)Next(3); break;
                case 6: val = (uint)(3 + Next(252)); break;
                case 7: val = (uint)((Number)obj).InternalValue ^ (1u << Next(3)); break;
                case 8: val = (uint)((Number)obj).InternalValue; break; // Copy previous (simulated as same)
                case 9: val = 0; break;
            }
            obj.MutatedValue = new Variant(val & 0xFF);
        }
    }

    [Mutator("MqttSubscribeMutateTopicCount")]
    [CMutator("mutate_subscribe_topic_count")]
    [Description("Mutates MQTT Subscribe Topic Count")]
    public class MqttSubscribeMutateTopicCount : MqttMutator
    {
        public MqttSubscribeMutateTopicCount(DataElement obj) : base(obj) { }
        public new static bool supportedDataElement(DataElement obj)
        {
            return obj is Peach.Core.Dom.Array && obj.Name == "topic_filters";
        }
        public override int count => 10;
        public override uint mutation { get; set; }

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj, (int)mutation); obj.mutationFlags = MutateOverride.Default; }
        public override void randomMutation(DataElement obj)
        {
            int[] weights = { 0, 40, 0, 40, 40, 0, 0, 0, 0, 0 };
            PerformMutation(obj, PickWeighted(weights));
            obj.mutationFlags = MutateOverride.Default;
        }

        private void PerformMutation(DataElement obj, int strategy)
        {
            var array = obj as Peach.Core.Dom.Array;
            uint orig = (uint)array.Count;
            uint val = orig;
            int MAX_TOPIC_FILTERS = 100; // Assume

            switch (strategy)
            {
                case 0: val = 0; break;
                case 1: val = (uint)MAX_TOPIC_FILTERS; break;
                case 2: val = (uint)MAX_TOPIC_FILTERS + 1; break;
                case 3: val = 1; break;
                case 4: val = (uint)(1 + Next(MAX_TOPIC_FILTERS)); break;
                case 5: val = (uint)(MAX_TOPIC_FILTERS + 2 + Next(255 - MAX_TOPIC_FILTERS - 2)); break;
                case 6: val = orig * 2; break;
                case 7: val = ~orig; break;
                case 8: val = orig ^ 1; break;
                case 9: val = orig; break; // Copy previous
            }
            
            array.SetCountOverride((int)val, null, 0);
        }
    }
}
