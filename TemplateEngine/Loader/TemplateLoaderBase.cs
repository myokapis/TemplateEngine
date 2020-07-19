/* ****************************************************************************
Copyright 2018-2022 Gene Graves

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
using System.IO;
using System.Threading.Tasks;
using TemplateEngine.Document;
using TemplateEngine.Writer;

namespace TemplateEngine.Loader
{

    /// <summary>
    /// This is a foundational class for concrete implementations; however, it can be used on its own.
    /// Loads template text from the file system. Also provides methods for creating a template
    /// writer from the template text.
    /// </summary>
    public class TemplateLoader<IWriter> : ITemplateLoader<IWriter> where IWriter : class, ITemplateWriter
    {
        /// <summary>
        /// A factory method for providing an ITemplate instance
        /// </summary>
        protected readonly Func<string, ITemplate> templateFactory;

        /// <summary>
        /// A factory method for providing an IWriter instance
        /// </summary>
        protected readonly Func<ITemplate, IWriter> writerFactory;

        /// <summary>
        /// Creates a new instance of a template loader.
        /// </summary>
        /// <param name="templateDirectory">Directory to search for template files</param>
        /// <param name="templateFactory">Delegate for instantiating a template from the file name</param>
        /// <param name="writerFactory">A method for creating an IWriter instance</param>
        public TemplateLoader(string templateDirectory, Func<string, ITemplate> templateFactory, Func<ITemplate, IWriter> writerFactory)
        {
            if (string.IsNullOrWhiteSpace(templateDirectory)) throw new ArgumentException("Invalid template path.");

            if (!Directory.Exists(templateDirectory))
                throw new ArgumentException("Template path does not exist or is inaccessible.");

            TemplateDirectory = templateDirectory;
            this.templateFactory = templateFactory;
            this.writerFactory = writerFactory;
        }

        /// <summary>
        /// Retrieves a copy of a template from cache or caches a new copy if the template was not found in the cache
        /// </summary>
        /// <param name="fileName">File name of the requested template</param>
        /// <returns>Copy of a template</returns>
        public virtual ITemplate GetTemplate(string fileName)
        {
            var templateText = GetTemplateText(fileName);
            return templateFactory(templateText);
        }

        /// <summary>
        /// Retrieves a copy of a template from cache or caches a new copy if the template was not found in the cache
        /// </summary>
        /// <param name="fileName">File name of the requested template</param>
        /// <returns>Copy of a template</returns>
        public virtual async Task<ITemplate> GetTemplateAsync(string fileName)
        {
            var templateText = await GetTemplateTextAsync(fileName);
            return templateFactory(templateText);
        }

        /// <summary>
        /// Creates a new TemplateWriter based on the requested template file
        /// </summary>
        /// <param name="fileName">The name of the file to be loaded</param>
        /// <returns>TemplateWriter</returns>
        public virtual IWriter GetWriter(string fileName)
        {
            var template = GetTemplate(fileName);
            return writerFactory(template);
        }

        /// <summary>
        /// Creates a new TemplateWriter based on the requested template file
        /// </summary>
        /// <param name="fileName">The name of the file to be loaded</param>
        /// <param name="sectionName">A child section for which a writer should be created</param>
        /// <returns>TemplateWriter</returns>
        public virtual IWriter GetWriter(string fileName, string sectionName)
        {
            var template = GetTemplate(fileName);
            return writerFactory(template).GetWriter(sectionName) as IWriter;
        }

        /// <summary>
        /// Creates a new TemplateWriter asynchronously based on the requested template file
        /// </summary>
        /// <param name="fileName">The name of the file to be loaded</param>
        /// <returns>TemplateWriter</returns>
        public virtual async Task<IWriter> GetWriterAsync(string fileName)
        {
            var template = await GetTemplateAsync(fileName);
            return writerFactory(template);
        }

        /// <summary>
        /// Creates a new TemplateWriter asynchronously based on the requested template file
        /// </summary>
        /// <param name="fileName">The name of the file to be loaded</param>
        /// <param name="sectionName">A child section for which a writer should be created</param>
        /// <returns>TemplateWriter</returns>
        public virtual async Task<IWriter> GetWriterAsync(string fileName, string sectionName)
        {
            var template = await GetTemplateAsync(fileName);
            return writerFactory(template).GetWriter(sectionName) as IWriter;
        }

        /// <summary>
        /// Locates the requested template file and returns the contents
        /// </summary>
        /// <param name="fileName">File name of the requested template</param>
        /// <returns>The contents of the template file as text</returns>
        public string GetTemplateText(string fileName)
        {
            var filePath = Path.Combine(TemplateDirectory, fileName);
            return File.ReadAllText(filePath);
        }

        /// <summary>
        /// Loads the requested template file asynchronously and returns the contents
        /// </summary>
        /// <param name="fileName">File name of the requested template</param>
        /// <returns>The contents of the template file as text</returns>
        public async Task<string> GetTemplateTextAsync(string fileName)
        {
            var filePath = Path.Combine(TemplateDirectory, fileName);
            return await File.ReadAllTextAsync(filePath);
        }

        /// <summary>
        /// Read only property containing the directory to be searched for templates
        /// </summary>
        public string TemplateDirectory { get; }

    }

}
