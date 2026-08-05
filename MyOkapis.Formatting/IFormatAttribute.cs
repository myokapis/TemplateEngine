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

namespace MyOkapis.Formatting
{

    /// <summary>
    /// Describes how a value should be converted to text. Implementations carry metadata only —
    /// the conversion itself is performed by an <see cref="IFormatter"/>.
    /// </summary>
    public interface IFormatAttribute
    {
        /// <summary>
        /// The standard or custom .Net format string to apply, e.g. "C2", "yyyy-MM-dd", "N0".
        /// </summary>
        string? FormatString { get; }

        /// <summary>
        /// The format provider (e.g. a <see cref="System.Globalization.NumberFormatInfo"/> or
        /// <see cref="System.Globalization.DateTimeFormatInfo"/>) to apply. When null, the default
        /// <see cref="Formatter"/> falls back to <see cref="System.Globalization.CultureInfo.CurrentCulture"/>,
        /// matching a plain, unformatted ToString() call.
        /// </summary>
        IFormatProvider? FormatProvider { get; }

        /// <summary>
        /// The minimum length to which the formatted value should be padded. Zero means no minimum.
        /// </summary>
        int MinLength { get; }

        /// <summary>
        /// The maximum length to which the formatted value should be truncated. Zero means no maximum.
        /// </summary>
        int MaxLength { get; }

        /// <summary>
        /// The justification to apply when padding is required.
        /// </summary>
        Justify Justify { get; }

        /// <summary>
        /// The character used to pad the formatted value when it is shorter than MinLength.
        /// </summary>
        char PaddingChar { get; }
    }

}
