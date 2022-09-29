using CsvHelper;
using EskerAP.Data.Famous;
using Microsoft.Extensions.Logging;
using System;
using System.Globalization;
using System.IO;
using System.Linq;

namespace EskerAP.Service
{
	public class CostCenterExporter : Exporter, Interface.ICostCenterExporter
	{
		private readonly ILogger _logger;
		private readonly ICaCostCenterRepo _caCostCenterRepo;
		private readonly string _folderPath;

		public CostCenterExporter(ILogger<CostCenterExporter> logger, ICaCostCenterRepo caCostCenterRepo, string folderPath)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_caCostCenterRepo = caCostCenterRepo ?? throw new ArgumentNullException(nameof(caCostCenterRepo));
			_folderPath = folderPath ?? throw new ArgumentNullException(nameof(folderPath));

			// Ensure the folder exists;
			base.EnsureFolderExists(_folderPath);
		}

		public void ExportCostCenters(string companyCode)
		{
			_logger.LogDebug("Invoking CostCenterExporter.ExportCostCenters() to folder:'{FolderPath}'", _folderPath);
			var filePath = base.GetFilePath(Domain.Constants.Erp.Famous, Domain.Constants.ExportType.Costcenters, _folderPath);

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
