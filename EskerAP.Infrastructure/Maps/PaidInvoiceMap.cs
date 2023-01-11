using CsvHelper.Configuration;

namespace EskerAP.Infrastructure.Maps
{
	public class PaidInvoiceMap : ClassMap<Domain.PaidInvoice>
	{
		public PaidInvoiceMap()
		{
			Map(x => x.CompanyCode).Index(0).Name("Company code");
			Map(x => x.VendorNumber).Index(1).Name("Vendor number");
			Map(x => x.InvoiceNumber).Index(2).Name("Invoice number");
			Map(x => x.PaymentDate).Index(3).Name("Payment date");
			Map(x => x.PaymentMethod).Index(4).Name("Payment method");
			Map(x => x.PaymentReference).Index(5).Name("Payment reference");
		}
	}
}
