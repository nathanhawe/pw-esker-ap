using EskerAp.UnitTests.Mocks;
using EskerAP.Domain;
using EskerAP.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;

namespace EskerAp.UnitTests
{
	[TestClass]
	public class VoucherImportTests
	{
		[TestMethod]
		public void Import_WithSftpIntegration_Success()
		{
			var config = new SftpConfig
			{
				Host = "NONUP2PCSPNH",
				Port = 2222,
				Username = "tester",
				Password = "Testing1!"
			};
			var sftpService = new SftpService(new MockLogger<SftpService>(), config);
			var mockImportRepo = new MockApVoucherImportRepo();
			var service = new VoucherImportService(
				new MockLogger<VoucherImportService>(),
				mockImportRepo,
				sftpService,
				new VoucherConverter(new MockLogger<VoucherConverter>()),
				new ErpAckService(new MockLogger<ErpAckService>()));

			mockImportRepo.Response = new ImportApVoucherResponse
			{
				SucceededCount = 1,
				RawXmlVoucherResponse = @"<Vouchers><ROWSET>
                  <Voucher>
                    <VoucherImportStatus>Import Successful</VoucherImportStatus>
                    <EntryNumber>427975</EntryNumber>
                    <VendorId>TEST</VendorId>
                    <InvoiceNumber>TEST01234567</InvoiceNumber>
                    <HoldFlag>N</HoldFlag>
                    <InvoiceDate>2022-10-20</InvoiceDate>
                    <PayTerms>PAYMENT</PayTerms>
                    <DiscountAmount>0</DiscountAmount>
                    <VoucherAmount>100</VoucherAmount>
                    <AllowDuplicateVendorInvoice>Y</AllowDuplicateVendorInvoice>
                    <LineCount>1</LineCount>
                    <NoteCount>0</NoteCount>
                    <Lines>
                      <ROWSET>
                        <Line LineSequence=""1"">
                          <CostCenterId>TEST</CostCenterId>
                          <PhaseId>1100</PhaseId>
                          <LineDescription>Testing Line #1.</LineDescription>
                          <Quantity>50</Quantity>
                          <Rate>2</Rate>
                          <Amount>100</Amount>
                        </Line>
                      </ROWSET>
                    </Lines>
                    <Notes>
                      <ROWSET>
                </ROWSET>
                    </Notes>
                  </Voucher>
                </ROWSET>
                </Vouchers>",
			};

			service.ImportVouchers("Out", "Out", "ErpAck");

			Assert.AreEqual(mockImportRepo.Voucher.Ruid, "CD#PHOK41000SGA.1021296811651564047");
			Assert.AreEqual(mockImportRepo.Voucher.VendorId, "EAN");
			Assert.AreEqual(mockImportRepo.Voucher.InvoiceNumber, "31364031");
			Assert.AreEqual(mockImportRepo.Voucher.InvoiceDate, new DateTime(2022, 11, 5));
			Assert.AreEqual(mockImportRepo.Voucher.PayTerms, "PAYMENT");
			Assert.AreEqual(mockImportRepo.Voucher.DueDate, new DateTime(2022, 11, 5));

			Assert.AreEqual(mockImportRepo.Voucher.LineCount, 3);
			Assert.AreEqual(1, mockImportRepo.Voucher.Lines.Where(x => x.CostCenterId == "6001" && x.GlAccountCode == "" && x.Quantity == 1 && x.Amount == 2582.78M && x.PhaseId == "").Count());
			Assert.AreEqual(1, mockImportRepo.Voucher.Lines.Where(x => x.CostCenterId == "1007" && x.GlAccountCode == "" && x.Quantity == 1 && x.Amount == 3759.36M && x.PhaseId == "").Count());
			Assert.AreEqual(1, mockImportRepo.Voucher.Lines.Where(x => x.CostCenterId == "1008" && x.GlAccountCode == "" && x.Quantity == 1 && x.Amount == 10022.6M && x.PhaseId == "").Count());

			Assert.AreEqual(mockImportRepo.Voucher.VoucherAmount, 16364.74M);

			Assert.AreEqual(mockImportRepo.Voucher.NoteCount, 0);
		}

		[TestMethod]
		public void Import_WithSftpIntegration_Fail()
		{
			var config = new SftpConfig
			{
				Host = "NONUP2PCSPNH",
				Port = 2222,
				Username = "tester",
				Password = "Testing1!"
			};
			var sftpService = new SftpService(new MockLogger<SftpService>(), config);
			var mockImportRepo = new MockApVoucherImportRepo();
			var service = new VoucherImportService(
				new MockLogger<VoucherImportService>(),
				mockImportRepo,
				sftpService,
				new VoucherConverter(new MockLogger<VoucherConverter>()),
				new ErpAckService(new MockLogger<ErpAckService>()));

			mockImportRepo.Response = new ImportApVoucherResponse
			{
				HeaderErrors = "vendor id is invalid, pay terms is required",
			};

			service.ImportVouchers("Out", "Out", "ErpAck");

			Assert.AreEqual(mockImportRepo.Voucher.Ruid, "CD#PHOK41000SGA.1021296811651564047");
			Assert.AreEqual(mockImportRepo.Voucher.VendorId, "EAN");
			Assert.AreEqual(mockImportRepo.Voucher.InvoiceNumber, "31364031");
			Assert.AreEqual(mockImportRepo.Voucher.InvoiceDate, new DateTime(2022, 11, 5));
			Assert.AreEqual(mockImportRepo.Voucher.PayTerms, "PAYMENT");
			Assert.AreEqual(mockImportRepo.Voucher.DueDate, new DateTime(2022, 11, 5));

			Assert.AreEqual(mockImportRepo.Voucher.LineCount, 3);
			Assert.AreEqual(1, mockImportRepo.Voucher.Lines.Where(x => x.CostCenterId == "6001" && x.GlAccountCode == "" && x.Quantity == 1 && x.Amount == 2582.78M && x.PhaseId == "").Count());
			Assert.AreEqual(1, mockImportRepo.Voucher.Lines.Where(x => x.CostCenterId == "1007" && x.GlAccountCode == "" && x.Quantity == 1 && x.Amount == 3759.36M && x.PhaseId == "").Count());
			Assert.AreEqual(1, mockImportRepo.Voucher.Lines.Where(x => x.CostCenterId == "1008" && x.GlAccountCode == "" && x.Quantity == 1 && x.Amount == 10022.6M && x.PhaseId == "").Count());

			Assert.AreEqual(mockImportRepo.Voucher.VoucherAmount, 16364.74M);

			Assert.AreEqual(mockImportRepo.Voucher.NoteCount, 0);
		}
	}
}
