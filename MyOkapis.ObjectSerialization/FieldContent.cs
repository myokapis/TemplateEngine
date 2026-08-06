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

using MyOkapis.Formatting;

namespace MyOkapis.ObjectSerialization
{

    /// <summary>
    /// A data transfer object for passing a field's value and metadata through the serialization
    /// pipeline.
    /// </summary>
    public class FieldContent
    {
        /// <summary>
        /// The name of the field.
        /// </summary>
        public required string FieldName { get; init; }

        /// <summary>
        /// The format attribute attached to the field.
        /// </summary>
        public IFormatAttribute? FormatAttribute { get; init; }

        /// <summary>
        /// Indicates if the field should be ignored during serialization.
        /// </summary>
        public bool Ignore { get; init; } = false;

        /// <summary>
        /// The serialization attribute attached to the field.
        /// </summary>
        public ISerializationAttribute? SerializationAttribute { get; init; }

        /// <summary>
        /// The field's data value.
        /// </summary>
        public object? Value { get; init; }
    }

}
