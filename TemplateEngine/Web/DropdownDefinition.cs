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

using System.Collections.Generic;

namespace TemplateEngine.Web
{

    /// <summary>
    /// Contains information about a dropdown field
    /// </summary>
    public readonly struct DropdownDefinition
    {
        /// <summary>
        /// Contains data and metadata for populating a dropdown
        /// </summary>
        /// <param name="sectionName">Template section containing the dropdown</param>
        /// <param name="fieldName">Field name from an external dataset that will provide the
        /// default value for the dropdown</param>
        /// <param name="data">The text and value pairs for each dropdown option</param>
        public DropdownDefinition(string sectionName, string fieldName, List<Option> data)
        {
            Data = data;
            FieldName = fieldName;
            SectionName = sectionName;
        }

        /// <summary>
        /// Text and value pairs for creating each dropdown option
        /// </summary>
        public List<Option> Data { get; }

        /// <summary>
        /// Field name from an external dataset that will provide the default value for the dropdown
        /// </summary>
        public string FieldName { get; }

        /// <summary>
        /// Template section containing the dropdown
        /// </summary>
        public string SectionName { get; }
    }

}
