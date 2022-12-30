using EskerAP.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace EskerAp.UnitTests
{
	[TestClass]
	public class SftpServiceTests
	{
		IConfigurationRoot _configuration;
		SftpConfig _config;
		string _folderPath;

		[TestInitialize]
		public void Setup()
		{

			_configuration ??= ConfigurationHelper.GetIConfigurationRoot();
			_folderPath = _configuration["Esker:Folders:MasterData"];

			_config = new SftpConfig
			{
				Host = _configuration["Esker:SFTP:Host"],
				Port = (int.TryParse(_configuration["Esker:SFTP:Port"], out int port) ? port : 0),
				Username = _configuration["Esker:SFTP:Username"],
				Password = _configuration["Esker:SFTP:Password"]
			};
		}

		[TestMethod]
		public void ListAllFiles()
		{
			var service = new SftpService(new MockLogger<SftpService>(), _config);
			foreach(var file in service.ListAllFiles(_folderPath).Where(x => x.IsRegularFile))
			{
				Console.WriteLine($"{file.FullName} - {file.Name}");
			}
		}
	}
}
