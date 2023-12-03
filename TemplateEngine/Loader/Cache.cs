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
using LazyCache;
using TemplateEngine.Document;

namespace TemplateEngine.Loader
{
    /// <summary>
    /// Default implementation of ICache. Wraps LazyCache.
    /// </summary>
    public class Cache : ICache
    {
        private IAppCache cache;

        /// <summary>
        /// Creates a wrapped instance of LazyCache
        /// </summary>
        public Cache()
        {
            cache = new CachingService();
        }

        /// <summary>
        /// Adds a cache entry synchronously
        /// </summary>
        /// <typeparam name="T">A type that implements ITemplate</typeparam>
        /// <param name="key">The cache lookup key for the object to be cached</param>
        /// <param name="item">The object to be cached</param>
        public void Add<T>(string key, T item) where T : ITemplate
        {
            cache.Add(key, item);
        }

        /// <summary>
        /// Looks up a cached object by the key and returns the object
        /// </summary>
        /// <typeparam name="T">A type that implements ITemplate</typeparam>
        /// <param name="key">The cache lookup key for the object to be retrieved</param>
        /// <returns>An object if found otherwise null</returns>
        public T Get<T>(string key) where T : ITemplate
        {
            return cache.Get<T>(key);
        }

        /// <summary>
        /// Looks up a cached object by the key and returns the object if it is found.
        /// Otherwise an instance of the object is created by the factory, cached, and returned.
        /// </summary>
        /// <typeparam name="T">A type that implements ITemplate</typeparam>
        /// <param name="key">The cache lookup key for the object to be retrieved</param>
        /// <param name="factory">A function to obtain an object if the key is not found in the cache</param>
        /// <returns>The cached object associated with the key</returns>
        public T GetOrAdd<T>(string key, Func<T> factory) where T : ITemplate
        {
            return cache.GetOrAdd<T>(key, factory);
        }

        /// <summary>
        /// Looks up a cached object by the key and returns the object if it is found.
        /// Otherwise an instance of the object is created by the factory, cached, and returned.
        /// </summary>
        /// <typeparam name="T">A type that implements ITemplate</typeparam>
        /// <param name="key">The cache lookup key for the object to be retrieved</param>
        /// <param name="factory">A function to obtain an object if the key is not found in the cache</param>
        /// <returns>The cached object associated with the key</returns>
        public async Task<T> GetOrAddAsync<T>(string key, Func<string, Task<T>> factory) where T : ITemplate
        {
            return await cache.GetOrAddAsync<T>(key, () => factory.Invoke(key));
        }

        /// <summary>
        /// Removes the object associated with the key from the cache.
        /// </summary>
        /// <param name="key">The cache lookup key for the object to be removed</param>
        public void Remove(string key)
        {
            cache.Remove(key);
        }
    }
}
