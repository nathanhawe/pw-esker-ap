using System;

namespace EskerAP.Domain
{
	public class PaidInvoice
	{
		/// <summary>
		/// Unique identifier of the company
		/// </summary>
		public string CompanyCode { get; set; }

		/// <summary>
		/// Vendor identifier
		/// </summary>
		public string VendorNumber { get; set; }

		/// <summary>
		/// Invoice number
		/// </summary>
		public string InvoiceNumber { get; set; }

		/// <summary>
		/// Date the payment was made
		/// </summary>
		public DateTime PaymentDate { get; set; }

		/// <summary>
		/// Esker payment method
		/// </summary>
		public string PaymentMethod { get; set; }

		/// <summary>
		/// Payment Reference (e.g., Check Number)
		/// </summary>
		public string PaymentReference { get; set; }

	}
}
