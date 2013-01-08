using Swish.Server;

namespace Swish.Tests
{
	class TcpServerTests
	{
		internal void GetIndex()
		{
			int port = TcpServer.DefaultPort;
			using( TcpServer server = new TcpServer(port, IanServerFunctions.ServeFunction))
			{

			}
		}
	}
}
