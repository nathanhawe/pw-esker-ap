using EskerAP.Domain;
using Microsoft.Extensions.Logging;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;

namespace EskerAP.Data.Famous
{
	public class GLAccountRepo : IGLAccountRepo
	{
		private readonly string _connectionString;
		private readonly string _schema;
		private readonly bool _hasSchema;
		private readonly ILogger<GLAccountRepo> _logger;

		public GLAccountRepo(string connectionString, string schema, ILogger<GLAccountRepo> logger)
		{
			_connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
			_schema = schema?.Trim();
			_hasSchema = !String.IsNullOrWhiteSpace(schema);
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public IEnumerable<Domain.GLAccount> GetGLAccounts()
		{
			_logger.LogDebug("GetGLAccounts() Invoked");

			var accounts = new List<GLAccount>();
			using OracleConnection con = new OracleConnection(_connectionString);
			using OracleCommand cmd = con.CreateCommand();
			try
			{
				con.Open();
				cmd.BindByName = true;
				cmd.CommandText = GetQuery();
				OracleDataReader reader = cmd.ExecuteReader();

				GLAccount record;
				while (reader.Read())
				{
					record = new GLAccount
					{
						Account = reader.IsDBNull(0) ? "" : reader.GetString(0),
						Description = reader.IsDBNull(1) ? "" : reader.GetString(1),
						Group = reader.IsDBNull(2) ? "" : reader.GetString(2),
					};
					accounts.Add(record);
				}
				reader.Dispose();
			}
			catch(Exception ex)
			{
				_logger.LogError("An exception occured while attempting to get Gl Accounts from Famous: {Message}.", ex.Message);
			}

			return accounts;

		}

		private string GetQuery()
		{
			return $@"SELECT 
				  GL_ACCOUNT.ID 
				, GL_ACCOUNT.NAME 
				, G1.NAME AS GROUP1
				, G2.NAME AS Group2
				, G3.NAME AS GROUP3
				, G4.NAME AS GROUP4
			FROM {(_hasSchema ? _schema + "." : "")}GL_ACCOUNT
				LEFT JOIN {(_hasSchema ? _schema + "." : "")}GL_ACCOUNT_GROUP G1 ON {(_hasSchema ? _schema + "." : "")}GL_ACCOUNT.ACCTGROUPONEIDX = G1.ACCTGROUPIDX  
				LEFT JOIN {(_hasSchema ? _schema + "." : "")}GL_ACCOUNT_GROUP G2 ON {(_hasSchema ? _schema + "." : "")}GL_ACCOUNT.ACCTGROUPTWOIDX = G2.ACCTGROUPIDX 
 				LEFT JOIN {(_hasSchema ? _schema + "." : "")}GL_ACCOUNT_GROUP G3 ON {(_hasSchema ? _schema + "." : "")}GL_ACCOUNT.ACCTGROUPTHREEIDX  = G3.ACCTGROUPIDX 
 				LEFT JOIN {(_hasSchema ? _schema + "." : "")}GL_ACCOUNT_GROUP G4 ON {(_hasSchema ? _schema + "." : "")}GL_ACCOUNT.ACCTGROUPFOURIDX = G4.ACCTGROUPIDX 
			 WHERE 
 				(GL_ACCOUNT.CCREQDFLAG = 'N' ) 
 				AND ( GL_ACCOUNT.INACTIVEFLAG = 'N' ) 
 				AND ( GL_ACCOUNT.BUDGETONLYFLAG = 'N' )
			 ORDER BY GL_ACCOUNT.NAME";
		}
	}
}
