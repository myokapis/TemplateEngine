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
using System.Threading.Tasks;
using TemplateEngine.Document;
using TemplateEngine.Writer;

namespace TemplateEngine.Loader
{

    /// <summary>
    /// Interface defining methods and properties of a template cache
    /// </summary>
    public interface ITemplateCache<IWriter> : ITemplateLoader<IWriter> where IWriter : ITemplateWriter
    {

        /// <summary>
        /// Checks if the requested template is currently in cache
        /// </summary>
        /// <param name="fileName">File name of the requested template</param>
        /// <returns>True or false</returns>
        bool IsTemplateCached(string fileName);

    }

}
