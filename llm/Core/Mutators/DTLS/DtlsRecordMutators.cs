using System;
using System.IO;
using System.Linq;
using Peach.Core.Dom;
using Peach.Core;
using Peach.Pro.Core.Mutators;
using Peach.LLM.Core.Mutators.DTLS;
using System.ComponentModel;
using Array = System.Array;

namespace Peach.LLM.Core.Mutators.DTLS
{
    [Mutator("DtlsMutateRecordHeaderEpoch")]
    [CMutator("mutate_record_header_epoch")]
    [Description("Mutates the Epoch field in the DTLS Record Header.")]
    public class DtlsMutateRecordHeaderEpoch : LLMMutator
    {
        public DtlsMutateRecordHeaderEpoch(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj is Number && obj.Name == "epoch" && obj.IsIn("header_common");
        }

        public override int count => 1;
        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }
        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        protected override void PerformMutation(DataElement obj)
        {
            var num = obj as Number;
            ushort cur = (ushort)(ulong)num.InternalValue;

            // Logic derived from C function: mutate_record_header_epoch
            // Categories A-H implemented via DtlsUtils or direct logic
            uint cat = DtlsUtils.UrandEpoch(100);
            ushort val = cur;

            if (cat < 18) val = 0;
            else if (cat < 33) val = (DtlsUtils.UrandEpoch(2) == 0) ? (ushort)0 : (ushort)0xFFFF;
            else if (cat < 48)
            {
                uint pick = DtlsUtils.UrandEpoch(4);
                if (pick == 0) val = 0;
                else if (pick == 1) val = 1;
                else if (pick == 2) val = 0; // Hint logic omitted, defaulting 0
                else val = cur;
            }
            else if (cat < 63)
            {
                ushort[] allowed = { 0, 1, 2, 3, 4, 5, 8, 16, 32, 64 };
                val = allowed[DtlsUtils.UrandEpoch((uint)allowed.Length)];
            }
            else if (cat < 70)
            {
                ushort bit = (ushort)(1u << (int)DtlsUtils.UrandEpoch(16));
                val = (ushort)(cur ^ bit);
            }
            else if (cat < 76) val = cur;
            else if (cat < 90)
            {
                ushort baseVal = (cur != 0) ? cur : (ushort)0;
                ushort delta = (ushort)DtlsUtils.UrandEpoch(7);
                int dir = (DtlsUtils.UrandEpoch(2) == 0) ? -1 : 1;
                val = DtlsUtils.ClampU16((uint)(baseVal + (dir * delta)));
            }
            else
            {
                // Mode H
                val = (ushort)(DtlsUtils.RndU32(0xFFFF));
            }

            // Perturbations
            if (DtlsUtils.UrandEpoch(100) < 8)
            {
                ushort mask = (ushort)(DtlsUtils.RndU32(0xFFFF));
                val ^= mask;
            }

            num.MutatedValue = new Variant((ulong)val);
        }
    }

    [Mutator("DtlsMutateRecordHeaderSequenceNumber")]
    [CMutator("mutate_record_header_sequence_number")]
    [Description("Mutates the Sequence Number field in the DTLS Record Header.")]
    public class DtlsMutateRecordHeaderSequenceNumber : LLMMutator
    {
        public DtlsMutateRecordHeaderSequenceNumber(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj is Number && obj.Name == "sequence_number" && obj.IsIn("header_common");
        }

        public override int count => 1;
        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }
        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        protected override void PerformMutation(DataElement obj)
        {
            // Logic derived from C function: mutate_record_header_sequence_number
            // sequence_number is 48-bit (6 bytes), represented as Number in PIT
            var num = obj as Number;
            ulong cur = (ulong)num.InternalValue;

            // Minimal RNG state for seq
            uint rnd = DtlsUtils.RndU32(100);
            ulong val = cur;

            if (rnd < 16)
            {
                val = (ulong)(DtlsUtils.RndU32(32));
            }
            else if (rnd < 28)
            {
                val = (DtlsUtils.RndU32(2) == 0) ? 0 : 0xFFFFFFFFFFFFUL;
            }
            else if (rnd < 44)
            {
                val = cur; // C logic uses neighbor/base which implies state, keeping simple
            }
            else if (rnd < 60)
            {
                val = (ulong)DtlsUtils.RndU32(0xFFFF);
            }
            else if (rnd < 70)
            {
                // Byte swap emulation
                byte[] bytes = BitConverter.GetBytes(cur);
                if (BitConverter.IsLittleEndian) Array.Reverse(bytes);
                // taking last 6 bytes
                byte[] b6 = new byte[6];
                Array.Copy(bytes, bytes.Length - 6, b6, 0, 6);

                uint a = DtlsUtils.RndU32(6);
                uint b = DtlsUtils.RndU32(6);
                byte t = b6[a]; b6[a] = b6[b]; b6[b] = t;

                // reconstruct
                val = 0;
                for (int i = 0; i < 6; i++) val = (val << 8) | b6[i];
            }
            else if (rnd < 90)
            {
                ulong delta = (ulong)DtlsUtils.RndU32(33);
                val = (DtlsUtils.RndU32(2) == 0) ? cur - delta : cur + delta;
            }
            else
            {
                val = ((ulong)DtlsUtils.RndU32(0xFFFFFF) << 24) ^ (ulong)DtlsUtils.RndU32(0xFFFFFF);
            }

            val &= 0xFFFFFFFFFFFFUL;
            num.MutatedValue = new Variant(val);
        }
    }

    [Mutator("DtlsMutateRecordHeaderLength")]
    [CMutator("mutate_record_header_length")]
    [Description("Mutates the Length field in the DTLS Record Header.")]
    public class DtlsMutateRecordHeaderLength : LLMMutator
    {
        public DtlsMutateRecordHeaderLength(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            // Target the 'length' field that is a sibling of 'header_common' inside 'dtls_record' choices
            return obj is Number && obj.Name == "length" && (
                   obj.parent.Name == "handshake_record" ||
                   obj.parent.Name == "ccs_record" ||
                   obj.parent.Name == "alert_record" ||
                   obj.parent.Name == "app_data_record");
        }

        public override int count => 1;
        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }
        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        protected override void PerformMutation(DataElement obj)
        {
            var num = obj as Number;
            ushort cur = (ushort)(ulong)num.InternalValue;

            // Logic derived from C function: mutate_record_header_length
            uint cat = DtlsUtils.RndU32(100);
            ushort val = cur;

            if (cat < 22) { /* Canonical: handled by relation usually, assume cur is canonical */ }
            else if (cat < 34)
            {
                ushort[] bnd = { 0, 13, 12, 0xFFFF };
                val = bnd[DtlsUtils.RndU32((uint)bnd.Length)];
            }
            else if (cat < 49)
            {
                uint pick = DtlsUtils.RndU32(5);
                if (pick == 1) val++;
                else if (pick == 2) val--;
            }
            else if (cat < 64)
            {
                val = (ushort)(val + DtlsUtils.RndU32(64));
            }
            else if (cat < 72)
            {
                val = (ushort)((val >> 8) | (val << 8));
            }
            else if (cat < 78)
            {
                uint m = 1u << (int)(1 + DtlsUtils.RndU32(3));
                val = (ushort)((val + (m - 1)) & ~(m - 1));
            }
            else if (cat < 92)
            {
                ushort delta = (ushort)DtlsUtils.RndU32(33);
                val = (DtlsUtils.RndU32(2) == 0) ? (ushort)(val - delta) : (ushort)(val + delta);
            }
            else
            {
                val = (ushort)DtlsUtils.RndU32(0xFFFF);
            }

            num.MutatedValue = new Variant((ulong)val);
        }
    }
}