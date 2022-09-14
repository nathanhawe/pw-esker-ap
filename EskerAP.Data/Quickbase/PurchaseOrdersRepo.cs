using EskerAP.Domain;
using EskerAP.Domain.Constants.Quickbase;
using QuickBase.Api;
using QuickBase.Api.Constants;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace EskerAP.Data.Quickbase
{
	public class PurchaseOrdersRepo : QuickbaseRepo<PurchaseOrder>, IPurchaseOrdersRepo
	{
		public PurchaseOrdersRepo(IQuickBaseConnection quickBaseConnection)
			: base(quickBaseConnection) { }

		/// <summary>
		/// Queries the Purchase Orders table in Quickbase for incomplete records.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<PurchaseOrder> Get()
		{
			var clist = $"{(int)PurchaseOrdersField.VendorId}.{(int)PurchaseOrdersField.PurposeOfPurchase}.{(int)PurchaseOrdersField.Date}.{(int)PurchaseOrdersField.Tax}.{(int)PurchaseOrdersField.MaximumFreightCharge}.{(int)PurchaseOrdersField.Total}.";
			var slist = $"{(int)PurchaseOrdersField.RecordId}";
			var query = $"{{{(int)PurchaseOrdersField.POPaid}.{ComparisonOperator.EX}.'0'}}AND{{{(int)PurchaseOrdersField.DateCreated}.{ComparisonOperator.OAF}.'01-01-2021'}}";

			return base.Get(TableId.PurchaseOrders, query, clist, slist, ConvertToPurchaseOrder);
		}

		/// <summary>
		/// Converts and XElement object representing an API_DoQuery response from the Purchase Orders
		/// table in Quickbase into a collection of <c>PurchaseOrder</c> objects.
		/// </summary>
		/// <param name="doQuery"></param>
		/// <returns></returns>
		private IEnumerable<PurchaseOrder> ConvertToPurchaseOrder(XElement doQuery)
		{
			var purchaseOrders = new List<PurchaseOrder>();
			var records = doQuery.Elements("record");

			foreach (var record in records)
			{
				var recordId = ParseInt(record.Attribute("rid")?.Value) ?? 0;
				var temp = new PurchaseOrder
				{
					RecordId = recordId
				};

				var fields = record.Elements("f");
				foreach (var field in fields)
				{
					var fieldId = ParseInt(field.Attribute("id")?.Value) ?? 0;

					switch (fieldId)
					{
						case (int)PurchaseOrdersField.VendorId: temp.VendorId = field.Value?.ToUpper()?.Trim() ?? String.Empty; break;
						case (int)PurchaseOrdersField.PurposeOfPurchase: temp.PurposeOfPurchase = field.Value?.Trim() ?? String.Empty; break;
						case (int)PurchaseOrdersField.Date: temp.Date = ParseDate(field.Value); break;
						case (int)PurchaseOrdersField.Tax: temp.Tax = ParseDecimal(field.Value) ?? 0; break;
						case (int)PurchaseOrdersField.MaximumFreightCharge: temp.Freight = ParseDecimal(field.Value) ?? 0; break;
						case (int)PurchaseOrdersField.Total: temp.Total = ParseDecimal(field.Value) ?? 0; break;							
					}
				}
				purchaseOrders.Add(temp);
			}

			return purchaseOrders;
		}
	}
}
