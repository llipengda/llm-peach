using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using Peach.Core;
using Peach.Core.Dom;
using Peach.Pro.Core.Mutators;
using System.Text;
using Peach.Core.IO;

using SysRandom = System.Random;
using SysEncoding = System.Text.Encoding;
using SysArray = System.Array;

namespace Peach.Pro.Core.Mutators.MQTT
{
    [Mutator("MqttConnectMutateWillProperties")]
    [Description("Mutates MQTT Will Properties")]
    public class MqttConnectMutateWillProperties : Mutator
    {
        public MqttConnectMutateWillProperties(DataElement obj) : base(obj) { }
        public new static bool supportedDataElement(DataElement obj) { return obj is Blob && obj.Name == "will_properties"; }
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
            using(var ms = new MemoryStream())
            using(var bw = new BinaryWriter(ms))
            {
                switch(strategy)
                {
                    case 0: // 合法单个属性
                        bw.Write(legal_will_prop_ids[context.Random.Next(legal_will_prop_ids.Length)]);
                        bw.Write((byte)0x00);
                        bw.Write((byte)context.Random.Next(256));
                        break;
                    case 1: // 合法多个属性混合
                        int count = 2 + context.Random.Next(4); // 2-5
                        for(int j=0; j<count; ++j)
                        {
                            bw.Write(legal_will_prop_ids[context.Random.Next(legal_will_prop_ids.Length)]);
                            bw.Write((byte)0x00);
                            bw.Write((byte)context.Random.Next(256));
                        }
                        break;
                    case 2: // 非法属性 ID混入
                        int len = 3 + context.Random.Next(5);
                        for(int j=0; j<len; ++j)
                        {
                            byte val = (context.Random.Next(2) != 0) ? (byte)0xFF : legal_will_prop_ids[context.Random.Next(legal_will_prop_ids.Length)];
                            bw.Write(val);
                        }
                        break;
                    case 3: // 超长属性随机填充 (Simulated by returning max buf)
                        byte[] large = new byte[65535]; // MAX_PROPERTIES_LEN? Assuming large.
                        new SysRandom().NextBytes(large);
                        return large;
                    case 4: // 重复属性段
                        byte id = legal_will_prop_ids[context.Random.Next(legal_will_prop_ids.Length)];
                        int repeat = 1 + context.Random.Next(5);
                        for(int j=0; j<repeat; ++j)
                        {
                            bw.Write(id);
                            bw.Write((byte)0x00);
                            bw.Write((byte)context.Random.Next(256));
                        }
                        break;
                    case 5: // 全 0x00
                        int len0 = 2 + context.Random.Next(10);
                        return new byte[len0];
                    case 6: // 全 0xFF
                        int lenF = 2 + context.Random.Next(10);
                        byte[] bufF = new byte[lenF];
                        for(int i=0; i<lenF; i++) bufF[i]=0xFF;
                        return bufF;
                    case 7: // bitflip + 插入垃圾尾部
                        {
                            int l = 5 + context.Random.Next(10);
                            byte[] buf = new byte[l + 5]; // space for tail
                            for(int j=0; j<l; ++j)
                            {
                                buf[j] = (byte)context.Random.Next(256);
                                if(context.Random.Next(3) == 0) buf[j] ^= (byte)(1 << context.Random.Next(8));
                            }
                            // Garbage tail
                            for(int j=l; j<l+5; ++j) buf[j] = (byte)context.Random.Next(256);
                            return buf;
                        }
                }
                return ms.ToArray();
            }
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

    [Mutator("MqttConnectAddWillProperties")]
    [Description("Adds MQTT Will Properties")]
    public class MqttConnectAddWillProperties : Mutator
    {
        public MqttConnectAddWillProperties(DataElement obj) : base(obj) { }
        public new static bool supportedDataElement(DataElement obj) { return obj is Blob && obj.Name == "will_properties"; }
        public override int count => 1;
        public override uint mutation { get; set; }
        public override void sequentialMutation(DataElement obj) 
        { 
            PerformMutation(obj);
            obj.mutationFlags = MutateOverride.Default; 
        }
        public override void randomMutation(DataElement obj) { sequentialMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
             using(var ms = new MemoryStream())
             using(var bw = new BinaryWriter(ms))
             {
                 int strategy = context.Random.Next(6);
                 switch (strategy)
                 {
                     case 0: bw.Write((byte)0x01); bw.Write((byte)1); break;
                     case 1: bw.Write((byte)0x02); PUT32(bw, (uint)context.Random.Next(3601)); break;
                     case 2: bw.Write((byte)0x18); PUT32(bw, (uint)context.Random.Next(601)); break;
                     case 3: bw.Write((byte)0x03); PUT_UTF8_LIT(bw, "text/plain"); break;
                     case 4: bw.Write((byte)0x08); PUT_UTF8_LIT(bw, "reply/topic"); break;
                     case 5: bw.Write((byte)0x09); PUT_BIN_RAND(bw, 8, 24); break;
                 }
                 // User props
                 string[] keys = {"source", "priority", "note", "device"};
                 string[] vals = {"sensor1", "high", "ok", "edge"};
                 int upn = context.Random.Next(3);
                 for(int t=0; t<upn; ++t) {
                     bw.Write((byte)0x26);
                     PUT_UTF8_LIT(bw, keys[context.Random.Next(4)]);
                     PUT_UTF8_LIT(bw, vals[context.Random.Next(4)]);
                 }
                 obj.MutatedValue = new Variant(ms.ToArray());
             }
        }
        
