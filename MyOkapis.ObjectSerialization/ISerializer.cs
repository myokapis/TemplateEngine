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

using System.Text;
using MyOkapis.ObjectAccess;

namespace MyOkapis.ObjectSerialization
{

    /// <summary>
    /// Describes the basic functionality of an object serializer.
    /// </summary>
    public interface ISerializer
    {
        /// <summary>
        /// Serializes an accessible object to a string.
        /// </summary>
        /// <param name="source">The object to be serialized.</param>
        /// <param name="options">Provides the options to be applied during serialization.</param>
        /// <returns>A string representing the original object.</returns>
        string Serialize(IAccessibleObject? source, SerializationOptions options);

        /// <summary>
        /// Serializes an accessible object to a string.
        /// </summary>
        /// <param name="source">The object to be serialized.</param>
        /// <param name="builder">A string builder into which the serialized object is written.</param>
        /// <param name="options">Provides the options to be applied during serialization.</param>
        void Serialize(IAccessibleObject? source, StringBuilder builder, SerializationOptions options);
    }

}
