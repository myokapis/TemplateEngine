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
using System.Linq;
using System.Text;
using System.Web;
using TemplateEngine.Document;
using TemplateEngine.Writer;
using SectionGroup = System.Collections.Generic.Dictionary<string, TemplateEngine.Writer.ITemplateWriter>;

namespace TemplateEngine.Web
{

    /// <summary>
    /// Extends the TemplateWriter with methods specific to working with HTML
    /// </summary>
    public class WebWriter : TemplateWriter, IWebWriter
    {

        /// <summary>
        /// Constructs a new WebWriter for a template
        /// </summary>
        /// <param name="template">The template on which the writer will operate</param>
        public WebWriter(ITemplate template) : base(template, new SectionGroup(), new SectionGroup())
        {
            foreach (var sectionName in template.ChildSectionNames)
            {
                var childWriter = new WebWriter(template.GetTemplate(sectionName), stack);
                sections.Add(sectionName, childWriter);
            }

            InitializeValueSet(valueSet);
        }

        /// <summary>
        /// Constructs a new WebWriter for a template
        /// </summary>
        /// <param name="template">The template on which the writer will operate</param>
        /// <param name="stack">The stack from the parent template</param>
        protected WebWriter(ITemplate template, Stack<ITemplateWriter> stack) : base(template, new SectionGroup(), new SectionGroup(), stack)
        {
            foreach (var sectionName in template.ChildSectionNames)
            {
                var childWriter = new WebWriter(template.GetTemplate(sectionName));
                sections.Add(sectionName, childWriter);
            }

            InitializeValueSet(valueSet);
        }

        /// <summary>
        /// Creates a main WebWriter initialized with the incoming values
        /// </summary>
        /// <param name="template">The ITemplate to be managed by the writer</param>
        /// <param name="sections">The standard sections included in the writer</param>
        /// <param name="registeredSections">Any registered sections to be included in the writer</param>
        protected WebWriter(ITemplate template, SectionGroup sections, SectionGroup registeredSections) :
            base(template, sections, registeredSections)
        {
            InitializeValueSet(valueSet);
        }

        /// <summary>
        /// Creates a child WebWriter initialized with the incoming values
        /// </summary>
        /// <param name="template">The ITemplate to be managed by the writer</param>
        /// <param name="sections">The standard sections included in the writer</param>
        /// <param name="registeredSections">Any registered sections to be included in the writer</param>
        /// <param name="stack">The initialized hierarchy of template writers</param>
        protected WebWriter(ITemplate template, SectionGroup sections, SectionGroup registeredSections, Stack<ITemplateWriter> stack) :
            base(template, sections, registeredSections, stack) {}

        /// <summary>
        /// Creates a shallow copy of a TemplateWriter instance.
        /// </summary>
        ///<param name="asMain">Causes the stack to be initialized as a main writer when true.
        ///Otherwise the stack of the base writer is copied to the new writer</param>
        public override ITemplateWriter Copy(bool asMain = false)
        {
            return asMain ? new WebWriter(template, sections, registeredSections) :
                new WebWriter(template, sections, registeredSections, stack);
        }

        /// <summary>
        /// Sets a field from a string value. The string is html encoded for safety.
        /// </summary>
        /// <param name="key">Name of the field to set</param>
        /// <param name="val">Field value</param>
        public override void SetField(string key, string val)
        {
            var currentWriter = (WebWriter)CurrentWriter;

            try
            {
                currentWriter.valueSet.FieldValues[key] = HttpUtility.HtmlEncode(val);
            }
            catch (KeyNotFoundException ex)
            {
                var msg = $"The field, {key}, was not found in the selected template.";
                throw new ArgumentException(msg, ex);
            }
        }

        /// <summary>
        /// Sets section fields and appends the section once for each object in the data collection
        /// </summary>
        /// <typeparam name="T">Type of the data object</typeparam>
        /// <param name="sectionName">Name of the section in which to set fields</param>
        /// <param name="data">Data collection</param>
        /// <param name="fieldDefinitions"><see cref="FieldDefinitions" /> object that defines special fields</param>
        public void SetMultiSectionFields<T>(string sectionName, IEnumerable<T> data, FieldDefinitions fieldDefinitions)
        {
            SelectSection(sectionName);
            SetMultiSectionFields<T>(data, fieldDefinitions);
            DeselectSection();
        }

        /// <summary>
        /// Sets section fields and appends the section once for each object in the data collection
        /// </summary>
        /// <typeparam name="T">Type of the data object</typeparam>
        /// <param name="data">Data collection</param>
        /// <param name="fieldDefinitions"><see cref="FieldDefinitions" /> object that defines special fields</param>
        public void SetMultiSectionFields<T>(IEnumerable<T> data, FieldDefinitions fieldDefinitions)
        {

            foreach (T row in data)
            {
                var accessor = new ViewModelAccessor<T>(row);

                foreach (KeyValuePair<string, string> kvp in accessor.FieldValues)
                {
                    if (!SetWebFields(fieldDefinitions, kvp.Key, kvp.Value))
                        SetField(kvp.Key, kvp.Value);
                }

                AppendSection();
            }

        }

