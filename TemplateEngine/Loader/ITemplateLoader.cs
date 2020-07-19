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

using System.Threading.Tasks;
using TemplateEngine.Document;
using TemplateEngine.Writer;

namespace TemplateEngine.Loader
{

    /// <summary>
    /// Interface defining a template loader
    /// </summary>
    public interface ITemplateLoader<IWriter> where IWriter : ITemplateWriter
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
        /// Reads a template file and returns the contents
        /// </summary>
        /// <param name="fileName">The name of the file to be loaded</param>
        /// <returns>String representation of the contents of the template file</returns>
        string GetTemplateText(string fileName);

        /// <summary>
        /// Reads a template file asynchronously and returns the contents
        /// </summary>
        /// <param name="fileName">The name of the file to be loaded</param>
        /// <returns>String representation of the contents of the template file</returns>
        Task<string> GetTemplateTextAsync(string fileName);

        /// <summary>
        /// Creates a new cref="TemplateWriter" based on the requested template file
        /// </summary>
        /// <param name="fileName">The name of the file to be loaded</param>
        /// <returns>cref="TemplateWriter"</returns>
        IWriter GetWriter(string fileName);

        /// <summary>
        /// Creates a new cref="TemplateWriter" based on the requested template file
        /// </summary>
        /// <param name="fileName">The name of the file to be loaded</param>
        /// <param name="sectionName">A child section for which a writer should be created</param>
        /// <returns>cref="TemplateWriter"</returns>
        IWriter GetWriter(string fileName, string sectionName);

        /// <summary>
        /// Creates a new cref="TemplateWriter" asynchronously based on the requested template file
        /// </summary>
        /// <param name="fileName">The name of the file to be loaded</param>
        /// <returns>cref="TemplateWriter"</returns>
        Task<IWriter> GetWriterAsync(string fileName);

        /// <summary>
        /// Creates a new cref="TemplateWriter" asynchronously based on the requested template file
        /// </summary>
        /// <param name="fileName">The name of the file to be loaded</param>
        /// <param name="sectionName">A child section for which a writer should be created</param>
        /// <returns>cref="TemplateWriter"</returns>
        Task<IWriter> GetWriterAsync(string fileName, string sectionName);

        /// <summary>
        /// The default template directory
        /// </summary>
        string TemplateDirectory { get; }
    }

}
