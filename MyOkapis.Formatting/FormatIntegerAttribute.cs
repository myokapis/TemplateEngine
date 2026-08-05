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
    /// Attribute for formatting property data as an integer string.
    /// </summary>
    public class FormatIntegerAttribute : FormatAttribute
    {

        /// <summary>
        /// Integer formatter constructor that accepts format parameters
        /// </summary>
        /// <param name="negativePattern">Pattern to be used for representing a negative value</param>
        /// <param name="groupSeparator">Character to be used to delimit thousand groups</param>
        public FormatIntegerAttribute(int negativePattern = 0, string? groupSeparator = null)
        {
            var formatInfo = (NumberFormatInfo)CultureInfo.CurrentCulture.NumberFormat.Clone();
            FormatString = "N0";

            // set the negative pattern
            if (negativePattern < 0 || negativePattern > 15) throw new ArgumentException("Invalid negative pattern");
            formatInfo.NumberNegativePattern = negativePattern;

            // set the group separators
            if (groupSeparator != null) formatInfo.NumberGroupSeparator = groupSeparator;

            FormatProvider = formatInfo;
        }

        /// <summary>
        /// Integer format constructor that accepts a format string
        /// </summary>
        /// <param name="formatString">Integer format string</param>
        public FormatIntegerAttribute(string? formatString)
        {
            FormatProvider = (NumberFormatInfo)CultureInfo.CurrentCulture.NumberFormat.Clone();
            FormatString = formatString;
        }

        /// <summary>
        /// Integer format constructor that accepts a number formatter
        /// </summary>
        /// <param name="formatInfo">A <see cref="NumberFormatInfo" /> to be used for formatting</param>
        public FormatIntegerAttribute(NumberFormatInfo formatInfo)
        {
            FormatProvider = formatInfo;
            FormatString = "N0";
        }

    }

}
