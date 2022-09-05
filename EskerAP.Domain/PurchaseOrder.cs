using System;
using System.Collections.Generic;
using System.Text;

namespace EskerAP.Domain
{
	public class PurchaseOrder
	{
		public int RecordId { get; set; }
		public string VendorId { get; set; }
		public string PurposeOfPurchase { get; set; }
		public DateTime Date { get; set; }
		public decimal Tax { get; set; }
		public decimal Freight { get; set; }
		public decimal Total { get; set; }

	}
}
