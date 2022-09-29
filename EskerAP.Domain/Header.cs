using EskerAP.Domain.Constants;
using System;

namespace EskerAP.Domain
{
	public class Header
	{
		/// <summary>
		/// Quickbase Record ID for matching
		/// </summary>
		public int RecordId { get; set; }

		/// <summary>
		/// The amount of tax from the Quickbase header
		/// </summary>
		public decimal Tax { get; set; }

		/// <summary>
		/// The amount of freight from the Quickbase header
		/// </summary>
		public decimal Freight { get; set; }

		/// <summary>
		/// Indicates if the Quickbase purchase order is CapEx
		/// </summary>
		public bool IsCapEx { get; set; } = false;

		/// <summary>
		/// The unique identifier for the specific company defined in Esker.
		/// </summary>
		public string CompanyCode { get; set; }

		/// <summary>
		/// The unique identifier of the vendor to whom the order is sent.
		/// </summary>
		public string VendorNumber { get; set; }

		/// <summary>
		/// Vendor number.  This vendor is different from the purchasing vendor specified in the VendorNumber field.
		/// </summary>
		public string DifferentInvoicingParty { get; set; }

		/// <summary>
		/// The Purchase Order's unique identifier
		/// </summary>
		public string OrderNumber { get; set; }

		/// <summary>
		/// Date the order was sent (YYYY-MM-DD).
		/// </summary>
		public DateTime OrderDate { get; set; }

		/// <summary>
		/// Purchase order total amount
		/// </summary>
		public decimal OrderedAmount { get; set; }

		/// <summary>
		/// Sum of the delivered amounts from Purchase Order items.
		/// </summary>
		public decimal DeliveredAmount { get; set; }

		/// <summary>
		/// Amount of the purchase order that has already been invoiced.
		/// </summary>
		public decimal InvoicedAmount { get; set; }

		/// <summary>
		/// Currency of the purchase order.
		/// </summary>
		public Currency Currency { get; set; } = Currency.USD;

		/// <summary>
		/// Identifier of the buyer associated with the purchase order (e.g., john.doe@example.com).
		/// </summary>
		public string Buyer { get; set; }

		/// <summary>
		/// Identifier of the recipient associated with the purchase order (e.g., john.doe@example.com).
		/// </summary>
		public string Receiver { get; set; }

		/// <summary>
		/// Indicates whether the purchase order was sent from the Procurement module or not.
		/// </summary>
		public bool IsLocalPO { get; set; } = false;

		/// <summary>
		/// Indicates whether the purchase order was created in the ERP or not.
		/// </summary>
		public bool IsCreatedInErp { get; set; } = true;

		/// <summary>
		/// Indicated whether an invoice is expected for the purchase order or not.
		/// </summary>
		public bool NoMoreInvoiceExpected { get; set; } = true;

	}
}