        private void PUT32(BinaryWriter bw, uint v) { bw.Write((byte)((v>>24)&0xFF)); bw.Write((byte)((v>>16)&0xFF)); bw.Write((byte)((v>>8)&0xFF)); bw.Write((byte)(v&0xFF)); }
        private void PUT16(BinaryWriter bw, ushort v) { bw.Write((byte)((v>>8)&0xFF)); bw.Write((byte)(v&0xFF)); }
        private void PUT_UTF8_LIT(BinaryWriter bw, string s) { byte[] b=SysEncoding.UTF8.GetBytes(s); PUT16(bw, (ushort)b.Length); bw.Write(b); }
        private void PUT_BIN_RAND(BinaryWriter bw, int min, int max) {
            int L = min + context.Random.Next(max - min + 1);
            PUT16(bw, (ushort)L);
            byte[] b = new byte[L];
            new SysRandom().NextBytes(b);
            bw.Write(b);
        }
    }

    [Mutator("MqttConnectDeleteWillProperties")]
    [Description("Deletes MQTT Will Properties")]
    public class MqttConnectDeleteWillProperties : Mutator
    {
        public MqttConnectDeleteWillProperties(DataElement obj) : base(obj) { }
        public new static bool supportedDataElement(DataElement obj) { return obj is Blob && obj.Name == "will_properties"; }
        public override int count => 1;
        public override uint mutation { get; set; }
        public override void sequentialMutation(DataElement obj) { obj.parent.Remove(obj); }
        public override void randomMutation(DataElement obj) { obj.parent.Remove(obj); }
    }

    [Mutator("MqttConnectMutateWillTopic")]
    [Description("Mutates MQTT Will Topic")]
    public class MqttConnectMutateWillTopic : Mutator
    {
        public MqttConnectMutateWillTopic(DataElement obj) : base(obj) { }
        public new static bool supportedDataElement(DataElement obj) { return obj is Peach.Core.Dom.String && obj.Name == "will_topic"; }
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

