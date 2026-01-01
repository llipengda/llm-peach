using System;
using System.Runtime.InteropServices;
using System.Text;
namespace Peach.Pro.Core.Mutators.LLM.RTSP
{
    // C Function: set_cstr
    // 安全字符串复制，防越界并确保 NUL 结尾
    public static void SetCstr(byte[] dst, int cap, string s)
    {
        if (dst == null || cap == 0) return;
        if (string.IsNullOrEmpty(s))
        {
            if (dst.Length > 0) dst[0] = 0;
            return;
        }

        // 防越界复制，并确保 NUL 结尾
        int i = 0;
        byte[] srcBytes = Encoding.UTF8.GetBytes(s);
        for (; i + 1 < cap && i < srcBytes.Length && srcBytes[i] != 0; ++i)
        {
            dst[i] = srcBytes[i];
        }
        dst[i] = 0;
    }

    // C Function: set_colon_space
    // 设置冒号空格分隔符
    public static void SetColonSpace(byte[] cs, int separatorLen)
    {
        // 假设 RTSP_SEPARATOR_LEN >= 2
        if (cs == null || cs.Length < 2) return;
        cs[0] = (byte)':';
        cs[1] = (byte)' ';
        if (separatorLen > 2 && cs.Length > 2)
        {
            cs[2] = 0;
        }
    }

    // C Function: set_crlf
    // 设置 CRLF 行结束符
    public static void SetCrlf(byte[] crlf, int crlfLen)
    {
        // 假设 RTSP_CRLF_LEN >= 2
        if (crlf == null || crlf.Length < 2) return;
        crlf[0] = (byte)'\r';
        crlf[1] = (byte)'\n';
        if (crlfLen > 2 && crlf.Length > 2)
        {
            crlf[2] = 0;
        }
    }

    // C Function: rng_seed
    // 随机数种子初始化
    private static bool _rngSeeded = false;
    private static Random _random = null;
    
    public static void RngSeed()
    {
        if (!_rngSeeded)
        {
            _random = new Random((int)DateTime.Now.Ticks);
            _rngSeeded = true;
        }
    }
    
    // 获取随机数生成器实例
    public static Random GetRandom()
    {
        if (!_rngSeeded)
        {
            RngSeed();
        }
        return _random ?? new Random();
    }

    // C Function: get_request_uri_ptr
    // 获取各类型中的 request_uri 指针
    // 注意：在 C# 中，这需要根据实际的 rtsp_packet_t 结构来实现
    // 这里提供一个通用接口，实际使用时需要根据具体结构体调整
    public static IntPtr GetRequestUriPtr(IntPtr p, int rtspType)
    {
        // 此函数需要根据实际的 rtsp_packet_t 结构体布局来实现
        // 使用 Marshal 或 unsafe 代码来访问结构体字段
        // 这里仅提供接口定义，具体实现需要根据实际结构体定义
        return IntPtr.Zero;
    }

    // C Function: set_uri
    // 安全写入 URI（截断到 RTSP_URI_LEN-1，始终以 '\0' 结尾）
    public static void SetUri(byte[] dst, string src, int uriLen = 256)
    {
        if (dst == null) return;
        int cap = uriLen;
        if (string.IsNullOrEmpty(src))
        {
            if (dst.Length > 0) dst[0] = 0;
            return;
        }

        byte[] srcBytes = Encoding.UTF8.GetBytes(src);
        int n = srcBytes.Length;
        if (n >= cap) n = cap - 1;

        int copyLen = Math.Min(n, dst.Length);
        Buffer.BlockCopy(srcBytes, 0, dst, 0, copyLen);
        if (copyLen < dst.Length)
        {
            dst[copyLen] = 0;
        }
    }

    // C Function: make_repeated_char
    // 生成重复字符的超长路径
    public static void MakeRepeatedChar(byte[] outBuf, int cap, byte ch, int count)
    {
        if (outBuf == null || cap == 0) return;
        if (count >= cap) count = cap - 1;

        for (int i = 0; i < count; ++i)
        {
            if (i < outBuf.Length)
            {
                outBuf[i] = ch;
            }
        }
        if (count < outBuf.Length)
        {
            outBuf[count] = 0;
        }
    }

