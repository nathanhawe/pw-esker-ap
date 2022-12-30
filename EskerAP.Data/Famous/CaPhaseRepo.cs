using EskerAP.Domain;
using Microsoft.Extensions.Logging;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;

namespace EskerAP.Data.Famous
{
	public class CaPhaseRepo : ICaPhaseRepo
	{
		private readonly string _connectionString;
		private readonly string _schema;
		private readonly bool _hasSchema;
		private readonly ILogger<CaPhaseRepo> _logger;

		public CaPhaseRepo(string connectionString, string schema, ILogger<CaPhaseRepo> logger)
		{
			_connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
			_schema = schema?.Trim();
			_hasSchema = !String.IsNullOrWhiteSpace(schema);
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public IEnumerable<Domain.Phase> GetPhases()
		{
			_logger.LogDebug("GetPhases() Invoked");

			var phases = new List<Phase>();
			using OracleConnection con = new OracleConnection(_connectionString);
			using OracleCommand cmd = con.CreateCommand();
			try
			{
				con.Open();
				cmd.BindByName = true;
				cmd.CommandText = GetQuery();
				OracleDataReader reader = cmd.ExecuteReader();

				Phase record;
				while (reader.Read())
				{
					record = new Phase
					{
						Id = reader.IsDBNull(0) ? "" : reader.GetString(0),
						Description = reader.IsDBNull(1) ? "" : reader.GetString(1),
					};
					phases.Add(record);
				}
				reader.Dispose();
			}
			catch(Exception ex)
			{
				_logger.LogError("An exception occured while attempting to get Phases from Famous: {Message}.", ex.Message);
			}

			return phases;

		}

		private string GetQuery()
		{
			return $@"SELECT
				  CA_PHASE.ID
				, CA_PHASE.NAME
			FROM {(_hasSchema ? _schema + "." : "")}CA_PHASE
			WHERE
				CA_PHASE.INACTIVEFLAG = 'N'
			ORDER BY CA_PHASE.ID";
		}
	}
}
