using CsvHelper;
using EskerAP.Data.Famous;
using Microsoft.Extensions.Logging;
using System;
using System.Globalization;
using System.IO;
using System.Linq;

namespace EskerAP.Service
{
	public class PhaseExporter : BaseExporter, Interface.IPhaseExporter
	{
		private readonly ILogger _logger;
		private readonly ICaPhaseRepo _repo;

		public PhaseExporter(ILogger<PhaseExporter> logger, ICaPhaseRepo repo)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_repo = repo ?? throw new ArgumentNullException(nameof(repo));
		}

		public void ExportPhases(string companyCode, string folderPath)
		{
			_logger.LogDebug("Invoking PhaseExporter.ExportPhases() to folder:'{folderPath}'", folderPath);

			// Ensure the folder exists;
			base.EnsureFolderExists(folderPath);

			var filePath = base.GetFilePath(Domain.Constants.Erp.Famous, Domain.Constants.ExportType.Phases, folderPath);

			try
			{

				// Query the Phases
				var phases = _repo.GetPhases().ToList();

				// Set the company code for all Phases.
				phases.ForEach(x => x.CompanyCode = companyCode);

				// Convert to CSV document
				using var writer = new StreamWriter(filePath);
				using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
				csv.Context.RegisterClassMap<Infrastructure.Maps.PhaseMap>();

				// Write document to disk
				csv.WriteRecords(phases);
			}
			catch (Exception ex)
			{
				_logger.LogError("An exception was thrown while attempting to export Phases: {Message}", ex.Message);
			}

		}
	}
}
