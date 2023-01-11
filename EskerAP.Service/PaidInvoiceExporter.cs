using CsvHelper;
using EskerAP.Data.Famous;
using Microsoft.Extensions.Logging;
using System;
using System.Globalization;
using System.IO;
using System.Linq;

namespace EskerAP.Service
{
	public class PaidInvoiceExporter : BaseExporter, Interface.IPaidInvoiceExporter
	{
		private readonly ILogger _logger;
		private readonly IApVoucherRepo _repo;

		public PaidInvoiceExporter(ILogger<PaidInvoiceExporter> logger, IApVoucherRepo repo)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_repo = repo ?? throw new ArgumentNullException(nameof(repo));
		}

		public void ExportPaidInvoices(string companyCode, string folderPath, int daysPast)
		{
			_logger.LogDebug("Invoking PaidInvoiceExporter.ExportPaidInvoices() to folder:'{folderPath}' for '{daysPast}' days past.", folderPath, daysPast);

			// Ensure the folder exists;
			base.EnsureFolderExists(folderPath);

			var filePath = base.GetFilePath(Domain.Constants.Erp.Famous, Domain.Constants.ExportType.PaidInvoices, folderPath);

			try
			{

				// Query the Paid Invoices
				var paidInvoices = _repo.GetPaidInvoices(daysPast).ToList();

				// Set the company code for all Paid Invoices.
				paidInvoices.ForEach(x => x.CompanyCode = companyCode);

				// Convert to CSV document
				using var writer = new StreamWriter(filePath);
				using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
				csv.Context.RegisterClassMap<Infrastructure.Maps.PaidInvoiceMap>();

				// Write document to disk
				csv.WriteRecords(paidInvoices);
			}
			catch (Exception ex)
			{
				_logger.LogError("An exception was thrown while attempting to export Paid Invoices: {Message}", ex.Message);
			}

		}
	}
}
