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
			_folderPath = _configuration["Esker:Folders:PaidInvoices"];

			_repo = new ApVoucherRepo(connectionString, schema, new MockLogger<ApVoucherRepo>());
		}

		[TestMethod]
		public void Export()
		{
			var exporter = new PaidInvoiceExporter(new MockLogger<PaidInvoiceExporter>(), _repo);
			exporter.ExportPaidInvoices("PW01", _folderPath, 90);
		}
	}
}
