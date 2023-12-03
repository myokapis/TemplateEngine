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

using System;
using System.Collections.Generic;

namespace TemplateEngine.Document
{

    /// <summary>
    /// Interface defining the methods and properties of a template
    /// </summary>
    public interface ITemplate
    {
        /// <summary>
        /// Names of all of the child sections within this template
        /// </summary>
        List<string> ChildSectionNames { get; }

        /// <summary>
        /// Names of all of the fields within this template
        /// </summary>
        List<string> FieldNames { get; }

        /// <summary>
        /// Flag indicating if the template is an empty template
        /// </summary>
        bool IsEmpty { get; }

        /// <summary>
        /// Flag indicating if the section represented by this template begins and ends on a single line
        /// </summary>
        bool IsSingleLine { get; }

        /// <summary>
        /// Length of the raw text used to construct this template
        /// </summary>
        int RawLength { get; }

        /// <summary>
        /// Name of the section represented by this template
        /// </summary>
        string SectionName { get; }

        /// <summary>
        /// A unique identifier for this instance
        /// </summary>
        Guid TemplateId { get; }

        /// <summary>
        /// An identifier shared by all templates constructed from a particular template
        /// </summary>
        Guid TemplateTypeId { get; }

        /// <summary>
        /// A collection of <see cref="TextBlock"/> contained within this template
        /// </summary>
        IEnumerable<TextBlock> TextBlocks { get; }

        /// <summary>
        /// Creates a copy of this template
        /// </summary>
        /// <returns>A copy of the current template</returns>
        ITemplate Copy();

        /// <summary>
        /// Gets a copy of a template based on this template or a template representing a subsection of this template
        /// </summary>
        /// <param name="name">Section name identifying the template (null returns a copy of this template)</param>
        /// <returns>A copy of the requested template</returns>
        ITemplate GetTemplate(string name);

        /// <summary>
        /// Renders the template definition as text
        /// </summary>
        /// <returns>Template text</returns>
        string ToString();

    }

}
