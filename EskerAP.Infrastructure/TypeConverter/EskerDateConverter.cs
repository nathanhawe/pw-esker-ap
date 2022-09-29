using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using System;

namespace EskerAP.Infrastructure.TypeConverter
{
    public class EskerDateConverter : DefaultTypeConverter
    {
        public override string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData)
        {
            if (value == null) return "";

            return ((DateTime)value).ToString("yyyy-MM-dd");
        }
    }
}
