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
	public class ApVoucherRepoTests
	{
		private IConfigurationRoot _configuration;
		private ApVoucherRepo _repo;
		private readonly MockLogger<ApVoucherRepo> _logger = new MockLogger<ApVoucherRepo>();

		[TestInitialize]
		public void Setup()
		{
			_configuration ??= ConfigurationHelper.GetIConfigurationRoot();
			var userId = _configuration["Oracle:UserId"];
			var password = _configuration["Oracle:Password"];
			var dataSource = _configuration["Oracle:DataSource"];
			var schema = _configuration["Oracle:Schema"];
			var connectionString = $"User id={userId};Password={password};Data Source={dataSource}";

			_repo = new ApVoucherRepo(connectionString, schema, _logger);
		}
		
		[TestMethod]
		public void GetPaidInvoices()
		{
			var temp = _repo.GetPaidInvoices(90);
			Print(temp);
		}

		private void Print(IEnumerable<PaidInvoice> lines)
		{
			Console.WriteLine($"There are '{lines.Count()}' Paid Invoices:");
			foreach (var line in lines)
			{
				Print(line);
			}
		}

		private void Print(PaidInvoice line)
		{
			Console.WriteLine("");
			var properties = typeof(PaidInvoice).GetProperties();
			foreach (var property in properties)
			{
				Console.Write($"     {property.Name}: '{property.GetValue(line)}'");
			}
		}
	}
}
