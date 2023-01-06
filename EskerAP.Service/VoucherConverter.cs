using EskerAP.Domain;
using Microsoft.Extensions.Logging;
using System;
using System.Xml;

namespace EskerAP.Service
{
	internal enum InvoiceElement
	{
		Invoice,
		VendorNumber,
		InvoiceNumber,
		InvoiceDate,
		PaymentTerms,
		DueDate,
		LineItems,
		CostCenter,
		GLAccount,
		Description,
		Quantity,
		Amount,
		Z_Phase
	}

	public class VoucherConverter : Interface.IVoucherConverter
	{
		private readonly ILogger<VoucherConverter> _logger;
		public VoucherConverter(ILogger<VoucherConverter> logger) 
		{ 
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public Voucher ConvertXmlString(string xml)
		{
			_logger.LogDebug("Invoked ConvertXmlString().");

			try
			{
				var voucher = new Voucher();
				XmlDocument doc = new XmlDocument();
				doc.LoadXml(xml);

				var invoice = doc.GetElementsByTagName($"{InvoiceElement.Invoice}")[0];
				var vendorNumber = doc.GetElementsByTagName($"{InvoiceElement.VendorNumber}")[0];
				var invoiceNumber = doc.GetElementsByTagName($"{InvoiceElement.InvoiceNumber}")[0];
				var invoiceDate = doc.GetElementsByTagName($"{InvoiceElement.InvoiceDate}")[0];
				var paymentTerms = doc.GetElementsByTagName($"{InvoiceElement.PaymentTerms}")[0];
				var dueDate = doc.GetElementsByTagName($"{InvoiceElement.DueDate}")[0];
				var lineItems = doc.GetElementsByTagName($"{InvoiceElement.LineItems}")[0].ChildNodes;

				voucher.Ruid = invoice.Attributes["RUID"].Value;
				voucher.VendorId = vendorNumber?.InnerText;
				voucher.InvoiceNumber = invoiceNumber?.InnerText;
				voucher.InvoiceDate = DateTime.TryParse(invoiceDate?.InnerText, out DateTime iDate) ? iDate : DateTime.MinValue;
				voucher.PayTerms = paymentTerms?.InnerText;
				voucher.DueDate = DateTime.TryParse(dueDate?.InnerText, out DateTime dDate) ? dDate : DateTime.MinValue;

				XmlNode costCenter, glAccount, description, amount, line, phase;
				
				for(int i = 0; i < lineItems.Count; i++)
				{
					line = lineItems[i];
					costCenter = line.SelectSingleNode($"{InvoiceElement.CostCenter}");
					glAccount = line.SelectSingleNode($"{InvoiceElement.GLAccount}");
					description = line.SelectSingleNode($"{InvoiceElement.Description}");
					amount = line.SelectSingleNode($"{InvoiceElement.Amount}");
					phase = line.SelectSingleNode($"{InvoiceElement.Z_Phase}");
					voucher.Lines.Add(new VoucherItem
					{
						CostCenterId = costCenter?.InnerText,
						GlAccountCode = glAccount?.InnerText,
						LineDescription = description?.InnerText,
						Quantity = 1,
						Rate = Decimal.TryParse(amount?.InnerText, out decimal rDec) ? rDec : 0,
						PhaseId = phase?.InnerText,
					});
				}
				
				return voucher;
			}
			catch (Exception ex)
			{
				_logger.LogError("An exception occurred while attempting to convert '{xml}' to a Voucher object. {Message}", xml, ex.Message);
				return null;
			}
		}
	}
}
