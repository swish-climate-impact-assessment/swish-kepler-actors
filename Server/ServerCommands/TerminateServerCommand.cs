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
		public const string TerminateCommand = "Terminate";
		public string Name { get { return TerminateCommand; } }

		public void Run(string command, Stream stream, TcpServer server)
		{
			Terminate(server);
		}

		internal static void Terminate(TcpServer server)
		{
			Console.WriteLine(DateTime.Now.ToLongTimeString() + " " + "Terminate");
			Thread.CurrentThread.IsBackground = true;

			Console.WriteLine(DateTime.Now.ToLongTimeString() + " " + "Terminate = true");
			Swish.Server.Program.Terminate = true;

			Console.WriteLine(DateTime.Now.ToLongTimeString() + " " + "server.Close()");
			server.Close();
			string endFileName = Path.Combine(Application.StartupPath, "End");
			File.WriteAllText(endFileName, "");

			TimeSpan timeOut = new TimeSpan(0, 0, 8);
			DateTime startTime = DateTime.Now;
			while (server.Running)
			{
				if ((DateTime.Now - startTime) > timeOut)
				{
					Console.WriteLine(DateTime.Now.ToLongTimeString() + " " + "Timeout");
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

			Console.WriteLine(DateTime.Now.ToLongTimeString() + " " + "Application.Exit()");
			Application.Exit();
		}

		private static void Terminate()
		{
			Console.WriteLine(DateTime.Now.ToLongTimeString() + " " + "Process.GetCurrentProcess().Kill()");
			Process.GetCurrentProcess().Kill();
		}

		public static void Terminate(string host)
		{
			int port = 39390;
			using (TcpClient connection = new TcpClient(host, port))
			using (NetworkStream stream = connection.GetStream())
			{
				string url = "/command?" + TerminateServerCommand.TerminateCommand;

				IanServerFunctions.WriteLine(stream, "GET" + " " + url + " " + "HTTP1.1");
				IanServerFunctions.WriteLine(stream, string.Empty);
			}
		}



	}
}