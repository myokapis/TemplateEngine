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

namespace MyOkapis.TemplateEngine.Writer
{

    /// <summary>
    /// An immutable collection of string tokens that are considered equivalent to True.
    /// </summary>
    public sealed class TrueValues
    {
        private readonly HashSet<string> values;

        /// <summary>
        /// Constructs a TrueValues instance from a collection of tokens to be considered equivalent to True.
        /// </summary>
        /// <param name="values">The tokens to be treated as equivalent to True. Comparisons are
        /// case-insensitive using ordinal rules.</param>
        public TrueValues(IEnumerable<string> values)
        {
            this.values = new HashSet<string>(values, StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// The default set of tokens considered equivalent to True: "Yes", "Y", "T", "True", "1"
        /// </summary>
        public static TrueValues Default { get; } = new TrueValues(new[] { "Yes", "Y", "T", "True", "1" });

        /// <summary>
        /// The tokens considered equivalent to True
        /// </summary>
        public IReadOnlySet<string> Values => values;

        /// <summary>
        /// Checks if a given value is equivalent to True
        /// </summary>
        /// <param name="value">Value to test</param>
        /// <returns>True or False</returns>
        public bool Contains(string value) => values.Contains(value);
    }

}
