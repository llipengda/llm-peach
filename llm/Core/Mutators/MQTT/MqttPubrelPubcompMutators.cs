using System;
using System.ComponentModel;
using System.IO;
using System.Text;
using Peach.Core;
using Peach.Core.Dom;
using Peach.Pro.Core.Mutators;
using Peach.Core.IO;

using static Peach.LLM.Core.Mutators.MQTT.MqttUtils;

using Encoding = System.Text.Encoding;

namespace Peach.LLM.Core.Mutators.MQTT
{
    // Shared helpers for PUBREL/PUBCOMP variable header fields.
    public abstract class PubrespPacketIdMutatorBase : LLMMutator
    {
        protected PubrespPacketIdMutatorBase(DataElement obj) : base(obj) { }

        protected void MutatePacketIdentifier(DataElement obj, int strategy)
        {
            uint current = (uint)((Number)obj).InternalValue;
            uint val = current;
            switch (strategy)
            {
                case 0: // Canonical minimal non-zero
                    val = 1;
                    break;
                case 1: // Zero (illegal boundary)
                    val = 0;
                    break;
                case 2: // Max boundary
                    val = 0xFFFF;
                    break;
                case 3: // Random valid range
                    val = 1 + (uint)Next(0xFFFF);
                    break;
                case 4: // Swap bytes (endianness variant)
                    val = ((current & 0xFF) << 8) | ((current >> 8) & 0xFF);
                    break;
                case 5: // Bit flip
                    val = current ^ (1u << Next(16));
                    break;
                case 6: // Prefix pattern on high byte
                    val = (uint)((0xA5 << 8) | (Next(0x100)));
                    break;
                case 7: // Small drift around current
                    val = (current + (uint)(1 + Next(7))) & 0xFFFF;
                    break;
                case 8: // Overflow shape
                    val = 0x10000u + (uint)Next(0x10000);
                    break;
                case 9: // Alignment to even/odd
                    val = (Next(2) == 0) ? (current & ~1u) : (current | 1u);
                    break;
            }
            obj.MutatedValue = new Variant(val & 0xFFFF);
        }
    }

    public abstract class PubrespReasonCodeMutatorBase : LLMMutator
    {
        protected PubrespReasonCodeMutatorBase(DataElement obj) : base(obj) { }

        protected void MutateReasonCode(DataElement obj, int strategy)
        {
            uint current = (uint)((Number)obj).InternalValue;
            uint val = current;
            switch (strategy)
            {
                case 0: // Canonical Success
                    val = 0x00;
                    break;
                case 1: // Allowed alternative per spec
                    val = 0x92; // Packet Identifier not found
                    break;
                case 2: // Generic error class
                    val = 0x80; // Unspecified error
                    break;
                case 3: // Boundary high
                    val = 0xFF;
                    break;
                case 4: // Random byte
                    val = (uint)Next(256);
                    break;
                case 5: // Flip low nibble
                    val = current ^ 0x0Fu;
                    break;
                case 6: // Mid-range reserved
                    val = 0x7F;
                    break;
                case 7: // Wrong-but-related reason
                    val = 0x10; // No matching subscribers (from PUBACK set)
                    break;
                case 8: // Toggle high bit
                    val = current ^ 0x80u;
                    break;
                case 9: // Randomized allowed mix
                    val = (Next(2) == 0) ? 0x00u : 0x92u;
                    break;
            }
            obj.MutatedValue = new Variant(val & 0xFF);
        }
    }

    public abstract class PubrespPropertiesMutatorBase : LLMMutator
    {
        protected PubrespPropertiesMutatorBase(DataElement obj) : base(obj) { }

        protected void MutateProperties(DataElement obj, int strategy)
        {
            obj.MutatedValue = new Variant(GenerateProperties(strategy));
        }

