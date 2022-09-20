using EskerAP.Domain;
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
		private string _connectionString;
		private string _schema;
		private bool _hasSchema;

		public CaCostCenterRepo(string connectionString, string schema)
		{
			_connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
			_schema = schema?.Trim();
			_hasSchema = !String.IsNullOrWhiteSpace(schema);
		}

		public IEnumerable<Domain.CostCenter> GetCostCenters()
		{
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
				Console.WriteLine(ex.Message);
			}

			return costCenters;

		}
	}
}
