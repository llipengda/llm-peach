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

using static Peach.LLM.Core.Mutators.MQTT.MqttUtils;
using System.Text;

namespace Peach.LLM.Core.Mutators.MQTT
{
    [Mutator("MqttConnectMutateFlags")]
    [CMutator("mutate_connect_flags")]
    [Description("Mutates MQTT Connect Flags")]
    public class MqttConnectMutateFlags : LLMMutator
    {
        public MqttConnectMutateFlags(DataElement obj) : base(obj) { }
        public new static bool supportedDataElement(DataElement obj)
        {
            return obj is Number && obj.Name == "connect_flags"
                && obj.parent != null && obj.parent.referenceName == "mqtt_connect_variable_header_t";
        }
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
            uint original = (uint)((Number)obj).InternalValue;
            uint mutated = original;
            switch (strategy)
            {
                case 0: // 合法组合：随机合法构造
                    uint clean = (uint)Next(2);
                    uint will = (uint)Next(2);
                    uint qos = (uint)Next(3);
                    uint retain = will != 0 ? (uint)Next(2) : 0;
                    uint user = (uint)Next(2);
                    uint pass = (uint)Next(2);

                    mutated = 0;
                    mutated |= user << 7;
                    mutated |= pass << 6;
                    mutated |= retain << 5;
                    mutated |= (qos & 0x03) << 3;
                    mutated |= will << 2;
                    mutated |= clean << 1;
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
                    mutated = original ^ (1u << Next(8));
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


    }

