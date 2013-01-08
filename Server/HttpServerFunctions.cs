using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using Swish.Server.ServerCommands;

namespace Swish.Server
{
	public static class IanServerFunctions
	{
		public static void ServeFunction(NetworkStream stream, TcpClient client, TcpServer server)
		{
			string method;
			string url;
			ReadRequest(out  method, out  url, stream);
			SortedList<string, string> httpHeaders = ReadHeaders(stream);
			Console.WriteLine(DateTime.Now.ToLongTimeString() + " " + "Method: " + method + " " + "Url: " + url);
			if (!method.Equals("GET"))
			{
				Console.WriteLine(DateTime.Now.ToLongTimeString() + " " + "Bad method");
				//WriteOk(stream, "text/html");
				//ServeFile(stream, "index.html");
				return;
			}

			// Get
			string cleanUrl = url.Trim().ToLower();
			if (string.IsNullOrWhiteSpace(cleanUrl) || cleanUrl.Length == 0 || cleanUrl == "/" || cleanUrl == "\\")
			{
				Console.WriteLine(DateTime.Now.ToLongTimeString() + " " + "Bad url");
				if (File.Exists("index.html"))
				{
					WriteOk(stream, "text/html");
					ServeFile(stream, "index.html");
				}
				return;
			}
			cleanUrl = cleanUrl.Substring(1);

			if (url == "/menu")
			{
				Console.WriteLine(DateTime.Now.ToLongTimeString() + " " + "Menu");
				ServeMenu(stream);
				return;
			} else if (url.StartsWith("/command"))
			{
				Console.WriteLine(DateTime.Now.ToLongTimeString() + " " + "Command");
				ServerCommandFunctions.ServeCommand(stream, url, server);
				return;
			}

			if (url.Contains(".."))
			{
				Console.WriteLine(DateTime.Now.ToLongTimeString() + " " + "Bad url");
				if (File.Exists("index.html"))
				{
					WriteOk(stream, "text/html");
					ServeFile(stream, "index.html");
				}
				return;
			}

			string fileUrl = FileFunctions.AdjustFileName(cleanUrl);

			if (!File.Exists(fileUrl))
			{
				Console.WriteLine(DateTime.Now.ToLongTimeString() + " " + "404: " + cleanUrl);
				if (File.Exists("index.html"))
				{
					WriteOk(stream, "text/html");
					ServeFile(stream, "index.html");
				}
				return;
			}

			string extension = Path.GetExtension(fileUrl);
			string type;
			if (extension != ".png" && extension != ".jpg")
			{
				type = "image/jpeg";
			} else if (extension == ".html")
			{
				type = "text/html";
			} else
			{
				type = "application/text";
			}

			WriteOk(stream, type);
			ServeFile(stream, fileUrl);
		}

		public static void ServeFile(Stream stream, string fileName)
		{
			StreamFunctions.Import(stream, fileName);
		}

		private static void ServeMenu(Stream stream)
		{
			WriteOk(stream, "text/html");
			WriteLine(stream, "<html><body>");
			WriteLine(stream, "menu3");
			WriteLine(stream, "</body></html>");
		}

		public static void ReadRequest(out string method, out string url, Stream stream)
		{
			string request = ReadLine(stream);
			string[] tokens = request.Split(' ');
			if (tokens.Length < 3)
			{
				throw new Exception("invalid http request line: " + request);
			}
			method = tokens[0].ToUpper();

			url = string.Empty;
			for (int tokenIndex = 1; tokenIndex + 1 < tokens.Length; tokenIndex++)
			{
				url += tokens[tokenIndex];
				if (tokenIndex + 2 < tokens.Length)
				{
					url += " ";
				}
			}
			string http_protocol_versionstring = tokens[2];
		}

		public static SortedList<string, string> ReadHeaders(Stream stream)
		{
			SortedList<string, string> httpHeaders = new SortedList<string, string>();
			while (true)
			{
				string line = ReadLine(stream);
				if (line == null || line.Equals(""))
				{
					break;
				}

				int separator = line.IndexOf(':');
				if (separator == -1)
				{
					throw new Exception("invalid http header line: " + line);
				}

				string name = line.Substring(0, separator);
				int pos = separator + 1;
				while ((pos < line.Length) && (line[pos] == ' '))
				{
					pos++; // strip any spaces
				}

				string value = line.Substring(pos, line.Length - pos);
				httpHeaders.Add(name, value);
			}

			return httpHeaders;
		}

		public static void WriteOk(Stream stream, string type)
		{
			WriteLine(stream, "HTTP/1.1 200 OK");
			WriteLine(stream, "Content-Type: " + type);
			WriteLine(stream, "Connection: close");
			WriteLine(stream, "");
		}

		internal static void Write404(Stream stream)
		{
			WriteLine(stream, "HTTP/1.1 Error 404");
			WriteLine(stream, "Connection: close");
			WriteLine(stream, "");
		}

		private static void WriteRedirect(Stream stream, string path)
		{
			WriteLine(stream, "HTTP/1.1 301 Temporary Redirect");
			WriteLine(stream, "Location: " + path);

			WriteLine(stream, "Content-Type: text/html");
			WriteLine(stream, "Connection: close");
			WriteLine(stream, "");
		}

		private static string ReadLine(Stream stream)
		{
			int next_char;
			string data = "";
			while (true)
			{
				next_char = stream.ReadByte();
				if (next_char == '\n') { break; }
				if (next_char == '\r') { continue; }
				if (next_char == -1) { Thread.Sleep(1); continue; };
				data += Convert.ToChar(next_char);
			}
			return data;
		}

		public static void WriteLine(Stream stream, string text)
		{
			byte[] array = System.Text.ASCIIEncoding.ASCII.GetBytes(text + Environment.NewLine);
			stream.Write(array, 0, array.Length);
		}

		internal static void WriteBlankOK(Stream stream)
		{
			WriteOk(stream, "text/html");
			WriteLine(stream, "");
		}
	}
}



