﻿using CsvHelper;
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
		private readonly string _folderPath;

		public PhaseExporter(ILogger<PhaseExporter> logger, ICaPhaseRepo repo, string folderPath)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_repo = repo ?? throw new ArgumentNullException(nameof(repo));
			_folderPath = folderPath ?? throw new ArgumentNullException(nameof(folderPath));

			// Ensure the folder exists;
			base.EnsureFolderExists(_folderPath);
		}

		public void ExportPhases(string companyCode)
		{
			_logger.LogDebug("Invoking PhaseExporter.ExportPhases() to folder:'{FolderPath}'", _folderPath);
			var filePath = base.GetFilePath(Domain.Constants.Erp.Famous, Domain.Constants.ExportType.Phases, _folderPath);

			try
			{

				// Query the Phases
				var phases = _repo.GetPhases().ToList();

				// Set the company code for all Phases.
				phases.ForEach(x => x.CompanyCode = companyCode);

				// Convert to CSV document
				using var writer = new StreamWriter(filePath);
				using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
				//csv.Context.TypeConverterCache.AddConverter<bool>(new Infrastructure.TypeConverter.EskerBooleanConverter());
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
