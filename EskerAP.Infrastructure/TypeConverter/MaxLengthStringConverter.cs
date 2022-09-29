using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

namespace EskerAP.Infrastructure.TypeConverter
{
    public class MaxLengthStringConverter : StringConverter, ITypeConverter
    {
        private readonly int _maxLength;

        public MaxLengthStringConverter(int maxLength)
        {
            _maxLength = maxLength;
        }

        public override string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData)
        {
            var convertedString = base.ConvertToString(value, row, memberMapData);

            if (convertedString.Length > _maxLength) return convertedString[0.._maxLength];

            return convertedString;
        }

    }
}
