namespace EskerAP.Domain.Constants.Quickbase
{
	/// <summary>
	/// Fields used in the Purchase Orders table within the Purchase Order Requests and Approvals application in Quickbase.
	/// </summary>
	public enum PurchaseOrdersField
	{
		Unknown = 0,
		DateCreated = 1,
		DateModified = 2,
		RecordId = 3,
		RecordOwner = 4,
		LastModifiedBy = 29,

		/// <summary>
		/// [Date] - Date
		/// </summary>
		Date = 7,

		/// <summary>
		/// [Requested By] - User
		/// </summary>
		RequestedBy = 8,

		/// <summary>
		/// [Total] - Currency
		/// </summary>
		Total = 51,

		/// <summary>
		/// [Maximum Freight Charge] - Currency
		/// </summary>
		MaximumFreightCharge = 80,

		/// <summary>
		/// [Purpose of Purchase] - String
		/// </summary>
		PurposeOfPurchase = 85,

		/// <summary>
		/// [Tax] - Currency
		/// </summary>
		Tax = 114,

		/// <summary>
		/// [PO No.] - String that concatenates the PO Number with a location prefix
		/// </summary>
		PONo = 507,

		/// <summary>
		/// [PO Rejected] - Checkbox indicates whether the purchase order was rejected during the approval phase.
		/// </summary>
		PORejected = 520,

		/// <summary>
		/// [Order Placed With Vendor By] - User 
		/// </summary>
		OrderPlacedWithVendorBy = 561,

		/// <summary>
		/// [PO Paid] - Checkbox indicates whether the purchase order has been completely paid.
		/// </summary>
		POPaid = 679,

		/// <summary>
		/// [Is this Purchase Related to a CapEX?] - Text
		/// </summary>
		IsCapEx = 888,

		/// <summary>
		/// [VENDORID] - String
		/// </summary>
		VendorId = 922,

		/// <summary>
		/// [Total Amount Received] - Currency that indicates the total dollar amount of items received.
		/// </summary>
		TotalAmountReceived = 935,
	}
}
