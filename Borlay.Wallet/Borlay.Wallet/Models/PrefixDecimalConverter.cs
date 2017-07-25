using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
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
            if (value == null)
                return null;

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

            decValue *= abs;

            if (count == 0)
                return Math.Round(decValue, 3);

            var prefix = prefixes[count];
            return $"{decValue} {prefix}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (value is string sValue)
                {
                    if (string.IsNullOrEmpty(sValue))
                        return null;

                    var prefix = FindPrefix(sValue);
                    var decValue = decimal.Parse(prefix.Value);

                    for (int i = 0; i < prefix.Count; i++)
                        decValue *= 1000;
                    return decValue;
                }
                else
                    return value;
            }
            catch(Exception e)
            {
                return new ValidationResult(false, e.Message);
            }
        }

        private PrefixResult FindPrefix(string value)
        {
            value = value.ToUpper();
            foreach(var pr in prefixes)
            {
                if (value.Contains(pr.Value))
                {
                    return new PrefixResult()
                    {
                        Count = pr.Key,
                        Value = value.Replace(pr.Value, ""),
                    };
                }
            }

            return new PrefixResult()
            {
                Count = 0,
                Value = value
            };
        }
    }

    public class PrefixResult
    {
        public int Count { get; set; }
        public string Value { get; set; }
    }

    
}
