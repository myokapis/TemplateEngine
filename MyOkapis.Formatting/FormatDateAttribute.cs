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
    /// Attribute for formatting property data as a date string.
    /// </summary>
    public class FormatDateAttribute : FormatAttribute
    {

        /// <summary>
        /// Date format constructor that accepts a format string
        /// </summary>
        /// <param name="formatString">Date format string</param>
        public FormatDateAttribute(string? formatString)
        {
            FormatProvider = (DateTimeFormatInfo)CultureInfo.CurrentCulture.DateTimeFormat.Clone();
            FormatString = formatString ?? "G";
        }

        /// <summary>
        /// Date format constructor that accepts a date formatter
        /// </summary>
        /// <param name="formatInfo">A <see cref="DateTimeFormatInfo" /> to be used for formatting</param>
        /// <param name="formatString">An optional string defining the output format</param>
        public FormatDateAttribute(DateTimeFormatInfo formatInfo, string? formatString = null)
        {
            FormatProvider = formatInfo;
            FormatString = formatString ?? "G";
        }

    }

}
