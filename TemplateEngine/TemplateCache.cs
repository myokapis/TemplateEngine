/* ****************************************************************************
Copyright 2018 Gene Graves

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
using System.Collections.Concurrent;
using System.Linq;

namespace TemplateEngine
{

    public interface ITemplateCache
    {
        void ClearCache();
        ITemplate GetTemplate(string fileName);
        bool IsTemplateCached(string fileName);
    }

    /// <summary>
    /// Cache that stores templates for retrieval.
    /// </summary>
    public class TemplateCache : ITemplateCache
    {
        protected int expiresAfterSeconds;
        protected int flushIntervalSeconds;
        protected DateTime lastFlushTime;
        protected ITemplateLoader templateLoader;
        protected ConcurrentDictionary<string, CachedTemplate> cache = new ConcurrentDictionary<string, CachedTemplate>();

        /// <summary>
        /// Constructs a new cache object
        /// </summary>
        /// <param name="templateLoader">An <cref="ITemplateLoader" /> to provide template files for parsing</param>
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

        protected CachedTemplate CreateCachedTemplate(string fileName)
        {
            var template = new Template(this.templateLoader.LoadTemplate(fileName));
            return new CachedTemplate(template, this.expiresAfterSeconds);
        }

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

        protected struct CachedTemplate
        {
            public CachedTemplate(Template template, int expiresAfterSeconds)
            {
                this.Template = template;
                this.Expires = DateTime.UtcNow.AddSeconds(expiresAfterSeconds);
            }

            public DateTime Expires { get; set; }
            public ITemplate Template { get; set; }
            
        }

    }

}
