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
	public class ApPayTermsRepoTests
	{
		private IConfigurationRoot _configuration;
		private ApPayTermsRepo _repo;
		private readonly MockLogger<ApPayTermsRepo> _logger = new MockLogger<ApPayTermsRepo>();

		[TestInitialize]
		public void Setup()
		{
			if(_configuration == null)	_configuration = ConfigurationHelper.GetIConfigurationRoot();

			var userId = _configuration["Oracle:UserId"];
			var password = _configuration["Oracle:Password"];
			var dataSource = _configuration["Oracle:DataSource"];
			var schema = _configuration["Oracle:Schema"];
			var connectionString = $"User id={userId};Password={password};Data Source={dataSource}";

			_repo = new ApPayTermsRepo(connectionString, schema, _logger);
		}
		
		[TestMethod]
		public void GetPayTerms()
		{
			var temp = _repo.GetPayTerms();
			Print(temp);
		}

		private void Print(IEnumerable<PaymentTerm> lines)
		{
			Console.WriteLine($"There are '{lines.Count()}' Pay Terms:");
			foreach (var line in lines)
			{
				Print(line);
			}
		}

		private void Print(PaymentTerm line)
		{
			Console.WriteLine("");
			var properties = typeof(PaymentTerm).GetProperties();
			foreach (var property in properties)
			{
				Console.Write($"     {property.Name}: '{property.GetValue(line)}'");
			}
		}
	}
}
