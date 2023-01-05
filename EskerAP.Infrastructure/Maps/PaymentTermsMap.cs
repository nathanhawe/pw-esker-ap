using CsvHelper.Configuration;

namespace EskerAP.Infrastructure.Maps
{
	public class PaymentTermsMap : ClassMap<Domain.PaymentTerm>
	{
		public PaymentTermsMap()
		{
			Map(x => x.CompanyCode).Index(0).Name("CompanyCode__");
			Map(x => x.PaymentTermCode).Index(1).Name("PaymentTermCode__");
			Map(x => x.Description).Index(2).Name("Description__");
			Map(x => x.DayLimit).Index(3).Name("DayLimit__");
			Map(x => x.LatePaymentFeeRate).Index(4).Name("LatePaymentFeeRate__");
			Map(x => x.ReferenceDate).Index(5).Name("ReferenceDate__");
			Map(x => x.PaymentDay).Index(6).Name("PaymentDay__");
			Map(x => x.EndOfMonth).Index(7).Name("EndOfMonth__");
			Map(x => x.EnableDynamicDiscounting).Index(8).Name("EnableDynamicDiscounting__");
			Map(x => x.DiscountRate30Days).Index(9).Name("DiscountRate30Days__");
			Map(x => x.DiscountPeriod).Index(10).Name("DiscountPeriod__");
			Map(x => x.DiscountRate).Index(11).Name("DiscountRate__");
		}
	}
}