        protected byte[] GenerateProperties(int strategy)
        {
            using (var ms = new MemoryStream())
            using (var bw = new BinaryWriter(ms))
            {
                switch (strategy)
                {
                    case 0: // Empty (canonical)
                        return new byte[0];
                    case 1: // Reason String short
                        WriteReasonString(bw, "success");
                        break;
                    case 2: // Long boundary reason
                        WriteReasonString(bw, RandomString(64, 192));
                        break;
                    case 3: // Single user property
                        WriteUserProperty(bw, "source", "pubresp");
                        break;
                    case 4: // Multiple user properties, varied length
                        {
                            int count = 1 + Next(4);
                            for (int i = 0; i < count; ++i)
                                WriteUserProperty(bw, RandomString(3, 10), RandomString(3, 20));
                        }
                        break;
                    case 5: // Invalid property id
                        bw.Write((byte)0xFF);
                        {
                            int len = 1 + Next(8);
                            for (int i = 0; i < len; ++i) bw.Write((byte)Next(256));
                        }
                        break;
                    case 6: // Padding/alignment variant
                        WriteReasonString(bw, "");
                        {
                            int pad = 1 + Next(6);
                            for (int i = 0; i < pad; ++i) bw.Write((byte)0x00);
                        }
                        break;
                    case 7: // Mixed valid set + suffix noise
                        if (Next(2) != 0) WriteReasonString(bw, RandomString(5, 24));
                        {
                            int up = Next(5);
                            for (int i = 0; i < up; ++i)
                                WriteUserProperty(bw, "k" + i, RandomString(1, 16));
                        }
                        if (Next(2) == 0)
                        {
                            int tail = 1 + Next(6);
                            for (int i = 0; i < tail; ++i) bw.Write((byte)Next(256));
                        }
                        break;
                }
                return ms.ToArray();
            }
        }

        protected void WriteReasonString(BinaryWriter bw, string reason)
        {
            bw.Write((byte)0x1F);
            var data = Encoding.UTF8.GetBytes(reason);
            bw.Write((byte)((data.Length >> 8) & 0xFF));
            bw.Write((byte)(data.Length & 0xFF));
            if (data.Length > 0) bw.Write(data);
        }

        protected void WriteUserProperty(BinaryWriter bw, string key, string value)
        {
            bw.Write((byte)0x26);
            var k = Encoding.UTF8.GetBytes(key);
            var v = Encoding.UTF8.GetBytes(value);
            bw.Write((byte)((k.Length >> 8) & 0xFF)); bw.Write((byte)(k.Length & 0xFF)); if (k.Length > 0) bw.Write(k);
            bw.Write((byte)((v.Length >> 8) & 0xFF)); bw.Write((byte)(v.Length & 0xFF)); if (v.Length > 0) bw.Write(v);
        }

