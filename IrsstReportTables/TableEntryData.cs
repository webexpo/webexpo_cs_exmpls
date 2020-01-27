using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace IrsstReportTables
{
    abstract public class TableEntryData
    {
        protected NumberFormatInfo nfi;

        public TableEntryData()
        {
            this.nfi = new NumberFormatInfo();
            this.nfi.NumberDecimalSeparator = ".";
        }

        public abstract override string ToString();

        public string ToSignificantDigits(double value, int significant_digits = 3)
        {
            // Use G format to get significant digits.
            // Then convert to double and use F format.
            string format1 = "{0:G" + significant_digits.ToString() + "}";
            string result = Convert.ToDouble(
                String.Format(format1, value)).ToString("F99");

            // Rmove trailing 0s.
            result = result.TrimEnd('0');

            // Rmove the decimal point and leading 0s,
            // leaving just the digits.
            string test = result.Replace(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator, "");//.TrimStart('0');

            // See if we have enough significant digits.
            if (significant_digits > test.Length)
            {
                // Add trailing 0s.
                result += new string('0', significant_digits - test.Length);
            }
            else
            {
                // See if we should remove the trailing decimal point.
                if ((significant_digits <= test.Length) &&
                    result.EndsWith(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator))
                    result = result.Substring(0, result.Length - 1);
            }

            result = test == "0" ? test : result.Replace(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator, this.nfi.NumberDecimalSeparator);
            return result;
        }


    }
}
