using EskerAP.Data.Famous;
using EskerAP.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EskerAp.UnitTests
{
	[TestClass]
	public class PhaseExporterTests
	{

		private IConfigurationRoot _configuration;
		private CaPhaseRepo _repo;
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

			_repo = new CaPhaseRepo(connectionString, schema, new MockLogger<CaPhaseRepo>());
		}

		[TestMethod]
		public void Export()
		{
			var exporter = new PhaseExporter(new MockLogger<PhaseExporter>(), _repo);
			exporter.ExportPhases("PW01", _folderPath);

		}
	}
}
