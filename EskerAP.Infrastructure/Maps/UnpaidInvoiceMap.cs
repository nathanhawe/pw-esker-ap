using CsvHelper.Configuration;

namespace EskerAP.Infrastructure.Maps
{
	public class UnpaidInvoiceMap : ClassMap<Domain.UnpaidInvoice>
	{
		public UnpaidInvoiceMap()
		{
			Map(x => x.VendorNumber).Index(0).Name("Vendor number");
			Map(x => x.InvoiceNumber).Index(1).Name("ERP Invoice number");
			Map(x => x.CompanyCode).Index(2).Name("Company code");
		}
	}
}
