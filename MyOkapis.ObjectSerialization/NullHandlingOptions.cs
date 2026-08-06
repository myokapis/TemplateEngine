/* ****************************************************************************
Copyright 2018-2026 Gene Graves

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

namespace MyOkapis.ObjectSerialization
{

    /// <summary>
    /// Controls how null field values are represented during structured (JSON/XML) serialization.
    /// </summary>
    public enum NullHandlingOptions
    {
        /// <summary>
        /// Omits fields with a null value from the output entirely.
        /// </summary>
        SuppressNull,

        /// <summary>
        /// Writes fields with a null value as an explicit null.
        /// </summary>
        PreserveNulls,

        /// <summary>
        /// Writes fields with a null value as an empty string.
        /// </summary>
        NullAsEmptyString
    }

}
