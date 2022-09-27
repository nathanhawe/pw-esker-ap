using EskerAP.Domain;
using Microsoft.Extensions.Logging;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;

namespace EskerAP.Data.Famous
{
	public class PurchaseOrderHeaderRepo : IPurchaseOrderHeaderRepo
	{
		private readonly string _connectionString;
		private readonly string _schema;
		private readonly bool _hasSchema;
		private readonly ILogger<PurchaseOrderHeaderRepo> _logger;

		public PurchaseOrderHeaderRepo(string connectionString, string schema, ILogger<PurchaseOrderHeaderRepo> logger)
		{
			_connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
			_schema = schema?.Trim();
			_hasSchema = !String.IsNullOrWhiteSpace(schema);
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public IEnumerable<Domain.Header> GetPurchaseOrderHeaders()
		{
			_logger.LogDebug("GetHeaders() Invoked");

			var accounts = new List<Header>();
			using OracleConnection con = new OracleConnection(_connectionString);
			using OracleCommand cmd = con.CreateCommand();
			try
			{
				con.Open();
				cmd.BindByName = true;
				cmd.CommandText = GetQuery();
				OracleDataReader reader = cmd.ExecuteReader();

				Header record;
				while (reader.Read())
				{
					record = new Header
					{
						OrderNumber = reader.IsDBNull(0) ? "" : reader.GetString(0),
						OrderDate = reader.IsDBNull(2) ? DateTime.Now : reader.GetDateTime(2),
						OrderedAmount = reader.IsDBNull(5) ? 0 : reader.GetDecimal(5),
						VendorNumber = reader.IsDBNull(6) ? "" : reader.GetString(6),
						DeliveredAmount = reader.IsDBNull(9) ? 0 : reader.GetDecimal(9),
					};
					accounts.Add(record);
				}
				reader.Dispose();
			}
			catch(Exception ex)
			{
				_logger.LogError("An exception occured while attempting to get Purchase Order Headers from Famous: {Message}.", ex.Message);
			}

			return accounts;

		}

		private string GetQuery()
		{
			return $@"SELECT 
				  AP_PO_HEADER.APPONO
				, AP_PO_HEADER.APPOHDRIDX 
				, AP_PO_HEADER.PODATETIME
				, AP_PO_HEADER.POSTATUS
				, AP_PO_HEADER.CURRENCYIDX
				, AP_PO_HEADER.POAMT
				, FC_NAME_A.ID
				, PO.GetTotalOrderAmt(AP_PO_HEADER.APPOHDRIDX) as ORDERAMT
				, PO.GetTotalOrderQnt(AP_PO_HEADER.APPOHDRIDX) as ORDERQNT
				, PO.GetTotalRecvAmt(AP_PO_HEADER.APPOHDRIDX) as RECVAMT
				, PO.GetTotalRecvQnt(AP_PO_HEADER.APPOHDRIDX) as RECVQNT
				, PO.Received(Ap_Po_Header.ApPoHdrIdx, NULL) as POReceived
			FROM {(_hasSchema ? _schema + "." : "")}AP_PO_HEADER,
				{(_hasSchema ? _schema + "." : "")}FC_NAME FC_NAME_A
			WHERE 
				( {(_hasSchema ? _schema + "." : "")}AP_PO_HEADER.VENDNAMEIDX = FC_NAME_A.NAMEIDX ) 
				AND ( {(_hasSchema ? _schema + "." : "")}AP_PO_HEADER.POSTATUS IN ('1','2','4'))";
		}
	}
}
