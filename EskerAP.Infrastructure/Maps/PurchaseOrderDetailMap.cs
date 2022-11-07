using CsvHelper.Configuration;

namespace EskerAP.Infrastructure.Maps
{
	public class PurchaseOrderDetailMap : ClassMap<Domain.Item>
	{
		private static TypeConverter.MaxLengthStringConverter MaxLength(int maxLength) => new TypeConverter.MaxLengthStringConverter(maxLength);
		public PurchaseOrderDetailMap()
		{
			Map(x => x.CompanyCode).Index(0).Name("CompanyCode__");
			Map(x => x.VendorNumber).Index(1).Name("VendorNumber__");
			Map(x => x.OrderNumber).Index(2).Name("OrderNumber__");
			Map(x => x.OrderDate).Index(3).Name("OrderDate))");
			Map(x => x.ItemNumber).Index(4).Name("ItemNumber__");
			Map(x => x.PartNumber).Index(5).Name("PartNumber__");
			Map(x => x.ItemType).Index(6).Name("ItemType__");
			Map(x => x.Description).Index(7).Name("Description__").TypeConverter(MaxLength(100));
			Map(x => x.GLAccount).Index(8).Name("GLAccount__");
			Map(x => x.Group).Index(9).Name("Group__");
			Map(x => x.CostCenter).Index(10).Name("CostCenter__");
			Map(x => x.ProjectCode).Index(11).Name("ProjectCode__");
			Map(x => x.InternalOrder).Index(12).Name("InternalOrder__");
			Map(x => x.WBSElement).Index(13).Name("WBSElement__");
			Map(x => x.WBSElementID).Index(14).Name("WBSElementID__");
			Map(x => x.FreeDimension1).Index(15).Name("FreeDimension1__");
			Map(x => x.FreeDimension1ID).Index(16).Name("FreeDimension1ID__");
			Map(x => x.BudgetId).Index(17).Name("BudgetID__");
			Map(x => x.UnitPrice).Index(18).Name("UnitPrice__");
			Map(x => x.OrderedAmount).Index(19).Name("OrderedAmount__");
			Map(x => x.UnitOfMeasureCode).Index(20).Name("UnitOfMeasureCode__");
			Map(x => x.OrderedQuantity).Index(21).Name("OrderedQuantity__");
			Map(x => x.InvoicedAmount).Index(22).Name("InvoicedAmount__");
			Map(x => x.InvoicedQuantity).Index(23).Name("InvoicedQuantity__");
			Map(x => x.DeliveredAmount).Index(24).Name("DeliveredAmount__");
			Map(x => x.DeliveredQuantity).Index(25).Name("DeliveredQuantity__");
			Map(x => x.Currency).Index(26).Name("Currency__");
			Map(x => x.TaxCode).Index(27).Name("TaxCode__");
			Map(x => x.TaxRate).Index(28).Name("TaxRate__");
			Map(x => x.NonDeductibleTaxRate).Index(29).Name("NonDeductibleTaxRate__");
			Map(x => x.Receiver).Index(30).Name("Receiver__").TypeConverter(MaxLength(50));
			Map(x => x.CostType).Index(31).Name("CostType__");
			Map(x => x.IsLocalPO).Index(32).Name("IsLocalPO__");
			Map(x => x.IsCreatedInErp).Index(33).Name("IsCreatedInERP__");
			Map(x => x.Griv).Index(34).Name("GRIV__");
			Map(x => x.NoMoreInvoiceExpected).Index(35).Name("NoMoreInvoiceExpected__");
			Map(x => x.NoGoodsReceipt).Index(36).Name("NoGoodsReceipt__");
		}
	}
}
