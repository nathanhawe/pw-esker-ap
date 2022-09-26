using EskerAP.Domain;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Text;
using Oracle.ManagedDataAccess.Types;
using Microsoft.Extensions.Logging;

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

		public void ImportVoucher(Domain.Voucher voucher)
		{
			_logger.LogDebug("Starting voucher import for voucher:'{voucher}' with connection string:'{connectionString}', Famous user ID:'{famousUserId}', and Famous password: '{famousPassword}'.", voucher, _connectionString, _famousUserId, _famousPassword);

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

				/* Error Handling */
				// Check for errors with XML parsing
				var temp2 = (OracleClob)cmd.Parameters["aclobOtherErrors"].Value;
				if (!temp2.IsNull)
				{
					string errors = temp2?.Value;
					Console.WriteLine(errors);
				}

				// Check for logical errors
				var temp3 = (OracleClob)cmd.Parameters["aVouchers"].Value;
				string status = temp3?.Value;
				Console.WriteLine(status);

			}
			catch(Exception ex)
			{
				_logger.LogError("An exception occured while importing voucher for {VendorId} and {InvoiceNumber}. Exception: {Message}", voucher.VendorId, voucher.InvoiceNumber, ex.Message);
			}
		}

		private byte[] GetVoucherByteArray(Voucher voucher)
		{
			var temp = GetVoucherXmlString(voucher);
			var encoding = new UnicodeEncoding();
			byte[] bytes = encoding.GetBytes(temp);

			return bytes;
		}
		private string GetVoucherXmlString(Voucher voucher)
		{
			return @$"
				<Vouchers>
					<ROWSET>
						<Voucher>
							<EntryNumber/>
							<VendorId>{StringHelper(voucher.VendorId,6)}</VendorId>
							<InvoiceNumber>{StringHelper(voucher.InvoiceNumber, 12)}</InvoiceNumber>
							<HoldFlag>{voucher.HoldFlag}</HoldFlag>
							<InvoiceDate>{DateHelper(voucher.InvoiceDate)}</InvoiceDate>
							<StubDescription>{StringHelper(voucher.StubDescription, 40)}</StubDescription>
							<PayTerms>{StringHelper(voucher.PayTerms,20)}</PayTerms>
							<DueDate>{DateHelper(voucher.DueDate)}</DueDate>
							<DiscountDate>{DateHelper(voucher.DiscountDate)}</DiscountDate>
							<PayByDate>{DateHelper(voucher.PayByDate)}</PayByDate>
							<DiscountAmount>{voucher.DiscountAmount}</DiscountAmount>
							<AccessGroupName>{StringHelper(voucher.AccessGroupName, 40)}</AccessGroupName>
							<PoSourceNumber>{StringHelper(voucher.PoSourceNumber, 8)}</PoSourceNumber>
							<AP1099Code>{StringHelper(voucher.AP1099Code, 20)}</AP1099Code>
							<VoucherAmount>{voucher.VoucherAmount}</VoucherAmount>
							<VoucherImportStatus>{voucher.VoucherImportStatus}</VoucherImportStatus>
							<AllowDuplicateVendorInvoice>{voucher.AllowDuplicateVendorInvoice}</AllowDuplicateVendorInvoice>
							<HeaderErrors/>
							<LineCount>{voucher.LineCount}</LineCount>
							<NoteCount>{voucher.NoteCount}</NoteCount>
							<Lines>
								<ROWSET>
									{GetVoucherLinesXmlString(voucher.Lines)}
								</ROWSET>
							</Lines>
							<Notes/>
						</Voucher>
					</ROWSET>
				</Vouchers>";
		}

		private string GetVoucherLinesXmlString(List<VoucherItem> lines)
		{
			var sb = new StringBuilder();
			foreach (var line in lines)
			{
				sb.AppendLine($@"
					<Line>
						<CostCenterId>{StringHelper(line.CostCenterId, 12)}</CostCenterId>
						<GrowerBlockId>{StringHelper(line.GrowBlockId, 12)}</GrowerBlockId>
						<PhaseId>{StringHelper(line.PhaseId, 6)}</PhaseId>
						<DepartmentId>{StringHelper(line.DepartmentId, 6)}</DepartmentId>
						<GlAccountId>{StringHelper(line.GlAccountCode, 12)}</GlAccountId>
						<LineDescription>{StringHelper(line.LineDescription, 40)}</LineDescription>
						<Hours>{(line.CostCenterId != null || line.GrowBlockId != null ? line.Hours.ToString() : "")}</Hours>
						<Quantity>{line.Quantity}</Quantity>
						<Rate>{line.Rate}</Rate>
						<Amount>{line.Amount}</Amount>
						<ChargeId>{StringHelper(line.ChargeId, 6)}</ChargeId>
						<PoolId>{StringHelper(line.PoolId, 12)}</PoolId>
						<LotId>{StringHelper(line.LotId, 12)}</LotId>
						<LineReference>{StringHelper(line.LineReference, 12)}</LineReference>
						<LineErrors/>
					</Line>");
			}
			return sb.ToString();
		}

		private string StringHelper(string text, int maxLength)
		{
			if (string.IsNullOrWhiteSpace(text)) return "";

			return text[..Math.Min(text.Length, maxLength)];
		}

		private string DateHelper(DateTime? date) => date?.ToString("yyyy-MM-dd") ?? "";
	}
}
