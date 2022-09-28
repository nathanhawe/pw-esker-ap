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
	public class PurchaseOrderDetailRepoTests
	{
		private IConfigurationRoot _configuration;
		private PurchaseOrderDetailRepo _repo;
		private readonly MockLogger<PurchaseOrderDetailRepo> _logger = new MockLogger<PurchaseOrderDetailRepo>();

		[TestInitialize]
		public void Setup()
		{
			_configuration ??= ConfigurationHelper.GetIConfigurationRoot();
			var userId = _configuration["Oracle:UserId"];
			var password = _configuration["Oracle:Password"];
			var dataSource = _configuration["Oracle:DataSource"];
			var schema = _configuration["Oracle:Schema"];
			var connectionString = $"User id={userId};Password={password};Data Source={dataSource}";

			_repo = new PurchaseOrderDetailRepo(connectionString, schema, _logger);
		}
		
		[TestMethod]
		public void GetPurchaseOrderDetails()
		{
			var temp = _repo.GetPurchaseOrderDetails();
			Print(temp);
		}

		private void Print(IEnumerable<Item> lines)
		{
			Console.WriteLine($"There are '{lines.Count()}' Purchase Order Items:");
			foreach (var line in lines)
			{
				Print(line);
			}
		}

		private void Print(Item line)
		{
			Console.WriteLine("");
			var properties = typeof(Item).GetProperties();
			foreach (var property in properties)
			{
				Console.Write($"     {property.Name}: '{property.GetValue(line)}'");
			}
		}
	}
}
