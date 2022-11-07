using EskerAP.Domain;
using EskerAP.Domain.Constants;
using Microsoft.Extensions.Logging;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace EskerAP.Data.Famous
{
	public class ApPayTermsRepo : IApPayTermsRepo
	{
		private readonly string _connectionString;
		private readonly string _schema;
		private readonly bool _hasSchema;
		private readonly ILogger<ApPayTermsRepo> _logger;

		public ApPayTermsRepo(string connectionString, string schema, ILogger<ApPayTermsRepo> logger)
		{
			_connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
			_schema = schema?.Trim();
			_hasSchema = !String.IsNullOrWhiteSpace(schema);
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public IEnumerable<Domain.PaymentTerm> GetPayTerms()
		{
			_logger.LogDebug("GetPayTerms() Invoked");

			var payTerms = new List<PaymentTerm>();
			using OracleConnection con = new OracleConnection(_connectionString);
			using OracleCommand cmd = con.CreateCommand();
			try
			{
				con.Open();
				cmd.BindByName = true;
				cmd.CommandText = $@"
					SELECT
						 apt.DESCR AS PAYMENTTERMCODE
						,apt.DESCR AS DESCRIPTION
						,0 AS LATEPAYMENTFEERATE
						, apt.DUEDATETYPE
						,apt.DUEDAYS
						,apt.DISCNTDAYS AS DiscountPeriod
						,apt.DISCNTPCNT * 100 AS DiscountRate
					FROM {(_hasSchema ? _schema + "." : "")}AP_PAY_TERMS apt
					";
				OracleDataReader reader = cmd.ExecuteReader();
				PaymentTerm record;
				while (reader.Read())
				{
					record = new PaymentTerm
					{
						PaymentTermCode = reader.IsDBNull(0) ? "" : reader.GetString(0),
						Description = reader.IsDBNull(1) ? "" : reader.GetString(1),
						LatePaymentFeeRate = reader.IsDBNull(2) ? "" : reader.GetString(2),
						DUEDATETYPE = reader.IsDBNull(3) ? "" : reader.GetString(3),
						DUEDAYS = reader.IsDBNull(4) ? 0 : reader.GetInt32(4),
						DiscountPeriod = reader.IsDBNull(5) ? "" : reader.GetString(5),
						DiscountRate = reader.IsDBNull(6) ? "" : reader.GetString(6)
					};
					payTerms.Add(record);
				}
				reader.Dispose();
			}
			catch(Exception ex)
			{
				_logger.LogError("An exception occured while attempting to get Payment Terms from Famous: {Message}.", ex.Message);
			}

			return payTerms;

		}
	}
}
