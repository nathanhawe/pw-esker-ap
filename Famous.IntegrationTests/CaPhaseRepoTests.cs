using EskerAP.Data.Famous;
using EskerAP.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace Famous.IntegrationTests
{
	[TestClass]
	public class CaPhaseRepoTests
	{
		private IConfigurationRoot _configuration;
		private CaPhaseRepo _repo;
		private readonly MockLogger<CaPhaseRepo> _logger = new MockLogger<CaPhaseRepo>();

		[TestInitialize]
		public void Setup()
		{
			_configuration ??= ConfigurationHelper.GetIConfigurationRoot();
			var userId = _configuration["Oracle:UserId"];
			var password = _configuration["Oracle:Password"];
			var dataSource = _configuration["Oracle:DataSource"];
			var schema = _configuration["Oracle:Schema"];
			var connectionString = $"User id={userId};Password={password};Data Source={dataSource}";

			_repo = new CaPhaseRepo(connectionString, schema, _logger);
		}
		
		[TestMethod]
		public void GetCaPhases()
		{
			var temp = _repo.GetPhases();
			Print(temp);
		}

		private void Print(IEnumerable<Phase> lines)
		{
			Console.WriteLine($"There are '{lines.Count()}' Phases:");
			foreach (var line in lines)
			{
				Print(line);
			}
		}

		private void Print(Phase line)
		{
			Console.WriteLine("");
			var properties = typeof(Phase).GetProperties();
			foreach (var property in properties)
			{
				Console.Write($"     {property.Name}: '{property.GetValue(line)}'");
			}
		}
	}
}
