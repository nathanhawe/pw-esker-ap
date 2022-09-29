using CsvHelper.Configuration;

namespace EskerAP.Infrastructure.Maps
{
	public class VendorMap : ClassMap<Domain.Vendor>
	{
		private static TypeConverter.MaxLengthStringConverter MaxLength(int maxLength) => new TypeConverter.MaxLengthStringConverter(maxLength);
		public VendorMap()
		{
			Map(x => x.CompanyCode).Index(0).Name("CompanyCode__");
			Map(x => x.Id).Index(1).Name("Number__");
			Map(x => x.Name).Index(2).Name("Name__").TypeConverter(MaxLength(35));
			Map(x => x.PhoneNumber).Index(3).Name("PhoneNumber__");
			Map(x => x.FaxNumber).Index(4).Name("FaxNumber__");
			Map(x => x.VATNumber).Index(5).Name("VATNumber__");
			Map(x => x.DUNSNumber).Index(6).Name("DUNSNumber__");
			Map(x => x.PreferredInvoiceType).Index(7).Name("PreferredInvoiceType__");
			Map(x => x.PaymentTermsCode).Index(8).Name("PaymentTermCode__");
			Map(x => x.Email).Index(9).Name("Email__");
			Map(x => x.GeneralAccount).Index(10).Name("GeneralAccount__");
			Map(x => x.TaxSystem).Index(11).Name("TaxSystem__");
			Map(x => x.Currency).Index(12).Name("Currency__");
			Map(x => x.ParafiscalTax).Index(13).Name("ParafiscalTax__");
			Map(x => x.SupplierDue).Index(14).Name("SupplierDue__");
			Map(x => x.Sub).Index(15).Name("Sub__");
			Map(x => x.Street).Index(16).Name("Street__");
			Map(x => x.PostOfficeBox).Index(17).Name("PostOfficeBox__");
			Map(x => x.City).Index(18).Name("City__");
			Map(x => x.PostalCode).Index(19).Name("PostalCode__");
			Map(x => x.Region).Index(20).Name("Region__");
			Map(x => x.Country).Index(21).Name("Country__");
		}
	}
}
