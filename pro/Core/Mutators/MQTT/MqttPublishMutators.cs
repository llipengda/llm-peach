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

namespace Peach.Pro.Core.Mutators.MQTT
{
    // Packet Identifier Mutators
    [Mutator("MqttPublishMutatePacketIdentifier")]
    [Description("Mutates MQTT Publish Packet ID")]
    public class MqttPublishMutatePacketIdentifier : Mutator
    {
         public MqttPublishMutatePacketIdentifier(DataElement obj) : base(obj) { }
         public new static bool supportedDataElement(DataElement obj)
         {
             return obj is Number && obj.Name == "packet_identifier" &&
                    obj.parent != null && obj.parent.Name.Contains("publish");
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
             uint current = (uint)((Number)obj).DefaultValue;
             switch(strategy) {
                 case 0: val = 0; break;
                 case 1: val = 1; break;
                 case 2: val = 0xFFFF; break;
                 case 3: val = (uint)context.Random.Next(0xFFFF); break;
                 case 4: val = (uint)context.Random.Next(int.MaxValue); break;
                 case 5: val = 0x7FFF; break;
                 case 6: val = 0x8000; break;
                 case 7: val = current ^ 0xAAAA; break;
                 case 8: val = ~current; break;
                 case 9: val = current + 1; break;
             }
             obj.MutatedValue = new Variant(val & 0xFFFF);
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

    [Mutator("MqttPublishAddPacketIdentifier")]
    [Description("Adds (Populates) MQTT Publish Packet ID")]
    public class MqttPublishAddPacketIdentifier : Mutator
    {
        public MqttPublishAddPacketIdentifier(DataElement obj) : base(obj) { }
        public new static bool supportedDataElement(DataElement obj) { return obj is Number && obj.Name == "packet_identifier" && obj.parent != null && obj.parent.Name.Contains("publish"); }
        public override int count => 1;
        public override uint mutation { get; set; }
        public override void sequentialMutation(DataElement obj) { 
             if ((uint)((Number)obj).DefaultValue == 0) obj.MutatedValue = new Variant(1 + context.Random.Next(0xFFFF));
             obj.mutationFlags = MutateOverride.Default; 
        }
        public override void randomMutation(DataElement obj) { sequentialMutation(obj); }
    }

    [Mutator("MqttPublishDeletePacketIdentifier")]
    [Description("Deletes (Clears) MQTT Publish Packet ID")]
    public class MqttPublishDeletePacketIdentifier : Mutator
    {
        public MqttPublishDeletePacketIdentifier(DataElement obj) : base(obj) { }
        public new static bool supportedDataElement(DataElement obj) { return obj is Number && obj.Name == "packet_identifier" && obj.parent != null && obj.parent.Name.Contains("publish"); }
        public override int count => 1;
        public override uint mutation { get; set; }
        public override void sequentialMutation(DataElement obj) { obj.MutatedValue = new Variant(0); obj.mutationFlags = MutateOverride.Default; }
        public override void randomMutation(DataElement obj) { obj.MutatedValue = new Variant(0); obj.mutationFlags = MutateOverride.Default; }
    }

    // Topic Name Mutators
    [Mutator("MqttPublishMutateTopicName")]
    [Description("Mutates MQTT Publish Topic Name")]
    public class MqttPublishMutateTopicName : Mutator
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
            string current = (string)obj.DefaultValue;
            switch(strategy) {
                case 0: val = ""; break;
                case 1: val = "+"; break;
                case 2: val = "#"; break;
                case 3: val = "invalid/#/test#"; break;
                case 4: val = new string('A', 65535 + 10); break;
                case 5: val = "sensor/+/temperature"; break;
                case 6: 
                    byte[] b = new byte[65535]; 
                    new SysRandom().NextBytes(b); 
                    obj.MutatedValue = new Variant(b); 
                    return;
                case 7: val = "home/kitchen/light"; break;
                case 8: val = "topic/!@#$%^&*()"; break;
                case 9: val = "prefix_" + current + "_suffix"; break;
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

    [Mutator("MqttPublishAddTopicName")]
    [Description("Adds (Populates) MQTT Publish Topic Name")]
    public class MqttPublishAddTopicName : Mutator
    {
        public MqttPublishAddTopicName(DataElement obj) : base(obj) { }
        public new static bool supportedDataElement(DataElement obj) { return obj is Peach.Core.Dom.String && obj.Name == "value" && obj.parent != null && obj.parent.Name == "topic_name"; }
        public override int count => 1;
        public override uint mutation { get; set; }
        public override void sequentialMutation(DataElement obj) { 
            if (string.IsNullOrEmpty((string)obj.DefaultValue)) obj.MutatedValue = new Variant("test/topic/added");
            obj.mutationFlags = MutateOverride.Default; 
        }
        public override void randomMutation(DataElement obj) { sequentialMutation(obj); }
    }

    [Mutator("MqttPublishDeleteTopicName")]
    [Description("Deletes (Clears) MQTT Publish Topic Name")]
    public class MqttPublishDeleteTopicName : Mutator
    {
        public MqttPublishDeleteTopicName(DataElement obj) : base(obj) { }
        public new static bool supportedDataElement(DataElement obj) { return obj is Peach.Core.Dom.String && obj.Name == "value" && obj.parent != null && obj.parent.Name == "topic_name"; }
        public override int count => 1;
        public override uint mutation { get; set; }
        public override void sequentialMutation(DataElement obj) { obj.MutatedValue = new Variant(""); obj.mutationFlags = MutateOverride.Default; }
        public override void randomMutation(DataElement obj) { obj.MutatedValue = new Variant(""); obj.mutationFlags = MutateOverride.Default; }
    }

    // Properties Mutators
    [Mutator("MqttPublishMutateProperties")]
    [Description("Mutates MQTT Publish Properties")]
    public class MqttPublishMutateProperties : Mutator
    {
        public MqttPublishMutateProperties(DataElement obj) : base(obj) { }
        public new static bool supportedDataElement(DataElement obj) { return obj is Blob && obj.Name == "properties" && obj.parent != null && obj.parent.Name.Contains("variable_header"); }
        public override int count => 6;
        public override uint mutation { get; set; }
        public override void sequentialMutation(DataElement obj) { PerformMutation(obj, (int)mutation); obj.mutationFlags = MutateOverride.Default; }
        public override void randomMutation(DataElement obj) { PerformMutation(obj, context.Random.Next(count)); obj.mutationFlags = MutateOverride.Default; }
        private void PerformMutation(DataElement obj, int strategy)
        {
             obj.MutatedValue = new Variant(GenerateProperties(strategy));
        }
        private byte[] GenerateProperties(int strategy) {
            using(var ms = new MemoryStream())
            using(var writer = new BinaryWriter(ms)) {
                bool used_pfi=false, used_mei=false, used_ct=false, used_rt=false, used_cd=false, used_ta=false;

                switch(strategy) {
                    case 0: // Clear
                        break;
                    case 1: // PFI=1 + optional CT
                        if (!used_pfi) { writer.Write((byte)0x01); writer.Write((byte)1); used_pfi=true; }
                        if (context.Random.Next(2)!=0 && !used_ct) { WriteUtf8(writer,0x03,"text/plain"); used_ct=true; } 
                        break;
                    case 2: // MEI + optional PFI
                        if (!used_mei) { writer.Write((byte)0x02); WriteBigEndian(writer, (uint)context.Random.Next(7200)); used_mei=true; }
                        if (context.Random.Next(2)!=0 && !used_pfi) { writer.Write((byte)0x01); writer.Write((byte)context.Random.Next(2)); used_pfi=true; } 
                        break;
                    case 3: // TA + optional RT
                        if (!used_ta) { writer.Write((byte)0x23); WriteBigEndian(writer, (ushort)(1+context.Random.Next(100))); used_ta=true; }
                        if (context.Random.Next(2)!=0 && !used_rt) { WriteUtf8(writer,0x08,"reply/topic"); used_rt=true; }
                        break;
                    case 4: // CD + RT
                        if (!used_rt) { WriteUtf8(writer,0x08,"reply/topic"); used_rt=true; }
                        if (!used_cd) { 
                            byte[] tmp=new byte[8 + context.Random.Next(17)]; 
                            new SysRandom().NextBytes(tmp); 
                            writer.Write((byte)0x09); WriteBigEndian(writer,(ushort)tmp.Length); writer.Write(tmp); used_cd=true; 
                        }
                        break;
                    case 5: // Mixed
                        if (!used_pfi && context.Random.Next(2)!=0){ writer.Write((byte)0x01); writer.Write((byte)context.Random.Next(2)); used_pfi=true; }
                        if (!used_mei && context.Random.Next(2)!=0){ writer.Write((byte)0x02); WriteBigEndian(writer, (uint)context.Random.Next(7200)); used_mei=true; }
                        if (!used_ct && context.Random.Next(2)!=0){ WriteUtf8(writer,0x03,"application/json"); used_ct=true; }
                        if (!used_rt && context.Random.Next(2)!=0){ WriteUtf8(writer,0x08,"resp/alpha"); used_rt=true; }
                        if (!used_cd && context.Random.Next(2)!=0){ 
                            byte[] tmp=new byte[6 + context.Random.Next(9)]; 
                            new SysRandom().NextBytes(tmp); 
                            writer.Write((byte)0x09); WriteBigEndian(writer,(ushort)tmp.Length); writer.Write(tmp); used_cd=true;
                        }
                        if (!used_ta && context.Random.Next(2)!=0){ writer.Write((byte)0x23); WriteBigEndian(writer, (ushort)(1+context.Random.Next(100))); used_ta=true; }
                        
                        int upn = context.Random.Next(4);
                        for(int i=0;i<upn;i++) { 
                            writer.Write((byte)0x26); 
                            WriteUtf8Val(writer, (i%2!=0)?"source":"note", (i%2!=0)?"edge":"ok"); 
                        }
                        break;
                }
                return ms.ToArray();
            }
        }
        private void WriteBigEndian(BinaryWriter w, uint v) { w.Write((byte)((v>>24)&0xFF)); w.Write((byte)((v>>16)&0xFF)); w.Write((byte)((v>>8)&0xFF)); w.Write((byte)(v&0xFF)); }
        private void WriteBigEndian(BinaryWriter w, ushort v) { w.Write((byte)((v>>8)&0xFF)); w.Write((byte)(v&0xFF)); }
        private void WriteUtf8(BinaryWriter w, byte id, string s) { w.Write(id); byte[] b=SysEncoding.UTF8.GetBytes(s); WriteBigEndian(w,(ushort)b.Length); w.Write(b); }
        private void WriteUtf8Val(BinaryWriter w, string s) { byte[] b=SysEncoding.UTF8.GetBytes(s); WriteBigEndian(w,(ushort)b.Length); w.Write(b); }
        private void WriteUtf8Val(BinaryWriter w, string k, string v) { WriteUtf8Val(w,k); WriteUtf8Val(w,v); }
    }

    [Mutator("MqttPublishAddProperties")]
    [Description("Adds MQTT Publish Properties")]
    public class MqttPublishAddProperties : Mutator
    {
        public MqttPublishAddProperties(DataElement obj) : base(obj) { }
        public new static bool supportedDataElement(DataElement obj) { return obj is Blob && obj.Name == "properties" && obj.parent != null && obj.parent.Name.Contains("variable_header"); }
        public override int count => 5;
        public override uint mutation { get; set; }
        public override void sequentialMutation(DataElement obj) { PerformMutation(obj, (int)mutation); obj.mutationFlags = MutateOverride.Default; }
        public override void randomMutation(DataElement obj) { PerformMutation(obj, context.Random.Next(count)); obj.mutationFlags = MutateOverride.Default; }
        private void PerformMutation(DataElement obj, int strategy)
        {
             using(var ms = new MemoryStream())
            using(var writer = new BinaryWriter(ms)) {
                byte[] b = GetOriginalBytes(obj);
                if(b != null && b.Length>0) return; // Only if empty

                switch(strategy){
                    case 0: writer.Write((byte)0x01); writer.Write((byte)1); break;
                    case 1: writer.Write((byte)0x02); WriteBigEndian(writer, (uint)context.Random.Next(3601)); break;
                    case 2: WriteUtf8(writer,0x03,"text/plain"); break;
                    case 3: writer.Write((byte)0x23); WriteBigEndian(writer, (ushort)(1+context.Random.Next(100))); break;
                    case 4: writer.Write((byte)0x26); WriteUtf8Val(writer,"key"); WriteUtf8Val(writer,"value"); break;
                }
                obj.MutatedValue = new Variant(ms.ToArray());
            }
        }
        private void WriteBigEndian(BinaryWriter w, uint v) { w.Write((byte)((v>>24)&0xFF)); w.Write((byte)((v>>16)&0xFF)); w.Write((byte)((v>>8)&0xFF)); w.Write((byte)(v&0xFF)); }
        private void WriteBigEndian(BinaryWriter w, ushort v) { w.Write((byte)((v>>8)&0xFF)); w.Write((byte)(v&0xFF)); }
        private void WriteUtf8(BinaryWriter w, byte id, string s) { w.Write(id); byte[] b=SysEncoding.UTF8.GetBytes(s); WriteBigEndian(w,(ushort)b.Length); w.Write(b); }
        private void WriteUtf8Val(BinaryWriter w, string s) { byte[] b=SysEncoding.UTF8.GetBytes(s); WriteBigEndian(w,(ushort)b.Length); w.Write(b); }
        
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

    [Mutator("MqttPublishDeleteProperties")]
    [Description("Deletes MQTT Publish Properties")]
    public class MqttPublishDeleteProperties : Mutator
    {
        public MqttPublishDeleteProperties(DataElement obj) : base(obj) { }
        public new static bool supportedDataElement(DataElement obj) { return obj is Blob && obj.Name == "properties" && obj.parent != null && obj.parent.Name.Contains("variable_header"); }
        public override int count => 1;
        public override uint mutation { get; set; }
        public override void sequentialMutation(DataElement obj) { obj.parent.Remove(obj); }
        public override void randomMutation(DataElement obj) { obj.parent.Remove(obj); }
    }

    [Mutator("MqttPublishRepeatProperties")]
    [Description("Repeats MQTT Publish User Property")]
    public class MqttPublishRepeatProperties : Mutator
    {
        public MqttPublishRepeatProperties(DataElement obj) : base(obj) { }
        public new static bool supportedDataElement(DataElement obj) { return obj is Blob && obj.Name == "properties" && obj.parent != null && obj.parent.Name.Contains("variable_header"); }
        public override int count => 1;
        public override uint mutation { get; set; }
        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); obj.mutationFlags = MutateOverride.Default; }
        public override void randomMutation(DataElement obj) { PerformMutation(obj); obj.mutationFlags = MutateOverride.Default; }
        private void PerformMutation(DataElement obj) {
            byte[] original = GetOriginalBytes(obj);
            if(original == null || original.Length == 0) return;
            int pos = 0;
            while(pos < original.Length) {
                byte id = original[pos];
                if(id == 0x26) {
                    // User Prop
                    int start = pos; pos++;
                    if(pos+2>original.Length) break;
                    int klen = (original[pos]<<8)|original[pos+1]; pos+=2+klen;
                    if(pos+2>original.Length) break;
                    int vlen = (original[pos]<<8)|original[pos+1]; pos+=2+vlen;
                    if(pos<=original.Length) {
                        using(var ms=new MemoryStream()){
                            ms.Write(original,0,original.Length);
                            ms.Write(original,start,pos-start);
                            obj.MutatedValue = new Variant(ms.ToArray());
                        }
                    }
                    break;
                }
                // Simple skip logic
                else if(id==0x01) { if(pos+2>original.Length)break; pos+=2; }
                else if(id==0x02) { if(pos+5>original.Length)break; pos+=5; }
                else if(id==0x23) { if(pos+3>original.Length)break; pos+=3; }
                else if(id==0x03 || id==0x08 || id==0x09 || id==0x15 || id==0x16) {
                    if(pos+3>original.Length)break;
                    int l = (original[pos+1]<<8)|original[pos+2]; 
                    if(pos+3+l>original.Length)break;
                    pos+=3+l;
                }
                else break; // Unknown
            }
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

    // Payload Mutators
    [Mutator("MqttPublishMutatePayload")]
    [Description("Mutates MQTT Publish Payload")]
    public class MqttPublishMutatePayload : Mutator
    {
        public MqttPublishMutatePayload(DataElement obj) : base(obj) { }
        public new static bool supportedDataElement(DataElement obj) { return obj is Blob && obj.Name == "payload" && obj.parent != null && obj.parent.Name.Contains("publish"); }
        public override int count => 10;
        public override uint mutation { get; set; }
        public override void sequentialMutation(DataElement obj) { PerformMutation(obj, (int)mutation); obj.mutationFlags = MutateOverride.Default; }
        public override void randomMutation(DataElement obj) 
        {
            int[] weights = { 40, 40, 0, 0, 0, 0, 0, 0, 0, 0 };
            PerformMutation(obj, PickWeighted(weights)); 
            obj.mutationFlags = MutateOverride.Default;
        }
        
        private void PerformMutation(DataElement obj, int strategy) {
            byte[] d = null;
            switch(strategy) {
                case 0: d=new byte[0]; break;
                case 1: d=new byte[65535]; for(int i=0;i<d.Length;i++) d[i]=(byte)'A'; break;
                case 2: d=new byte[context.Random.Next(65535)]; new SysRandom().NextBytes(d); break;
                case 3: d=SysEncoding.UTF8.GetBytes("msg_"+context.Random.Next(1000)); break;
                case 4: d=new byte[16]; for(int i=0;i<16;i++) d[i]=0xFF; break;
                case 5: d=SysEncoding.UTF8.GetBytes("{\"key\":\"val"+context.Random.Next(100)+"\"}"); break;
                case 6: d=new byte[70000]; break; 
                case 7: d=new byte[]{0xC0,0x00}; break;
                case 8: d=SysEncoding.UTF8.GetBytes("topic/name"); break;
                case 9: 
                     byte[] orig = GetOriginalBytes(obj);
                     if(orig!=null && orig.Length>0 && orig.Length*2 < 65535) {
                         d=new byte[orig.Length*2]; SysArray.Copy(orig,0,d,0,orig.Length); SysArray.Copy(orig,0,d,orig.Length,orig.Length);
                     } else d=new byte[10];
                     break;
            }
            if(d==null) d=new byte[0];
            obj.MutatedValue=new Variant(d);
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

    [Mutator("MqttPublishAddPayload")]
    [Description("Adds MQTT Publish Payload")]
    public class MqttPublishAddPayload : Mutator
    {
        public MqttPublishAddPayload(DataElement obj) : base(obj) { }
        public new static bool supportedDataElement(DataElement obj) { return obj is Blob && obj.Name == "payload" && obj.parent != null && obj.parent.Name.Contains("publish"); }
        public override int count => 1;
        public override uint mutation { get; set; }
        public override void sequentialMutation(DataElement obj) { 
            byte[] b = GetOriginalBytes(obj);
            if(b != null && b.Length==0) obj.MutatedValue = new Variant(SysEncoding.UTF8.GetBytes("hello"));
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

    [Mutator("MqttPublishDeletePayload")]
    [Description("Deletes MQTT Publish Payload")]
    public class MqttPublishDeletePayload : Mutator
    {
        public MqttPublishDeletePayload(DataElement obj) : base(obj) { }
        public new static bool supportedDataElement(DataElement obj) { return obj is Blob && obj.Name == "payload" && obj.parent != null && obj.parent.Name.Contains("publish"); }
        public override int count => 1;
        public override uint mutation { get; set; }
        public override void sequentialMutation(DataElement obj) { obj.parent.Remove(obj); }
        public override void randomMutation(DataElement obj) { obj.parent.Remove(obj); }
    }

    // Flags Mutators
    [Mutator("MqttPublishMutateQoS")]
    [Description("Mutates MQTT Publish QoS")]
    public class MqttPublishMutateQoS : Mutator
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

        private void PerformMutation(DataElement obj, int strategy) {
            uint val = 0;
            uint current = (uint)((Number)obj).DefaultValue;
            switch(strategy){
                case 0: val=0; break;
                case 1: val=1; break;
                case 2: val=2; break;
                case 3: val=3; break; // Illegal
                case 4: val=255; break; // Extreme illegal
                case 5: val=(uint)context.Random.Next(256); break;
                case 6: val=(current + 1) % 4; break;
                case 7: val= 0xFF & ~current; break;
                case 8: val=(context.Random.Next(10)==0)?4u:2u; break;
                case 9: val=0xAA; break;
            }
            obj.MutatedValue = new Variant(val & 0x3);
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

    [Mutator("MqttPublishMutateDup")]
    [Description("Mutates MQTT Publish Dup")]
    public class MqttPublishMutateDup : Mutator
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

        private void PerformMutation(DataElement obj, int strategy) {
            uint val = 0;
            uint current = (uint)((Number)obj).DefaultValue;
            switch(strategy){
                case 0: val=0; break;
                case 1: val=1; break;
                case 2: val = (current == 0) ? 1u : 0u; break;
                case 3: val=2; break;
                case 4: val=255; break;
                case 5: val=(uint)context.Random.Next(256); break;
                case 6: val=current ^ 1; break;
                case 7: val=0xAA; break;
                case 8: val=(uint)((context.Random.Next(2)==0)?0:1); break;
                case 9: val=(uint)(current + context.Random.Next(3)); break;
            }
            obj.MutatedValue = new Variant(val & 1);
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

    [Mutator("MqttPublishMutateRetain")]
    [Description("Mutates MQTT Publish Retain")]
    public class MqttPublishMutateRetain : Mutator
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

        private void PerformMutation(DataElement obj, int strategy) {
             uint val = 0;
             uint current = (uint)((Number)obj).DefaultValue;
             switch(strategy){
                 case 0: val=0; break;
                 case 1: val=1; break;
                 case 2: val = current ^ 1; break;
                 case 3: val=2; break;
                 case 4: val=255; break;
                 case 5: val=(uint)context.Random.Next(256); break;
                 case 6: val=0xFF; break;
                 case 7: val=1; break; // Logic in C uses qos check, here we assume 1
                 case 8: val=current+1; break;
                 case 9: val=(uint)(context.Random.Next(2)*3); break;
             }
             obj.MutatedValue = new Variant(val & 1);
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
}
