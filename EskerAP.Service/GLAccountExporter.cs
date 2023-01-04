using CsvHelper;
using EskerAP.Data.Famous;
using Microsoft.Extensions.Logging;
using System;
using System.Globalization;
using System.IO;
using System.Linq;

namespace EskerAP.Service
{
	public class GLAccountExporter : BaseExporter, Interface.IGLAccountExporter
	{
		private readonly ILogger _logger;
		private readonly IGLAccountRepo _repo;

		public GLAccountExporter(ILogger<GLAccountExporter> logger, IGLAccountRepo repo)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_repo = repo ?? throw new ArgumentNullException(nameof(repo));
		}

		public void ExportGLAccounts(string companyCode, string folderPath)
		{
			_logger.LogDebug("Invoking GLAccountExporter.ExportGLAccounts() to folder:'{folderPath}'", folderPath);
			
			// Ensure the folder exists;
			base.EnsureFolderExists(folderPath);

			var filePath = base.GetFilePath(Domain.Constants.Erp.Famous, Domain.Constants.ExportType.GLaccount, folderPath);

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
