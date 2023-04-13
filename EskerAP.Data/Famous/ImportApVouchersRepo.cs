using EskerAP.Domain;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Text;
using Oracle.ManagedDataAccess.Types;
using Microsoft.Extensions.Logging;
using System.Xml.Linq;
using System.Xml;
using System.Security.Cryptography;

namespace EskerAP.Data.Famous
{
	public class ImportApVouchersRepo : IImportApVouchersRepo
	{
		private readonly string _connectionString;
		private readonly string _schema;
		private readonly bool _hasSchema;
		private readonly string _famousUserId;
		private readonly string _famousPassword;
		private readonly ILogger<ImportApVouchersRepo> _logger;

		public ImportApVouchersRepo(string connectionString, string schema, string famousUserId, string famousPassword, ILogger<ImportApVouchersRepo> logger)
		{
			_connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
			_schema = schema?.Trim();
			_hasSchema = !String.IsNullOrWhiteSpace(schema);
			_famousUserId = famousUserId ?? throw new ArgumentNullException(nameof(famousUserId));
			_famousPassword = famousPassword ?? throw new ArgumentNullException(nameof(famousPassword));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public ImportApVoucherResponse ImportVoucher(Voucher voucher)
		{
			_logger.LogDebug("Starting voucher import for voucher:'{voucher}' with connection string:'{connectionString}', Famous user ID:'{famousUserId}', and Famous password: '{famousPassword}'.", voucher, _connectionString, _famousUserId, _famousPassword);

			var response = new ImportApVoucherResponse();

			using OracleConnection con = new OracleConnection(_connectionString);
			using OracleCommand cmd = con.CreateCommand();
			try
			{
				
				con.Open();
				cmd.BindByName = true;


				// Create a temporary CLOB
				cmd.Parameters.Clear();
				cmd.CommandText = "declare xx clob; begin dbms_lob.createtemporary(xx, false, 0); :tempclob := xx; end;";
				cmd.Parameters.Add(new OracleParameter("tempclob", OracleDbType.Clob)).Direction = System.Data.ParameterDirection.InputOutput;
				cmd.ExecuteNonQuery();

				// Assign values
				var byteArray = GetVoucherByteArray(voucher);
				var tempLob = (OracleClob)cmd.Parameters[0].Value;
				tempLob.BeginChunkWrite();
				tempLob.Write(byteArray, 0, byteArray.Length);
				tempLob.EndChunkWrite();

				// Attempt login first
				cmd.Parameters.Clear();
				cmd.CommandText = $"{(_hasSchema ? _schema + "." : "")}FAPI.LogIn";
				cmd.CommandType = System.Data.CommandType.StoredProcedure;
				cmd.Parameters.Add("anReturn", OracleDbType.Int32, System.Data.ParameterDirection.Output);
				cmd.Parameters.Add("avFamousUser", OracleDbType.Varchar2, _famousUserId, System.Data.ParameterDirection.Input);
				cmd.Parameters.Add("avPassword", OracleDbType.Varchar2, _famousPassword, System.Data.ParameterDirection.Input);
				cmd.ExecuteNonQuery();

				var temp = (OracleDecimal)cmd.Parameters["anReturn"].Value;
				int userId = System.Convert.ToInt32(temp.Value);


				// Invoke ImportApVoucher Procedure
				cmd.Parameters.Clear();
				cmd.CommandText = $"{(_hasSchema ? _schema + "." : "")}FAPI.ImportApVouchers";
				cmd.CommandType = System.Data.CommandType.StoredProcedure;

				cmd.Parameters.Add("aVouchers", OracleDbType.Clob).Value = tempLob;
				cmd.Parameters.Add("anSucceeded", OracleDbType.Int32).Value = 0;
				cmd.Parameters.Add("anFailed", OracleDbType.Int32).Value = 0;
				cmd.Parameters.Add("anSkipped", OracleDbType.Int32).Value = 0;
				cmd.Parameters.Add("aclobOtherErrors", OracleDbType.Clob).Value = null;
				cmd.Parameters.Add("anReturnCode", OracleDbType.Int32).Value = 0;
				cmd.Parameters.Add("anFamousUserIdx", OracleDbType.Int32).Value = userId;

				cmd.ExecuteNonQuery();

				/* Error Handling and Response Creation */
				response.SucceededCount = ((OracleDecimal)cmd.Parameters["anSucceeded"].Value).ToInt32();
				response.Failedcount = ((OracleDecimal)cmd.Parameters["anFailed"].Value).ToInt32();
				response.SkippedCount = ((OracleDecimal)cmd.Parameters["anSkipped"].Value).ToInt32();

				// Check for errors with XML parsing
				var otherErrors = (OracleClob)cmd.Parameters["aclobOtherErrors"].Value;
				if (!otherErrors.IsNull)
				{
					response.OtherErrors = otherErrors?.Value;
				}

				// Check for logical errors
				var voucherOutClob = (OracleClob)cmd.Parameters["aVouchers"].Value;
				if (!voucherOutClob.IsNull)
				{
					response.RawXmlVoucherResponse = voucherOutClob?.Value;
					var document = XElement.Parse(voucherOutClob?.Value);
					response.HeaderErrors = GetHeaderErrors(document);
					response.LineErrors = GetLineErrors(document);
				}
				
			}
			catch(Exception ex)
			{
				_logger.LogError("An exception occured while importing voucher for {VendorId} and {InvoiceNumber}. Exception: {Message}", voucher.VendorId, voucher.InvoiceNumber, ex.Message);
				response.Exception = ex;
			}

			return response;
		}

		private string GetHeaderErrors(XElement document)
		{
			var sb = new StringBuilder();
			var headerErrors = document.Descendants("HeaderErrors");
			foreach (var headerError in headerErrors)
			{
				sb.Append(headerError?.Value?.Trim());
			}
			return sb.ToString();
		}

		private List<string> GetLineErrors(XElement document)
		{
			var response = new List<string>();
			var lineErrors = document.Descendants("LineErrors");
			foreach(var error in lineErrors)
			{ 
				if(!string.IsNullOrWhiteSpace(error?.Value))
				{
					response.Add(error.Value);
				}
			}

			return response;
		}

		private byte[] GetVoucherByteArray(Voucher voucher)
		{
			var temp = GetVoucherXmlString(voucher);
			var encoding = new UnicodeEncoding();
			byte[] bytes = encoding.GetBytes(temp);

			return bytes;
		}
		private string GetVoucherXmlString(Voucher v)
		{
			var doc = new XmlDocument();
			var vouchers = doc.CreateElement("Vouchers");
			var vouchersRowset = doc.CreateElement("ROWSET");
			var voucher = doc.CreateElement("Voucher");
			var entryNumber = doc.CreateElement("EntryNumber");
			var vendorId = doc.CreateElement("VendorId");
			var invoiceNumber = doc.CreateElement("InvoiceNumber");
			var holdFlag = doc.CreateElement("HoldFlag");
			var invoiceDate = doc.CreateElement("InvoiceDate");
			var stubDescription = doc.CreateElement("StubDescription");
			var payTerms = doc.CreateElement("PayTerms");
			var dueDate = doc.CreateElement("DueDate");
			var discountDate = doc.CreateElement("DiscountDate");
			var payByDate = doc.CreateElement("PayByDate");
			var discountAmount = doc.CreateElement("DiscountAmount");
			var accessGroupName = doc.CreateElement("AccessGroupName");
			var poSourceNumber = doc.CreateElement("PoSourceNumber");
			var ap1099Code = doc.CreateElement("AP1099Code");
			var voucherAmount = doc.CreateElement("VoucherAmount");
			var voucherImportStatus = doc.CreateElement("VoucherImportStatus");
			var allowDuplicateVendorInvoice = doc.CreateElement("AllowDuplicateVendorInvoice");
			var headerErrors = doc.CreateElement("HeaderErrors");
			var lineCount = doc.CreateElement("LineCount");
			var noteCount = doc.CreateElement("NoteCount");
			var lines = doc.CreateElement("Lines");
			var linesRowset = doc.CreateElement("ROWSET");

			vendorId.InnerText = StringHelper(v.VendorId, 6);
			invoiceNumber.InnerText = StringHelper(v.InvoiceNumber, 12);
			holdFlag.InnerText = v.HoldFlag.ToString();
			invoiceDate.InnerText = DateHelper(v.InvoiceDate);
			stubDescription.InnerText = StringHelper(v.StubDescription, 40);
			payTerms.InnerText = StringHelper(v.PayTerms,20);
			dueDate.InnerText = DateHelper(v.DueDate);
			discountDate.InnerText = DateHelper(v.DiscountDate);
			payByDate.InnerText = DateHelper(v.PayByDate);
			discountAmount.InnerText = v.DiscountAmount.ToString();
			accessGroupName.InnerText = StringHelper(v.AccessGroupName, 40);
			poSourceNumber.InnerText = StringHelper(v.PoSourceNumber, 8);
			ap1099Code.InnerText = StringHelper(v.AP1099Code, 20);
			voucherAmount.InnerText = v.VoucherAmount.ToString();
			voucherImportStatus.InnerText = v.VoucherImportStatus;
			allowDuplicateVendorInvoice.InnerText = v.AllowDuplicateVendorInvoice.ToString();
			lineCount.InnerText = v.LineCount.ToString();
			noteCount.InnerText = v.NoteCount.ToString();


			AddLinesTo(doc, linesRowset, v.Lines);
			lines.AppendChild(linesRowset);

			voucher.AppendChild(entryNumber);
			voucher.AppendChild(vendorId);
			voucher.AppendChild(invoiceNumber);
			voucher.AppendChild(holdFlag);
			voucher.AppendChild(invoiceDate);
			voucher.AppendChild(stubDescription);
			voucher.AppendChild(payTerms);
			voucher.AppendChild(dueDate);
			voucher.AppendChild(discountDate); 
			voucher.AppendChild(payByDate);
			voucher.AppendChild(discountAmount);
			voucher.AppendChild(accessGroupName);
			voucher.AppendChild(poSourceNumber);
			voucher.AppendChild(ap1099Code);
			voucher.AppendChild(voucherAmount);
			voucher.AppendChild(voucherImportStatus);
			voucher.AppendChild(allowDuplicateVendorInvoice);
			voucher.AppendChild(headerErrors);
			voucher.AppendChild(lineCount);
			voucher.AppendChild(noteCount);
			voucher.AppendChild(lines);

			vouchersRowset.AppendChild(voucher);
			vouchers.AppendChild(vouchersRowset);
			doc.AppendChild(vouchers);

			return doc.OuterXml;
		}

		private void AddLinesTo(XmlDocument doc, XmlElement rowset, List<VoucherItem> lines)
		{
			foreach(var line in lines)
			{
				var lineElement = doc.CreateElement("Line");
				var costCenterId = doc.CreateElement("CostCenterId");
				var growerBlockId = doc.CreateElement("GrowerBlockId");
				var phaseId = doc.CreateElement("PhaseId");
				var departmentId = doc.CreateElement("DepartmentId");
				var glAccountId = doc.CreateElement("GlAccountId");
				var lineDescription = doc.CreateElement("LineDescription");
				var hours = doc.CreateElement("Hours");
				var quantity = doc.CreateElement("Quantity");
				var rate = doc.CreateElement("Rate");
				var amount = doc.CreateElement("Amount");
				var chargeId = doc.CreateElement("ChargeId");
				var poolId = doc.CreateElement("PoolId");
				var lotId = doc.CreateElement("LotId");
				var lineReference = doc.CreateElement("LineReference");
				var lineErrors = doc.CreateElement("LineErrors");

				costCenterId.InnerText = StringHelper(line.CostCenterId, 12);
				growerBlockId.InnerText = StringHelper(line.GrowBlockId, 12);
				phaseId.InnerText = StringHelper(line.PhaseId, 6);
				departmentId.InnerText = StringHelper(line.DepartmentId, 6);
				glAccountId.InnerText = StringHelper(line.GlAccountCode, 12);
				lineDescription.InnerText = StringHelper(line.LineDescription, 40);
				hours.InnerText = (line.CostCenterId != null || line.GrowBlockId != null ? line.Hours.ToString() : "");
				quantity.InnerText = line.Quantity.ToString();
				rate.InnerText = line.Rate.ToString();
				amount.InnerText = line.Amount.ToString();
				chargeId.InnerText = StringHelper(line.ChargeId, 6);
				poolId.InnerText = StringHelper(line.PoolId, 12);
				lotId.InnerText = StringHelper(line.LotId, 12);
				lineReference.InnerText = StringHelper(line.LineReference, 12);

				lineElement.AppendChild(costCenterId);
				lineElement.AppendChild(growerBlockId);
				lineElement.AppendChild(phaseId);
				lineElement.AppendChild(departmentId);
				lineElement.AppendChild(glAccountId);
				lineElement.AppendChild(lineDescription);
				lineElement.AppendChild(hours);
				lineElement.AppendChild(quantity);
				lineElement.AppendChild(rate);
				lineElement.AppendChild(amount);
				lineElement.AppendChild(chargeId);
				lineElement.AppendChild(poolId);
				lineElement.AppendChild(lotId);
				lineElement.AppendChild(lineReference);
				lineElement.AppendChild(lineErrors);
				rowset.AppendChild(lineElement);
			}
		}

		private string StringHelper(string text, int maxLength)
		{
			if (string.IsNullOrWhiteSpace(text)) return "";

			return text[..Math.Min(text.Length, maxLength)];
		}

		private string DateHelper(DateTime? date) => date?.ToString("yyyy-MM-dd") ?? "";
	}
}
