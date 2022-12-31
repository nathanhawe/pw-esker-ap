using EskerAP.Service.Interface;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace EskerAP.Service
{
	public class MasterDataExportService : IMasterDataExportService
	{
		private readonly ILogger<MasterDataExportService> _logger;
		private readonly ICostCenterExporter _costCenterExporter;
		private readonly IGLAccountExporter _glAccountExporter;
		private readonly IPaymentTermsExporter _paymentTermsExporter;
		private readonly IPhaseExporter _phaseExporter;
		private readonly IPurchaseOrderExporter _purchaseOrderExporter;
		private readonly IVendorExporter _vendorExporter;
		private readonly ISftpService _sftpService;

		public MasterDataExportService(
			ILogger<MasterDataExportService> logger, 
			ICostCenterExporter costCenterExporter,
			IGLAccountExporter glAccountExporter,
			IPaymentTermsExporter paymentTermsExporter,
			IPhaseExporter phaseExporter,
			IPurchaseOrderExporter purchaseOrderExporter,
			IVendorExporter vendorExporter,
			ISftpService sftpService) 
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_costCenterExporter = costCenterExporter ?? throw new ArgumentNullException(nameof(costCenterExporter));
			_glAccountExporter = glAccountExporter ?? throw new ArgumentNullException(nameof(glAccountExporter));
			_paymentTermsExporter = paymentTermsExporter ?? throw new ArgumentNullException(nameof(paymentTermsExporter));
			_phaseExporter = phaseExporter ?? throw new ArgumentNullException(nameof(phaseExporter));
			_purchaseOrderExporter = purchaseOrderExporter ?? throw new ArgumentNullException(nameof(purchaseOrderExporter));
			_vendorExporter = vendorExporter ?? throw new ArgumentNullException(nameof(vendorExporter));
			_sftpService = sftpService ?? throw new ArgumentNullException(nameof(sftpService));
		}

		public void ExportMasterData(string localDirectory, string remoteDirectory, string companyCode, bool purchaseOrdersOnly = false)
		{
			_logger.LogDebug("Invoked ExportMasterData() with '{localDirectory}', '{remoteDirectory}', '{companyCode}', and '{purchaseOrdersOnly}'", localDirectory, remoteDirectory, companyCode, purchaseOrdersOnly);
			try
			{
				// Create all of the export files
				if (!purchaseOrdersOnly)
				{
					_costCenterExporter.ExportCostCenters(companyCode);
					_glAccountExporter.ExportGLAccounts(companyCode);
					_paymentTermsExporter.ExportPaymentTerms(companyCode);
					_phaseExporter.ExportPhases(companyCode);
					_vendorExporter.ExportVendors(companyCode);
				}
				_purchaseOrderExporter.ExportPurchaseOrders(companyCode);

				// Get a list of the local files
				var localFiles = System.IO.Directory.GetFiles(localDirectory);

				// Transfer the files to the SFTP server
				string remoteFilePath;
				foreach (var file in localFiles)
				{
					remoteFilePath = file.Replace(".csv", ".part");

					// Transfer the local file
					_sftpService.UploadFile(file, remoteFilePath);

					// Rename the remote file
					_sftpService.RenameRemoteFile(remoteFilePath, file);

					// Delete the local file
					System.IO.File.Delete(file);

				}
			}
			catch(Exception ex)
			{
				_logger.LogError("An exception occurred while executing ExportMasterData: {Message}", ex.Message);
			}
		}
	}
}
