using EskerAP.Data.Famous;
using EskerAP.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EskerAp.UnitTests
{
	[TestClass]
	public class GLAccountExporterTests
	{

		private IConfigurationRoot _configuration;
		private GLAccountRepo _repo;
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

			_repo = new GLAccountRepo(connectionString, schema, new MockLogger<GLAccountRepo>());
		}

		[TestMethod]
		public void Export()
		{
			var exporter = new GLAccountExporter(new MockLogger<GLAccountExporter>(), _repo, _folderPath);
			exporter.ExportGLAccounts("PW01");

		}
	}
}
