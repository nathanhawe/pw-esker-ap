using EskerAP.Domain;
using EskerAP.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace EskerAp.UnitTests
{
	[TestClass]
	public class ErpAckServiceTests
	{
        private ErpAckService _erpAckService = new ErpAckService(new MockLogger<ErpAckService>());

		[TestMethod]
		public void Success()
		{
            var importResult = new ImportApVoucherResponse
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

            var result = _erpAckService.GetErpAckXmlString(importResult, "CD#RPOR12941.13031");
            Assert.AreEqual(result, @"<?xml version=""1.0"" encoding=""utf-8""?><ERPAck><EskerInvoiceID>CD#RPOR12941.13031</EskerInvoiceID><ERPID>427975</ERPID></ERPAck>");
		}

		[TestMethod]
		public void Fail_Exception()
		{
			var importResult = new ImportApVoucherResponse
			{
				Exception = new Exception("OracleConnection.ConnectionString is invalid"),
			};

			var result = _erpAckService.GetErpAckXmlString(importResult, "CD#RPOR12941.13031");
			Assert.AreEqual(result, @"<?xml version=""1.0"" encoding=""utf-8""?><ERPAck><EskerInvoiceID>CD#RPOR12941.13031</EskerInvoiceID><ERPPostingError>OracleConnection.ConnectionString is invalid</ERPPostingError></ERPAck>");
		}

		[TestMethod]
		public void Fail_Header()
		{
			var importResult = new ImportApVoucherResponse
			{
				HeaderErrors = "vendor id is invalid, pay terms is required",
			};

			var result = _erpAckService.GetErpAckXmlString(importResult, "CD#RPOR12941.13031");
			Assert.AreEqual(result, @"<?xml version=""1.0"" encoding=""utf-8""?><ERPAck><EskerInvoiceID>CD#RPOR12941.13031</EskerInvoiceID><ERPPostingError>Header: vendor id is invalid, pay terms is required</ERPPostingError></ERPAck>");
		}

		[TestMethod]
		public void Fail_Other()
		{
			var importResult = new ImportApVoucherResponse
			{
				OtherErrors = "incorrect password",
			};

			var result = _erpAckService.GetErpAckXmlString(importResult, "CD#RPOR12941.13031");
			Assert.AreEqual(result, @"<?xml version=""1.0"" encoding=""utf-8""?><ERPAck><EskerInvoiceID>CD#RPOR12941.13031</EskerInvoiceID><ERPPostingError>Other: incorrect password</ERPPostingError></ERPAck>");
		}

		[TestMethod]
		public void Fail_Lines()
		{
			var importResult = new ImportApVoucherResponse();

			importResult.LineErrors.Add("must specifiy both department and account");
			importResult.LineErrors.Add("invalid phase id");

			var result = _erpAckService.GetErpAckXmlString(importResult, "CD#RPOR12941.13031");
			Assert.AreEqual(result, @"<?xml version=""1.0"" encoding=""utf-8""?><ERPAck><EskerInvoiceID>CD#RPOR12941.13031</EskerInvoiceID><ERPPostingError>Lines: must specifiy both department and account;invalid phase id;</ERPPostingError></ERPAck>");
		}
	}
}
