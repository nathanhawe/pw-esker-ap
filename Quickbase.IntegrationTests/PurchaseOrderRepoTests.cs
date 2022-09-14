using EskerAP.Data.Quickbase;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuickBase.Api;
using System.Collections.Generic;
using System;
using EskerAP.Domain;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace Quickbase.IntegrationTests
{
	[TestClass]
	public class PurchaseOrderRepoTests
	{
		private IConfigurationRoot _configuration;
		private QuickBaseConnection _quickBaseConnection;
		private PurchaseOrdersRepo _repo;

		[TestInitialize]
		public void Setup()
		{
			if (_configuration == null)
			{
				_configuration = ConfigurationHelper.GetIConfigurationRoot();
			}
			var realm = _configuration["Quickbase:Realm"];
			var usertoken = _configuration["Quickbase:UserToken"];
			var logger = new MockLogger<QuickBaseConnection>();

			_quickBaseConnection = new QuickBaseConnection(realm, usertoken, logger);
			_repo = new PurchaseOrdersRepo(_quickBaseConnection);
		}

		[TestMethod]
		public void DoQuery()
		{
			var temp = _repo.Get();
			Print(temp);
		}

		private void Print(IEnumerable<PurchaseOrder> lines)
		{
			Console.WriteLine($"There are '{lines.Count()}' Purchase Orders:");
			foreach (var line in lines)
			{
				Print(line);
			}
		}

		private void Print(PurchaseOrder line)
		{
			Console.WriteLine($"Record #{line.RecordId}");
			var properties = typeof(PurchaseOrder).GetProperties();
			foreach (var property in properties)
			{
				Console.Write($"     {property.Name}: '{property.GetValue(line)}'");
			}
		}
	}
}
