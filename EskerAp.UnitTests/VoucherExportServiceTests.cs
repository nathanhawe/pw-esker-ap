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
		private UnpaidInvoiceReader _reader;
		private string _paidInvoiceFolderPath;
		private string _unpaidInvoiceFolderPath;

		[TestInitialize]
		public void Setup()
		{
			_configuration ??= ConfigurationHelper.GetIConfigurationRoot();
			var userId = _configuration["Oracle:UserId"];
			var password = _configuration["Oracle:Password"];
			var dataSource = _configuration["Oracle:DataSource"];
			var schema = _configuration["Oracle:Schema"];
			var connectionString = $"User id={userId};Password={password};Data Source={dataSource}";

			_paidInvoiceFolderPath = _configuration["Esker:Folders:PaidInvoices"];
			_unpaidInvoiceFolderPath = _configuration["Esker:Folders:UnpaidInvoices"];
			var sftpConfig = new SftpConfig
			{
				Host = _configuration["Esker:SFTP:Host"],
				Port = (int.TryParse(_configuration["Esker:SFTP:Port"], out int port) ? port : 0),
				Username = _configuration["Esker:SFTP:Username"],
				Password = _configuration["Esker:SFTP:Password"]
			};
			var sftpService = new SftpService(new MockLogger<SftpService>(), sftpConfig);
			_reader = new UnpaidInvoiceReader(new MockLogger<UnpaidInvoiceReader>(), sftpService);
			_service = new VoucherExportService(
				new MockLogger<VoucherExportService>(),
				new PaidInvoiceExporter(new MockLogger<PaidInvoiceExporter>(), new ApVoucherRepo(connectionString, schema, new MockLogger<ApVoucherRepo>()), _reader),
				sftpService);
		}

		[TestMethod]
		public void ExportPaidInvoices_Integration()
		{
			_service.ExportPaidInvoices(_paidInvoiceFolderPath, _paidInvoiceFolderPath, _unpaidInvoiceFolderPath, "PW01");
		}

	}
}
