using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace Swish.Server
{
	static class Program
	{
		public static bool Terminate { get; set; }

		private static void Main(string[] arguments)
		{
			try
			{
				string bestVersionFileName = VersionFunctions.BestExecutable;
				if (!string.IsNullOrWhiteSpace(bestVersionFileName) && bestVersionFileName != Application.ExecutablePath)
				{
					ProcessFunctions.Run(bestVersionFileName, string.Join(" ", arguments), null, true, TimeSpan.Zero, true, false, true);
					return;
				}
			} catch (Exception errorException)
			{
				Console.WriteLine(DateTime.Now.ToLongTimeString() + " " + ExceptionFunctions.Write(errorException, false));
			}

			try
			{
				DisplayStartupInformation();

				KillOtherServers();

				if (ProcessFunctions.MonoEnvironment)
				{
					try
					{
						ProcessFunctions.Run("sudo", "iptables -I INPUT -p tcp --dport 39390 -j ACCEPT", null, false, TimeSpan.Zero, true, false, false);
					} catch (Exception errorException2)
					{
						Console.WriteLine(DateTime.Now.ToLongTimeString() + " " + ExceptionFunctions.Write(errorException2, true));
					}
				}

				RunServer();
			} finally
			{
				Console.WriteLine(DateTime.Now.ToLongTimeString() + " " + "End main");
			}
		}

		private static void RunServer()
		{
			string endFileName = Path.Combine(Application.StartupPath, "End");
			if (File.Exists(endFileName))
			{
				File.Delete(endFileName);
			}

			Thread.Sleep(1000);
			TcpServer server = null;
			int port = 39390;
			while (true)
			{
				if (File.Exists(endFileName))
				{
					Console.WriteLine(DateTime.Now.ToLongTimeString() + " " + "End file found");
					using (server) { }
					server = null;
					break;
				}
				if (Terminate)
				{
					using (server) { }
					server = null;
					break;
				}
				try
				{
					if (server == null)
					{
						server = new TcpServer(port, IanServerFunctions.ServeFunction);

						Console.WriteLine(DateTime.Now.ToLongTimeString() + " " + "Server running on port " + port);

						//DateTime startTime = DateTime.Now;
						//TimeSpan timeOut = new TimeSpan(0, 0, 8);
						//while (true)
						//{
						//    if (server.Running)
						//    {
						//        break;
						//    }
						//    if ((DateTime.Now - startTime) >= timeOut)
						//    {
						//        break;
						//    }
						//    Thread.Sleep(100);
						//}
						//if (server.Running)
						//{
						//}
					}

					if (!server.Running)
					{
						Console.WriteLine(DateTime.Now.ToLongTimeString() + " " + "Server not running");
						using (server) { }
						server = null;
						continue;
					}
				} catch (Exception errorException2)
				{
					Console.WriteLine(DateTime.Now.ToLongTimeString() + " " + ExceptionFunctions.Write(errorException2, true));
					Thread.Sleep(5000);
					continue;
				}
				Thread.Sleep(1000);
			}
		}

		private static void KillOtherServers()
		{
			string fileName = "Swish.Server";


			Process[] processes = Process.GetProcesses();
			Process currentProcess = Process.GetCurrentProcess();
			Process parentProcess = ProcessFunctions.ParentProcess(currentProcess);
			for (int processIndex = 0; processIndex < processes.Length; processIndex++)
			{
				Process process = processes[processIndex];
				try
				{
					if ((currentProcess != null && process.Id == currentProcess.Id) || (parentProcess != null && process.Id == parentProcess.Id))
					{
						continue;
					}

					if (process.HasExited || (true
						&& !process.StartInfo.FileName.Contains(fileName)
						&& !process.ProcessName.Contains(fileName)
						)
					)
					{
						continue;
					}
					//} catch { }
					//try
					//{
					Console.WriteLine(DateTime.Now.ToLongTimeString() + " " + "Close process: " + process.ProcessName);
					string endFileName = ProcessFunctions.TryGetFileName(process);
					endFileName = Path.GetDirectoryName(endFileName);
					endFileName = Path.Combine(endFileName, "End");
					File.WriteAllText(endFileName, "");

					if (!ProcessFunctions.WaitProcessEnd(process, new TimeSpan(0, 0, 8)))
					{
						process.Kill();
					}
				} catch //(Exception error)
				{
					//Console.WriteLine(DateTime.Now.ToLongTimeString() + " " + ExceptionFunctions.Write(error));
				}
			}
		}

		private static void DisplayStartupInformation()
		{

			if (!ProcessFunctions.MonoEnvironment)
			{
				Console.WriteLine(DateTime.Now.ToLongTimeString() + " " + "CLR environment");
				Process process = Process.GetCurrentProcess();
				process.PriorityClass = ProcessPriorityClass.Normal;
			} else
			{
				Console.WriteLine(DateTime.Now.ToLongTimeString() + " " + "Mono environment");
			}
			Console.WriteLine(DateTime.Now.ToLongTimeString() + " " + "StartupPath: " + Application.StartupPath);
		}


	}
}
