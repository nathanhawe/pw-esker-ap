using EskerAP.Domain;
using Microsoft.Extensions.Logging;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace EskerAP.Data.Famous
{
	public class CaCostCenterRepo : ICaCostCenterRepo
	{
		private readonly string _connectionString;
		private readonly string _schema;
		private readonly bool _hasSchema;
		private readonly ILogger<CaCostCenterRepo> _logger;

		public CaCostCenterRepo(string connectionString, string schema, ILogger<CaCostCenterRepo> logger)
		{
			_connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
			_schema = schema?.Trim();
			_hasSchema = !String.IsNullOrWhiteSpace(schema);
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public IEnumerable<Domain.CostCenter> GetCostCenters()
		{
			_logger.LogDebug("GetCostCenters() Invoked");

			var costCenters = new List<CostCenter>();
			using OracleConnection con = new OracleConnection(_connectionString);
			using OracleCommand cmd = con.CreateCommand();
			try
			{
				con.Open();
				cmd.BindByName = true;
				cmd.CommandText = $"SELECT COSTCENTERIDX,ID,NAME FROM {(_hasSchema ? _schema + "." : "")}CA_COST_CENTER WHERE INACTIVEFLAG = 'N'";
				OracleDataReader reader = cmd.ExecuteReader();
				CostCenter record;
				while (reader.Read())
				{
					record = new CostCenter
					{
						Idx = reader.IsDBNull(0) ? "" : reader.GetString(0),
						Id = reader.IsDBNull(1) ? "" : reader.GetString(1),
						Description = reader.IsDBNull(2) ? "" : reader.GetString(2),
					};
					costCenters.Add(record);
				}
				reader.Dispose();
			}
			catch(Exception ex)
			{
				_logger.LogError("An exception occured while attempting to get Cost Centers from Famous: {Message}.", ex.Message);
			}

			return costCenters;

		}
	}
}
