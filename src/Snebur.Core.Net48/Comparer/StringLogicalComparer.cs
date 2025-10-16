using System;
using System.Collections.Generic;
using System.Globalization;

namespace Snebur.Comparer
{
    public class StringLogicalComparer : IComparer<string>
    {
        private readonly NumberFormatInfo defaultNumberFormat = NumberFormatInfo.InvariantInfo;
        private readonly string decimalSeparator = NumberFormatInfo.InvariantInfo.NumberDecimalSeparator;

        public StringLogicalComparer()
        {
            this.IgnoreCase = false;
        }

        public StringLogicalComparer(bool ignoreCase, bool floatNumbers)
        {
            this.IgnoreCase = ignoreCase;
            this.FloatNumbers = floatNumbers;
        }

        public bool IgnoreCase { get; private set; }
        public bool FloatNumbers { get; private set; }

        public int Compare(string x, string y)
        {
            int count = Math.Min(x.Length, y.Length);

            for (int i = 0; i < count; i++)
            {
                char xChar = x[i];
                if (this.IgnoreCase)
                    xChar = char.ToLower(xChar);

                char yChar = y[i];
                if (this.IgnoreCase)
                    yChar = char.ToLower(yChar);

                if (char.IsDigit(xChar) && char.IsDigit(yChar))
                {
                    String xsDigits = this.GetAllDigits(x, i);
                    String ysDigits = this.GetAllDigits(y, i);

                    double xNumber = Convert.ToDouble(xsDigits, this.defaultNumberFormat);
                    double yNumber = Convert.ToDouble(ysDigits, this.defaultNumberFormat);

                    int result = xNumber.CompareTo(yNumber);
                    if (result != 0)
                    {
                        return result;
                    }
                    i += Math.Max(xsDigits.Length, ysDigits.Length);
                }
                else
                {
                    int result = xChar.CompareTo(yChar);
                    if (result != 0)
                    {
                        return result;
                    }
                }
            }
            return 0;
        }

        private String GetAllDigits(String text, int startIndex)
        {
            String result = String.Empty;
            for (int i = startIndex; i < text.Length; i++)
            {
                if (!char.IsDigit(text[i]))
                {
                    if (!this.FloatNumbers || Convert.ToString(text[i]) != this.decimalSeparator)
                    {
                        break;
                    }
                }
                result += text[i];
            }
            if (result.EndsWith(this.decimalSeparator))
            {
                result = result.Substring(0, result.Length - 1);
            }
            return result;
        }
    }
}