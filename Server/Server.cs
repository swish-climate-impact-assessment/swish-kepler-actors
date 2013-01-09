using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Swish.Server
{
	public class TcpServer: IDisposable
	{
		public delegate void ServeClientFunction(NetworkStream stream, TcpClient client, TcpServer server);

		private Thread _mainThread;
		public const int DefaultPort = 39390;
		private bool _runMain = false;
		private bool _active = false;
		private int _port = DefaultPort;
		private ServeClientFunction _function;

		public TcpServer(int port, ServeClientFunction function)
		{
			_function = function;
			Port = port;
			Start();
		}

		~TcpServer()
		{
			Dispose(false);
		}

		public void Close()
		{
			Stop();
			Dispose(true);
		}

		private void Stop()
		{
			if (!Running)
			{
				return;
			}

			_runMain = false;
			Thread.Sleep(100);
		}

		void IDisposable.Dispose()
		{
			Dispose(true);
		}

		private void Dispose(bool disposing)
		{
			_runMain = false;
			_mainThread = null;
			_active = false;

			if (!disposing)
			{
				if (_mainThread != null)
				{
					_runMain = false;
				}
				return;
			}
			GC.SuppressFinalize(this);
			_mainThread = null;
		}

		private object _lockItem = new object();
		private void EnterLock()
		{
			Monitor.Enter(_lockItem);
		}

		private void ExitLock()
		{
			Monitor.Exit(_lockItem);
		}

		internal int Port
		{
			get { return _port; }
			private set
			{
				if (value < IPEndPoint.MinPort || value > IPEndPoint.MaxPort)
				{
					throw new Exception("Port (" + value + ") out of range\n\tMin: " + IPEndPoint.MinPort + " Max: " + IPEndPoint.MaxPort);
				}

				if (!_runMain)
				{
					_port = value;
				} else
				{
					Stop();
					_port = value;
					Start();
				}
			}
		}

		public bool Running
		{
			get { return _mainThread != null && _active && _runMain; }
		}

		private void Start()
		{
			try
			{
				EnterLock();
				if (Running)
				{
					return;
				}

				_active = false;
				_runMain = true;
				_mainThread = new Thread(new ThreadStart(ServerMain));
				_mainThread.Name = "ServerMain";
				_mainThread.IsBackground = true;
				_mainThread.Start();
			} finally
			{
				ExitLock();
			}

			DateTime startTime = DateTime.Now;
			TimeSpan timeOut = new TimeSpan(0, 0, 8);
			while (true)
			{
				if (_active)
				{
					break;
				}
				if (_mainThread == null || !_runMain || (DateTime.Now - startTime) >= timeOut)
				{
					Close();
					throw new Exception("Server failed to start on port: " + _port);
				}
				Thread.Sleep(100);
			}

		}

		internal void ServerMain()
		{
			TcpListener tcpListner = null;
			try
			{
				tcpListner = new TcpListener(IPAddress.Any, _port);
				tcpListner.Start();
				_active = true;
				while (_runMain)
				{
					if (!tcpListner.Pending())
					{
						Thread.Sleep(100);
						continue;
					}
					TcpClient socket = tcpListner.AcceptTcpClient();
					if (socket == null)
					{
						Thread.Sleep(100);
						continue;
					}

					Thread thread = new Thread(ServeClient);
					thread.Name = "ServeClient" + DateTime.Now.Ticks.ToString();
					thread.Start(socket);
				}
			} catch (Exception error)
			{
				Console.WriteLine(DateTime.Now.ToLongTimeString() + " " + ExceptionFunctions.Write(error, true));
			} finally
			{
				if (tcpListner != null)
				{
					tcpListner.Stop();
				}
				_active = false;
				_runMain = false;
				_mainThread = null;
			}
		}

		private void ServeClient(object argument)
		{
			try
			{
				using (TcpClient client = argument as TcpClient)
				using (NetworkStream stream = client.GetStream())
				{
					if (client.Client.RemoteEndPoint != null && client.Client.RemoteEndPoint is IPEndPoint)
					{
						IPEndPoint endPoint = client.Client.RemoteEndPoint as IPEndPoint;
						Console.WriteLine(DateTime.Now.ToLongTimeString() + " " + endPoint.Address + ":" + endPoint.Port + " " + DateTime.Now.ToLocalTime());
					} 
					_function(stream, client, this);
					stream.Flush();
					client.Close();
				}
			} catch (Exception error)
			{
				Console.WriteLine(DateTime.Now.ToLongTimeString() + " " + ExceptionFunctions.Write(error, false));
			}
		}

	}
}


