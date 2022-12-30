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
	public class PurchaseOrdersRepo : QuickbaseRepo<Header>, IPurchaseOrdersRepo
	{
		public PurchaseOrdersRepo(IQuickBaseConnection quickBaseConnection)
			: base(quickBaseConnection) { }

		/// <summary>
		/// Queries the Purchase Orders table in Quickbase for incomplete records.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<Header> Get()
		{
			var clist = GetClist();
			var slist = $"{(int)PurchaseOrdersField.RecordId}";
			var query = $"{{{(int)PurchaseOrdersField.POPaid}.{ComparisonOperator.EX}.'0'}}AND{{{(int)PurchaseOrdersField.DateCreated}.{ComparisonOperator.OAF}.'01-01-2021'}}AND{{{(int)PurchaseOrdersField.PORejected}.{ComparisonOperator.EX}.'0'}}";

			return base.Get(TableId.PurchaseOrders, query, clist, slist, ConvertToPurchaseOrder);
		}

		private string GetClist()
		{
			var clist = $"{(int)PurchaseOrdersField.VendorId}.";
			clist += $"{(int)PurchaseOrdersField.PONo}.";
			clist += $"{(int)PurchaseOrdersField.Date}.";
			clist += $"{(int)PurchaseOrdersField.Total}.";
			clist += $"{(int)PurchaseOrdersField.TotalAmountReceived}.";
			clist += $"{(int)PurchaseOrdersField.OrderPlacedWithVendorBy}.";
			clist += $"{(int)PurchaseOrdersField.RequestedBy}.";
			clist += $"{(int)PurchaseOrdersField.Tax}.";
			clist += $"{(int)PurchaseOrdersField.MaximumFreightCharge}.";
			clist += $"{(int)PurchaseOrdersField.IsCapEx}.";

			return clist;
		}

		/// <summary>
		/// Converts and XElement object representing an API_DoQuery response from the Purchase Orders
		/// table in Quickbase into a collection of <c>PurchaseOrder</c> objects.
		/// </summary>
		/// <param name="doQuery"></param>
		/// <returns></returns>
		private IEnumerable<Header> ConvertToPurchaseOrder(XElement doQuery)
		{
			var purchaseOrders = new List<Header>();
			var records = doQuery.Elements("record");

			foreach (var record in records)
			{
				var recordId = ParseInt(record.Attribute("rid")?.Value) ?? 0;
				var temp = new Header
				{
					RecordId = recordId
				};

				var fields = record.Elements("f");
				foreach (var field in fields)
				{
					var fieldId = ParseInt(field.Attribute("id")?.Value) ?? 0;

					switch (fieldId)
					{
						case (int)PurchaseOrdersField.VendorId: temp.VendorNumber = field.Value?.ToUpper()?.Trim() ?? String.Empty; break;
						case (int)PurchaseOrdersField.PONo: temp.OrderNumber = "Q" + field.Value?.ToUpper()?.Trim() ?? String.Empty; break;
						case (int)PurchaseOrdersField.Date: temp.OrderDate = ParseDate(field.Value); break;
						case (int)PurchaseOrdersField.Total: temp.OrderedAmount = ParseDecimal(field.Value) ?? 0; break;
						case (int)PurchaseOrdersField.TotalAmountReceived: temp.DeliveredAmount = ParseDecimal(field.Value) ?? 0; break;
						case (int)PurchaseOrdersField.OrderPlacedWithVendorBy: temp.Buyer = field.Value?.Trim() ?? String.Empty; break;
						case (int)PurchaseOrdersField.RequestedBy: temp.Receiver = field.Value?.Trim() ?? String.Empty; break;
						case (int)PurchaseOrdersField.Tax: temp.Tax = ParseDecimal(field.Value) ?? 0; break;
						case (int)PurchaseOrdersField.MaximumFreightCharge: temp.Freight = ParseDecimal(field.Value) ?? 0; break;
						case (int)PurchaseOrdersField.IsCapEx: temp.IsCapEx = (field.Value?.ToUpper() ??"") == "YES"; break;
					}
				}
				purchaseOrders.Add(temp);
			}

			return purchaseOrders;
		}
	}
}
