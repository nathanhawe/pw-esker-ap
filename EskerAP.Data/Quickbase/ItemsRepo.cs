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
	public class ItemsRepo : QuickbaseRepo<Item>, IItemsRepo
	{
		public ItemsRepo(IQuickBaseConnection quickBaseConnection)
			: base(quickBaseConnection) { }

		/// <summary>
		/// Queries the Items table in Quickbase for all records associated with an incomplete Purchase Order.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<Item> Get()
		{
			string clist = GetClist();
			var slist = $"{(int)ItemsField.RecordId}";
			var query = $"{{{(int)ItemsField.POPaid}.{ComparisonOperator.EX}.'0'}}AND{{{(int)ItemsField.PODate}.{ComparisonOperator.OAF}.'01-01-2021'}}";

			return base.Get(TableId.Items, query, clist, slist, ConvertToItem);
		}

		/// <summary>
		/// Returns the CLIST value for querying records from the Items table in Quickbase.
		/// </summary>
		/// <returns></returns>
		private string GetClist()
		{
			var clist = $"{(int)ItemsField.VendorId}."; 
			clist += $"{(int)ItemsField.PONumber}.";
			clist += $"{(int)ItemsField.PODate}.";
			clist += $"{(int)ItemsField.IventoryItemNumber}.";
			clist += $"{(int)ItemsField.ItemDescription}.";
			clist += $"{(int)ItemsField.CostPerItem}.";
			clist += $"{(int)ItemsField.QuantityOrdered}.";
			clist += $"{(int)ItemsField.Units}.";
			clist += $"{(int)ItemsField.TotalQuantityReceived}.";
			clist += $"{(int)ItemsField.IsCapEx}.";

			return clist;
		}


		/// <summary>
		/// Converts and XElement object representing an API_DoQuery response from the Items
		/// table in Quickbase into a collection of <c>Item</c> objects.
		/// </summary>
		/// <param name="doQuery"></param>
		/// <returns></returns>
		private IEnumerable<Item> ConvertToItem(XElement doQuery)
		{
			var items = new List<Item>();
			var records = doQuery.Elements("record");

			foreach (var record in records)
			{
				var recordId = ParseInt(record.Attribute("rid")?.Value) ?? 0;
				var temp = new Item
				{
					RecordId = recordId
				};

				var fields = record.Elements("f");
				foreach (var field in fields)
				{
					var fieldId = ParseInt(field.Attribute("id")?.Value) ?? 0;

					switch (fieldId)
					{
						case (int)ItemsField.VendorId: temp.VendorNumber = field.Value?.ToUpper()?.Trim() ?? String.Empty; break;
						case (int)ItemsField.PONumber: temp.OrderNumber = field.Value?.ToUpper()?.Trim() ?? String.Empty; break;
						case (int)ItemsField.PODate: temp.OrderDate = ParseDate(field.Value); break;
						case (int)ItemsField.IventoryItemNumber: temp.PartNumber = field.Value?.Trim() ?? String.Empty; break;
						case (int)ItemsField.ItemDescription: temp.Description = field.Value?.Trim() ?? String.Empty; break;
						case (int)ItemsField.CostPerItem: temp.UnitPrice = ParseDecimal(field.Value) ?? 0; break;
						case (int)ItemsField.QuantityOrdered: temp.OrderedQuantity = ParseDecimal(field.Value) ?? 0; break;
						case (int)ItemsField.Units: temp.UnitOfMeasureCode = field.Value?.Trim() ?? String.Empty; break;
						case (int)ItemsField.TotalQuantityReceived: temp.DeliveredQuantity = ParseDecimal(field.Value) ?? 0; break;
						case (int)ItemsField.IsCapEx: temp.CostType = (field.Value?.ToUpper()?.Trim() ?? String.Empty) == "YES" ? Domain.Constants.CostType.CapEx : Domain.Constants.CostType.OpEx; break;
					}
				}
				items.Add(temp);
			}

			return items;
		}
	}
}
