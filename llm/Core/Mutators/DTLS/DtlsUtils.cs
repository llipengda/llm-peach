using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Peach.LLM.Core.Mutators.DTLS
{
    public static class DtlsUtils
    {
        private static Random _rng = new Random();

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

        public static byte[] ExtBuildMinimal()
        {
            // Minimal extension: renegotiation info: 0xFF01 0001 00
            return new byte[] { 0x00, 0x05, 0xFF, 0x01, 0x00, 0x01, 0x00 };
        }

        public static byte[] ExtCombine(byte[] extType, byte[] extLen, byte[] extData)
        {
            Debug.Assert(extType.Length == 2);
            Debug.Assert(extLen.Length == 2);
            Debug.Assert(extData.Length == DtlsUtils.RdU16(extLen, 0));
            byte[] ext = new byte[extType.Length + extLen.Length + extData.Length];
            Array.Copy(extType, 0, ext, 0, extType.Length);
            Array.Copy(extLen, 0, ext, extType.Length, extLen.Length);
            Array.Copy(extData, 0, ext, extType.Length + extLen.Length, extData.Length);
            return ext;
        }

        public static byte[] ExtBuildEcdheLike()
        {
            // supported_groups (named curves) (0x000A)
            var sg = new byte[]
            {
                0x00, 0x04, 0x00, 0x17, 0x00, 0x18
            };
            var sgLen = new byte[]
            {
                0x00, 0x06
            };
            var sgType = new byte[]
            {
                0x00, 0x0A
            };

            // ec_point_formats (0x000B)
            var epf = new byte[]
            {
                0x01, 0x00
            };
            var epfLen = new byte[]
            {
                0x00, 0x02
            };
            var epfType = new byte[]
            {
                0x00, 0x0B
            };

            // signature_algorithms (0x000D)
            var sa = new byte[]
            {
                0x00, 0x06,
                0x04, 0x03,
                0x02, 0x01,
                0x04, 0x01
            };
            var saLen = new byte[]
            {
                0x00, 0x08
            };
            var saType = new byte[]
            {
                0x00, 0x0D
            };

            // extended_master_secret (0x0017) length=0
            var ems = new byte[0];
            var emsLen = new byte[]
            {
                0x00, 0x00
            };
            var emsType = new byte[]
            {
                0x00, 0x17
            };

            // renegotiation_info (0xFF01)
            var ri = new byte[]
            {
                0x00
            };
            var riLen = new byte[]
            {
                0x00, 0x01
            };
            var riType = new byte[]
            {
                0xFF, 0x01
            };

            var totalLen = sg.Length + sgLen.Length + sgType.Length +
                           epf.Length + epfLen.Length + epfType.Length +
                           sa.Length + saLen.Length + saType.Length +
                           ems.Length + emsLen.Length + emsType.Length +
                           ri.Length + riLen.Length + riType.Length;

            return new byte[]
            {
                (byte)((totalLen >> 8) & 0xFFu),
                (byte)(totalLen & 0xFFu)
            }.Concat(ExtCombine(sgType, sgLen, sg))
             .Concat(ExtCombine(epfType, epfLen, epf))
             .Concat(ExtCombine(saType, saLen, sa))
             .Concat(ExtCombine(emsType, emsLen, ems))
             .Concat(ExtCombine(riType, riLen, ri))
             .ToArray();
        }

        public static readonly uint DTLS_MAX_EXTENSIONS_LEN = 512;
        public static byte[] ExtAddPaddingToAlign(byte[] ext, uint align)
        {
            if (align == 0)
                return ext;

            if (ext.Length < 2)
                throw new ArgumentException("ext length must be at least 2 to hold extension length");

            var cur = (uint)ext.Length - 2;
            var want = (cur + align - 1) & ~(align - 1);
            if (want > DTLS_MAX_EXTENSIONS_LEN)
                want = DTLS_MAX_EXTENSIONS_LEN;

            if (want <= cur)
                return ext;

            var maxPad = DTLS_MAX_EXTENSIONS_LEN - cur;

            if (maxPad < 4)
                return ext;

            var padDataLen = want - cur;
            if (padDataLen < 4)
                padDataLen = 4;
            if (padDataLen > maxPad)
                padDataLen = maxPad;

            var dLen = padDataLen - 4;
            var use = Math.Min(dLen, 256);
            var pad = new byte[use];

            _rng.NextBytes(pad);

            var remain = dLen;
            while (remain > 0)
            {
                var chunk = Math.Min(remain, 256);
                ext = ext.Concat(ExtCombine(
                    new byte[] { 0x00, 0x15 }, // padding type
                    new byte[] { (byte)((chunk >> 8) & 0xFFu), (byte)(chunk & 0xFFu) }, // length
                    pad.Take((int)chunk).ToArray() // data
                )).ToArray();
                remain -= chunk;
            }

            var newLen = (uint)ext.Length - 2;
            ext[0] = (byte)((newLen >> 8) & 0xFFu);
            ext[1] = (byte)(newLen & 0xFFu);

            return ext;
        }

        public static byte[] ExtAppend(byte[] ext, ushort typ, byte[] data)
        {
            if (ext == null || ext.Length < 2 || data == null)
                throw new ArgumentException("ext and data must be non-null and ext length at least 2");
            int declaredLen = (ext[0] << 8) | ext[1];
            if (ext.Length < 2 + declaredLen)
                throw new ArgumentException("ext length inconsistent with declared length");
            ushort dataLen = (ushort)(data.Length);
            byte[] newExt = new byte[ext.Length + 4 + dataLen];
            Array.Copy(ext, 0, newExt, 0, ext.Length);
            int offset = ext.Length;
            newExt[offset++] = (byte)(typ >> 8);
            newExt[offset++] = (byte)(typ & 0xFF);
            newExt[offset++] = (byte)(dataLen >> 8);
            newExt[offset++] = (byte)(dataLen & 0xFF);
            Array.Copy(data, 0, newExt, offset, dataLen);
            uint newDeclaredLen = (uint)(declaredLen + 4 + dataLen);
            newExt[0] = (byte)((newDeclaredLen >> 8) & 0xFFu);
            newExt[1] = (byte)(newDeclaredLen & 0xFFu);
            return newExt;
        }

        public static List<ExtensionEntry> ExtParseEntries(byte[] ext, out uint cnt)
        {
            if (ext == null || ext.Length < 2)
            {
                cnt = 0;
                return new List<ExtensionEntry>();
            }

            int declaredLen = (ext[0] << 8) | ext[1];

            if (ext.Length < 2 + declaredLen)
            {
                cnt = 0;
                return new List<ExtensionEntry>();
            }

            var entries = new List<ExtensionEntry>();
            int offset = 2;
            int end = 2 + declaredLen;

            while (offset < end)
            {
                if (end - offset < 4) break;

                ushort type = (ushort)((ext[offset] << 8) | ext[offset + 1]);
                ushort len = (ushort)((ext[offset + 2] << 8) | ext[offset + 3]);
                int dataOffset = offset + 4;

                if (dataOffset + len > end) break;

                entries.Add(new ExtensionEntry
                {
                    Type = type,
                    Len = len,
                    DataOffset = dataOffset
                });

                offset = dataOffset + len;
            }

            cnt = (uint)entries.Count;
            return entries;
        }

        public static byte[] ExtRebuildShuffled(byte[] ext)
        {
            if (ext == null || ext.Length < 2)
                return ext;

            int declaredLen = (ext[0] << 8) | ext[1];

            var entries = ExtParseEntries(ext, out uint cnt);

            if (cnt == 0)
                return ext;

            if (entries.Count > 1)
            {
                for (int i = entries.Count - 1; i > 0; i--)
                {
                    int j = _rng.Next(0, i + 1);
                    var temp = entries[i];
                    entries[i] = entries[j];
                    entries[j] = temp;
                }
            }

            byte[] result = new byte[2 + declaredLen];

            result[0] = ext[0];
            result[1] = ext[1];

            int writePos = 2;
            foreach (var entry in entries)
            {
                result[writePos++] = (byte)(entry.Type >> 8);
                result[writePos++] = (byte)(entry.Type & 0xFF);
                result[writePos++] = (byte)(entry.Len >> 8);
                result[writePos++] = (byte)(entry.Len & 0xFF);

                if (entry.Len > 0)
                {
                    Array.Copy(ext, entry.DataOffset, result, writePos, entry.Len);
                    writePos += entry.Len;
                }
            }

            return result;
        }

        public struct ExtensionEntry
        {
            public ushort Type;
            public ushort Len;
            public int DataOffset;
        }

        public static byte[] ExtBuildGreaseMix()
        {
            byte[] currentBytes;

            if (_rng.Next(2) == 0)
            {
                currentBytes = DtlsUtils.ExtBuildMinimal();
            }
            else
            {
                currentBytes = DtlsUtils.ExtBuildEcdheLike();
            }

            /* Add 0..3 "GREASE-like" unknown extensions with small lengths */
            int k = _rng.Next(4);
            for (int i = 0; i < k; i++)
            {
                ushort typ = (ushort)(0x0A0A + _rng.Next(0xF0F0));

                int len = _rng.Next(12);

                byte[] buf = new byte[len];
                if (len > 0)
                {
                    _rng.NextBytes(buf);
                }

                currentBytes = DtlsUtils.ExtAppend(currentBytes, typ, buf);
            }

            /* Maybe add padding to a boundary */
            if (_rng.Next(2) == 0)
            {
                uint align = (_rng.Next(2) == 0) ? 8u : 16u;
                currentBytes = DtlsUtils.ExtAddPaddingToAlign(currentBytes, align);
            }

            /* Maybe shuffle */
            if (_rng.Next(2) == 0)
            {
                currentBytes = DtlsUtils.ExtRebuildShuffled(currentBytes);
            }

            return currentBytes;
        }

        public static byte[] ShallowPerturb(byte[] ext)
        {
            // C: if (!ext || !ext->present) return;
            if (ext == null || ext.Length < 2) return ext;

            // C: if (ext->total_len < 5) return;
            if (ext.Length < 7) return ext;

            /* Try to flip bytes mostly in data areas, avoid breaking type/len too often */
            uint cnt;
            var ents = DtlsUtils.ExtParseEntries(ext, out cnt);

            // C: if (ext_parse_entries(...) != 0 || cnt == 0)
            if (cnt == 0)
            {
                /* fallback: flip some bytes in raw */
                int flips = 1 + _rng.Next(3);
                for (int i = 0; i < flips; i++)
                {
                    int pos = _rng.Next(ext.Length);
                    ext[pos] ^= (byte)(1 << _rng.Next(8));
                }
                return ext;
            }

            int targetFlips = 1 + _rng.Next(4);
            for (int i = 0; i < targetFlips; i++)
            {
                var e = ents[_rng.Next((int)cnt)];

                if (e.Len == 0) continue;

                int j = _rng.Next(e.Len);
                // data offset + j
                int pos = e.DataOffset + j;

                if (pos < ext.Length)
                {
                    // C: ext->raw[pos] ^= (uint8_t)(rng32() & 0xFFu);
                    ext[pos] ^= (byte)_rng.Next(256);
                }
            }

            return ext;
        }

        // Constants from dtls_packets.h
        public const uint DTLS_MAX_SESSION_ID_LEN = 32;
        public const uint DTLS_MAX_COOKIE_LEN = 255;
        public const uint DTLS_MAX_CIPHER_SUITES_BYTES = 256;
        public const uint DTLS_MAX_CERT_BLOB_LEN = 8192;
        public const uint DTLS_MAX_DH_Y_LEN = 512;
        public const uint DTLS_MAX_RSA_ENC_PMS_LEN = 512;
        public const uint DTLS_MAX_PSK_IDENTITY_LEN = 256;

        // Common cipher suites
        private static readonly ushort[] k_cs_common = {
            0xC02B, /* TLS_ECDHE_ECDSA_WITH_AES_128_GCM_SHA256 */
            0xC02C, /* TLS_ECDHE_ECDSA_WITH_AES_256_GCM_SHA384 */
            0xC02F, /* TLS_ECDHE_RSA_WITH_AES_128_GCM_SHA256 */
            0xC030, /* TLS_ECDHE_RSA_WITH_AES_256_GCM_SHA384 */
            0xC00A, /* TLS_ECDHE_ECDSA_WITH_AES_256_CBC_SHA */
            0xC009, /* TLS_ECDHE_ECDSA_WITH_AES_128_CBC_SHA */
            0xC013, /* TLS_ECDHE_RSA_WITH_AES_128_CBC_SHA */
            0xC014, /* TLS_ECDHE_RSA_WITH_AES_256_CBC_SHA */
            0x00A8, /* TLS_PSK_WITH_AES_128_GCM_SHA256 */
            0x00A9, /* TLS_PSK_WITH_AES_256_GCM_SHA384 */
            0x008C, /* TLS_PSK_WITH_AES_128_CBC_SHA */
            0x008D, /* TLS_PSK_WITH_AES_256_CBC_SHA */
            0x00FF, /* TLS_EMPTY_RENEGOTIATION_INFO_SCSV */
            0x5600  /* TLS_FALLBACK_SCSV */
        };

        // RNG state for cipher suites
        private static uint _g_seed_cs = 0xA51CE0E1u;
        private static uint Xs32Cs()
        {
            uint x = _g_seed_cs;
            x ^= x << 13;
            x ^= x >> 17;
            x ^= x << 5;
            _g_seed_cs = x;
            return x;
        }
        public static uint UrandCs(uint n) { return n != 0 ? (Xs32Cs() % n) : 0; }

        // RNG state for client hello random
        private static uint _g_seed_chr = 0xC0FFEE11u;
        private static uint Xs32ChrInternal()
        {
            uint x = _g_seed_chr;
            x ^= x << 13;
            x ^= x >> 17;
            x ^= x << 5;
            _g_seed_chr = x;
            return x;
        }
        public static uint Xs32Chr() { return Xs32ChrInternal(); }
        public static uint UrandChr(uint n) { return n != 0 ? (Xs32ChrInternal() % n) : 0; }

        // RNG state for certificate
        private static uint _g_seed_cert = 0x85EBCA6Bu;
        private static uint Xs32Cert()
        {
            uint x = _g_seed_cert;
            x ^= x << 13;
            x ^= x >> 17;
            x ^= x << 5;
            _g_seed_cert = x;
            return x;
        }
        public static uint UrandCert(uint n) { return n != 0 ? (Xs32Cert() % n) : 0; }

        // Helper functions for cipher suites
        public static ushort PickCommonSuite()
        {
            return k_cs_common[UrandCs((uint)k_cs_common.Length)];
        }

        public static ushort EvenDown(ushort v) { return (ushort)(v & ~1u); }

        public static ushort EvenUpClamped(ushort v, ushort cap)
        {
            ushort r = (ushort)((v + 1u) & ~1u);
            return (r > cap) ? cap : r;
        }

        public static ushort CsCount(ushort bytesLen) { return (ushort)(bytesLen / 2u); }

        public static void CsWriteAt(byte[] buf, ushort idx, ushort suite)
        {
            ushort off = (ushort)(idx * 2u);
            WrU16(buf, suite, off);
        }

        public static ushort CsReadAt(byte[] buf, ushort idx)
        {
            ushort off = (ushort)(idx * 2u);
            return RdU16(buf, off);
        }

        public static ushort CsFind(byte[] buf, ushort cnt, ushort suite)
        {
            for (ushort i = 0; i < cnt; i++)
                if (CsReadAt(buf, i) == suite) return i;
            return 0xFFFF;
        }

        public static void CsShuffle(byte[] buf, ushort cnt)
        {
            if (buf == null || cnt < 2) return;
            for (ushort i = (ushort)(cnt - 1); i > 0; i--)
            {
                ushort j = (ushort)UrandCs((uint)(i + 1));
                ushort a = CsReadAt(buf, i);
                ushort b = CsReadAt(buf, j);
                CsWriteAt(buf, i, b);
                CsWriteAt(buf, j, a);
            }
        }

        public static void CsReverse(byte[] buf, ushort cnt)
        {
            if (buf == null || cnt < 2) return;
            for (ushort i = 0; i < (ushort)(cnt / 2u); i++)
            {
                ushort j = (ushort)(cnt - 1u - i);
                ushort a = CsReadAt(buf, i);
                ushort b = CsReadAt(buf, j);
                CsWriteAt(buf, i, b);
                CsWriteAt(buf, j, a);
            }
        }

        // Helper functions for random field
        public static void FillBytes(byte[] dst, int len)
        {
            if (dst == null || len == 0) return;
            for (int i = 0; i < len; i++) dst[i] = (byte)UrandChr(256);
        }

        public static void XorBytes(byte[] dst, int len, byte mask)
        {
            if (dst == null || len == 0) return;
            for (int i = 0; i < len; i++) dst[i] ^= mask;
        }

        public static void RotlBytes1(byte[] dst, int len)
        {
            if (dst == null || len < 2) return;
            byte first = dst[0];
            Array.Copy(dst, 1, dst, 0, len - 1);
            dst[len - 1] = first;
        }

        public static void SwapHalves(byte[] dst, int len)
        {
            if (dst == null || len < 2) return;
            int h = len / 2;
            for (int i = 0; i < h; i++)
            {
                byte t = dst[i];
                dst[i] = dst[i + h];
                dst[i + h] = t;
            }
        }

        // Helper for certificate blob
        public static void FillRand(byte[] dst, int n)
        {
            if (dst == null || n == 0) return;
            for (int i = 0; i < n; i++) dst[i] = (byte)UrandCert(256);
        }

        public static void MemRev(byte[] buf, int len)
        {
            if (buf == null || len < 2) return;
            for (int i = 0; i < len / 2; i++)
            {
                byte t = buf[i];
                buf[i] = buf[len - 1 - i];
                buf[len - 1 - i] = t;
            }
        }

        public static void MemXor(byte[] buf, int len, byte mask)
        {
            if (buf == null || len == 0) return;
            for (int i = 0; i < len; i++) buf[i] ^= mask;
        }

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

        // Mix shallow and deep mutations
        public static void MixShallowDeep(byte[] p, int n)
        {
            if (p == null || n == 0) return;
            // Shallow: flip a few bits
            int flips = 1 + _rng.Next(Math.Min(8, n));
            for (int i = 0; i < flips; i++)
            {
                int idx = _rng.Next(n);
                p[idx] ^= (byte)(1 << _rng.Next(8));
            }
            // Deep: sometimes rotate or XOR
            if (_rng.Next(2) == 0)
            {
                if (_rng.Next(2) == 0)
                {
                    MemRotl(p, n, 1 + _rng.Next(7));
                }
                else
                {
                    byte mask = (byte)_rng.Next(256);
                    XorBytes(p, n, mask);
                }
            }
        }
    }

}