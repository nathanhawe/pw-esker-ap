using EskerAP.Domain;
using EskerAP.Service.Interface;
using Microsoft.Extensions.Logging;
using System;

namespace EskerAP.Service
{
	public class ErpAckService : IErpAckService
	{
		private readonly ILogger<ErpAckService> _logger;

		public ErpAckService(ILogger<ErpAckService> logger)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public string GetErpAckXmlString(ImportApVoucherResponse importApVoucherResponse, string ruid)
		{
			_logger.LogDebug("Attempting to create an ERP Ack XML string for {importApVoucherResponse}", importApVoucherResponse);
			if (importApVoucherResponse == null) throw new ArgumentNullException(nameof(importApVoucherResponse));

			return $@"<?xml version=""1.0"" encoding=""utf-8""?><ERPAck><EskerInvoiceID>{ruid}</EskerInvoiceID>{(importApVoucherResponse.ImportWasSuccessful ? GetERPIDElement(importApVoucherResponse) : GetERPPostingErrorElement(importApVoucherResponse))}</ERPAck>";
		}

		private string GetERPPostingErrorElement(ImportApVoucherResponse importApVoucherResponse)
		{
			var message = "";

			// Exceptions
			if (importApVoucherResponse.Exception != null) message += importApVoucherResponse.Exception.Message;

			// Header
			if (importApVoucherResponse.HeaderErrors != null) message += $"Header: {importApVoucherResponse.HeaderErrors}";

			// Other
			if (importApVoucherResponse.OtherErrors != null) message += $"Other: {importApVoucherResponse.OtherErrors}";

			// Line
			if (importApVoucherResponse.LineErrors.Count > 0) message += "Lines: ";
			foreach (var lineError in importApVoucherResponse.LineErrors) message += $"{lineError};";

			if (message.Length > 1024) message = message[..1024];

			return $@"<ERPPostingError>{message}</ERPPostingError>";
		}

		private string GetERPIDElement(ImportApVoucherResponse importApVoucherResponse)
		{
			return $"<ERPID>{importApVoucherResponse.EntryNumber}</ERPID>";
		}
	}
}
