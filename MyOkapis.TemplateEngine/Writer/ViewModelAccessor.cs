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

using MyOkapis.Formatting;
using MyOkapis.ObjectAccess;

namespace MyOkapis.TemplateEngine.Writer
{

    /// <summary>
    /// Allows a data object's properties to be accessed by index or name, formatted according to
    /// any <see cref="IFormatAttribute"/> declared on the property. Also allows a data object's
    /// properties to be iterated.
    /// </summary>
    /// <remarks>
    /// This is a thin composition over <see cref="AccessibleObject{T}"/> (property discovery and
    /// caching) and <see cref="IFormatter"/> (value-to-string conversion) — it owns neither
    /// concern itself.
    /// </remarks>
    /// <typeparam name="T">Type of the data object</typeparam>
    public class ViewModelAccessor<T>
    {
        private readonly AccessibleObject<T> accessibleObject;
        private readonly IFormatter formatter;

        /// <summary>
        /// Creates a model accessor from a data object, using the default formatter.
        /// </summary>
        /// <param name="model">The data object for which properties will be accessed</param>
        public ViewModelAccessor(T model) : this(model, Formatter.Default) { }

        /// <summary>
        /// Creates a model accessor from a data object, using the supplied formatter.
        /// </summary>
        /// <param name="model">The data object for which properties will be accessed</param>
        /// <param name="formatter">The formatter to use when converting field values to strings</param>
        public ViewModelAccessor(T model, IFormatter formatter)
        {
            accessibleObject = new AccessibleObject<T>(model);
            this.formatter = formatter;
        }

        /// <summary>
        /// Count of accessible property fields on the data object
        /// </summary>
        public int Count => accessibleObject.Count;

        /// <summary>
        /// A collection of key-value pairs containing each field name and its corresponding
        /// formatted value
        /// </summary>
        public IEnumerable<KeyValuePair<string, string>> FieldValues
        {
            get
            {
                foreach (var (descriptor, value) in accessibleObject.Values)
                {
                    yield return new KeyValuePair<string, string>(descriptor.Name, FormatValue(descriptor, value));
                }
            }
        }

        /// <summary>
        /// The data object for which properties will be accessed
        /// </summary>
        public T Model => accessibleObject.Model;

        /// <summary>
        /// Returns a formatted field value based on a field index
        /// </summary>
        /// <param name="fieldIndex">The index of the field to be accessed</param>
        /// <returns>A string representing the value of the field</returns>
        public string this[int fieldIndex]
        {
            get
            {
                var descriptor = AccessibleObject<T>.Fields[fieldIndex];
                return FormatValue(descriptor, accessibleObject.GetValue(descriptor));
            }
        }

        /// <summary>
        /// Returns a formatted field value based on a field name
        /// </summary>
        /// <param name="fieldName">The name of the field to be accessed</param>
        /// <returns>A string representing the value of the field</returns>
        public string this[string fieldName]
        {
            get
            {
                var descriptor = AccessibleObject<T>.Fields.First(f => f.Name == fieldName);
                return FormatValue(descriptor, accessibleObject.GetValue(descriptor));
            }
        }

        /// <summary>
        /// Formats a field's value using its format attribute, if one exists. Otherwise formatting
        /// uses the data type's default ToString() behavior.
        /// </summary>
        /// <param name="field">The field descriptor whose value will be formatted</param>
        /// <param name="value">The raw field value</param>
        /// <returns>A formatted string representing the field's value</returns>
        protected string FormatValue(FieldDescriptor field, object? value)
        {
            var formatAttribute = field.GetAttribute<IFormatAttribute>();
            return formatter.Format(value, formatAttribute);
        }
    }
}
