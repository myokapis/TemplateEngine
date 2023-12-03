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

namespace TemplateEngine.Web
{

    /// <summary>
    /// Contains the display text and field value for a dropdown item
    /// </summary>
    public struct Option
    {
        // TODO: allow this in next version
        //public Option() { }

        /// <summary>
        /// Constructs an Option instance
        /// </summary>
        /// <param name="text">The text to be displayed in the dropdown option</param>
        /// <param name="value">The value associated with the dropdown option</param>
        public Option(string text, string value)
        {
            Text = text;
            Value = value;
        }

        /// <summary>
        /// The text to be displayed in the dropdown option
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// The value associated with the dropdown option
        /// </summary>
        public string Value { get; set; }
    }

}