             switch(strategy)
             {
                 case 0: // 使用预设 topic
                     val = base_topics[context.Random.Next(base_topics.Length)];
                     break;
                 case 1: // 生成合法随机 topic
                     {
                         int len = 1 + context.Random.Next(20); 
                         StringBuilder sb = new StringBuilder();
                         for(int j=0; j<len; ++j) sb.Append(valid_chars[context.Random.Next(valid_chars.Length)]);
                         val = sb.ToString();
                     }
                     break;
                 case 2: // 生成超长 topic
                     {
                         int len = 200 + context.Random.Next(20); 
                         StringBuilder sb = new StringBuilder();
                         for(int j=0; j<len; ++j) sb.Append(valid_chars[context.Random.Next(valid_chars.Length)]);
                         val = sb.ToString();
                     }
                     break;
                 case 3: // 拼接合法 + 非法片段
                     {
                         val = base_topics[context.Random.Next(base_topics.Length)];
                         val += "\xFF"; // Append invalid char
                     }
                     break;
                 case 4: // 插入特殊符号 & bitflip
                     {
                         int len = 1 + context.Random.Next(20);
                         byte[] b = new byte[len];
                         for(int j=0; j<len; ++j) {
                             if (context.Random.Next(4) == 0) b[j] = (byte)'#';
                             else {
                                 b[j] = (byte)valid_chars[context.Random.Next(valid_chars.Length)];
                                 if(context.Random.Next(3)==0) b[j] ^= (byte)(1 << context.Random.Next(8));
                             }
                         }
                         obj.MutatedValue = new Variant(b);
                         return;
                     }
                 case 5: // 全 NULL 字节
                     obj.MutatedValue = new Variant(new byte[20]); 
                     return;
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

    [Mutator("MqttConnectAddWillTopic")]
    [Description("Adds MQTT Will Topic")]
    public class MqttConnectAddWillTopic : Mutator
    {
        public MqttConnectAddWillTopic(DataElement obj) : base(obj) { }
        public new static bool supportedDataElement(DataElement obj) { return obj is Peach.Core.Dom.String && obj.Name == "will_topic"; }
        public override int count => 1;
        public override uint mutation { get; set; }
        public override void sequentialMutation(DataElement obj) 
        { 
            if(string.IsNullOrEmpty((string)obj.DefaultValue)) 
            {
                string[] sample_topics = { "sensor/temp", "a/b/c", "device/+/status", "home/+/light/#", "你好/测试" };
                obj.MutatedValue = new Variant(sample_topics[context.Random.Next(sample_topics.Length)]);
            }
            obj.mutationFlags=MutateOverride.Default; 
        }
        public override void randomMutation(DataElement obj) { sequentialMutation(obj); }
    }

    [Mutator("MqttConnectDeleteWillTopic")]
    [Description("Deletes MQTT Will Topic")]
    public class MqttConnectDeleteWillTopic : Mutator
    {
        public MqttConnectDeleteWillTopic(DataElement obj) : base(obj) { }
        public new static bool supportedDataElement(DataElement obj) { return obj is Peach.Core.Dom.String && obj.Name == "will_topic"; }
        public override int count => 1;
        public override uint mutation { get; set; }
        public override void sequentialMutation(DataElement obj) { obj.parent.Remove(obj); }
        public override void randomMutation(DataElement obj) { obj.parent.Remove(obj); }
    }

    [Mutator("MqttConnectMutateWillPayload")]
    [Description("Mutates MQTT Will Payload")]
    public class MqttConnectMutateWillPayload : Mutator
    {
        public MqttConnectMutateWillPayload(DataElement obj) : base(obj) { }
        public new static bool supportedDataElement(DataElement obj) { return obj is Blob && obj.Name == "will_payload"; }
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

             switch(strategy)
             {
                 case 0: // 合法 UTF-8 文本
                     {
                         int l = 5 + context.Random.Next(20);
                         char[] c = new char[l];
                         for(int j=0;j<l;++j) c[j] = valid_chars[context.Random.Next(valid_chars.Length)];
                         val = SysEncoding.UTF8.GetBytes(new string(c));
                     }
                     break;
                 case 1: // 二进制模式
                     {
                         int l = 1 + context.Random.Next(64);
                         val = new byte[l];
                         new SysRandom().NextBytes(val);
                     }
                     break;
                 case 2: // 空 payload
                     val = new byte[0];
                     break;
                 case 3: // 超长 payload
                     val = new byte[65535]; 
                     new SysRandom().NextBytes(val);
                     break;
                 case 4: // 插入 NULL 字节 + 随机尾随数据
                     {
                         int l = 5 + context.Random.Next(10);
                         val = new byte[l];
                         for(int j=0;j<l;++j) val[j] = (byte)valid_chars[context.Random.Next(valid_chars.Length)];
                         val[context.Random.Next(l)] = 0;
                     }
                     break;
                 case 5: // 非法 UTF-8 序列
                     val = new byte[] { 0xC3, 0x28, 0xA0, 0xA1, 0xE2, 0x28, 0xA1 };
                     break;
                 case 6: // 混合合法文本 + 垃圾二进制
                     {
                         int l = 10 + context.Random.Next(30);
                         int split = context.Random.Next(l);
                         val = new byte[l];
                         for(int j=0;j<split;++j) val[j] = (byte)valid_chars[context.Random.Next(valid_chars.Length)];
                         for(int j=split;j<l;++j) val[j] = (byte)context.Random.Next(256);
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
    }

    [Mutator("MqttConnectAddWillPayload")]
    [Description("Adds MQTT Will Payload")]
    public class MqttConnectAddWillPayload : Mutator
    {
        public MqttConnectAddWillPayload(DataElement obj) : base(obj) { }
        public new static bool supportedDataElement(DataElement obj) { return obj is Blob && obj.Name == "will_payload"; }
        public override int count => 1;
        public override uint mutation { get; set; }
        public override void sequentialMutation(DataElement obj) 
        {
            byte[] b = GetOriginalBytes(obj);
            if(b != null && b.Length==0) 
            {
                string[] samples = { "device offline", "error: timeout", "{\"status\": \"dead\"}", "MQTT last will", "\xDE\xAD\xBE\xEF" };
                string data = samples[context.Random.Next(samples.Length)];
                obj.MutatedValue = new Variant(SysEncoding.UTF8.GetBytes(data));
            }
            obj.mutationFlags=MutateOverride.Default; 
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

    [Mutator("MqttConnectDeleteWillPayload")]
    [Description("Deletes MQTT Will Payload")]
    public class MqttConnectDeleteWillPayload : Mutator
    {
        public MqttConnectDeleteWillPayload(DataElement obj) : base(obj) { }
        public new static bool supportedDataElement(DataElement obj) { return obj is Blob && obj.Name == "will_payload"; }
        public override int count => 1;
        public override uint mutation { get; set; }
        public override void sequentialMutation(DataElement obj) { obj.parent.Remove(obj); }
        public override void randomMutation(DataElement obj) { obj.parent.Remove(obj); }
    }
}