    [Mutator("MqttConnectMutateKeepAlive")]
    [CMutator("mutate_connect_keep_alive")]
    [Description("Mutates MQTT Connect Keep Alive")]
    public class MqttConnectMutateKeepAlive : LLMMutator
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
            uint orig = (uint)((Number)obj).InternalValue;
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
                    mutated = (uint)Next(10000); // 合法范围内随机值
                    break;
                case 4:
                    mutated = orig + (uint)Next(1000); // 正向扰动
                    if (mutated > 65535) mutated = 65535;
                    break;
                case 5:
                    int val = (int)orig - Next(1000); // 负向扰动
                    mutated = (uint)val; // cast to uint simulates underflow behavior of C uint16 arithmetic before masking
                    break;
                case 6:
                    mutated = (uint)Next(int.MaxValue); // 全随机
                    break;
            }
            obj.MutatedValue = new Variant(mutated & 0xFFFF);
        }


    }

    [Mutator("MqttConnectMutateProperties")]
    [CMutator("mutate_connect_properties")]
    [Description("Mutates MQTT Connect Properties")]
    public class MqttConnectMutateProperties : LLMMutator
    {
        public MqttConnectMutateProperties(DataElement obj) : base(obj) { }
        public new static bool supportedDataElement(DataElement obj)
        {
            return obj is Blob && obj.Name == "properties" && obj.parent != null && obj.parent.referenceName == "mqtt_connect_variable_header_t";
        }
        public override int count => 1;
        public override uint mutation { get; set; }
        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); obj.mutationFlags = MutateOverride.Default; }
        public override void randomMutation(DataElement obj) { PerformMutation(obj); obj.mutationFlags = MutateOverride.Default; }

        protected override void PerformMutation(DataElement obj)
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

                int num_props = 1 + Next(6);
                for (int n = 0; n < num_props; ++n)
                {
                    int pick = Next(9);
                    switch (pick)
                    {
                        case 0: if (!used_ses) { PUT8(buf, 0x11); PUT32(buf, (uint)Next(86400)); used_ses = true; } break;
                        case 1: if (!used_rcv) { PUT8(buf, 0x12); PUT16(buf, (ushort)(1 + Next(1024))); used_rcv = true; } break;
                        case 2: if (!used_max) { PUT8(buf, 0x13); PUT32(buf, (uint)(512 + Next(65536))); used_max = true; } break;
                        case 3: if (!used_ta) { PUT8(buf, 0x22); PUT16(buf, (ushort)(1 + Next(100))); used_ta = true; } break; // 0x22 or 0x15? C code define PID_TA_MAX 0x22.
                        case 4: if (!used_rr) { PUT8(buf, 0x17); PUT8(buf, (byte)Next(2)); used_rr = true; } break;
                        case 5: if (!used_rp) { PUT8(buf, 0x19); PUT8(buf, (byte)Next(2)); used_rp = true; } break;
                        case 6: // User Property
                            PUT8(buf, 0x26); PUT_UTF8(buf, "key", 32); PUT_UTF8(buf, "val", 32);
                            break;
                        case 7: if (!used_am) { PUT8(buf, 0x15); PUT_UTF8(buf, "PLAIN", 64); used_am = true; } break;
                        case 8:
                            if (!used_ad)
                            {
                                byte[] tmp = new byte[16];
                                int L = 4 + Next(8);
                                for (int t = 0; t < L; ++t) tmp[t] = (byte)Next(256);
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
    [CMutator("add_connect_properties")]
    [Description("Adds MQTT Connect Properties")]
    public class MqttConnectAddProperties : LLMMutator
    {
        public MqttConnectAddProperties(DataElement obj) : base(obj) { }
        public new static bool supportedDataElement(DataElement obj)
        {
            return obj is Blob && obj.Name == "properties" && obj.parent != null && obj.parent.referenceName == "mqtt_connect_variable_header_t";
        }
        public override int count => 1;
        public override uint mutation { get; set; }
        public override void sequentialMutation(DataElement obj)
        {
            byte[] b = obj.Bytes();
            if (b != null && b.Length == 0)
            {
                obj.MutatedValue = new Variant(new byte[] { 0x11, 0x00, 0x00, 0x00, 0x0A }); // Session Expiry = 10
            }
            obj.mutationFlags = MutateOverride.Default;
        }
        public override void randomMutation(DataElement obj) { sequentialMutation(obj); }
    }

    [Mutator("MqttConnectDeleteProperties")]
    [CMutator("delete_connect_properties")]
    [Description("Deletes MQTT Connect Properties")]
    public class MqttConnectDeleteProperties : LLMMutator
    {
        public MqttConnectDeleteProperties(DataElement obj) : base(obj) { }
        public new static bool supportedDataElement(DataElement obj)
        {
            return obj is Blob && obj.Name == "properties" && obj.parent != null && obj.parent.referenceName == "mqtt_connect_variable_header_t";
        }
        public override int count => 1;
        public override uint mutation { get; set; }
        public override void sequentialMutation(DataElement obj) { obj.parent.Remove(obj); }
        public override void randomMutation(DataElement obj) { obj.parent.Remove(obj); }
    }

    [Mutator("MqttConnectMutateClientId")]
    [CMutator("mutate_connect_client_id")]
    [Description("Mutates MQTT Client ID")]
    public class MqttConnectMutateClientId : LLMMutator
    {
        public MqttConnectMutateClientId(DataElement obj) : base(obj) { }
        public new static bool supportedDataElement(DataElement obj)
        {
            return obj is Peach.Core.Dom.String && obj.parent != null && obj.parent.Name == "client_id";
        }
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
            string cid = FromUtf8(obj.Bytes());
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
                        int len = 1 + Next(23);
                        char[] chars = new char[len];
                        for (int j = 0; j < len; ++j) chars[j] = valid_chars[Next(valid_chars.Length)];
                        cid = new string(chars);
                    }
                    break;
                case 2: // 超长 ID
                    {
                        int len = 24 + Next(40);
                        char[] chars = new char[len];
                        for (int j = 0; j < len; ++j) chars[j] = valid_chars[Next(valid_chars.Length)];
                        cid = new string(chars);
                    }
                    break;
                case 3: // 插入非法字符混合
                    {
                        int len = 5 + Next(30);
                        char[] chars = new char[len];
                        for (int j = 0; j < len; ++j)
                        {
                            if (Next(3) == 0)
                                chars[j] = bad_chars[Next(bad_chars.Length)];
                            else
                                chars[j] = valid_chars[Next(valid_chars.Length)];
                        }
                        cid = new string(chars);
                    }
                    break;
                case 4: // 全数字 ID
                    {
                        int len = 3 + Next(20);
                        char[] chars = new char[len];
                        for (int j = 0; j < len; ++j) chars[j] = (char)('0' + Next(10));
                        cid = new string(chars);
                    }
                    break;
                case 5: // bit-flip 原 ID 若非空
                    if (orig_len > 0)
                    {
                        byte[] b = SysEncoding.UTF8.GetBytes(cid);
                        int flips = 1 + Next(3);
                        for (int f = 0; f < flips; ++f)
                        {
                            int pos = Next(b.Length);
                            b[pos] ^= (byte)(1 << Next(8));
                        }
                        obj.MutatedValue = new Variant(b);
                        return;
                    }
                    break;
                case 6: // 拼接合法段 + 非法段
                    {
                        int len1 = 3 + Next(10);
                        int len2 = 3 + Next(10);
                        char[] chars = new char[len1 + len2];
                        for (int j = 0; j < len1; ++j) chars[j] = valid_chars[Next(valid_chars.Length)];
                        for (int j = 0; j < len2; ++j) chars[len1 + j] = bad_chars[Next(bad_chars.Length)];
                        cid = new string(chars);
                    }
                    break;
                case 7: // 截断 ID（部分丢失）
                    if (orig_len > 2)
                    {
                        int new_len = 1 + Next(orig_len - 1);
                        cid = cid.Substring(0, new_len);
                    }
                    break;
            }
            obj.MutatedValue = new Variant(ToUtf8(cid));
        }


    }

    [Mutator("MqttConnectAddClientId")]
    [CMutator("add_connect_client_id")]
    [Description("Adds MQTT Client ID")]
    public class MqttConnectAddClientId : LLMMutator
    {
        public MqttConnectAddClientId(DataElement obj) : base(obj) { }
        public new static bool supportedDataElement(DataElement obj) { return obj is Peach.Core.Dom.Block && obj.Name == "client_id"; }
        public override int count => 1;
        public override uint mutation { get; set; }
        public override void sequentialMutation(DataElement obj)
        {
            var clientId = FromMqttString(obj.Bytes());
            if (string.IsNullOrEmpty(clientId))
                obj.MutatedValue = new Variant(ToMqttString("client" + Next(10000)));
            obj.mutationFlags = MutateOverride.Default;
        }
        public override void randomMutation(DataElement obj) { sequentialMutation(obj); }
    }

    [Mutator("MqttConnectDeleteClientId")]
    [CMutator("delete_connect_client_id")]
    [Description("Deletes MQTT Client ID")]
    public class MqttConnectDeleteClientId : LLMMutator
    {
        public MqttConnectDeleteClientId(DataElement obj) : base(obj) { }
        public new static bool supportedDataElement(DataElement obj)
        {
            return obj is Peach.Core.Dom.Block && obj.Name == "client_id";
        }
        public override int count => 1;
        public override uint mutation { get; set; }
        public override void sequentialMutation(DataElement obj) { obj.parent.Remove(obj); }
        public override void randomMutation(DataElement obj) { obj.parent.Remove(obj); }
    }

    [Mutator("MqttConnectMutateUserName")]
    [CMutator("mutate_connect_user_name")]
    [Description("Mutates MQTT User Name")]
    public class MqttConnectMutateUserName : LLMMutator
    {
        public MqttConnectMutateUserName(DataElement obj) : base(obj) { }
        public new static bool supportedDataElement(DataElement obj) { return obj is Peach.Core.Dom.Block && obj.Name == "user_name"; }
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
            string val = FromMqttString(obj.Bytes());
            string valid_chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_-!@#";
            string[] special_cases = { "", "admin", "root", "user!@#$%^&*()", "A_very_very_long_username_string_that_may_overflow_the_buffer_lol", "\xFF\xFE\xFD" };

            switch (strategy)
            {
                case 0: // 合法随机用户名
                    {
                        int len = 5 + Next(20);
                        char[] chars = new char[len];
                        for (int j = 0; j < len; ++j) chars[j] = valid_chars[Next(valid_chars.Length)];
                        val = new string(chars);
                    }
                    break;
                case 1: // 使用特殊测试用例
                    val = special_cases[Next(special_cases.Length)];
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
                        int len = 5 + Next(10);
                        char[] chars = new char[len];
                        for (int j = 0; j < len; ++j) chars[j] = valid_chars[Next(valid_chars.Length)];
                        int pos = Next(len);
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
                            if (Next(2) != 0) b[j] ^= (byte)(1 << Next(8));
                        }
                        obj.MutatedValue = new Variant(b);
                        return;
                    }
            }
            obj.MutatedValue = new Variant(ToMqttString(val));
        }


    }

    [Mutator("MqttConnectAddUserName")]
    [CMutator("add_connect_user_name")]
    [Description("Adds MQTT User Name")]
    public class MqttConnectAddUserName : LLMMutator
    {
        public MqttConnectAddUserName(DataElement obj) : base(obj) { }
        public new static bool supportedDataElement(DataElement obj) { return obj is Peach.Core.Dom.Block && obj.Name == "user_name"; }
        public override int count => 1;
        public override uint mutation { get; set; }
        public override void sequentialMutation(DataElement obj)
        {
            var origin = FromMqttString(obj.Bytes());
            if (string.IsNullOrEmpty(origin))
                obj.MutatedValue = new Variant(ToMqttString("user_" + Next(int.MaxValue)));
            obj.mutationFlags = MutateOverride.Default;
        }
        public override void randomMutation(DataElement obj) { sequentialMutation(obj); }
    }

    [Mutator("MqttConnectDeleteUserName")]
    [CMutator("delete_connect_user_name")]
    [Description("Deletes MQTT User Name")]
    public class MqttConnectDeleteUserName : LLMMutator
    {
        public MqttConnectDeleteUserName(DataElement obj) : base(obj) { }
        public new static bool supportedDataElement(DataElement obj)
        {
            return obj is Peach.Core.Dom.Block && obj.Name == "user_name";
        }
        public override int count => 1;
        public override uint mutation { get; set; }
        public override void sequentialMutation(DataElement obj) { obj.parent.Remove(obj); }
        public override void randomMutation(DataElement obj) { obj.parent.Remove(obj); }
    }

    [Mutator("MqttConnectMutatePassword")]
    [CMutator("mutate_connect_password")]
    [Description("Mutates MQTT Password")]
    public class MqttConnectMutatePassword : LLMMutator
    {
        public MqttConnectMutatePassword(DataElement obj) : base(obj) { }
        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.Name == "password";
        }
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
                    string src = common_passwords[Next(common_passwords.Length)];
                    val = SysEncoding.UTF8.GetBytes(src);
                    break;
                case 1: // 空密码
                    val = new byte[0];
                    break;
                case 2: // 固定二进制垃圾
                    val = new byte[] { 0x00, 0xFF, 0xAA, 0x55 };
                    break;
                case 3: // 随机二进制串
                    val = new byte[Next(23)];
                    NextBytes(val);
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
                        int len = 5 + Next(10);
                        val = new byte[len];
                        for (int j = 0; j < len; ++j) val[j] = (byte)('a' + Next(26));
                        int pos = Next(len);
                        val[pos] = 0;
                    }
                    break;
                case 7: // Bit-flip
                    {
                        val = obj.Bytes();
                        if (val == null) val = new byte[0];
                        else val = (byte[])val.Clone();

                        if (val.Length == 0)
                        {
                            val = new byte[5 + Next(10)];
                            for (int j = 0; j < val.Length; ++j) val[j] = (byte)('a' + Next(26));
                        }
                        int flip_pos = Next(val.Length);
                        val[flip_pos] ^= (byte)(1 << Next(8));
                    }
                    break;
            }
            obj.MutatedValue = new Variant(val);
        }


    }

    [Mutator("MqttConnectAddPassword")]
    [CMutator("add_connect_password")]
    [Description("Adds MQTT Password")]
    public class MqttConnectAddPassword : LLMMutator
    {
        public MqttConnectAddPassword(DataElement obj) : base(obj) { }
        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.Name == "password";
        }
        public override int count => 1;
        public override uint mutation { get; set; }
        public override void sequentialMutation(DataElement obj)
        {
            byte[] b = obj.Bytes();
            if (b != null && b.Length == 0)
            {
                obj.MutatedValue = new Variant(SysEncoding.UTF8.GetBytes("secret_pass"));
            }
            obj.mutationFlags = MutateOverride.Default;
        }
        public override void randomMutation(DataElement obj) { sequentialMutation(obj); }
    }

    [Mutator("MqttConnectDeletePassword")]
    [CMutator("delete_connect_password")]
    [Description("Deletes MQTT Password")]
    public class MqttConnectDeletePassword : LLMMutator
    {
        public MqttConnectDeletePassword(DataElement obj) : base(obj) { }
        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.Name == "password";
        }
        public override int count => 1;
        public override uint mutation { get; set; }
        public override void sequentialMutation(DataElement obj) { obj.parent.Remove(obj); }
        public override void randomMutation(DataElement obj) { obj.parent.Remove(obj); }
    }

    [Mutator("MqttConnectMutateWillProperties")]
    [CMutator("mutate_connect_will_properties")]
    [Description("Mutates MQTT Will Properties")]
    public class MqttConnectMutateWillProperties : LLMMutator
    {
        public MqttConnectMutateWillProperties(DataElement obj) : base(obj) { }
        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.Name == "will_properties";
        }
        public override int count => 8;
        public override uint mutation { get; set; }

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj, (int)mutation); obj.mutationFlags = MutateOverride.Default; }
        public override void randomMutation(DataElement obj)
        {
            int[] weights = { 40, 40, 0, 0, 0, 0, 0, 0 };
            PerformMutation(obj, PickWeighted(weights));
            obj.mutationFlags = MutateOverride.Default;
        }

        private void PerformMutation(DataElement obj, int strategy)
        {
            obj.MutatedValue = new Variant(GenerateProperties(strategy));
        }

        private byte[] GenerateProperties(int strategy)
        {
            byte[] legal_will_prop_ids = { 0x01, 0x02, 0x03, 0x08, 0x09, 0x26, 0x27, 0x28 };
            using (var ms = new MemoryStream())
            using (var bw = new BinaryWriter(ms))
            {
                switch (strategy)
                {
                    case 0: // 合法单个属性
                        bw.Write(legal_will_prop_ids[Next(legal_will_prop_ids.Length)]);
                        bw.Write((byte)0x00);
                        bw.Write((byte)Next(256));
                        break;
                    case 1: // 合法多个属性混合
                        int count = 2 + Next(4); // 2-5
                        for (int j = 0; j < count; ++j)
                        {
                            bw.Write(legal_will_prop_ids[Next(legal_will_prop_ids.Length)]);
                            bw.Write((byte)0x00);
                            bw.Write((byte)Next(256));
                        }
                        break;
                    case 2: // 非法属性 ID混入
                        int len = 3 + Next(5);
                        for (int j = 0; j < len; ++j)
                        {
                            byte val = (Next(2) != 0) ? (byte)0xFF : legal_will_prop_ids[Next(legal_will_prop_ids.Length)];
                            bw.Write(val);
                        }
                        break;
                    case 3: // 超长属性随机填充 (Simulated by returning max buf)
                        byte[] large = new byte[65535]; // MAX_PROPERTIES_LEN? Assuming large.
                        NextBytes(large);
                        return large;
                    case 4: // 重复属性段
                        byte id = legal_will_prop_ids[Next(legal_will_prop_ids.Length)];
                        int repeat = 1 + Next(5);
                        for (int j = 0; j < repeat; ++j)
                        {
                            bw.Write(id);
                            bw.Write((byte)0x00);
                            bw.Write((byte)Next(256));
                        }
                        break;
                    case 5: // 全 0x00
                        int len0 = 2 + Next(10);
                        return new byte[len0];
                    case 6: // 全 0xFF
                        int lenF = 2 + Next(10);
                        byte[] bufF = new byte[lenF];
                        for (int i = 0; i < lenF; i++) bufF[i] = 0xFF;
                        return bufF;
                    case 7: // bitflip + 插入垃圾尾部
                        {
                            int l = 5 + Next(10);
                            byte[] buf = new byte[l + 5]; // space for tail
                            for (int j = 0; j < l; ++j)
                            {
                                buf[j] = (byte)Next(256);
                                if (Next(3) == 0) buf[j] ^= (byte)(1 << Next(8));
                            }
                            // Garbage tail
                            for (int j = l; j < l + 5; ++j) buf[j] = (byte)Next(256);
                            return buf;
                        }
                }
                return ms.ToArray();
            }
        }
    }

    [Mutator("MqttConnectAddWillProperties")]
    [CMutator("add_connect_will_properties")]
    [Description("Adds MQTT Will Properties")]
    public class MqttConnectAddWillProperties : LLMMutator
    {
        public MqttConnectAddWillProperties(DataElement obj) : base(obj) { }
        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.Name == "will_properties";
        }
        public override int count => 1;
        public override uint mutation { get; set; }
        public override void sequentialMutation(DataElement obj)
        {
            PerformMutation(obj);
            obj.mutationFlags = MutateOverride.Default;
        }
        public override void randomMutation(DataElement obj) { sequentialMutation(obj); }

        protected override void PerformMutation(DataElement obj)
        {
            using (var ms = new MemoryStream())
            using (var bw = new BinaryWriter(ms))
            {
                int strategy = Next(6);
                switch (strategy)
                {
                    case 0: bw.Write((byte)0x01); bw.Write((byte)1); break;
                    case 1: bw.Write((byte)0x02); PUT32(bw, (uint)Next(3601)); break;
                    case 2: bw.Write((byte)0x18); PUT32(bw, (uint)Next(601)); break;
                    case 3: bw.Write((byte)0x03); PUT_UTF8_LIT(bw, "text/plain"); break;
                    case 4: bw.Write((byte)0x08); PUT_UTF8_LIT(bw, "reply/topic"); break;
                    case 5: bw.Write((byte)0x09); PUT_BIN_RAND(bw, 8, 24); break;
                }
                // User props
                string[] keys = { "source", "priority", "note", "device" };
                string[] vals = { "sensor1", "high", "ok", "edge" };
                int upn = Next(3);
                for (int t = 0; t < upn; ++t)
                {
                    bw.Write((byte)0x26);
                    PUT_UTF8_LIT(bw, keys[Next(4)]);
                    PUT_UTF8_LIT(bw, vals[Next(4)]);
                }
                obj.MutatedValue = new Variant(ms.ToArray());
            }
        }

        private void PUT32(BinaryWriter bw, uint v) { bw.Write((byte)((v >> 24) & 0xFF)); bw.Write((byte)((v >> 16) & 0xFF)); bw.Write((byte)((v >> 8) & 0xFF)); bw.Write((byte)(v & 0xFF)); }
        private void PUT16(BinaryWriter bw, ushort v) { bw.Write((byte)((v >> 8) & 0xFF)); bw.Write((byte)(v & 0xFF)); }
        private void PUT_UTF8_LIT(BinaryWriter bw, string s) { byte[] b = SysEncoding.UTF8.GetBytes(s); PUT16(bw, (ushort)b.Length); bw.Write(b); }
        private void PUT_BIN_RAND(BinaryWriter bw, int min, int max)
        {
            int L = min + Next(max - min + 1);
            PUT16(bw, (ushort)L);
            byte[] b = new byte[L];
            NextBytes(b);
            bw.Write(b);
        }
    }

    [Mutator("MqttConnectDeleteWillProperties")]
    [CMutator("delete_connect_will_properties")]
    [Description("Deletes MQTT Will Properties")]
    public class MqttConnectDeleteWillProperties : LLMMutator
    {
        public MqttConnectDeleteWillProperties(DataElement obj) : base(obj) { }
        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.Name == "will_properties";
        }
        public override int count => 1;
        public override uint mutation { get; set; }
        public override void sequentialMutation(DataElement obj) { obj.parent.Remove(obj); }
        public override void randomMutation(DataElement obj) { obj.parent.Remove(obj); }
    }

    [Mutator("MqttConnectMutateWillTopic")]
    [CMutator("mutate_connect_will_topic")]
    [Description("Mutates MQTT Will Topic")]
    public class MqttConnectMutateWillTopic : LLMMutator
    {
        public MqttConnectMutateWillTopic(DataElement obj) : base(obj) { }
        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.Name == "will_topic";
        }
        public override int count => 6;
        public override uint mutation { get; set; }

        public override void sequentialMutation(DataElement obj) { PerformMutation(obj, (int)mutation); obj.mutationFlags = MutateOverride.Default; }
        public override void randomMutation(DataElement obj)
        {
            int[] weights = { 0, 50, 0, 0, 0, 0 };
            PerformMutation(obj, PickWeighted(weights));
            obj.mutationFlags = MutateOverride.Default;
        }

        private void PerformMutation(DataElement obj, int strategy)
        {
            string val = "";
            string[] base_topics = { "", "/", "home/sensor", "+/#", "#/invalid", "topic\x00mid", "\xC3\x28", "\xFF\xFF\xFF" };
            string valid_chars = "abcdefghijklmnopqrstuvwxyz/+-_0123456789";

            switch (strategy)
            {
                case 0: // 使用预设 topic
                    val = base_topics[Next(base_topics.Length)];
                    break;
                case 1: // 生成合法随机 topic
                    {
                        int len = 1 + Next(20);
                        StringBuilder sb = new StringBuilder();
                        for (int j = 0; j < len; ++j) sb.Append(valid_chars[Next(valid_chars.Length)]);
                        val = sb.ToString();
                    }
                    break;
                case 2: // 生成超长 topic
                    {
                        int len = 200 + Next(20);
                        StringBuilder sb = new StringBuilder();
                        for (int j = 0; j < len; ++j) sb.Append(valid_chars[Next(valid_chars.Length)]);
                        val = sb.ToString();
                    }
                    break;
                case 3: // 拼接合法 + 非法片段
                    {
                        val = base_topics[Next(base_topics.Length)];
                        val += "\xFF"; // Append invalid char
                    }
                    break;
                case 4: // 插入特殊符号 & bitflip
                    {
                        int len = 1 + Next(20);
                        byte[] b = new byte[len];
                        for (int j = 0; j < len; ++j)
                        {
                            if (Next(4) == 0) b[j] = (byte)'#';
                            else
                            {
                                b[j] = (byte)valid_chars[Next(valid_chars.Length)];
                                if (Next(3) == 0) b[j] ^= (byte)(1 << Next(8));
                            }
                        }
                        var sb = new StringBuilder();
                        for (int j = 0; j < len; ++j) sb.Append((char)b[j]);
                        val = sb.ToString();
                        break;
                    }
                case 5: // 全 NULL 字节
                    val = new string('\0', 20);
                    break;
            }
            obj.MutatedValue = new Variant(ToMqttString(val));
        }
    }

    [Mutator("MqttConnectAddWillTopic")]
    [CMutator("add_connect_will_topic")]
    [Description("Adds MQTT Will Topic")]
    public class MqttConnectAddWillTopic : LLMMutator
    {
        public MqttConnectAddWillTopic(DataElement obj) : base(obj) { }
        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.Name == "will_topic";
        }
        public override int count => 1;
        public override uint mutation { get; set; }
        public override void sequentialMutation(DataElement obj)
        {
            if (string.IsNullOrEmpty(FromMqttString(obj.Bytes())))
            {
                string[] sample_topics = { "sensor/temp", "a/b/c", "device/+/status", "home/+/light/#", "你好/测试" };
                obj.MutatedValue = new Variant(ToMqttString(sample_topics[Next(sample_topics.Length)]));
            }
            obj.mutationFlags = MutateOverride.Default;
        }
        public override void randomMutation(DataElement obj) { sequentialMutation(obj); }
    }

    [Mutator("MqttConnectDeleteWillTopic")]
    [CMutator("delete_connect_will_topic")]
    [Description("Deletes MQTT Will Topic")]
    public class MqttConnectDeleteWillTopic : LLMMutator
    {
        public MqttConnectDeleteWillTopic(DataElement obj) : base(obj) { }
        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.Name == "will_topic";
        }
        public override int count => 1;
        public override uint mutation { get; set; }
        public override void sequentialMutation(DataElement obj) { obj.parent.Remove(obj); }
        public override void randomMutation(DataElement obj) { obj.parent.Remove(obj); }
    }

    [Mutator("MqttConnectMutateWillPayload")]
    [CMutator("mutate_connect_will_payload")]
    [Description("Mutates MQTT Will Payload")]
    public class MqttConnectMutateWillPayload : LLMMutator
    {
        public MqttConnectMutateWillPayload(DataElement obj) : base(obj) { }
        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.Name == "will_payload";
        }
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
            byte[] val = null;
            string valid_chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*()_+-=[]{}|;:',.<>?/`~";

            switch (strategy)
            {
                case 0: // 合法 UTF-8 文本
                    {
                        int l = 5 + Next(20);
                        char[] c = new char[l];
                        for (int j = 0; j < l; ++j) c[j] = valid_chars[Next(valid_chars.Length)];
                        val = ToUtf8(new string(c));
                    }
                    break;
                case 1: // 二进制模式
                    {
                        int l = 1 + Next(64);
                        val = new byte[l];
                        NextBytes(val);
                    }
                    break;
                case 2: // 空 payload
                    val = new byte[0];
                    break;
                case 3: // 超长 payload
                    val = new byte[65535];
                    NextBytes(val);
                    break;
                case 4: // 插入 NULL 字节 + 随机尾随数据
                    {
                        int l = 5 + Next(10);
                        val = new byte[l];
                        for (int j = 0; j < l; ++j) val[j] = (byte)valid_chars[Next(valid_chars.Length)];
                        val[Next(l)] = 0;
                    }
                    break;
                case 5: // 非法 UTF-8 序列
                    val = new byte[] { 0xC3, 0x28, 0xA0, 0xA1, 0xE2, 0x28, 0xA1 };
                    break;
                case 6: // 混合合法文本 + 垃圾二进制
                    {
                        int l = 10 + Next(30);
                        int split = Next(l);
                        val = new byte[l];
                        for (int j = 0; j < split; ++j) val[j] = (byte)valid_chars[Next(valid_chars.Length)];
                        for (int j = split; j < l; ++j) val[j] = (byte)Next(256);
                    }
                    break;
            }
            obj.MutatedValue = new Variant(val);
        }
    }

    [Mutator("MqttConnectAddWillPayload")]
    [CMutator("add_connect_will_payload")]
    [Description("Adds MQTT Will Payload")]
    public class MqttConnectAddWillPayload : LLMMutator
    {
        public MqttConnectAddWillPayload(DataElement obj) : base(obj) { }
        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.Name == "will_payload";
        }
        public override int count => 1;
        public override uint mutation { get; set; }
        public override void sequentialMutation(DataElement obj)
        {
            byte[] b = obj.Bytes();
            if (b != null && b.Length == 0)
            {
                string[] samples = { "device offline", "error: timeout", "{\"status\": \"dead\"}", "MQTT last will", "\xDE\xAD\xBE\xEF" };
                string data = samples[Next(samples.Length)];
                obj.MutatedValue = new Variant(ToUtf8(data));
            }
            obj.mutationFlags = MutateOverride.Default;
        }
        public override void randomMutation(DataElement obj) { sequentialMutation(obj); }
    }

    [Mutator("MqttConnectDeleteWillPayload")]
    [CMutator("delete_connect_will_payload")]
    [Description("Deletes MQTT Will Payload")]
    public class MqttConnectDeleteWillPayload : LLMMutator
    {
        public MqttConnectDeleteWillPayload(DataElement obj) : base(obj) { }
        public new static bool supportedDataElement(DataElement obj) {
            return obj.Name == "will_payload";
        }
        public override int count => 1;
        public override uint mutation { get; set; }
        public override void sequentialMutation(DataElement obj) { obj.parent.Remove(obj); }
        public override void randomMutation(DataElement obj) { obj.parent.Remove(obj); }
    }
}
