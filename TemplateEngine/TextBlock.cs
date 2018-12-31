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

namespace TemplateEngine
{

    /// <summary>
    /// Contains information about a block of text in a template
    /// </summary>
    public readonly struct TextBlock
    {
        /// <summary>
        /// Creates a new text block
        /// </summary>
        /// <param name="type">Enum <cref="TextBlockType" /> for the type of the text block</param>
        /// <param name="text">Text belonging to the block</param>
        /// <param name="referenceName">The name associated with the text block</param>
        /// <param name="tagText">Text associated with the section tag</param>
        public TextBlock(TextBlockType type, string text, string referenceName = null, string tagText = null)
        {
            this.ReferenceName = referenceName;
            this.TagText = tagText;
            this.Text = text;
            this.Type = type;
        }

        /// <summary>
        /// The name associated with the text block 
        /// </summary>
        public string ReferenceName { get; }

        /// <summary>
        /// Text associated with the section tag
        /// </summary>
        public string TagText { get; }

        /// <summary>
        /// Text belonging to the block
        /// </summary>
        public string Text { get; }

        /// <summary>
        /// Enum <cref="TextBlockType" /> for the type of the text block
        /// </summary>
        public TextBlockType Type { get; }
    }

}
