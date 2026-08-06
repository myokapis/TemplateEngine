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

namespace MyOkapis.ObjectAccess
{

    /// <summary>
    /// Describes a type whose properties can be discovered and accessed. Implemented directly by
    /// a type that owns its own field metadata (as opposed to <see cref="AccessibleObject{T}"/>,
    /// which wraps a type that has no such knowledge of itself).
    /// </summary>
    public interface IAccessibleObject
    {
        /// <summary>
        /// Count of accessible property fields on the object
        /// </summary>
        int FieldCount { get; }

        /// <summary>
        /// Metadata about each of the accessible properties
        /// </summary>
        IEnumerable<FieldDescriptor> FieldDescriptors { get; }
    }

    /// <summary>
    /// Describes a type whose properties can be discovered and accessed, including access to the
    /// field metadata at the type level with no instance required.
    /// </summary>
    /// <typeparam name="T">The implementing type itself</typeparam>
    public interface IAccessibleObject<T> : IAccessibleObject where T : IAccessibleObject<T>
    {
        /// <summary>
        /// Metadata about each of the type's accessible properties. Available without an instance.
        /// </summary>
        static abstract IReadOnlyList<FieldDescriptor> Fields { get; }
    }

}
