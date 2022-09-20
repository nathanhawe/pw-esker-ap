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
	public class CaCostCenterRepoTests
	{
		private IConfigurationRoot _configuration;
		private CaCostCenterRepo _repo;

		[TestInitialize]
		public void Setup()
		{
			if(_configuration == null)	_configuration = ConfigurationHelper.GetIConfigurationRoot();

			var userId = _configuration["Famous:UserId"];
			var password = _configuration["Famous:Password"];
			var dataSource = _configuration["Famous:DataSource"];
			var schema = _configuration["Famous:Schema"];
			var connectionString = $"User id={userId};Password={password};Data Source={dataSource}";

			_repo = new CaCostCenterRepo(connectionString, schema);
		}
		
		[TestMethod]
		public void GetCostCenters()
		{
			var temp = _repo.GetCostCenters();
			Print(temp);
		}

		private void Print(IEnumerable<CostCenter> lines)
		{
			Console.WriteLine($"There are '{lines.Count()}' Cost Centers:");
			foreach (var line in lines)
			{
				Print(line);
			}
		}

		private void Print(CostCenter line)
		{
			Console.WriteLine("");
			var properties = typeof(CostCenter).GetProperties();
			foreach (var property in properties)
			{
				Console.Write($"     {property.Name}: '{property.GetValue(line)}'");
			}
		}
	}
}
