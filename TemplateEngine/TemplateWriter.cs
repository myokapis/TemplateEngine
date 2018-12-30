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
        private Dictionary<string, List<ITemplateWriter>> providerSets = new Dictionary<string, List<ITemplateWriter>>();
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
        }

        /// <summary>
        /// Creates a new TemplateWriter based on an existing TemplateWriter instance
        /// </summary>
        /// <param name="templateWriter"></param>
        protected TemplateWriter(TemplateWriter templateWriter)
        {
            this.template = templateWriter.template;
            this.sections = templateWriter.sections;
            this.sectionSets = templateWriter.sectionSets.ToDictionary(s => s.Key, s => new List<ITemplateWriter>());
            InitializeFieldValues();
            this.registeredSections = templateWriter.registeredSections;
        }

        private ITemplateWriter currentWriter => this.stack?.Peek();

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

        public void AppendSection(bool deselect = false)
        {
            if(this.selectedProvider != null)
            {
                var isRoot = this.selectedProvider.IsRootSelected;
                this.selectedProvider.AppendSection();
                if (isRoot && deselect) this.selectedProvider = null;
            }

            AppendSection();
            if (deselect) DeselectSection();
        }

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

                this.selectedProvider.DeselectSection();
                return;
            }

            if (this.stack.Count == 1) throw new InvalidOperationException("Cannot deselect the parent section");

            // clear any pending data from the current writer, remove the writer from the stack, and save it
            var writer = this.currentWriter;
            writer.Clear();
            this.stack.Pop();
            (this.currentWriter as TemplateWriter).SaveSection(writer);
        }

        public string GetContent(bool appendAll = false)
        {
            if (appendAll && (this.stack != null)) this.AppendAll();
            var sb = new StringBuilder(this.template.RawLength);
            GetContent(sb);
            return sb.ToString();
        }

        public ITemplateWriter GetWriter(string sectionName)
        {
            // check for a match with the current section
            if (this.template.SectionName == sectionName) return this;

            // check for a match with child sections
            if (this.sections.ContainsKey(sectionName)) return this.sections[sectionName];

            // otherwise delegate the check to each child section
            foreach(var section in this.sections.Values)
            {
                var writer = section.GetWriter(sectionName);
                if (writer != null) return writer;
            }

            return null;
        }

        public bool HasData => (this.fieldValueSets.Count > 0)
            || !this.template.IsEmpty
            || (this.providerSets.Count > 0);

        public string HashCode => this.template.TemplateId.ToString();

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
            return true;
        }

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
                var result = childWriter.RegisterFieldProvider(fieldName, writer);
                return result;
            }

            // otherwise register the writer to this section
            return this.RegisterFieldProvider(fieldName, writer);
        }

        protected void SaveProvider(string fieldName, ITemplateWriter writer)
        {
            this.providerSets[fieldName].Add(writer);

        }

        protected void SaveSection(ITemplateWriter writer)
        {
            this.sectionSets[writer.SectionName].Add(writer);
        }

        public string SectionName => this.template.SectionName;

        public void SelectProvider(string fieldName)
        {
            var cw = this.currentWriter as TemplateWriter;
            var provider = (this.currentWriter as TemplateWriter).GetProvider(fieldName);
            var writer = new TemplateWriter(provider as TemplateWriter);
            this.stack.Push(writer);
        }

        // TODO: maybe change this to a param array and have the code iterate the selection params
        public void SelectSection(string sectionName)
        {
            // delegate to the provider if one is selected
            if(this.selectedProvider != null)
            {
                this.selectedProvider.SelectSection(sectionName);
                return;
            }

            var section = (this.currentWriter as TemplateWriter).GetChildSection(sectionName);
            var writer = new TemplateWriter(section as TemplateWriter);
            this.stack.Push(writer);
        }

        public string SelectedSectionName
        {
            get
            {
                var currentTemplate = (this.stack == null) ? this.template : ((TemplateWriter)this.stack.Peek()).template;
                return currentTemplate.SectionName;
            }
        }

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

        public void SetMultiSectionFields<T>(string sectionName, IEnumerable<T> data)
        {
            SelectSection(sectionName);
            SetMultiSectionFields(data, null);
        }

        public void SetMultiSectionFields<T>(string sectionName, IEnumerable<T> data, FieldDefinitions fieldDefinitions)
        {
            SelectSection(sectionName);
            SetMultiSectionFields<T>(data, fieldDefinitions);
        }

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

        public void SetSectionFields<T>(string sectionName, T data)
        {
            SelectSection(sectionName);
            SetSectionFields(data, SectionOptions.AppendDeselect, null);
        }

        public void SetSectionFields<T>(string sectionName, T data, FieldDefinitions fieldDefinitions)
        {
            SelectSection(sectionName);
            SetSectionFields(data, SectionOptions.AppendDeselect, fieldDefinitions);
        }

        public void SetSectionFields<T>(string sectionName, T data, SectionOptions sectionOptions, FieldDefinitions fieldDefinitions = null)
        {
            SelectSection(sectionName);
            SetSectionFields(data, sectionOptions, fieldDefinitions);
        }

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

        public void SetField<T>(string key, T val)
        {
            var stringVal = (val == null) ? "" : val.ToString();
            SetField(key, stringVal);
        }

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

        protected void WriteTextBlocks(StringBuilder sb, Dictionary<string, string> fieldValueSet)
        {

            foreach (var textBlock in this.template.TextBlocks)
            {

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
