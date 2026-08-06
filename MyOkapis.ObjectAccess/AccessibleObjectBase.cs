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

using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace MyOkapis.ObjectAccess
{

    /// <summary>
    /// Optional base class for a type that wants to directly implement <see cref="IAccessibleObject{T}"/>
    /// instead of being wrapped by <see cref="AccessibleObject{T}"/>. Prefer this when you own the
    /// type and want polymorphic access via <see cref="IAccessibleObject"/> across a collection of
    /// different accessible types, or static field metadata with no instance in hand. Prefer the
    /// <see cref="AccessibleObject{T}"/> wrapper for types you don't own, or don't want coupled to
    /// this library.
    /// </summary>
    /// <remarks>
    /// Delegates to <see cref="AccessibleObject{T}"/>'s cache rather than reflecting independently,
    /// so both patterns share one cached field list per type. FieldCount/FieldDescriptors are marked
    /// NotMapped/JsonIgnore/XmlIgnore so general-purpose reflection-based serializers (including
    /// System.Text.Json and System.Xml.Serialization, which the JSON/XML serializers in
    /// MyOkapis.ObjectSerialization call into directly) don't try to serialize this bookkeeping —
    /// FieldDescriptors in particular carries raw PropertyInfo, which isn't serializable at all.
    /// </remarks>
    /// <typeparam name="T">The implementing type itself</typeparam>
    public abstract class AccessibleObjectBase<T> : IAccessibleObject<T>
        where T : AccessibleObjectBase<T>, IAccessibleObject<T>
    {
        /// <summary>
        /// Metadata about each of the type's accessible properties. Available without an instance.
        /// </summary>
        public static IReadOnlyList<FieldDescriptor> Fields => AccessibleObject<T>.Fields;

        /// <summary>
        /// Count of accessible property fields on the object
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        [XmlIgnore]
        public int FieldCount => Fields.Count;

        /// <summary>
        /// Metadata about each of the accessible properties
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        [XmlIgnore]
        public IEnumerable<FieldDescriptor> FieldDescriptors => Fields;
    }

}
