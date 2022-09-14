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
	public class ItemsRepoTests
	{
		private IConfigurationRoot _configuration;
		private QuickBaseConnection _quickBaseConnection;
		private ItemsRepo _repo;

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
			_repo = new ItemsRepo(_quickBaseConnection);
		}

		[TestMethod]
		public void DoQuery()
		{
			var temp = _repo.Get();
			Print(temp);
		}

		private void Print(IEnumerable<Item> lines)
		{
			Console.WriteLine($"There are '{lines.Count()}' Items:");
			foreach (var line in lines)
			{
				Print(line);
			}
		}

		private void Print(Item line)
		{
			Console.WriteLine($"Record #{line.RecordId}");
			var properties = typeof(Item).GetProperties();
			foreach (var property in properties)
			{
				Console.Write($"     {property.Name}: '{property.GetValue(line)}'");
			}
		}
	}
}
