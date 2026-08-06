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
using System.Linq;
using System.Reflection;
using System.Threading;

namespace MyOkapis.ObjectAccess
{

    /// <summary>
    /// Allows a data object's properties to be discovered, cached, and accessed by index or name,
    /// independent of how those values might subsequently be formatted or consumed. Any property
    /// marked with a [NotMapped] attribute is excluded.
    /// </summary>
    /// <typeparam name="T">Type of the data object</typeparam>
    public class AccessibleObject<T>
    {
        private static readonly Lazy<FieldDescriptor[]> fields =
            new Lazy<FieldDescriptor[]>(GetFieldDescriptors, LazyThreadSafetyMode.ExecutionAndPublication);

        /// <summary>
        /// Creates an accessor for a data object
        /// </summary>
        /// <param name="model">The data object whose properties will be accessed</param>
        public AccessibleObject(T model)
        {
            Model = model;
        }

        /// <summary>
        /// Count of accessible property fields on the data object
        /// </summary>
        public int Count => fields.Value.Length;

        /// <summary>
        /// The cached field descriptors for this type, shared by every instance
        /// </summary>
        public static IReadOnlyList<FieldDescriptor> Fields => fields.Value;

        /// <summary>
        /// The data object whose properties are being accessed
        /// </summary>
        public T Model { get; }

        /// <summary>
        /// Returns a field's raw, unformatted value based on a field index
        /// </summary>
        /// <param name="fieldIndex">The index of the field to be accessed</param>
        /// <returns>The property's value</returns>
        public object? this[int fieldIndex] => GetValue(fields.Value[fieldIndex]);

        /// <summary>
        /// Returns a field's raw, unformatted value based on a field name
        /// </summary>
        /// <param name="fieldName">The name of the field to be accessed</param>
        /// <returns>The property's value</returns>
        public object? this[string fieldName] => GetValue(fields.Value.First(f => f.Name == fieldName));

        /// <summary>
        /// Gets a field's raw, unformatted value from the underlying model
        /// </summary>
        /// <param name="field">The field descriptor identifying the property to read</param>
        /// <returns>The property's value</returns>
        public object? GetValue(in FieldDescriptor field) => field.PropertyInfo.GetValue(Model);

        /// <summary>
        /// Iterates each field descriptor together with its current, raw value
        /// </summary>
        public IEnumerable<(FieldDescriptor Field, object? Value)> Values
        {
            get
            {
                foreach (var descriptor in fields.Value)
                {
                    yield return (descriptor, GetValue(descriptor));
                }
            }
        }

        /// <summary>
        /// Finds the public properties of the type and caches each one alongside its custom
        /// attributes. Any properties marked with a [NotMapped] attribute are ignored, as are
        /// properties declared on <see cref="AccessibleObjectBase{T}"/> itself (bookkeeping
        /// members, not data, for types that opt into the direct-implementation pattern).
        /// </summary>
        protected static FieldDescriptor[] GetFieldDescriptors()
        {
            return typeof(T).GetProperties()
                .Where(pi => !Attribute.IsDefined(pi, typeof(NotMappedAttribute)))
                .Where(pi => !(pi.DeclaringType?.IsGenericType == true
                    && pi.DeclaringType.GetGenericTypeDefinition() == typeof(AccessibleObjectBase<>)))
                .Select(pi => new FieldDescriptor(pi))
                .ToArray();
        }
    }

}
