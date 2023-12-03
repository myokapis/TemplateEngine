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
using System.Text;
using LazyCache;
using TemplateEngine.Document;
using TemplateEngine.Writer;

namespace TemplateEngine.Loader
{

    /// <summary>
    /// An opinionated convenience class for loading and caching templates and creating template writers
    /// </summary>
    public class TemplateCache : TemplateCacheBase<ITemplateWriter>
    {

        /// <summary>
        /// Sets the template directory, creates a cache instance, and provides default factory methods for 
        /// creating Template and TemplateWriter objects.
        /// </summary>
        /// <param name="templateDirectory">The path in which the template document files are stored</param>
        public TemplateCache(string templateDirectory) : base(templateDirectory, (text) => new Template(text), (template) => new TemplateWriter(template), new Cache()) { }

        /// <summary>
        /// Sets the template directory and cache instance, and provides default factory methods for 
        /// creating Template and TemplateWriter objects.
        /// </summary>
        /// <param name="templateDirectory">The path in which the template document files are stored</param>
        /// <param name="cache">The instance in which templates will be cached</param>
        public TemplateCache(string templateDirectory, ICache cache) : base(templateDirectory, (text) => new Template(text), (template) => new TemplateWriter(template), cache) { }

        /// <summary>
        /// Sets the template directory and cache factory, and provides default factory methods for 
        /// creating Template and TemplateWriter objects.
        /// </summary>
        /// <param name="templateDirectory">The path in which the template document files are stored</param>
        /// <param name="cacheFactory">A factory method to provide an instance in which templates will be cached</param>
        public TemplateCache(string templateDirectory, Func<ICache> cacheFactory) : base(templateDirectory, (text) => new Template(text), (template) => new TemplateWriter(template), cacheFactory) { }


    }

}
