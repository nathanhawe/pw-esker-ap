using CsvHelper;
using EskerAP.Domain;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace EskerAP.Service
{
	public class PurchaseOrderExporter : BaseExporter, Interface.IPurchaseOrderExporter
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
			List<Domain.Header> headers;
			List<Domain.Item> details;

			/* Get Headers */
			headers = _qbHeaderRepo.Get().ToList();
			headers.AddRange(_faHeaderRepo.GetPurchaseOrderHeaders().ToList());
			headers.ForEach(x => x.CompanyCode = companyCode);

			/* Get Details */
			details = _qbDetailRepo.Get().ToList();

			// Add freight and tax detail from QB headers
			var taxLines = headers.Where(x => x.Tax > 0).Select(x => new Item
			{
				CompanyCode = companyCode,
				VendorNumber = x.VendorNumber,
				OrderNumber = x.OrderNumber,
				OrderDate = x.OrderDate,
				ItemNumber = "t01",
				Description = "Tax amount from QB PO header",
				OrderedAmount = x.Tax,
				CostType = (x.IsCapEx ? Domain.Constants.CostType.CapEx : Domain.Constants.CostType.OpEx),
				ItemType = Domain.Constants.ItemType.AmountBased
			}).ToList();

			var freightLines = headers.Where(x => x.Freight > 0).Select(x => new Item
			{
				CompanyCode = companyCode,
				VendorNumber = x.VendorNumber,
				OrderNumber = x.OrderNumber,
				OrderDate = x.OrderDate,
				ItemNumber = "f01",
				Description = "Freight amount from QB PO header",
				OrderedAmount = x.Freight,
				CostType = (x.IsCapEx ? Domain.Constants.CostType.CapEx : Domain.Constants.CostType.OpEx),
				ItemType = Domain.Constants.ItemType.AmountBased
			}).ToList();

			details.AddRange(taxLines);
			details.AddRange(freightLines);

			// Add Famous details
			details.AddRange(_faDetailRepo.GetPurchaseOrderDetails().ToList());
			details.ForEach(x => x.CompanyCode = companyCode);

			// Export documents
			ExportHeaders(headers);
			ExportDetails(details);
		}

		private void ExportHeaders(List<Domain.Header> headers)
		{
			_logger.LogDebug("Invoking PurchaseOrderExporter.ExportHeaders() to folder:'{FolderPath}'", _folderPath);
			var filePath = base.GetFilePath(Domain.Constants.Erp.Combined, Domain.Constants.ExportType.PurchaseorderHeaders, _folderPath);
			try
			{
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
				_logger.LogError("An exception was thrown while attempting to export Purchase Order Headers: {Message}", ex.Message);
			}
		}

		private void ExportDetails(List<Domain.Item> details)
		{
			_logger.LogDebug("Invoking PurchaseOrderExporter.ExportDetails() to folder:'{FolderPath}'", _folderPath);
			var filePath = base.GetFilePath(Domain.Constants.Erp.Combined, Domain.Constants.ExportType.PurchaseorderItems, _folderPath);

			try
			{
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
				_logger.LogError("An exception was thrown while attempting to export Purchase Order Details: {Message}", ex.Message);
			}
		}
	}
}
