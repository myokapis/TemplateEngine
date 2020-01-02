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
using System.IO;

namespace TemplateEngine
{

    /// <summary>
    /// Interface defining a template loader
    /// </summary>
    public interface ITemplateLoader
    {
        /// <summary>
        /// Reads a template file and returns the contents
        /// </summary>
        /// <param name="fileName">The name of the file to be loaded</param>
        /// <returns>String representation of the contents of the template file</returns>
        string LoadTemplate(string fileName);

        /// <summary>
        /// The default template directory
        /// </summary>
        string TemplateDirectory { get; }
    }

    /// <summary>
    /// Loads template text from the file system
    /// </summary>
    public class TemplateLoader : ITemplateLoader
    {

        /// <summary>
        /// Creates a new instance of a template loader
        /// </summary>
        /// <param name="templateDirectory">Directory to search for template files</param>
        public TemplateLoader(string templateDirectory)
        {
            if (string.IsNullOrWhiteSpace(templateDirectory)) throw new ArgumentException("Invalid template path.");

            if (!Directory.Exists(templateDirectory))
                throw new ArgumentException("Template path does not exist or is inaccessible.");

            this.TemplateDirectory = templateDirectory;
        }

        /// <summary>
        /// Locates the requested template file and returns the contents
        /// </summary>
        /// <param name="fileName">File name of the requested template</param>
        /// <returns>The contents of the template file as text</returns>
        public string LoadTemplate(string fileName)
        {
            var filePath = Path.Combine(this.TemplateDirectory, fileName);
            return File.ReadAllText(filePath);
        }

        /// <summary>
        /// Read only property containing the directory to be searched for templates
        /// </summary>
        public string TemplateDirectory { get; }

    }

}
