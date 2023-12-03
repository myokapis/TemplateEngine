/* ****************************************************************************
Copyright 2018-2023 Gene Graves

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

   http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
**************************************************************************** */

using System;
using System.Globalization;

namespace TemplateEngine.Formatters
{

    /// <summary>
    /// Attribute for formatting property data as a currency string.
    /// </summary>
    public class FormatCurrencyAttribute : FormatAttribute
    {

        /// <summary>
        /// Currency formatter constructor that accepts format parameters
        /// </summary>
        /// <param name="decimalPlaces">Number of decimal places to include in the result</param>
        /// <param name="negativePattern">Pattern to be used for representing a negative value</param>
        /// <param name="groupSeparator">Character to be used to delimit thousand groups</param>
        /// <param name="decimalSeparator">Character to be used as a decimal separator</param>
        public FormatCurrencyAttribute(int decimalPlaces = 2, int negativePattern = 0,
            string? groupSeparator = null, string? decimalSeparator = null)
        {
            FormatInfo = (NumberFormatInfo)Culture.NumberFormat.Clone();
            FormatString = "C";

            // override number of decimal places shown
            if (decimalPlaces < 0) throw new ArgumentException("Number of decimal places must be greater than zero");
            FormatInfo.CurrencyDecimalDigits = decimalPlaces;

            // override negative pattern
            if (negativePattern < 0 || negativePattern > 15) throw new ArgumentException("Invalid negative pattern");
            FormatInfo.CurrencyNegativePattern = negativePattern;

            // set format attributes
            if (groupSeparator != null) FormatInfo.CurrencyGroupSeparator = groupSeparator;
            if (decimalSeparator != null) FormatInfo.CurrencyDecimalSeparator = decimalSeparator;
        }

        /// <summary>
        /// Currency format constructor that accepts a format string
        /// </summary>
        /// <param name="formatString">Currency format string</param>
        public FormatCurrencyAttribute(string formatString)
        {
            // TODO: verify this or should it be the default numberformat object
            FormatInfo = (NumberFormatInfo)Culture.NumberFormat.Clone();
            FormatString = formatString;
        }

        /// <summary>
        /// Currency format constructor that accepts a number formatter
        /// </summary>
        /// <param name="formatInfo">A <see cref="NumberFormatInfo" /> to be used for formatting</param>
        public FormatCurrencyAttribute(NumberFormatInfo formatInfo)
        {
            FormatInfo = formatInfo;
            FormatString = "C";
        }

        private NumberFormatInfo FormatInfo { get; }

        private string FormatString { get; }

        /// <summary>
        /// Formats an object using the elements provided to this formatter's constructor
        /// </summary>
        /// <param name="data">Data to be formatted</param>
        /// <returns>Object data formatted as a currency string</returns>
        public override string FormatData(object? data)
        {
            if (data == null) return "";

            if (double.TryParse(data.ToString(), out var dbl))
            {
                return dbl.ToString(FormatString, FormatInfo);
            }

            return "";
        }

    }

}
