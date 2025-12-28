using System;

namespace Peach.Pro.Core.Mutators.LLM.DTLS
{
    public static class DtlsUtils
    {
        // C Function: clamp_u32
        public static uint ClampU32(uint v, uint lo, uint hi)
        {
            if (v < lo) return lo;
            if (v > hi) return hi;
            return v;
        }

        // C Function: swap_u8
        // Note: C# uses 'ref' for pointer-like behavior
        public static void SwapU8(ref byte a, ref byte b)
        {
            byte t = a; a = b; b = t;
        }

        // C Function: swap_u16
        public static void SwapU16(ref ushort a, ref ushort b)
        {
            ushort t = a; a = b; b = t;
        }

        // C Function: swap_u32
        public static void SwapU32(ref uint a, ref uint b)
        {
            uint t = a; a = b; b = t;
        }

        // C Function: rd_u16
        public static ushort RdU16(byte[] p, int offset = 0)
        {
            return (ushort)(((ushort)p[offset] << 8) | (ushort)p[offset + 1]);
        }

        // C Function: wr_u16
        public static void WrU16(byte[] p, ushort v, int offset = 0)
        {
            p[offset] = (byte)(v >> 8);
            p[offset + 1] = (byte)(v & 0xff);
        }

        // C Function: rd_u24
        public static uint RdU24(byte[] p, int offset = 0)
        {
            return ((uint)p[offset] << 16) | ((uint)p[offset + 1] << 8) | (uint)p[offset + 2];
        }

        // C Function: wr_u24
        public static void WrU24(byte[] p, uint v, int offset = 0)
        {
            p[offset] = (byte)((v >> 16) & 0xff);
            p[offset + 1] = (byte)((v >> 8) & 0xff);
            p[offset + 2] = (byte)(v & 0xff);
        }

        // C Function: xorshift32_state
        public static uint Xorshift32State(ref uint state)
        {
            uint x = (state != 0) ? state : 0xA3C59AC3u;
            x ^= x << 13;
            x ^= x >> 17;
            x ^= x << 5;
            state = x;
            return x;
        }

        // C Function: rnd_u32
        // Requires a global or passed state; using a simple field for demonstration
        private static uint _g_dtls_rng_state = 0xA3C59AC3u;
        public static uint RndU32(uint maxExclusive)
        {
            if (maxExclusive == 0) return 0;
            return Xorshift32State(ref _g_dtls_rng_state) % maxExclusive;
        }

        // C Function: rnd_u32_state
        public static uint RndU32State(ref uint state, uint lo, uint hi)
        {
            if (hi < lo) { uint t = lo; lo = hi; hi = t; }
            uint span = hi - lo + 1u;
            uint r = Xorshift32State(ref state);
            if (span == 0) return lo;
            return lo + (r % span);
        }

        // C Function: mem_rotl
        public static void MemRotl(byte[] buf, int n, int k)
        {
            if (buf == null || n == 0) return;
            k %= n;
            if (k == 0) return;

            byte[] tmp = new byte[k];
            Array.Copy(buf, 0, tmp, 0, k);
            Array.Copy(buf, k, buf, 0, n - k);
            Array.Copy(tmp, 0, buf, n - k, k);
        }

        // C Function: urand_epoch
        private static uint _g_seed_epoch = 0xA51CE0E1u;
        public static uint UrandEpoch(uint n)
        {
            if (n == 0) return 0;
            _g_seed_epoch ^= _g_seed_epoch << 13;
            _g_seed_epoch ^= _g_seed_epoch >> 17;
            _g_seed_epoch ^= _g_seed_epoch << 5;
            return _g_seed_epoch % n;
        }

        // C Function: clamp_u16
        public static ushort ClampU16(uint v)
        {
            return (ushort)(v & 0xFFFFu);
        }

        // C Function: rd_u48_be
        public static ulong RdU48Be(byte[] b, int offset = 0)
        {
            return ((ulong)b[offset] << 40) | ((ulong)b[offset + 1] << 32) | ((ulong)b[offset + 2] << 24) |
                   ((ulong)b[offset + 3] << 16) | ((ulong)b[offset + 4] << 8) | (ulong)b[offset + 5];
        }

        // C Function: wr_u48_be
        public static void WrU48Be(byte[] b, ulong x, int offset = 0)
        {
            b[offset] = (byte)((x >> 40) & 0xFFu);
            b[offset + 1] = (byte)((x >> 32) & 0xFFu);
            b[offset + 2] = (byte)((x >> 24) & 0xFFu);
            b[offset + 3] = (byte)((x >> 16) & 0xFFu);
            b[offset + 4] = (byte)((x >> 8) & 0xFFu);
            b[offset + 5] = (byte)(x & 0xFFu);
        }

        // C Function: clamp_u48
        public static ulong ClampU48(ulong x)
        {
            return x & 0xFFFFFFFFFFFFUL;
        }

        // C Function: pick_common_hs_type
        private static readonly byte[] k_hs_types_common = { 0, 1, 2, 3, 11, 12, 13, 14, 15, 16, 20 };
        private static uint _g_seed_hmt = 0xC0FFEE11u;
        public static byte PickCommonHsType()
        {
            _g_seed_hmt ^= _g_seed_hmt << 13;
            _g_seed_hmt ^= _g_seed_hmt >> 17;
            _g_seed_hmt ^= _g_seed_hmt << 5;
            uint idx = _g_seed_hmt % (uint)k_hs_types_common.Length;
            return k_hs_types_common[idx];
        }
    }
}