    // C Function: rand_digits
    // 随机数字字符串（用于端口、随机路径片段）
    public static void RandDigits(byte[] outBuf, int cap, int digits, Random random = null)
    {
        if (outBuf == null || cap == 0) return;
        if (random == null) random = GetRandom();
        if (digits >= cap) digits = cap - 1;

        for (int i = 0; i < digits; ++i)
        {
            if (i < outBuf.Length)
            {
                outBuf[i] = (byte)('0' + (random.Next(10)));
            }
        }
        if (digits < outBuf.Length)
        {
            outBuf[digits] = 0;
        }
    }

    // C Function: num_ops
    // 返回操作数量（针对 URI 操作数组）
    public static int NumOps<T>(T[] ops)
    {
        return ops != null ? ops.Length : 0;
    }

    // C Function: weighted_pick_idx
    // 简单按权重选择（总权重<=0时退回0）
    public static int WeightedPickIdx(int[] weights, int n, Random random = null)
    {
        if (weights == null || n <= 0) return 0;
        if (random == null) random = GetRandom();

        long total = 0;
        for (int i = 0; i < n; ++i)
        {
            total += (weights[i] > 0 ? weights[i] : 0);
        }

        if (total <= 0) return 0;

        long r = random.Next((int)total);
        long acc = 0;

        for (int i = 0; i < n; ++i)
        {
            int wi = (weights[i] > 0 ? weights[i] : 0);
            if (wi == 0) continue;
            acc += wi;
            if (r < acc) return i;
        }

        return 0;
    }

    // C Function: get_cseq_header_ptr
    // 获取 CSeq header 指针
    // 注意：在 C# 中需要根据实际的 rtsp_packet_t 结构来实现
    public static IntPtr GetCseqHeaderPtr(IntPtr pkt, int rtspType)
    {
        // 此函数需要根据实际的 rtsp_packet_t 结构体布局来实现
        // 使用 Marshal 或 unsafe 代码来访问结构体字段
        return IntPtr.Zero;
    }

    // C Function: ensure_header_shape
    // 确保 CSeq header 的形状（设置默认值）
    public static void EnsureHeaderShape(byte[] name, int nameCap, byte[] colonSpace, int separatorLen, byte[] crlf, int crlfLen, ref int number)
    {
        if (name == null || colonSpace == null || crlf == null) return;

        if (name.Length > 0 && name[0] == 0)
        {
            byte[] cseqBytes = Encoding.UTF8.GetBytes("CSeq");
            int copyLen = Math.Min(cseqBytes.Length, nameCap - 1);
            Buffer.BlockCopy(cseqBytes, 0, name, 0, copyLen);
            if (copyLen < name.Length)
            {
                name[copyLen] = 0;
            }
        }

        SetColonSpace(colonSpace, separatorLen);
        SetCrlf(crlf, crlfLen);

        if (number < 1) number = 1; // 正常合法值至少为1
    }

    // C Function: get_accept_ptr
    // 获取 Accept header 指针
    public static IntPtr GetAcceptPtr(IntPtr p, int rtspType)
    {
        // 此函数需要根据实际的 rtsp_packet_t 结构体布局来实现
        return IntPtr.Zero;
    }

    // C Function: acc_ops_count
    // 返回 Accept 操作数量
    public static int AccOpsCount<T>(T[] ops)
    {
        return ops != null ? ops.Length : 0;
    }

