using EskerAP.Domain;
using Microsoft.Extensions.Logging;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;

namespace EskerAP.Data.Famous
{
	public class ApVoucherRepo : IApVoucherRepo
	{
		private readonly string _connectionString;
		private readonly string _schema;
		private readonly bool _hasSchema;
		private readonly ILogger<ApVoucherRepo> _logger;

		public ApVoucherRepo(string connectionString, string schema, ILogger<ApVoucherRepo> logger)
		{
			_connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
			_schema = schema?.Trim();
			_hasSchema = !String.IsNullOrWhiteSpace(schema);
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public IEnumerable<Domain.PaidInvoice> GetPaidInvoices(int daysPast)
		{
			_logger.LogDebug("GetPaidInvoices() Invoked with current {daysPast}", daysPast);

			var paidInvoices = new List<PaidInvoice>();
			using OracleConnection con = new OracleConnection(_connectionString);
			using OracleCommand cmd = con.CreateCommand();
			try
			{
				con.Open();
				cmd.BindByName = true;
				cmd.CommandText = GetQuery(daysPast);
				OracleDataReader reader = cmd.ExecuteReader();

				PaidInvoice record;
				while (reader.Read())
				{
					record = new PaidInvoice
					{
						VendorNumber = reader.IsDBNull(0) ? "" : reader.GetString(0),
						InvoiceNumber = reader.IsDBNull(1) ? "" : reader.GetString(1),
						PaymentDate = reader.IsDBNull(2) ? DateTime.Now : reader.GetDateTime(2),
						PaymentMethod = GetEskerPaymentMethod(reader.IsDBNull(3) ? "" : reader.GetString(3)),
						PaymentReference = reader.IsDBNull(4) ? "" : reader.GetString(4),
					};

					paidInvoices.Add(record);
				}
				reader.Dispose();
			}
			catch(Exception ex)
			{
				_logger.LogError("An exception occured while attempting to get Paid Invoices from Famous: {Message}.", ex.Message);
			}

			return paidInvoices;

		}

		private string GetEskerPaymentMethod(string paymentType)
		{
			if (paymentType == Domain.Constants.PaymentType.Cash) return Domain.Constants.PaymentMethod.Cash;
			if (paymentType == Domain.Constants.PaymentType.Electronic) return Domain.Constants.PaymentMethod.Eft;
			if (paymentType == Domain.Constants.PaymentType.ManualCheck) return Domain.Constants.PaymentMethod.Check;
			if (paymentType == Domain.Constants.PaymentType.PrintedCheck) return Domain.Constants.PaymentMethod.Check;

			return "";
		}

		private string GetQuery(int daysPast)
		{
			return $@"SELECT
				-- COMPANY CODE
				 fn.ID AS VENDORNUMBER
				,hdr.VENDINVCNO AS INVOICENUMBER
				,apr.PAYDATE AS PAYMENTDATE
				,apr.PAYMENTTYPE 
				,apr.CHECKNO 
			FROM {(_hasSchema ? _schema + "." : "")}AP_VOUCHER_HEADER hdr
				LEFT JOIN {(_hasSchema ? _schema + "." : "")}FC_NAME fn ON
					hdr.VENDNAMEIDX = fn.NAMEIDX 
				LEFT JOIN {(_hasSchema ? _schema + "." : "")}AP_PAID_RUN apr ON
					hdr.APRUNIDX = apr.APRUNIDX 
			WHERE 
				hdr.APSTATUS = '4' 
				AND hdr.PAIDAMT > 0
				AND hdr.VENDINVCNO IS NOT NULL
				AND apr.PAYDATE >= SYSDATE - {daysPast}
			ORDER BY apr.PAYMENTTYPE  DESC";
		}
	}
}
