using CsvHelper.Configuration;

namespace EskerAP.Infrastructure.Maps
{
	public class GLAccountMap : ClassMap<Domain.GLAccount>
	{
		private static TypeConverter.MaxLengthStringConverter MaxLength(int maxLength) => new TypeConverter.MaxLengthStringConverter(maxLength);
		public GLAccountMap()
		{
			Map(x => x.CompanyCode).Index(0).Name("CompanyCode__");
			Map(x => x.Account).Index(1).Name("Account__");
			Map(x => x.Description).Index(2).Name("Description__").TypeConverter(MaxLength(256));
			Map(x => x.Group).Index(3).Name("Group__");
			Map(x => x.Allocable1).Index(4).Name("Allocable1__");
			Map(x => x.Allocable2).Index(5).Name("Allocable2__");
			Map(x => x.Allocable3).Index(6).Name("Allocable3__");
			Map(x => x.Allocable4).Index(7).Name("Allocable4__");
			Map(x => x.Allocable5).Index(8).Name("Allocable5__");
			Map(x => x.Manager).Index(9).Name("Manager__");			
		}
	}
}
