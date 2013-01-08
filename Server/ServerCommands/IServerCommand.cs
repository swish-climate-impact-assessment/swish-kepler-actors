using System.IO;

namespace Swish.Server.ServerCommands
{
	interface IServerCommand
	{
		string Name { get; }
		void Run(string command, Stream stream, TcpServer server);
	}
}