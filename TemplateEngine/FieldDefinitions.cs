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

using System.Collections.Generic;
using System.Linq;

namespace TemplateEngine
{

    /// <summary>
    /// Defines special fields such as dropdowns and checkboxes. Provides helper methods for rendering those fields
    /// </summary>
    public class FieldDefinitions
    {
        private Dictionary<string, DropdownDefinition> dropdownDefinitions = new Dictionary<string, DropdownDefinition>();

        /// <summary>
        /// Creates an empty FieldDefinitions object
        /// </summary>
        public FieldDefinitions() { }

        /// <summary>
        /// Creates a FieldDefinitions object containing a collection of checkboxes and dropdowns
        /// </summary>
        /// <param name="checkboxes">The collection of checkboxes to be rendered</param>
        /// <param name="dropdowns">The collection of dropdowns to be rendered</param>
        public FieldDefinitions(IEnumerable<string> checkboxes, IEnumerable<DropdownDefinition> dropdowns)
        {
            if (checkboxes != null) Checkboxes = checkboxes.ToArray();
            if (dropdowns != null) dropdownDefinitions = dropdowns.ToDictionary(d => d.FieldName, d => d);
        }

        /// <summary>
        /// The collection of checkboxes to be rendered
        /// </summary>
        public IEnumerable<string> Checkboxes { get; private set; } = new string[0];

        /// <summary>
        /// The collection of dropdowns to be rendered
        /// </summary>
        public IEnumerable<DropdownDefinition> Dropdowns => this.dropdownDefinitions.Values;

        /// <summary>
        /// The collection of field names associated with the dropdowns to be rendered
        /// </summary>
        public IEnumerable<string> DropdownFieldNames => this.dropdownDefinitions.Keys;

        /// <summary>
        /// Sets the list of checkboxes to be rendered
        /// </summary>
        /// <param name="fieldNames">The name of each checkbox field to be rendered</param>
        public void SetCheckboxes(params string[] fieldNames)
        {
            Checkboxes = fieldNames;
        }

        /// <summary>
        /// Sets the list of dropdowns to be rendered
        /// </summary>
        /// <param name="dropdowns">A collection of dropdown definitions for the dropdowns to be rendered</param>
        public void SetDropdowns(params DropdownDefinition[] dropdowns)
        {
            dropdownDefinitions = dropdowns.ToDictionary(d => d.FieldName, d => d);
        }

    }

    /// <summary>
    /// Contains information about a dropdown field
    /// </summary>
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

    /// <summary>
    /// Contains the display text and field value for a dropdown item
    /// </summary>
    public class Option
    {
        public string Text { get; set; }
        public string Value { get; set; }
    }

}
