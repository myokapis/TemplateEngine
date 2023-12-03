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
using System.Text;
using TemplateEngine.Document;

namespace TemplateEngine.Writer
{

    /// <summary>
    /// Interface defining the methods and properties of a template writer
    /// </summary>
    public interface ITemplateWriter
    {
        /// <summary>
        /// Append and deselect all sections in the hierarchy beginning with the current section and ending
        /// at the section designated by the sectionName parameter. If no sectionName parameter is provided
        /// then the entire hierarchy is appended.
        /// </summary>
        /// <param name="sectionName">Name of the last section to append</param>
        void AppendAll(string? sectionName = null);

        /// <summary>
        /// Appends the current section and optionally deselects the current section.
        /// </summary>
        /// <param name="deselect">Sets the deselect behavior</param>
        void AppendSection(bool deselect = false);

        /// <summary>
        /// Resets the data fields in the current section.
        /// </summary>
        void Clear();

        /// <summary>
        /// Checks if a named section exists as a child section or registered section
        /// </summary>
        /// <param name="sectionName">Name of the section being tested for existence</param>
        /// <returns>True if the section in question exists in this writer</returns>
        bool ContainsSection(string sectionName);

        /// <summary>
        /// Create a copy of the writer
        /// </summary>
        /// <returns>A new writer having the same structure but without data</returns>
        ITemplateWriter Copy(bool asMain = false);

        /// <summary>
        /// A reference to the currently active template writer
        /// </summary>
        ITemplateWriter CurrentWriter { get; }

        /// <summary>
        /// Deselects the current section and makes the parent section the new selected section
        /// </summary>
        void DeselectSection();

        /// <summary>
        /// Generates template text with data fields populated. Optionally appends all sections of the
        /// hierarchy before generating the output.
        /// </summary>
        /// <param name="appendAll">Sets the append all behavior</param>
        /// <returns>A text document with template fields populated</returns>
        string GetContent(bool appendAll = false);

        /// <summary>
        /// Iterates the appended value sets and merges their data into the template
        /// </summary>
        /// <param name="sb"><see cref="StringBuilder"/> to which the populated template is written</param>
        void GetContent(StringBuilder sb);

        /// <summary>
        /// Gets a TemplateWriter for the requested section
        /// </summary>
        /// <param name="sectionName">Name of the section for which a writer is to be returned</param>
        /// <returns>Working copy of ITemplateWriter /> for the requested section</returns>
        ITemplateWriter GetWriter(string sectionName);

        /// <summary>
        /// Indicates if the template contains any field data
        /// </summary>
        bool HasData { get; }

        /// <summary>
        /// The names of all registered field providers
        /// </summary>
        IEnumerable<string> RegisteredFieldProviders { get; }

        /// <summary>
        /// Allows a template writer to be bound to a field and to populate a field
        /// </summary>
        /// <param name="fieldName">Name of the field to be bound</param>
        /// <param name="writer">Writer instance that will provide data for the field</param>
        /// <returns>Indicates if the writer was successfully bound to the field</returns>
        bool RegisterFieldProvider(string fieldName, ITemplateWriter writer);

        /// <summary>
        /// Allows a template writer to be bound to a field and to populate a field in a template subsection
        /// </summary>
        /// <param name="sectionName">Name of child section containing the field to be bound</param>
        /// <param name="fieldName">Name of the field to be bound</param>
        /// <param name="writer">Writer instance that will provide data for the field</param>
        /// <returns>Indicates if the writer was successfully bound to the field</returns>
        bool RegisterFieldProvider(string sectionName, string fieldName, ITemplateWriter writer);

        /// <summary>
        /// Removes all populated field data and clears the current section
        /// </summary>
        void Reset();

        /// <summary>
        /// Section name associated with this writer
        /// </summary>
        string SectionName { get; }

        /// <summary>
        /// Selects a provider that will act as the current section
        /// </summary>
        /// <param name="fieldName">Name of the field to which the provider is bound</param>
        void SelectProvider(string fieldName);

        /// <summary>
        /// Selects a child section to be the current section
        /// </summary>
        /// <param name="sectionName">Name of the section to select</param>
        void SelectSection(string sectionName);

        /// <summary>
        /// Name of the currently selected section
        /// </summary>
        string SelectedSectionName { get; }

        /// <summary>
        /// Sets a field from a string value
        /// </summary>
        /// <param name="key">Name of the field to set</param>
        /// <param name="val">Field value</param>
        void SetField(string key, string val);

        /// <summary>
        /// Sets a field from a generic value
        /// </summary>
        /// <typeparam name="T">Type of the field value</typeparam>
        /// <param name="key">Name of the field to set</param>
        /// <param name="val">Field value</param>
        void SetField<T>(string key, T val);

        /// <summary>
        /// Sets section fields and appends the section once for each object in the data collection
        /// </summary>
        /// <typeparam name="T">Type of the data object</typeparam>
        /// <param name="sectionName">Name of the section in which to set fields</param>
        /// <param name="data">Data collection</param>
        void SetMultiSectionFields<T>(string sectionName, IEnumerable<T> data);

        /// <summary>
        /// Sets section fields and appends the section once for each object in the data collection
        /// </summary>
        /// <typeparam name="T">Type of the data object</typeparam>
        /// <param name="data">Data collection</param>
        void SetMultiSectionFields<T>(IEnumerable<T> data);

        /// <summary>
        /// Sets section fields from a data object
        /// </summary>
        /// <typeparam name="T">Type of the data object</typeparam>
        /// <param name="sectionName">Name of the section to set</param>
        /// <param name="data">Data object</param>
        void SetSectionFields<T>(string sectionName, T data);

        /// <summary>
        /// Sets section fields from a data object
        /// </summary>
        /// <typeparam name="T">Type of the data object</typeparam>
        /// <param name="sectionName">Name of the section to set</param>
        /// <param name="data">Data object</param>
        /// <param name="sectionOptions"><see cref="SectionOptions" /> for desired append and deselect behavior</param>
        void SetSectionFields<T>(string sectionName, T data, SectionOptions sectionOptions);

        /// <summary>
        /// Sets section fields from a data object
        /// </summary>
        /// <typeparam name="T">Type of the data object</typeparam>
        /// <param name="data">Data object</param>
        /// <param name="sectionOptions"><see cref="SectionOptions" /> for desired append and deselect behavior</param>
        void SetSectionFields<T>(T data, SectionOptions sectionOptions);

        /// <summary>
        /// The template the writer is managing
        /// </summary>
        public ITemplate Template { get; }

        /// <summary>
        /// Unique id of the template bound to this writer
        /// </summary>
        Guid TemplateId { get; }

        /// <summary>
        /// Unique id of this instance
        /// </summary>
        Guid WriterId { get; }

    }

}
