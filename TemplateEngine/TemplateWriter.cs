/* ****************************************************************************
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
        private Stack<ITemplateWriter> stack;

        /// <summary>
        /// The current, unappended value set for this writer
        /// </summary>
        protected ValueSet valueSet = new ValueSet();

        /// <summary>
        /// The collection of appended value sets for this writer
        /// </summary>
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
            this.sections = new Dictionary<string, ITemplateWriter>();
            this.registeredSections = new Dictionary<string, ITemplateWriter>();
            PrepareSections();
            InitializeValueSet();
            this.WriterId = Guid.NewGuid();
        }

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

        /// <summary>
        /// Append and deselect all sections in the hierarchy beginning with the current section and ending
        /// at the section designated by the sectionName parameter. If no sectionName parameter is provided
        /// then the entire hierarchy is appended.
        /// </summary>
        /// <param name="sectionName">Name of the last section to append</param>
        public void AppendAll(string sectionName = null)
        {
            if (this.stack == null) return;
            var continueFlag = true;

            while (continueFlag)
            {
                var deselectFlag = this.stack.Count > 1;
                continueFlag = deselectFlag && (SelectedSectionName != sectionName);
                AppendSection();
                if (deselectFlag) DeselectSection();
            }
        }

        /// <summary>
        /// Appends the current section and optionally deselects the current section.
        /// </summary>
        /// <param name="deselect">Sets the deselect behavior</param>
        public void AppendSection(bool deselect = false)
        {
            AppendSection();
            if (deselect) DeselectSection();
        }

        /// <summary>
        /// Resets the data fields in the current section.
        /// </summary>
        public void Clear()
        {
            var currentWriter = (CurrentWriter as TemplateWriter);
            currentWriter.InitializeValueSet();
        }

        public ITemplateWriter CurrentWriter => this.stack?.Peek() ?? this;

        /// <summary>
        /// A reference to the currently active template writer
        /// </summary>
        public ITemplateWriter CurrentWriter => this.stack?.Peek() ?? this;

        /// <summary>
        /// Deselects the current section and makes the parent section the new selected section
        /// </summary>
        public void DeselectSection()
        {
            if (this.stack.Count == 1) throw new InvalidOperationException("Cannot deselect the parent section");

            // clear any pending data from the current writer and remove it from the stack
            CurrentWriter.Clear();
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
            var sb = new StringBuilder(this.template.RawLength);
            GetContent(sb);
            return sb.ToString();
        }

        /// <summary>
        /// Gets a TemplateWriter for the requested section
        /// </summary>
        /// <param name="sectionName">Name of the section for which a writer is to be returned</param>
        /// <returns><see cref="ITemplateWriter" /> for the requested section</returns>
        protected ITemplateWriter GetSection(string sectionName)
        {

            // matches current section
            if (this.template.SectionName == sectionName)
                return this;

            // matches child section
            if (this.sections.ContainsKey(sectionName))
                return this.sections[sectionName];

            // matches registered section (field provider)
            if (this.registeredSections.ContainsKey(sectionName))
            return this.registeredSections[sectionName];
            
            // otherwise delegate the check to each child section
            foreach (var section in this.sections.Values)
            {
                var writer = (section as TemplateWriter)?.GetSection(sectionName);
                if (writer != null) return writer;
            }

            return null;
        }

        /// <summary>
        /// Gets a TemplateWriter for the requested section
        /// </summary>
        /// <param name="sectionName">Name of the section for which a writer is to be returned</param>
        /// <param name="asMain">When true the writer is created as a standalone writer</param>
        /// <returns>Working copy of <see cref="ITemplateWriter" /> for the requested section</returns>
        public ITemplateWriter GetWriter(string sectionName, bool asMain = false)
        {
            var section = GetSection(sectionName) as TemplateWriter;
            var writer = new TemplateWriter(section);

            if (asMain)
            {
                writer.stack = new Stack<ITemplateWriter>();
                writer.stack.Push(writer);
            }

            return writer;
        }

        /// <summary>
        /// Indicates if the template contains any field data
        /// </summary>
        public bool HasData => this.valueSets.Any(v => v?.HasData ?? false);

#if(DEBUG)
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
#endif

        /// <summary>
        /// Removes all populated field data and clears the current section
        /// </summary>
        public void Reset()
        {
            valueSets.Clear();
            InitializeValueSet();
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

            // get a new instance of the provider and set its stack
            var provider = new TemplateWriter(writer as TemplateWriter) as TemplateWriter;
            provider.stack = this.stack;

            // register the writer
            this.registeredSections.Add(fieldName, provider);

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
            if (this.template.SectionName != sectionName)
            {
                // get the requested child section
                var childWriter = this.GetSection(sectionName) as TemplateWriter;

                // skip if registering a template writer for a non-existent section or field
                if (childWriter == null) return false;

                // delegate the call
                return childWriter.RegisterFieldProvider(fieldName, writer);
            }

            // otherwise register the writer to this section
            return this.RegisterFieldProvider(fieldName, writer);
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
            var cw = CurrentWriter as TemplateWriter;
            var provider = cw.GetProvider(fieldName) as TemplateWriter;
            provider.IsProvider = true;
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
            var writer = currentWriter.GetChildSection(sectionName) as TemplateWriter;
            if (writer.stack == null) writer.stack = currentWriter.stack;
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
            DeselectSection();
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
            }

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
        /// <param name="sectionOptions"><see cref="SectionOptions" /> for desired append and deselect behavior</param>
        /// <param name="fieldDefinitions"><see cref="FieldDefinitions" /> object that defines special fields</param>
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
                    DropdownDefinition definition = definitions.Dropdowns
                        .Where(d => d.FieldName == kvp.Key)
                        .FirstOrDefault();

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
            var currentWriter = CurrentWriter as TemplateWriter;

            try
            {
                currentWriter.valueSet.FieldValues[key] = val;
            }
            catch(KeyNotFoundException ex)
            {
                var msg = $"The field, {key}, was not found in the selected template.";
                throw new ArgumentException(msg, ex);
            }
        }

        /// <summary>
        /// Sets a field from a generic value
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

        /// <summary>
        /// Appends the current section and optionally deselects the current section.
        /// </summary>
        protected void AppendSection()
        {
            var currentWriter = (CurrentWriter as TemplateWriter);
            currentWriter.AppendValueSet();
        }

        /// <summary>
        /// Appends the working value set and initializes a new working value set
        /// </summary>
        protected void AppendValueSet()
        {
            if (valueSets == null) valueSets = new List<ValueSet>();
            valueSets.Add(valueSet);
            InitializeValueSet();
        }

        /// <summary>
        /// Iterates the appended value sets and merges their data into the template
        /// </summary>
        /// <param name="sb"><see cref="StringBuilder"/> to which the populated template is written</param>
        protected void GetContent(StringBuilder sb)
        {
            // iterate each set of appended data and write a copy of the template containing the data
            foreach (var valueSet in this.valueSets)
            {
                WriteTextBlocks(sb, valueSet);
            }

            Reset();
        }

        /// <summary>
        /// Returns the template writer for a child section
        /// </summary>
        /// <param name="sectionName">The name of the child section</param>
        /// <returns>The child section's template writer</returns>
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

        /// <summary>
        /// Gets the registered provider for a field
        /// </summary>
        /// <param name="fieldName">The name of the field that has the provider registered</param>
        /// <returns>The template writer registered to the field</returns>
        protected ITemplateWriter GetProvider(string fieldName)
        {

            if(this.valueSet.FieldWriters.TryGetValue(fieldName, out var fieldWriter))
                return fieldWriter;

            if (!this.registeredSections.TryGetValue(fieldName, out var registeredProvider))
            {
                var msg = $"A registered provider for field, {fieldName}, was not found in this section.";
                throw new ArgumentException(msg);
            }

            var provider = new TemplateWriter(registeredProvider as TemplateWriter);
            this.valueSet.FieldWriters[fieldName] = provider;

            return provider;
        }

        /// <summary>
        /// Creates and initializes a new working value set
        /// </summary>
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

        /// <summary>
        /// Indicates if this template is registered as a field provider
        /// </summary>
        public bool IsProvider { get; protected set; }

        /// <summary>
        /// Adds a child writer to the current writer for each child section in the current template
        /// </summary>
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

        /// <summary>
        /// Unique id of this instance
        /// </summary>
        public Guid WriterId { get; }

        /// <summary>
        /// Generates output from all of the <see cref="TextBlock"/> in the writer's template
        /// </summary>
        /// <param name="sb"><see cref="StringBuilder"/> to which the output will be written</param>
        /// <param name="valueSet"><see cref="ValueSet"/> to be used for populating the writer's template</param>
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
                    var hasData = writer.HasData;

                    // get the extra text for the section to be written
                    if(writer.template.IsSingleLine && hasData && extraText == null)
                    {
                        extraText = this.template.TextBlocks
                            .Where(b => b.ReferenceName == textBlock.ReferenceName
                                && (b.Type == TextBlockType.Prefix || b.Type == TextBlockType.Suffix))
                            .Select(b => b.TagText)
                            .ToList();
                    }

                    // write the extra text for the opening tag
                    if (extraText != null && hasData)
                    {
                        sb.Append(extraText[0]);
                        sb.Append(extraText[1]);
                    }

                    writer.GetContent(sb);

                    // write the extra text for the closing tag
                    if (extraText != null && hasData)
                    {
                        sb.Append(extraText[2]);
                        sb.Append(extraText[3]);
                    }
                }
                else if (textBlock.Type == TextBlockType.Field)
                {
                    if (this.registeredSections.ContainsKey(textBlock.ReferenceName))
                    {
                        if(valueSet.FieldWriters.TryGetValue(textBlock.ReferenceName, out var writer))
                            (writer as TemplateWriter).GetContent(sb);
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
