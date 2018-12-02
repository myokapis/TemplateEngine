using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace TemplateEngine
{

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

        protected TemplateWriter(ITemplate template, string sectionName)
        {
            this.template = template.GetTemplate(sectionName);
            this.sections = new Dictionary<string, ITemplateWriter>();
            PrepareSections();
            InitializeFieldValues();
            this.registeredSections = new Dictionary<string, ITemplateWriter>();
        }

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

        // TODO: change this to a param array and have the code iterate the selection params
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

        public void SetSectionFields<T>(string sectionName, T data) where T : IEnumerable<T>
        {
            SelectSection(sectionName);
            SetSectionFields<T>(data, SectionOptions.AppendDeselect, null);
        }

        public void SetSectionFields<T>(string sectionName, T data, FieldDefinitions fieldDefinitions) where T : IEnumerable<T>
        {
            SelectSection(sectionName);
            SetSectionFields<T>(data, SectionOptions.AppendDeselect, fieldDefinitions);
        }

        public void SetSectionFields<T>(string sectionName, T data, SectionOptions sectionOptions,
            FieldDefinitions fieldDefinitions = null) where T : IEnumerable<T>
        {
            SelectSection(sectionName);
            SetSectionFields<T>(data, sectionOptions, fieldDefinitions);
        }

        public void SetSectionFields<T>(T data, SectionOptions sectionOptions, FieldDefinitions fieldDefinitions = null) where T : IEnumerable<T>
        {
            FieldDefinitions definitions = (fieldDefinitions != null) ? fieldDefinitions : new FieldDefinitions();

            foreach (T row in data)
            {
                ViewModelAccessor<T> accessor = new ViewModelAccessor<T>(row);

                foreach (KeyValuePair<string, string> kvp in accessor.FieldValues)
                {
                    if (definitions.Checkboxes.Contains(kvp.Key))
                    {
                        // TODO: handle any case (upper, lower, mixed)
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

            }

            if (sectionOptions.Deselect) DeselectSection();

        }

        #endregion

        #region Field Setters For POCO

        public void SetSectionFields<T>(T data, string sectionName)
        {
            SelectSection(sectionName);
            SetSectionFields(SectionOptions.AppendDeselect, data, null);
        }

        public void SetSectionFields<T>(T data, string sectionName, FieldDefinitions fieldDefinitions)
        {
            SelectSection(sectionName);
            SetSectionFields(SectionOptions.AppendDeselect, data, fieldDefinitions);
        }

        public void SetSectionFields<T>(SectionOptions sectionOptions, T data, string sectionName, FieldDefinitions fieldDefinitions = null)
        {
            SelectSection(sectionName);
            SetSectionFields(sectionOptions, data, fieldDefinitions);
        }

        public void SetSectionFields<T>(SectionOptions sectionOptions, T data, FieldDefinitions fieldDefinitions = null)
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
                    // iterate each writer for the given section
                    foreach(var writer in this.sectionSets[textBlock.ReferenceName])
                    {
                        (writer as TemplateWriter).GetContent(sb);
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
