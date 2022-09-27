using EskerAP.Domain;
using Microsoft.Extensions.Logging;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;

namespace EskerAP.Data.Famous
{
	public class PurchaseOrderDetailRepo : IPurchaseOrderDetailRepo
	{
		private readonly string _connectionString;
		private readonly string _schema;
		private readonly bool _hasSchema;
		private readonly ILogger<PurchaseOrderDetailRepo> _logger;

		public PurchaseOrderDetailRepo(string connectionString, string schema, ILogger<PurchaseOrderDetailRepo> logger)
		{
			_connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
			_schema = schema?.Trim();
			_hasSchema = !String.IsNullOrWhiteSpace(schema);
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public IEnumerable<Domain.Item> GetPurchaseOrderDetails()
		{
			_logger.LogDebug("GetItems() Invoked");

			var accounts = new List<Item>();
			using OracleConnection con = new OracleConnection(_connectionString);
			using OracleCommand cmd = con.CreateCommand();
			try
			{
				con.Open();
				cmd.BindByName = true;
				cmd.CommandText = GetQuery();
				OracleDataReader reader = cmd.ExecuteReader();

				Item record;
				while (reader.Read())
				{
					record = new Item
					{
						VendorNumber = reader.IsDBNull(0) ? "" : reader.GetString(0),
						OrderNumber = reader.IsDBNull(1) ? "" : reader.GetString(1),
						OrderDate = reader.IsDBNull(2) ? DateTime.Now : reader.GetDateTime(2),
						ItemNumber = reader.IsDBNull(3) ? "" : reader.GetString(3),
						Description = reader.IsDBNull(4) ? "" : reader.GetString(4),
						CostCenter = reader.IsDBNull(5) ? "" : reader.GetString(5),
						UnitPrice = reader.IsDBNull(6) ? 0 : reader.GetDecimal(6),
						OrderedAmount = reader.IsDBNull(7) ? 0 : reader.GetDecimal(7),
						UnitOfMeasureCode = reader.IsDBNull(8) ? "" : reader.GetString(8),
						OrderedQuantity = reader.IsDBNull(9) ? 0 : reader.GetDecimal(9),
						DeliveredQuantity = reader.IsDBNull(11) ? 0 : reader.GetDecimal(11),
						TRXTYPE = reader.IsDBNull(12) ? "" : reader.GetString(12)
					};
					accounts.Add(record);
				}
				reader.Dispose();
			}
			catch(Exception ex)
			{
				_logger.LogError("An exception occured while attempting to get Purchase Order Details from Famous: {Message}.", ex.Message);
			}

			return accounts;

		}

		private string GetQuery()
		{
			return $@"(
					-- Detail: Products
					SELECT
						  Name.ID AS VendorNumber
						, Header.APPONO AS OrderNumber
						, Header.PODATETIME AS OrderDate
						, Line.LINENO AS ItemNumber
						-- There is no Part Number
						-- There is no Item Type
						, Line.RECVDESCR AS Description
						-- There is no GLAccount
						-- There is no Group
						, cacc.ID AS CostCenter
						, Product.PRICE AS UnitPrice
						, Product.AMT AS OrderedAmount
						, UnitOfMeasure.UOM AS UnitOfMeasure
						, Product.QNT AS OrderedQuantity
						, Product.RECVAMT AS DeliveredAmount
						, Product.RECVQNT AS DeliveredQuantity
						, Detail.TRXTYPE -- 1 = Product, 2 = Charge, 3 = Deleted?
					FROM {(_hasSchema ? _schema + "." : "")}AP_PO_DETAIL Detail
						-- PO HEADER
						LEFT JOIN {(_hasSchema ? _schema + "." : "")}AP_PO_HEADER Header ON
							Detail.APPOHDRIDX = Header.APPOHDRIDX
						-- PO LINE
						LEFT JOIN {(_hasSchema ? _schema + "." : "")}AP_PO_LINE Line ON
							Detail.APPOHDRIDX = Line.APPOHDRIDX 
							AND Detail.APPOLINETRXTYPE = Line.APPOLINETRXTYPE 
							AND Detail.APPOLINESEQ = Line.APPOLINESEQ 
						-- Product
						LEFT JOIN {(_hasSchema ? _schema + "." : "")}AP_PO_PRODUCT Product ON 
							Detail.APPOHDRIDX = Product.APPOHDRIDX
							AND Detail.APPODTLSEQ = Product.APPODTLSEQ
						LEFT JOIN {(_hasSchema ? _schema + "." : "")}IC_PRODUCT_ID ProductId ON
							Product.PRODUCTIDX = ProductId.PRODUCTIDX
						LEFT JOIN {(_hasSchema ? _schema + "." : "")}CA_COST_CENTER cacc ON
							Product.COSTCENTERIDX = cacc.COSTCENTERIDX 
						LEFT JOIN {(_hasSchema ? _schema + "." : "")}FC_UNIT_OF_MEASURE UnitOfMeasure ON
							Product.UOMIDX = UnitOfMeasure.UOMIDX 
						-- Vendor Name
						LEFT JOIN {(_hasSchema ? _schema + "." : "")}FC_NAME Name ON
							Header.VENDNAMEIDX = Name.NAMEIDX 
					WHERE 
						Detail.GLDELETECODE = 'N'
						AND Detail.TRXTYPE = 1 -- 1 = Product, 2 = Charge, 3 = Deleted?
						AND Header.POSTATUS IN ('1', '2', '4') -- 1 = Partial, 2 = Ordered, 3 = Deleted, 4 = Received, 5 = Completed
					UNION
					-- Detail: Charges
					SELECT
						  Name.ID AS VendorNumber
						, Header.APPONO AS OrderNumber
						, Header.PODATETIME AS OrderDate
						, 0 AS ItemNumber-- There is no good Line number
						-- There is no Part Number
						-- There is no Item Type
						, fccc.DESCR AS Description
						-- There is no GLAccount
						-- There is no Group
						, cacc2.ID AS CostCenter
						, 0 AS UnitPrice -- Will need to be back filled in program
						, Charge.AMT AS OrderedAmount
						, '' AS UnitOfMeasure-- There is no unit of measure
						, Charge.QNT AS OrderedQuantity
						, Charge.RECVAMT  AS DeliveredAmount
						, Charge.RECVQNT AS DeliveredQuantity
						, Detail.TRXTYPE -- 1 = Product, 2 = Charge, 3 = Deleted?
					FROM {(_hasSchema ? _schema + "." : "")}AP_PO_DETAIL Detail
						-- PO HEADER
						LEFT JOIN {(_hasSchema ? _schema + "." : "")}AP_PO_HEADER Header ON
							Detail.APPOHDRIDX = Header.APPOHDRIDX
						-- PO LINE
						LEFT JOIN {(_hasSchema ? _schema + "." : "")}AP_PO_LINE Line ON
							Detail.APPOHDRIDX = Line.APPOHDRIDX 
							AND Detail.APPOLINETRXTYPE = Line.APPOLINETRXTYPE 
							AND Detail.APPOLINESEQ = Line.APPOLINESEQ 
						-- Charge
						LEFT JOIN {(_hasSchema ? _schema + "." : "")}AP_PO_CHARGE Charge ON
							Detail.APPOHDRIDX = Charge.APPOHDRIDX 
							AND Detail.APPODTLSEQ = Charge.APPODTLSEQ
						LEFT JOIN {(_hasSchema ? _schema + "." : "")}FC_CHARGE_CODE fccc ON
							Charge.FCCHARGEIDX = fccc.FCCHARGEIDX
						LEFT JOIN {(_hasSchema ? _schema + "." : "")}CA_COST_CENTER cacc2 ON
							fccc.COSTCENTERIDX = cacc2.COSTCENTERIDX
						-- Vendor Name
						LEFT JOIN {(_hasSchema ? _schema + "." : "")}FC_NAME Name ON
							Header.VENDNAMEIDX = Name.NAMEIDX 
					WHERE 
						Detail.GLDELETECODE = 'N'
						AND Detail.TRXTYPE = 2 -- 1 = Product, 2 = Charge, 3 = Deleted?
						AND Charge.CHARGEMETHODTYPE IN ('1', '2') -- 1 = Normal, 2 = Delivered, 3 = Memo
						AND Header.POSTATUS IN ('1', '2', '4') -- 1 = Partial, 2 = Ordered, 3 = Deleted, 4 = Received, 5 = Completed
				)
				ORDER BY
					  OrderNumber ASC
					, TRXTYPE ASC
					, ItemNumber ASC";
		}
	}
}
