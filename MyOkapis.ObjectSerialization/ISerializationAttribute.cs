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
    /// Describes properties to control serialization.
    /// </summary>
    public interface ISerializationAttribute
    {
        /// <summary>
        /// Forces a field terminator to be written even when no data is present in the field.
        /// </summary>
        bool AlwaysTerminate { get; init; }

        /// <summary>
        /// Indicates if the field contains sensitive data that should be masked.
        /// </summary>
        bool Masked { get; init; }

        /// <summary>
        /// Gets or sets the character to be used when masking field data.
        /// </summary>
        char MaskChar { get; init; }

        /// <summary>
        /// Gets or sets the character to be written when terminating a field.
        /// </summary>
        string? Terminator { get; init; }
    }

}
