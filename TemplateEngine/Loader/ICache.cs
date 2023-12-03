/* ****************************************************************************
Copyright 2018-2023 Gene Graves

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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateEngine.Document;

namespace TemplateEngine.Loader
{
    /// <summary>
    /// Interface defining basic caching functionality required for TemplateEngine.
    /// </summary>
    public interface ICache
    {
        /// <summary>
        /// Adds a cache entry synchronously
        /// </summary>
        /// <typeparam name="T">A type that implements ITemplate</typeparam>
        /// <param name="key">The cache lookup key for the object to be cached</param>
        /// <param name="item">The object to be cached</param>
        void Add<T>(string key, T item) where T : ITemplate;

        /// <summary>
        /// Looks up a cached object by the key and returns the object
        /// </summary>
        /// <typeparam name="T">A type that implements ITemplate</typeparam>
        /// <param name="key">The cache lookup key for the object to be retrieved</param>
        /// <returns>An object if found otherwise null</returns>
        T? Get<T>(string key) where T : ITemplate;

        /// <summary>
        /// Looks up a cached object by the key and returns the object if it is found.
        /// Otherwise an instance of the object is created by the factory, cached, and returned.
        /// </summary>
        /// <typeparam name="T">A type that implements ITemplate</typeparam>
        /// <param name="key">The cache lookup key for the object to be retrieved</param>
        /// <param name="factory">A function to obtain an object if the key is not found in the cache</param>
        /// <returns>The cached object associated with the key</returns>
        T GetOrAdd<T>(string key, Func<T> factory) where T : ITemplate;

        /// <summary>
        /// Looks up a cached object by the key and returns the object if it is found.
        /// Otherwise an instance of the object is created by the factory, cached, and returned.
        /// </summary>
        /// <typeparam name="T">A type that implements ITemplate</typeparam>
        /// <param name="key">The cache lookup key for the object to be retrieved</param>
        /// <param name="factory">A function to obtain an object if the key is not found in the cache</param>
        /// <returns>The cached object associated with the key</returns>
        Task<T> GetOrAddAsync<T>(string key, Func<string, Task<T>> factory) where T : ITemplate;

        /// <summary>
        /// Removes the object associated with the key from the cache.
        /// </summary>
        /// <param name="key">The cache lookup key for the object to be removed</param>
        void Remove(string key);

    }
}
