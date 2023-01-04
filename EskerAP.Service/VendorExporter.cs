using CsvHelper;
using EskerAP.Data.Famous;
using Microsoft.Extensions.Logging;
using System;
using System.Globalization;
using System.IO;
using System.Linq;

namespace EskerAP.Service
{
	public class VendorExporter : BaseExporter, Interface.IVendorExporter
	{
		private readonly ILogger _logger;
		private readonly IApVendorRepo _repo;

		public VendorExporter(ILogger<VendorExporter> logger, IApVendorRepo repo)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_repo = repo ?? throw new ArgumentNullException(nameof(repo));
		}

		public void ExportVendors(string companyCode, string folderPath)
		{
			_logger.LogDebug("Invoking VendorExporter.ExportVendors() to folder:'{folderPath}'", folderPath);

			// Ensure the folder exists;
			base.EnsureFolderExists(folderPath);

			var filePath = base.GetFilePath(Domain.Constants.Erp.Famous, Domain.Constants.ExportType.Vendors, folderPath);

			try
			{

				// Query the vendors
				var vendors = _repo.GetVendors().ToList();

				// Set the company code for all vendors.
				vendors.ForEach(x => x.CompanyCode = companyCode);

				// Convert to CSV document
				using var writer = new StreamWriter(filePath);
				using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
				csv.Context.TypeConverterCache.AddConverter<bool>(new Infrastructure.TypeConverter.EskerBooleanConverter());
				csv.Context.RegisterClassMap<Infrastructure.Maps.VendorMap>();

				// Write document to disk
				csv.WriteRecords(vendors);
			}
			catch (Exception ex)
			{
				_logger.LogError("An exception was thrown while attempting to export vendors: {Message}", ex.Message);
			}

		}
	}
}
