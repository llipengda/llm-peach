using System;
using System.IO;
using System.Linq;
using Peach.Core.Dom;
using Peach.Core;
using Peach.Pro.Core.Mutators;
using Peach.LLM.Core.Mutators.DTLS;
using System.ComponentModel;

namespace Peach.LLM.Core.Mutators.DTLS
{
    [Mutator("DtlsMutateHandshakeHeaderMsgType")]
    [CMutator("mutate_handshake_header_msg_type")]
    [Description("Mutates the Handshake Message Type.")]
    public class DtlsMutateHandshakeHeaderMsgType : LLMMutator
    {
        public DtlsMutateHandshakeHeaderMsgType(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj is Number && obj.Name == "msg_type" &&
                   (obj.IsIn("client_hello") || obj.IsIn("server_hello") || obj.IsIn("handshake_messages"));
        }

        public override int count => 1;
        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }
        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        protected override void PerformMutation(DataElement obj)
        {
            // Logic derived from C function: mutate_handshake_header_msg_type
            var num = obj as Number;
            byte cur = (byte)(ulong)num.InternalValue;
            byte val = cur;
            uint cat = DtlsUtils.RndU32(100);

            if (cat < 18) { /* canonical */ }
            else if (cat < 30)
            {
                byte[] bnd = { 0, 1, 20, 255 };
                val = bnd[DtlsUtils.RndU32((uint)bnd.Length)];
            }
            else if (cat < 46)
            {
                val = DtlsUtils.PickCommonHsType();
            }
            else if (cat < 66)
            {
                val = DtlsUtils.PickCommonHsType();
            }
            else if (cat < 74)
            {
                int rot = (int)DtlsUtils.RndU32(7) + 1;
                val = (byte)((cur << rot) | (cur >> (8 - rot)));
            }
            else if (cat < 92)
            {
                byte delta = (byte)(1 + DtlsUtils.RndU32(5));
                val = (DtlsUtils.RndU32(2) == 0) ? (byte)(cur + delta) : (byte)(cur - delta);
            }
            else
            {
                val = (byte)DtlsUtils.RndU32(256);
            }

            num.MutatedValue = new Variant((ulong)val);
        }
    }

    [Mutator("DtlsMutateHandshakeHeaderLength")]
    [CMutator("mutate_handshake_header_length")]
    [Description("Mutates the Handshake Message Length (24-bit).")]
    public class DtlsMutateHandshakeHeaderLength : LLMMutator
    {
        public DtlsMutateHandshakeHeaderLength(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj is Number && obj.Name == "length" && obj.lengthAsBits == 24 && obj.parent != null &&
                   (obj.parent.Name.EndsWith("_hello") || obj.parent.Name == "certificate"); // Heuristic for handshake msgs
        }

        public override int count => 1;
        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }
        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        protected override void PerformMutation(DataElement obj)
        {
            var num = obj as Number;
            uint cur = (uint)(ulong)num.InternalValue;
            uint val = cur;
            uint cat = DtlsUtils.RndU32(100);

            // C logic mirrors record length logic but 24-bit
            if (cat < 20) { /* canonical */ }
            else if (cat < 34)
            {
                uint[] bnd = { 0, 1, 12, 0xFFFFFF };
                val = bnd[DtlsUtils.RndU32((uint)bnd.Length)];
            }
            else if (cat < 70)
            {
                val = DtlsUtils.RndU32(0x1000000); // 24-bit max
            }
            else if (cat < 78)
            {
                // shape: byte flip
                uint mask = 1u << (int)DtlsUtils.RndU32(24);
                val ^= mask;
            }
            else
            {
                val = (uint)(DtlsUtils.RndU32(0x10000) + cur); // sweep
            }

            num.MutatedValue = new Variant((ulong)(val & 0xFFFFFF));
        }
    }

    [Mutator("DtlsMutateHandshakeHeaderMessageSeq")]
    [CMutator("mutate_handshake_header_message_seq")]
    [Description("Mutates the Handshake Message Sequence.")]
    public class DtlsMutateHandshakeHeaderMessageSeq : LLMMutator
    {
        public DtlsMutateHandshakeHeaderMessageSeq(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj is Number && obj.Name == "message_seq";
        }

        public override int count => 1;
        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }
        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        protected override void PerformMutation(DataElement obj)
        {
            var num = obj as Number;
            ushort cur = (ushort)(ulong)num.InternalValue;
            ushort val = cur;
            uint cat = DtlsUtils.RndU32(100);

            if (cat < 16) val = (ushort)((DtlsUtils.RndU32(2) == 0) ? 0 : 1);
            else if (cat < 30) val = (ushort)DtlsUtils.RndU32(0xFFFF);
            else if (cat < 66) val = (ushort)DtlsUtils.RndU32(65536);
            else if (cat < 76) val = (ushort)((cur << 8) | (cur >> 8));
            else if (cat < 92) val = (ushort)(cur + 1);
            else val = (ushort)DtlsUtils.RndU32(256);

            num.MutatedValue = new Variant((ulong)val);
        }
    }

    [Mutator("DtlsMutateHandshakeHeaderFragmentOffset")]
    [CMutator("mutate_handshake_header_fragment_offset")]
    [Description("Mutates the Handshake Fragment Offset.")]
    public class DtlsMutateHandshakeHeaderFragmentOffset : LLMMutator
    {
        public DtlsMutateHandshakeHeaderFragmentOffset(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj is Number && obj.Name == "fragment_offset";
        }

        public override int count => 1;
        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }
        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        protected override void PerformMutation(DataElement obj)
        {
            var num = obj as Number;
            uint cur = (uint)(ulong)num.InternalValue;
            uint val = cur;
            uint cat = DtlsUtils.RndU32(100);

            if (cat < 20) val = 0;
            else if (cat < 34) val = DtlsUtils.RndU32(1024);
            else if (cat < 70) val = DtlsUtils.RndU32(0xFFFFFF);
            else val = cur ^ (1u << (int)DtlsUtils.RndU32(24));

            num.MutatedValue = new Variant((ulong)(val & 0xFFFFFF));
        }
    }

    [Mutator("DtlsMutateHandshakeHeaderFragmentLength")]
    [CMutator("mutate_handshake_header_fragment_length")]
    [Description("Mutates the Handshake Fragment Length.")]
    public class DtlsMutateHandshakeHeaderFragmentLength : LLMMutator
    {
        public DtlsMutateHandshakeHeaderFragmentLength(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj is Number && obj.Name == "fragment_length";
        }

        public override int count => 1;
        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }
        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        protected override void PerformMutation(DataElement obj)
        {
            var num = obj as Number;
            uint cur = (uint)(ulong)num.InternalValue;
            uint val = cur;
            uint cat = DtlsUtils.RndU32(100);

            if (cat < 16) { /* canonical: usually matches length */ }
            else if (cat < 30) val = DtlsUtils.RndU32(1024);
            else if (cat < 66) val = DtlsUtils.RndU32(0xFFFFFF);
            else val = cur ^ (1u << (int)DtlsUtils.RndU32(24));

            num.MutatedValue = new Variant((ulong)(val & 0xFFFFFF));
        }
    }
}