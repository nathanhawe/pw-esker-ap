using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

namespace EskerAP.Infrastructure.TypeConverter
{
    public class EskerBooleanConverter : DefaultTypeConverter
    {
        public override string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData)
        {
            if (value == null) return "";

            return (bool)value ? "yes" : "no";
        }
    }
}
