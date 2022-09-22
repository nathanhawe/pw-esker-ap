using EskerAP.Domain;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Data.OracleClient;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Oracle.ManagedDataAccess.Types;
using System.Data;

namespace EskerAP.Data.Famous
{
	public class ImportApVouchersRepo : IImportApVouchersRepo
	{
		private string _connectionString;
		private string _schema;
		private bool _hasSchema;
		private string _famousUserId;
		private string _famousPassword;

		public ImportApVouchersRepo(string connectionString, string schema, string famousUserId, string famousPassword)
		{
			_connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
			_schema = schema?.Trim();
			_hasSchema = !String.IsNullOrWhiteSpace(schema);
			_famousUserId = famousUserId ?? throw new ArgumentNullException(nameof(famousUserId));
			_famousPassword = famousPassword ?? throw new ArgumentNullException(nameof(famousPassword));
		}

		public void ImportVoucher(Domain.Voucher voucher)
		{
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
				var byteArray = ByteArray();
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


				// Make query
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

				// Errors with parsing
				var temp2 = (OracleClob)cmd.Parameters["aclobOtherErrors"].Value;
				if (!temp2.IsNull)
				{
					string errors = temp2?.Value;
					Console.WriteLine(errors);
				}

				// Errors logical errors
				var temp3 = (OracleClob)cmd.Parameters["aVouchers"].Value;
				string status = temp3?.Value;
				Console.WriteLine(status);

			}
			catch(Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
		}

		private byte[] ByteArray()
		{
			var temp = XmlString();
			var encoding = new UnicodeEncoding();
			byte[] bytes = encoding.GetBytes(temp);

			return bytes;
		}
		private string XmlString()
		{
			return @"
				<Vouchers>
					<ROWSET>
						<Voucher>
							<EntryNumber/>
							<VendorId>TEST</VendorId>
							<InvoiceNumber>TEST012345</InvoiceNumber>
							<HoldFlag>N</HoldFlag>
							<InvoiceDate>2021-09-20</InvoiceDate>
							<StubDescription/>
							<PayTerms>PAYMENT</PayTerms>
							<DueDate/>
							<DiscountDate/>
							<PayByDate/>
							<DiscountAmount/>
							<AccessGroupName/>
							<PoSourceNumber/>
							<AP1099Code/>
							<VoucherAmount>100</VoucherAmount>
							<VoucherImportStatus>Import Ready</VoucherImportStatus>
							<AllowDuplicateVendorInvoice>N</AllowDuplicateVendorInvoice>
							<HeaderErrors/>
							<LineCount>1</LineCount>
							<NoteCount>0</NoteCount>
							<Lines>
								<ROWSET>
									<Line>
										<CostCenterId>TEST</CostCenterId>
										<GrowerBlockId/>
										<PhaseId>1100</PhaseId>
										<DepartmentId/>
										<GlAccountId/>
										<LineDescription>Testing line #1.</LineDescription>
										<Hours/>
										<Quantity>50</Quantity>
										<Rate>2</Rate>
										<Amount>100</Amount>
										<ChargeId/>
										<PoolId/>
										<LotId/>
										<LineReference/>
										<LineErrors/>
									</Line>
								</ROWSET>
							</Lines>
							<Notes/>
						</Voucher>
					</ROWSET>
				</Vouchers>";
		}
	}
}
