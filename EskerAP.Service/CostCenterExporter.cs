using CsvHelper;
using EskerAP.Data.Famous;
using Microsoft.Extensions.Logging;
using System;
using System.Globalization;
using System.IO;
using System.Linq;

namespace EskerAP.Service
{
	public class CostCenterExporter : BaseExporter, Interface.ICostCenterExporter
	{
		private readonly ILogger _logger;
		private readonly ICaCostCenterRepo _caCostCenterRepo;

		public CostCenterExporter(ILogger<CostCenterExporter> logger, ICaCostCenterRepo caCostCenterRepo)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_caCostCenterRepo = caCostCenterRepo ?? throw new ArgumentNullException(nameof(caCostCenterRepo));
		}

		public void ExportCostCenters(string companyCode, string folderPath)
		{
			_logger.LogDebug("Invoking CostCenterExporter.ExportCostCenters() to folder:'{folderPath}'", folderPath);

			// Ensure the folder exists;
			base.EnsureFolderExists(folderPath);

			var filePath = base.GetFilePath(Domain.Constants.Erp.Famous, Domain.Constants.ExportType.Costcenters, folderPath);

			try
			{

				// Query the cost centers
				var costCenters = _caCostCenterRepo.GetCostCenters().ToList();

				// Set the company code for all cost centers.
				costCenters.ForEach(x => x.CompanyCode = companyCode);

				// Convert to CSV document
				using var writer = new StreamWriter(filePath);
				using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
				csv.Context.RegisterClassMap<Infrastructure.Maps.CostCenterMap>();

				// Write document to disk
				csv.WriteRecords(costCenters);
			}
			catch (Exception ex)
			{
				_logger.LogError("An exception was thrown while attempting to export cost centers: {Message}", ex.Message);
			}

		}
	}
}
