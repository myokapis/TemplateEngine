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
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace TemplateEngine
{

    /// <summary>
    /// Provides methods for manipulating and filling template sections with data and returning the text of the 
    /// populated template
    /// </summary>
    public class TemplateWriter : ITemplateWriter
    {
        private ITemplate template;
        private Dictionary<string, ITemplateWriter> sections;
        private ITemplateWriter selectedProvider = null;
        private string selectedProviderField = null;
        private Stack<ITemplateWriter> stack;
        private Dictionary<string, string> fieldValueSet = new Dictionary<string, string>();
        private List<Dictionary<string, string>> fieldValueSets = new List<Dictionary<string, string>>();
        private Dictionary<string, ITemplateWriter> registeredSections;
        //private Dictionary<string, List<ITemplateWriter>> providerSets = new Dictionary<string, List<ITemplateWriter>>();
        protected Dictionary<string, List<ITemplateWriter>> sectionSets = new Dictionary<string, List<ITemplateWriter>>();

        /// <summary>
        /// Constructs a new TemplateWriter for a template
        /// </summary>
        /// <param name="template">The template on which the writer will operate</param>
        public TemplateWriter(ITemplate template)
        {
            this.template = template;
            this.stack = new Stack<ITemplateWriter>();
            this.stack.Push(this);
            this.sections = new Dictionary<string, ITemplateWriter>();
            PrepareSections();
            InitializeFieldValues();
            this.registeredSections = new Dictionary<string, ITemplateWriter>();
            this.WriterId = Guid.NewGuid();
        }

        /// <summary>
        /// Constructs a new TemplateWriter for a section of a template
        /// </summary>
        /// <param name="template">The parent template containing the requested section</param>
        /// <param name="sectionName">The template section on which the writer will operate</param>
        protected TemplateWriter(ITemplate template, string sectionName)
        {
            this.template = template.GetTemplate(sectionName);
            this.sections = new Dictionary<string, ITemplateWriter>();
            PrepareSections();
            InitializeFieldValues();
            this.registeredSections = new Dictionary<string, ITemplateWriter>();
            this.WriterId = Guid.NewGuid();
        }

        /// <summary>
        /// Creates a new TemplateWriter based on an existing TemplateWriter instance
        /// </summary>
        /// <param name="templateWriter">TemplateWriter to clone</param>
        protected TemplateWriter(TemplateWriter templateWriter, bool makeRoot = false)
        {
            this.template = templateWriter.template;
            this.sections = templateWriter.sections;
            this.sectionSets = templateWriter.sectionSets.ToDictionary(s => s.Key, s => new List<ITemplateWriter>());
            InitializeFieldValues();
            this.registeredSections = templateWriter.registeredSections;
            this.WriterId = Guid.NewGuid();

            if (makeRoot)
            {
                this.stack = new Stack<ITemplateWriter>();
                stack.Push(this);
            }
        }

        private static List<string> traceResults = new List<string>();
        public static void ClearTrace() => traceResults.Clear();
        public static bool EnableTrace { get; set; }
        public static List<string> TraceResults => new List<string>(traceResults);
        private static void WriteTrace(TemplateWriter writer, TextBlock textBlock)
        {
            if (!EnableTrace) return;
            traceResults.Add($"Writer: {writer.SectionName}-{writer.WriterId}; TextBlock: {textBlock.Type}-{textBlock.ReferenceName}");
        }

        private ITemplateWriter currentWriter => this.stack?.Peek();

        /// <summary>
        /// Append and deselect all sections in the hierarchy beginning with the current section and ending
        /// at the section designated by the sectionName parameter. If no sectionName parameter is provided
        /// then the entire hierarchy is appended.
        /// </summary>
        /// <param name="sectionName">Name of the last section to append</param>
        public void AppendAll(string sectionName = null)
        {
            if(this.selectedProvider != null)
            {
                this.selectedProvider.AppendAll();
                this.selectedProvider = null;
            }

            if (this.stack == null) return;
            var continueFlag = true;

            while(continueFlag)
            {
                continueFlag = (SelectedSectionName != "@MAIN") && (SelectedSectionName != sectionName);
                AppendSection(SelectedSectionName != "@MAIN");
            }
        }

        /// <summary>
        /// Appends the current section and optionally deselects the current section.
        /// </summary>
        /// <param name="deselect">Sets the deselect behavior</param>
        public void AppendSection(bool deselect = false)
        {
            if(this.selectedProvider != null)
            {
                var isRoot = this.selectedProvider.IsRootSelected;
                //this.selectedProvider.AppendSection();
                if (isRoot && deselect) this.selectedProvider = null;
            }

            AppendSection();
            if (deselect) DeselectSection();
        }

        /// <summary>
        /// Resets the data fields in the current section.
        /// </summary>
        public void Clear()
        {
            if(this.selectedProvider != null)
            {
                this.selectedProvider.Clear();
                return;
            }

            if (this.stack == null || this.currentWriter == this)
            {
                InitializeFieldValues();
            }
            else
            {
                this.currentWriter.Clear();
            }
        }

        /// <summary>
        /// Deselects the current section and makes the parent section the new selected section
        /// </summary>
        public void DeselectSection()
        {
            if(this.selectedProvider != null)
            {
                if(this.selectedProvider.IsRootSelected)
                {
                    this.selectedProvider.Clear();
                    var parentSection = this.stack.Peek();
                    (parentSection as TemplateWriter).SaveProvider(this.selectedProviderField, this.selectedProvider);
                    this.selectedProvider = null;
                    this.selectedProviderField = null;
                    return;
                }

                //this.selectedProvider.DeselectSection();
                //return;
            }

            if (this.stack.Count == 1) throw new InvalidOperationException("Cannot deselect the parent section");

            // clear any pending data from the current writer, remove the writer from the stack, and save it
            var writer = this.currentWriter;
            writer.Clear();
            this.stack.Pop();
            (this.currentWriter as TemplateWriter).SaveSection(writer);
        }

        /// <summary>
        /// Generates template text with data fields populated. Optionally appends all sections of the
        /// hierarchy before generating the output.
        /// </summary>
        /// <param name="appendAll">Sets the append all behavior</param>
        /// <returns>A text document with template fields populated</returns>
        public string GetContent(bool appendAll = false)
        {
            if (appendAll && (this.stack != null)) this.AppendAll();
            var sb = new StringBuilder(this.template.RawLength);
            GetContent(sb);
            return sb.ToString();
        }

        /// <summary>
        /// Gets a TemplateWriter for the requested section
        /// </summary>
        /// <param name="sectionName">Name of the section for which a writer is to be returned</param>
        /// <returns><cref="ITemplateWriter" /> for the requested section</returns>
        public ITemplateWriter GetWriter(string sectionName, bool makeRoot = false)
        {
            ITemplateWriter writer = null;

            if (this.template.SectionName == sectionName)
            {
                // matches current section
                writer = this;
            }
            else if (this.sections.ContainsKey(sectionName))
            {
                // matches child section
                writer = this.sections[sectionName];
            }
            else
            {
                // otherwise delegate the check to each child section
                foreach (var section in this.sections.Values)
                {
                    writer = section.GetWriter(sectionName);
                    if (writer != null) break;
                }
            }

            return new TemplateWriter(writer as TemplateWriter, makeRoot);
        }

        /// <summary>
        /// Indicates if the template contains any text or fields
        /// </summary>
        public bool HasData => (this.fieldValueSets.Count > 0)
            || !this.template.IsEmpty;

        public List<string> Inspect()
        {

        }

        protected Inspect(List<string> inspection, int Level)
        {

        }

        /// <summary>
        /// Indicates if the currently selected section is a root section
        /// </summary>
        public bool IsRootSelected
        {
            get
            {
                // if a provider is selected then delegate to the provider
                if (this.selectedProvider != null) return this.selectedProvider.IsRootSelected;

                // if the current section is the base section then see if the root is selected
                return (this.stack?.Peek()?.SelectedSectionName == "@MAIN");
            }
        }

        /// <summary>
        /// Removes all populated field data and clears the current section
        /// </summary>
        public void Reset()
        {
            if (this.stack == null || this.currentWriter == this)
            {
                // if this is a child section or if this is the parent section and it is selected then clear the pending data
                this.fieldValueSets.Clear();
                InitializeFieldValues();
            }
            else
            {
                // clear all pending data in the selected section
                this.currentWriter.Reset();
            }
        }

        /// <summary>
        /// Allows a template writer to be bound to a field and to populate a field
        /// </summary>
        /// <param name="fieldName">Name of the field to be bound</param>
        /// <param name="writer">Writer instance that will provide data for the field</param>
        /// <returns>Indicates if the writer was successfully bound to the field</returns>
        public bool RegisterFieldProvider(string fieldName, ITemplateWriter writer)
        {
            // skip if the writer is the current writer
            if (this == writer) return false;

            // skip if registering a template writer for a non-existent field
            if (!this.template.FieldNames.Contains(fieldName)) return false;

            // skip if the field already has a writer registered to it
            if (this.registeredSections.ContainsKey(fieldName)) return false;

            // otherwise register the writer
            this.registeredSections.Add(fieldName, writer);
            this.sectionSets.Add(fieldName, new List<ITemplateWriter>());
            return true;
        }

        /// <summary>
        /// Allows a template writer to be bound to a field and to populate a field in a template subsection
        /// </summary>
        /// <param name="sectionName">Name of child section containing the field to be bound</param>
        /// <param name="fieldName">Name of the field to be bound</param>
        /// <param name="writer">Writer instance that will provide data for the field</param>
        /// <returns>Indicates if the writer was successfully bound to the field</returns>
        public bool RegisterFieldProvider(string sectionName, string fieldName, ITemplateWriter writer)
        {

            // delegate the call to a child section if the requested section isn't this section
            if(this.template.SectionName != sectionName)
            {
                // get the requested child section
                var childWriter = this.GetWriter(sectionName);

                // skip if registering a template writer for a non-existent section or field
                if (childWriter == null) return false;

                // delegate the call
                return childWriter.RegisterFieldProvider(fieldName, writer);
            }

            // otherwise register the writer to this section
            return this.RegisterFieldProvider(fieldName, writer);
        }

        protected void SaveProvider(string fieldName, ITemplateWriter writer)
        {
            //this.providerSets[fieldName].Add(writer);
            this.sectionSets[fieldName].Add(writer);
        }

        protected void SaveSection(ITemplateWriter writer)
        {
            this.sectionSets[writer.SectionName].Add(writer);
        }

        /// <summary>
        /// Section name associated with this writer
        /// </summary>
        public string SectionName => this.template.SectionName;

        /// <summary>
        /// Selects a provider that will act as the current section
        /// </summary>
        /// <param name="fieldName">Name of the field to which the provider is bound</param>
        public void SelectProvider(string fieldName)
        {
            var cw = this.currentWriter as TemplateWriter;
            var provider = cw.GetProvider(fieldName);
            var writer = new TemplateWriter(provider as TemplateWriter);
            cw.selectedProvider = writer;
            this.stack.Push(writer);
        }

        /// <summary>
        /// Selects a child section to be the current section
        /// </summary>
        /// <param name="sectionName">Name of the section to select</param>
        // TODO: maybe change this to a param array and have the code iterate the selection params
        public void SelectSection(string sectionName)
        {
            //// delegate to the provider if one is selected
            //if(this.selectedProvider != null)
            //{
            //    this.selectedProvider.SelectSection(sectionName);
            //    return;
            //}

            var section = (this.currentWriter as TemplateWriter).GetChildSection(sectionName);
            var writer = new TemplateWriter(section as TemplateWriter);
            this.stack.Push(writer);
        }

        /// <summary>
        /// Name of the currently selected section
        /// </summary>
        public string SelectedSectionName
        {
            get
            {
                var currentTemplate = (this.stack == null) ? this.template : ((TemplateWriter)this.stack.Peek()).template;
                return currentTemplate.SectionName;
            }
        }

        /// <summary>
        /// Sets option fields in a section
        /// </summary>
        /// <param name="sectionName">Name of the option section</param>
        /// <param name="data">Option data</param>
        /// <param name="selectedValue">Value of the selected option</param>
        public void SetOptionFields(string sectionName, IEnumerable<Option> data, string selectedValue = "")
        {
            SelectSection(sectionName.ToUpper());

            foreach (Option option in data)
            {
                SetField("TEXT", option.Text);
                SetField("VALUE", option.Value);
                SetField("SELECTED", (option.Value == selectedValue) ? "selected='selected'" : "");
                AppendSection();
            }

            DeselectSection();
        }

        #region Field Setters For IEnumerable

        /// <summary>
        /// Sets section fields and appends the section once for each object in the data collection
        /// </summary>
        /// <typeparam name="T">Type of the data object</typeparam>
        /// <param name="sectionName">Name of the section in which to set fields</param>
        /// <param name="data">Data collection</param>
        public void SetMultiSectionFields<T>(string sectionName, IEnumerable<T> data)
        {
            SelectSection(sectionName);
            SetMultiSectionFields(data, null);
        }

        /// <summary>
        /// Sets section fields and appends the section once for each object in the data collection
        /// </summary>
        /// <typeparam name="T">Type of the data object</typeparam>
        /// <param name="sectionName">Name of the section in which to set fields</param>
        /// <param name="data">Data collection</param>
        /// <param name="fieldDefinitions"><cref="FieldDefinitions" /> object that defines special fields</param>
        public void SetMultiSectionFields<T>(string sectionName, IEnumerable<T> data, FieldDefinitions fieldDefinitions)
        {
            SelectSection(sectionName);
            SetMultiSectionFields<T>(data, fieldDefinitions);
        }

        /// <summary>
        /// Sets section fields and appends the section once for each object in the data collection
        /// </summary>
        /// <typeparam name="T">Type of the data object</typeparam>
        /// <param name="data">Data collection</param>
        /// <param name="fieldDefinitions"><cref="FieldDefinitions" /> object that defines special fields</param>
        public void SetMultiSectionFields<T>(IEnumerable<T> data, FieldDefinitions fieldDefinitions = null)
        {
            FieldDefinitions definitions = (fieldDefinitions != null) ? fieldDefinitions : new FieldDefinitions();

            foreach (T row in data)
            {
                ViewModelAccessor<T> accessor = new ViewModelAccessor<T>(row);

                foreach (KeyValuePair<string, string> kvp in accessor.FieldValues)
                {
                    if (definitions.Checkboxes.Contains(kvp.Key))
                    {
                        string checkedValue = (TrueValues.Contains(kvp.Value)) ? "checked='checked'" : "";
                        SetField(kvp.Key, checkedValue);
                    }
                    else if (definitions.DropdownFieldNames.Contains(kvp.Key))
                    {
                        DropdownDefinition definition = definitions.Dropdowns.Where<DropdownDefinition>(d => d.FieldName == kvp.Key).FirstOrDefault<DropdownDefinition>();
                        SetOptionFields(definition.SectionName, definition.Data, kvp.Value);
                    }
                    else
                    {
                        SetField(kvp.Key, kvp.Value);
                    }
                }

                AppendSection();

                var selectedSectionName = this.currentWriter.SelectedSectionName;
                DeselectSection();
                SelectSection(selectedSectionName);

            }

            DeselectSection();

        }

        #endregion

        #region Field Setters For POCO

        /// <summary>
        /// Sets section fields from a data object
        /// </summary>
        /// <typeparam name="T">Type of the data object</typeparam>
        /// <param name="sectionName">Name of the section to set</param>
        /// <param name="data">Data object</param>
        public void SetSectionFields<T>(string sectionName, T data)
        {
            SelectSection(sectionName);
            SetSectionFields(data, SectionOptions.AppendDeselect, null);
        }

        /// <summary>
        /// Sets section fields from a data object
        /// </summary>
        /// <typeparam name="T">Type of the data object</typeparam>
        /// <param name="sectionName">Name of the section to set</param>
        /// <param name="data">Data object</param>
        /// <param name="fieldDefinitions"><cref="FieldDefinitions" /> object that defines special fields</param>
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
        /// <param name="sectionOptions"><cref="SectionOptions" /> for desired append and deselect behavior</param>
        /// <param name="fieldDefinitions"><cref="FieldDefinitions" /> object that defines special fields</param>
        public void SetSectionFields<T>(string sectionName, T data, SectionOptions sectionOptions, FieldDefinitions fieldDefinitions = null)
        {
            SelectSection(sectionName);
            SetSectionFields(data, sectionOptions, fieldDefinitions);
        }

        /// <summary>
        /// Sets section fields from a data object
        /// </summary>
        /// <typeparam name="T">Type of the data object</typeparam>
        /// <param name="data">Data object</param>
        /// <param name="sectionOptions"><cref="SectionOptions" /> for desired append and deselect behavior</param>
        /// <param name="fieldDefinitions"><cref="FieldDefinitions" /> object that defines special fields</param>
        public void SetSectionFields<T>(T data, SectionOptions sectionOptions, FieldDefinitions fieldDefinitions = null)
        {

            FieldDefinitions definitions = (fieldDefinitions != null) ? fieldDefinitions : new FieldDefinitions();

            ViewModelAccessor<T> accessor = new ViewModelAccessor<T>(data);

            foreach (KeyValuePair<string, string> kvp in accessor.FieldValues)
            {
                if (definitions.Checkboxes.Contains(kvp.Key))
                {
                    string checkedValue = (TrueValues.Contains(kvp.Value)) ? "checked='checked'" : "";
                    SetField(kvp.Key, checkedValue);
                }
                else if (definitions.DropdownFieldNames.Contains(kvp.Key))
                {
                    DropdownDefinition definition = definitions.Dropdowns.Where<DropdownDefinition>(d => d.FieldName == kvp.Key).FirstOrDefault<DropdownDefinition>();
                    SetOptionFields(definition.SectionName, definition.Data, kvp.Value);
                }
                else
                {
                    SetField(kvp.Key, kvp.Value);
                }

            }

            if (sectionOptions.Append) AppendSection();
            if (sectionOptions.Deselect) DeselectSection();
        }

        #endregion

        #region Field Setters For Base Types

        /// <summary>
        /// Sets a field from a string value
        /// </summary>
        /// <param name="key">Name of the field to set</param>
        /// <param name="val">Field value</param>
        public void SetField(string key, string val)
        {
            if (this.stack == null || this.currentWriter == this)
            {
                try
                {
                    this.fieldValueSet[key] = val;
                }
                catch (KeyNotFoundException ex)
                {
                    var msg = $"The field, {key}, was not found in the selected template.";
                    throw new ArgumentException(msg, ex);
                }
            }
            else
            {
                this.currentWriter.SetField(key, val);
            }
        }

        /// <summary>
        /// Sets a field from a string value
        /// </summary>
        /// <typeparam name="T">Type of the field value</typeparam>
        /// <param name="key">Name of the field to set</param>
        /// <param name="val">Field value</param>
        public void SetField<T>(string key, T val)
        {
            var stringVal = (val == null) ? "" : val.ToString();
            SetField(key, stringVal);
        }

        /// <summary>
        /// Unique id of the template bound to this writer
        /// </summary>
        public Guid TemplateId => this.template.TemplateId;

        #endregion

        #region Protected Methods

        protected void AppendSection()
        {
            if (this.stack == null || this.currentWriter == this)
            {
                this.fieldValueSets.Add(fieldValueSet);
                InitializeFieldValues();
            }
            else
            {
                this.currentWriter.AppendSection();
            }
        }

        protected void GetContent(StringBuilder sb)
        {
            // iterate each set of appended data and write a copy of the template containing the data
            foreach (var fieldValueSet in this.fieldValueSets)
            {
                WriteTextBlocks(sb, fieldValueSet);
            }

            Reset();
        }

        protected ITemplateWriter GetChildSection(string sectionName)
        {
            try
            {
                return this.sections[sectionName];
            }
            catch (KeyNotFoundException ex)
            {
                var msg = $"Section, {sectionName}, was not found as a child of the current section.";
                throw new ArgumentException(msg, ex);
            }
        }

        protected ITemplateWriter GetProvider(string fieldName)
        {
            try
            {
                return this.registeredSections[fieldName];
            }
            catch (KeyNotFoundException ex)
            {
                var msg = $"A registered provider for field, {fieldName}, was not found in this section.";
                throw new ArgumentException(msg, ex);
            }
        }

        protected void InitializeFieldValues()
        {
            this.fieldValueSet = this.template.FieldNames.ToDictionary(n => n, n => (string)null);
        }

        protected void PrepareSections()
        {
            foreach(var sectionName in this.template.ChildSectionNames)
            {
                var childWriter = new TemplateWriter(this.template, sectionName);
                this.sections.Add(sectionName, childWriter);
                this.sectionSets.Add(sectionName, new List<ITemplateWriter>());
            }
        }

        // unique id of this instance
        public Guid WriterId { get; }

        protected void WriteTextBlocks(StringBuilder sb, Dictionary<string, string> fieldValueSet)
        {

            foreach (var textBlock in this.template.TextBlocks)
            {
                WriteTrace(this, textBlock);

                if (textBlock.Type == TextBlockType.Text)
                {
                    sb.Append(textBlock.Text);
                }
                else if (textBlock.Type == TextBlockType.Section)
                {

                    List<string> extraText = null;
                    
                    // iterate each writer for the given section
                    foreach (var writer in this.sectionSets[textBlock.ReferenceName])
                    {
                        var tw = (writer as TemplateWriter);

                        // get the extra text for the section to be written section
                        if(tw.template.IsSingleLine && extraText == null)
                        {
                            extraText = this.template.TextBlocks
                                .Where(b => b.ReferenceName == textBlock.ReferenceName
                                    && (b.Type == TextBlockType.Prefix || b.Type == TextBlockType.Suffix))
                                .Select(b => b.TagText)
                                .ToList();
                        }

                        // write the extra text for the opening tag
                        if (extraText != null && tw.HasData)
                        {
                            sb.Append(extraText[0]);
                            sb.Append(extraText[1]);
                        }

                        tw.GetContent(sb);

                        // write the extra text for the closing tag
                        if (extraText != null && tw.HasData)
                        {
                            sb.Append(extraText[2]);
                            sb.Append(extraText[3]);
                        }
                    }
                }
                else if (textBlock.Type == TextBlockType.Field)
                {
                    if (this.registeredSections.ContainsKey(textBlock.ReferenceName))
                    {
                        // iterate each writer for the field
                        foreach (var writer in this.sectionSets[textBlock.ReferenceName])
                        {
                            (writer as TemplateWriter).GetContent(sb);
                        }
                    }
                    else
                    {
                        var fieldValue = fieldValueSet[textBlock.ReferenceName];
                        sb.Append(fieldValue);
                    }
                }
            }

        }

        #endregion

    }

}
