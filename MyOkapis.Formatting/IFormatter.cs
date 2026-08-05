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
    /// Converts a value into its formatted text representation. Implementations have no knowledge
    /// of where the value came from (reflection, a direct call, etc.) — only the value and an
    /// optional <see cref="IFormatAttribute"/> describing how it should be rendered.
    /// </summary>
    public interface IFormatter
    {
        /// <summary>
        /// Formats a value, dispatching to the appropriate conversion based on its runtime type.
        /// </summary>
        /// <param name="value">The value to be formatted</param>
        /// <param name="attribute">Optional metadata controlling the format string, provider, and padding</param>
        /// <returns>A formatted string representing the value, or "" if the value is null</returns>
        string Format(object? value, IFormatAttribute? attribute);

        /// <summary>
        /// Formats a date/time value to a string.
        /// </summary>
        /// <param name="value">The value to be formatted</param>
        /// <param name="attribute">Optional metadata controlling the format string, provider, and padding</param>
        /// <returns>A formatted string representing the value</returns>
        string Format(DateTime value, IFormatAttribute? attribute);

        /// <summary>
        /// Formats a decimal value to a string.
        /// </summary>
        /// <param name="value">The value to be formatted</param>
        /// <param name="attribute">Optional metadata controlling the format string, provider, and padding</param>
        /// <returns>A formatted string representing the value</returns>
        string Format(decimal value, IFormatAttribute? attribute);

        /// <summary>
        /// Formats an integer value to a string.
        /// </summary>
        /// <param name="value">The value to be formatted</param>
        /// <param name="attribute">Optional metadata controlling the format string, provider, and padding</param>
        /// <returns>A formatted string representing the value</returns>
        string Format(int value, IFormatAttribute? attribute);

        /// <summary>
        /// Formats a string value, applying the attribute's format string as a composite format
        /// template if one is present.
        /// </summary>
        /// <param name="value">The value to be formatted</param>
        /// <param name="attribute">Optional metadata controlling the format string, provider, and padding</param>
        /// <returns>A formatted string representing the value, or "" if the value is null</returns>
        string Format(string? value, IFormatAttribute? attribute);
    }

}
