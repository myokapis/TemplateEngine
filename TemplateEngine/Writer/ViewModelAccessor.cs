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
using TemplateEngine.Formatters;

namespace TemplateEngine.Writer
{

    // TODO: consider whether this is thread safe

    /// <summary>
    /// Allows a data object's properties to be accessed by index or name.
    /// Also allows a data object's properties to be iterated.
    /// </summary>
    /// <typeparam name="T">Type of the data object</typeparam>
    public class ViewModelAccessor<T>
    {
        private static object[] indexes = new object[] { };

        private static Lazy<PropertyInfo[]> fieldProperties =
            new Lazy<PropertyInfo[]>(GetFieldProperties, System.Threading.LazyThreadSafetyMode.ExecutionAndPublication);
        
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
        public int Count => fieldProperties.Value.Length;

        /// <summary>
        /// A collection of key-value pairs containing each field name and its corresponding value
        /// </summary>
        public IEnumerable<KeyValuePair<string, string>> FieldValues
        {
            get
            {
                foreach (PropertyInfo pi in fieldProperties.Value)
                {
                    yield return new KeyValuePair<string, string>(pi.Name, FormatValue(pi));
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
                var pi = fieldProperties.Value.ElementAt(fieldIndex);
                return FormatValue(pi);
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
                var pi = fieldProperties.Value.First(p => p.Name == fieldName);
                return FormatValue(pi);
            }
        }

        #region "protected methods"

        ///// <summary>
        ///// A collection of <see cref="PropertyInfo"/> for public properties of this class
        ///// </summary>
        //protected static PropertyInfo[] FieldProperties => fieldProperties.Value;

        /// <summary>
        /// Formats a property's value using a custom format attribute if one exists. Otherwise
        /// formatting uses the data type's default ToString() behavior.
        /// </summary>
        /// <param name="pi">The property whose value will be formatted</param>
        /// <returns>A formatted string representing the property's value</returns>
        protected string FormatValue(PropertyInfo pi)
        {
            object? value = pi.GetValue(Model);
            if (value == null) return "";

            FormatAttribute? formatter = (Attribute.GetCustomAttribute(pi, typeof(FormatAttribute)) as FormatAttribute);
            var formattedValue = (formatter == null) ? value.ToString() : formatter.FormatData(value);
            return formattedValue ?? "";
        }

        /// <summary>
        /// A static method that finds public properties of the class. Any properties marked with
        /// a [NotMapped] attribute are ignored.
        /// </summary>
        protected static PropertyInfo[] GetFieldProperties()
        {
            //fieldProperties = new List<PropertyInfo>();

            //foreach (PropertyInfo pi in typeof(T).GetProperties())
            //{
            //    if (!Attribute.IsDefined(pi, typeof(NotMappedAttribute))) fieldProperties.Add(pi);
            //}
            return typeof(T).GetProperties()
                .Where(pi => !Attribute.IsDefined(pi, typeof(NotMappedAttribute)))
                .ToArray();

        }

        #endregion
    }
}
