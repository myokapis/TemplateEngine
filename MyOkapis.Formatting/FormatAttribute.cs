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
    /// Abstract base for attributes that describe how a property's value should be formatted.
    /// Carries metadata only; use an <see cref="IFormatter"/> to actually perform the conversion.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public abstract class FormatAttribute : Attribute, IFormatAttribute
    {
        private int? minLength;
        private int? maxLength;

        /// <summary>
        /// Shortcut for setting MinLength and MaxLength to the same value.
        /// </summary>
        /// <remarks>
        /// Do not write code that uses the `get` value; the getter only exists because it is
        /// required as an attribute property.
        /// </remarks>
        public virtual int FixedLength
        {
            get => MaxLength == MinLength ? MaxLength : 0;
            set
            {
                if (maxLength.HasValue && maxLength.Value != value)
                    throw new InvalidOperationException("Fixed length cannot be set if the max length is already set to a different value.");

                if (minLength.HasValue && minLength.Value != value)
                    throw new InvalidOperationException("Fixed length cannot be set if the min length is already set to a different value.");

                MaxLength = value;
                MinLength = value;
            }
        }

        /// <inheritdoc />
        public virtual string? FormatString { get; protected set; }

        /// <inheritdoc />
        public virtual IFormatProvider? FormatProvider { get; protected set; }

        /// <inheritdoc />
        public virtual Justify Justify { get; init; } = Justify.Left;

        /// <inheritdoc />
        public virtual int MaxLength
        {
            get => maxLength ?? 0;
            set
            {
                if (maxLength.HasValue && maxLength.Value != value)
                    throw new InvalidOperationException("Max length has already been set.");

                if (minLength.HasValue && value < minLength.Value)
                    throw new InvalidOperationException("Max length cannot be less than min length.");

                maxLength = value;
            }
        }

        /// <inheritdoc />
        public virtual int MinLength
        {
            get => minLength ?? 0;
            set
            {
                if (minLength.HasValue && minLength.Value != value)
                    throw new InvalidOperationException("Min length has already been set.");

                if (maxLength.HasValue && value > maxLength.Value)
                    throw new InvalidOperationException("Min length cannot be greater than max length.");

                minLength = value;
            }
        }

        /// <inheritdoc />
        public virtual char PaddingChar { get; init; } = ' ';
    }

}
