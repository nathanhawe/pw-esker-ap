using EskerAP.Domain.Constants;
using System;
using System.Collections.Generic;
using System.Text;

namespace EskerAP.Domain
{
	public class Item
	{
		/// <summary>
		/// Quickbase Record ID for matching
		/// </summary>
		public int RecordId { get; set; }

		/// <summary>
		/// The unique identifier for the specific company defined in Esker.
		/// </summary>
		public string CompanyCode { get; set; }

		/// <summary>
		/// The unique identifier of the vendor to whom the order is sent.
		/// </summary>
		public string VendorNumber { get; set; }

		/// <summary>
		/// Order number to which the item belongs.
		/// </summary>
		public string OrderNumber { get; set; }

		/// <summary>
		/// Date the order was sent (YYYY-MM-DD).
		/// </summary>
		public DateTime OrderDate { get; set; }

		/// <summary>
		/// Line number within the purchase order.
		/// </summary>
		public string ItemNumber { get; set; }

		/// <summary>
		/// Service or part number.
		/// </summary>
		public string PartNumber { get; set; }

		/// <summary>
		/// Indicates if the item is a service with a global amount or an item with a price per quantity.
		/// </summary>
		public ItemType ItemType { get; set; } = ItemType.QuantityBased;

		/// <summary>
		/// Item description
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// Identifier of the G/L Account of the line item.
		/// </summary>
		public string GLAccount { get; set; }

		/// <summary>
		/// The expense category description.
		/// </summary>
		public string Group { get; set; }

		/// <summary>
		/// Identifier of the cost center of the line item.
		/// </summary>
		public string CostCenter { get; set; }

		/// <summary>
		/// Name of the optional custom dimension.
		/// </summary>
		public string FreeDimension1 { get; set; }

		/// <summary>
		/// Code of the optional custom dimension.
		/// </summary>
		public string FreeDimension1ID { get; set; }

		/// <summary>
		/// Identifier of the budget.
		/// </summary>
		public string BudgetId { get; set; }

		/// <summary>
		/// Item unit price.
		/// </summary>
		public decimal UnitPrice { get; set; }

		/// <summary>
		/// Item total ordered amount.
		/// </summary>
		public decimal OrderedAmount { get; set; }

		/// <summary>
		/// Unit of measure.
		/// </summary>
		public string UnitOfMeasureCode { get; set; }

		/// <summary>
		/// Item ordered quantity.
		/// </summary>
		public decimal OrderedQuantity { get; set; }

		/// <summary>
		/// Amount of the item that has already been invoiced.
		/// </summary>
		public decimal InvoicedAmount { get; set; }

		/// <summary>
		/// Quantity of the item that has already been invoiced.
		/// </summary>
		public decimal InvoicedQuantity { get; set; }

		/// <summary>
		/// Amount of the item that has already been delivered.
		/// </summary>
		public decimal DeliveredAmount => this.DeliveredQuantity * this.UnitPrice;

		/// <summary>
		/// Quantity of the item that has already been delivered.
		/// </summary>
		public decimal DeliveredQuantity { get; set; }

		/// <summary>
		/// Currency of the item when purchase order has been sent from the Procurement module.
		/// </summary>
		public Currency Currency { get; set; } = Currency.USD;

		/// <summary>
		/// Tax code for the item.
		/// </summary>
		public string TaxCode { get; set; }

		/// <summary>
		/// Tax rate value, as a percentage (e.g. 10 for 10%).
		/// </summary>
		public decimal TaxRate { get; set; }

		/// <summary>
		/// Non-deductible tax rate value as a percentage (e.g. 10 for 10%).
		/// </summary>
		public decimal NonDeductibleTaxRate { get; set; }

		public string Receiver { get; set; }

		public CostType CostType { get; set; }
		
		/// <summary>
		/// Indicates whether the purchase order was sent from the Procurement module or not.
		/// </summary>
		public bool IsLocalPO { get; set; } = false;

		/// <summary>
		/// Indicates whether the purchase order was created in the ERP or not.
		/// </summary>
		public bool IsCreatedInErp { get; set; } = true;

		/// <summary>
		/// Indicates whether the invoice reconcilliation should be based on goods and services receipt 
		/// or on purchase order.  If selected, the reconcilliation is based on goods and services receipt. 
		/// </summary>
		public bool Griv { get; set; } = true;

		/// <summary>
		/// Indicates whether an invoice is expected for the PO line item or not.
		/// </summary>
		public bool NoMoreInvoiceExpected { get; set; } = true;

		/// <summary>
		/// Indicates whether a goods and services receipt is expected for the item or not.
		/// </summary>
		public bool NoGoodsReceipt { get; set; } = true;

	}
}
