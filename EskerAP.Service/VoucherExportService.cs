﻿using EskerAP.Service.Interface;
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

		public void ExportPaidInvoices(string paidInvoiceLocalDirectory, string paidInvoiceRemoteDirectory, string unpaidInvoiceRemoteDirectory, string companyCode)
		{
			_logger.LogDebug("Invoked ExportPaidInvoices() with '{paidInvoiceLocalDirectory}', '{paidInvoiceRemoteDirectory}', '{unpaidInvoiceRemoteDirectory}', and '{companyCode}'.", paidInvoiceLocalDirectory, paidInvoiceRemoteDirectory, unpaidInvoiceRemoteDirectory, companyCode);
			try
			{
				// Create the export file.
				_paidInvoiceExporter.ExportPaidInvoices(companyCode, paidInvoiceLocalDirectory, unpaidInvoiceRemoteDirectory);
				
				// Get a list of the local files
				var localFiles = System.IO.Directory.GetFiles(paidInvoiceLocalDirectory);

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
