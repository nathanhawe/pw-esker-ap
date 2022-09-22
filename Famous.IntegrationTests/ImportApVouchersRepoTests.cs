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
	public class ImportApVouchersRepoTests
	{
		private IConfigurationRoot _configuration;
		private ImportApVouchersRepo _repo;

		[TestInitialize]
		public void Setup()
		{
			if(_configuration == null)	_configuration = ConfigurationHelper.GetIConfigurationRoot();

			var userId = _configuration["Oracle:UserId"];
			var password = _configuration["Oracle:Password"];
			var dataSource = _configuration["Oracle:DataSource"];
			var schema = _configuration["Oracle:Schema"];
			var connectionString = $"User id={userId};Password={password};Data Source={dataSource}";
			var famousUser = _configuration["Famous:UserId"];
			var famousPassword = _configuration["Famous:Password"];

			_repo = new ImportApVouchersRepo(connectionString, schema, famousUser, famousPassword);
		}
		
		[TestMethod]
		public void AddVoucher()
		{
			_repo.ImportVoucher(new Voucher());
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
