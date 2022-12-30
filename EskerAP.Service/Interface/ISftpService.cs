using Renci.SshNet.Sftp;
using System.Collections.Generic;

namespace EskerAP.Service.Interface
{
	public interface ISftpService
	{
		public IEnumerable<SftpFile> ListAllFiles(string remoteDirectory = ".");
		public bool UploadFile(string localFilePath, string remoteFilePath);
		public bool DownloadFile(string remoteFilePath, string localFilePath);
		public bool DeleteRemoteFile(string remoteFilePath);

	}
}
