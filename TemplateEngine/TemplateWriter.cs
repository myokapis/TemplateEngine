// TODO: public methods should get current writer and call protected method
//       protected methods should all operate against this

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
        private Dictionary<string, ITemplateWriter> registeredSections;
        //private ITemplateWriter selectedProvider = null;
        //private string selectedProviderField = null;
        private Stack<ITemplateWriter> stack;
        protected ValueSet valueSet = new ValueSet();
        protected List<ValueSet> valueSets = new List<ValueSet>();

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
            this.registeredSections = new Dictionary<string, ITemplateWriter>();
            PrepareSections();
            InitializeValueSet();
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
            //this.stack = 
            this.sections = new Dictionary<string, ITemplateWriter>();
            this.registeredSections = new Dictionary<string, ITemplateWriter>();
            PrepareSections();
            InitializeValueSet();
            this.WriterId = Guid.NewGuid();
        }

        // TODO: change these to Clone() methods and include those in the interface
        //       make sure all public methods are compatible with any ITemplateWriter

        /// <summary>
        /// Creates a new TemplateWriter based on an existing TemplateWriter instance
        /// </summary>
        /// <param name="templateWriter">TemplateWriter to clone</param>
        protected TemplateWriter(TemplateWriter templateWriter) //, bool isProvider = false)
        {
            this.template = templateWriter.template;
            this.sections = templateWriter.sections;
            this.registeredSections = templateWriter.registeredSections;
            this.stack = templateWriter.stack;
            InitializeValueSet();
            this.WriterId = Guid.NewGuid();

            //if (makeRoot)
            //{
            //    this.stack = new Stack<ITemplateWriter>();
            //    stack.Push(this);
            //}
        }

#if (DEBUG)
        private static List<string> traceResults = new List<string>();
        public static void ClearTrace() => traceResults.Clear();
        public static bool EnableTrace { get; set; }
        public static List<string> TraceResults => new List<string>(traceResults);
        private static void WriteTrace(TemplateWriter writer, TextBlock textBlock)
        {
            if (!EnableTrace) return;
            traceResults.Add($"Writer: {writer.SectionName}-{writer.WriterId}; TextBlock: {textBlock.Type}-{textBlock.ReferenceName}");
        }
