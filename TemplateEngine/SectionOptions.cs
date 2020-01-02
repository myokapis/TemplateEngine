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

namespace TemplateEngine
{

    /// <summary>
    /// Defines a set of options to be used when writing a section
    /// </summary>
    public class SectionOptions
    {

        /// <summary>
        /// Static constructor creates the three valid combinations of options
        /// </summary>
        static SectionOptions()
        {
            AppendDeselect = new SectionOptions(true, true);
            AppendOnly = new SectionOptions(true, false);
            Set = new SectionOptions(false, false);
        }

        /// <summary>
        /// An option that indicates that a copy of the section should be appended after data has been populated
        /// then the section should be deselected
        /// </summary>
        public static SectionOptions AppendDeselect { get; }

        /// <summary>
        /// An option that indicates that a copy of the section should be appended after data has been populated
        /// then the section should remain selected
        /// </summary>
        public static SectionOptions AppendOnly { get; }

        /// <summary>
        /// An option that indicates the section should not be appended and should remain selected after data has 
        /// been populated
        /// </summary>
        public static SectionOptions Set { get; }

        /// <summary>
        /// Protected constructor for generating an instance with specific property settings
        /// </summary>
        /// <param name="append">Value to which the Append property will be set</param>
        /// <param name="deselect">Value to which the Deselect property will be set</param>
        protected SectionOptions(bool append, bool deselect)
        {
            this.Append = append;
            this.Deselect = deselect;
        }

        /// <summary>
        /// Flag to indicate if the section should be appended
        /// </summary>
        public bool Append { get; }

        /// <summary>
        /// Flag to indicate if the section should be deselected
        /// </summary>
        public bool Deselect { get; }
    }

}
