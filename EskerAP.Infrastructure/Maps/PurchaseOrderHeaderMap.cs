using CsvHelper.Configuration;

namespace EskerAP.Infrastructure.Maps
{
	public class PurchaseOrderHeaderMap : ClassMap<Domain.Header>
	{
		private static TypeConverter.MaxLengthStringConverter MaxLength(int maxLength) => new TypeConverter.MaxLengthStringConverter(maxLength);

		public PurchaseOrderHeaderMap()
		{
			Map(x => x.CompanyCode).Index(0).Name("CompanyCode__");
			Map(x => x.VendorNumber).Index(1).Name("VendorNumber__");
			Map(x => x.DifferentInvoicingParty).Index(2).Name("DifferentInvoicingParty__");
			Map(x => x.OrderNumber).Index(3).Name("OrderNumber__");
			Map(x => x.OrderDate).Index(4).Name("OrderDate__");
			Map(x => x.OrderedAmount).Index(5).Name("OrderedAmount__");
			Map(x => x.DeliveredAmount).Index(6).Name("DeliveredAmount__");
			Map(x => x.InvoicedAmount).Index(7).Name("InvoicedAmount__");
			Map(x => x.Currency).Index(8).Name("Currency__");
			Map(x => x.Buyer).Index(9).Name("Buyer__");
			Map(x => x.Receiver).Index(10).Name("Receiver__");
			Map(x => x.IsLocalPO).Index(11).Name("IsLocalPO__");
			Map(x => x.IsCreatedInErp).Index(12).Name("IsCreatedInERP__");
			Map(x => x.NoMoreInvoiceExpected).Index(13).Name("NoMoreInvoiceExpected__");
		}
	}
}
