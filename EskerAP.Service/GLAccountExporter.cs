using CsvHelper;
using EskerAP.Data.Famous;
using Microsoft.Extensions.Logging;
using System;
using System.Globalization;
using System.IO;
using System.Linq;

namespace EskerAP.Service
{
	public class GLAccountExporter : Exporter, Interface.IGLAccountExporter
	{
		private readonly ILogger _logger;
		private readonly IGLAccountRepo _repo;
		private readonly string _folderPath;

		public GLAccountExporter(ILogger<GLAccountExporter> logger, IGLAccountRepo repo, string folderPath)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_repo = repo ?? throw new ArgumentNullException(nameof(repo));
			_folderPath = folderPath ?? throw new ArgumentNullException(nameof(folderPath));

			// Ensure the folder exists;
			base.EnsureFolderExists(_folderPath);
		}

		public void ExportGLAccounts(string companyCode)
		{
			_logger.LogDebug("Invoking GLAccountExporter.ExportGLAccounts() to folder:'{FolderPath}'", _folderPath);
			var filePath = $"{_folderPath}\\FAM__GLaccount__{DateTime.Now:yyyyMMddHHmmss}.csv";

			try
			{

				// Query the GL Accounts
				var accounts = _repo.GetGLAccounts().ToList();

				// Set the company code for all GLAccounts.
				accounts.ForEach(x => x.CompanyCode = companyCode);

				// Convert to CSV document
				using var writer = new StreamWriter(filePath);
				using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
				csv.Context.TypeConverterCache.AddConverter<bool>(new Infrastructure.TypeConverter.EskerBooleanConverter());
				csv.Context.RegisterClassMap<Infrastructure.Maps.GLAccountMap>();

				// Write document to disk
				csv.WriteRecords(accounts);
			}
			catch (Exception ex)
			{
				_logger.LogError("An exception was thrown while attempting to export GLAccounts: {Message}", ex.Message);
			}

		}
	}
}
