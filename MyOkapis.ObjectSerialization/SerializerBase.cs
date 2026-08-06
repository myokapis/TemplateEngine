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

using System.Text;
using MyOkapis.Formatting;
using MyOkapis.ObjectAccess;

namespace MyOkapis.ObjectSerialization
{

    /// <summary>
    /// An abstract class that defines the basic functionality of an object serializer.
    /// </summary>
    /// <typeparam name="TOptions">The specific options type this serializer accepts.</typeparam>
    public abstract class SerializerBase<TOptions> : ISerializer where TOptions : notnull, SerializationOptions
    {
        /// <summary>
        /// The formatter used to convert field values to text.
        /// </summary>
        protected readonly IFormatter formatter;

        /// <summary>
        /// Constructs an instance of the serializer.
        /// </summary>
        /// <param name="formatter">The formatter to be used when preparing field data.</param>
        protected SerializerBase(IFormatter formatter)
        {
            this.formatter = formatter;
        }

        /// <summary>
        /// Serializes an accessible object to a string.
        /// </summary>
        /// <param name="source">The object to be serialized.</param>
        /// <param name="options">Provides the options to be applied during serialization.</param>
        /// <returns>A string representing the original object.</returns>
        public abstract string Serialize(IAccessibleObject? source, TOptions options);

        /// <summary>
        /// Serializes an accessible object to a string.
        /// </summary>
        /// <param name="source">The object to be serialized.</param>
        /// <param name="builder">A string builder into which the serialized object is written.</param>
        /// <param name="options">Provides the options to be applied during serialization.</param>
        public abstract void Serialize(IAccessibleObject? source, StringBuilder builder, TOptions options);

        /// <inheritdoc />
        string ISerializer.Serialize(IAccessibleObject? source, SerializationOptions options) => Serialize(source, (TOptions)options);

        /// <inheritdoc />
        void ISerializer.Serialize(IAccessibleObject? source, StringBuilder builder, SerializationOptions options) => Serialize(source, builder, (TOptions)options);

        /// <summary>
        /// Returns a FieldContent object for a given field on the source object.
        /// </summary>
        /// <param name="source">The object being serialized.</param>
        /// <param name="field">The field descriptor associated with the object.</param>
        /// <returns>A FieldContent object containing the field's data and relevant metadata.</returns>
        protected virtual FieldContent ExtractContent(IAccessibleObject source, FieldDescriptor field)
        {
            return new FieldContent()
            {
                FieldName = field.Name,
                FormatAttribute = field.GetAttribute<IFormatAttribute>(),
                SerializationAttribute = field.GetAttribute<ISerializationAttribute>(),
                Value = field.PropertyInfo.GetValue(source)
            };
        }

        /// <summary>
        /// Formats a field's value, applying masking if requested. Width, justification, and
        /// truncation are already handled by <see cref="IFormatter"/> based on the field's
        /// FormatAttribute, so this does not need to apply them separately.
        /// </summary>
        /// <param name="fieldContent">A container for field metadata and the value of the field.</param>
        /// <param name="applyMasking">Indicates whether masking should be applied.</param>
        /// <returns>The field's formatted, optionally masked, text value.</returns>
        protected virtual string FormatField(FieldContent fieldContent, bool applyMasking)
        {
            var formattedValue = formatter.Format(fieldContent.Value, fieldContent.FormatAttribute);
            return applyMasking ? MaskValue(formattedValue, fieldContent) : formattedValue;
        }

        /// <summary>
        /// Masks a field's value if the field's SerializationAttribute requests it.
        /// </summary>
        /// <param name="value">The value to be masked.</param>
        /// <param name="fieldContent">A container for field metadata.</param>
        /// <returns>The masked value, or the original value if masking does not apply.</returns>
        protected virtual string MaskValue(string value, FieldContent fieldContent)
        {
            var attribute = fieldContent.SerializationAttribute;
            if (!(attribute?.Masked ?? false)) return value;

            return attribute.MaskChar == '\0'
                ? $"<{value.Length} character(s)>"
                : new string(attribute.MaskChar, value.Length);
        }
    }

}
