﻿/* ****************************************************************************
Copyright 2018-2022 Gene Graves

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
    /// Attribute for formatting property data as a numeric string.
    /// </summary>
    public class FormatNumberAttribute : FormatAttribute
    {

        /// <summary>
        /// Numeric formatter constructor that accepts format parameters
        /// </summary>
        /// <param name="decimalPlaces">Number of decimal places to include in the result</param>
        /// <param name="negativePattern">Pattern to be used for representing a negative value</param>
        /// <param name="groupSeparator">Character to be used to delimit thousand groups</param>
        /// <param name="decimalSeparator">Character to be used as a decimal separator</param>
        public FormatNumberAttribute(int decimalPlaces = 2, int negativePattern = 0,
            string groupSeparator = null, string decimalSeparator = null)
        {
            FormatInfo = (NumberFormatInfo)Culture.NumberFormat.Clone();
            FormatString = "N";

            // override number of decimal places shown
            if (decimalPlaces < 0) throw new ArgumentException("Number of decimal places must be greater than zero");
            FormatInfo.NumberDecimalDigits = decimalPlaces;

            // change the negative sign pattern
            if (negativePattern < 0 || negativePattern > 15) throw new ArgumentException("Invalid negative pattern");
            FormatInfo.NumberNegativePattern = negativePattern;

            // change the group separator
            if (groupSeparator != null) FormatInfo.NumberGroupSeparator = groupSeparator;

            // change the decimal separator
            if (decimalSeparator != null) FormatInfo.NumberDecimalSeparator = decimalSeparator;
        }

        /// <summary>
        /// Numeric format constructor that accepts a format string
        /// </summary>
        /// <param name="formatString">Numeric format string</param>
        public FormatNumberAttribute(string formatString)
        {
            FormatString = formatString;
        }

        /// <summary>
        /// Numeric format constructor that accepts a number formatter
        /// </summary>
        /// <param name="formatInfo">A <see cref="NumberFormatInfo" /> to be used for formatting</param>
        public FormatNumberAttribute(NumberFormatInfo formatInfo)
        {
            FormatInfo = formatInfo;
            FormatString = "N";
        }

        private NumberFormatInfo FormatInfo { get; }

        private string FormatString { get; set; }

        /// <summary>
        /// Formats an object using the elements provided to this formatter's constructor
        /// </summary>
        /// <param name="data">Data to be formatted</param>
        /// <returns>Object data formatted as a numeric string</returns>
        public override string FormatData(object data)
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