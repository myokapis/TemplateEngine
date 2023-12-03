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

namespace TemplateEngine.Document
{

    /// <summary>
    /// Enum indicating the type of a text block
    /// </summary>
    public enum TextBlockType
    {
        /// <summary>
        /// A template field that can be populated with data
        /// </summary>
        Field = 1,

        /// <summary>
        /// Whitespace prefixing a section
        /// </summary>
        Prefix = 2,

        /// <summary>
        /// A placeholder for a section
        /// </summary>
        Section = 4,

        /// <summary>
        /// An opening or closing section tag
        /// </summary>
        SectionTag = 8,

        /// <summary>
        /// Whitespacing following a section
        /// </summary>
        Suffix = 16,

        /// <summary>
        /// A block of text within a section
        /// </summary>
        Text = 32,

        /// <summary>
        /// A placeholder for a literal section
        /// </summary>
        Literal = 64
    }

}
