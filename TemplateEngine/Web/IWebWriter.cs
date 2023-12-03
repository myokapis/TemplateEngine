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

using System.Collections.Generic;
using TemplateEngine.Writer;

namespace TemplateEngine.Web
{

    /// <summary>
    /// An interface defining additional methods and properties specific to web writers
    /// </summary>
    public interface IWebWriter : ITemplateWriter
    {

        ///// <summary>
        ///// Convenience method for ensuring a proper cast when calling GetWriter from the derived type
        ///// </summary>
        ///// <param name="sectionName">Name of the section for which a writer is to be returned</param>
        ///// <returns>Working copy of IWebWriter /> for the requested section</returns>
        //IWebWriter GetWebWriter(string sectionName);

        /// <summary>
        /// Sets section fields and appends the section once for each object in the data collection
        /// </summary>
        /// <typeparam name="T">Type of the data object</typeparam>
        /// <param name="sectionName">Name of the section in which to set fields</param>
        /// <param name="data">Data collection</param>
        /// <param name="fieldDefinitions"><see cref="FieldDefinitions" /> object that defines special fields</param>
        void SetMultiSectionFields<T>(string sectionName, IEnumerable<T> data, FieldDefinitions fieldDefinitions);

        /// <summary>
        /// Sets section fields and appends the section once for each object in the data collection
        /// </summary>
        /// <typeparam name="T">Type of the data object</typeparam>
        /// <param name="data">Data collection</param>
        /// <param name="fieldDefinitions"><see cref="FieldDefinitions" /> object that defines special fields</param>
        void SetMultiSectionFields<T>(IEnumerable<T> data, FieldDefinitions fieldDefinitions);

        /// <summary>
        /// Sets option fields in a section
        /// </summary>
        /// <param name="sectionName">Name of the option section</param>
        /// <param name="data">Option data</param>
        /// <param name="selectedValue">Value of the selected option</param>
        void SetOptionFields(string sectionName, IEnumerable<Option> data, string? selectedValue = null);

        /// <summary>
        /// Sets section fields from a data object
        /// </summary>
        /// <typeparam name="T">Type of the data object</typeparam>
        /// <param name="sectionName">Name of the section to set</param>
        /// <param name="data">Data object</param>
        /// <param name="sectionOptions"><see cref="SectionOptions" /> for desired append and deselect behavior</param>
        /// <param name="fieldDefinitions"><see cref="FieldDefinitions" /> object that defines special fields</param>
        void SetSectionFields<T>(string sectionName, T data, SectionOptions sectionOptions, FieldDefinitions fieldDefinitions);

        /// <summary>
        /// Sets section fields from a data object
        /// </summary>
        /// <typeparam name="T">Type of the data object</typeparam>
        /// <param name="data">Data object</param>
        /// <param name="sectionOptions"><see cref="SectionOptions" /> for desired append and deselect behavior</param>
        /// <param name="fieldDefinitions"><see cref="FieldDefinitions" /> object that defines special fields</param>
        void SetSectionFields<T>(T data, SectionOptions sectionOptions, FieldDefinitions fieldDefinitions);

    }

}
