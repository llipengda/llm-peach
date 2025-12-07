using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.IO;
using Peach.Core;
using Peach.Core.Dom;
using Peach.Pro.Core.Mutators;
using Peach.Core.IO; // Added for BitwiseStream

using SysRandom = System.Random;
using SysEncoding = System.Text.Encoding;

using static Peach.Pro.Core.Mutators.MQTT.MqttUtils;

namespace Peach.Pro.Core.Mutators.MQTT
{
    [Mutator("MqttAuthMutateReasonCode")]
    [CMutator("mutate_auth_reason_code")]
    [Description("Mutates MQTT Auth Reason Code")]
    public class MqttAuthMutateReasonCode : Mutator
    {
        public MqttAuthMutateReasonCode(DataElement obj) : base(obj) { }
        public new static bool supportedDataElement(DataElement obj) { return obj is Number && obj.Name == "reason_code" && obj.IsIn("auth"); }
        public override int count => 10;
        public override uint mutation { get; set; }

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj, (int)mutation); obj.mutationFlags = MutateOverride.Default; }
        public override void randomMutation(DataElement obj)
        {
            int[] weights = { 50, 50, 50, 0, 0, 0, 0, 0, 0, 0 };
            PerformMutation(obj, PickWeighted(weights));
            obj.mutationFlags = MutateOverride.Default;
        }

        private void PerformMutation(DataElement obj, int strategy)
        {
            uint val = 0;
            switch (strategy)
            {
                case 0: val = 0x00; break;
                case 1: val = 0x18; break;
                case 2: val = 0x19; break;
                case 3: val = 0xFF; break;
                case 4: val = 0x7F; break;
                case 5: val = 0x80; break;
                case 6: val = (uint)context.Random.Next(256); break;
                case 7: val = 0x01; break;
                case 8: val = 0xFE; break;
                case 9: val = 0x10; break;
            }
            obj.MutatedValue = new Variant(val & 0xFF);
        }
    }

    [Mutator("MqttAuthAddReasonCode")]
    [CMutator("add_auth_reason_code")]
    [Description("Adds MQTT Auth Reason Code")]
    public class MqttAuthAddReasonCode : Mutator
    {
        public MqttAuthAddReasonCode(DataElement obj) : base(obj) { }
        public new static bool supportedDataElement(DataElement obj) { return obj is Number && obj.Name == "reason_code" && obj.IsIn("auth"); }
        public override int count => 1;
        public override uint mutation { get; set; }
        public override void sequentialMutation(DataElement obj) { obj.MutatedValue = new Variant(0x00); obj.mutationFlags = MutateOverride.Default; }
        public override void randomMutation(DataElement obj) { obj.MutatedValue = new Variant(0x00); obj.mutationFlags = MutateOverride.Default; }
    }

    [Mutator("MqttAuthDeleteReasonCode")]
    [CMutator("delete_auth_reason_code")]
    [Description("Deletes MQTT Auth Reason Code")]
    public class MqttAuthDeleteReasonCode : Mutator
    {
        public MqttAuthDeleteReasonCode(DataElement obj) : base(obj) { }
        public new static bool supportedDataElement(DataElement obj) { return obj is Number && obj.Name == "reason_code" && obj.IsIn("auth"); }
        public override int count => 1;
        public override uint mutation { get; set; }
        public override void sequentialMutation(DataElement obj) { obj.parent.Remove(obj); }
        public override void randomMutation(DataElement obj) { obj.parent.Remove(obj); }
    }

    [Mutator("MqttAuthMutateProperties")]
    [CMutator("mutate_auth_properties")]
    [Description("Mutates MQTT Auth Properties")]
    public class MqttAuthMutateProperties : Mutator
    {
        public MqttAuthMutateProperties(DataElement obj) : base(obj) { }
        public new static bool supportedDataElement(DataElement obj) { return obj is Blob && obj.Name == "properties" && obj.IsIn("auth"); }
        public override int count => 6;
        public override uint mutation { get; set; }
        public override void sequentialMutation(DataElement obj) { PerformMutation(obj, (int)mutation); obj.mutationFlags = MutateOverride.Default; }
        public override void randomMutation(DataElement obj) { PerformMutation(obj, context.Random.Next(count)); obj.mutationFlags = MutateOverride.Default; }

        private void PerformMutation(DataElement obj, int strategy)
        {
            obj.MutatedValue = new Variant(GenerateProperties(strategy));
        }

        private byte[] GenerateProperties(int strategy)
        {
            using (var ms = new MemoryStream())
            using (var bw = new BinaryWriter(ms))
            {
                bool used_method = false, used_data = false, used_reason = false;
                switch (strategy)
                {
                    case 0: break;
                    case 1: // Method
                        if (!used_method) { WriteMethod(bw); used_method = true; }
                        break;
                    case 2: // Method + Data
                        WriteMethod(bw); used_method = true;
                        WriteData(bw); used_data = true;
                        break;
                    case 3: // Reason String
                        if (!used_reason) { WriteReason(bw); used_reason = true; }
                        break;
                    case 4: // User Prop 1..3
                        int count = 1 + context.Random.Next(3); for (int i = 0; i < count; i++) WriteUserProp(bw);
                        break;
                    case 5: // Mixed
                        if (!used_method) { WriteMethod(bw); used_method = true; }
                        if (used_method && context.Random.Next(2) != 0 && !used_data) { WriteData(bw); used_data = true; }
                        int up = context.Random.Next(3);
                        for (int i = 0; i < up; i++) WriteUserProp(bw);
                        if (!used_reason && context.Random.Next(2) != 0) { WriteReason(bw); used_reason = true; }
                        break;
                }
                return ms.ToArray();
            }
        }
        private void WriteMethod(BinaryWriter bw)
        {
            bw.Write((byte)0x15);
            WriteUtf8(bw, (context.Random.Next(2) == 0) ? "PLAIN" : "SCRAM-SHA-256");
        }
        private void WriteData(BinaryWriter bw)
        {
            bw.Write((byte)0x16);
            int len = 8 + context.Random.Next(9);
            byte[] d = new byte[len];
            // Fix for Random.NextBytes not found in Peach.Core.Random
            for (int i = 0; i < d.Length; i++) d[i] = (byte)context.Random.Next(256);
            bw.Write((byte)((len >> 8) & 0xFF)); bw.Write((byte)(len & 0xFF)); bw.Write(d);
        }
        private void WriteReason(BinaryWriter bw) { bw.Write((byte)0x1F); string[] rs = { "ok", "continue", "reauth" }; WriteUtf8(bw, rs[context.Random.Next(rs.Length)]); }
        private void WriteUserProp(BinaryWriter bw)
        {
            string[] k = { "source", "priority", "note", "device" };
            string[] v = { "client", "high", "ok", "edge" };
            bw.Write((byte)0x26);
            WriteUtf8(bw, k[context.Random.Next(4)]);
            WriteUtf8(bw, v[context.Random.Next(4)]);
        }
        private void WriteUtf8(BinaryWriter writer, string str) { byte[] b = SysEncoding.UTF8.GetBytes(str); writer.Write((byte)((b.Length >> 8) & 0xFF)); writer.Write((byte)(b.Length & 0xFF)); writer.Write(b); }
    }

    [Mutator("MqttAuthAddProperties")]
    [CMutator("add_auth_properties")]
    [Description("Adds MQTT Auth Properties")]
    public class MqttAuthAddProperties : Mutator
    {
        public MqttAuthAddProperties(DataElement obj) : base(obj) { }
        public new static bool supportedDataElement(DataElement obj) { return obj is Blob && obj.Name == "properties" && obj.IsIn("auth"); }
        public override int count => 1;
        public override uint mutation { get; set; }
        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); obj.mutationFlags = MutateOverride.Default; }
        public override void randomMutation(DataElement obj) { PerformMutation(obj); obj.mutationFlags = MutateOverride.Default; }
        private void PerformMutation(DataElement obj)
        {
            byte[] b = obj.Bytes();
            if (b != null && b.Length > 0) return;

            using (var ms = new MemoryStream())
            using (var bw = new BinaryWriter(ms))
            {
                bw.Write((byte)0x15);
                byte[] buf = SysEncoding.UTF8.GetBytes("PLAIN");
                bw.Write((byte)((buf.Length >> 8) & 0xFF));
                bw.Write((byte)(buf.Length & 0xFF));
                bw.Write(buf);
                obj.MutatedValue = new Variant(ms.ToArray());
            }
        }

    }

    [Mutator("MqttAuthRepeatProperties")]
    [CMutator("repeat_auth_properties")]
    [Description("Repeats MQTT Auth User Property")]
    public class MqttAuthRepeatProperties : Mutator
    {
        public MqttAuthRepeatProperties(DataElement obj) : base(obj) { }
        public new static bool supportedDataElement(DataElement obj) { return obj is Blob && obj.Name == "properties" && obj.IsIn("auth"); }
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
                byte id = original[pos];
                if (id == 0x26)
                {
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
                else if (id == 0x15 || id == 0x1F || id == 0x16)
                {
                    pos++;
                    if (pos + 2 > original.Length) break;
                    int len = (original[pos] << 8) | original[pos + 1];
                    pos += 2 + len;
                }
                else break;
            }
        }
    }
}
