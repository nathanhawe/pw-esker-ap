namespace EskerAP.Domain.Constants.Quickbase
{
	/// <summary>
	/// Fields used in the Items table within the Purchase Order Requests and Approvals application in Quickbase.
	/// </summary>
	public enum ItemsField
	{
		Unknown = 0,

		/// <summary>
		/// [Record ID#] - Unique record identifier in Quickbase.
		/// </summary>
		RecordId = 3,

		/// <summary>
		/// [PO - Date] - Date the purchase order was created.
		/// </summary>
		PODate = 8,

		/// <summary>
		/// [Item Description] - String that describes the item.
		/// </summary>
		ItemDescription = 10,

		/// <summary>
		/// [Inventory Item Number] - Integer that is the unique identifier of the item in the Inventory Database.
		/// </summary>
		IventoryItemNumber = 12,

		/// <summary>
		/// [Cost per Item] - Currency indicates the per item cost to four decimal places.
		/// </summary>
		CostPerItem = 14,

		/// <summary>
		/// [Units] - String that indicates the unit of measure.
		/// </summary>
		Units = 15,

		/// <summary>
		/// [Quantity Ordered] - Numeric indicates the total quantity of item to purchase.
		/// </summary>
		QuantityOrdered = 16,

		/// <summary>
		/// [Total Quantity Received] - Numeric indicates the quantity of item received so far.
		/// </summary>
		TotalQuantityReceived = 19,

		/// <summary>
		/// [PO No.] - String indicates the the purchase order number the item belongs to.
		/// </summary>
		PONumber = 23,

		/// <summary>
		/// [PO - PO Paid] - Checkbox indicates whether the purchase order has been completely paid.
		/// </summary>
		POPaid = 56,

		/// <summary>
		/// [VENDORID] - String indicates the vendors unique ID used in Famous.
		/// </summary>
		VendorId = 57,

		/// <summary>
		/// [PO - Is this Purchase Related to a CapEx?] - String where "Yes" indicates that the purchase order is CapEx.
		/// </summary>
		IsCapEx = 58,

		/// <summary>
		/// [PO - PO Rejected] - Checkbox indicates that the purchase order was rejected during the approval phase.
		/// </summary>
		PORejected = 60,
	}
}
