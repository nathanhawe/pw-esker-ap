using Microsoft.Extensions.Logging;
using Renci.SshNet;
using Renci.SshNet.Sftp;
using System;
using System.Collections.Generic;
using System.Text;

namespace EskerAP.Service
{
	public class SftpConfig
	{
		public string Host { get; set; }
		public int Port { get; set; }
		public string Username { get; set; }
		public string Password { get; set; }
	}

	public class SftpService : Interface.ISftpService, IDisposable
	{
		private readonly ILogger<SftpService> _logger;
		private readonly SftpConfig _config;
		private SftpClient _client;

		public SftpService(ILogger<SftpService> logger, SftpConfig config) 
		{ 
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_config = config ?? throw new ArgumentNullException(nameof(config));

			try
			{
				_client = new SftpClient(_config.Host, (_config.Port == 0 ? 22 : _config.Port), _config.Username, _config.Password);
				_client.Connect();
			}
			catch(Exception ex)
			{
				_logger.LogError("An exception occurred while attempting to connect to the remote SFTP Server: {Message}", ex.Message);
			}

		}

		public bool DeleteRemoteFile(string remoteFilePath)
		{
			throw new NotImplementedException();
		}

		public void Dispose()
		{
			_client.Disconnect();
			_client.Dispose();
		}

		public bool DownloadFile(string remoteFilePath, string localFilePath)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<SftpFile> ListAllFiles(string remoteDirectory = ".")
		{
			try
			{
				return _client.ListDirectory(remoteDirectory);
			}
			catch(Exception ex)
			{
				_logger.LogError("An exception occurred while attempting to list files at the remote directory '{Directory}': {Message}", remoteDirectory, ex.Message);
				return null;
			}
		}

		public bool UploadFile(string localFilePath, string remoteFilePath)
		{
			throw new NotImplementedException();
		}
	}
}