    // C Function: weighted_pick_idx_accept
    // Accept 加权选择索引
    public static int WeightedPickIdxAccept(int[] weights, int nOps, Random random = null)
    {
        if (weights == null || nOps == 0)
        {
            return 0; // 防御式返回
        }
        if (random == null) random = GetRandom();

        long total = 0;

        // 1) 计算所有正权重之和
        for (int i = 0; i < nOps; i++)
        {
            if (weights[i] > 0)
            {
                total += weights[i];
            }
        }

        // 2) 如果所有权重都 <= 0，则退化成均匀随机
        if (total <= 0)
        {
            return random.Next(nOps);
        }

        // 3) 在 [0, total) 中选一个随机数
        long r = random.Next((int)total);
        long acc = 0;

        // 4) 累加权重，找到落点对应的下标
        for (int i = 0; i < nOps; i++)
        {
            if (weights[i] <= 0)
            {
                continue; // 权重 <= 0 的 op 不参与选择
            }
            acc += weights[i];
            if (r < acc)
            {
                return i;
            }
        }

        // 理论上不会走到这里，保险起见返回最后一个
        return nOps - 1;
    }

    // C Function: get_ae_ptr
    // 获取 Accept-Encoding header 指针
    public static IntPtr GetAePtr(IntPtr p, int rtspType)
    {
        // 此函数需要根据实际的 rtsp_packet_t 结构体布局来实现
        return IntPtr.Zero;
    }

    // C Function: ae_ops_count
    // 返回 Accept-Encoding 操作数量
    public static int AeOpsCount<T>(T[] ops)
    {
        return ops != null ? ops.Length : 0;
    }

    // C Function: get_al_ptr
    // 获取 Accept-Language header 指针
    public static IntPtr GetAlPtr(IntPtr p, int rtspType)
    {
        // 此函数需要根据实际的 rtsp_packet_t 结构体布局来实现
        return IntPtr.Zero;
    }

    // C Function: al_ops_count
    // 返回 Accept-Language 操作数量
    public static int AlOpsCount<T>(T[] ops)
    {
        return ops != null ? ops.Length : 0;
    }

    // C Function: get_auth_ptr
    // 获取 Authorization header 指针
    public static IntPtr GetAuthPtr(IntPtr p, int rtspType)
    {
        // 此函数需要根据实际的 rtsp_packet_t 结构体布局来实现
        return IntPtr.Zero;
    }

    // C Function: auth_ops_count
    // 返回 Authorization 操作数量
    public static int AuthOpsCount<T>(T[] ops)
    {
        return ops != null ? ops.Length : 0;
    }

    // C Function: get_bw_ptr
    // 获取 Bandwidth header 指针
    public static IntPtr GetBwPtr(IntPtr p, int rtspType)
    {
        // 此函数需要根据实际的 rtsp_packet_t 结构体布局来实现
        return IntPtr.Zero;
    }

    // C Function: bw_ops_count
    // 返回 Bandwidth 操作数量
    public static int BwOpsCount<T>(T[] ops)
    {
        return ops != null ? ops.Length : 0;
    }

    // C Function: get_bs_ptr
    // 获取 Blocksize header 指针
    public static IntPtr GetBsPtr(IntPtr p, int rtspType)
    {
        // 此函数需要根据实际的 rtsp_packet_t 结构体布局来实现
        return IntPtr.Zero;
    }

    // C Function: bs_ops_count
    // 返回 Blocksize 操作数量
    public static int BsOpsCount<T>(T[] ops)
    {
        return ops != null ? ops.Length : 0;
    }

    // C Function: ensure_accept_shape
    // 确保 Accept header 的形状
    public static void EnsureAcceptShape(byte[] name, int nameCap, byte[] colonSpace, int separatorLen, ref byte slash)
    {
        if (name == null || colonSpace == null) return;

        if (name.Length > 0 && name[0] == 0)
        {
            byte[] acceptBytes = Encoding.UTF8.GetBytes("Accept");
            int copyLen = Math.Min(acceptBytes.Length, nameCap - 1);
            Buffer.BlockCopy(acceptBytes, 0, name, 0, copyLen);
            if (copyLen < name.Length)
            {
                name[copyLen] = 0;
            }
        }

        SetColonSpace(colonSpace, separatorLen);
        if (slash == 0) slash = (byte)'/';
    }
}

