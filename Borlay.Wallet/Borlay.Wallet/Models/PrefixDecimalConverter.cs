using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Borlay.Wallet.Models
{
    public class PrefixDecimalConverter : IValueConverter
    {
        private readonly IDictionary<int, string> prefixes = new Dictionary<int, string>();

        public PrefixDecimalConverter()
        {
            prefixes = new Dictionary<int, string>()
            {
                { 1, "K" },
                { 2, "M" },
                { 3, "G" },
                { 4, "T" },
                { 5, "P" },
                { 6, "E" },
                { 7, "Z" },
            };
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var decValue = (decimal)value;
            var abs = decValue < 0 ? -1 : 1;
            decValue = Math.Abs(decValue);
            var count = 0;

            while (decValue >= 1000)
            {
                decValue = Math.Truncate(decValue);
                decValue /= 1000;
                
                count++;
            }

            if (count == 0)
                return value;

            var prefix = prefixes[count];
            decValue *= abs;
            return $"{decValue} {prefix}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is string sValue)
            {
                var splited = sValue.Split(' ');
                var decValue = decimal.Parse(splited[0]);
                var prefix = splited.Length > 1 ? splited[1] : "";

                if(!string.IsNullOrWhiteSpace(prefix))
                {
                    var count = prefixes.Single(p => p.Value == prefix).Key;
                    for (int i = 0; i < count; i++)
                        decValue *= 1000;
                    return decValue;
                }
                return decValue;
            }
            else
                return value;
        }
    }
}
