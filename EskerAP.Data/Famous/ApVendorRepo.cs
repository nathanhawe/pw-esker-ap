using EskerAP.Domain;
using Microsoft.Extensions.Logging;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;

namespace EskerAP.Data.Famous
{
	public class ApVendorRepo : IApVendorRepo
	{
		private readonly string _connectionString;
		private readonly string _schema;
		private readonly bool _hasSchema;
		private readonly ILogger<ApVendorRepo> _logger;

		public ApVendorRepo(string connectionString, string schema, ILogger<ApVendorRepo> logger)
		{
			_connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
			_schema = schema?.Trim();
			_hasSchema = !String.IsNullOrWhiteSpace(schema);
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public IEnumerable<Domain.Vendor> GetVendors()
		{
			_logger.LogDebug("GetVendors() Invoked");

			var accounts = new List<Vendor>();
			using OracleConnection con = new OracleConnection(_connectionString);
			using OracleCommand cmd = con.CreateCommand();
			try
			{
				con.Open();
				cmd.BindByName = true;
				cmd.CommandText = GetQuery();
				OracleDataReader reader = cmd.ExecuteReader();

				Vendor record;
				while (reader.Read())
				{
					record = new Vendor
					{
						Id = reader.IsDBNull(0) ? "" : reader.GetString(0),
						Name = reader.IsDBNull(1) ? "" : reader.GetString(1),
						PhoneNumber = reader.IsDBNull(2) ? "" : reader.GetString(2),
						FaxNumber = reader.IsDBNull(3) ? "" : reader.GetString(3),
						DUNSNumber = reader.IsDBNull(4) ? "" : reader.GetString(4),
						PaymentTermsCode = reader.IsDBNull(5) ? "" : reader.GetString(5),
						Email = reader.IsDBNull(6) ? "" : reader.GetString(6),
						Street = reader.IsDBNull(7) ? "" : reader.GetString(7),
						City = reader.IsDBNull(8) ? "" : reader.GetString(8),
						PostalCode = reader.IsDBNull(9) ? "" : reader.GetString(9),
						Region = reader.IsDBNull(10) ? "" : reader.GetString(10),
						Country = reader.IsDBNull(11) ? "" : reader.GetString(11),
					};
					accounts.Add(record);
				}
				reader.Dispose();
			}
			catch(Exception ex)
			{
				_logger.LogError("An exception occured while attempting to get Vendors from Famous: {Message}.", ex.Message);
			}

			return accounts;

		}

		private string GetQuery()
		{
			return $@"SELECT
				  Name.ID AS VendorId
				, Name.LASTCONAME AS Name
				,( SELECT Voice FROM 
					(SELECT
						TO_NCHAR(AREACODE) || TRIM(BOTH FROM COMMSTRING) AS Voice 
					FROM {(_hasSchema ? _schema + "." : "")}FC_NAME_COMM fnc
					WHERE 
						fnc.NAMEIDX = Vendor.VENDNAMEIDX 
						AND fnc.INACTIVEFLAG = 'N'
						AND fnc.COMMTYPE = 2 -- 2 = Voice, 3 = Fax (no area code field), 4 = Email
					ORDER BY 
						  fnc.NAMECOMMSEQ ASC
						, fnc.ORDERBY) 
					WHERE ROWNUM = 1
				) AS PhoneNumber
				,( SELECT COMMSTRING FROM 
					(SELECT
						COMMSTRING 
					FROM {(_hasSchema ? _schema + "." : "")}FC_NAME_COMM fnc
					WHERE 
						fnc.NAMEIDX = Vendor.VENDNAMEIDX 
						AND fnc.INACTIVEFLAG = 'N'
						AND fnc.COMMTYPE = 3 -- 2 = Voice, 3 = Fax (no area code field), 4 = Email
					ORDER BY 
						  fnc.NAMECOMMSEQ ASC
						, fnc.ORDERBY) 
					WHERE ROWNUM = 1
				) AS FaxNumber
				-- VATNumber
				, NameLocation.DUNSNO AS DUNSNumber
				-- PreferredInvoiceType
				, PayTerms.DESCR AS PaymentTermCode
				,( SELECT COMMSTRING FROM 
					(SELECT
						COMMSTRING 
					FROM {(_hasSchema ? _schema + "." : "")}FC_NAME_COMM fnc
					WHERE 
						fnc.NAMEIDX = Vendor.VENDNAMEIDX 
						AND fnc.INACTIVEFLAG = 'N'
						AND fnc.COMMTYPE = 4 -- 2 = Voice, 3 = Fax (no area code field), 4 = Email
					ORDER BY 
						  fnc.NAMECOMMSEQ ASC
						, fnc.ORDERBY) 
					WHERE ROWNUM = 1
				) AS Email
				-- General Account
				-- TaxSystem
				-- Currency
				-- Parafiscal Tax
				-- Supplier Due
				-- Sub
				, NameLocation.ADDRESS AS Street
				-- Post Office Box
				, NameLocation.CITY 
				, NameLocation.ZIP  AS PostalCode
				, NameLocation.STATE  AS Region
				, NameLocation.COUNTRY
			FROM {(_hasSchema ? _schema + "." : "")}AP_VENDOR Vendor
				LEFT JOIN {(_hasSchema ? _schema + "." : "")}FC_NAME Name ON 
					Vendor.VENDNAMEIDX = Name.NAMEIDX
				LEFT JOIN {(_hasSchema ? _schema + "." : "")}FC_NAME_ROLE fnr ON
					Name.NAMEIDX = fnr.NAMEIDX 
					AND fnr.NAMETYPE = '3'
				LEFT JOIN {(_hasSchema ? _schema + "." : "")}FC_NAME_LOCATION NameLocation ON
					Name.NAMEIDX = NameLocation.NAMEIDX 
					AND NameLocation.INACTIVEFLAG = 'N'
					AND NameLocation.NAMELOCATIONSEQ = 1
				LEFT JOIN {(_hasSchema ? _schema + "." : "")}AP_PAY_TERMS PayTerms ON
					Vendor.APPAYTERMSIDX = PayTerms.APPAYTERMSIDX
			WHERE fnr.INACTIVEFLAG = 'N'
			ORDER BY 
				Name.ID";
		}
	}
}