        protected string RandomString(int min, int max)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.";
            int len = min + Next(max - min + 1);
            var buffer = new char[len];
            for (int i = 0; i < len; ++i) buffer[i] = chars[Next(chars.Length)];
            return new string(buffer);
        }
    }

    // Packet Identifier mutators
    [Mutator("MqttPubrelMutatePacketIdentifier")]
    [CMutator("mutate_pubrel_packet_identifier")]
    [Description("Mutates MQTT PUBREL Packet Identifier")]
    public class MqttPubrelMutatePacketIdentifier : PubrespPacketIdMutatorBase
    {
        public MqttPubrelMutatePacketIdentifier(DataElement obj) : base(obj) { }
        public new static bool supportedDataElement(DataElement obj)
        {
            return obj is Number && obj.Name == "packet_identifier" && obj.IsIn("pubrel");
        }
        public override int count => 10;
        public override uint mutation { get; set; }
        public override void sequentialMutation(DataElement obj) { MutatePacketIdentifier(obj, (int)mutation); obj.mutationFlags = MutateOverride.Default; }
        public override void randomMutation(DataElement obj)
        {
            int[] weights = { 30, 20, 20, 40, 20, 30, 10, 20, 10, 20 };
            MutatePacketIdentifier(obj, PickWeighted(weights));
            obj.mutationFlags = MutateOverride.Default;
        }
    }

    [Mutator("MqttPubcompMutatePacketIdentifier")]
    [CMutator("mutate_pubcomp_packet_identifier")]
    [Description("Mutates MQTT PUBCOMP Packet Identifier")]
    public class MqttPubcompMutatePacketIdentifier : PubrespPacketIdMutatorBase
    {
        public MqttPubcompMutatePacketIdentifier(DataElement obj) : base(obj) { }
        public new static bool supportedDataElement(DataElement obj)
        {
            return obj is Number && obj.Name == "packet_identifier" && obj.IsIn("pubcomp");
        }
        public override int count => 10;
        public override uint mutation { get; set; }
        public override void sequentialMutation(DataElement obj) { MutatePacketIdentifier(obj, (int)mutation); obj.mutationFlags = MutateOverride.Default; }
        public override void randomMutation(DataElement obj)
        {
            int[] weights = { 30, 20, 20, 40, 20, 30, 10, 20, 10, 20 };
            MutatePacketIdentifier(obj, PickWeighted(weights));
            obj.mutationFlags = MutateOverride.Default;
        }
    }

    // Reason Code mutators (optional field)
    [Mutator("MqttPubrelMutateReasonCode")]
    [CMutator("mutate_pubrel_reason_code")]
    [Description("Mutates MQTT PUBREL Reason Code")]
    public class MqttPubrelMutateReasonCode : PubrespReasonCodeMutatorBase
    {
        public MqttPubrelMutateReasonCode(DataElement obj) : base(obj) { }
        public new static bool supportedDataElement(DataElement obj)
        {
            return obj is Number && obj.Name == "reason_code" && obj.IsIn("pubrel");
        }
        public override int count => 10;
        public override uint mutation { get; set; }
        public override void sequentialMutation(DataElement obj) { MutateReasonCode(obj, (int)mutation); obj.mutationFlags = MutateOverride.Default; }
        public override void randomMutation(DataElement obj)
        {
            int[] weights = { 50, 50, 10, 10, 10, 10, 0, 0, 10, 50 };
            MutateReasonCode(obj, PickWeighted(weights));
            obj.mutationFlags = MutateOverride.Default;
        }
    }

    [Mutator("MqttPubrelAddReasonCode")]
    [CMutator("add_pubrel_reason_code")]
    [Description("Adds MQTT PUBREL Reason Code")]
    public class MqttPubrelAddReasonCode : LLMMutator
    {
        public MqttPubrelAddReasonCode(DataElement obj) : base(obj) { }
        public new static bool supportedDataElement(DataElement obj)
        {
            return obj is Number && obj.Name == "reason_code" && obj.IsIn("pubrel");
        }
        public override int count => 1;
        public override uint mutation { get; set; }
        public override void sequentialMutation(DataElement obj) { obj.MutatedValue = new Variant(0x00); obj.mutationFlags = MutateOverride.Default; }
        public override void randomMutation(DataElement obj) { sequentialMutation(obj); }
    }

    [Mutator("MqttPubrelDeleteReasonCode")]
    [CMutator("delete_pubrel_reason_code")]
    [Description("Deletes MQTT PUBREL Reason Code")]
    public class MqttPubrelDeleteReasonCode : LLMMutator
    {
        public MqttPubrelDeleteReasonCode(DataElement obj) : base(obj) { }
        public new static bool supportedDataElement(DataElement obj)
        {
            return obj is Number && obj.Name == "reason_code" && obj.IsIn("pubrel");
        }
        public override int count => 1;
        public override uint mutation { get; set; }
        public override void sequentialMutation(DataElement obj) { obj.parent.Remove(obj); }
        public override void randomMutation(DataElement obj) { obj.parent.Remove(obj); }
    }

    [Mutator("MqttPubcompMutateReasonCode")]
    [CMutator("mutate_pubcomp_reason_code")]
    [Description("Mutates MQTT PUBCOMP Reason Code")]
    public class MqttPubcompMutateReasonCode : PubrespReasonCodeMutatorBase
    {
        public MqttPubcompMutateReasonCode(DataElement obj) : base(obj) { }
        public new static bool supportedDataElement(DataElement obj)
        {
            return obj is Number && obj.Name == "reason_code" && obj.IsIn("pubcomp");
        }
        public override int count => 10;
        public override uint mutation { get; set; }
        public override void sequentialMutation(DataElement obj) { MutateReasonCode(obj, (int)mutation); obj.mutationFlags = MutateOverride.Default; }
        public override void randomMutation(DataElement obj)
        {
            int[] weights = { 50, 50, 10, 10, 10, 10, 0, 0, 10, 50 };
            MutateReasonCode(obj, PickWeighted(weights));
            obj.mutationFlags = MutateOverride.Default;
        }
    }

    [Mutator("MqttPubcompAddReasonCode")]
    [CMutator("add_pubcomp_reason_code")]
    [Description("Adds MQTT PUBCOMP Reason Code")]
    public class MqttPubcompAddReasonCode : LLMMutator
    {
        public MqttPubcompAddReasonCode(DataElement obj) : base(obj) { }
        public new static bool supportedDataElement(DataElement obj)
        {
            return obj is Number && obj.Name == "reason_code" && obj.IsIn("pubcomp");
        }
        public override int count => 1;
        public override uint mutation { get; set; }
        public override void sequentialMutation(DataElement obj) { obj.MutatedValue = new Variant(0x00); obj.mutationFlags = MutateOverride.Default; }
        public override void randomMutation(DataElement obj) { sequentialMutation(obj); }
    }

    [Mutator("MqttPubcompDeleteReasonCode")]
    [CMutator("delete_pubcomp_reason_code")]
    [Description("Deletes MQTT PUBCOMP Reason Code")]
    public class MqttPubcompDeleteReasonCode : LLMMutator
    {
        public MqttPubcompDeleteReasonCode(DataElement obj) : base(obj) { }
        public new static bool supportedDataElement(DataElement obj)
        {
            return obj is Number && obj.Name == "reason_code" && obj.IsIn("pubcomp");
        }
        public override int count => 1;
        public override uint mutation { get; set; }
        public override void sequentialMutation(DataElement obj) { obj.parent.Remove(obj); }
        public override void randomMutation(DataElement obj) { obj.parent.Remove(obj); }
    }

    // Properties mutators (optional field)
    [Mutator("MqttPubrelMutateProperties")]
    [CMutator("mutate_pubrel_properties")]
    [Description("Mutates MQTT PUBREL Properties")]
    public class MqttPubrelMutateProperties : PubrespPropertiesMutatorBase
    {
        public MqttPubrelMutateProperties(DataElement obj) : base(obj) { }
        public new static bool supportedDataElement(DataElement obj)
        {
            return obj is Blob && obj.Name == "properties" && obj.IsIn("pubrel");
        }
        public override int count => 8;
        public override uint mutation { get; set; }
        public override void sequentialMutation(DataElement obj) { MutateProperties(obj, (int)mutation); obj.mutationFlags = MutateOverride.Default; }
        public override void randomMutation(DataElement obj)
        {
            int[] weights = { 30, 20, 10, 20, 20, 10, 10, 20 };
            MutateProperties(obj, PickWeighted(weights));
            obj.mutationFlags = MutateOverride.Default;
        }
    }

    [Mutator("MqttPubrelAddProperties")]
    [CMutator("add_pubrel_properties")]
    [Description("Adds MQTT PUBREL Properties")]
    public class MqttPubrelAddProperties : PubrespPropertiesMutatorBase
    {
        public MqttPubrelAddProperties(DataElement obj) : base(obj) { }
        public new static bool supportedDataElement(DataElement obj)
        {
            return obj is Blob && obj.Name == "properties" && obj.IsIn("pubrel");
        }
        public override int count => 1;
        public override uint mutation { get; set; }
        public override void sequentialMutation(DataElement obj)
        {
            byte[] b = obj.Bytes();
            if (b != null && b.Length > 0) return;
            obj.MutatedValue = new Variant(GenerateProperties(1));
            obj.mutationFlags = MutateOverride.Default;
        }
        public override void randomMutation(DataElement obj) { sequentialMutation(obj); }
    }

    [Mutator("MqttPubrelDeleteProperties")]
    [CMutator("delete_pubrel_properties")]
    [Description("Deletes MQTT PUBREL Properties")]
    public class MqttPubrelDeleteProperties : LLMMutator
    {
        public MqttPubrelDeleteProperties(DataElement obj) : base(obj) { }
        public new static bool supportedDataElement(DataElement obj)
        {
            return obj is Blob && obj.Name == "properties" && obj.IsIn("pubrel");
        }
        public override int count => 1;
        public override uint mutation { get; set; }
        public override void sequentialMutation(DataElement obj) { obj.parent.Remove(obj); }
        public override void randomMutation(DataElement obj) { obj.parent.Remove(obj); }
    }

    [Mutator("MqttPubrelRepeatProperties")]
    [CMutator("repeat_pubrel_properties")]
    [Description("Repeats MQTT PUBREL First Property Block")]
    public class MqttPubrelRepeatProperties : LLMMutator
    {
        public MqttPubrelRepeatProperties(DataElement obj) : base(obj) { }
        public new static bool supportedDataElement(DataElement obj)
        {
            return obj is Blob && obj.Name == "properties" && obj.IsIn("pubrel");
        }
        public override int count => 1;
        public override uint mutation { get; set; }
        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); obj.mutationFlags = MutateOverride.Default; }
        public override void randomMutation(DataElement obj) { PerformMutation(obj); obj.mutationFlags = MutateOverride.Default; }
        protected override void PerformMutation(DataElement obj)
        {
            byte[] original = obj.Bytes();
            if (original == null || original.Length == 0) return;

            int pos = 0;
            while (pos < original.Length)
            {
                byte id = original[pos];
                if (id == 0x1F)
                {
                    int start = pos;
                    pos++;
                    if (pos + 2 > original.Length) break;
                    int len = (original[pos] << 8) | original[pos + 1];
                    pos += 2 + len;
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
                else if (id == 0x26)
                {
                    int start = pos;
                    pos++;
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
                else break;
            }
        }
    }

    [Mutator("MqttPubcompMutateProperties")]
    [CMutator("mutate_pubcomp_properties")]
    [Description("Mutates MQTT PUBCOMP Properties")]
    public class MqttPubcompMutateProperties : PubrespPropertiesMutatorBase
    {
        public MqttPubcompMutateProperties(DataElement obj) : base(obj) { }
        public new static bool supportedDataElement(DataElement obj)
        {
            return obj is Blob && obj.Name == "properties" && obj.IsIn("pubcomp");
        }
        public override int count => 8;
        public override uint mutation { get; set; }
        public override void sequentialMutation(DataElement obj) { MutateProperties(obj, (int)mutation); obj.mutationFlags = MutateOverride.Default; }
        public override void randomMutation(DataElement obj)
        {
            int[] weights = { 30, 20, 10, 20, 20, 10, 10, 20 };
            MutateProperties(obj, PickWeighted(weights));
            obj.mutationFlags = MutateOverride.Default;
        }
    }

    [Mutator("MqttPubcompAddProperties")]
    [CMutator("add_pubcomp_properties")]
    [Description("Adds MQTT PUBCOMP Properties")]
    public class MqttPubcompAddProperties : PubrespPropertiesMutatorBase
    {
        public MqttPubcompAddProperties(DataElement obj) : base(obj) { }
        public new static bool supportedDataElement(DataElement obj)
        {
            return obj is Blob && obj.Name == "properties" && obj.IsIn("pubcomp");
        }
        public override int count => 1;
        public override uint mutation { get; set; }
        public override void sequentialMutation(DataElement obj)
        {
            byte[] b = obj.Bytes();
            if (b != null && b.Length > 0) return;
            obj.MutatedValue = new Variant(GenerateProperties(1));
            obj.mutationFlags = MutateOverride.Default;
        }
        public override void randomMutation(DataElement obj) { sequentialMutation(obj); }
    }

    [Mutator("MqttPubcompDeleteProperties")]
    [CMutator("delete_pubcomp_properties")]
    [Description("Deletes MQTT PUBCOMP Properties")]
    public class MqttPubcompDeleteProperties : LLMMutator
    {
        public MqttPubcompDeleteProperties(DataElement obj) : base(obj) { }
        public new static bool supportedDataElement(DataElement obj)
        {
            return obj is Blob && obj.Name == "properties" && obj.IsIn("pubcomp");
        }
        public override int count => 1;
        public override uint mutation { get; set; }
        public override void sequentialMutation(DataElement obj) { obj.parent.Remove(obj); }
        public override void randomMutation(DataElement obj) { obj.parent.Remove(obj); }
    }

    [Mutator("MqttPubcompRepeatProperties")]
    [CMutator("repeat_pubcomp_properties")]
    [Description("Repeats MQTT PUBCOMP First Property Block")]
    public class MqttPubcompRepeatProperties : LLMMutator
    {
        public MqttPubcompRepeatProperties(DataElement obj) : base(obj) { }
        public new static bool supportedDataElement(DataElement obj)
        {
            return obj is Blob && obj.Name == "properties" && obj.IsIn("pubcomp");
        }
        public override int count => 1;
        public override uint mutation { get; set; }
        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); obj.mutationFlags = MutateOverride.Default; }
        public override void randomMutation(DataElement obj) { PerformMutation(obj); obj.mutationFlags = MutateOverride.Default; }
        protected override void PerformMutation(DataElement obj)
        {
            byte[] original = obj.Bytes();
            if (original == null || original.Length == 0) return;

            int pos = 0;
            while (pos < original.Length)
            {
                byte id = original[pos];
                if (id == 0x1F)
                {
                    int start = pos;
                    pos++;
                    if (pos + 2 > original.Length) break;
                    int len = (original[pos] << 8) | original[pos + 1];
                    pos += 2 + len;
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
                else if (id == 0x26)
                {
                    int start = pos;
                    pos++;
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
                else break;
            }
        }
    }
}
