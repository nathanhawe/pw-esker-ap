using EskerAP.Data.Famous;
using EskerAP.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Famous.IntegrationTests
{
	[TestClass]
	public class ImportApVouchersRepoTests
	{
		private IConfigurationRoot _configuration;
		private ImportApVouchersRepo _repo;
		private readonly MockLogger<ImportApVouchersRepo> _logger = new MockLogger<ImportApVouchersRepo>();

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

			_repo = new ImportApVouchersRepo(connectionString, schema, famousUser, famousPassword, _logger);
		}
		
		[TestMethod]
		public void AddVoucher()
		{
			var testVoucher = new Voucher
			{
				VendorId = "TEST",
				InvoiceNumber = "TEST012345678910", // Will be truncated to TEST01234567
				InvoiceDate = new DateTime(2022,10,20),
				PayTerms = "PAYMENT",
				AllowDuplicateVendorInvoice = 'Y',
			};
			testVoucher.Lines.Add(new VoucherItem
			{
				CostCenterId = "TEST",
				PhaseId = "1100",
				LineDescription = "Testing Line #1.",
				Quantity = 50,
				Rate = 2,
			});

			var response = _repo.ImportVoucher(testVoucher);
			Print(response);
		}

		[TestMethod]
		public void AddVoucher_WithSpecialCharacters()
		{
			var testVoucher = new Voucher
			{
				VendorId = "TEST",
				InvoiceNumber = "TEST012345678910", // Will be truncated to TEST01234567
				InvoiceDate = new DateTime(2022, 10, 20),
				PayTerms = "PAYMENT",
				AllowDuplicateVendorInvoice = 'Y',
			};
			testVoucher.Lines.Add(new VoucherItem
			{
				CostCenterId = "4002",
				PhaseId = "61152",
				LineDescription = "CRAFTSMAN Screwdriver Set, Slotted &</>'\" Phillips, 14 -",
				Quantity = 1,
				Rate = 25.98M,
			});
			testVoucher.Lines.Add(new VoucherItem
			{
				CostCenterId = "4002",
				PhaseId = "61152",
				LineDescription = "Eastvolt & Mechanic <Tool> 'Kits', \"Drive\" Socket \\Set, 46",
				Quantity = 1,
				Rate = 9.53M,
			});
			var response = _repo.ImportVoucher(testVoucher);
			Print(response);
		}
		[TestMethod]
		public void HeaderErrorsReturnFailedImportResponse()
		{
			var testVoucher = new Voucher
			{
				VendorId = "TEST",
				InvoiceNumber = "TEST01234560",
				InvoiceDate = new DateTime(2021, 9, 20),
				PayTerms = "",
			};
			testVoucher.Lines.Add(new VoucherItem
			{
				CostCenterId = "TEST",
				PhaseId = "1100",
				LineDescription = "Testing Line #1.",
				Quantity = 50,
				Rate = 2,
			});
			var response = _repo.ImportVoucher(testVoucher);
			Print(response);
			Assert.IsNotNull(response);
			Assert.IsFalse(response.ImportWasSuccessful);
			Assert.IsFalse(String.IsNullOrWhiteSpace(response.HeaderErrors));
			Assert.AreEqual(response.HeaderErrors, "pay terms is required");
		}
		[TestMethod]
		public void LineErrorsReturnFailedImportResponse()
		{
			var testVoucher = new Voucher
			{
				VendorId = "TEST",
				InvoiceNumber = "TEST01234560",
				InvoiceDate = new DateTime(2021, 9, 20),
				PayTerms = "PAYMENT",
			};
			testVoucher.Lines.Add(new VoucherItem
			{
				CostCenterId = "",
				PhaseId = "1100",
				LineDescription = "Testing Line #1.",
				Quantity = 50,
				Rate = 2,
			});
			var response = _repo.ImportVoucher(testVoucher);
			Print(response);
			Assert.IsNotNull(response);
			Assert.IsFalse(response.ImportWasSuccessful);
			Assert.IsTrue(response.LineErrors.Count() > 0);
		}
		[TestMethod]
		public void ExceptionsReturnFailedImportResponse()
		{
			var testVoucher = new Voucher
			{
				VendorId = "TEST",
				InvoiceNumber = "TEST012345678910",
				InvoiceDate = new DateTime(2021, 9, 20),
				PayTerms = "PAYMENT",
			};
			testVoucher.Lines.Add(new VoucherItem
			{
				CostCenterId = "TEST",
				PhaseId = "1100",
				LineDescription = "Testing Line #1.",
				Quantity = 50,
				Rate = 2,
			});
			var testRepo = new ImportApVouchersRepo("", "", "", "", _logger);
			var response = testRepo.ImportVoucher(testVoucher);
			Print(response);
			Assert.IsNotNull(response);
			Assert.IsFalse(response.ImportWasSuccessful);
			Assert.IsNotNull(response.Exception);
			Assert.AreEqual(response.Exception.Message, "OracleConnection.ConnectionString is invalid");
		}

		private void Print(ImportApVoucherResponse response)
		{
			Console.WriteLine($"ImportWasSuccessful: '{response.ImportWasSuccessful}'");
			Console.WriteLine($"EntryNumber: '{response.EntryNumber}'");
			Console.WriteLine($"SucceededCount: '{response.SucceededCount}'");
			Console.WriteLine($"FailedCount: '{response.Failedcount}'");
			Console.WriteLine($"SkippedCount: '{response.SkippedCount}'");
			Console.WriteLine($"OtherErrors: '{response.OtherErrors}'");
			Console.WriteLine($"HeaderErrors: '{response.HeaderErrors}'");
			Console.WriteLine($"HasException: '{!(response.Exception == null)}'");
			Console.WriteLine($"ExceptionMessage: '{response.Exception?.Message}'");
			Console.WriteLine($"LineErrors:");
			foreach (var line in response.LineErrors)
			{
				Console.WriteLine(line);
			}
			Console.WriteLine($"RawVoucherXml: '{response.RawXmlVoucherResponse}'");
		}
	}
}
