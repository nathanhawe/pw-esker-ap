using Microsoft.Extensions.Logging;
using Renci.SshNet;
using Renci.SshNet.Sftp;
using System;
using System.Collections.Generic;
using System.IO;
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
				_logger.LogDebug("Connecting to {Username}@{Host}:{Port}.", _config.Username, _config.Host, _config.Port);
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
			_logger.LogDebug("Attempting to delete remote file '{remoteFilePath}'.", remoteFilePath);
			throw new NotImplementedException();
		}

		public void Dispose()
		{
			_client.Disconnect();
			_client.Dispose();
		}

		public bool DownloadFile(string remoteFilePath, string localFilePath)
		{
			_logger.LogDebug("Attempting to download remote file '{remoteFilePath}' to '{localFilePath}'.", remoteFilePath, localFilePath);
			throw new NotImplementedException();
		}

		public IEnumerable<SftpFile> ListAllFiles(string remoteDirectory = ".")
		{
			try
			{
				_logger.LogDebug("Attempting to retrieve list of remote files at '{remoteDirectory}'.", remoteDirectory);
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
			try
			{
				_logger.LogDebug("Attempting to upload file '{localFilePath}' to '{remoteFilePath}'.", localFilePath, remoteFilePath);
				using var s = File.OpenRead(localFilePath);
				_client.UploadFile(s, remoteFilePath);
				return true;
			}
			catch(Exception ex)
			{
				_logger.LogError("An exception occurred while attempting to upload the local file '{localFilePath}' to remote file '{remoteFilePath}': {Message}", localFilePath, remoteFilePath, ex.Message);
				return false;
			}
		}

		public bool RenameRemoteFile(string remoteFilePath, string newFilePath)
		{
			try
			{
				_logger.LogDebug("Attempting to rename remote file '{remoteFilePath}' to '{newFilePath}'.", remoteFilePath, newFilePath);
				_client.RenameFile(remoteFilePath, newFilePath);
				return true;
			}
			catch(Exception ex) 
			{
				_logger.LogError("An error occurred attempting to rename the remote file '{remoteFilePath}' to '{newFilePath}': {Message}", remoteFilePath, newFilePath, ex.Message);
				return false;
			}
		}
	}
}
