/* ****************************************************************************
Copyright 2018 Gene Graves

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
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using TemplateEngine.Formats;

namespace TemplateEngine
{
    // TODO: maybe use Lazy<T> for loading field properties
    public class ViewModelAccessor<T>
    {

        private static List<PropertyInfo> _fieldProperties = null;
        private static object[] _indexes = new object[] { };
        private T _model;

        public ViewModelAccessor(T Model)
        {
            _model = Model;
        }

        public int Count
        {
            get
            {
                return FieldProperties.Count;
            }
        }

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

        public T Model
        {
            get
            {
                return _model;
            }
        }

        // return the value of the field at the specified index
        public string this[int fieldIndex]
        {
            get
            {
                var pi = FieldProperties.ElementAt(fieldIndex);
                return FormatValue(pi);
            }
        }

        // return the field value for the specified field name
        public string this[string fieldName]
        {
            get
            {
                PropertyInfo pi = FieldProperties.Where(p => p.Name == fieldName).FirstOrDefault();
                return FormatValue(pi);
            }
        }

        #region "protected methods"

        protected static List<PropertyInfo> FieldProperties
        {
            get
            {
                if (_fieldProperties == null) GetFieldProperties();
                return _fieldProperties;
            }
        }

        protected string FormatValue(PropertyInfo pi)
        {
            object value = pi.GetValue(_model);
            if (value == null) return "";

            var formatter = (FormatAttribute)Attribute.GetCustomAttribute(pi, typeof(FormatAttribute));
            return formatter == null ? value.ToString() : formatter.FormatData(value);
        }

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
