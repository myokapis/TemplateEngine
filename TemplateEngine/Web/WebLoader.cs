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
using TemplateEngine.Document;
using TemplateEngine.Loader;

namespace TemplateEngine.Web
{
    /// <summary>
    /// An opinionated convenience class for creating templates and web template writers
    /// </summary>
    public class WebLoader : TemplateLoaderBase<IWebWriter>
    {

        /// <summary>
        /// Sets the template directory and provides default factory methods for Template and WebWriter
        /// </summary>
        /// <param name="templateDirectory">The path in which the template document files are stored</param>
        public WebLoader(string templateDirectory) :
            base(templateDirectory, (text) => new Template(text), (template) => new WebWriter(template)) { }
    }

}
