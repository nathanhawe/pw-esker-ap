using EskerAP.Data.Famous;
using EskerAP.Domain;
using EskerAP.Service.Interface;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace EskerAP.Service
{
	public class VoucherImportService : Interface.IVoucherImportService
	{
		private readonly ILogger<VoucherImportService> _logger;
		private readonly IImportApVouchersRepo _importApVouchersRepo;
		private readonly ISftpService _sftpService;
		private readonly IVoucherConverter _voucherConverter;
		private readonly IErpAckService _erpAckService;

		public VoucherImportService(
			ILogger<VoucherImportService> logger,
			IImportApVouchersRepo importApVouchersRepo,
			ISftpService sftpService,
			IVoucherConverter voucherConverter,
			IErpAckService erpAckService) 
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_importApVouchersRepo = importApVouchersRepo ?? throw new ArgumentNullException(nameof(importApVouchersRepo));
			_sftpService = sftpService ?? throw new ArgumentNullException(nameof(sftpService));
			_voucherConverter = voucherConverter ?? throw new ArgumentNullException(nameof(voucherConverter));
			_erpAckService = erpAckService ?? throw new ArgumentNullException(nameof(erpAckService));
		}

		public void ImportVouchers(string remoteDirectory, string localDirectory, string erpAckDirectory)
		{
			_logger.LogDebug("Attempting to import vouchers from '{remoteDirectory}' using local directory '{localDirectory}'.", remoteDirectory, localDirectory);

			try
			{
				// Ensure the directories exist
				if (!Directory.Exists(localDirectory)) Directory.CreateDirectory(localDirectory);
				if (!Directory.Exists(erpAckDirectory)) Directory.CreateDirectory(erpAckDirectory);

				// Download all files from the remote connection
				var remoteFiles = _sftpService.ListAllFiles(remoteDirectory);
				foreach (var file in remoteFiles.Where(x => x.IsRegularFile))
				{
					if (_sftpService.DownloadFile(file.FullName, $"{localDirectory}\\{file.Name}")) file.Delete();
				}

				// Process the downloaded files
				var localVoucherFiles = Directory.GetFiles(localDirectory, "*.xml");
				string xmlString, ackXml;
				Voucher voucher;
				ImportApVoucherResponse importResponse;
				foreach (var file in localVoucherFiles)
				{
					xmlString = File.ReadAllText(file);
					voucher = _voucherConverter.ConvertXmlString(xmlString);
					importResponse = _importApVouchersRepo.ImportVoucher(voucher);

					// Create an Ack File
					ackXml = _erpAckService.GetErpAckXmlString(importResponse, voucher.Ruid);
					File.WriteAllText($"{erpAckDirectory}\\{DateTime.Now.ToFileTime()}.xml", ackXml, Encoding.UTF8);

					// Archive local file
					ArchiveFile(file);
				}

				// Upload all Ack files then archive
				var localAckFiles = Directory.GetFiles(erpAckDirectory, "*.xml");
				string remoteFilePath;
				foreach (var file in localAckFiles)
				{
					remoteFilePath = file.Replace(".xml", ".part");

					// Transfer the local file
					_sftpService.UploadFile(file, remoteFilePath);

					// Rename the remote file
					_sftpService.RenameRemoteFile(remoteFilePath, file);

					// Archive the local file
					ArchiveFile(file);
				}
			}
			catch (Exception ex)
			{
				_logger.LogError("An error occured while importing vouchers: {Message}", ex.Message);
			}
		}

		private void ArchiveFile(string filePath)
		{
			File.Move(filePath, $"{filePath}_{DateTime.Now.ToFileTime()}.bak");
		}
	}
}
