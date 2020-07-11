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

using System;
using System.Threading.Tasks;
using LazyCache;
using Microsoft.Extensions.Caching.Memory;

namespace TemplateEngine
{

    /// <summary>
    /// Interface defining methods and properties of a template cache
    /// </summary>
    public interface ITemplateCache
    {

        /// <summary>
        /// Retrieves a copy of a template from cache or caches a new copy if the template was not found in the cache
        /// </summary>
        /// <param name="fileName">File name of the requested template</param>
        /// <returns>Copy of a template</returns>
        ITemplate GetTemplate(string fileName);

        /// <summary>
        /// Retrieves a copy of a template from cache or caches a new copy if the template was not found in the cache
        /// </summary>
        /// <param name="fileName">File name of the requested template</param>
        /// <returns>Copy of a template</returns>
        Task<ITemplate> GetTemplateAsync(string fileName);

        /// <summary>
        /// Checks if the requested template is currently in cache
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
        /// Reference to an object for loading templates
        /// </summary>
        protected ITemplateLoader templateLoader;
        protected IAppCache cache;

        /// <summary>
        /// Constructs a new cache object
        /// </summary>
        /// <param name="templateLoader">An <see cref="ITemplateLoader" /> to provide template files for parsing</param>
        /// <param name="expiresAfterSeconds">Number of seconds a template is considered fresh within the cache</param>
        public TemplateCache(IAppCache cache, ITemplateLoader templateLoader) : base()
        {
            this.cache = cache;
            this.templateLoader = templateLoader;
        }

        #region public methods

        /// <summary>
        /// Retrieves a copy of a template from cache or caches a new copy if the template was not found in the cache
        /// </summary>
        /// <param name="fileName">File name of the requested template</param>
        /// <returns>Copy of a template</returns>
        public ITemplate GetTemplate(string fileName)
        {
            Func<ITemplate> createTemplate = () => CreateTemplate(fileName);
            return cache.GetOrAdd(fileName, createTemplate).Copy();
        }

        /// <summary>
        /// Retrieves a copy of a template from cache or caches a new copy if the template was not found in the cache
        /// </summary>
        /// <param name="fileName">File name of the requested template</param>
        /// <returns>Copy of a template</returns>
        public async Task<ITemplate> GetTemplateAsync(string fileName)
        {
            Func<Task<ITemplate>> createTemplate = () => CreateTemplateAsync(fileName);
            var task = await cache.GetOrAddAsync(fileName, createTemplate);
            return task.Copy();
        }

        /// <summary>
        /// Checks if the requested template is currently in cache
        /// </summary>
        /// <param name="fileName">File name of the requested template</param>
        /// <returns>True or false</returns>
        public bool IsTemplateCached(string fileName) => cache.Get<ITemplate>(fileName) != null;

        #endregion

        #region protected methods

        protected ITemplate CreateTemplate(string fileName)
        {
            var templateText = templateLoader.LoadTemplateText(fileName);
            return new Template(templateText);
        }

        protected async Task<ITemplate> CreateTemplateAsync(string fileName)
        {
            var templateText = await templateLoader.LoadTemplateTextAsync(fileName);
            return new Template(templateText);
        }

        #endregion

    }

}
