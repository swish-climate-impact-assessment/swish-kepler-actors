using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Net.Sockets;

namespace Swish.Server.ServerCommands
{
	public class TerminateServerCommand: IServerCommand
	{
		public const string Command = "Terminate";
		public string Name { get { return Command; } }

		public void Run(string command, Stream stream, TcpServer server)
		{
			Terminate(server);
		}

		internal static void Terminate(TcpServer server)
		{
			Thread.CurrentThread.IsBackground = true;

			Swish.Server.Program.Terminate = true;

			server.Close();
			string endFileName = Path.Combine(Application.StartupPath, "End");
			File.WriteAllText(endFileName, "");

			TimeSpan timeOut = new TimeSpan(0, 0, 8);
			DateTime startTime = DateTime.Now;
			while (server.Running)
			{
				if ((DateTime.Now - startTime) > timeOut)
				{
					break;
				}
				Thread.Sleep(100);
			}

			if (!server.Running)
			{
				return;
			}

			Thread killThread = new Thread(Terminate);
			killThread.IsBackground = true;
			killThread.Start();

			Application.Exit();
		}

		private static void Terminate()
		{
			Process.GetCurrentProcess().Kill();
		}

		public static void Terminate(string host)
		{
			int port = 39390;
			using (TcpClient connection = new TcpClient(host, port))
			using (NetworkStream stream = connection.GetStream())
			{
				string url = "/command?" + TerminateServerCommand.Command;

				IanServerFunctions.WriteLine(stream, "GET" + " " + url + " " + "HTTP1.1");
				IanServerFunctions.WriteLine(stream, string.Empty);
			}
		}



	}
}