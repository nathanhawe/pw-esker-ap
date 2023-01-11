using EskerAP.Service.Interface;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace EskerAP.Service
{
	public class VoucherExportService : Interface.IVoucherExportService
	{
		private readonly ILogger _logger;
		private readonly IPaidInvoiceExporter _paidInvoiceExporter;
		private readonly ISftpService _sftpService;

		public VoucherExportService(ILogger<VoucherExportService> logger, IPaidInvoiceExporter paidInvoiceExporter, ISftpService sftpService)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_paidInvoiceExporter = paidInvoiceExporter ?? throw new ArgumentNullException(nameof(paidInvoiceExporter));
			_sftpService = sftpService ?? throw new ArgumentNullException(nameof(sftpService));
		}

		public void ExportPaidInvoices(string localDirectory, string remoteDirectory, string companyCode, int daysPast)
		{
			_logger.LogDebug("Invoked ExportPaidInvoices() with '{localDirectory}', '{remoteDirectory}', '{companyCode}', and '{daysPast}'.", localDirectory, remoteDirectory, companyCode, daysPast);
			try
			{
				// Create the export file.
				_paidInvoiceExporter.ExportPaidInvoices(companyCode, localDirectory, daysPast);
				
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
			catch (Exception ex)
			{
				_logger.LogError("An exception occurred while executing ExportPaidInvoices: {Message}", ex.Message);
			}
		}
	}
}
