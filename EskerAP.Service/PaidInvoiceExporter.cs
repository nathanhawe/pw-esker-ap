using CsvHelper;
using EskerAP.Data.Famous;
using EskerAP.Domain;
using EskerAP.Service.Interface;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace EskerAP.Service
{
	public class PaidInvoiceExporter : BaseExporter, Interface.IPaidInvoiceExporter
	{
		private readonly ILogger _logger;
		private readonly IApVoucherRepo _repo;
		private readonly IUnpaidInvoiceReader _unpaidInvoiceReader;

		public PaidInvoiceExporter(ILogger<PaidInvoiceExporter> logger, IApVoucherRepo repo, IUnpaidInvoiceReader unpaidInvoiceReader)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_repo = repo ?? throw new ArgumentNullException(nameof(repo));
			_unpaidInvoiceReader = unpaidInvoiceReader ?? throw new ArgumentNullException(nameof(unpaidInvoiceReader));
		}

		public void ExportPaidInvoices(string companyCode, string paidInvoiceFolderPath, string unpaidInvoiceFolderPath)
		{
			_logger.LogDebug("Invoking PaidInvoiceExporter.ExportPaidInvoices() to folder:'{paidInvoiceFolderPath}' using unpaid invoices in '{unpaidInvoiceFolderPath}' days past.", paidInvoiceFolderPath, unpaidInvoiceFolderPath);

			// Ensure the folder exists;
			base.EnsureFolderExists(paidInvoiceFolderPath);

			var filePath = base.GetFilePath(Domain.Constants.Erp.Famous, Domain.Constants.ExportType.PaidInvoices, paidInvoiceFolderPath);

			try
			{
				var paidInvoices = new List<PaidInvoice>();

				// Get the list of unpaid invoices to query
				var unpaidInvoices = _unpaidInvoiceReader.GetUnpaidInvoices(unpaidInvoiceFolderPath, unpaidInvoiceFolderPath);

				// Query the Paid Invoices
				foreach(var unpaid in unpaidInvoices)
				{
					var temp = _repo.GetPaidInvoice(unpaid.VendorNumber, unpaid.InvoiceNumber);
					if (temp != null) paidInvoices.Add(temp);
				}

				if (paidInvoices.Count() > 0)
				{

					// Set the company code for all Paid Invoices.
					paidInvoices.ForEach(x => x.CompanyCode = companyCode);

					// Convert to CSV document
					using var writer = new StreamWriter(filePath);
					using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
					csv.Context.TypeConverterCache.AddConverter<DateTime>(new Infrastructure.TypeConverter.EskerDateConverter());
					csv.Context.TypeConverterCache.AddConverter<bool>(new Infrastructure.TypeConverter.EskerBooleanConverter());
					csv.Context.RegisterClassMap<Infrastructure.Maps.PaidInvoiceMap>();

					// Write document to disk
					csv.WriteRecords(paidInvoices);

					_logger.LogDebug("'{Count}' paid invoices were written to '{filePath}'.", paidInvoices.Count(), filePath);
				}
				else
				{
					_logger.LogDebug("No paid invoices found.  Skipping file creation.");
				}
			}
			catch (Exception ex)
			{
				_logger.LogError("An exception was thrown while attempting to export Paid Invoices: {Message}", ex.Message);
			}

		}
	}
}
