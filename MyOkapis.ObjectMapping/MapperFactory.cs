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

namespace MyOkapis.ObjectMapping
{

    /// <summary>
    /// A delegate for resolving a mapper instance by type, e.g. from a DI container. Used by
    /// <see cref="CompositeMapper{TSource, TTarget}"/> to delegate nested object graphs to other
    /// mappers.
    /// </summary>
    /// <typeparam name="TSource">The type of the source object the resolved mapper accepts.</typeparam>
    /// <param name="mapperType">The type of the mapper to be resolved.</param>
    /// <returns>An instance of the requested mapper type, or null if it could not be resolved.</returns>
    public delegate IMapper<TSource>? MapperFactory<TSource>(Type mapperType);

}
