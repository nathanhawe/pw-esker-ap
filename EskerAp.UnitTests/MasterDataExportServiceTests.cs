using EskerAP.Data.Famous;
using EskerAP.Data.Quickbase;
using EskerAP.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuickBase.Api;

namespace EskerAp.UnitTests
{
	[TestClass]
	public class MasterDataExportServiceTests
	{

		private IConfigurationRoot _configuration;
		private MasterDataExportService _service;
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

			_folderPath = _configuration["Esker:Folders:MasterData"];
			var qbConnection = new QuickBaseConnection(realm, userToken, new MockLogger<QuickBaseConnection>());
			var sftpConfig = new SftpConfig
			{
				Host = _configuration["Esker:SFTP:Host"],
				Port = (int.TryParse(_configuration["Esker:SFTP:Port"], out int port) ? port : 0),
				Username = _configuration["Esker:SFTP:Username"],
				Password = _configuration["Esker:SFTP:Password"]
			};

			_service = new MasterDataExportService(
				new MockLogger<MasterDataExportService>(),
				new CostCenterExporter(new MockLogger<CostCenterExporter>(), new CaCostCenterRepo(connectionString, schema, new MockLogger<CaCostCenterRepo>())),
				new GLAccountExporter(new MockLogger<GLAccountExporter>(), new GLAccountRepo(connectionString, schema, new MockLogger<GLAccountRepo>())),
				new PaymentTermsExporter(new MockLogger<PaymentTermsExporter>(), new ApPayTermsRepo(connectionString, schema, new MockLogger<ApPayTermsRepo>())),
				new PhaseExporter(new MockLogger<PhaseExporter>(), new CaPhaseRepo(connectionString, schema, new MockLogger<CaPhaseRepo>())),
				new PurchaseOrderExporter(
					new MockLogger<PurchaseOrderExporter>(),
					new PurchaseOrderHeaderRepo(connectionString, schema, new MockLogger<PurchaseOrderHeaderRepo>()),
					new PurchaseOrderDetailRepo(connectionString, schema, new MockLogger<PurchaseOrderDetailRepo>()),
					new PurchaseOrdersRepo(qbConnection),
					new ItemsRepo(qbConnection)),
				new VendorExporter(new MockLogger<VendorExporter>(), new ApVendorRepo(connectionString, schema, new MockLogger<ApVendorRepo>())),
				new SftpService(new MockLogger<SftpService>(), sftpConfig));




		}

		[TestMethod]
		public void ExportMasterData_IntegrationTest()
		{
			_service.ExportMasterData(_folderPath, _folderPath, "PW01");
		}

		[TestMethod]
		public void ExportMasterData_POOnly_IntegrationTest()
		{
			_service.ExportMasterData(_folderPath, _folderPath, "PW01", true);
		}
	}
}
