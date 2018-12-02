using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TemplateEngine
{
    public class FieldDefinitions
    {
        private Dictionary<string, DropdownDefinition> dropdownDefinitions = new Dictionary<string, DropdownDefinition>();

        public FieldDefinitions() { }

        public FieldDefinitions(IEnumerable<string> checkboxes, IEnumerable<DropdownDefinition> dropdowns)
        {
            if (checkboxes != null) Checkboxes = checkboxes.ToArray();
            if (dropdowns != null) dropdownDefinitions = dropdowns.ToDictionary(d => d.FieldName, d => d);
        }

        public IEnumerable<string> Checkboxes { get; private set; } = new string[0];

        public IEnumerable<DropdownDefinition> Dropdowns => this.dropdownDefinitions.Values;

        public IEnumerable<string> DropdownFieldNames => this.dropdownDefinitions.Keys;

        public void SetCheckboxes(params string[] fieldNames)
        {
            Checkboxes = fieldNames;
        }

        public void SetDropdowns(params DropdownDefinition[] dropdowns)
        {
            dropdownDefinitions = dropdowns.ToDictionary(d => d.FieldName, d => d);
        }

    }

    public struct DropdownDefinition
    {
        public List<Option> Data { get; set; }
        public string FieldName { get; set; }
        public string SectionName { get; set; }
    }

    public class Option
    {
        public string Text { get; set; }
        public string Value { get; set; }
    }
}
