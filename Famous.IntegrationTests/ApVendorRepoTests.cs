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
	public class ApVendorRepoTests
	{
		private IConfigurationRoot _configuration;
		private ApVendorRepo _repo;
		private readonly MockLogger<ApVendorRepo> _logger = new MockLogger<ApVendorRepo>();

		[TestInitialize]
		public void Setup()
		{
			_configuration ??= ConfigurationHelper.GetIConfigurationRoot();
			var userId = _configuration["Oracle:UserId"];
			var password = _configuration["Oracle:Password"];
			var dataSource = _configuration["Oracle:DataSource"];
			var schema = _configuration["Oracle:Schema"];
			var connectionString = $"User id={userId};Password={password};Data Source={dataSource}";

			_repo = new ApVendorRepo(connectionString, schema, _logger);
		}
		
		[TestMethod]
		public void GetApVendors()
		{
			var temp = _repo.GetVendors();
			Print(temp);
		}

		private void Print(IEnumerable<Vendor> lines)
		{
			Console.WriteLine($"There are '{lines.Count()}' Vendors:");
			foreach (var line in lines)
			{
				Print(line);
			}
		}

		private void Print(Vendor line)
		{
			Console.WriteLine("");
			var properties = typeof(Vendor).GetProperties();
			foreach (var property in properties)
			{
				Console.Write($"     {property.Name}: '{property.GetValue(line)}'");
			}
		}
	}
}
