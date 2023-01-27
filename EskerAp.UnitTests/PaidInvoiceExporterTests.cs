using EskerAP.Data.Famous;
using EskerAP.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EskerAp.UnitTests
{
	[TestClass]
	public class PaidInvoiceExporterTests
	{

		private IConfigurationRoot _configuration;
		private ApVoucherRepo _repo;
		private UnpaidInvoiceReader _reader;
		private SftpService _sftpService;

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
			var config = new SftpConfig
			{
				Host = _configuration["Esker:SFTP:Host"],
				Port = (int.TryParse(_configuration["Esker:SFTP:Port"], out int port) ? port : 0),
				Username = _configuration["Esker:SFTP:Username"],
				Password = _configuration["Esker:SFTP:Password"]
			};

			_paidInvoiceFolderPath = _configuration["Esker:Folders:PaidInvoices"];
			_unpaidInvoiceFolderPath = _configuration["Esker:Folders:UnpaidInvoices"];
			_sftpService = new SftpService(new MockLogger<SftpService>(), config);
			_repo = new ApVoucherRepo(connectionString, schema, new MockLogger<ApVoucherRepo>());
			_reader = new UnpaidInvoiceReader(new MockLogger<UnpaidInvoiceReader>(), _sftpService);
		}

		[TestMethod]
		public void Export()
		{
			var exporter = new PaidInvoiceExporter(new MockLogger<PaidInvoiceExporter>(), _repo, _reader);
			exporter.ExportPaidInvoices("PW01", _paidInvoiceFolderPath, _unpaidInvoiceFolderPath);
		}
	}
}

