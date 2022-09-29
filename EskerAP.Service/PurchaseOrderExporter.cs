using CsvHelper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace EskerAP.Service
{
	public class PurchaseOrderExporter : Exporter, Interface.IPurchaseOrderExporter
	{
		private readonly ILogger _logger;
		private readonly Data.Famous.IPurchaseOrderHeaderRepo _faHeaderRepo;
		private readonly Data.Famous.IPurchaseOrderDetailRepo _faDetailRepo;
		private readonly Data.Quickbase.IPurchaseOrdersRepo _qbHeaderRepo;
		private readonly Data.Quickbase.IItemsRepo _qbDetailRepo;
		private readonly string _folderPath;

		public PurchaseOrderExporter(
			ILogger<PurchaseOrderExporter> logger,
			Data.Famous.IPurchaseOrderHeaderRepo famousPOHeaderRepo,
			Data.Famous.IPurchaseOrderDetailRepo famousPODetailRepo, 
			Data.Quickbase.IPurchaseOrdersRepo quickbasePOHeaderRepo,
			Data.Quickbase.IItemsRepo quickbasePODetailRepo,
			string folderPath)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_faHeaderRepo = famousPOHeaderRepo ?? throw new ArgumentNullException(nameof(famousPOHeaderRepo));
			_faDetailRepo = famousPODetailRepo ?? throw new ArgumentNullException(nameof(famousPODetailRepo));
			_qbHeaderRepo = quickbasePOHeaderRepo ?? throw new ArgumentException(nameof(quickbasePOHeaderRepo));
			_qbDetailRepo = quickbasePODetailRepo ?? throw new ArgumentNullException(nameof(quickbasePODetailRepo));
			_folderPath = folderPath ?? throw new ArgumentNullException(nameof(folderPath));

			// Ensure the folder exists;
			base.EnsureFolderExists(_folderPath);
		}

		/// <summary>
		/// The method generates four files: purchase order headers and purchase order details for both
		/// ERPs Famous and Quickbase.
		/// </summary>
		/// <param name="companyCode"></param>
		public void ExportPurchaseOrders(string companyCode)
		{
			var qbHeaders = ExportQuickbaseHeaders(companyCode);
			ExportQuickbaseDetails(companyCode, qbHeaders);
			ExportFamousHeaders(companyCode);
			ExportFamousDetails(companyCode);
		}

		private List<Domain.Header> ExportQuickbaseHeaders(string companyCode)
		{
			_logger.LogDebug("Invoking PurchaseOrderExporter.ExportQuickbaseHeaders() to folder:'{FolderPath}'", _folderPath);
			var filePath = base.GetFilePath(Domain.Constants.Erp.Quickbase, Domain.Constants.ExportType.PurchaseorderHeaders, _folderPath);
			var headers = new List<Domain.Header>();
			try
			{
				// Query the PO headers
				headers = _qbHeaderRepo.Get().ToList();

				// Set the company code for all headers.
				headers.ForEach(x => x.CompanyCode = companyCode);

				// Convert to CSV document
				using var writer = new StreamWriter(filePath);
				using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
				csv.Context.TypeConverterCache.AddConverter<bool>(new Infrastructure.TypeConverter.EskerBooleanConverter());
				csv.Context.TypeConverterCache.AddConverter<DateTime>(new Infrastructure.TypeConverter.EskerDateConverter());
				csv.Context.RegisterClassMap<Infrastructure.Maps.PurchaseOrderHeaderMap>();

				// Write document to disk
				csv.WriteRecords(headers);
			}
			catch (Exception ex)
			{
				_logger.LogError("An exception was thrown while attempting to export Quickbase Purchase Order Headers: {Message}", ex.Message);
			}

			// Return the headers so that the freight and tax can be written as line items.
			return headers;
		}

		private void ExportQuickbaseDetails(string companyCode, List<Domain.Header> headers)
		{
			_logger.LogDebug("Invoking PurchaseOrderExporter.ExportQuickbaseDetails() to folder:'{FolderPath}'", _folderPath);
			var filePath = base.GetFilePath(Domain.Constants.Erp.Quickbase, Domain.Constants.ExportType.PurchaseorderItems, _folderPath);

			try
			{
				// Query the details
				var details = _qbDetailRepo.Get().ToList();

				// TODO: Add freight and tax detail from QB headers

				// Set the company code for all details.
				details.ForEach(x => x.CompanyCode = companyCode);

				// Convert to CSV document
				using var writer = new StreamWriter(filePath);
				using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
				csv.Context.TypeConverterCache.AddConverter<bool>(new Infrastructure.TypeConverter.EskerBooleanConverter());
				csv.Context.TypeConverterCache.AddConverter<DateTime>(new Infrastructure.TypeConverter.EskerDateConverter());
				csv.Context.RegisterClassMap<Infrastructure.Maps.PurchaseOrderDetailMap>();

				// Write document to disk
				csv.WriteRecords(details);
			}
			catch (Exception ex)
			{
				_logger.LogError("An exception was thrown while attempting to export Quickbase Purchase Order Details: {Message}", ex.Message);
			}
		}

		private void ExportFamousHeaders(string companyCode)
		{
			_logger.LogDebug("Invoking PurchaseOrderExporter.ExportFamousHeaders() to folder:'{FolderPath}'", _folderPath);
			var filePath = base.GetFilePath(Domain.Constants.Erp.Famous, Domain.Constants.ExportType.PurchaseorderHeaders, _folderPath);

			try
			{
				// Query the PO headers
				var headers = _faHeaderRepo.GetPurchaseOrderHeaders().ToList();

				// Set the company code for all headers.
				headers.ForEach(x => x.CompanyCode = companyCode);

				// Convert to CSV document
				using var writer = new StreamWriter(filePath);
				using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
				csv.Context.TypeConverterCache.AddConverter<bool>(new Infrastructure.TypeConverter.EskerBooleanConverter());
				csv.Context.TypeConverterCache.AddConverter<DateTime>(new Infrastructure.TypeConverter.EskerDateConverter());
				csv.Context.RegisterClassMap<Infrastructure.Maps.PurchaseOrderHeaderMap>();

				// Write document to disk
				csv.WriteRecords(headers);
			}
			catch (Exception ex)
			{
				_logger.LogError("An exception was thrown while attempting to export Famous Purchase Order Headers: {Message}", ex.Message);
			}
		}

		private void ExportFamousDetails(string companyCode)
		{
			_logger.LogDebug("Invoking PurchaseOrderExporter.ExportFamousDetails() to folder:'{FolderPath}'", _folderPath);
			var filePath = base.GetFilePath(Domain.Constants.Erp.Famous, Domain.Constants.ExportType.PurchaseorderItems, _folderPath);

			try
			{
				// Query the details
				var details = _faDetailRepo.GetPurchaseOrderDetails().ToList();

				// Set the company code for all details.
				details.ForEach(x => x.CompanyCode = companyCode);

				// Convert to CSV document
				using var writer = new StreamWriter(filePath);
				using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
				csv.Context.TypeConverterCache.AddConverter<bool>(new Infrastructure.TypeConverter.EskerBooleanConverter());
				csv.Context.TypeConverterCache.AddConverter<DateTime>(new Infrastructure.TypeConverter.EskerDateConverter());
				csv.Context.RegisterClassMap<Infrastructure.Maps.PurchaseOrderDetailMap>();

				// Write document to disk
				csv.WriteRecords(details);
			}
			catch (Exception ex)
			{
				_logger.LogError("An exception was thrown while attempting to export Famous Purchase Order Details: {Message}", ex.Message);
			}
		}
	}
}
