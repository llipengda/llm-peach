using System.Net.Sockets;
using System.Runtime.InteropServices;
using Mono.Unix;
using Peach.Core;

namespace Peach.Pro.OS.Linux
{
	[PlatformImpl(Platform.OS.Linux)]
	public class NetworkAdapterImpl : NetworkAdapter
	{
		#region P/Invokes

		const int SIOCGIFMTU = 0x8921;
		const int SIOCSIFMTU = 0x8922;

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
		struct IfReq
		{
			public IfReq(string ifr_name)
			{
				this.ifr_name = ifr_name;
				this.ifru_mtu = 0;
			}

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
			public string ifr_name;
			public uint ifru_mtu;
		}

		[DllImport("libc", SetLastError = true)]
		private static extern int ioctl(int fd, int request, ref IfReq mtu);

		#endregion

		Socket socket;
		IfReq ifr;

		public NetworkAdapterImpl(string name)
			: base(name)
		{
			socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
			ifr = new IfReq(name);
		}

		public override void Dispose()
		{
			socket.Dispose();
			socket = null;
		}

		public override uint? MTU
		{
			get
			{
				int ret = ioctl(socket.Handle.ToInt32(), SIOCGIFMTU, ref ifr);
				UnixMarshal.ThrowExceptionForLastErrorIf(ret);
				return ifr.ifru_mtu;
			}
			set
			{
				if (!MTU.HasValue)
					return;

				ifr.ifru_mtu = value.Value;
				int ret = ioctl(socket.Handle.ToInt32(), SIOCSIFMTU, ref ifr);
				UnixMarshal.ThrowExceptionForLastErrorIf(ret);
			}
		}
	}
}
