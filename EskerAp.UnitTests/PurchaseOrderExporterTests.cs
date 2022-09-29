using EskerAP.Data.Famous;
using EskerAP.Data.Quickbase;
using EskerAP.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuickBase.Api;

namespace EskerAp.UnitTests
{
	[TestClass]
	public class PurchaseOrderExporterTests
	{

		private IConfigurationRoot _configuration;
		private IQuickBaseConnection _quickbaseConnection;
		private EskerAP.Data.Famous.IPurchaseOrderHeaderRepo _famousHeaderRepo;
		private EskerAP.Data.Famous.IPurchaseOrderDetailRepo _famousDetailRepo;
		private EskerAP.Data.Quickbase.IPurchaseOrdersRepo _quickbaseHeaderRepo;
		private EskerAP.Data.Quickbase.IItemsRepo _quickbaseDetailRepo;

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
			_folderPath = _configuration["Esker:Folders:MasterData"];

			var realm = _configuration["Quickbase:Realm"];
			var usertoken = _configuration["Quickbase:UserToken"];
			var logger = new MockLogger<QuickBaseConnection>();
			_quickbaseConnection = new QuickBaseConnection(realm, usertoken, logger);

			_famousHeaderRepo = new PurchaseOrderHeaderRepo(connectionString, schema, new MockLogger<PurchaseOrderHeaderRepo>());
			_famousDetailRepo = new PurchaseOrderDetailRepo(connectionString, schema, new MockLogger<PurchaseOrderDetailRepo>());
			_quickbaseHeaderRepo = new PurchaseOrdersRepo(_quickbaseConnection);
			_quickbaseDetailRepo = new ItemsRepo(_quickbaseConnection);
		}

		[TestMethod]
		public void Export()
		{
			var exporter = new PurchaseOrderExporter(
				new MockLogger<PurchaseOrderExporter>(),
				_famousHeaderRepo,
				_famousDetailRepo,
				_quickbaseHeaderRepo,
				_quickbaseDetailRepo,
				_folderPath);

			exporter.ExportPurchaseOrders("TestCompany01");

		}
	}
}
