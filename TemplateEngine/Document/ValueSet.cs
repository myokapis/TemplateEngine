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

using System.Collections.Generic;
using System.Linq;
using TemplateEngine.Writer;

namespace TemplateEngine.Document
{

    /// <summary>
    /// 
    /// </summary>
    public class ValueSet
    {
        /// <summary>
        /// Gets or sets the value of a field
        /// </summary>
        /// <remarks>Field name is the key</remarks>
        public Dictionary<string, string?> FieldValues { get; set; } = new Dictionary<string, string?>();

        /// <summary>
        /// Gets or sets the field writer associated with a field
        /// </summary>
        /// <remarks>Field name is the key</remarks>
        public Dictionary<string, ITemplateWriter> FieldWriters { get; set; } = new Dictionary<string, ITemplateWriter>();

        /// <summary>
        /// Gets or sets the section writer associated with a section
        /// </summary>
        /// <remarks>Section name is the key</remarks>
        public Dictionary<string, ITemplateWriter> SectionWriters { get; set; } = new Dictionary<string, ITemplateWriter>();

        /// <summary>
        /// Empties the value set of all field values, field writers, and section writers
        /// </summary>
        public void Clear()
        {
            FieldValues.Clear();
            FieldWriters.Clear();
            SectionWriters.Clear();
        }

        /// <summary>
        /// Returns true if the value set has been populated
        /// </summary>
        public bool HasData => FieldValues.Any(v => v.Value != null)
            || FieldWriters.Count > 0
            || SectionWriters.Count > 0;

    }

}
