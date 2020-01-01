/* ****************************************************************************
Copyright 2018-2020 Gene Graves

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

// TODO: look at memcache instead of writing this beast from scratch

using System;
using System.Collections.Concurrent;
using System.Linq;

namespace TemplateEngine
{

    /// <summary>
    /// Interface defining methods and properties of a template cache
    /// </summary>
    public interface ITemplateCache
    {
        /// <summary>
        /// Forces the cache to be flushed of all expired and unexpired cached templates
        /// </summary>
        void ClearCache();

        /// <summary>
        /// Retrieves a copy of a template from cache or caches a new copy if the template was not found in the cache
        /// </summary>
        /// <param name="fileName">File name of the requested template</param>
        /// <returns>Copy of a template</returns>
        ITemplate GetTemplate(string fileName);

        /// <summary>
        /// Flag indicating if the requested template is currently in cache
        /// </summary>
        /// <param name="fileName">File name of the requested template</param>
        /// <returns>True or false</returns>
        bool IsTemplateCached(string fileName);
    }

    /// <summary>
    /// Cache that stores templates for retrieval.
    /// </summary>
    public class TemplateCache : ITemplateCache
    {
        /// <summary>
        /// Number of seconds to retain the template in cache
        /// </summary>
        protected int expiresAfterSeconds;

        /// <summary>
        /// Number of seconds between attempts to flush the cache
        /// </summary>
        protected int flushIntervalSeconds;

        /// <summary>
        /// Last time at which the cache was flushed
        /// </summary>
        protected DateTime lastFlushTime;

        /// <summary>
        /// Reference to an object for loading templates
        /// </summary>
        protected ITemplateLoader templateLoader;

        /// <summary>
        /// The template cache
        /// </summary>
        protected ConcurrentDictionary<string, CachedTemplate> cache = new ConcurrentDictionary<string, CachedTemplate>();

        /// <summary>
        /// Constructs a new cache object
        /// </summary>
        /// <param name="templateLoader">An <see cref="ITemplateLoader" /> to provide template files for parsing</param>
        /// <param name="expiresAfterSeconds">Number of seconds a template is considered fresh within the cache</param>
        /// <param name="flushIntervalSeconds">Number of seconds between cache flushes</param>
        public TemplateCache(ITemplateLoader templateLoader, int expiresAfterSeconds = 120, int flushIntervalSeconds = 120)
        {
            if (expiresAfterSeconds < 0) throw new ArgumentException("Expiration seconds must be greater than zero.");
            if (flushIntervalSeconds < 0) throw new ArgumentException("Flush interval must be greater than zero.");

            this.expiresAfterSeconds = expiresAfterSeconds;
            this.flushIntervalSeconds = flushIntervalSeconds;
            this.lastFlushTime = DateTime.UtcNow;
            this.templateLoader = templateLoader;
        }

        #region public methods

        /// <summary>
        /// Forces the cache to be flushed of all expired and unexpired cached templates
        /// </summary>
        public void ClearCache()
        {
            FlushCache(true);
        }

        /// <summary>
        /// Retrieves a copy of a template from cache or caches a new copy if the template was not found in the cache
        /// </summary>
        /// <param name="fileName">File name of the requested template</param>
        /// <returns>Copy of a template</returns>
        public ITemplate GetTemplate(string fileName)
        {
            // flush the cache
            if (this.lastFlushTime.AddSeconds(this.flushIntervalSeconds) < DateTime.UtcNow) FlushCache();

            // lookup the cached template
            var cachedTemplate = this.cache.GetOrAdd(fileName, (key) => CreateCachedTemplate(key));

            return cachedTemplate.Template.Copy();
        }

        /// <summary>
        /// Flag indicating if the requested template is currently in cache
        /// </summary>
        /// <param name="fileName">File name of the requested template</param>
        /// <returns>True or false</returns>
        public bool IsTemplateCached(string fileName)
        {
            // flush the cache
            if (this.lastFlushTime.AddSeconds(this.flushIntervalSeconds) < DateTime.UtcNow) FlushCache();

            return this.cache.ContainsKey(fileName);
        }

        #endregion

        #region protected methods

        /// <summary>
        /// Loads a template from a file and caches the template
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        protected CachedTemplate CreateCachedTemplate(string fileName)
        {
            var template = new Template(this.templateLoader.LoadTemplate(fileName));
            return new CachedTemplate(template, this.expiresAfterSeconds);
        }

        /// <summary>
        /// Flushes expired entries from the cache
        /// </summary>
        /// <param name="forceFlush">Forces all cached entries to be flushed</param>
        protected void FlushCache(bool forceFlush = false)
        {
            lock (this.cache)
            {
                var expiredKeys = this.cache
                    .Where(c => forceFlush || c.Value.Expires <= DateTime.UtcNow)
                    .Select(c => c.Key);

                foreach (var expiredKey in expiredKeys)
                {
                    this.cache.TryRemove(expiredKey, out var removedTemplate);
                }
            }
        }

        #endregion

        /// <summary>
        /// Contains a template and expiration date
        /// </summary>
        protected struct CachedTemplate
        {
            /// <summary>
            /// Creates an instance
            /// </summary>
            /// <param name="template">The template to be cached</param>
            /// <param name="expiresAfterSeconds">The number of seconds to retain the template in cache</param>
            public CachedTemplate(Template template, int expiresAfterSeconds)
            {
                this.Template = template;
                this.Expires = DateTime.UtcNow.AddSeconds(expiresAfterSeconds);
            }

            /// <summary>
            /// The number of seconds to retain the template in cache
            /// </summary>
            public DateTime Expires { get; set; }

            /// <summary>
            /// The template to be cached
            /// </summary>
            public ITemplate Template { get; set; }
            
        }

    }

}
