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
    /// Default, stateless implementation of <see cref="IFormatter"/>. Safe to share as a singleton.
    /// </summary>
    public sealed class Formatter : IFormatter
    {
        /// <summary>
        /// A shared, stateless default instance for use where a formatter has not been supplied via DI.
        /// </summary>
        public static IFormatter Default { get; } = new Formatter();

        /// <inheritdoc />
        public string Format(object? value, IFormatAttribute? attribute)
        {
            if (value is null) return "";

            var formatted = value switch
            {
                string s => FormatCompositeString(s, attribute),
                IFormattable formattable => formattable.ToString(attribute?.FormatString, attribute?.FormatProvider ?? CultureInfo.CurrentCulture),
                _ => value.ToString() ?? ""
            };

            return ApplyWidth(formatted, attribute);
        }

        /// <inheritdoc />
        public string Format(DateTime value, IFormatAttribute? attribute) => Format((object)value, attribute);

        /// <inheritdoc />
        public string Format(decimal value, IFormatAttribute? attribute) => Format((object)value, attribute);

        /// <inheritdoc />
        public string Format(int value, IFormatAttribute? attribute) => Format((object)value, attribute);

        /// <inheritdoc />
        public string Format(string? value, IFormatAttribute? attribute) => value is null ? "" : Format((object)value, attribute);

        /// <summary>
        /// Applies a format string as a composite format template (e.g. "[{0}]") to a string value.
        /// Returns the value unchanged if no format string is present.
        /// </summary>
        private static string FormatCompositeString(string value, IFormatAttribute? attribute)
        {
            if (string.IsNullOrEmpty(attribute?.FormatString)) return value;
            return string.Format(attribute.FormatProvider, attribute.FormatString, value);
        }

        /// <summary>
        /// Truncates to MaxLength and pads to MinLength using the attribute's justification and
        /// padding character. A no-op when no attribute, or no length constraints, are present.
        /// </summary>
        private static string ApplyWidth(string value, IFormatAttribute? attribute)
        {
            if (attribute is null) return value;

            // truncate the string if necessary
            var workingValue = (attribute.MaxLength > 0) && (value.Length > attribute.MaxLength)
                ? value[..attribute.MaxLength]
                : value;

            // padding only applies when a min length has been set and the data does not already fit
            if (attribute.MinLength == 0 || workingValue.Length >= attribute.MinLength)
                return workingValue;

            if (attribute.Justify != Justify.Center)
                return attribute.Justify == Justify.Left
                    ? workingValue.PadRight(attribute.MinLength, attribute.PaddingChar)
                    : workingValue.PadLeft(attribute.MinLength, attribute.PaddingChar);

            // calculate left and right padding to apply for center justification
            var padChars = attribute.MinLength - workingValue.Length;
            var leftPadding = new string(attribute.PaddingChar, padChars / 2);
            var rightPadding = new string(attribute.PaddingChar, padChars - leftPadding.Length);

            return $"{leftPadding}{workingValue}{rightPadding}";
        }
    }

}
