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

using MyOkapis.ObjectAccess;

namespace MyOkapis.ObjectMapping
{

    /// <summary>
    /// An abstract class expressing base mapper behavior for mapping a source object's data onto
    /// an accessible target object.
    /// </summary>
    /// <typeparam name="TSource">The type of the source object providing data for the mapping.</typeparam>
    /// <typeparam name="TTarget">The type of the target object to be populated.</typeparam>
    public abstract class Mapper<TSource, TTarget> : IMapper<TSource> where TTarget : IAccessibleObject
    {
        /// <summary>
        /// Maps data from the source object to a target object.
        /// </summary>
        /// <param name="source">The source object providing data for the mapping.</param>
        /// <returns>An instance of the target object with field values set.</returns>
        public abstract TTarget Map(TSource source);

        /// <inheritdoc />
        IAccessibleObject IMapper<TSource>.Map(TSource source) => Map(source);

        /// <summary>
        /// Sets field values on a target object from a collection of values keyed by field name.
        /// Fields with no matching key are left unchanged.
        /// </summary>
        /// <param name="target">The object on which data is to be set.</param>
        /// <param name="fieldValues">The collection of values to be used when setting field values.</param>
        /// <returns>The target object with field values set.</returns>
        public virtual TTarget SetFields(TTarget target, IReadOnlyDictionary<string, object?> fieldValues)
        {
            foreach (var field in target.FieldDescriptors)
            {
                if (!fieldValues.TryGetValue(field.Name, out var value)) continue;
                field.PropertyInfo.SetValue(target, value);
            }

            return target;
        }

        /// <inheritdoc />
        IAccessibleObject IMapper.SetFields(IAccessibleObject target, IReadOnlyDictionary<string, object?> fieldValues)
            => SetFields((TTarget)target, fieldValues);
    }

}
