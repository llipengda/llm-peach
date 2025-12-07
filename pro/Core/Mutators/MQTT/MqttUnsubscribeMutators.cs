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
    [Mutator("MqttUnsubscribeMutatePacketIdentifier")]
    [CMutator("mutate_unsubscribe_packet_identifier")]
    [Description("Mutates MQTT Unsubscribe Packet ID")]
    public class MqttUnsubscribeMutatePacketIdentifier : Mutator
    {
        public MqttUnsubscribeMutatePacketIdentifier(DataElement obj) : base(obj) { }
        public new static bool supportedDataElement(DataElement obj) { return obj is Number && obj.Name == "packet_identifier" && obj.IsIn("unsubscribe"); }
        public override int count => 10;
        public override uint mutation { get; set; }

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj, (int)mutation); obj.mutationFlags = MutateOverride.Default; }
        public override void randomMutation(DataElement obj)
        {
            int[] weights = { 40, 40, 0, 0, 0, 0, 40, 40, 0, 0 };
            PerformMutation(obj, PickWeighted(weights));
            obj.mutationFlags = MutateOverride.Default;
        }

        private void PerformMutation(DataElement obj, int strategy)
        {
            uint val = 0;
            uint current = (uint)((Number)obj).InternalValue;
            switch (strategy)
            {
                case 0: val = 1; break;
                case 1: val = 0xFFFF; break;
                case 2: val = 0; break;
                case 3: val = current ^ 0xFFFF; break;
                case 4: val = ((current & 0xFF) << 8) | (current >> 8); break;
                case 5: val = (uint)context.Random.Next(0xFFFF); break;
                case 6: val = 0x1234; break;
                case 7: val = current; break;
                case 8: val = 0xABCD; break;
                case 9: val = (uint)(0xFFFF + context.Random.Next(100)); break;
            }
            obj.MutatedValue = new Variant(val & 0xFFFF);
        }
    }

    [Mutator("MqttUnsubscribeMutateProperties")]
    [CMutator("mutate_unsubscribe_properties")]
    [Description("Mutates MQTT Unsubscribe Properties")]
    public class MqttUnsubscribeMutateProperties : Mutator
    {
        public MqttUnsubscribeMutateProperties(DataElement obj) : base(obj) { }
        public new static bool supportedDataElement(DataElement obj) { return obj is Blob && obj.Name == "properties" && obj.IsIn("unsubscribe"); }
        public override int count => 4;
        public override uint mutation { get; set; }
        public override void sequentialMutation(DataElement obj) { PerformMutation(obj, (int)mutation); obj.mutationFlags = MutateOverride.Default; }
        public override void randomMutation(DataElement obj) { PerformMutation(obj, context.Random.Next(count)); obj.mutationFlags = MutateOverride.Default; }

        private void PerformMutation(DataElement obj, int strategy)
        {
            using (var ms = new MemoryStream())
            using (var bw = new BinaryWriter(ms))
            {
                int num = strategy; // 0..3
                string[] keys = { "source", "priority", "note", "device", "region" };
                string[] vals = { "sensor1", "high", "ok", "edge", "cn-north" };

                for (int i = 0; i < num; i++)
                {
                    bw.Write((byte)0x26);
                    WriteUtf8(bw, keys[context.Random.Next(keys.Length)]);
                    WriteUtf8(bw, vals[context.Random.Next(vals.Length)]);
                }
                obj.MutatedValue = new Variant(ms.ToArray());
            }
        }
        private void WriteUtf8(BinaryWriter writer, string str)
        {
            byte[] b = SysEncoding.UTF8.GetBytes(str);
            writer.Write((byte)((b.Length >> 8) & 0xFF));
            writer.Write((byte)(b.Length & 0xFF));
            writer.Write(b);
        }
    }

    [Mutator("MqttUnsubscribeAddProperties")]
    [CMutator("add_unsubscribe_properties")]
    [Description("Adds MQTT Unsubscribe Properties")]
    public class MqttUnsubscribeAddProperties : Mutator
    {
        public MqttUnsubscribeAddProperties(DataElement obj) : base(obj) { }
        public new static bool supportedDataElement(DataElement obj) { return obj is Blob && obj.Name == "properties" && obj.IsIn("unsubscribe"); }
        public override int count => 1;
        public override uint mutation { get; set; }
        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); obj.mutationFlags = MutateOverride.Default; }
        public override void randomMutation(DataElement obj) { PerformMutation(obj); obj.mutationFlags = MutateOverride.Default; }
        private void PerformMutation(DataElement obj)
        {
            // If empty, add one
            byte[] b = obj.Bytes();
            if (b != null && b.Length > 0) return;
            using (var ms = new MemoryStream())
            using (var bw = new BinaryWriter(ms))
            {
                bw.Write((byte)0x26);
                WriteUtf8(bw, "key");
                WriteUtf8(bw, "value");
                obj.MutatedValue = new Variant(ms.ToArray());
            }
        }
        private void WriteUtf8(BinaryWriter writer, string str)
        {
            byte[] b = SysEncoding.UTF8.GetBytes(str);
            writer.Write((byte)((b.Length >> 8) & 0xFF));
            writer.Write((byte)(b.Length & 0xFF));
            writer.Write(b);
        }
    }

    [Mutator("MqttUnsubscribeDeleteProperties")]
    [CMutator("delete_unsubscribe_properties")]
    [Description("Deletes MQTT Unsubscribe Properties")]
    public class MqttUnsubscribeDeleteProperties : Mutator
    {
        public MqttUnsubscribeDeleteProperties(DataElement obj) : base(obj) { }
        public new static bool supportedDataElement(DataElement obj) { return obj is Blob && obj.Name == "properties" && obj.IsIn("unsubscribe"); }
        public override int count => 1;
        public override uint mutation { get; set; }
        public override void sequentialMutation(DataElement obj) { obj.parent.Remove(obj); }
        public override void randomMutation(DataElement obj) { obj.parent.Remove(obj); }
    }

    [Mutator("MqttUnsubscribeRepeatProperties")]
    [CMutator("repeat_unsubscribe_properties")]
    [Description("Repeats MQTT Unsubscribe User Property")]
    public class MqttUnsubscribeRepeatProperties : Mutator
    {
        public MqttUnsubscribeRepeatProperties(DataElement obj) : base(obj) { }
        public new static bool supportedDataElement(DataElement obj) { return obj is Blob && obj.Name == "properties" && obj.IsIn("unsubscribe"); }
        public override int count => 1;
        public override uint mutation { get; set; }
        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); obj.mutationFlags = MutateOverride.Default; }
        public override void randomMutation(DataElement obj) { PerformMutation(obj); obj.mutationFlags = MutateOverride.Default; }
        private void PerformMutation(DataElement obj)
        {
            byte[] original = obj.Bytes();
            if (original == null || original.Length == 0) return;
            int pos = 0;
            while (pos < original.Length)
            {
                if (original[pos] == 0x26)
                {
                    int start = pos; pos++;
                    if (pos + 2 > original.Length) break;
                    int klen = (original[pos] << 8) | original[pos + 1]; pos += 2 + klen;
                    if (pos + 2 > original.Length) break;
                    int vlen = (original[pos] << 8) | original[pos + 1]; pos += 2 + vlen;
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
                else break;
            }
        }
    }

    [Mutator("MqttUnsubscribeMutateTopicFilters")]
    [CMutator("mutate_unsubscribe_topic_filters")]
    [Description("Mutates MQTT Unsubscribe Topic Filters")]
    public class MqttUnsubscribeMutateTopicFilters : Mutator
    {
        public MqttUnsubscribeMutateTopicFilters(DataElement obj) : base(obj) { }
        public new static bool supportedDataElement(DataElement obj)
        {
            return obj is Peach.Core.Dom.Block && obj.Name == "topic_filters" && obj.IsIn("unsubscribe");
        }
        public override int count => 10;
        public override uint mutation { get; set; }
        public override void sequentialMutation(DataElement obj) { PerformMutation(obj, (int)mutation); obj.mutationFlags = MutateOverride.Default; }
        public override void randomMutation(DataElement obj)
        {
            int[] weights = { 0, 0, 0, 0, 40, 0, 0, 0, 0, 0 };
            PerformMutation(obj, PickWeighted(weights));
            obj.mutationFlags = MutateOverride.Default;
        }
        private void PerformMutation(DataElement obj, int strategy)
        {
            // Placeholder for structure logic. Requires DOM manipulation not easily available.
            // Strategies 0..9 as defined in C.
        }
    }

    [Mutator("MqttUnsubscribeRepeatTopicFilters")]
    [CMutator("repeat_unsubscribe_topic_filters")]
    [Description("Repeats MQTT Unsubscribe Topic Filters")]
    public class MqttUnsubscribeRepeatTopicFilters : Mutator
    {
        public MqttUnsubscribeRepeatTopicFilters(DataElement obj) : base(obj) { }
        public new static bool supportedDataElement(DataElement obj)
        {
            return obj is Peach.Core.Dom.Block && obj.Name == "topic_filters" && obj.IsIn("unsubscribe");
        }
        public override int count => 1;
        public override uint mutation { get; set; }
        public override void sequentialMutation(DataElement obj) { }
        public override void randomMutation(DataElement obj) { }
    }
}
