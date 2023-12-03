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
using System.Threading.Tasks;
using LazyCache;
using TemplateEngine.Document;
using TemplateEngine.Writer;

namespace TemplateEngine.Loader
{

    /// <summary>
    /// Cache that stores templates for retrieval.
    /// </summary>
    public class TemplateCacheBase<IWriter> : TemplateLoaderBase<IWriter>, ITemplateCache<IWriter> where IWriter : class, ITemplateWriter
    {

        /// <summary>
        /// The instance of ICache in which templates are to be stored
        /// </summary>
        protected readonly ICache cache;

        /// <summary>
        /// Constructs a new cache object
        /// </summary>
        /// <param name="templateDirectory">The directory in which template files are stored</param>
        /// <param name="templateFactory">A function to create a new template</param>
        /// <param name="writerFactory">A function to create a new writer</param>
        /// <param name="cache">Cache instance to be used for storing templates</param>
        public TemplateCacheBase(string templateDirectory, Func<string, ITemplate> templateFactory, Func<ITemplate, IWriter> writerFactory, ICache cache) :
            base(templateDirectory, templateFactory, writerFactory)
        {
            this.cache = cache;
        }

        /// <summary>
        /// Constructs a new cache object
        /// </summary>
        /// <param name="templateDirectory">The directory in which template files are stored</param>
        /// <param name="templateFactory">A function to create a new template</param>
        /// <param name="writerFactory">A function to create a new writer</param>
        /// <param name="cacheFactory">A function to create a cache instance to be used for storing templates</param>
        public TemplateCacheBase(string templateDirectory, Func<string, ITemplate> templateFactory, Func<ITemplate, IWriter> writerFactory, Func<ICache> cacheFactory) :
            base(templateDirectory, templateFactory, writerFactory)
        {
            this.cache = cacheFactory();
        }

        /// <summary>
        /// Retrieves a copy of a template from cache or caches a new copy if the template was not found in the cache
        /// </summary>
        /// <param name="fileName">File name of the requested template</param>
        /// <returns>Copy of a template</returns>
        public override ITemplate GetTemplate(string fileName)
        {
            var template = cache.GetOrAdd<ITemplate>(fileName, () => base.GetTemplate(fileName));
            return template.Copy();
        }

        /// <summary>
        /// Retrieves a copy of a template from cache or caches a new copy if the template was not found in the cache
        /// </summary>
        /// <param name="fileName">File name of the requested template</param>
        /// <returns>Copy of a template</returns>
        public override async Task<ITemplate> GetTemplateAsync(string fileName)
        {
            var template = await cache.GetOrAddAsync<ITemplate>(fileName, base.GetTemplateAsync);
            return template.Copy();
        }

        /// <summary>
        /// Checks if the requested template is currently in cache
        /// </summary>
        /// <param name="fileName">File name of the requested template</param>
        /// <returns>True or false</returns>
        public bool IsTemplateCached(string fileName) => cache.Get<ITemplate>(fileName) != null;

    }

}
