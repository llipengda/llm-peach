using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using Peach.Core;
using Peach.Core.Dom;
using Peach.Pro.Core.Mutators;
using Peach.Core.IO;

using SysRandom = System.Random;
using SysEncoding = System.Text.Encoding;
using SysArray = System.Array;

namespace Peach.Pro.Core.Mutators.MQTT
{
    [Mutator("MqttConnectMutateFlags")]
    [Description("Mutates MQTT Connect Flags")]
    public class MqttConnectMutateFlags : Mutator
    {
        public MqttConnectMutateFlags(DataElement obj) : base(obj) { }
        public new static bool supportedDataElement(DataElement obj) { return obj is Number && obj.Name == "connect_flags" && (obj.parent != null && obj.parent.Name.Contains("variable_header")); }
        public override int count => 7;
        public override uint mutation { get; set; }

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj, (int)mutation); obj.mutationFlags = MutateOverride.Default; }
        public override void randomMutation(DataElement obj)
        {
            int[] weights = { 70, 0, 0, 0, 0, 0, 0 };
            PerformMutation(obj, PickWeighted(weights));
            obj.mutationFlags = MutateOverride.Default;
        }

        private void PerformMutation(DataElement obj, int strategy)
        {
            uint original = (uint)((Number)obj).DefaultValue;
            uint mutated = original;
            switch (strategy)
            {
                case 0: // 合法组合：随机合法构造
                    uint clean = (uint)context.Random.Next(2);
                    uint will = (uint)context.Random.Next(2);
                    uint qos = (uint)context.Random.Next(3);
                    uint retain = will != 0 ? (uint)context.Random.Next(2) : 0;
                    uint user = (uint)context.Random.Next(2);
                    uint pass = (uint)context.Random.Next(2);

                    mutated = 0;
                    mutated |= (user << 7);
                    mutated |= (pass << 6);
                    mutated |= (retain << 5);
                    mutated |= ((qos & 0x03) << 3);
                    mutated |= (will << 2);
                    mutated |= (clean << 1);
                    mutated |= 0x00; // reserved bit
                    break;
                case 1: // 设置非法 QoS = 3
                    mutated = (original & ~0x18u) | (3u << 3);
                    break;
                case 2: // 设置 Retain/QoS 但未设置 WillFlag
                    mutated = (1u << 5) | (2u << 3);
                    mutated &= ~0x04u;
                    break;
                case 3: // 设置保留位（非法）
                    mutated = original | 0x01u;
                    break;
                case 4: // bitflip
                    mutated = original ^ (1u << context.Random.Next(8));
                    break;
                case 5: // rotate left
                    mutated = ((original << 1) | (original >> 7)) & 0xFF;
                    break;
                case 6: // rotate right
                    mutated = ((original >> 1) | (original << 7)) & 0xFF;
                    break;
            }
            obj.MutatedValue = new Variant(mutated);
        }

        private int PickWeighted(int[] weights)
        {
            int sum = 0;
            foreach (int w in weights) sum += w;
            if (sum <= 0) return 0;
            int r = context.Random.Next(sum);
            for (int i = 0; i < weights.Length; i++)
            {
                if (r < weights[i]) return i;
                r -= weights[i];
            }
            return weights.Length - 1;
        }
    }

    [Mutator("MqttConnectMutateKeepAlive")]
    [Description("Mutates MQTT Connect Keep Alive")]
    public class MqttConnectMutateKeepAlive : Mutator
    {
        public MqttConnectMutateKeepAlive(DataElement obj) : base(obj) { }
        public new static bool supportedDataElement(DataElement obj) { return obj is Number && obj.Name == "keep_alive"; }
        public override int count => 7;
        public override uint mutation { get; set; }

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj, (int)mutation); obj.mutationFlags = MutateOverride.Default; }
        public override void randomMutation(DataElement obj)
        {
            int[] weights = { 20, 20, 20, 20, 0, 0, 0 };
            PerformMutation(obj, PickWeighted(weights));
            obj.mutationFlags = MutateOverride.Default;
        }

        private void PerformMutation(DataElement obj, int strategy)
        {
            uint orig = (uint)((Number)obj).DefaultValue;
            uint mutated = orig;
            switch (strategy)
            {
                case 0:
                    mutated = 0; // 关闭 keep_alive
                    break;
                case 1:
                    mutated = 60; // 常见的设置（1 分钟）
                    break;
                case 2:
                    mutated = 65535; // 最大合法值
                    break;
                case 3:
                    mutated = (uint)context.Random.Next(10000); // 合法范围内随机值
                    break;
                case 4:
                    mutated = orig + (uint)context.Random.Next(1000); // 正向扰动
                    if (mutated > 65535) mutated = 65535;
                    break;
                case 5:
                    int val = (int)orig - context.Random.Next(1000); // 负向扰动
                    mutated = (uint)val; // cast to uint simulates underflow behavior of C uint16 arithmetic before masking
                    break;
                case 6:
                    mutated = (uint)context.Random.Next(int.MaxValue); // 全随机
                    break;
            }
            obj.MutatedValue = new Variant(mutated & 0xFFFF);
        }

        private int PickWeighted(int[] weights)
        {
            int sum = 0;
            foreach (int w in weights) sum += w;
            if (sum <= 0) return 0;
            int r = context.Random.Next(sum);
            for (int i = 0; i < weights.Length; i++)
            {
                if (r < weights[i]) return i;
                r -= weights[i];
            }
            return weights.Length - 1;
        }
    }

    [Mutator("MqttConnectMutateProperties")]
    [Description("Mutates MQTT Connect Properties")]
    public class MqttConnectMutateProperties : Mutator
    {
        public MqttConnectMutateProperties(DataElement obj) : base(obj) { }
        public new static bool supportedDataElement(DataElement obj) { return obj is Blob && obj.Name == "properties" && obj.parent != null && obj.parent.Name.Contains("variable_header"); }
        public override int count => 1;
        public override uint mutation { get; set; }
        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); obj.mutationFlags = MutateOverride.Default; }
        public override void randomMutation(DataElement obj) { PerformMutation(obj); obj.mutationFlags = MutateOverride.Default; }

        private void PerformMutation(DataElement obj)
        {
            // mutate_connect_properties logic from C
            obj.MutatedValue = new Variant(GenerateRandomProperties());
        }

        private byte[] GenerateRandomProperties()
        {
            using (var ms = new MemoryStream())
            using (var buf = new BinaryWriter(ms))
            {
                bool used_ses = false, used_rcv = false, used_max = false, used_ta = false;
                bool used_rr = false, used_rp = false, used_am = false, used_ad = false;

                int num_props = 1 + context.Random.Next(6);
                for (int n = 0; n < num_props; ++n)
                {
                    int pick = context.Random.Next(9);
                    switch (pick)
                    {
                        case 0: if (!used_ses) { PUT8(buf, 0x11); PUT32(buf, (uint)context.Random.Next(86400)); used_ses = true; } break;
                        case 1: if (!used_rcv) { PUT8(buf, 0x12); PUT16(buf, (ushort)(1 + context.Random.Next(1024))); used_rcv = true; } break;
                        case 2: if (!used_max) { PUT8(buf, 0x13); PUT32(buf, (uint)(512 + context.Random.Next(65536))); used_max = true; } break;
                        case 3: if (!used_ta) { PUT8(buf, 0x22); PUT16(buf, (ushort)(1 + context.Random.Next(100))); used_ta = true; } break; // 0x22 or 0x15? C code define PID_TA_MAX 0x22.
                        case 4: if (!used_rr) { PUT8(buf, 0x17); PUT8(buf, (byte)context.Random.Next(2)); used_rr = true; } break;
                        case 5: if (!used_rp) { PUT8(buf, 0x19); PUT8(buf, (byte)context.Random.Next(2)); used_rp = true; } break;
                        case 6: // User Property
                            PUT8(buf, 0x26); PUT_UTF8(buf, "key", 32); PUT_UTF8(buf, "val", 32);
                            break;
                        case 7: if (!used_am) { PUT8(buf, 0x15); PUT_UTF8(buf, "PLAIN", 64); used_am = true; } break;
                        case 8:
                            if (!used_ad)
                            {
                                byte[] tmp = new byte[16];
                                int L = 4 + context.Random.Next(8);
                                for (int t = 0; t < L; ++t) tmp[t] = (byte)context.Random.Next(256);
                                PUT8(buf, 0x16);
                                PUT_BIN(buf, tmp, L);
                                used_ad = true;
                            }
                            break;
                    }
                }
                return ms.ToArray();
            }
        }

        private void PUT8(BinaryWriter bw, byte v) { bw.Write(v); }
        private void PUT16(BinaryWriter bw, ushort v) { bw.Write((byte)((v >> 8) & 0xFF)); bw.Write((byte)(v & 0xFF)); }
        private void PUT32(BinaryWriter bw, uint v) { bw.Write((byte)((v >> 24) & 0xFF)); bw.Write((byte)((v >> 16) & 0xFF)); bw.Write((byte)((v >> 8) & 0xFF)); bw.Write((byte)(v & 0xFF)); }
        private void PUT_UTF8(BinaryWriter bw, string s, int maxs)
        {
            byte[] b = SysEncoding.UTF8.GetBytes(s);
            ushort len = (ushort)b.Length;
            PUT16(bw, len);
            if (len > 0) bw.Write(b);
        }
        private void PUT_BIN(BinaryWriter bw, byte[] p, int n)
        {
            ushort L = (ushort)n;
            PUT16(bw, L);
            if (L > 0) bw.Write(p, 0, L);
        }
    }

    [Mutator("MqttConnectAddProperties")]
    [Description("Adds MQTT Connect Properties")]
    public class MqttConnectAddProperties : Mutator
    {
        public MqttConnectAddProperties(DataElement obj) : base(obj) { }
        public new static bool supportedDataElement(DataElement obj) { return obj is Blob && obj.Name == "properties" && obj.parent != null && obj.parent.Name.Contains("variable_header"); }
        public override int count => 1;
        public override uint mutation { get; set; }
        public override void sequentialMutation(DataElement obj)
        {
            byte[] b = GetOriginalBytes(obj);
            if (b != null && b.Length == 0)
            {
                obj.MutatedValue = new Variant(new byte[] { 0x11, 0x00, 0x00, 0x00, 0x0A }); // Session Expiry = 10
            }
            obj.mutationFlags = MutateOverride.Default;
        }
        public override void randomMutation(DataElement obj) { sequentialMutation(obj); }

        private byte[] GetOriginalBytes(DataElement obj)
        {
            try { return (byte[])obj.DefaultValue; } catch { }
            try { 
                var bs = (BitwiseStream)obj.DefaultValue; 
                if(bs != null) {
                    long pos = bs.PositionBits;
                    bs.SeekBits(0, SeekOrigin.Begin);
                    var ms = new MemoryStream();
                    bs.CopyTo(ms);
                    bs.SeekBits(pos, SeekOrigin.Begin);
                    return ms.ToArray();
                }
            } catch { }
            return null;
        }
    }

    [Mutator("MqttConnectDeleteProperties")]
    [Description("Deletes MQTT Connect Properties")]
    public class MqttConnectDeleteProperties : Mutator
    {
        public MqttConnectDeleteProperties(DataElement obj) : base(obj) { }
        public new static bool supportedDataElement(DataElement obj) { return obj is Blob && obj.Name == "properties" && obj.parent != null && obj.parent.Name.Contains("variable_header"); }
        public override int count => 1;
        public override uint mutation { get; set; }
        public override void sequentialMutation(DataElement obj) { obj.parent.Remove(obj); }
        public override void randomMutation(DataElement obj) { obj.parent.Remove(obj); }
    }

    [Mutator("MqttConnectMutateClientId")]
    [Description("Mutates MQTT Client ID")]
    public class MqttConnectMutateClientId : Mutator
    {
        public MqttConnectMutateClientId(DataElement obj) : base(obj) { }
        public new static bool supportedDataElement(DataElement obj) { return obj is Peach.Core.Dom.String && obj.Name == "client_id"; }
        public override int count => 8;
        public override uint mutation { get; set; }

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj, (int)mutation); obj.mutationFlags = MutateOverride.Default; }
        public override void randomMutation(DataElement obj)
        {
            int[] weights = { 0, 70, 0, 0, 0, 0, 0, 0 };
            PerformMutation(obj, PickWeighted(weights));
            obj.mutationFlags = MutateOverride.Default;
        }

        private void PerformMutation(DataElement obj, int strategy)
        {
            string cid = (string)obj.DefaultValue;
            int orig_len = cid.Length;
            string valid_chars = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string bad_chars = " \t\r\n#@$%^&*()[]{}<>?!|~";

            switch (strategy)
            {
                case 0: // 空 ID
                    cid = "";
                    break;
                case 1: // 合法随机 ID（长度 1-23）
                    {
                        int len = 1 + context.Random.Next(23);
                        char[] chars = new char[len];
                        for (int j = 0; j < len; ++j) chars[j] = valid_chars[context.Random.Next(valid_chars.Length)];
                        cid = new string(chars);
                    }
                    break;
                case 2: // 超长 ID
                    {
                        int len = 24 + context.Random.Next(40);
                        char[] chars = new char[len];
                        for (int j = 0; j < len; ++j) chars[j] = valid_chars[context.Random.Next(valid_chars.Length)];
                        cid = new string(chars);
                    }
                    break;
                case 3: // 插入非法字符混合
                    {
                        int len = 5 + context.Random.Next(30);
                        char[] chars = new char[len];
                        for (int j = 0; j < len; ++j)
                        {
                            if (context.Random.Next(3) == 0)
                                chars[j] = bad_chars[context.Random.Next(bad_chars.Length)];
                            else
                                chars[j] = valid_chars[context.Random.Next(valid_chars.Length)];
                        }
                        cid = new string(chars);
                    }
                    break;
                case 4: // 全数字 ID
                    {
                        int len = 3 + context.Random.Next(20);
                        char[] chars = new char[len];
                        for (int j = 0; j < len; ++j) chars[j] = (char)('0' + context.Random.Next(10));
                        cid = new string(chars);
                    }
                    break;
                case 5: // bit-flip 原 ID 若非空
                    if (orig_len > 0)
                    {
                        byte[] b = SysEncoding.UTF8.GetBytes(cid);
                        int flips = 1 + context.Random.Next(3);
                        for (int f = 0; f < flips; ++f)
                        {
                            int pos = context.Random.Next(b.Length);
                            b[pos] ^= (byte)(1 << context.Random.Next(8));
                        }
                        obj.MutatedValue = new Variant(b);
                        return;
                    }
                    break;
                case 6: // 拼接合法段 + 非法段
                    {
                        int len1 = 3 + context.Random.Next(10);
                        int len2 = 3 + context.Random.Next(10);
                        char[] chars = new char[len1 + len2];
                        for (int j = 0; j < len1; ++j) chars[j] = valid_chars[context.Random.Next(valid_chars.Length)];
                        for (int j = 0; j < len2; ++j) chars[len1 + j] = bad_chars[context.Random.Next(bad_chars.Length)];
                        cid = new string(chars);
                    }
                    break;
                case 7: // 截断 ID（部分丢失）
                    if (orig_len > 2)
                    {
                        int new_len = 1 + context.Random.Next(orig_len - 1);
                        cid = cid.Substring(0, new_len);
                    }
                    break;
            }
            obj.MutatedValue = new Variant(cid);
        }

        private int PickWeighted(int[] weights)
        {
            int sum = 0;
            foreach (int w in weights) sum += w;
            if (sum <= 0) return 0;
            int r = context.Random.Next(sum);
            for (int i = 0; i < weights.Length; i++)
            {
                if (r < weights[i]) return i;
                r -= weights[i];
            }
            return weights.Length - 1;
        }
    }

    [Mutator("MqttConnectAddClientId")]
    [Description("Adds MQTT Client ID")]
    public class MqttConnectAddClientId : Mutator
    {
        public MqttConnectAddClientId(DataElement obj) : base(obj) { }
        public new static bool supportedDataElement(DataElement obj) { return obj is Peach.Core.Dom.String && obj.Name == "client_id"; }
        public override int count => 1;
        public override uint mutation { get; set; }
        public override void sequentialMutation(DataElement obj)
        {
            if (string.IsNullOrEmpty((string)obj.DefaultValue))
                obj.MutatedValue = new Variant("client" + context.Random.Next(10000));
            obj.mutationFlags = MutateOverride.Default;
        }
        public override void randomMutation(DataElement obj) { sequentialMutation(obj); }
    }

    [Mutator("MqttConnectDeleteClientId")]
    [Description("Deletes MQTT Client ID")]
    public class MqttConnectDeleteClientId : Mutator
    {
        public MqttConnectDeleteClientId(DataElement obj) : base(obj) { }
        public new static bool supportedDataElement(DataElement obj) { return obj is Peach.Core.Dom.String && obj.Name == "client_id"; }
        public override int count => 1;
        public override uint mutation { get; set; }
        public override void sequentialMutation(DataElement obj) { obj.parent.Remove(obj); }
        public override void randomMutation(DataElement obj) { obj.parent.Remove(obj); }
    }

    [Mutator("MqttConnectMutateUserName")]
    [Description("Mutates MQTT User Name")]
    public class MqttConnectMutateUserName : Mutator
    {
        public MqttConnectMutateUserName(DataElement obj) : base(obj) { }
        public new static bool supportedDataElement(DataElement obj) { return obj is Peach.Core.Dom.String && obj.Name == "user_name"; }
        public override int count => 7;
        public override uint mutation { get; set; }

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj, (int)mutation); obj.mutationFlags = MutateOverride.Default; }
        public override void randomMutation(DataElement obj)
        {
            int[] weights = { 70, 0, 0, 0, 0, 0, 0 };
            PerformMutation(obj, PickWeighted(weights));
            obj.mutationFlags = MutateOverride.Default;
        }

        private void PerformMutation(DataElement obj, int strategy)
        {
            string val = (string)obj.DefaultValue;
            string valid_chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_-!@#";
            string[] special_cases = { "", "admin", "root", "user!@#$%^&*()", "A_very_very_long_username_string_that_may_overflow_the_buffer_lol", "\xFF\xFE\xFD" };

            switch (strategy)
            {
                case 0: // 合法随机用户名
                    {
                        int len = 5 + context.Random.Next(20);
                        char[] chars = new char[len];
                        for (int j = 0; j < len; ++j) chars[j] = valid_chars[context.Random.Next(valid_chars.Length)];
                        val = new string(chars);
                    }
                    break;
                case 1: // 使用特殊测试用例
                    val = special_cases[context.Random.Next(special_cases.Length)];
                    break;
                case 2: // 非法非 ASCII 序列 + UTF-8 序列污染
                    obj.MutatedValue = new Variant(new byte[] { 0xC3, 0x28, 0xA0, 0xA1, 0xFF, 0xFE });
                    return;
                case 3: // 缓冲区溢出模拟：填满 + 不写 \0
                    val = new string('A', 23); // MAX_CLIENT_ID_LEN? C code uses MAX_CLIENT_ID_LEN for buffer. Assuming standard.
                    // But Peach handles strings. Just return long string.
                    val = new string('A', 65535);
                    break;
                case 4: // 清空用户名
                    val = "";
                    break;
                case 5: // NULL 字节注入 + 随机后缀
                    {
                        int len = 5 + context.Random.Next(10);
                        char[] chars = new char[len];
                        for (int j = 0; j < len; ++j) chars[j] = valid_chars[context.Random.Next(valid_chars.Length)];
                        int pos = context.Random.Next(len);
                        chars[pos] = '\0';
                        val = new string(chars);
                    }
                    break;
                case 6: // 随机 bit-flip
                    {
                        byte[] b = SysEncoding.UTF8.GetBytes(val);
                        if (b.Length == 0) b = SysEncoding.UTF8.GetBytes("user");
                        for (int j = 0; j < b.Length; ++j)
                        {
                            if (context.Random.Next(2) != 0) b[j] ^= (byte)(1 << context.Random.Next(8));
                        }
                        obj.MutatedValue = new Variant(b);
                        return;
                    }
            }
            obj.MutatedValue = new Variant(val);
        }

        private int PickWeighted(int[] weights)
        {
            int sum = 0;
            foreach (int w in weights) sum += w;
            if (sum <= 0) return 0;
            int r = context.Random.Next(sum);
            for (int i = 0; i < weights.Length; i++)
            {
                if (r < weights[i]) return i;
                r -= weights[i];
            }
            return weights.Length - 1;
        }
    }

    [Mutator("MqttConnectAddUserName")]
    [Description("Adds MQTT User Name")]
    public class MqttConnectAddUserName : Mutator
    {
        public MqttConnectAddUserName(DataElement obj) : base(obj) { }
        public new static bool supportedDataElement(DataElement obj) { return obj is Peach.Core.Dom.String && obj.Name == "user_name"; }
        public override int count => 1;
        public override uint mutation { get; set; }
        public override void sequentialMutation(DataElement obj) { if (string.IsNullOrEmpty((string)obj.DefaultValue)) obj.MutatedValue = new Variant("user_" + context.Random.Next(int.MaxValue)); obj.mutationFlags = MutateOverride.Default; }
        public override void randomMutation(DataElement obj) { sequentialMutation(obj); }
    }

    [Mutator("MqttConnectDeleteUserName")]
    [Description("Deletes MQTT User Name")]
    public class MqttConnectDeleteUserName : Mutator
    {
        public MqttConnectDeleteUserName(DataElement obj) : base(obj) { }
        public new static bool supportedDataElement(DataElement obj) { return obj is Peach.Core.Dom.String && obj.Name == "user_name"; }
        public override int count => 1;
        public override uint mutation { get; set; }
        public override void sequentialMutation(DataElement obj) { obj.parent.Remove(obj); }
        public override void randomMutation(DataElement obj) { obj.parent.Remove(obj); }
    }

    [Mutator("MqttConnectMutatePassword")]
    [Description("Mutates MQTT Password")]
    public class MqttConnectMutatePassword : Mutator
    {
        public MqttConnectMutatePassword(DataElement obj) : base(obj) { }
        public new static bool supportedDataElement(DataElement obj) { return obj is Blob && obj.Name == "password"; }
        public override int count => 8;
        public override uint mutation { get; set; }

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj, (int)mutation); obj.mutationFlags = MutateOverride.Default; }
        public override void randomMutation(DataElement obj)
        {
            int[] weights = { 70, 0, 0, 0, 0, 0, 0, 0 };
            PerformMutation(obj, PickWeighted(weights));
            obj.mutationFlags = MutateOverride.Default;
        }

        private void PerformMutation(DataElement obj, int strategy)
        {
            byte[] val = null;
            string[] common_passwords = { "", "123456", "password", "pass!@#$_", "admin123", "\x00\x01\xFF\xFE", "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" };

            switch (strategy)
            {
                case 0: // 使用预置常见密码
                    string src = common_passwords[context.Random.Next(common_passwords.Length)];
                    val = SysEncoding.UTF8.GetBytes(src);
                    break;
                case 1: // 空密码
                    val = new byte[0];
                    break;
                case 2: // 固定二进制垃圾
                    val = new byte[] { 0x00, 0xFF, 0xAA, 0x55 };
                    break;
                case 3: // 随机二进制串
                    val = new byte[context.Random.Next(23)]; 
                    new SysRandom().NextBytes(val);
                    break;
                case 4: // 超长填充（全'A'）
                    val = new byte[23];
                    for (int i = 0; i < 23; i++) val[i] = (byte)'A';
                    break;
                case 5: // 非法 UTF-8 序列
                    val = new byte[] { 0xC3, 0x28, 0xA0, 0xA1 };
                    break;
                case 6: // NULL 字节混入 + 后缀
                    {
                        int len = 5 + context.Random.Next(10);
                        val = new byte[len];
                        for (int j = 0; j < len; ++j) val[j] = (byte)('a' + context.Random.Next(26));
                        int pos = context.Random.Next(len);
                        val[pos] = 0;
                    }
                    break;
                case 7: // Bit-flip
                    {
                        val = GetOriginalBytes(obj);
                        if (val == null) val = new byte[0];
                        else val = (byte[])val.Clone();

                        if (val.Length == 0)
                        {
                            val = new byte[5 + context.Random.Next(10)];
                            for (int j = 0; j < val.Length; ++j) val[j] = (byte)('a' + context.Random.Next(26));
                        }
                        int flip_pos = context.Random.Next(val.Length);
                        val[flip_pos] ^= (byte)(1 << context.Random.Next(8));
                    }
                    break;
            }
            obj.MutatedValue = new Variant(val);
        }

        private int PickWeighted(int[] weights)
        {
            int sum = 0;
            foreach (int w in weights) sum += w;
            if (sum <= 0) return 0;
            int r = context.Random.Next(sum);
            for (int i = 0; i < weights.Length; i++)
            {
                if (r < weights[i]) return i;
                r -= weights[i];
            }
            return weights.Length - 1;
        }

        private byte[] GetOriginalBytes(DataElement obj)
        {
            try { return (byte[])obj.DefaultValue; } catch { }
            try { 
                var bs = (BitwiseStream)obj.DefaultValue; 
                if(bs != null) {
                    long pos = bs.PositionBits;
                    bs.SeekBits(0, SeekOrigin.Begin);
                    var ms = new MemoryStream();
                    bs.CopyTo(ms);
                    bs.SeekBits(pos, SeekOrigin.Begin);
                    return ms.ToArray();
                }
            } catch { }
            return null;
        }
    }

    [Mutator("MqttConnectAddPassword")]
    [Description("Adds MQTT Password")]
    public class MqttConnectAddPassword : Mutator
    {
        public MqttConnectAddPassword(DataElement obj) : base(obj) { }
        public new static bool supportedDataElement(DataElement obj) { return obj is Blob && obj.Name == "password"; }
        public override int count => 1;
        public override uint mutation { get; set; }
        public override void sequentialMutation(DataElement obj)
        {
            byte[] b = GetOriginalBytes(obj);
            if (b != null && b.Length == 0)
            {
                obj.MutatedValue = new Variant(SysEncoding.UTF8.GetBytes("secret_pass"));
            }
            obj.mutationFlags = MutateOverride.Default;
        }
        public override void randomMutation(DataElement obj) { sequentialMutation(obj); }

        private byte[] GetOriginalBytes(DataElement obj)
        {
            try { return (byte[])obj.DefaultValue; } catch { }
            try { 
                var bs = (BitwiseStream)obj.DefaultValue; 
                if(bs != null) {
                    long pos = bs.PositionBits;
                    bs.SeekBits(0, SeekOrigin.Begin);
                    var ms = new MemoryStream();
                    bs.CopyTo(ms);
                    bs.SeekBits(pos, SeekOrigin.Begin);
                    return ms.ToArray();
                }
            } catch { }
            return null;
        }
    }

    [Mutator("MqttConnectDeletePassword")]
    [Description("Deletes MQTT Password")]
    public class MqttConnectDeletePassword : Mutator
    {
        public MqttConnectDeletePassword(DataElement obj) : base(obj) { }
        public new static bool supportedDataElement(DataElement obj) { return obj is Blob && obj.Name == "password"; }
        public override int count => 1;
        public override uint mutation { get; set; }
        public override void sequentialMutation(DataElement obj) { obj.parent.Remove(obj); }
        public override void randomMutation(DataElement obj) { obj.parent.Remove(obj); }
    }
}
