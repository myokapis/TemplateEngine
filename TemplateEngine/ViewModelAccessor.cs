﻿/* ****************************************************************************
Copyright 2018-2020 Gene Graves

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
using TemplateEngine.Formats;

namespace TemplateEngine
{

    // TODO: maybe use Lazy<T> for loading field properties
    //       also consider whether this is thread safe

    /// <summary>
    /// Allows a data object's properties to be accessed by index or name.
    /// Also allows a data object's properties to be iterated.
    /// </summary>
    /// <typeparam name="T">Type of the data object</typeparam>
    public class ViewModelAccessor<T>
    {

        private static List<PropertyInfo> _fieldProperties = null;
        private static object[] _indexes = new object[] { };

        /// <summary>
        /// Creates a model accessor from a data object
        /// </summary>
        /// <param name="Model">The data object for which properties will be accessed</param>
        public ViewModelAccessor(T Model)
        {
            this.Model = Model;
        }

        /// <summary>
        /// Count of accessible property fields on the data object
        /// </summary>
        public int Count
        {
            get
            {
                return FieldProperties.Count;
            }
        }

        /// <summary>
        /// A collection of key-value pairs containing each field name and its corresponding value
        /// </summary>
        public IEnumerable<KeyValuePair<string, string>> FieldValues
        {
            get
            {
                foreach (PropertyInfo pi in FieldProperties)
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
                var pi = FieldProperties.ElementAt(fieldIndex);
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
                PropertyInfo pi = FieldProperties.Where(p => p.Name == fieldName).FirstOrDefault();
                return FormatValue(pi);
            }
        }

        #region "protected methods"

        /// <summary>
        /// A collection of <see cref="PropertyInfo"/> for public properties of this class
        /// </summary>
        protected static List<PropertyInfo> FieldProperties
        {
            get
            {
                if (_fieldProperties == null) GetFieldProperties();
                return _fieldProperties;
            }
        }

        /// <summary>
        /// Formats a property's value using a custom format attribute if one exists. Otherwise
        /// formatting uses the data type's default ToString() behavior.
        /// </summary>
        /// <param name="pi">The property whose value will be formatted</param>
        /// <returns>A formatted string representing the property's value</returns>
        protected string FormatValue(PropertyInfo pi)
        {
            object value = pi.GetValue(Model);
            if (value == null) return "";

            var formatter = (FormatAttribute)Attribute.GetCustomAttribute(pi, typeof(FormatAttribute));
            return formatter == null ? value.ToString() : formatter.FormatData(value);
        }

        /// <summary>
        /// A static method that finds public properties of the class. Any properties marked with
        /// a [NotMapped] attribute are ignored.
        /// </summary>
        protected static void GetFieldProperties()
        {
            _fieldProperties = new List<PropertyInfo>();

            foreach (PropertyInfo pi in typeof(T).GetProperties())
            {
                if (!Attribute.IsDefined(pi, typeof(NotMappedAttribute))) _fieldProperties.Add(pi);
            }
        }

        #endregion
    }
}
