using System.IO;
using System;
using System.Collections.Generic;
using System.Xml;

namespace Swish
{
	public static class MetadataFunctions
	{
		public static string FileName(string fileName)
		{
			if (string.IsNullOrWhiteSpace(fileName))
			{
				throw new ArgumentException("fileName");
			}
			return fileName + "_SwishMetadata.xml";
		}

		public static bool Exists(string fileName)
		{
			string metadataFileName = FileName(fileName);
			return FileFunctions.FileExists(metadataFileName);
		}

		public static void Save(string fileName, List<MetadataRecord> metadata)
		{
			XmlDocument document = new XmlDocument();

			XmlElement _metadata = document.CreateElement("SwishMetadata");
			for (int recordIndex = 0; recordIndex < metadata.Count; recordIndex++)
			{
				MetadataRecord record = metadata[recordIndex];
				XmlElement _record = document.CreateElement("Record");
				AddElement(document, _record, "Operation", record.Operation);
				if (record.Arguments.Count > 0)
				{
					XmlElement arguments = document.CreateElement("Arguments");
					for (int argumentIndex = 0; argumentIndex < record.Arguments.Count; argumentIndex++)
					{
						Tuple<string, string> argument = record.Arguments[argumentIndex];
						XmlElement _argument = document.CreateElement("Argument");
						AddElement(document, _argument, "Name", argument.Item1);
						AddElement(document, _argument, "Value", argument.Item2);
						arguments.AppendChild(_argument);
					}
					_record.AppendChild(arguments);
				}
				AddElement(document, _record, "User", record.User);
				if (record.Time != default(DateTime))
				{
					AddElement(document, _record, "Time", record.Time.ToShortDateString() + " " + record.Time.ToLongTimeString());
				}
				AddElement(document, _record, "ComputerName", record.ComputerName);
				_metadata.AppendChild(_record);
			}
			document.AppendChild(_metadata);

			string metadataFileName = FileName(fileName);
			document.Save(metadataFileName);
		}

		private static void AddElement(XmlDocument document, XmlElement parent, string name, string value)
		{
			if (!string.IsNullOrWhiteSpace(value))
			{
				XmlElement operation = document.CreateElement(name);
				operation.AppendChild(document.CreateTextNode(value));
				parent.AppendChild(operation);
			}
		}

		public static List<MetadataRecord> Load(string fileName)
		{
			XmlDocument document = new XmlDocument();
			string metadataFileName = FileName(fileName);
			document.Load(metadataFileName);

			XmlElement metadataElement = document.DocumentElement;
			if (metadataElement.Name != "SwishMetadata")
			{
				throw new Exception("Expected \"SwishMetadata\" element");
			}

			List<MetadataRecord> metadata = new List<MetadataRecord>();
			for (int recordIndex = 0; recordIndex < metadataElement.ChildNodes.Count; recordIndex++)
			{
				XmlNode element = metadataElement.ChildNodes[recordIndex];
				if (element.Name != "Record")
				{
					throw new Exception("Expected \"Record\" element");
				}

				MetadataRecord record = ReadRecord(element);
				metadata.Add(record);
			}

			return metadata;
		}

		private static MetadataRecord ReadRecord(XmlNode recordElement)
		{
			MetadataRecord record = new MetadataRecord();
			for (int recordIndex = 0; recordIndex < recordElement.ChildNodes.Count; recordIndex++)
			{
				XmlNode element = recordElement.ChildNodes[recordIndex];
				switch (element.Name)
				{
				case "Operation":
					record.Operation = element.InnerText;
					break;

				case "Arguments":
					record.Arguments = ReadArguments(element);
					break;

				case "User":
					record.User = element.InnerText;
					break;

				case "Time":
					string dateTimeString = element.InnerText;
					record.Time = DateTime.Parse(dateTimeString);
					break;

				case "ComputerName":
					record.ComputerName = element.InnerText;
					break;

				default:
					throw new Exception("Unexpected element found \"" + element.Name + "\"");
				}
			}

			return record;
		}

		private static List<Tuple<string, string>> ReadArguments(XmlNode argumentElement)
		{
			List<Tuple<string, string>> arguments = new List<Tuple<string, string>>();
			for (int recordIndex = 0; recordIndex < argumentElement.ChildNodes.Count; recordIndex++)
			{
				XmlNode element = argumentElement.ChildNodes[recordIndex];
				if (element.Name != "Argument")
				{
					throw new Exception("Expected \"Argument\"");
				}

				string name = string.Empty;
				string value = string.Empty;
				for (int itemIndex = 0; itemIndex < element.ChildNodes.Count; itemIndex++)
				{
					XmlNode valueElement = element.ChildNodes[itemIndex];
					switch (valueElement.Name)
					{
					case "Name":
						name = valueElement.InnerText;
						break;

					case "Value":
						value = valueElement.InnerText;
						break;

					default:
						throw new Exception("Unknown element found");
					}
				}

				if (string.IsNullOrWhiteSpace(name))
				{
					throw new Exception("Name undefined");
				}

				arguments.Add(new Tuple<string, string>(name, value));
			}

			return arguments;
		}

	}
}
