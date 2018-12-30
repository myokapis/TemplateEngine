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

using System.Collections.Generic;
using System.Linq;

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

    public readonly struct DropdownDefinition
    {
        public DropdownDefinition(string sectionName, string fieldName, List<Option> data)
        {
            this.Data = data;
            this.FieldName = fieldName;
            this.SectionName = sectionName;
        }

        public List<Option> Data { get; }
        public string FieldName { get; }
        public string SectionName { get; }
    }

    public class Option
    {
        public string Text { get; set; }
        public string Value { get; set; }
    }

}
