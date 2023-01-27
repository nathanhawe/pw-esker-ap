using EskerAP.Data.Famous;
using EskerAP.Data.Quickbase;
using EskerAP.Domain;
using EskerAP.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuickBase.Api;
using System.Collections.Generic;
using System;
using System.Linq;

namespace EskerAp.UnitTests
{
	[TestClass]
	public class UnpaidInvoiceReaderTests
	{

		private IConfigurationRoot _configuration;
		private UnpaidInvoiceReader _service;
		private string _folderPath;

		[TestInitialize]
		public void Setup()
		{
			_configuration ??= ConfigurationHelper.GetIConfigurationRoot();
			var userId = _configuration["Oracle:UserId"];
			var password = _configuration["Oracle:Password"];
			var dataSource = _configuration["Oracle:DataSource"];
			var schema = _configuration["Oracle:Schema"];
			var connectionString = $"User id={userId};Password={password};Data Source={dataSource}";
			var realm = _configuration["Quickbase:Realm"];
			var userToken = _configuration["Quickbase:UserToken"];

			_folderPath = _configuration["Esker:Folders:UnpaidInvoices"];
			var qbConnection = new QuickBaseConnection(realm, userToken, new MockLogger<QuickBaseConnection>());
			var sftpConfig = new SftpConfig
			{
				Host = _configuration["Esker:SFTP:Host"],
				Port = (int.TryParse(_configuration["Esker:SFTP:Port"], out int port) ? port : 0),
				Username = _configuration["Esker:SFTP:Username"],
				Password = _configuration["Esker:SFTP:Password"]
			};

			_service = new UnpaidInvoiceReader(
				new MockLogger<UnpaidInvoiceReader>(),
				new SftpService(new MockLogger<SftpService>(), sftpConfig));
		}

		[TestMethod]
		public void ReadUnpaidInvoices_Integration()
		{
			var temp = _service.GetUnpaidInvoices(_folderPath, _folderPath);
			Print(temp);
		}

		private void Print(IEnumerable<UnpaidInvoice> lines)
		{
			Console.WriteLine($"There are '{lines.Count()}' Unpaid Invoices:");
			foreach (var line in lines)
			{
				Print(line);
			}
		}

		private void Print(UnpaidInvoice line)
		{
			if (line != null)
			{


				Console.WriteLine("");
				var properties = typeof(UnpaidInvoice).GetProperties();
				foreach (var property in properties)
				{
					Console.Write($"     {property.Name}: '{property.GetValue(line)}'");
				}
			}
			else
			{
				Console.WriteLine("No record returned.");
			}
		}

	}
}
