using CsvHelper;
using EskerAP.Data.Famous;
using Microsoft.Extensions.Logging;
using System;
using System.Globalization;
using System.IO;
using System.Linq;

namespace EskerAP.Service
{
	public class PaymentTermsExporter : BaseExporter, Interface.IPaymentTermsExporter
	{
		private readonly ILogger _logger;
		private readonly IApPayTermsRepo _apPayTermsRepo;

		public PaymentTermsExporter(ILogger<PaymentTermsExporter> logger, IApPayTermsRepo caPaymentTermsRepo)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_apPayTermsRepo = caPaymentTermsRepo ?? throw new ArgumentNullException(nameof(caPaymentTermsRepo));
		}

		public void ExportPaymentTerms(string companyCode, string folderPath)
		{
			_logger.LogDebug("Invoking PaymentTermsExporter.ExportPaymentTerms() to folder:'{folderPath}'", folderPath);

			// Ensure the folder exists;
			base.EnsureFolderExists(folderPath);

			var filePath = base.GetFilePath(Domain.Constants.Erp.Famous, Domain.Constants.ExportType.Paymentterms, folderPath);

			try
			{

				// Query the cost centers
				var PaymentTerms = _apPayTermsRepo.GetPayTerms().ToList();

				// Set the company code for all cost centers.
				PaymentTerms.ForEach(x => x.CompanyCode = companyCode);

				// Convert to CSV document
				using var writer = new StreamWriter(filePath);
				using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
				csv.Context.RegisterClassMap<Infrastructure.Maps.PaymentTermsMap>();

				// Write document to disk
				csv.WriteRecords(PaymentTerms);
			}
			catch (Exception ex)
			{
				_logger.LogError("An exception was thrown while attempting to export payment terms: {Message}", ex.Message);
			}

		}
	}
}
