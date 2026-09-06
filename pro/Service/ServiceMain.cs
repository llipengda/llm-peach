using Peach.Pro.Core.Runtime;

namespace PeachService
{
	public class ServiceMain
	{
		static int Main(string[] args)
		{
			using (var service = new Service())
			{
				return service.Run(args);
			}
		}
	}
}
