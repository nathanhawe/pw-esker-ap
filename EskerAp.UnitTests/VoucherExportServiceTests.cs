using EskerAP.Data.Famous;
using EskerAP.Data.Quickbase;
using EskerAP.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuickBase.Api;

namespace EskerAp.UnitTests
{
	[TestClass]
	public class VoucherExportServiceTests
	{

		private IConfigurationRoot _configuration;
		private VoucherExportService _service;
		private string _folderPath;

		[TestInitialize]
		public void Setup()
		{
			_configuration ??= ConfigurationHelper.GetIConfigurationRoot();
			var userId = _configuration["Oracle:UserId"];
			var password = _configuration["Oracle:Password"];
			var dataSource = _configuration["Oracle:DataSource"];
			var schema = _configuration["Oracle:Schema"];
			var connectionString = $"User id={userId};Password={password};Data Source={dataSource}";
			var realm = _configuration["Quickbase:Realm"];
			var userToken = _configuration["Quickbase:UserToken"];

			_folderPath = _configuration["Esker:Folders:PaidInvoices"];
			var qbConnection = new QuickBaseConnection(realm, userToken, new MockLogger<QuickBaseConnection>());
			var sftpConfig = new SftpConfig
			{
				Host = _configuration["Esker:SFTP:Host"],
				Port = (int.TryParse(_configuration["Esker:SFTP:Port"], out int port) ? port : 0),
				Username = _configuration["Esker:SFTP:Username"],
				Password = _configuration["Esker:SFTP:Password"]
			};

			_service = new VoucherExportService(
				new MockLogger<VoucherExportService>(),
				new PaidInvoiceExporter(new MockLogger<PaidInvoiceExporter>(), new ApVoucherRepo(connectionString, schema, new MockLogger<ApVoucherRepo>())),
				new SftpService(new MockLogger<SftpService>(), sftpConfig));
		}

		[TestMethod]
		public void ExportPaidInvoices_Integration()
		{
			_service.ExportPaidInvoices(_folderPath, _folderPath, "PW01", 90);
		}

	}
}
