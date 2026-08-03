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

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Threading;
using MyOkapis.TemplateEngine.Formatters;

namespace MyOkapis.TemplateEngine.Writer
{

    // TODO: consider whether this is thread safe

    /// <summary>
    /// Allows a data object's properties to be accessed by index or name.
    /// Also allows a data object's properties to be iterated.
    /// </summary>
    /// <typeparam name="T">Type of the data object</typeparam>
    public class ViewModelAccessor<T>
    {
        private static readonly Lazy<FieldAccessor[]> fieldAccessors =
            new Lazy<FieldAccessor[]>(GetFieldAccessors, LazyThreadSafetyMode.ExecutionAndPublication);

        /// <summary>
        /// Creates a model accessor from a data object
        /// </summary>
        /// <param name="model">The data object for which properties will be accessed</param>
        public ViewModelAccessor(T model)
        {
            Model = model;
        }

        /// <summary>
        /// Count of accessible property fields on the data object
        /// </summary>
        public int Count => fieldAccessors.Value.Length;

        /// <summary>
        /// A collection of key-value pairs containing each field name and its corresponding value
        /// </summary>
        public IEnumerable<KeyValuePair<string, string>> FieldValues
        {
            get
            {
                foreach (var accessor in fieldAccessors.Value)
                {
                    yield return new KeyValuePair<string, string>(accessor.PropertyInfo.Name, FormatValue(accessor));
                }
            }
        }

        /// <summary>
        /// The data object for which properties will be accessed
        /// </summary>
        public T Model { get; }

        /// <summary>
        /// Returns a field value based on a field index
        /// </summary>
        /// <param name="fieldIndex">The index of the field to be accessed</param>
        /// <returns>A string representing the value of the field</returns>
        public string this[int fieldIndex]
        {
            get
            {
                var accessor = fieldAccessors.Value.ElementAt(fieldIndex);
                return FormatValue(accessor);
            }
        }

        /// <summary>
        /// Returns a field value based on a field name
        /// </summary>
        /// <param name="fieldName">The name of the field to be accessed</param>
        /// <returns>A string representing the value of the field</returns>
        public string this[string fieldName]
        {
            get
            {
                var accessor = fieldAccessors.Value.First(a => a.PropertyInfo.Name == fieldName);
                return FormatValue(accessor);
            }
        }

        #region "protected methods"

        /// <summary>
        /// Formats a property's value using its cached format attribute if one exists. Otherwise
        /// formatting uses the data type's default ToString() behavior.
        /// </summary>
        /// <param name="accessor">The field accessor whose value will be formatted</param>
        /// <returns>A formatted string representing the property's value</returns>
        protected string FormatValue(in FieldAccessor accessor)
        {
            object? value = accessor.PropertyInfo.GetValue(Model);
            if (value == null) return "";

            var formattedValue = accessor.FormatAttribute?.FormatData(value) ?? value.ToString();
            return formattedValue ?? "";
        }

        /// <summary>
        /// A static method that finds public properties of the class and caches each property's
        /// format attribute alongside it, avoiding a repeated reflection lookup on every field access.
        /// Any properties marked with a [NotMapped] attribute are ignored.
        /// </summary>
        protected static FieldAccessor[] GetFieldAccessors()
        {
            return typeof(T).GetProperties()
                .Where(pi => !Attribute.IsDefined(pi, typeof(NotMappedAttribute)))
                .Select(pi => new FieldAccessor
                {
                    PropertyInfo = pi,
                    FormatAttribute = Attribute.GetCustomAttribute(pi, typeof(FormatAttribute)) as FormatAttribute
                })
                .ToArray();
        }

        /// <summary>
        /// Caches a property together with its resolved format attribute, if any.
        /// </summary>
        protected readonly struct FieldAccessor
        {
            /// <summary>
            /// The reflected property info for the property.
            /// </summary>
            public required PropertyInfo PropertyInfo { get; init; }

            /// <summary>
            /// The format attribute attached to the property, if any.
            /// </summary>
            public FormatAttribute? FormatAttribute { get; init; }
        }

        #endregion
    }
}