#endif

        //private ITemplateWriter currentWriter => this.stack?.Peek() ?? this;

        /// <summary>
        /// Append and deselect all sections in the hierarchy beginning with the current section and ending
        /// at the section designated by the sectionName parameter. If no sectionName parameter is provided
        /// then the entire hierarchy is appended.
        /// </summary>
        /// <param name="sectionName">Name of the last section to append</param>
        public void AppendAll(string sectionName = null)
        {
            //if(this.selectedProvider != null)
            //{
            //    this.selectedProvider.AppendAll();
            //    this.selectedProvider = null;
            //}

            if (this.stack == null) return;
            var continueFlag = true;

            while (continueFlag)
            {
                //var currentWriter = (this.currentWriter as TemplateWriter);
                var deselectFlag = CurrentWriter.IsProvider || SelectedSectionName != "@MAIN";
                continueFlag = deselectFlag && (SelectedSectionName != sectionName);
                //continueFlag = (this.stack.Count > 1) && (SelectedSectionName != sectionName);
                //currentWriter.AppendSection();
                AppendSection();
                if (deselectFlag) DeselectSection();
                //if (this.stack.Count > 1) DeselectSection();
            }
        }

        /// <summary>
        /// Appends the current section and optionally deselects the current section.
        /// </summary>
        /// <param name="deselect">Sets the deselect behavior</param>
        public void AppendSection(bool deselect = false)
        {
            //if(this.selectedProvider != null)
            //{
            //    var isRoot = this.selectedProvider.IsRootSelected;
            //    if (isRoot && deselect) this.selectedProvider = null;
            //}

            //var currentWriter = (this.currentWriter as TemplateWriter);
            //currentWriter.AppendSection();
            AppendSection();
            if (deselect) DeselectSection();
        }

        /// <summary>
        /// Resets the data fields in the current section.
        /// </summary>
        public void Clear()
        {
            //if(this.selectedProvider != null)
            //{
            //    this.selectedProvider.Clear();
            //    return;
            //}

            //if (this.stack == null || this.currentWriter == this)
            //{
            //    InitializeValueSet();
            //}
            //else
            //{
            //    this.currentWriter.Clear();
            //}

            var currentWriter = (CurrentWriter as TemplateWriter);
            currentWriter.InitializeValueSet();
        }

        public ITemplateWriter CurrentWriter => this.stack?.Peek() ?? this;

        /// <summary>
        /// Deselects the current section and makes the parent section the new selected section
        /// </summary>
        public void DeselectSection()
        {
            //if(this.selectedProvider != null)
            //{
            //    if(this.selectedProvider.IsRootSelected)
            //    {
            //        this.selectedProvider.Clear();
            //        var parentSection = this.stack.Peek();
            //        //(parentSection as TemplateWriter).SaveProvider(this.selectedProviderField, this.selectedProvider);
            //        this.selectedProvider = null;
            //        this.selectedProviderField = null;
            //        return;
            //    }

            //}

            if (this.stack.Count == 1) throw new InvalidOperationException("Cannot deselect the parent section");

            //// clear any pending data from the current writer and remove it from the stack
            //CurrentWriter.Clear();
            this.stack.Pop();
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
            //if (appendAll) this.AppendAll();
            var sb = new StringBuilder(this.template.RawLength);
            GetContent(sb);
            return sb.ToString();
        }

        /// <summary>
        /// Gets a TemplateWriter for the requested section
        /// </summary>
        /// <param name="sectionName">Name of the section for which a writer is to be returned</param>
        /// <returns><cref="ITemplateWriter" /> for the requested section</returns>
        public ITemplateWriter GetWriter(string sectionName)
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
            else if(this.registeredSections.ContainsKey(sectionName))
            {
                // matches registered section (field provider)
                writer = this.registeredSections[sectionName];
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

            return writer; // new TemplateWriter(writer as TemplateWriter);
        }

        /// <summary>
        /// Indicates if the template contains any text or fields
        /// </summary>
        public bool HasData => (this.valueSets.Exists(v => v.FieldValues.Count() > 0))
            || (!this.template.IsEmpty && !this.template.IsSingleLine);

        // TODO: revamp this
        public List<string> Inspect()
        {
            var level = $"{this.WriterId}:{this.SectionName}";
            var inspection = new List<string> { level };

            foreach (var section in this.sections)
            {
                foreach (var sectionSet in this.valueSets.Select(v => v.SectionWriters))
                {
                    ((TemplateWriter)sectionSet[section.Key]).Inspect(inspection, level);
                }
            }

            return inspection;
        }

        // TODO: revamp this
        protected void Inspect(List<string> inspection, string parentLevel)
        {
            var level = $"{parentLevel}--->{this.WriterId}:{this.SectionName}";
            inspection.Add(level);

            foreach (var section in this.sections)
            {
                foreach (var sectionSet in this.valueSets.Select(v => v.SectionWriters))
                {
                    ((TemplateWriter)sectionSet[section.Key]).Inspect(inspection, level);
                }
            }
        }

        ///// <summary>
        ///// Indicates if the currently selected section is a root section
        ///// </summary>
        //public bool IsRootSelected
        //{
        //    get
        //    {
        //        // if a provider is selected then delegate to the provider
        //        if (this.selectedProvider != null) return this.selectedProvider.IsRootSelected;

        //        // if the current section is the base section then see if the root is selected
        //        return (this.stack?.Peek()?.SelectedSectionName == "@MAIN");
        //    }
        //}

        public bool IsProvider { get; protected set; }

        /// <summary>
        /// Removes all populated field data and clears the current section
        /// </summary>
        public void Reset()
        {
            //if (this.stack == null || this.currentWriter == this)
            //{
            //    // if this is a child section or if this is the parent section and it is selected then clear the pending data
            //    this.valueSets.Clear();
            //    InitializeValueSet();
            //}
            //else
            //{
            //    // clear all pending data in the selected section
            //    this.currentWriter.Reset();
            //}
            //var currentWriter = this.currentWriter as TemplateWriter;
            //currentWriter.valueSets.Clear();
            //currentWriter.InitializeValueSet();

            valueSets.Clear();
            InitializeValueSet();
        }

        /// <summary>
        /// Allows a template writer to be bound to a field and to populate a field
        /// </summary>
        /// <param name="fieldName">Name of the field to be bound</param>
        /// <param name="writer">Writer instance that will provide data for the field</param>
        /// <returns>The writer instance if registration succeeded</returns>
        public ITemplateWriter RegisterFieldProvider(string fieldName, ITemplateWriter writer)
        {
            var currentWriter = this.CurrentWriter as TemplateWriter;

            // skip if the writer is the current writer
            if (currentWriter == writer) return null;

            return currentWriter.RegisterFieldProvider(fieldName, writer, this);
        }

        /// <summary>
        /// Allows a template writer to be bound to a field and to populate a field in a template subsection
        /// </summary>
        /// <param name="sectionName">Name of child section containing the field to be bound</param>
        /// <param name="fieldName">Name of the field to be bound</param>
        /// <param name="writer">Writer instance that will provide data for the field</param>
        /// <returns>The writer instance if registration succeeded</returns>
        public ITemplateWriter RegisterFieldProvider(string sectionName, string fieldName, ITemplateWriter writer)
        {

            // delegate the call to a child section if the requested section isn't this section
            if (this.template.SectionName != sectionName)
            {
                // get the requested child section
                var childWriter = this.GetWriter(sectionName) as TemplateWriter;

                // skip if registering a template writer for a non-existent section or field
                if (childWriter == null) return null;

                // delegate the call
                return childWriter.RegisterFieldProvider(fieldName, writer, this);
            }

            // otherwise register the writer to this section
            return this.RegisterFieldProvider(fieldName, writer, this);
        }

        //// TODO: decide if this can be removed or if it needs logic added
        //protected void SaveProvider(string fieldName, ITemplateWriter writer)
        //{
        //    //this.providerSets[fieldName].Add(writer);
        //    //this.sectionSets[fieldName].Add(writer);
        //}

        //// TODO: decide if this can be removed
        //protected void SaveSection(ITemplateWriter writer)
        //{
        //    //this.sectionSets[writer.SectionName].Add(writer);
        //}

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
            var cw = CurrentWriter as TemplateWriter;
            var provider = new TemplateWriter(cw.GetProvider(fieldName) as TemplateWriter);
            //var writer = new TemplateWriter(provider as TemplateWriter);
            provider.IsProvider = true;
            //cw.selectedProvider = writer;
            this.stack.Push(provider);
        }

        /// <summary>
        /// Selects a child section to be the current section
        /// </summary>
        /// <param name="sectionName">Name of the section to select</param>
        // TODO: maybe change this to a param array and have the code iterate the selection params
        public void SelectSection(string sectionName)
        {
            var currentWriter = CurrentWriter as TemplateWriter;
            var writer = new TemplateWriter(currentWriter.GetChildSection(sectionName) as TemplateWriter);
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
                //return ((TemplateWriter)this.stack.Peek()).template.SectionName;
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
            FieldDefinitions definitions = fieldDefinitions ?? new FieldDefinitions();

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

                //// TODO: see if this mess is really necessary
                //var selectedSectionName = this.currentWriter.SelectedSectionName;
                //DeselectSection();
                //SelectSection(selectedSectionName);

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
            //if (this.stack == null || this.currentWriter == this)
            //{
            //    try
            //    {
            //        this.valueSet.FieldValues[key] = val;
            //    }
            //    catch (KeyNotFoundException ex)
            //    {
            //        var msg = $"The field, {key}, was not found in the selected template.";
            //        throw new ArgumentException(msg, ex);
            //    }
            //}
            //else
            //{
            //    this.currentWriter.SetField(key, val);
            //}

            try
            {
                var currentWriter = CurrentWriter as TemplateWriter;
                currentWriter.valueSet.FieldValues[key] = val;
            }
            catch (KeyNotFoundException ex)
            {
                var msg = $"The field, {key}, was not found in the selected template.";
                throw new ArgumentException(msg, ex);
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

        // unique id of this instance
        public Guid WriterId { get; }

        #endregion

        #region Protected Methods

        protected void AppendSection()
        {
            this.valueSets.Add(this.valueSet);
            this.InitializeValueSet();
        }

        protected void GetContent(StringBuilder sb)
        {
            // iterate each set of appended data and write a copy of the template containing the data
            foreach (var valueSet in this.valueSets)
            {
                WriteTextBlocks(sb, valueSet);
            }

            Reset();
        }

        protected ITemplateWriter GetChildSection(string sectionName)
        {
            try
            {
                return this.valueSet.SectionWriters[sectionName];
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
                return this.valueSet.FieldWriters[fieldName];
            }
            catch (KeyNotFoundException ex)
            {
                var msg = $"A registered provider for field, {fieldName}, was not found in this section.";
                throw new ArgumentException(msg, ex);
            }
        }

        protected void InitializeValueSet()
        {
            var valueSet = new ValueSet();

            valueSet.FieldValues = this.template.FieldNames.ToDictionary(n => n, n => (string)null);

            valueSet.FieldWriters = this.registeredSections.ToDictionary(s => s.Key,
                s => new TemplateWriter((TemplateWriter)s.Value) as ITemplateWriter);

            valueSet.SectionWriters = this.sections.ToDictionary(s => s.Key,
                s => new TemplateWriter((TemplateWriter)s.Value) as ITemplateWriter);

            this.valueSet = valueSet;
        }

        // TODO: determine if this is needed any more; why is it necessary to copy the objects?
        protected void PrepareSections()
        {
            foreach(var sectionName in this.template.ChildSectionNames)
            {
                var childWriter = new TemplateWriter(this.template, sectionName);
                childWriter.stack = this.stack;
                this.sections.Add(sectionName, childWriter);
            }
        }

        protected ITemplateWriter RegisterFieldProvider(string fieldName, ITemplateWriter writer, TemplateWriter delegatingWriter)
        {
            // skip if registering a template writer for a non-existent field
            if (!this.template.FieldNames.Contains(fieldName)) return null;

            // skip if the field already has a writer registered to it
            if (this.registeredSections.ContainsKey(fieldName)) return null;

            // get a new instance of the provider and set its stack
            var provider = new TemplateWriter(writer as TemplateWriter); // as TemplateWriter;
            provider.stack = delegatingWriter.stack;

            // register the writer
            this.registeredSections.Add(fieldName, provider);

            // add the provider to the current valueSet
            this.valueSet.FieldWriters.Add(fieldName, new TemplateWriter(provider) as ITemplateWriter);

            // ensure all existing value sets contain the writer
            foreach (var valueSet in this.valueSets)
            {
                valueSet.FieldWriters.Add(fieldName, new TemplateWriter(provider) as ITemplateWriter);
            }

            return delegatingWriter as ITemplateWriter;
        }

        protected void WriteTextBlocks(StringBuilder sb, ValueSet valueSet)
        {

            foreach (var textBlock in this.template.TextBlocks)
            {
#if (DEBUG)
                WriteTrace(this, textBlock);
#endif
                if (textBlock.Type == TextBlockType.Text)
                {
                    sb.Append(textBlock.Text);
                }
                else if (textBlock.Type == TextBlockType.Section)
                {
                    List<string> extraText = null;

                    // get the writer for the given section
                    var writer = valueSet.SectionWriters[textBlock.ReferenceName] as TemplateWriter;

                    // get the extra text for the section to be written
                    if(writer.template.IsSingleLine && writer.HasData)
                    {
                        extraText = this.template.TextBlocks
                            .Where(b => b.ReferenceName == textBlock.ReferenceName
                                && (b.Type == TextBlockType.Prefix || b.Type == TextBlockType.Suffix))
                            .Select(b => b.TagText)
                            .ToList();
                    }

                    // write the extra text for the opening tag
                    if (extraText != null)
                    {
                        sb.Append(extraText[0]);
                        sb.Append(extraText[1]);
                    }

                    writer.GetContent(sb);

                    // write the extra text for the closing tag
                    if (extraText != null)
                    {
                        sb.Append(extraText[2]);
                        sb.Append(extraText[3]);
                    }
                }
                else if (textBlock.Type == TextBlockType.Field)
                {
                    if (this.registeredSections.ContainsKey(textBlock.ReferenceName))
                    {
                        var writer = valueSet.FieldWriters[textBlock.ReferenceName] as TemplateWriter;
                        writer.GetContent(sb);
                    }
                    else
                    {
                        var fieldValue = valueSet.FieldValues[textBlock.ReferenceName];
                        sb.Append(fieldValue);
                    }
                }
            }

        }

        #endregion

    }

}
