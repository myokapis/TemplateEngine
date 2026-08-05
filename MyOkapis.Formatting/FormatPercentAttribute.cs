/* ****************************************************************************
Copyright 2018-2026 Gene Graves

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

using System.Globalization;

namespace MyOkapis.Formatting
{

    /// <summary>
    /// Attribute for formatting property data as a percentage string.
    /// </summary>
    public class FormatPercentAttribute : FormatAttribute
    {

        /// <summary>
        /// Percentage formatter constructor that accepts format parameters
        /// </summary>
        /// <param name="decimalPlaces">Number of decimal places to include in the result</param>
        /// <param name="negativePattern">Pattern to be used for representing a negative value</param>
        /// <param name="groupSeparator">Character to be used to delimit thousand groups</param>
        /// <param name="decimalSeparator">Character to be used as a decimal separator</param>
        public FormatPercentAttribute(int decimalPlaces = 2, int negativePattern = 0,
            string? groupSeparator = null, string? decimalSeparator = null)
        {
            var formatInfo = (NumberFormatInfo)CultureInfo.CurrentCulture.NumberFormat.Clone();
            FormatString = "P";

            // override number of decimal places shown
            if (decimalPlaces < 0) throw new ArgumentException("Number of decimal places must be greater than zero");
            formatInfo.PercentDecimalDigits = decimalPlaces;

            // change the negative number format
            if (negativePattern < 0 || negativePattern > 15) throw new ArgumentException("Invalid negative pattern");
            formatInfo.PercentNegativePattern = negativePattern;

            // change the group separator
            if (groupSeparator != null) formatInfo.PercentGroupSeparator = groupSeparator;

            // change the decimal separator
            if (decimalSeparator != null) formatInfo.PercentDecimalSeparator = decimalSeparator;

            FormatProvider = formatInfo;
        }

        /// <summary>
        /// Percentage format constructor that accepts a format string
        /// </summary>
        /// <param name="formatString">Percentage format string</param>
        public FormatPercentAttribute(string? formatString)
        {
            FormatProvider = (NumberFormatInfo)CultureInfo.CurrentCulture.NumberFormat.Clone();
            FormatString = formatString;
        }

        /// <summary>
        /// Percentage format constructor that accepts a number formatter
        /// </summary>
        /// <param name="formatInfo">A <see cref="NumberFormatInfo" /> to be used for formatting</param>
        /// <param name="formatString">Percentage format string</param>
        public FormatPercentAttribute(NumberFormatInfo formatInfo, string? formatString = null)
        {
            FormatProvider = formatInfo;
            FormatString = formatString ?? "P";
        }

    }

}
