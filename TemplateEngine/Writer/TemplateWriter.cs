/* ****************************************************************************
Copyright 2018-2022 Gene Graves

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
using TemplateEngine.Document;
using SectionGroup = System.Collections.Generic.Dictionary<string, TemplateEngine.Writer.ITemplateWriter>;

// TODO: move protected and public methods into proper sections

namespace TemplateEngine.Writer
{

    /// <summary>
    /// Provides methods for manipulating and filling template sections with data and returning the text of the 
    /// populated template
    /// </summary>
    public class TemplateWriter : ITemplateWriter
    {
        /// <summary>
        /// The template object on which this writer is based
        /// </summary>
        protected ITemplate template;

        /// <summary>
        /// The collection of main and child sections in this writer
        /// </summary>
        protected SectionGroup sections;

        /// <summary>
        /// A collection of psuedo-sections that are actually markup fields that have
        /// a writer registered as a field provider
        /// </summary>
        protected SectionGroup registeredSections;

        /// <summary>
        /// The stack containing the hierarchy of selected sections
        /// </summary>
        protected Stack<ITemplateWriter> stack;

        /// <summary>
        /// A unique identifier for a writer instance
        /// </summary>
        protected Guid writerId;

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
            stack = new Stack<ITemplateWriter>();
            stack.Push(this);
            sections = new SectionGroup();
            registeredSections = new SectionGroup();

            foreach (var sectionName in template.ChildSectionNames)
            {
                var childWriter = new TemplateWriter(template.GetTemplate(sectionName), stack);
                sections.Add(sectionName, childWriter);
            }

            InitializeValueSet();
            writerId = Guid.NewGuid();
        }

        /// <summary>
        /// Constructs a new TemplateWriter for a template
        /// </summary>
        /// <param name="template">The template on which the writer will operate</param>
        /// <param name="stack">The stack from the parent template</param>
        protected TemplateWriter(ITemplate template, Stack<ITemplateWriter> stack)
        {
            this.template = template;
            sections = new SectionGroup();
            registeredSections = new SectionGroup();

            foreach (var sectionName in template.ChildSectionNames)
            {
                var childWriter = new TemplateWriter(template.GetTemplate(sectionName));
                childWriter.stack = stack;
                sections.Add(sectionName, childWriter);
            }

            InitializeValueSet();
            writerId = Guid.NewGuid();
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        protected TemplateWriter() {}

        /// <summary>
        /// Creates a main TemplateWriter initialized with the incoming values
        /// </summary>
        /// <param name="template">The ITemplate to be managed by the writer</param>
        /// <param name="sections">The standard sections included in the writer</param>
        /// <param name="registeredSections">Any registered sections to be included in the writer</param>
        protected TemplateWriter(ITemplate template, SectionGroup sections, SectionGroup registeredSections)
        {
            this.template = template;
            this.sections = sections;
            this.registeredSections = registeredSections;
            stack = new Stack<ITemplateWriter>();
            stack.Push(this);
            InitializeValueSet();
            writerId = Guid.NewGuid();
        }

        /// <summary>
        /// Creates a child TemplateWriter initialized with the incoming values
        /// </summary>
        /// <param name="template">The ITemplate to be managed by the writer</param>
        /// <param name="sections">The standard sections included in the writer</param>
        /// <param name="registeredSections">Any registered sections to be included in the writer</param>
        /// <param name="stack">The initialized hierarchy of template writers</param>
        protected TemplateWriter(ITemplate template, SectionGroup sections, SectionGroup registeredSections, Stack<ITemplateWriter> stack)
        {
            this.template = template;
            this.sections = sections;
            this.registeredSections = registeredSections;
            this.stack = stack;
            InitializeValueSet();
            writerId = Guid.NewGuid();
        }

#if (DEBUG)
        private static List<string> traceResults = new List<string>();

        /// <summary>
        /// Clears all recorded trace entries (DEBUG build only)
        /// </summary>
        public static void ClearTrace() => traceResults.Clear();

        /// <summary>
        /// Enables the recording of trace entries (DEBUG build only)
        /// </summary>
        public static bool EnableTrace { get; set; }

        /// <summary>
        /// Outputs all recorded trace entries (DEBUG build only)
        /// </summary>
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
            if (stack == null) return;
            var continueFlag = true;

            while (continueFlag)
            {
                var deselectFlag = stack.Count > 1;
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

        ///// <summary>
        ///// Convenience method for casting derived types
        ///// </summary>
        ///// <typeparam name="T">ITemplateWriter or a derived type</typeparam>
        ///// <returns>Writer cast as the requested type</returns>
        //T As<T>() where T : ITemplateWriter
        //{
        //    return (T)this;
        //}

        /// <summary>
        /// Resets the data fields in the current section.
        /// </summary>
        public void Clear()
        {
            var currentWriter = (CurrentWriter as TemplateWriter);
            currentWriter.InitializeValueSet();
        }

        /// <summary>
        /// Checks if a named section exists as a child section or registered section
        /// </summary>
        /// <param name="sectionName">Name of the section being tested for existence</param>
        /// <returns>True if the section in question exists in this writer</returns>
        public bool ContainsSection(string sectionName)
        {
            return sections.ContainsKey(sectionName) || registeredSections.ContainsKey(sectionName);
        }

        /// <summary>
        /// Creates a shallow copy of a TemplateWriter instance.
        /// </summary>
        ///<param name="asMain">Causes the stack to be initialized as a main writer when true.
        ///Otherwise the stack of the base writer is copied to the new writer</param>
        public virtual ITemplateWriter Copy(bool asMain = false)
        {
            return asMain ? new TemplateWriter(template, sections, registeredSections) :
                new TemplateWriter(template, sections, registeredSections, stack);
        }

        /// <summary>
        /// A reference to the currently active template writer
        /// </summary>
        public ITemplateWriter CurrentWriter => stack?.Peek() ?? this;

        /// <summary>
        /// Deselects the current section and makes the parent section the new selected section
        /// </summary>
        public void DeselectSection()
        {
            if (stack.Count == 1) throw new InvalidOperationException("Cannot deselect the parent section");

            // clear any pending data from the current writer and remove it from the stack
            CurrentWriter.Clear();
            stack.Pop();
        }

        /// <summary>
        /// Generates template text with data fields populated. Optionally appends all sections of the
        /// hierarchy before generating the output.
        /// </summary>
        /// <param name="appendAll">Sets the append all behavior</param>
        /// <returns>A text document with template fields populated</returns>
        public string GetContent(bool appendAll = false)
        {
            if (appendAll && (stack != null)) AppendAll();
            var sb = new StringBuilder(template.RawLength);
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
            if (template.SectionName == sectionName)
                return this;

            // matches child section
            if (sections.ContainsKey(sectionName))
                return sections[sectionName];

            // matches registered section (field provider)
            if (registeredSections.ContainsKey(sectionName))
                return registeredSections[sectionName];

            // otherwise delegate the check to each child section
            foreach (TemplateWriter section in sections.Values)
            {
                var writer = section?.GetSection(sectionName);
                if (writer != null) return writer;
            }

            return null;
        }

        /// <summary>
        /// Gets a TemplateWriter for the requested section
        /// </summary>
        /// <param name="sectionName">Name of the section for which a writer is to be returned</param>
        /// <returns>Working copy of <see cref="ITemplateWriter" /> for the requested section</returns>
        public virtual ITemplateWriter GetWriter(string sectionName)
        {
            return GetSection(sectionName).Copy(true);
        }

        /// <summary>
        /// Indicates if the template contains any field data
        /// </summary>
        public bool HasData => valueSets.Any(v => v?.HasData ?? false);

#if(DEBUG)

        /// <summary>
        /// Shows the hierarchy of writers for the selected sections (DEBUG build only)
        /// </summary>
        /// <returns>An ordered collection of writers and their associated sections</returns>
        public List<string> Inspect()
        {
            var level = $"{WriterId}:{SectionName}";
            var inspection = new List<string> { level };

            foreach (var section in sections)
            {
                foreach (var sectionSet in valueSets.Select(v => v.SectionWriters))
                {
                    ((TemplateWriter)sectionSet[section.Key]).Inspect(inspection, level);
                }
            }

            return inspection;
        }

        /// <summary>
        /// Recursively adds writers to the inspection hierarchy (DEBUG build only)
        /// </summary>
        /// <param name="inspection">The List of writers in the hierarchy</param>
        /// <param name="parentLevel">The metadata for the parent level for which
        /// children will be added to the hierarchy</param>
        protected void Inspect(List<string> inspection, string parentLevel)
        {
            var level = $"{parentLevel}--->{WriterId}:{SectionName}";
            inspection.Add(level);

            foreach (var section in sections)
            {
                foreach (var sectionSet in valueSets.Select(v => v.SectionWriters))
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
        /// The names of all registered field providers
        /// </summary>
        public IEnumerable<string> RegisteredFieldProviders => registeredSections.Keys;

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
            if (!template.FieldNames.Contains(fieldName)) return false;

            // skip if the field already has a writer registered to it
            if (registeredSections.ContainsKey(fieldName)) return false;

            // get a new instance of the provider and set its stack
            var provider = writer.Copy();

            // register the writer
            registeredSections.Add(fieldName, provider);

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
            if (template.SectionName != sectionName)
            {
                // get the requested child section
                var childWriter = GetSection(sectionName); // as TemplateWriter;

                // skip if registering a template writer for a non-existent section or field
                if (childWriter == null) return false;

                // delegate the call
                return childWriter.RegisterFieldProvider(fieldName, writer);
            }

            // otherwise register the writer to this section
            return RegisterFieldProvider(fieldName, writer);
        }

        /// <summary>
        /// Section name associated with this writer
        /// </summary>
        public string SectionName => template.SectionName;

        /// <summary>
        /// Selects a provider that will act as the current section
        /// </summary>
        /// <param name="fieldName">Name of the field to which the provider is bound</param>
        public void SelectProvider(string fieldName)
        {
            var cw = CurrentWriter as TemplateWriter;
            var provider = cw.GetProvider(fieldName) as TemplateWriter;
            provider.IsProvider = true;
            stack.Push(provider);
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
            stack.Push(writer);
        }

        /// <summary>
        /// Name of the currently selected section
        /// </summary>
        public string SelectedSectionName
        {
            get
            {
                var currentTemplate = (stack == null) ? template : ((TemplateWriter)stack.Peek()).template;
                return currentTemplate.SectionName;
            }
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
            SetMultiSectionFields(data);
            DeselectSection();
        }

        /// <summary>
        /// Sets section fields and appends the section once for each object in the data collection
        /// </summary>
        /// <typeparam name="T">Type of the data object</typeparam>
        /// <param name="data">Data collection</param>
        public void SetMultiSectionFields<T>(IEnumerable<T> data)
        {

            foreach (T row in data)
            {
                ViewModelAccessor<T> accessor = new ViewModelAccessor<T>(row);

                foreach (KeyValuePair<string, string> kvp in accessor.FieldValues)
                {
                    SetField(kvp.Key, kvp.Value);
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
            SetSectionFields(data, SectionOptions.AppendDeselect);
        }

        /// <summary>
        /// Sets section fields from a data object
        /// </summary>
        /// <typeparam name="T">Type of the data object</typeparam>
        /// <param name="sectionName">Name of the section to set</param>
        /// <param name="data">Data object</param>
        /// <param name="sectionOptions"><see cref="SectionOptions" /> for desired append and deselect behavior</param>
        /// <param name="fieldDefinitions"><see cref="FieldDefinitions" /> object that defines special fields</param>
        public void SetSectionFields<T>(string sectionName, T data, SectionOptions sectionOptions)
        {
            SelectSection(sectionName);
            SetSectionFields(data, sectionOptions);
        }

        /// <summary>
        /// Sets section fields from a data object
        /// </summary>
        /// <typeparam name="T">Type of the data object</typeparam>
        /// <param name="data">Data object</param>
        /// <param name="sectionOptions"><see cref="SectionOptions" /> for desired append and deselect behavior</param>
        public void SetSectionFields<T>(T data, SectionOptions sectionOptions)
        {
            ViewModelAccessor<T> accessor = new ViewModelAccessor<T>(data);

            foreach (KeyValuePair<string, string> kvp in accessor.FieldValues)
            {
                SetField(kvp.Key, kvp.Value);
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
        public virtual void SetField(string key, string val)
        {
            var currentWriter = CurrentWriter as TemplateWriter;

            try
            {
                currentWriter.valueSet.FieldValues[key] = val;
            }
            catch (KeyNotFoundException ex)
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
        /// The template the writer is managing
        /// </summary>
        public ITemplate Template => template;

        /// <summary>
        /// Unique id of the template bound to this writer
        /// </summary>
        public Guid TemplateId => template.TemplateId;

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
            foreach (var valueSet in valueSets)
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
                return valueSet.SectionWriters[sectionName];
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

            if (valueSet.FieldWriters.TryGetValue(fieldName, out var fieldWriter))
                return fieldWriter;

            if (!registeredSections.TryGetValue(fieldName, out var registeredProvider))
            {
                var msg = $"A registered provider for field, {fieldName}, was not found in this section.";
                throw new ArgumentException(msg);
            }

            var provider = registeredProvider.Copy();
            valueSet.FieldWriters[fieldName] = provider;

            return provider;
        }

        /// <summary>
        /// Creates and initializes a new working value set
        /// </summary>
        protected void InitializeValueSet()
        {
            var newValueSet = new ValueSet();

            newValueSet.FieldValues = template.FieldNames.ToDictionary(n => n, n => (string)null);
            newValueSet.FieldWriters = registeredSections.ToDictionary(s => s.Key, s => s.Value.Copy());
            newValueSet.SectionWriters = sections.ToDictionary(s => s.Key, s => s.Value.Copy());

            valueSet = newValueSet;
        }

        /// <summary>
        /// Indicates if this template is registered as a field provider
        /// </summary>
        public bool IsProvider { get; protected set; }

        /// <summary>
        /// Unique id of this instance
        /// </summary>
        public Guid WriterId => writerId;

        /// <summary>
        /// A delegate to allow child classes to handle literal text blocks differently
        /// </summary>
        protected Action<StringBuilder, TextBlock> WriteLiteralTextBlock { get; set; }

        /// <summary>
        /// Generates output from all of the <see cref="TextBlock"/> in the writer's template
        /// </summary>
        /// <param name="sb"><see cref="StringBuilder"/> to which the output will be written</param>
        /// <param name="valueSet"><see cref="ValueSet"/> to be used for populating the writer's template</param>
        protected void WriteTextBlocks(StringBuilder sb, ValueSet valueSet)
        {

            foreach (var textBlock in template.TextBlocks)
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
                    if (writer.template.IsSingleLine && hasData && extraText == null)
                    {
                        extraText = template.TextBlocks
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
                    if (registeredSections.ContainsKey(textBlock.ReferenceName))
                    {
                        if (valueSet.FieldWriters.TryGetValue(textBlock.ReferenceName, out var writer))
                            (writer as TemplateWriter).GetContent(sb);
                    }
                    else
                    {
                        var fieldValue = valueSet.FieldValues[textBlock.ReferenceName];
                        sb.Append(fieldValue);
                    }
                }
                else if (textBlock.Type == TextBlockType.Literal)
                {
                    // use the default handler for literal text blocks unless an alternate handler is defined
                    if (WriteLiteralTextBlock == null)
                        sb.Append(textBlock.Text);
                    else
                        WriteLiteralTextBlock(sb, textBlock);
                }
            }

        }

        #endregion

    }

}
