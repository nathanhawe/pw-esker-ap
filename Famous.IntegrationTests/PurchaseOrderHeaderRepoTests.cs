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
	public class PurchaseOrderHeaderRepoTests
	{
		private IConfigurationRoot _configuration;
		private PurchaseOrderHeaderRepo _repo;
		private readonly MockLogger<PurchaseOrderHeaderRepo> _logger = new MockLogger<PurchaseOrderHeaderRepo>();

		[TestInitialize]
		public void Setup()
		{
			_configuration ??= ConfigurationHelper.GetIConfigurationRoot();
			var userId = _configuration["Oracle:UserId"];
			var password = _configuration["Oracle:Password"];
			var dataSource = _configuration["Oracle:DataSource"];
			var schema = _configuration["Oracle:Schema"];
			var connectionString = $"User id={userId};Password={password};Data Source={dataSource}";

			_repo = new PurchaseOrderHeaderRepo(connectionString, schema, _logger);
		}
		
		[TestMethod]
		public void GetPurchaseOrderHeaders()
		{
			var temp = _repo.GetPurchaseOrderHeaders();
			Print(temp);
		}

		private void Print(IEnumerable<Header> lines)
		{
			Console.WriteLine($"There are '{lines.Count()}' Purchase Order Headers:");
			foreach (var line in lines)
			{
				Print(line);
			}
		}

		private void Print(Header line)
		{
			Console.WriteLine("");
			var properties = typeof(Header).GetProperties();
			foreach (var property in properties)
			{
				Console.Write($"     {property.Name}: '{property.GetValue(line)}'");
			}
		}
	}
}
