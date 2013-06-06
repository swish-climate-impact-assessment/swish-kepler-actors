using System;

namespace Swish.Adapters.PostGreSqlPasswordFile
{
	class PostGreSqlPassword
	{

		private string _password = string.Empty;
		public string Password
		{
			get { return _password; }
			set
			{
				if (string.IsNullOrWhiteSpace(value))
				{
					_password = string.Empty;
					return;
				}
				_password = value;
			}
		}

		private string _userName = string.Empty;
		public string UserName
		{
			get { return _userName; }
			set
			{
				if (string.IsNullOrWhiteSpace(value))
				{
					_userName = string.Empty;
					return;
				}
				_userName = value;
			}
		}


		private string _databaseName = string.Empty;
		public string DatabaseName
		{
			get { return _databaseName; }
			set
			{
				if (string.IsNullOrWhiteSpace(value))
				{
					_databaseName = string.Empty;
					return;
				}
				_databaseName = value;
			}
		}


		public int Port { get; set; }

		private string _address = string.Empty;
		public string Address
		{
			get { return _address; }
			set
			{
				if (string.IsNullOrWhiteSpace(value))
				{
					_address = string.Empty;
					return;
				}
				_address = value;
			}
		}


	}
}
