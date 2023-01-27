using CsvHelper;
using EskerAP.Domain;
using EskerAP.Service.Interface;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace EskerAP.Service
{
	public class UnpaidInvoiceReader : BaseExporter, Interface.IUnpaidInvoiceReader
	{
		private readonly ILogger<UnpaidInvoiceReader> _logger;
		private readonly ISftpService _sftpService;

		public UnpaidInvoiceReader(ILogger<UnpaidInvoiceReader> logger, ISftpService sftpService)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_sftpService = sftpService ?? throw new ArgumentNullException(nameof(sftpService));
		}

		public IEnumerable<UnpaidInvoice> GetUnpaidInvoices(string remoteDirectory, string localDirectory)
		{
			_logger.LogDebug("Invoking UnpaidInvoiceReader.GetUnpaidInvoices() with '{remoteDirectory}' and '{localDirectory}'.", remoteDirectory, localDirectory);

			// Ensure the folder exists
			base.EnsureFolderExists(localDirectory);

			var unpaidInvoices = new List<UnpaidInvoice>();
			try 
			{
				// Retrieve the files from Esker
				var remoteFiles = _sftpService.ListAllFiles(remoteDirectory);
				foreach (var remoteFile in remoteFiles.Where(x => x.IsRegularFile))
				{
					if (_sftpService.DownloadFile(remoteFile.FullName, $"{localDirectory}\\{remoteFile.Name}")) remoteFile.Delete();
				}

				// Read the downloaded files and add to collection
				var localFiles = Directory.GetFiles(localDirectory, "*.csv");
				foreach(var localFile in localFiles)
				{
					_logger.LogDebug("Reading file '{localFile}'.", localFile);

					using var reader = new StreamReader(localFile);
					using var csv = new CsvReader(reader, CultureInfo.InvariantCulture, false);
					csv.Context.RegisterClassMap<Infrastructure.Maps.UnpaidInvoiceMap>();
					unpaidInvoices.AddRange(csv.GetRecords<UnpaidInvoice>());

					// Delete the local file
					reader.Close();
					System.IO.File.Delete(localFile);
				}
			}
			catch (Exception ex)
			{
				_logger.LogError("An error occurred while importing unpaid invoices: {Message}.", ex.Message);
			}

			return unpaidInvoices;
		}
	}
}
