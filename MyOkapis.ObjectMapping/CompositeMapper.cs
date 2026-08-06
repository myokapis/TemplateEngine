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
    /// An abstract class expressing mapper behavior for source objects that contain nested groups,
    /// each of which should be delegated to another mapper.
    /// </summary>
    /// <typeparam name="TSource">The type of the source object providing data for the mapping.</typeparam>
    /// <typeparam name="TTarget">The type of the target object to be populated.</typeparam>
    public abstract class CompositeMapper<TSource, TTarget> : Mapper<TSource, TTarget> where TTarget : IAccessibleObject
    {
        /// <summary>
        /// A delegate for resolving a child mapper instance by type.
        /// </summary>
        protected readonly MapperFactory<TSource> mapperFactory;

        /// <summary>
        /// Constructs an instance of the composite mapper class.
        /// </summary>
        /// <param name="mapperFactory">A delegate to resolve a nested mapper class by type.</param>
        protected CompositeMapper(MapperFactory<TSource> mapperFactory)
        {
            this.mapperFactory = mapperFactory;
        }

        /// <summary>
        /// Delegates mapping of a nested group to another mapper, resolved via the mapper factory.
        /// </summary>
        /// <typeparam name="TChildMapper">The type of the child mapper to which mapping will be delegated.</typeparam>
        /// <param name="source">The source object providing data for the nested group.</param>
        /// <returns>An instance of the nested group's target object, with field values set.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the child mapper cannot be resolved.</exception>
        protected virtual IAccessibleObject MapChild<TChildMapper>(TSource source) where TChildMapper : IMapper<TSource>
        {
            var mapper = mapperFactory(typeof(TChildMapper))
                ?? throw new InvalidOperationException($"No mapper of type {typeof(TChildMapper).Name} could be resolved.");

            return mapper.Map(source);
        }
    }

}
