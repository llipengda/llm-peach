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
using SysArray = System.Array;

using static Peach.LLM.Core.Mutators.MQTT.MqttUtils;

namespace Peach.LLM.Core.Mutators.MQTT
{
    // Packet Identifier Mutators
    [Mutator("MqttPublishMutatePacketIdentifier")]
    [CMutator("mutate_publish_packet_identifier")]
    [Description("Mutates MQTT Publish Packet ID")]
    public class MqttPublishMutatePacketIdentifier : LLMMutator
    {
        public MqttPublishMutatePacketIdentifier(DataElement obj) : base(obj) { }
        public new static bool supportedDataElement(DataElement obj)
        {
            return obj is Number && obj.Name == "packet_identifier" &&
                   obj.IsIn("publish");
        }
        public override int count => 10;
        public override uint mutation { get; set; }

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj, (int)mutation); obj.mutationFlags = MutateOverride.Default; }
        public override void randomMutation(DataElement obj)
        {
            int[] weights = { 0, 40, 40, 40, 0, 40, 40, 0, 0, 0 };
            PerformMutation(obj, PickWeighted(weights));
            obj.mutationFlags = MutateOverride.Default;
        }

        private void PerformMutation(DataElement obj, int strategy)
        {
            uint val = 0;
            uint current = (uint)((Number)obj).InternalValue;
            switch (strategy)
            {
                case 0: val = 0; break;
                case 1: val = 1; break;
                case 2: val = 0xFFFF; break;
                case 3: val = (uint)Next(0xFFFF); break;
                case 4: val = (uint)Next(int.MaxValue); break;
                case 5: val = 0x7FFF; break;
                case 6: val = 0x8000; break;
                case 7: val = current ^ 0xAAAA; break;
                case 8: val = ~current; break;
                case 9: val = current + 1; break;
            }
            obj.MutatedValue = new Variant(val & 0xFFFF);
        }
    }

    [Mutator("MqttPublishAddPacketIdentifier")]
    [CMutator("add_publish_packet_identifier")]
    [Description("Adds (Populates) MQTT Publish Packet ID")]
    public class MqttPublishAddPacketIdentifier : LLMMutator
    {
        public MqttPublishAddPacketIdentifier(DataElement obj) : base(obj) { }
        public new static bool supportedDataElement(DataElement obj)
        {
            return obj is Number && obj.Name == "packet_identifier" && obj.IsIn("publish");
        }
        public override int count => 1;
        public override uint mutation { get; set; }
        public override void sequentialMutation(DataElement obj)
        {
            if ((uint)((Number)obj).InternalValue == 0) obj.MutatedValue = new Variant(1 + Next(0xFFFF));
            obj.mutationFlags = MutateOverride.Default;
        }
        public override void randomMutation(DataElement obj) { sequentialMutation(obj); }
    }

    [Mutator("MqttPublishDeletePacketIdentifier")]
    [CMutator("delete_publish_packet_identifier")]
    [Description("Deletes (Clears) MQTT Publish Packet ID")]
    public class MqttPublishDeletePacketIdentifier : LLMMutator
    {
        public MqttPublishDeletePacketIdentifier(DataElement obj) : base(obj) { }
        public new static bool supportedDataElement(DataElement obj)
        {
            return obj is Number && obj.Name == "packet_identifier" && obj.IsIn("publish");
        }
        public override int count => 1;
        public override uint mutation { get; set; }
        public override void sequentialMutation(DataElement obj) { obj.MutatedValue = new Variant(0); obj.mutationFlags = MutateOverride.Default; }
        public override void randomMutation(DataElement obj) { obj.MutatedValue = new Variant(0); obj.mutationFlags = MutateOverride.Default; }
    }

    // Topic Name Mutators
    [Mutator("MqttPublishMutateTopicName")]
    [CMutator("mutate_publish_topic_name")]
    [Description("Mutates MQTT Publish Topic Name")]
    public class MqttPublishMutateTopicName : LLMMutator
    {
        public MqttPublishMutateTopicName(DataElement obj) : base(obj) { }
        public new static bool supportedDataElement(DataElement obj) { return obj is Peach.Core.Dom.String && obj.Name == "value" && obj.parent != null && obj.parent.Name == "topic_name"; }
        public override int count => 10;
        public override uint mutation { get; set; }

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj, (int)mutation); obj.mutationFlags = MutateOverride.Default; }
        public override void randomMutation(DataElement obj)
        {
            int[] weights = { 0, 0, 0, 0, 0, 0, 0, 50, 0, 0 };
            PerformMutation(obj, PickWeighted(weights));
            obj.mutationFlags = MutateOverride.Default;
        }

        private void PerformMutation(DataElement obj, int strategy)
        {
            string val = "topic";
            string current = (string)obj.InternalValue;
            switch (strategy)
            {
                case 0: val = ""; break;
                case 1: val = "+"; break;
                case 2: val = "#"; break;
                case 3: val = "invalid/#/test#"; break;
                case 4: val = new string('A', 65535 + 10); break;
                case 5: val = "sensor/+/temperature"; break;
                case 6:
                    byte[] b = new byte[65535];
                    NextBytes(b);
                    obj.MutatedValue = new Variant(b);
                    return;
                case 7: val = "home/kitchen/light"; break;
                case 8: val = "topic/!@#$%^&*()"; break;
                case 9: val = "prefix_" + current + "_suffix"; break;
            }
            obj.MutatedValue = new Variant(ToUtf8(val));
        }
    }

    [Mutator("MqttPublishAddTopicName")]
    [CMutator("add_publish_topic_name")]
    [Description("Adds (Populates) MQTT Publish Topic Name")]
    public class MqttPublishAddTopicName : LLMMutator
    {
        public MqttPublishAddTopicName(DataElement obj) : base(obj) { }
        public new static bool supportedDataElement(DataElement obj) { return obj is Peach.Core.Dom.String && obj.Name == "value" && obj.parent != null && obj.parent.Name == "topic_name"; }
        public override int count => 1;
        public override uint mutation { get; set; }
        public override void sequentialMutation(DataElement obj)
        {
            if (string.IsNullOrEmpty((string)obj.InternalValue)) obj.MutatedValue = new Variant("test/topic/added");
            obj.mutationFlags = MutateOverride.Default;
        }
        public override void randomMutation(DataElement obj) { sequentialMutation(obj); }
    }

    [Mutator("MqttPublishDeleteTopicName")]
    [CMutator("delete_publish_topic_name")]
    [Description("Deletes (Clears) MQTT Publish Topic Name")]
    public class MqttPublishDeleteTopicName : LLMMutator
    {
        public MqttPublishDeleteTopicName(DataElement obj) : base(obj) { }
        public new static bool supportedDataElement(DataElement obj)
        {
            return obj is Peach.Core.Dom.String && obj.Name == "value" && obj.parent != null && obj.parent.Name == "topic_name";
        }
        public override int count => 1;
        public override uint mutation { get; set; }
        public override void sequentialMutation(DataElement obj) { obj.MutatedValue = new Variant(""); obj.mutationFlags = MutateOverride.Default; }
        public override void randomMutation(DataElement obj) { obj.MutatedValue = new Variant(""); obj.mutationFlags = MutateOverride.Default; }
    }

    // Properties Mutators
    [Mutator("MqttPublishMutateProperties")]
    [CMutator("mutate_publish_properties")]
    [Description("Mutates MQTT Publish Properties")]
    public class MqttPublishMutateProperties : LLMMutator
    {
        public MqttPublishMutateProperties(DataElement obj) : base(obj) { }
        public new static bool supportedDataElement(DataElement obj) { return obj is Blob && obj.Name == "properties" && obj.IsIn("publish"); }
        public override int count => 6;
        public override uint mutation { get; set; }
        public override void sequentialMutation(DataElement obj) { PerformMutation(obj, (int)mutation); obj.mutationFlags = MutateOverride.Default; }
        public override void randomMutation(DataElement obj) { PerformMutation(obj, Next(count)); obj.mutationFlags = MutateOverride.Default; }
        private void PerformMutation(DataElement obj, int strategy)
        {
            obj.MutatedValue = new Variant(GenerateProperties(strategy));
        }
        private byte[] GenerateProperties(int strategy)
        {
            using (var ms = new MemoryStream())
            using (var writer = new BinaryWriter(ms))
            {
                bool used_pfi = false, used_mei = false, used_ct = false, used_rt = false, used_cd = false, used_ta = false;

                switch (strategy)
                {
                    case 0: // Clear
                        break;
                    case 1: // PFI=1 + optional CT
                        if (!used_pfi) { writer.Write((byte)0x01); writer.Write((byte)1); used_pfi = true; }
                        if (Next(2) != 0 && !used_ct) { WriteUtf8(writer, 0x03, "text/plain"); used_ct = true; }
                        break;
                    case 2: // MEI + optional PFI
                        if (!used_mei) { writer.Write((byte)0x02); WriteBigEndian(writer, (uint)Next(7200)); used_mei = true; }
                        if (Next(2) != 0 && !used_pfi) { writer.Write((byte)0x01); writer.Write((byte)Next(2)); used_pfi = true; }
                        break;
                    case 3: // TA + optional RT
                        if (!used_ta) { writer.Write((byte)0x23); WriteBigEndian(writer, (ushort)(1 + Next(100))); used_ta = true; }
                        if (Next(2) != 0 && !used_rt) { WriteUtf8(writer, 0x08, "reply/topic"); used_rt = true; }
                        break;
                    case 4: // CD + RT
                        if (!used_rt) { WriteUtf8(writer, 0x08, "reply/topic"); used_rt = true; }
                        if (!used_cd)
                        {
                            byte[] tmp = new byte[8 + Next(17)];
                            NextBytes(tmp);
                            writer.Write((byte)0x09); WriteBigEndian(writer, (ushort)tmp.Length); writer.Write(tmp); used_cd = true;
                        }
                        break;
                    case 5: // Mixed
                        if (!used_pfi && Next(2) != 0) { writer.Write((byte)0x01); writer.Write((byte)Next(2)); used_pfi = true; }
                        if (!used_mei && Next(2) != 0) { writer.Write((byte)0x02); WriteBigEndian(writer, (uint)Next(7200)); used_mei = true; }
                        if (!used_ct && Next(2) != 0) { WriteUtf8(writer, 0x03, "application/json"); used_ct = true; }
                        if (!used_rt && Next(2) != 0) { WriteUtf8(writer, 0x08, "resp/alpha"); used_rt = true; }
                        if (!used_cd && Next(2) != 0)
                        {
                            byte[] tmp = new byte[6 + Next(9)];
                            NextBytes(tmp);
                            writer.Write((byte)0x09); WriteBigEndian(writer, (ushort)tmp.Length); writer.Write(tmp); used_cd = true;
                        }
                        if (!used_ta && Next(2) != 0) { writer.Write((byte)0x23); WriteBigEndian(writer, (ushort)(1 + Next(100))); used_ta = true; }

                        int upn = Next(4);
                        for (int i = 0; i < upn; i++)
                        {
                            writer.Write((byte)0x26);
                            WriteUtf8Val(writer, (i % 2 != 0) ? "source" : "note", (i % 2 != 0) ? "edge" : "ok");
                        }
                        break;
                }
                return ms.ToArray();
            }
        }
        private void WriteBigEndian(BinaryWriter w, uint v) { w.Write((byte)((v >> 24) & 0xFF)); w.Write((byte)((v >> 16) & 0xFF)); w.Write((byte)((v >> 8) & 0xFF)); w.Write((byte)(v & 0xFF)); }
        private void WriteBigEndian(BinaryWriter w, ushort v) { w.Write((byte)((v >> 8) & 0xFF)); w.Write((byte)(v & 0xFF)); }
        private void WriteUtf8(BinaryWriter w, byte id, string s) { w.Write(id); byte[] b = SysEncoding.UTF8.GetBytes(s); WriteBigEndian(w, (ushort)b.Length); w.Write(b); }
        private void WriteUtf8Val(BinaryWriter w, string s) { byte[] b = SysEncoding.UTF8.GetBytes(s); WriteBigEndian(w, (ushort)b.Length); w.Write(b); }
        private void WriteUtf8Val(BinaryWriter w, string k, string v) { WriteUtf8Val(w, k); WriteUtf8Val(w, v); }
    }

    [Mutator("MqttPublishAddProperties")]
    [CMutator("add_publish_properties")]
    [Description("Adds MQTT Publish Properties")]
    public class MqttPublishAddProperties : LLMMutator
    {
        public MqttPublishAddProperties(DataElement obj) : base(obj) { }
        public new static bool supportedDataElement(DataElement obj) { return obj is Blob && obj.Name == "properties" && obj.IsIn("publish"); }
        public override int count => 5;
        public override uint mutation { get; set; }
        public override void sequentialMutation(DataElement obj) { PerformMutation(obj, (int)mutation); obj.mutationFlags = MutateOverride.Default; }
        public override void randomMutation(DataElement obj) { PerformMutation(obj, Next(count)); obj.mutationFlags = MutateOverride.Default; }
        private void PerformMutation(DataElement obj, int strategy)
        {
            using (var ms = new MemoryStream())
            using (var writer = new BinaryWriter(ms))
            {
                byte[] b = GetOriginalBytes(obj);
                if (b != null && b.Length > 0) return; // Only if empty

                switch (strategy)
                {
                    case 0: writer.Write((byte)0x01); writer.Write((byte)1); break;
                    case 1: writer.Write((byte)0x02); WriteBigEndian(writer, (uint)Next(3601)); break;
                    case 2: WriteUtf8(writer, 0x03, "text/plain"); break;
                    case 3: writer.Write((byte)0x23); WriteBigEndian(writer, (ushort)(1 + Next(100))); break;
                    case 4: writer.Write((byte)0x26); WriteUtf8Val(writer, "key"); WriteUtf8Val(writer, "value"); break;
                }
                obj.MutatedValue = new Variant(ms.ToArray());
            }
        }
        private void WriteBigEndian(BinaryWriter w, uint v) { w.Write((byte)((v >> 24) & 0xFF)); w.Write((byte)((v >> 16) & 0xFF)); w.Write((byte)((v >> 8) & 0xFF)); w.Write((byte)(v & 0xFF)); }
        private void WriteBigEndian(BinaryWriter w, ushort v) { w.Write((byte)((v >> 8) & 0xFF)); w.Write((byte)(v & 0xFF)); }
        private void WriteUtf8(BinaryWriter w, byte id, string s) { w.Write(id); byte[] b = SysEncoding.UTF8.GetBytes(s); WriteBigEndian(w, (ushort)b.Length); w.Write(b); }
        private void WriteUtf8Val(BinaryWriter w, string s) { byte[] b = SysEncoding.UTF8.GetBytes(s); WriteBigEndian(w, (ushort)b.Length); w.Write(b); }

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

    [Mutator("MqttPublishDeleteProperties")]
    [CMutator("delete_publish_properties")]
    [Description("Deletes MQTT Publish Properties")]
    public class MqttPublishDeleteProperties : LLMMutator
    {
        public MqttPublishDeleteProperties(DataElement obj) : base(obj) { }
        public new static bool supportedDataElement(DataElement obj) { return obj is Blob && obj.Name == "properties" && obj.IsIn("publish"); }
        public override int count => 1;
        public override uint mutation { get; set; }
        public override void sequentialMutation(DataElement obj) { obj.parent.Remove(obj); }
        public override void randomMutation(DataElement obj) { obj.parent.Remove(obj); }
    }

    [Mutator("MqttPublishRepeatProperties")]
    [CMutator("repeat_publish_properties")]
    [Description("Repeats MQTT Publish User Property")]
    public class MqttPublishRepeatProperties : LLMMutator
    {
        public MqttPublishRepeatProperties(DataElement obj) : base(obj) { }
        public new static bool supportedDataElement(DataElement obj) { return obj is Blob && obj.Name == "properties" && obj.IsIn("publish"); }
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
                    // User Prop
                    int start = pos; pos++;
                    if (pos + 2 > original.Length) break;
                    int klen = (original[pos] << 8) | original[pos + 1]; pos += 2 + klen;
                    if (pos + 2 > original.Length) break;
                    int vlen = (original[pos] << 8) | original[pos + 1]; pos += 2 + vlen;
                    if (pos <= original.Length)
                    {
                        using (var ms = new MemoryStream())
                        {
                            ms.Write(original, 0, original.Length);
                            ms.Write(original, start, pos - start);
                            obj.MutatedValue = new Variant(ms.ToArray());
                        }
                    }
                    break;
                }
                // Simple skip logic
                else if (id == 0x01) { if (pos + 2 > original.Length) break; pos += 2; }
                else if (id == 0x02) { if (pos + 5 > original.Length) break; pos += 5; }
                else if (id == 0x23) { if (pos + 3 > original.Length) break; pos += 3; }
                else if (id == 0x03 || id == 0x08 || id == 0x09 || id == 0x15 || id == 0x16)
                {
                    if (pos + 3 > original.Length) break;
                    int l = (original[pos + 1] << 8) | original[pos + 2];
                    if (pos + 3 + l > original.Length) break;
                    pos += 3 + l;
                }
                else break; // Unknown
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

    // Payload Mutators
    [Mutator("MqttPublishMutatePayload")]
    [CMutator("mutate_publish_payload")]
    [Description("Mutates MQTT Publish Payload")]
    public class MqttPublishMutatePayload : LLMMutator
    {
        public MqttPublishMutatePayload(DataElement obj) : base(obj) { }
        public new static bool supportedDataElement(DataElement obj) { return obj is Blob && obj.Name == "payload" && obj.IsIn("publish"); }
        public override int count => 10;
        public override uint mutation { get; set; }
        public override void sequentialMutation(DataElement obj) { PerformMutation(obj, (int)mutation); obj.mutationFlags = MutateOverride.Default; }
        public override void randomMutation(DataElement obj)
        {
            int[] weights = { 40, 40, 0, 0, 0, 0, 0, 0, 0, 0 };
            PerformMutation(obj, PickWeighted(weights));
            obj.mutationFlags = MutateOverride.Default;
        }

        private void PerformMutation(DataElement obj, int strategy)
        {
            byte[] d = null;
            switch (strategy)
            {
                case 0: d = new byte[0]; break;
                case 1: d = new byte[65535]; for (int i = 0; i < d.Length; i++) d[i] = (byte)'A'; break;
                case 2: d = new byte[Next(65535)]; NextBytes(d); break;
                case 3: d = SysEncoding.UTF8.GetBytes("msg_" + Next(1000)); break;
                case 4: d = new byte[16]; for (int i = 0; i < 16; i++) d[i] = 0xFF; break;
                case 5: d = SysEncoding.UTF8.GetBytes("{\"key\":\"val" + Next(100) + "\"}"); break;
                case 6: d = new byte[70000]; break;
                case 7: d = new byte[] { 0xC0, 0x00 }; break;
                case 8: d = SysEncoding.UTF8.GetBytes("topic/name"); break;
                case 9:
                    byte[] orig = GetOriginalBytes(obj);
                    if (orig != null && orig.Length > 0 && orig.Length * 2 < 65535)
                    {
                        d = new byte[orig.Length * 2]; SysArray.Copy(orig, 0, d, 0, orig.Length); SysArray.Copy(orig, 0, d, orig.Length, orig.Length);
                    }
                    else d = new byte[10];
                    break;
            }
            if (d == null) d = new byte[0];
            obj.MutatedValue = new Variant(d);
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

    [Mutator("MqttPublishAddPayload")]
    [CMutator("add_publish_payload")]
    [Description("Adds MQTT Publish Payload")]
    public class MqttPublishAddPayload : LLMMutator
    {
        public MqttPublishAddPayload(DataElement obj) : base(obj) { }
        public new static bool supportedDataElement(DataElement obj) { return obj is Blob && obj.Name == "payload" && obj.IsIn("publish"); }
        public override int count => 1;
        public override uint mutation { get; set; }
        public override void sequentialMutation(DataElement obj)
        {
            byte[] b = GetOriginalBytes(obj);
            if (b != null && b.Length == 0) obj.MutatedValue = new Variant(SysEncoding.UTF8.GetBytes("hello"));
            obj.mutationFlags = MutateOverride.Default;
        }
        public override void randomMutation(DataElement obj) { sequentialMutation(obj); }
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

    [Mutator("MqttPublishDeletePayload")]
    [CMutator("delete_publish_payload")]
    [Description("Deletes MQTT Publish Payload")]
    public class MqttPublishDeletePayload : LLMMutator
    {
        public MqttPublishDeletePayload(DataElement obj) : base(obj) { }
        public new static bool supportedDataElement(DataElement obj) { return obj is Blob && obj.Name == "payload" && obj.IsIn("publish"); }
        public override int count => 1;
        public override uint mutation { get; set; }
        public override void sequentialMutation(DataElement obj) { obj.parent.Remove(obj); }
        public override void randomMutation(DataElement obj) { obj.parent.Remove(obj); }
    }

    // Flags Mutators
    [Mutator("MqttPublishMutateQoS")]
    [CMutator("mutate_publish_qos")]
    [Description("Mutates MQTT Publish QoS")]
    public class MqttPublishMutateQoS : LLMMutator
    {
        public MqttPublishMutateQoS(DataElement obj) : base(obj) { }
        public new static bool supportedDataElement(DataElement obj) { return obj is Number && obj.Name.StartsWith("qos") && obj.lengthAsBits == 2; }
        public override int count => 10;
        public override uint mutation { get; set; }

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj, (int)mutation); obj.mutationFlags = MutateOverride.Default; }
        public override void randomMutation(DataElement obj)
        {
            int[] weights = { 40, 40, 40, 0, 0, 0, 0, 0, 0, 0 };
            PerformMutation(obj, PickWeighted(weights));
            obj.mutationFlags = MutateOverride.Default;
        }

        private void PerformMutation(DataElement obj, int strategy)
        {
            uint val = 0;
            uint current = (uint)((Number)obj).InternalValue;
            switch (strategy)
            {
                case 0: val = 0; break;
                case 1: val = 1; break;
                case 2: val = 2; break;
                case 3: val = 3; break; // Illegal
                case 4: val = 255; break; // Extreme illegal
                case 5: val = (uint)Next(256); break;
                case 6: val = (current + 1) % 4; break;
                case 7: val = 0xFF & ~current; break;
                case 8: val = (Next(10) == 0) ? 4u : 2u; break;
                case 9: val = 0xAA; break;
            }
            obj.MutatedValue = new Variant(val & 0x3);
        }
    }

    [Mutator("MqttPublishMutateDup")]
    [CMutator("mutate_publish_dup")]
    [Description("Mutates MQTT Publish Dup")]
    public class MqttPublishMutateDup : LLMMutator
    {
        public MqttPublishMutateDup(DataElement obj) : base(obj) { }
        public new static bool supportedDataElement(DataElement obj) { return obj is Number && obj.Name == "dup"; }
        public override int count => 10;
        public override uint mutation { get; set; }

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj, (int)mutation); obj.mutationFlags = MutateOverride.Default; }
        public override void randomMutation(DataElement obj)
        {
            int[] weights = { 40, 40, 40, 0, 0, 0, 0, 0, 0, 0 };
            PerformMutation(obj, PickWeighted(weights));
            obj.mutationFlags = MutateOverride.Default;
        }

        private void PerformMutation(DataElement obj, int strategy)
        {
            uint val = 0;
            uint current = (uint)((Number)obj).InternalValue;
            switch (strategy)
            {
                case 0: val = 0; break;
                case 1: val = 1; break;
                case 2: val = (current == 0) ? 1u : 0u; break;
                case 3: val = 2; break;
                case 4: val = 255; break;
                case 5: val = (uint)Next(256); break;
                case 6: val = current ^ 1; break;
                case 7: val = 0xAA; break;
                case 8: val = (uint)((Next(2) == 0) ? 0 : 1); break;
                case 9: val = (uint)(current + Next(3)); break;
            }
            obj.MutatedValue = new Variant(val & 1);
        }
    }

    [Mutator("MqttPublishMutateRetain")]
    [CMutator("mutate_publish_retain")]
    [Description("Mutates MQTT Publish Retain")]
    public class MqttPublishMutateRetain : LLMMutator
    {
        public MqttPublishMutateRetain(DataElement obj) : base(obj) { }
        public new static bool supportedDataElement(DataElement obj) { return obj is Number && obj.Name == "retain"; }
        public override int count => 10;
        public override uint mutation { get; set; }

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj, (int)mutation); obj.mutationFlags = MutateOverride.Default; }
        public override void randomMutation(DataElement obj)
        {
            int[] weights = { 40, 40, 0, 0, 0, 0, 0, 0, 0, 0 };
            PerformMutation(obj, PickWeighted(weights));
            obj.mutationFlags = MutateOverride.Default;
        }

        private void PerformMutation(DataElement obj, int strategy)
        {
            uint val = 0;
            uint current = (uint)((Number)obj).InternalValue;
            switch (strategy)
            {
                case 0: val = 0; break;
                case 1: val = 1; break;
                case 2: val = current ^ 1; break;
                case 3: val = 2; break;
                case 4: val = 255; break;
                case 5: val = (uint)Next(256); break;
                case 6: val = 0xFF; break;
                case 7: val = 1; break; // Logic in C uses qos check, here we assume 1
                case 8: val = current + 1; break;
                case 9: val = (uint)(Next(2) * 3); break;
            }
            obj.MutatedValue = new Variant(val & 1);
        }
    }
}
