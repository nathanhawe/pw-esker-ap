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
	public class VoucherConverterTests
	{
		[TestMethod]
		public void ConvertString_PostingDate_OverridesInvoiceDate()
		{
			var voucherConverter = new VoucherConverter(new MockLogger<VoucherConverter>());
			var voucher = voucherConverter.ConvertXmlString(TestString());
			
			Assert.IsNotNull(voucher);
			Print(voucher);

			Assert.AreEqual(voucher.Ruid, "CD#PHOK41000SGA.1021296811651564047");
			Assert.AreEqual(voucher.VendorId, "EAN");
			Assert.AreEqual(voucher.InvoiceNumber, "31364031");
			Assert.AreEqual(voucher.InvoiceDate, new DateTime(2023, 01, 05));
			Assert.AreEqual(voucher.PayTerms, "PAYMENT");
			Assert.AreEqual(voucher.DueDate, new DateTime(2022, 11, 5));
			Assert.AreEqual(voucher.PoSourceNumber, "Test1234");
			Assert.AreEqual(voucher.StubDescription, "Test Description #1");
			
			Assert.AreEqual(voucher.LineCount, 3);
			Assert.AreEqual(1, voucher.Lines.Where(x => x.CostCenterId == "6001" && x.GlAccountCode == "" && x.DepartmentId == "" && x.Quantity == 1 && x.Amount == 2582.78M && x.PhaseId == "1").Count());
			Assert.AreEqual(1, voucher.Lines.Where(x => x.CostCenterId == "1007" && x.GlAccountCode == "" && x.DepartmentId == "" && x.Quantity == 1 && x.Amount == 3759.36M && x.PhaseId == "2").Count()); 
			Assert.AreEqual(1, voucher.Lines.Where(x => x.CostCenterId == "" && x.GlAccountCode == "1008" && x.DepartmentId == "01" && x.Quantity == 1 && x.Amount == 10022.6M && x.PhaseId == "").Count());

			Assert.AreEqual(voucher.VoucherAmount, 16364.74M);

			Assert.AreEqual(voucher.NoteCount, 0);
			
		}

		[TestMethod]
		public void ConvertString_NoPostingDate_UseInvoiceDate()
		{
			var voucherConverter = new VoucherConverter(new MockLogger<VoucherConverter>());
			var voucher = voucherConverter.ConvertXmlString(TestString_BlankPostingDate());

			Assert.IsNotNull(voucher);
			Print(voucher);

			Assert.AreEqual(voucher.Ruid, "CD#PHOK41000SGA.1021296811651564047");
			Assert.AreEqual(voucher.VendorId, "EAN");
			Assert.AreEqual(voucher.InvoiceNumber, "31364031");
			Assert.AreEqual(voucher.InvoiceDate, new DateTime(2022, 11, 5));
			Assert.AreEqual(voucher.PayTerms, "PAYMENT");
			Assert.AreEqual(voucher.DueDate, new DateTime(2022, 11, 5));
			Assert.AreEqual(voucher.PoSourceNumber, "Test1234");
			Assert.AreEqual(voucher.StubDescription, "Test Description Blank Posting Date #1 and some extra for cut off");

			Assert.AreEqual(voucher.LineCount, 3);
			Assert.AreEqual(1, voucher.Lines.Where(x => x.CostCenterId == "6001" && x.GlAccountCode == "" && x.DepartmentId == "" && x.Quantity == 1 && x.Amount == 2582.78M && x.PhaseId == "1").Count());
			Assert.AreEqual(1, voucher.Lines.Where(x => x.CostCenterId == "1007" && x.GlAccountCode == "" && x.DepartmentId == "" && x.Quantity == 1 && x.Amount == 3759.36M && x.PhaseId == "2").Count());
			Assert.AreEqual(1, voucher.Lines.Where(x => x.CostCenterId == "" && x.GlAccountCode == "1008" && x.DepartmentId == "01" && x.Quantity == 1 && x.Amount == 10022.6M && x.PhaseId == "").Count());

			Assert.AreEqual(voucher.VoucherAmount, 16364.74M);

			Assert.AreEqual(voucher.NoteCount, 0);

		}

		[TestMethod]
		public void ConvertString_NoPostingDateSelfTerminated_UseInvoiceDate()
		{
			var voucherConverter = new VoucherConverter(new MockLogger<VoucherConverter>());
			var voucher = voucherConverter.ConvertXmlString(TestString_BlankPostingDate2());

			Assert.IsNotNull(voucher);
			Print(voucher);

			Assert.AreEqual(voucher.Ruid, "CD#PHOK41000SGA.1021296811651564047");
			Assert.AreEqual(voucher.VendorId, "EAN");
			Assert.AreEqual(voucher.InvoiceNumber, "31364031");
			Assert.AreEqual(voucher.InvoiceDate, new DateTime(2022, 11, 5));
			Assert.AreEqual(voucher.PayTerms, "PAYMENT");
			Assert.AreEqual(voucher.DueDate, new DateTime(2022, 11, 5));
			Assert.AreEqual(voucher.PoSourceNumber, "Test1234");
			Assert.AreEqual(voucher.StubDescription, "Test Description Blank Posting Date #2 and some extra for cut off");

			Assert.AreEqual(voucher.LineCount, 3);
			Assert.AreEqual(1, voucher.Lines.Where(x => x.CostCenterId == "6001" && x.GlAccountCode == "" && x.DepartmentId == "" && x.Quantity == 1 && x.Amount == 2582.78M && x.PhaseId == "1").Count());
			Assert.AreEqual(1, voucher.Lines.Where(x => x.CostCenterId == "1007" && x.GlAccountCode == "" && x.DepartmentId == "" && x.Quantity == 1 && x.Amount == 3759.36M && x.PhaseId == "2").Count());
			Assert.AreEqual(1, voucher.Lines.Where(x => x.CostCenterId == "" && x.GlAccountCode == "1008" && x.DepartmentId == "01" && x.Quantity == 1 && x.Amount == 10022.6M && x.PhaseId == "").Count());

			Assert.AreEqual(voucher.VoucherAmount, 16364.74M);

			Assert.AreEqual(voucher.NoteCount, 0);

		}

		[TestMethod]
		public void ConvertString_NullDueDate()
		{
			var voucherConverter = new VoucherConverter(new MockLogger<VoucherConverter>());
			var voucher = voucherConverter.ConvertXmlString(TestString_NullDueDate());

			Assert.IsNotNull(voucher);
			Print(voucher);

			Assert.AreEqual(voucher.Ruid, "CD#PHOK41000SGA.1021296811651564047");
			Assert.AreEqual(voucher.VendorId, "EAN");
			Assert.AreEqual(voucher.InvoiceNumber, "31364031");
			Assert.AreEqual(voucher.InvoiceDate, new DateTime(2023, 01, 05));
			Assert.AreEqual(voucher.PayTerms, "PAYMENT");
			Assert.AreEqual(voucher.DueDate, null);
			Assert.AreEqual(voucher.PoSourceNumber, "Test1234");
			Assert.AreEqual(voucher.StubDescription, "Test Description #1");

			Assert.AreEqual(voucher.LineCount, 3);
			Assert.AreEqual(1, voucher.Lines.Where(x => x.CostCenterId == "6001" && x.GlAccountCode == "" && x.DepartmentId == "" && x.Quantity == 1 && x.Amount == 2582.78M && x.PhaseId == "1").Count());
			Assert.AreEqual(1, voucher.Lines.Where(x => x.CostCenterId == "1007" && x.GlAccountCode == "" && x.DepartmentId == "" && x.Quantity == 1 && x.Amount == 3759.36M && x.PhaseId == "2").Count());
			Assert.AreEqual(1, voucher.Lines.Where(x => x.CostCenterId == "" && x.GlAccountCode == "1008" && x.DepartmentId == "01" && x.Quantity == 1 && x.Amount == 10022.6M && x.PhaseId == "").Count());

			Assert.AreEqual(voucher.VoucherAmount, 16364.74M);

			Assert.AreEqual(voucher.NoteCount, 0);

		}

		[TestMethod]
		public void ConvertString_MissingDueDateElement()
		{
			var voucherConverter = new VoucherConverter(new MockLogger<VoucherConverter>());
			var voucher = voucherConverter.ConvertXmlString(TestString_NullDueDate2());

			Assert.IsNotNull(voucher);
			Print(voucher);

			Assert.AreEqual(voucher.Ruid, "CD#PHOK41000SGA.1021296811651564047");
			Assert.AreEqual(voucher.VendorId, "EAN");
			Assert.AreEqual(voucher.InvoiceNumber, "31364031");
			Assert.AreEqual(voucher.InvoiceDate, new DateTime(2023, 01, 05));
			Assert.AreEqual(voucher.PayTerms, "PAYMENT");
			Assert.AreEqual(voucher.DueDate, null);
			Assert.AreEqual(voucher.PoSourceNumber, "Test1234");
			Assert.AreEqual(voucher.StubDescription, "Test Description #1");

			Assert.AreEqual(voucher.LineCount, 3);
			Assert.AreEqual(1, voucher.Lines.Where(x => x.CostCenterId == "6001" && x.GlAccountCode == "" && x.DepartmentId == "" && x.Quantity == 1 && x.Amount == 2582.78M && x.PhaseId == "1").Count());
			Assert.AreEqual(1, voucher.Lines.Where(x => x.CostCenterId == "1007" && x.GlAccountCode == "" && x.DepartmentId == "" && x.Quantity == 1 && x.Amount == 3759.36M && x.PhaseId == "2").Count());
			Assert.AreEqual(1, voucher.Lines.Where(x => x.CostCenterId == "" && x.GlAccountCode == "1008" && x.DepartmentId == "01" && x.Quantity == 1 && x.Amount == 10022.6M && x.PhaseId == "").Count());

			Assert.AreEqual(voucher.VoucherAmount, 16364.74M);

			Assert.AreEqual(voucher.NoteCount, 0);

		}

		private void Print(Voucher voucher)
		{
			var properties = typeof(Voucher).GetProperties();
			foreach (var property in properties)
			{
				if (property.Name == "Lines") continue;
				if (property.Name == "Notes") continue;

				Console.WriteLine($"{property.Name}: '{property.GetValue(voucher)}'");
			}

			Print(voucher.Lines);
			Print(voucher.Notes);
		}

		private void Print(IEnumerable<VoucherItem> lines)
		{
			Console.WriteLine($"There are '{lines.Count()}' Items:");
			foreach (var line in lines)
			{
				Print(line);
			}
		}

		private void Print(IEnumerable<VoucherNote> notes)
		{
			Console.WriteLine($"There are '{notes.Count()}' Notes:");
			foreach (var note in notes)
			{
				Print(note);
			}
		}

		private void Print(VoucherItem line)
		{
			var properties = typeof(VoucherItem).GetProperties();
			foreach (var property in properties)
			{
				Console.Write($"     {property.Name}: '{property.GetValue(line)}'");
			}
			Console.Write("\n");
		}

		private void Print(VoucherNote note)
		{
			var properties = typeof(VoucherNote).GetProperties();
			foreach (var property in properties)
			{
				Console.Write($"     {property.Name}: '{property.GetValue(note)}'");
			}
			Console.Write("\n");
		}

		private string TestString()
		{
			return @"<?xml version=""1.0"" encoding=""UTF-8""?><Invoice RUID=""CD#PHOK41000SGA.1021296811651564047""><AlternativePayee></AlternativePayee><Assignment></Assignment><BaselineDate/><BusinessArea></BusinessArea><CalculateTax>1</CalculateTax><CompanyCode>PW01</CompanyCode><ContractNumber></ContractNumber><DiscountLimitDate>2022-11-05</DiscountLimitDate><DueDate>2022-11-05</DueDate><ERPLinkingDate>2023-01-05</ERPLinkingDate><ERPPaymentBlocked>0</ERPPaymentBlocked><ERPPostingDate>2023-01-05</ERPPostingDate><ERP>generic</ERP><EstimatedDiscountAmount>0</EstimatedDiscountAmount><ExchangeRate>1</ExchangeRate><GRIV>0</GRIV><HeaderText></HeaderText><History>09/12/2022 20:59:15 - ERP posting error: A timeout occurred during ERP integration. Please make sure that the invoice was not created in your ERP system before you retry posting it.
09/12/2022 20:59:15 - Posted by AP Specialists
</History><InvoiceAmount>16364.74</InvoiceAmount><InvoiceCurrency>USD</InvoiceCurrency><InvoiceDate>2022-11-05</InvoiceDate><InvoiceDescription>Test Description #1</InvoiceDescription><InvoiceNumber>31364031</InvoiceNumber><InvoiceReferenceNumber></InvoiceReferenceNumber><InvoiceType>Non-PO Invoice</InvoiceType><LocalCurrency>USD</LocalCurrency><LocalEstimatedDiscountAmount>0</LocalEstimatedDiscountAmount><LocalInvoiceAmount>16364.74</LocalInvoiceAmount><LocalNetAmount>16364.74</LocalNetAmount><LocalTaxAmount>0</LocalTaxAmount><ManualLink>0</ManualLink><NetAmount>16364.74</NetAmount><OrderNumber>Test1234</OrderNumber><PaymentApprovalStatus>Not requested</PaymentApprovalStatus><PaymentTerms>PAYMENT</PaymentTerms><PostingDate>2023-01-05</PostingDate><ReceptionMethod>Email</ReceptionMethod><SAPPaymentMethod></SAPPaymentMethod><SelectedBankAccountID></SelectedBankAccountID><TaxAmount>0</TaxAmount><UnplannedDeliveryCosts>0</UnplannedDeliveryCosts><VendorCity>KANSAS CITY </VendorCity><VendorCountry></VendorCountry><VendorName>EAN SERVICES,LLC</VendorName><VendorNumber>EAN</VendorNumber><VendorPOBox></VendorPOBox><VendorRegion>MO </VendorRegion><VendorStreet>PO BOX 840173</VendorStreet><VendorZipCode>64184-0173</VendorZipCode><VerificationDate>2023-01-05</VerificationDate><ApproversList><item><ApprovalDate>2022-12-09</ApprovalDate><Approved>1</Approved><ApproverComment>ERP posting error (A timeout occurred during ERP integration. Please make sure that the invoice was not created in your ERP system before you retry posting it.): Posted by Service User</ApproverComment><ApproverEmail></ApproverEmail><ApproverID>apspecialistsprocess.su@20019011.esk</ApproverID><ApproverLabelRole>AP Specialist</ApproverLabelRole><Approver>AP Specialists</Approver></item><item><ApprovalDate/><Approved>0</Approved><ApproverComment></ApproverComment><ApproverEmail></ApproverEmail><ApproverID>apspecialistsprocess.su@20019011.esk</ApproverID><ApproverLabelRole>AP Specialist</ApproverLabelRole><Approver>AP Specialists</Approver></item></ApproversList><LineItems><item><Amount>2582.78</Amount><Assignment></Assignment><BusinessArea></BusinessArea><CCDescription>HR</CCDescription><CostCenter>6001</CostCenter><Description></Description><GLAccount></GLAccount><GLDescription></GLDescription><InternalOrder></InternalOrder><LineType>GL</LineType><TaxAmount>0</TaxAmount><TaxCode></TaxCode><TaxJurisdiction></TaxJurisdiction><TaxRate>0</TaxRate><Z_Phase>1</Z_Phase></item><item><Amount>3759.36</Amount><Assignment></Assignment><BusinessArea></BusinessArea><CCDescription>Farming Overhead- East</CCDescription><CostCenter>1007</CostCenter><Description></Description><GLAccount></GLAccount><GLDescription></GLDescription><InternalOrder></InternalOrder><LineType>GL</LineType><TaxAmount>0</TaxAmount><TaxCode></TaxCode><TaxJurisdiction></TaxJurisdiction><TaxRate>0</TaxRate><Z_Phase>2</Z_Phase></item><item><Amount>10022.6</Amount><Assignment></Assignment><BusinessArea></BusinessArea><CCDescription></CCDescription><CostCenter></CostCenter><Description></Description><GLAccount>1008</GLAccount><GLDescription></GLDescription><InternalOrder></InternalOrder><LineType>GL</LineType><TaxAmount>0</TaxAmount><TaxCode></TaxCode><TaxJurisdiction></TaxJurisdiction><TaxRate>0</TaxRate><Z_Phase></Z_Phase></item></LineItems><InvoiceDocumentURL>https://cc6.ondemand.esker.com:443/ondemand/webaccess/asj/ManageDocumentsCheck.link?ruid=CD%23PHOK41000SGA.1021296811651564047</InvoiceDocumentURL><InvoiceImageURL>https://cc6.ondemand.esker.com:443/ondemand/webaccess/asj/attach.file?id=CD%23PHOK41000SGA.1021296811651564047&amp;attachment=0</InvoiceImageURL></Invoice>";
		}

		private string TestString_BlankPostingDate()
		{
			return @"<?xml version=""1.0"" encoding=""UTF-8""?><Invoice RUID=""CD#PHOK41000SGA.1021296811651564047""><AlternativePayee></AlternativePayee><Assignment></Assignment><BaselineDate/><BusinessArea></BusinessArea><CalculateTax>1</CalculateTax><CompanyCode>PW01</CompanyCode><ContractNumber></ContractNumber><DiscountLimitDate>2022-11-05</DiscountLimitDate><DueDate>2022-11-05</DueDate><ERPLinkingDate>2023-01-05</ERPLinkingDate><ERPPaymentBlocked>0</ERPPaymentBlocked><ERPPostingDate>2023-01-05</ERPPostingDate><ERP>generic</ERP><EstimatedDiscountAmount>0</EstimatedDiscountAmount><ExchangeRate>1</ExchangeRate><GRIV>0</GRIV><HeaderText></HeaderText><History>09/12/2022 20:59:15 - ERP posting error: A timeout occurred during ERP integration. Please make sure that the invoice was not created in your ERP system before you retry posting it.
09/12/2022 20:59:15 - Posted by AP Specialists
</History><InvoiceAmount>16364.74</InvoiceAmount><InvoiceCurrency>USD</InvoiceCurrency><InvoiceDate>2022-11-05</InvoiceDate><InvoiceDescription>Test Description Blank Posting Date #1 and some extra for cut off</InvoiceDescription><InvoiceNumber>31364031</InvoiceNumber><InvoiceReferenceNumber></InvoiceReferenceNumber><InvoiceType>Non-PO Invoice</InvoiceType><LocalCurrency>USD</LocalCurrency><LocalEstimatedDiscountAmount>0</LocalEstimatedDiscountAmount><LocalInvoiceAmount>16364.74</LocalInvoiceAmount><LocalNetAmount>16364.74</LocalNetAmount><LocalTaxAmount>0</LocalTaxAmount><ManualLink>0</ManualLink><NetAmount>16364.74</NetAmount><OrderNumber>Test1234</OrderNumber><PaymentApprovalStatus>Not requested</PaymentApprovalStatus><PaymentTerms>PAYMENT</PaymentTerms><PostingDate></PostingDate><ReceptionMethod>Email</ReceptionMethod><SAPPaymentMethod></SAPPaymentMethod><SelectedBankAccountID></SelectedBankAccountID><TaxAmount>0</TaxAmount><UnplannedDeliveryCosts>0</UnplannedDeliveryCosts><VendorCity>KANSAS CITY </VendorCity><VendorCountry></VendorCountry><VendorName>EAN SERVICES,LLC</VendorName><VendorNumber>EAN</VendorNumber><VendorPOBox></VendorPOBox><VendorRegion>MO </VendorRegion><VendorStreet>PO BOX 840173</VendorStreet><VendorZipCode>64184-0173</VendorZipCode><VerificationDate>2023-01-05</VerificationDate><ApproversList><item><ApprovalDate>2022-12-09</ApprovalDate><Approved>1</Approved><ApproverComment>ERP posting error (A timeout occurred during ERP integration. Please make sure that the invoice was not created in your ERP system before you retry posting it.): Posted by Service User</ApproverComment><ApproverEmail></ApproverEmail><ApproverID>apspecialistsprocess.su@20019011.esk</ApproverID><ApproverLabelRole>AP Specialist</ApproverLabelRole><Approver>AP Specialists</Approver></item><item><ApprovalDate/><Approved>0</Approved><ApproverComment></ApproverComment><ApproverEmail></ApproverEmail><ApproverID>apspecialistsprocess.su@20019011.esk</ApproverID><ApproverLabelRole>AP Specialist</ApproverLabelRole><Approver>AP Specialists</Approver></item></ApproversList><LineItems><item><Amount>2582.78</Amount><Assignment></Assignment><BusinessArea></BusinessArea><CCDescription>HR</CCDescription><CostCenter>6001</CostCenter><Description></Description><GLAccount></GLAccount><GLDescription></GLDescription><InternalOrder></InternalOrder><LineType>GL</LineType><TaxAmount>0</TaxAmount><TaxCode></TaxCode><TaxJurisdiction></TaxJurisdiction><TaxRate>0</TaxRate><Z_Phase>1</Z_Phase></item><item><Amount>3759.36</Amount><Assignment></Assignment><BusinessArea></BusinessArea><CCDescription>Farming Overhead- East</CCDescription><CostCenter>1007</CostCenter><Description></Description><GLAccount></GLAccount><GLDescription></GLDescription><InternalOrder></InternalOrder><LineType>GL</LineType><TaxAmount>0</TaxAmount><TaxCode></TaxCode><TaxJurisdiction></TaxJurisdiction><TaxRate>0</TaxRate><Z_Phase>2</Z_Phase></item><item><Amount>10022.6</Amount><Assignment></Assignment><BusinessArea></BusinessArea><CCDescription></CCDescription><CostCenter></CostCenter><Description></Description><GLAccount>1008</GLAccount><GLDescription></GLDescription><InternalOrder></InternalOrder><LineType>GL</LineType><TaxAmount>0</TaxAmount><TaxCode></TaxCode><TaxJurisdiction></TaxJurisdiction><TaxRate>0</TaxRate><Z_Phase></Z_Phase></item></LineItems><InvoiceDocumentURL>https://cc6.ondemand.esker.com:443/ondemand/webaccess/asj/ManageDocumentsCheck.link?ruid=CD%23PHOK41000SGA.1021296811651564047</InvoiceDocumentURL><InvoiceImageURL>https://cc6.ondemand.esker.com:443/ondemand/webaccess/asj/attach.file?id=CD%23PHOK41000SGA.1021296811651564047&amp;attachment=0</InvoiceImageURL></Invoice>";
		}

		private string TestString_BlankPostingDate2()
		{
			return @"<?xml version=""1.0"" encoding=""UTF-8""?><Invoice RUID=""CD#PHOK41000SGA.1021296811651564047""><AlternativePayee></AlternativePayee><Assignment></Assignment><BaselineDate/><BusinessArea></BusinessArea><CalculateTax>1</CalculateTax><CompanyCode>PW01</CompanyCode><ContractNumber></ContractNumber><DiscountLimitDate>2022-11-05</DiscountLimitDate><DueDate>2022-11-05</DueDate><ERPLinkingDate>2023-01-05</ERPLinkingDate><ERPPaymentBlocked>0</ERPPaymentBlocked><ERPPostingDate>2023-01-05</ERPPostingDate><ERP>generic</ERP><EstimatedDiscountAmount>0</EstimatedDiscountAmount><ExchangeRate>1</ExchangeRate><GRIV>0</GRIV><HeaderText></HeaderText><History>09/12/2022 20:59:15 - ERP posting error: A timeout occurred during ERP integration. Please make sure that the invoice was not created in your ERP system before you retry posting it.
09/12/2022 20:59:15 - Posted by AP Specialists
</History><InvoiceAmount>16364.74</InvoiceAmount><InvoiceCurrency>USD</InvoiceCurrency><InvoiceDate>2022-11-05</InvoiceDate><InvoiceDescription>Test Description Blank Posting Date #2 and some extra for cut off</InvoiceDescription><InvoiceNumber>31364031</InvoiceNumber><InvoiceReferenceNumber></InvoiceReferenceNumber><InvoiceType>Non-PO Invoice</InvoiceType><LocalCurrency>USD</LocalCurrency><LocalEstimatedDiscountAmount>0</LocalEstimatedDiscountAmount><LocalInvoiceAmount>16364.74</LocalInvoiceAmount><LocalNetAmount>16364.74</LocalNetAmount><LocalTaxAmount>0</LocalTaxAmount><ManualLink>0</ManualLink><NetAmount>16364.74</NetAmount><OrderNumber>Test1234</OrderNumber><PaymentApprovalStatus>Not requested</PaymentApprovalStatus><PaymentTerms>PAYMENT</PaymentTerms><PostingDate/><ReceptionMethod>Email</ReceptionMethod><SAPPaymentMethod></SAPPaymentMethod><SelectedBankAccountID></SelectedBankAccountID><TaxAmount>0</TaxAmount><UnplannedDeliveryCosts>0</UnplannedDeliveryCosts><VendorCity>KANSAS CITY </VendorCity><VendorCountry></VendorCountry><VendorName>EAN SERVICES,LLC</VendorName><VendorNumber>EAN</VendorNumber><VendorPOBox></VendorPOBox><VendorRegion>MO </VendorRegion><VendorStreet>PO BOX 840173</VendorStreet><VendorZipCode>64184-0173</VendorZipCode><VerificationDate>2023-01-05</VerificationDate><ApproversList><item><ApprovalDate>2022-12-09</ApprovalDate><Approved>1</Approved><ApproverComment>ERP posting error (A timeout occurred during ERP integration. Please make sure that the invoice was not created in your ERP system before you retry posting it.): Posted by Service User</ApproverComment><ApproverEmail></ApproverEmail><ApproverID>apspecialistsprocess.su@20019011.esk</ApproverID><ApproverLabelRole>AP Specialist</ApproverLabelRole><Approver>AP Specialists</Approver></item><item><ApprovalDate/><Approved>0</Approved><ApproverComment></ApproverComment><ApproverEmail></ApproverEmail><ApproverID>apspecialistsprocess.su@20019011.esk</ApproverID><ApproverLabelRole>AP Specialist</ApproverLabelRole><Approver>AP Specialists</Approver></item></ApproversList><LineItems><item><Amount>2582.78</Amount><Assignment></Assignment><BusinessArea></BusinessArea><CCDescription>HR</CCDescription><CostCenter>6001</CostCenter><Description></Description><GLAccount></GLAccount><GLDescription></GLDescription><InternalOrder></InternalOrder><LineType>GL</LineType><TaxAmount>0</TaxAmount><TaxCode></TaxCode><TaxJurisdiction></TaxJurisdiction><TaxRate>0</TaxRate><Z_Phase>1</Z_Phase></item><item><Amount>3759.36</Amount><Assignment></Assignment><BusinessArea></BusinessArea><CCDescription>Farming Overhead- East</CCDescription><CostCenter>1007</CostCenter><Description></Description><GLAccount></GLAccount><GLDescription></GLDescription><InternalOrder></InternalOrder><LineType>GL</LineType><TaxAmount>0</TaxAmount><TaxCode></TaxCode><TaxJurisdiction></TaxJurisdiction><TaxRate>0</TaxRate><Z_Phase>2</Z_Phase></item><item><Amount>10022.6</Amount><Assignment></Assignment><BusinessArea></BusinessArea><CCDescription></CCDescription><CostCenter></CostCenter><Description></Description><GLAccount>1008</GLAccount><GLDescription></GLDescription><InternalOrder></InternalOrder><LineType>GL</LineType><TaxAmount>0</TaxAmount><TaxCode></TaxCode><TaxJurisdiction></TaxJurisdiction><TaxRate>0</TaxRate><Z_Phase></Z_Phase></item></LineItems><InvoiceDocumentURL>https://cc6.ondemand.esker.com:443/ondemand/webaccess/asj/ManageDocumentsCheck.link?ruid=CD%23PHOK41000SGA.1021296811651564047</InvoiceDocumentURL><InvoiceImageURL>https://cc6.ondemand.esker.com:443/ondemand/webaccess/asj/attach.file?id=CD%23PHOK41000SGA.1021296811651564047&amp;attachment=0</InvoiceImageURL></Invoice>";
		}

		private string TestString_NullDueDate()
		{
			return @"<?xml version=""1.0"" encoding=""UTF-8""?><Invoice RUID=""CD#PHOK41000SGA.1021296811651564047""><AlternativePayee></AlternativePayee><Assignment></Assignment><BaselineDate/><BusinessArea></BusinessArea><CalculateTax>1</CalculateTax><CompanyCode>PW01</CompanyCode><ContractNumber></ContractNumber><DiscountLimitDate>2022-11-05</DiscountLimitDate><DueDate></DueDate><ERPLinkingDate>2023-01-05</ERPLinkingDate><ERPPaymentBlocked>0</ERPPaymentBlocked><ERPPostingDate>2023-01-05</ERPPostingDate><ERP>generic</ERP><EstimatedDiscountAmount>0</EstimatedDiscountAmount><ExchangeRate>1</ExchangeRate><GRIV>0</GRIV><HeaderText></HeaderText><History>09/12/2022 20:59:15 - ERP posting error: A timeout occurred during ERP integration. Please make sure that the invoice was not created in your ERP system before you retry posting it.
09/12/2022 20:59:15 - Posted by AP Specialists
</History><InvoiceAmount>16364.74</InvoiceAmount><InvoiceCurrency>USD</InvoiceCurrency><InvoiceDate>2022-11-05</InvoiceDate><InvoiceDescription>Test Description #1</InvoiceDescription><InvoiceNumber>31364031</InvoiceNumber><InvoiceReferenceNumber></InvoiceReferenceNumber><InvoiceType>Non-PO Invoice</InvoiceType><LocalCurrency>USD</LocalCurrency><LocalEstimatedDiscountAmount>0</LocalEstimatedDiscountAmount><LocalInvoiceAmount>16364.74</LocalInvoiceAmount><LocalNetAmount>16364.74</LocalNetAmount><LocalTaxAmount>0</LocalTaxAmount><ManualLink>0</ManualLink><NetAmount>16364.74</NetAmount><OrderNumber>Test1234</OrderNumber><PaymentApprovalStatus>Not requested</PaymentApprovalStatus><PaymentTerms>PAYMENT</PaymentTerms><PostingDate>2023-01-05</PostingDate><ReceptionMethod>Email</ReceptionMethod><SAPPaymentMethod></SAPPaymentMethod><SelectedBankAccountID></SelectedBankAccountID><TaxAmount>0</TaxAmount><UnplannedDeliveryCosts>0</UnplannedDeliveryCosts><VendorCity>KANSAS CITY </VendorCity><VendorCountry></VendorCountry><VendorName>EAN SERVICES,LLC</VendorName><VendorNumber>EAN</VendorNumber><VendorPOBox></VendorPOBox><VendorRegion>MO </VendorRegion><VendorStreet>PO BOX 840173</VendorStreet><VendorZipCode>64184-0173</VendorZipCode><VerificationDate>2023-01-05</VerificationDate><ApproversList><item><ApprovalDate>2022-12-09</ApprovalDate><Approved>1</Approved><ApproverComment>ERP posting error (A timeout occurred during ERP integration. Please make sure that the invoice was not created in your ERP system before you retry posting it.): Posted by Service User</ApproverComment><ApproverEmail></ApproverEmail><ApproverID>apspecialistsprocess.su@20019011.esk</ApproverID><ApproverLabelRole>AP Specialist</ApproverLabelRole><Approver>AP Specialists</Approver></item><item><ApprovalDate/><Approved>0</Approved><ApproverComment></ApproverComment><ApproverEmail></ApproverEmail><ApproverID>apspecialistsprocess.su@20019011.esk</ApproverID><ApproverLabelRole>AP Specialist</ApproverLabelRole><Approver>AP Specialists</Approver></item></ApproversList><LineItems><item><Amount>2582.78</Amount><Assignment></Assignment><BusinessArea></BusinessArea><CCDescription>HR</CCDescription><CostCenter>6001</CostCenter><Description></Description><GLAccount></GLAccount><GLDescription></GLDescription><InternalOrder></InternalOrder><LineType>GL</LineType><TaxAmount>0</TaxAmount><TaxCode></TaxCode><TaxJurisdiction></TaxJurisdiction><TaxRate>0</TaxRate><Z_Phase>1</Z_Phase></item><item><Amount>3759.36</Amount><Assignment></Assignment><BusinessArea></BusinessArea><CCDescription>Farming Overhead- East</CCDescription><CostCenter>1007</CostCenter><Description></Description><GLAccount></GLAccount><GLDescription></GLDescription><InternalOrder></InternalOrder><LineType>GL</LineType><TaxAmount>0</TaxAmount><TaxCode></TaxCode><TaxJurisdiction></TaxJurisdiction><TaxRate>0</TaxRate><Z_Phase>2</Z_Phase></item><item><Amount>10022.6</Amount><Assignment></Assignment><BusinessArea></BusinessArea><CCDescription></CCDescription><CostCenter></CostCenter><Description></Description><GLAccount>1008</GLAccount><GLDescription></GLDescription><InternalOrder></InternalOrder><LineType>GL</LineType><TaxAmount>0</TaxAmount><TaxCode></TaxCode><TaxJurisdiction></TaxJurisdiction><TaxRate>0</TaxRate><Z_Phase></Z_Phase></item></LineItems><InvoiceDocumentURL>https://cc6.ondemand.esker.com:443/ondemand/webaccess/asj/ManageDocumentsCheck.link?ruid=CD%23PHOK41000SGA.1021296811651564047</InvoiceDocumentURL><InvoiceImageURL>https://cc6.ondemand.esker.com:443/ondemand/webaccess/asj/attach.file?id=CD%23PHOK41000SGA.1021296811651564047&amp;attachment=0</InvoiceImageURL></Invoice>";
		}

		private string TestString_NullDueDate2()
		{
			return @"<?xml version=""1.0"" encoding=""UTF-8""?><Invoice RUID=""CD#PHOK41000SGA.1021296811651564047""><AlternativePayee></AlternativePayee><Assignment></Assignment><BaselineDate/><BusinessArea></BusinessArea><CalculateTax>1</CalculateTax><CompanyCode>PW01</CompanyCode><ContractNumber></ContractNumber><DiscountLimitDate>2022-11-05</DiscountLimitDate><ERPLinkingDate>2023-01-05</ERPLinkingDate><ERPPaymentBlocked>0</ERPPaymentBlocked><ERPPostingDate>2023-01-05</ERPPostingDate><ERP>generic</ERP><EstimatedDiscountAmount>0</EstimatedDiscountAmount><ExchangeRate>1</ExchangeRate><GRIV>0</GRIV><HeaderText></HeaderText><History>09/12/2022 20:59:15 - ERP posting error: A timeout occurred during ERP integration. Please make sure that the invoice was not created in your ERP system before you retry posting it.
09/12/2022 20:59:15 - Posted by AP Specialists
</History><InvoiceAmount>16364.74</InvoiceAmount><InvoiceCurrency>USD</InvoiceCurrency><InvoiceDate>2022-11-05</InvoiceDate><InvoiceDescription>Test Description #1</InvoiceDescription><InvoiceNumber>31364031</InvoiceNumber><InvoiceReferenceNumber></InvoiceReferenceNumber><InvoiceType>Non-PO Invoice</InvoiceType><LocalCurrency>USD</LocalCurrency><LocalEstimatedDiscountAmount>0</LocalEstimatedDiscountAmount><LocalInvoiceAmount>16364.74</LocalInvoiceAmount><LocalNetAmount>16364.74</LocalNetAmount><LocalTaxAmount>0</LocalTaxAmount><ManualLink>0</ManualLink><NetAmount>16364.74</NetAmount><OrderNumber>Test1234</OrderNumber><PaymentApprovalStatus>Not requested</PaymentApprovalStatus><PaymentTerms>PAYMENT</PaymentTerms><PostingDate>2023-01-05</PostingDate><ReceptionMethod>Email</ReceptionMethod><SAPPaymentMethod></SAPPaymentMethod><SelectedBankAccountID></SelectedBankAccountID><TaxAmount>0</TaxAmount><UnplannedDeliveryCosts>0</UnplannedDeliveryCosts><VendorCity>KANSAS CITY </VendorCity><VendorCountry></VendorCountry><VendorName>EAN SERVICES,LLC</VendorName><VendorNumber>EAN</VendorNumber><VendorPOBox></VendorPOBox><VendorRegion>MO </VendorRegion><VendorStreet>PO BOX 840173</VendorStreet><VendorZipCode>64184-0173</VendorZipCode><VerificationDate>2023-01-05</VerificationDate><ApproversList><item><ApprovalDate>2022-12-09</ApprovalDate><Approved>1</Approved><ApproverComment>ERP posting error (A timeout occurred during ERP integration. Please make sure that the invoice was not created in your ERP system before you retry posting it.): Posted by Service User</ApproverComment><ApproverEmail></ApproverEmail><ApproverID>apspecialistsprocess.su@20019011.esk</ApproverID><ApproverLabelRole>AP Specialist</ApproverLabelRole><Approver>AP Specialists</Approver></item><item><ApprovalDate/><Approved>0</Approved><ApproverComment></ApproverComment><ApproverEmail></ApproverEmail><ApproverID>apspecialistsprocess.su@20019011.esk</ApproverID><ApproverLabelRole>AP Specialist</ApproverLabelRole><Approver>AP Specialists</Approver></item></ApproversList><LineItems><item><Amount>2582.78</Amount><Assignment></Assignment><BusinessArea></BusinessArea><CCDescription>HR</CCDescription><CostCenter>6001</CostCenter><Description></Description><GLAccount></GLAccount><GLDescription></GLDescription><InternalOrder></InternalOrder><LineType>GL</LineType><TaxAmount>0</TaxAmount><TaxCode></TaxCode><TaxJurisdiction></TaxJurisdiction><TaxRate>0</TaxRate><Z_Phase>1</Z_Phase></item><item><Amount>3759.36</Amount><Assignment></Assignment><BusinessArea></BusinessArea><CCDescription>Farming Overhead- East</CCDescription><CostCenter>1007</CostCenter><Description></Description><GLAccount></GLAccount><GLDescription></GLDescription><InternalOrder></InternalOrder><LineType>GL</LineType><TaxAmount>0</TaxAmount><TaxCode></TaxCode><TaxJurisdiction></TaxJurisdiction><TaxRate>0</TaxRate><Z_Phase>2</Z_Phase></item><item><Amount>10022.6</Amount><Assignment></Assignment><BusinessArea></BusinessArea><CCDescription></CCDescription><CostCenter></CostCenter><Description></Description><GLAccount>1008</GLAccount><GLDescription></GLDescription><InternalOrder></InternalOrder><LineType>GL</LineType><TaxAmount>0</TaxAmount><TaxCode></TaxCode><TaxJurisdiction></TaxJurisdiction><TaxRate>0</TaxRate><Z_Phase></Z_Phase></item></LineItems><InvoiceDocumentURL>https://cc6.ondemand.esker.com:443/ondemand/webaccess/asj/ManageDocumentsCheck.link?ruid=CD%23PHOK41000SGA.1021296811651564047</InvoiceDocumentURL><InvoiceImageURL>https://cc6.ondemand.esker.com:443/ondemand/webaccess/asj/attach.file?id=CD%23PHOK41000SGA.1021296811651564047&amp;attachment=0</InvoiceImageURL></Invoice>";
		}
	}
}
