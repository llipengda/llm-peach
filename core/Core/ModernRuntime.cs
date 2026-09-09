using System.Runtime.CompilerServices;
using System.Text;

namespace Peach.Core
{
	internal static class ModernRuntime
	{
#pragma warning disable CA2255 // This library must register legacy code-page support before use.
		[ModuleInitializer]
#pragma warning restore CA2255
		internal static void Initialize()
		{
			System.Text.Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
		}
	}
}
