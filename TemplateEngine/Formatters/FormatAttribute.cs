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
using System.Globalization;

namespace TemplateEngine.Formatters
{

    /// <summary>
    /// Abstract class for building format attributes that can be applied to properties to
    /// define the way the property data should be represented as text.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public abstract class FormatAttribute : Attribute
    {
        /// <summary>
        /// Abstract method to convert a value into an appropriately formatted string
        /// </summary>
        /// <param name="data">Value to be formatted</param>
        /// <returns>Data formatted as a string</returns>
        public abstract string FormatData(object data);

        /// <summary>
        /// Shorthand reference to the current culture
        /// </summary>
        public CultureInfo Culture => CultureInfo.CurrentCulture;
    }

}
