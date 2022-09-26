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
	public class GLAccountRepoTests
	{
		private IConfigurationRoot _configuration;
		private GLAccountRepo _repo;
		private readonly MockLogger<GLAccountRepo> _logger = new MockLogger<GLAccountRepo>();

		[TestInitialize]
		public void Setup()
		{
			_configuration ??= ConfigurationHelper.GetIConfigurationRoot();
			var userId = _configuration["Oracle:UserId"];
			var password = _configuration["Oracle:Password"];
			var dataSource = _configuration["Oracle:DataSource"];
			var schema = _configuration["Oracle:Schema"];
			var connectionString = $"User id={userId};Password={password};Data Source={dataSource}";

			_repo = new GLAccountRepo(connectionString, schema, _logger);
		}
		
		[TestMethod]
		public void GetGLAccounts()
		{
			var temp = _repo.GetGLAccounts();
			Print(temp);
		}

		private void Print(IEnumerable<GLAccount> lines)
		{
			Console.WriteLine($"There are '{lines.Count()}' GL Accounts:");
			foreach (var line in lines)
			{
				Print(line);
			}
		}

		private void Print(GLAccount line)
		{
			Console.WriteLine("");
			var properties = typeof(GLAccount).GetProperties();
			foreach (var property in properties)
			{
				Console.Write($"     {property.Name}: '{property.GetValue(line)}'");
			}
		}
	}
}
