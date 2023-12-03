﻿/* ****************************************************************************
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
using TemplateEngine.Web;
using TemplateEngine.Writer;

namespace TemplateEngine.Loader
{

    /// <summary>
    /// An opinionated convenience class for loading templates and creating template writers
    /// </summary>
    public class TemplateLoader : TemplateLoaderBase<ITemplateWriter>
    {

        /// <summary>
        /// Sets the template directory and provides default factory methods for creating 
        /// Template and TemplateWriter objects.
        /// </summary>
        /// <param name="templateDirectory">The path in which the template document files are stored</param>
        public TemplateLoader(string templateDirectory) :
            base(templateDirectory, (text) => new Template(text), (template) => new TemplateWriter(template)) { }

    }

}