        /// <summary>
        /// Sets section fields from a data object
        /// </summary>
        /// <typeparam name="T">Type of the data object</typeparam>
        /// <param name="sectionName">Name of the section to set</param>
        /// <param name="data">Data object</param>
        /// <param name="fieldDefinitions"><see cref="FieldDefinitions" /> object that defines special fields</param>
        public void SetSectionFields<T>(string sectionName, T data, FieldDefinitions fieldDefinitions)
        {
            SelectSection(sectionName);
            SetSectionFields(data, SectionOptions.AppendDeselect, fieldDefinitions);
        }

        /// <summary>
        /// Sets section fields from a data object
        /// </summary>
        /// <typeparam name="T">Type of the data object</typeparam>
        /// <param name="sectionName">Name of the section to set</param>
        /// <param name="data">Data object</param>
        /// <param name="sectionOptions"><see cref="SectionOptions" /> for desired append and deselect behavior</param>
        /// <param name="fieldDefinitions"><see cref="FieldDefinitions" /> object that defines special fields</param>
        public void SetSectionFields<T>(string sectionName, T data, SectionOptions sectionOptions, FieldDefinitions fieldDefinitions)
        {
            SelectSection(sectionName);
            SetSectionFields(data, sectionOptions, fieldDefinitions);
        }

        /// <summary>
        /// Sets section fields from a data object
        /// </summary>
        /// <typeparam name="T">Type of the data object</typeparam>
        /// <param name="data">Data object</param>
        /// <param name="sectionOptions"><see cref="SectionOptions" /> for desired append and deselect behavior</param>
        /// <param name="fieldDefinitions"><see cref="FieldDefinitions" /> object that defines special fields</param>
        public void SetSectionFields<T>(T data, SectionOptions sectionOptions, FieldDefinitions fieldDefinitions)
        {

            ViewModelAccessor<T> accessor = new ViewModelAccessor<T>(data);

            foreach (KeyValuePair<string, string> kvp in accessor.FieldValues)
            {
                if (!SetWebFields(fieldDefinitions, kvp.Key, kvp.Value))
                    SetField(kvp.Key, kvp.Value);
            }

            if (sectionOptions.Append) AppendSection();
            if (sectionOptions.Deselect) DeselectSection();
        }

        /// <summary>
        /// Sets option fields in a section
        /// </summary>
        /// <param name="sectionName">Name of the option section</param>
        /// <param name="data">Option data</param>
        /// <param name="selectedValue">Value of the selected option</param>
        public void SetOptionFields(string sectionName, IEnumerable<Option> data, string? selectedValue = null)
        {
            SelectSection(sectionName.ToUpper());

            foreach (Option option in data)
            {
                SetField("TEXT", option.Text);
                SetField("VALUE", option.Value);

                if(selectedValue != null)
                    SetField("SELECTED", (option.Value == selectedValue) ? "selected='selected'" : "");

                AppendSection();
            }

            DeselectSection();
        }

        /// <summary>
        /// Sets web-specific fields such as checkboxes and dropdown lists.
        /// </summary>
        /// <param name="fieldDefinitions">A FieldDefinitions object containing the checkbox and dropdown fields available to be set</param>
        /// <param name="fieldName">The name of the current field to be set</param>
        /// <param name="value">The value to be set for the current field</param>
        /// <returns>True if a matching field was found and set</returns>
        protected bool SetWebFields(FieldDefinitions fieldDefinitions, string fieldName, string value)
        {
            if (fieldDefinitions == null) return false;

            if (fieldDefinitions.Checkboxes.Contains(fieldName))
            {
                var checkedValue = (TrueValues.Contains(value)) ? "checked='checked'" : "";
                SetField(fieldName, checkedValue);
                return true;
            }
            else if (fieldDefinitions.DropdownFieldNames.Contains(fieldName))
            {
                var definition = fieldDefinitions.Dropdowns
                    .FirstOrDefault(d => d.FieldName == fieldName);

                SetOptionFields(definition.SectionName, definition.Data, value);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Default handler for writing literal text blocks in a web writer
        /// </summary>
        /// <param name="sb">The string builder to receive the literal text</param>
        /// <param name="textBlock">A text block containing the literal text</param>
        protected override void WriteLiteralTextBlock(StringBuilder sb, TextBlock textBlock)
        {
            sb.Append(HttpUtility.HtmlEncode(textBlock.Text));
        }

    }

}
