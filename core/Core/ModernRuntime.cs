using System.Runtime.CompilerServices;
using System.Text;

namespace Peach.Core
{
	internal static class ModernRuntime
	{
		[ModuleInitializer]
		internal static void Initialize()
		{
			System.Text.Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
		}
	}
}
