using System;
using System.IO;
using System.Linq;
using Peach.Core.Dom;
using Peach.Core;
using Peach.Pro.Core.Mutators;
using Peach.LLM.Core.Mutators.DTLS;
using System.ComponentModel;
using Array = System.Array;

using static Peach.LLM.Core.Mutators.DTLS.DtlsUtils;

namespace Peach.LLM.Core.Mutators.DTLS
{
    [Mutator("DtlsMutateClientHelloClientVersion")]
    [CMutator("mutate_client_hello_client_version")]
    [Description("Mutates the ClientHello client_version.")]
    public class DtlsMutateClientHelloClientVersion : LLMMutator
    {
        public DtlsMutateClientHelloClientVersion(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj is Block && obj.Name == "client_version" && obj.IsIn("client_hello");
        }

        public override int count => 1;
        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }
        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        protected override void PerformMutation(DataElement obj)
        {
            var blk = obj as Block;
            var majElem = blk.find("major") as Number;
            var minElem = blk.find("minor") as Number;

            byte maj = (byte)(ulong)majElem.InternalValue;
            byte min = (byte)(ulong)minElem.InternalValue;

            uint cat = DtlsUtils.RndU32(100);
            if (cat < 18) { maj = 0xFE; min = 0xFD; } // DTLS 1.2
            else if (cat < 32)
            {
                uint r = DtlsUtils.RndU32(6);
                if (r == 0) { maj = 0; min = 0; }
                else if (r == 1) { maj = 0xFF; min = 0xFF; }
                else if (r == 2) { maj = 0xFE; min = 0; }
                else if (r == 3) { maj = 0xFE; min = 0xFF; }
                else { maj = 0x03; min = 0x03; }
            }
            else if (cat < 68)
            {
                maj = 0xFE;
                uint r = DtlsUtils.RndU32(8);
                if (r == 0)
                    min = 0xFD;
                else if (r == 1)
                    min = 0xFF;
                else
                    min = (byte)r;
            }
            else
            {
                maj = (byte)DtlsUtils.RndU32(256);
                min = (byte)DtlsUtils.RndU32(256);
            }

            majElem.MutatedValue = new Variant((ulong)maj);
            minElem.MutatedValue = new Variant((ulong)min);
        }
    }

    [Mutator("DtlsMutateClientHelloRandom")]
    [CMutator("mutate_client_hello_random")]
    [Description("Mutates the ClientHello Random field.")]
    public class DtlsMutateClientHelloRandom : LLMMutator
    {
        public DtlsMutateClientHelloRandom(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj is Blob && obj.Name == "random" && obj.IsIn("client_hello");
        }

        public override int count => 1;
        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }
        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        protected override void PerformMutation(DataElement obj)
        {
            var blob = obj as Blob;
            byte[] cur = blob.Bytes();
            if (cur.Length != 32) cur = new byte[32];

            // Collect observed randoms for equivalence-class alternatives
            // (In C# we can't easily collect from other packets, so we'll use current value)
            byte[] seen = new byte[32];
            Array.Copy(cur, seen, 32);
            bool hasSeen = cur.Length == 32;

            byte[] out_val = new byte[32];
            Array.Copy(cur, out_val, 32);

            uint cat = DtlsUtils.UrandChr(100);

            if (cat < 10)
            {
                // A. Canonical form: time-like prefix (4 bytes) + randomness (rest)
                uint t = DtlsUtils.Xs32Chr();
                out_val[0] = (byte)(t >> 24);
                out_val[1] = (byte)(t >> 16);
                out_val[2] = (byte)(t >> 8);
                out_val[3] = (byte)t;
                // Fill remaining 28 bytes with random
                for (int i = 4; i < 32; i++)
                {
                    out_val[i] = (byte)DtlsUtils.UrandChr(256);
                }
            }
            else if (cat < 22)
            {
                // B. Boundaries: all-0, all-FF, alternating patterns
                switch (DtlsUtils.UrandChr(4))
                {
                    case 0:
                        for (int k = 0; k < 32; k++) out_val[k] = 0x00;
                        break;
                    case 1:
                        for (int k = 0; k < 32; k++) out_val[k] = 0xFF;
                        break;
                    case 2:
                        for (int k = 0; k < 32; k++) out_val[k] = (byte)((k & 1) != 0 ? 0x00 : 0xFF);
                        break;
                    default:
                        for (int k = 0; k < 32; k++) out_val[k] = (byte)((k & 1) != 0 ? 0xAA : 0x55);
                        break;
                }
            }
            else if (cat < 34)
            {
                // C. Equivalence-class alternatives: reuse from another seed/packet
                if (hasSeen)
                {
                    Array.Copy(seen, out_val, 32);
                }
                else
                {
                    DtlsUtils.FillBytes(out_val, 32);
                }
            }
            else if (cat < 52)
            {
                // D. Allowed range: any 32 bytes; bias to "mostly random" with a few fixed bytes
                DtlsUtils.FillBytes(out_val, 32);
                // keep some bytes stable to model partial invariants
                if (DtlsUtils.UrandChr(2) == 0) { out_val[0] = cur[0]; out_val[1] = cur[1]; }
                if (DtlsUtils.UrandChr(3) == 0) { out_val[31] = cur[31]; }
            }
            else if (cat < 66)
            {
                // E. Encoding-shape variant: reorder / mirrored / rotate (still 32 bytes)
                switch (DtlsUtils.UrandChr(4))
                {
                    case 0:
                        DtlsUtils.RotlBytes1(out_val, 32);
                        break;
                    case 1:
                        // reverse
                        for (int k = 0; k < 16; k++)
                        {
                            byte t = out_val[k];
                            out_val[k] = out_val[31 - k];
                            out_val[31 - k] = t;
                        }
                        break;
                    case 2:
                        DtlsUtils.SwapHalves(out_val, 32);
                        break;
                    default:
                        // interleave halves
                        byte[] tmp = new byte[32];
                        for (int k = 0; k < 16; k++)
                        {
                            tmp[2 * k] = out_val[k];
                            tmp[2 * k + 1] = out_val[16 + k];
                        }
                        Array.Copy(tmp, out_val, 32);
                        break;
                }
            }
            else if (cat < 74)
            {
                // F. Padding/alignment: not meaningful; simulate by forcing nibble alignment
                for (int k = 0; k < 32; k++)
                {
                    out_val[k] = (byte)(out_val[k] & (DtlsUtils.UrandChr(2) != 0 ? 0xF0u : 0x0Fu));
                    if (DtlsUtils.UrandChr(3) == 0) out_val[k] |= (byte)(DtlsUtils.UrandChr(16));
                }
            }
            else if (cat < 86)
            {
                // G. In-range sweep: tweak a contiguous window deterministically
                uint start = DtlsUtils.UrandChr(32);
                uint win = 1u + DtlsUtils.UrandChr(8);
                if (start + win > 32) win = 32 - start;
                byte base_byte = (byte)DtlsUtils.UrandChr(256);
                for (uint k = 0; k < win; k++)
                {
                    out_val[start + k] = (byte)(base_byte + k);
                }
            }
            else
            {
                // H. Random valid mix
                DtlsUtils.FillBytes(out_val, 32);
                if (hasSeen && DtlsUtils.UrandChr(3) == 0)
                {
                    // splice with an observed one
                    uint cut = DtlsUtils.UrandChr(33);
                    Array.Copy(seen, 0, out_val, 0, (int)cut);
                }
                if (DtlsUtils.UrandChr(4) == 0)
                {
                    // sprinkle a small structured prefix
                    out_val[0] = 0xDE; out_val[1] = 0xAD; out_val[2] = 0xBE; out_val[3] = 0xEF;
                }
            }

            // Randomized perturbations: shallow + deep
            if (DtlsUtils.UrandChr(100) < 22)
            {
                // shallow: flip a few bytes
                uint flips = 1u + DtlsUtils.UrandChr(4);
                for (uint f = 0; f < flips; f++)
                {
                    uint idx = DtlsUtils.UrandChr(32);
                    out_val[idx] ^= (byte)(1u << (int)DtlsUtils.UrandChr(8));
                }
            }
            if (DtlsUtils.UrandChr(100) < 10)
            {
                // deep: xor-mask whole block or rotate
                if (DtlsUtils.UrandChr(2) == 0)
                    DtlsUtils.XorBytes(out_val, 32, (byte)(1u + DtlsUtils.UrandChr(255)));
                else
                    DtlsUtils.RotlBytes1(out_val, 32);
            }

            blob.MutatedValue = new Variant(out_val);
        }
    }

    [Mutator("DtlsAddClientHelloSessionId")]
    [CMutator("add_client_hello_session_id")]
    [Description("Adds a Session ID to ClientHello if empty.")]
    public class DtlsAddClientHelloSessionId : LLMMutator
    {
        public DtlsAddClientHelloSessionId(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj is Blob && obj.Name == "id" && obj.parent.Name == "session_id" && obj.IsIn("client_hello");
        }

        public override int count => 1;
        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }
        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        protected override void PerformMutation(DataElement obj)
        {
            var blob = obj as Blob;
            if (blob.Value.Length == 0)
            {
                // Add canonical 32 bytes
                byte[] val = new byte[32];
                NextBytes(val);
                blob.MutatedValue = new Variant(val);
            }
        }
    }

    [Mutator("DtlsDeleteClientHelloSessionId")]
    [CMutator("delete_client_hello_session_id")]
    [Description("Deletes the Session ID from ClientHello.")]
    public class DtlsDeleteClientHelloSessionId : LLMMutator
    {
        public DtlsDeleteClientHelloSessionId(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj is Blob && obj.Name == "id" && obj.parent.Name == "session_id" && obj.IsIn("client_hello");
        }

        public override int count => 1;
        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }
        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        protected override void PerformMutation(DataElement obj)
        {
            var blob = obj as Blob;
            blob.MutatedValue = new Variant(new byte[0]);
        }
    }

    [Mutator("DtlsMutateClientHelloSessionId")]
    [CMutator("mutate_client_hello_session_id")]
    [Description("Mutates the Session ID in ClientHello.")]
    public class DtlsMutateClientHelloSessionId : LLMMutator
    {
        public DtlsMutateClientHelloSessionId(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj is Blob && obj.Name == "id" && obj.parent.Name == "session_id" && obj.IsIn("client_hello");
        }

        public override int count => 1;
        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }
        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        protected override void PerformMutation(DataElement obj)
        {
            var blob = obj as Blob;
            byte[] cur = blob.Bytes();

            uint cat = DtlsUtils.RndU32(100);
            int len = cur.Length;

            if (cat < 12)
            {
                // A: empty or full
                if (DtlsUtils.RndU32(2) == 0) len = 0;
                else len = 32;
            }
            else if (cat < 56)
            {
                len = (int)DtlsUtils.RndU32(33); // 0..32
            }

            byte[] newVal = new byte[len];
            NextBytes(newVal);
            blob.MutatedValue = new Variant(newVal);
        }
    }

    [Mutator("DtlsAddClientHelloCookie")]
    [CMutator("add_client_hello_cookie")]
    [Description("Adds a Cookie to ClientHello if empty.")]
    public class DtlsAddClientHelloCookie : LLMMutator
    {
        public DtlsAddClientHelloCookie(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj is Blob && obj.Name == "cookie" && obj.IsIn("client_hello");
        }

        public override int count => 1;
        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }
        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        protected override void PerformMutation(DataElement obj)
        {
            var blob = obj as Blob;
            if (blob.Value.Length == 0)
            {
                int len = (int)(1 + DtlsUtils.RndU32(255));
                byte[] val = new byte[len];
                NextBytes(val);
                blob.MutatedValue = new Variant(val);
            }
        }
    }

    [Mutator("DtlsDeleteClientHelloCookie")]
    [CMutator("delete_client_hello_cookie")]
    [Description("Deletes the Cookie from ClientHello.")]
    public class DtlsDeleteClientHelloCookie : LLMMutator
    {
        public DtlsDeleteClientHelloCookie(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj is Blob && obj.Name == "cookie" && obj.IsIn("client_hello");
        }

        public override int count => 1;
        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }
        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        protected override void PerformMutation(DataElement obj)
        {
            var blob = obj as Blob;
            blob.MutatedValue = new Variant(new byte[0]);
        }
    }

    [Mutator("DtlsMutateClientHelloCookie")]
    [CMutator("mutate_client_hello_cookie")]
    [Description("Mutates the Cookie in ClientHello.")]
    public class DtlsMutateClientHelloCookie : LLMMutator
    {
        public DtlsMutateClientHelloCookie(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj is Blob && obj.Name == "cookie" && obj.IsIn("client_hello");
        }

        public override int count => 1;
        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }
        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        protected override void PerformMutation(DataElement obj)
        {
            var blob = obj as Blob;
            byte[] val = blob.Bytes();

            // C logic: mutate length 0..255, content random/structured
            int len = val.Length;
            uint cat = DtlsUtils.RndU32(100);

            if (cat < 10)
            {
                if (DtlsUtils.RndU32(2) == 0) len = 0;
                else len = (int)(16 + DtlsUtils.RndU32(17));
            }
            else if (cat < 57)
            {
                uint r = DtlsUtils.RndU32(100);
                if (r < 25) len = 0;
                else len = (int)(1 + DtlsUtils.RndU32(255));
            }

            byte[] newVal = new byte[len];
            NextBytes(newVal); // Fill random
            blob.MutatedValue = new Variant(newVal);
        }
    }

    [Mutator("DtlsAddClientHelloCipherSuites")]
    [CMutator("add_client_hello_cipher_suites")]
    [Description("Adds entries to Cipher Suites.")]
    public class DtlsAddClientHelloCipherSuites : LLMMutator
    {
        public DtlsAddClientHelloCipherSuites(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj is Blob && obj.Name == "cipher_suites" && obj.IsIn("client_hello");
        }

        public override int count => 1;
        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }
        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        protected override void PerformMutation(DataElement obj)
        {
            var blob = obj as Blob;
            // Add a few common suites
            byte[] extra = { 0xC0, 0x2B, 0xC0, 0x2F }; // TLS_ECDHE_ECDSA_WITH_AES_128_GCM_SHA256 etc
            byte[] cur = blob.Bytes();
            byte[] newBytes = new byte[cur.Length + extra.Length];
            cur.CopyTo(newBytes, 0);
            extra.CopyTo(newBytes, cur.Length);
            blob.MutatedValue = new Variant(newBytes);
        }
    }

    [Mutator("DtlsDeleteClientHelloCipherSuites")]
    [CMutator("delete_client_hello_cipher_suites")]
    [Description("Deletes Cipher Suites (sets to empty).")]
    public class DtlsDeleteClientHelloCipherSuites : LLMMutator
    {
        public DtlsDeleteClientHelloCipherSuites(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj is Blob && obj.Name == "cipher_suites" && obj.IsIn("client_hello");
        }

        public override int count => 1;
        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }
        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        protected override void PerformMutation(DataElement obj)
        {
            var blob = obj as Blob;
            blob.MutatedValue = new Variant(new byte[0]);
        }
    }

    [Mutator("DtlsMutateClientHelloCipherSuites")]
    [CMutator("mutate_client_hello_cipher_suites")]
    [Description("Mutates Cipher Suites list.")]
    public class DtlsMutateClientHelloCipherSuites : LLMMutator
    {
        public DtlsMutateClientHelloCipherSuites(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj is Blob && obj.Name == "cipher_suites" && obj.IsIn("client_hello");
        }

        public override int count => 1;
        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }
        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        protected override void PerformMutation(DataElement obj)
        {
            var blob = obj as Blob;
            byte[] cur_buf = blob.Bytes();
            if (cur_buf == null) cur_buf = new byte[0];
            ushort cur_len = (ushort)cur_buf.Length;
            if (cur_len > DtlsUtils.DTLS_MAX_CIPHER_SUITES_BYTES) cur_len = (ushort)DtlsUtils.DTLS_MAX_CIPHER_SUITES_BYTES;
            cur_len = DtlsUtils.EvenDown(cur_len);

            // Ensure buffer is properly sized
            byte[] out_buf = new byte[DtlsUtils.DTLS_MAX_CIPHER_SUITES_BYTES];
            if (cur_buf.Length > 0)
            {
                Array.Copy(cur_buf, out_buf, Math.Min(cur_len, cur_buf.Length));
            }
            ushort out_len = cur_len;

            uint cat = DtlsUtils.UrandCs(100);

            if (cat < 12)
            {
                // A. Canonical form: common set, ordered, include SCSV sometimes
                ushort cnt = (ushort)(4u + DtlsUtils.UrandCs(6)); // 4..9
                ushort bytes = (ushort)(cnt * 2u);
                if (bytes > DtlsUtils.DTLS_MAX_CIPHER_SUITES_BYTES) bytes = (ushort)DtlsUtils.DTLS_MAX_CIPHER_SUITES_BYTES;
                cnt = DtlsUtils.CsCount(bytes);

                for (ushort k = 0; k < cnt; k++) DtlsUtils.CsWriteAt(out_buf, k, DtlsUtils.PickCommonSuite());

                // ensure one GCM/ECDHE-ish near the front
                if (cnt > 0) DtlsUtils.CsWriteAt(out_buf, 0, 0xC02B);

                // maybe ensure SCSV present near end
                if (cnt >= 2 && DtlsUtils.UrandCs(2) == 0)
                    DtlsUtils.CsWriteAt(out_buf, (ushort)(cnt - 1u), 0x00FF);

                // remove duplicates by re-rolling
                for (ushort k = 0; k < cnt; k++)
                {
                    ushort s = DtlsUtils.CsReadAt(out_buf, k);
                    for (ushort t = (ushort)(k + 1u); t < cnt; t++)
                    {
                        if (DtlsUtils.CsReadAt(out_buf, t) == s)
                            DtlsUtils.CsWriteAt(out_buf, t, DtlsUtils.PickCommonSuite());
                    }
                }

                out_len = bytes;
                // zero tail
                for (int i = out_len; i < DtlsUtils.DTLS_MAX_CIPHER_SUITES_BYTES; i++) out_buf[i] = 0;
            }
            else if (cat < 27)
            {
                // B. Boundaries: 0, 2, 4, 32, 64, 256 bytes (even, clamped)
                ushort[] lens = { 0, 2, 4, 32, 64, 256 };
                out_len = lens[DtlsUtils.UrandCs((uint)lens.Length)];
                out_len = DtlsUtils.EvenUpClamped(out_len, (ushort)DtlsUtils.DTLS_MAX_CIPHER_SUITES_BYTES);
                ushort cnt = DtlsUtils.CsCount(out_len);

                for (int i = 0; i < DtlsUtils.DTLS_MAX_CIPHER_SUITES_BYTES; i++) out_buf[i] = 0;
                for (ushort k = 0; k < cnt; k++)
                {
                    uint r = DtlsUtils.UrandCs(100);
                    if (r < 70) DtlsUtils.CsWriteAt(out_buf, k, DtlsUtils.PickCommonSuite());
                    else DtlsUtils.CsWriteAt(out_buf, k, (ushort)DtlsUtils.UrandCs(0x10000u));
                }
            }
            else if (cat < 38)
            {
                // C. Equivalence-class alternatives: reuse another observed list, maybe permute
                // (In C# we can't easily collect from other packets, so fallback to common list)
                ushort cnt = (ushort)(2u + DtlsUtils.UrandCs(6));
                ushort bytes = (ushort)(cnt * 2u);
                if (bytes > DtlsUtils.DTLS_MAX_CIPHER_SUITES_BYTES) bytes = (ushort)DtlsUtils.DTLS_MAX_CIPHER_SUITES_BYTES;
                cnt = DtlsUtils.CsCount(bytes);
                for (ushort k = 0; k < cnt; k++) DtlsUtils.CsWriteAt(out_buf, k, DtlsUtils.PickCommonSuite());
                out_len = bytes;
                if (DtlsUtils.UrandCs(2) == 0) DtlsUtils.CsShuffle(out_buf, cnt);
                // zero tail
                for (int i = out_len; i < DtlsUtils.DTLS_MAX_CIPHER_SUITES_BYTES; i++) out_buf[i] = 0;
            }
            else if (cat < 60)
            {
                // D. Allowed range: any even length 0..256, each suite is uint16
                uint r = DtlsUtils.UrandCs(100);
                if (r < 15) out_len = 0;
                else if (r < 70) out_len = (ushort)(2u * (1u + DtlsUtils.UrandCs(16)));  // 2..32
                else if (r < 90) out_len = (ushort)(2u * (1u + DtlsUtils.UrandCs(64)));  // 2..128
                else out_len = (ushort)(2u * (1u + DtlsUtils.UrandCs(128)));             // 2..256
                out_len = DtlsUtils.EvenUpClamped(out_len, (ushort)DtlsUtils.DTLS_MAX_CIPHER_SUITES_BYTES);

                ushort cnt = DtlsUtils.CsCount(out_len);
                for (int i = 0; i < DtlsUtils.DTLS_MAX_CIPHER_SUITES_BYTES; i++) out_buf[i] = 0;

                for (ushort k = 0; k < cnt; k++)
                {
                    uint pcommon = (k < 6) ? 85u : 55u;
                    if (DtlsUtils.UrandCs(100) < pcommon)
                        DtlsUtils.CsWriteAt(out_buf, k, DtlsUtils.PickCommonSuite());
                    else
                        DtlsUtils.CsWriteAt(out_buf, k, (ushort)DtlsUtils.UrandCs(0x10000u));
                }

                // ensure at least one non-zero suite if non-empty
                if (cnt > 0 && DtlsUtils.CsReadAt(out_buf, 0) == 0x0000)
                    DtlsUtils.CsWriteAt(out_buf, 0, 0xC02B);
            }
            else if (cat < 73)
            {
                // E. Encoding-shape variant: reorder within list; duplicate marker suites; place SCSV at ends
                if (out_len == 0)
                {
                    ushort cnt = (ushort)(4u + DtlsUtils.UrandCs(6));
                    ushort bytes = (ushort)(cnt * 2u);
                    if (bytes > DtlsUtils.DTLS_MAX_CIPHER_SUITES_BYTES) bytes = (ushort)DtlsUtils.DTLS_MAX_CIPHER_SUITES_BYTES;
                    cnt = DtlsUtils.CsCount(bytes);
                    for (ushort k = 0; k < cnt; k++) DtlsUtils.CsWriteAt(out_buf, k, DtlsUtils.PickCommonSuite());
                    out_len = bytes;
                }
                ushort _cnt = DtlsUtils.CsCount(out_len);
                switch (DtlsUtils.UrandCs(4))
                {
                    case 0:
                        DtlsUtils.CsShuffle(out_buf, _cnt);
                        break;
                    case 1:
                        DtlsUtils.CsReverse(out_buf, _cnt);
                        break;
                    case 2:
                        // move a random element to front
                        if (_cnt >= 2)
                        {
                            ushort j = (ushort)DtlsUtils.UrandCs(_cnt);
                            ushort v = DtlsUtils.CsReadAt(out_buf, j);
                            for (ushort k = j; k > 0; k--)
                                DtlsUtils.CsWriteAt(out_buf, k, DtlsUtils.CsReadAt(out_buf, (ushort)(k - 1u)));
                            DtlsUtils.CsWriteAt(out_buf, 0, v);
                        }
                        break;
                    default:
                        // ensure SCSV exists and positioned
                        if (_cnt >= 1)
                        {
                            if (DtlsUtils.CsFind(out_buf, _cnt, 0x00FF) == 0xFFFFu)
                                DtlsUtils.CsWriteAt(out_buf, (ushort)(_cnt - 1u), 0x00FF);
                            if (_cnt >= 2 && DtlsUtils.CsFind(out_buf, _cnt, 0x5600) == 0xFFFFu)
                                DtlsUtils.CsWriteAt(out_buf, (ushort)(_cnt - 2u), 0x5600);
                        }
                        break;
                }
            }
            else if (cat < 82)
            {
                // F. Padding/alignment: keep even length; emphasize "aligned tail" of zeros (still bytes within vector)
                if (out_len == 0)
                {
                    out_len = (ushort)(2u * (4u + DtlsUtils.UrandCs(8))); // 8..22 suites
                    out_len = DtlsUtils.EvenUpClamped(out_len, (ushort)DtlsUtils.DTLS_MAX_CIPHER_SUITES_BYTES);
                    ushort cnt = DtlsUtils.CsCount(out_len);
                    for (ushort k = 0; k < cnt; k++) DtlsUtils.CsWriteAt(out_buf, k, DtlsUtils.PickCommonSuite());
                }
                ushort _cnt = DtlsUtils.CsCount(out_len);
                // zero out last 1..4 suites
                ushort z = (ushort)(1u + DtlsUtils.UrandCs(4));
                if (z > _cnt) z = _cnt;
                for (ushort k = 0; k < z; k++)
                    DtlsUtils.CsWriteAt(out_buf, (ushort)(_cnt - 1u - k), 0x0000);
            }
            else if (cat < 92)
            {
                // G. In-range sweep: sweep suite IDs over a narrow band (valid uint16 space), keep even len
                ushort cnt = (ushort)(2u + DtlsUtils.UrandCs(24)); // 2..25
                ushort bytes = (ushort)(cnt * 2u);
                if (bytes > DtlsUtils.DTLS_MAX_CIPHER_SUITES_BYTES) bytes = (ushort)DtlsUtils.DTLS_MAX_CIPHER_SUITES_BYTES;
                cnt = DtlsUtils.CsCount(bytes);

                ushort base_suite = (ushort)DtlsUtils.UrandCs(0x10000u);
                for (ushort k = 0; k < cnt; k++)
                    DtlsUtils.CsWriteAt(out_buf, k, (ushort)(base_suite + k));
                out_len = (ushort)(cnt * 2u);
                // zero tail
                for (int i = out_len; i < DtlsUtils.DTLS_MAX_CIPHER_SUITES_BYTES; i++) out_buf[i] = 0;
            }
            else
            {
                // H. Random valid mix: splice prefix from observed + tail random/common, then perturb
                ushort cnt;
                if (cur_len > 0)
                {
                    out_len = cur_len;
                }
                else
                {
                    out_len = (ushort)(2u * (2u + DtlsUtils.UrandCs(32))); // 4..68 suites
                    out_len = DtlsUtils.EvenUpClamped(out_len, (ushort)DtlsUtils.DTLS_MAX_CIPHER_SUITES_BYTES);
                }
                cnt = DtlsUtils.CsCount(out_len);

                // start with random/common
                for (ushort k = 0; k < cnt; k++)
                {
                    if (DtlsUtils.UrandCs(100) < 70)
                        DtlsUtils.CsWriteAt(out_buf, k, DtlsUtils.PickCommonSuite());
                    else
                        DtlsUtils.CsWriteAt(out_buf, k, (ushort)DtlsUtils.UrandCs(0x10000u));
                }

                // ensure at least one strong suite somewhere
                if (cnt > 0)
                {
                    ushort pos = (ushort)DtlsUtils.UrandCs(cnt);
                    DtlsUtils.CsWriteAt(out_buf, pos, 0xC02B);
                }
            }

            // Randomized perturbations: shallow + deep diversity
            if (DtlsUtils.UrandCs(100) < 28 && out_len > 0)
            {
                // flip a few bits in random bytes inside the list
                uint flips = 1u + DtlsUtils.UrandCs(8);
                for (uint f = 0; f < flips; f++)
                {
                    ushort idx = (ushort)DtlsUtils.UrandCs(out_len);
                    out_buf[idx] ^= (byte)(1u << (int)DtlsUtils.UrandCs(8));
                }
            }
            if (DtlsUtils.UrandCs(100) < 14 && out_len >= 2)
            {
                // deep: swap two suites
                ushort cnt = DtlsUtils.CsCount(out_len);
                if (cnt >= 2)
                {
                    ushort a = (ushort)DtlsUtils.UrandCs(cnt);
                    ushort b = (ushort)DtlsUtils.UrandCs(cnt);
                    ushort va = DtlsUtils.CsReadAt(out_buf, a);
                    ushort vb = DtlsUtils.CsReadAt(out_buf, b);
                    DtlsUtils.CsWriteAt(out_buf, a, vb);
                    DtlsUtils.CsWriteAt(out_buf, b, va);
                }
            }
            if (DtlsUtils.UrandCs(100) < 10)
            {
                // occasionally adjust length slightly but keep even+clamped
                if (DtlsUtils.UrandCs(2) == 0)
                {
                    out_len = (out_len >= 2) ? (ushort)(out_len - 2u) : (ushort)0;
                }
                else
                {
                    out_len = (out_len + 2u <= DtlsUtils.DTLS_MAX_CIPHER_SUITES_BYTES) ? (ushort)(out_len + 2u) : out_len;
                }
                out_len = DtlsUtils.EvenDown(out_len);
            }

            // Resize output to actual length
            byte[] final = new byte[out_len];
            Array.Copy(out_buf, final, out_len);
            blob.MutatedValue = new Variant(final);
        }
    }

    [Mutator("DtlsAddClientHelloCompressionMethods")]
    [CMutator("add_client_hello_compression_methods")]
    [Description("Adds Compression Methods.")]
    public class DtlsAddClientHelloCompressionMethods : LLMMutator
    {
        public DtlsAddClientHelloCompressionMethods(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj is Blob && obj.Name == "compression_methods" && obj.IsIn("client_hello");
        }

        public override int count => 1;
        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }
        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        protected override void PerformMutation(DataElement obj)
        {
            var blob = obj as Blob;
            if (blob.Value.Length == 0)
            {
                blob.MutatedValue = new Variant(new byte[] { 0 }); // Null compression
            }
        }
    }

    [Mutator("DtlsDeleteClientHelloCompressionMethods")]
    [CMutator("delete_client_hello_compression_methods")]
    [Description("Deletes Compression Methods.")]
    public class DtlsDeleteClientHelloCompressionMethods : LLMMutator
    {
        public DtlsDeleteClientHelloCompressionMethods(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj is Blob && obj.Name == "compression_methods" && obj.IsIn("client_hello");
        }

        public override int count => 1;
        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }
        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        protected override void PerformMutation(DataElement obj)
        {
            var blob = obj as Blob;
            blob.MutatedValue = new Variant(new byte[0]);
        }
    }

    [Mutator("DtlsMutateClientHelloCompressionMethods")]
    [CMutator("mutate_client_hello_compression_methods")]
    [Description("Mutates Compression Methods.")]
    public class DtlsMutateClientHelloCompressionMethods : LLMMutator
    {
        public DtlsMutateClientHelloCompressionMethods(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj is Blob && obj.Name == "compression_methods" && obj.IsIn("client_hello");
        }

        public override int count => 1;
        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }
        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        protected override void PerformMutation(DataElement obj)
        {
            var blob = obj as Blob;
            byte[] val = blob.Bytes();

            // Randomize, ensure 0 is present sometimes
            if (val.Length > 0)
            {
                NextBytes(val);
                if (DtlsUtils.RndU32(2) == 0) val[0] = 0;
            }
            blob.MutatedValue = new Variant(val);
        }
    }

    [Mutator("DtlsAddClientHelloExtensions")]
    [CMutator("add_client_hello_extensions")]
    [Description("Adds Extensions to ClientHello.")]
    public class DtlsAddClientHelloExtensions : LLMMutator
    {
        public DtlsAddClientHelloExtensions(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.Name == "extensions" && obj.IsIn("client_hello");
        }

        public override int count => 1;
        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }
        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        protected override void PerformMutation(DataElement obj)
        {
            if (obj.Value.Length == 0)
            {
                if (Next(2) == 0)
                {
                    // Add minimal extension (renegotiation info: 0xFF01 0001 00)
                    byte[] ext = { 0x00, 0x05, 0xFF, 0x01, 0x00, 0x01, 0x00 };
                    obj.MutatedValue = new Variant(ext);
                }
                else
                {
                    byte[] ext = { 0x00 };
                    obj.MutatedValue = new Variant(ext);
                }
            }
        }
    }

    [Mutator("DtlsDeleteClientHelloExtensions")]
    [CMutator("delete_client_hello_extensions")]
    [Description("Deletes Extensions from ClientHello.")]
    public class DtlsDeleteClientHelloExtensions : LLMMutator
    {
        public DtlsDeleteClientHelloExtensions(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.Name == "extensions" && obj.IsIn("client_hello");
        }

        public override int count => 1;
        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }
        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        protected override void PerformMutation(DataElement obj)
        {
            obj.parent.Remove(obj);
        }
    }

    [Mutator("DtlsMutateClientHelloExtensions")]
    [CMutator("mutate_client_hello_extensions")]
    [Description("Mutates ClientHello Extensions raw blob.")]
    public class DtlsMutateClientHelloExtensions : LLMMutator
    {
        public DtlsMutateClientHelloExtensions(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj.Name == "extensions" && obj.IsIn("client_hello");
        }

        public override int count => 1;
        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }
        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        protected override void PerformMutation(DataElement obj)
        {
            int cat = Next(8);
            byte[] val = null;

            switch (cat)
            {
                case 0:
                    if (Next(3) == 0)
                    {
                        obj.parent.Remove(obj);
                    }
                    else if (Next(2) == 0)
                    {
                        val = new byte[] { 0x00 };
                    }
                    else
                    {
                        val = DtlsUtils.ExtBuildMinimal();
                    }
                    break;
                case 1:
                    if (Next(2) == 0)
                    {
                        val = DtlsUtils.ExtBuildMinimal();
                    }
                    else
                    {
                        val = DtlsUtils.ExtBuildEcdheLike();
                    }
                    var targets = new uint[]
                    {
                        0, 4, 8, 16, 32, 64, 128, 255, 256, 384, 511, 512
                    };
                    var t = targets[Next(targets.Length)];
                    if (t == 0)
                    {
                        obj.parent.Remove(obj);
                    }
                    else if (t <= DtlsUtils.DTLS_MAX_EXTENSIONS_LEN)
                    {
                        DtlsUtils.ExtAddPaddingToAlign(val, 16);
                        var len = (uint)val.Length - 2;
                        if (len < t && t - len >= 4)
                        {
                            var remain = t - len;
                            var dLen = remain - 4;
                            var use = Math.Min(dLen, 256);
                            var buf = new byte[use];
                            NextBytes(buf);
                            val = val.Concat(DtlsUtils.ExtCombine(
                                new byte[] { 0x00, 0xFF },
                                new byte[] { (byte)(use >> 8), (byte)(use & 0xFF) },
                                buf)).ToArray();
                        }
                    }
                    break;
                case 2:
                    if (obj.Bytes().Length == 0 || obj.Bytes().Length == 2)
                    {
                        if (Next(2) == 0)
                        {
                            val = DtlsUtils.ExtBuildMinimal();
                        }
                        else
                        {
                            val = DtlsUtils.ExtBuildEcdheLike();
                        }
                    }
                    else
                    {
                        val = DtlsUtils.ExtRebuildShuffled(val);

                        var ents = DtlsUtils.ExtParseEntries(val, out var cnt);
                        if (cnt > 0)
                        {
                            var e0 = ents[0];
                            byte[] tmp = new byte[e0.Len > 64 ? 64 : e0.Len];
                            if (e0.Len > 0)
                            {
                                Array.Copy(val, e0.DataOffset, tmp, 0, tmp.Length);
                            }
                            val = DtlsUtils.ExtAppend(val, e0.Type, tmp);
                        }
                    }
                    break;
                case 3: /* D. Allowed bitfield/enum/range */
                    {
                        /* Keep structure well-formed and tweak known fields within their domains */
                        if (val == null || val.Length <= 2)
                        {
                            val = DtlsUtils.ExtBuildEcdheLike();
                        }

                        uint cnt;
                        var entries = DtlsUtils.ExtParseEntries(val, out cnt);
                        if (cnt == 0) break;

                        // 为了方便修改，我们直接在 val 数组上操作
                        // 注意：C# 数组修改是引用传递，直接修改 val[index] 即可

                        foreach (var e in entries)
                        {
                            /* signature_algorithms (0x000D) */
                            if (e.Type == 0x000D && e.Len >= 4)
                            {
                                int baseOffset = e.DataOffset; // DataOffset 指向 Type/Len 之后的具体数据开始处
                                                               // 结构: [list_len (2 bytes)] [sig_alg_pairs ...]
                                int listLen = RdU16(val, baseOffset);

                                /* clamp to even and within buffer */
                                if (listLen > (e.Len - 2)) listLen = e.Len - 2;
                                listLen &= ~1; // make even
                                WrU16(val, (ushort)listLen, baseOffset);

                                /* tweak a few pairs */
                                int pairs = listLen / 2;
                                if (pairs > 0)
                                {
                                    int tweak = 1 + Next(3);
                                    for (int _t = 0; _t < tweak; _t++)
                                    {
                                        int pi = Next(pairs);
                                        int pos = baseOffset + 2 + (pi * 2);
                                        if (pos + 1 < baseOffset + e.Len)
                                        {
                                            /* hash: 1..6 (common), sig: 1..3 (rsa/dsa/ecdsa) */
                                            val[pos + 0] = (byte)(1 + Next(6));
                                            val[pos + 1] = (byte)(1 + Next(3));
                                        }
                                    }
                                }
                            }

                            /* ec_point_formats (0x000B) */
                            if (e.Type == 0x000B && e.Len >= 1)
                            {
                                int baseOffset = e.DataOffset;
                                // 结构: [list_len (1 byte)] [formats ...]
                                byte l = val[baseOffset];
                                if (l > (e.Len - 1)) l = (byte)(e.Len - 1);
                                val[baseOffset] = l;

                                if (l > 0)
                                {
                                    int flips = 1 + Next(2);
                                    for (int _t = 0; _t < flips; _t++)
                                    {
                                        int pos = baseOffset + 1 + Next(l);
                                        if (pos < baseOffset + e.Len)
                                        {
                                            val[pos] = (byte)Next(3);
                                        }
                                    }
                                }
                            }

                            /* supported_groups (0x000A) */
                            if (e.Type == 0x000A && e.Len >= 4)
                            {
                                int baseOffset = e.DataOffset;
                                int listLen = RdU16(val, baseOffset);
                                if (listLen > (e.Len - 2)) listLen = e.Len - 2;
                                listLen &= ~1;
                                WrU16(val, (ushort)listLen, baseOffset);

                                int ng = listLen / 2;
                                if (ng > 0)
                                {
                                    int gidx = Next(ng);
                                    int pos = baseOffset + 2 + (gidx * 2);
                                    if (pos + 1 < baseOffset + e.Len)
                                    {
                                        /* choose common named groups 23..25 (0x0017..0x0019) */
                                        ushort grp = (ushort)(23 + Next(3));
                                        WrU16(val, grp, pos);
                                    }
                                }
                            }
                        }
                    }
                    break;

                case 4: /* E. Encoding-shape variant */
                    val = DtlsUtils.ExtBuildGreaseMix();
                    break;

                case 5: /* F. Padding/alignment */
                    {
                        if (val == null || val.Length <= 2) val = DtlsUtils.ExtBuildMinimal();

                        uint align = (Next(2) == 0) ? 8u : 16u;
                        val = DtlsUtils.ExtAddPaddingToAlign(val, align);

                        /* insert tiny zero-length extension */
                        if (Next(2) == 0)
                        {
                            ushort typ = (ushort)Next(0xFFFF);
                            val = DtlsUtils.ExtAppend(val, typ, new byte[0]);
                        }

                        if (Next(2) == 0) val = DtlsUtils.ExtRebuildShuffled(val);
                    }
                    break;

                case 6: /* G. In-range sweep */
                    {
                        if (val == null || val.Length <= 2) val = DtlsUtils.ExtBuildEcdheLike();

                        int steps = 1 + Next(4);
                        for (int s = 0; s < steps; s++)
                        {
                            int action = Next(3);
                            if (action == 0)
                            {
                                /* append small unknown ext */
                                ushort typ = (ushort)(0x7F00 + Next(0x0100));
                                int len = Next(8);
                                byte[] buf = new byte[len];
                                if (len > 0) NextBytes(buf);
                                val = DtlsUtils.ExtAppend(val, typ, buf);
                            }
                            else if (action == 1)
                            {
                                val = DtlsUtils.ExtAddPaddingToAlign(val, 16);
                            }
                            else
                            {
                                val = DtlsUtils.ExtRebuildShuffled(val);
                            }
                        }
                    }
                    break;

                case 7: /* H. Random valid mix */
                    if (Next(2) == 0) val = DtlsUtils.ExtBuildEcdheLike();
                    else val = DtlsUtils.ExtBuildGreaseMix();
                    break;
                default:
                    throw new Exception("Unreachable");
            }

            if (val != null)
                obj.MutatedValue = new Variant(val);
        }
    }
}