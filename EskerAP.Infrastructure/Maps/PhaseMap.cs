using CsvHelper.Configuration;

namespace EskerAP.Infrastructure.Maps
{
	public class PhaseMap : ClassMap<Domain.Phase>
	{
		private static TypeConverter.MaxLengthStringConverter MaxLength(int maxLength) => new TypeConverter.MaxLengthStringConverter(maxLength);
		public PhaseMap()
		{
			Map(x => x.CompanyCode).Index(0).Name("CompanyCode__");
			Map(x => x.Id).Index(1).Name("Phase__");
			Map(x => x.Description).Index(2).Name("Description__");
		}
	}
}
