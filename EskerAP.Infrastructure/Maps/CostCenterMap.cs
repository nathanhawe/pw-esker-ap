using CsvHelper.Configuration;

namespace EskerAP.Infrastructure.Maps
{
	public class CostCenterMap : ClassMap<Domain.CostCenter>
	{
		public CostCenterMap()
		{
			Map(x => x.CompanyCode).Index(0).Name("CompanyCode__");
			Map(x => x.Id).Index(1).Name("CostCenter__");
			Map(x => x.Description).Index(2).Name("Description__").TypeConverter(new TypeConverter.MaxLengthStringConverter(50));
			Map(x => x.Manager).Index(3).Name("Manager__");
		}
	}
}
