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
using System.Threading.Tasks;
using TemplateEngine.Document;
using TemplateEngine.Loader;
using TemplateEngine.Writer;

namespace TemplateEngine.Web
{

    // TODO: Have master presenter register field providers with matching sections at root.
    //       Also render in auto methods.

    /// <summary>
    /// A base presenter with helper methods to set up a master page and bind
    /// it to source(s) for content.
    /// </summary>
    /// <remarks>
    /// The master presenter makes default assumptions about the structure of the
    /// master page. It assumes head, body, and tail sections that can be filled
    /// with content from a content writer. Developers can customize this approach
    /// and still use the master presenter.
    /// </remarks>
    public class MasterPresenterBase
    {
        /// <summary>
        /// The writer to provide content for the master template markup fields
        /// </summary>
        protected IWebWriter? contentWriter;

        /// <summary>
        /// The writer holding the master template
        /// </summary>
        protected IWebWriter? masterWriter;

        /// <summary>
        /// A template loader for loading the master template, content template, and
        /// other templates as needed
        /// </summary>
        protected readonly ITemplateLoader<IWebWriter> templateLoader;

        /// <summary>
        /// Constructor for the master presenter
        /// </summary>
        /// <param name="templateLoader">
        /// A template loader for loading the master template, content template, and
        /// other templates as needed
        /// </param>
        public MasterPresenterBase(ITemplateLoader<IWebWriter> templateLoader)
        {
            this.templateLoader = templateLoader;
        }

        /// <summary>
        /// A helper method for appending all unappended sections and rendering the final content
        /// </summary>
        /// <returns>The generated content from the master writer</returns>
        protected string GetContent()
        {
            if (masterWriter == null)
                throw new ApplicationException(messages["NoMasterWriter"]);

            masterWriter.AppendAll();
            return masterWriter.GetContent();
        }

        /// <summary>
        /// A helper method to load the content template and create a writer for it
        /// </summary>
        /// <param name="contentFileName"></param>
        /// <returns>The content writer</returns>
        protected async Task<IWebWriter> SetupContentWriter(string contentFileName)
        {
            contentWriter = await templateLoader.GetWriterAsync(contentFileName);
            return contentWriter;
        }

        /// <summary>
        /// Automatically sets up the master template sections by convention using the content template
        /// </summary>
        /// <returns>The master writer</returns>
        protected IWebWriter SetupMasterPage()
        {
            if (contentWriter == null)
                throw new ApplicationException(messages["NoContentWriter"]);

            var head = contentWriter.ContainsSection(Head) ? Head : null;
            var body = contentWriter.ContainsSection(Body) ? Body : null;
            var tail = contentWriter.ContainsSection(Tail) ? Tail : null;
            return SetupMasterPage(head, body, tail);
        }

        /// <summary>
        /// Sets up the master template sections using the content template
        /// </summary>
        /// <param name="headSection">
        /// The name of the section in the content template that should provide content for the head 
        /// section of the master template
        /// </param>
        /// <param name="bodySection">
        /// The name of the section in the content template that should provide content for the body 
        /// section of the master template
        /// </param>
        /// <param name="tailSection">
        /// The name of the section in the content template that should provide content for the tail 
        /// section of the master template
        /// </param>
        /// <returns>The master writer</returns>
        protected IWebWriter SetupMasterPage(string? headSection = null, string? bodySection = null, string? tailSection = null)
        {
            if (contentWriter == null)
                throw new ApplicationException(messages["NoContentWriter"]);

            var head = headSection != null ? contentWriter.GetWriter(headSection) as IWebWriter : null;
            var body = bodySection != null ? (IWebWriter)contentWriter.GetWriter(bodySection) : null;
            var tail = tailSection != null ? contentWriter.GetWriter(tailSection) as IWebWriter : null;
            return SetupMasterPage(head, body, tail);
        }

        /// <summary>
        /// Sets up the master template sections using the provided writers
        /// </summary>
        /// <param name="head">The writer to provide content for the head section of the master template</param>
        /// <param name="body">The writer to provide content for the body section of the master template</param>
        /// <param name="tail">The writer to provide content for the tail section of the master template</param>
        /// <returns>The master writer</returns>
        protected IWebWriter SetupMasterPage(IWebWriter? head = null, IWebWriter? body = null, IWebWriter? tail = null)
        {
            if (masterWriter == null)
                throw new ApplicationException(messages["NoMasterWriter"]);

            masterWriter.Reset();

            // reset all providers
            head?.Reset();
            body?.Reset();
            tail?.Reset();

            // setup the master page section providers
            if (head != null) masterWriter.RegisterFieldProvider(Head, head);
            if (body != null) masterWriter.RegisterFieldProvider(Body, body);
            if (tail != null) masterWriter.RegisterFieldProvider(Tail, tail);

            return masterWriter;
        }

        /// <summary>
        /// A helper method to load the master and content templates and setup writers for them
        /// </summary>
        /// <param name="masterFileName">The name of the master template file</param>
        /// <param name="contentFileName">The name of the content template file</param>
        /// <param name="autoSetup">A flag to enable or disable setting up the head, body, and 
        /// tail sections of the master template using the content template</param>
        /// <returns>The master writer</returns>
        protected async Task<IWebWriter> SetupWriters(string masterFileName, string contentFileName, bool autoSetup = true)
        {
            masterWriter = await templateLoader.GetWriterAsync(masterFileName);
            contentWriter = await templateLoader.GetWriterAsync(contentFileName);

            return autoSetup ? SetupMasterPage() : masterWriter;
        }

        /// <summary>
        /// Adds content to the master writer for a previously registered field provider
        /// and allows customization of the content through a page builder method
        /// </summary>
        /// <param name="providerName">The name of the field provider that will supply the content</param>
        /// <param name="pageBuilder">An async function to provide customized content</param>
        /// <returns>The master writer</returns>
        protected async Task WriteMasterSectionAsync(string providerName, Func<IWebWriter, Task> pageBuilder)
        {
            if (masterWriter == null)
                throw new ApplicationException(messages["NoMasterWriter"]);

            masterWriter.SelectProvider(providerName);
            await pageBuilder.Invoke(masterWriter);
            masterWriter.AppendSection(true);
        }

        /// <summary>
        /// Adds content to the master writer for a previously registered field provider
        /// </summary>
        /// <param name="providerName">The name of the field provider that will supply the content</param>
        protected void WriteMasterSection(string providerName)
        {
            if (masterWriter == null)
                throw new ApplicationException(messages["NoMasterWriter"]);

            masterWriter.SelectProvider(providerName);
            masterWriter.AppendSection(true);
        }

        /// <summary>
        /// Adds content to the master writer for a previously registered field provider
        /// and allows customization of the content through a page builder method
        /// </summary>
        /// <param name="providerName">The name of the field provider that will supply the content</param>
        /// <param name="pageBuilder">A function to provide customized content</param>
        /// <returns>The master writer</returns>
        protected void WriteMasterSection(string providerName, Action<IWebWriter> pageBuilder)
        {
            if (masterWriter == null)
                throw new ApplicationException(messages["NoMasterWriter"]);

            masterWriter.SelectProvider(providerName);
            pageBuilder.Invoke(masterWriter);
            masterWriter.AppendSection(true);
        }

        /// <summary>
        /// Adds content to the master writer by convention for all of the registered field providers
        /// </summary>
        protected void WriteMasterSections()
        {
            if (contentWriter == null)
                throw new ApplicationException(messages["NoContentWriter"]);

            Sections.ForEach(section =>
            {
                if (contentWriter.ContainsSection(section))
                    WriteMasterSection(section);
            });
        }

        /// <summary>
        /// DRY set of messages
        /// </summary>
        protected static Dictionary<string, string> messages = new()
        {
            { "NoMasterWriter", "No master writer has been loaded for this presenter." },
            { "NoContentWriter", "No content writer has been loaded for this presenter." }
        };

        /// <summary>
        /// A customizable helper property that provides a default name for the body section
        /// </summary>
        public static string Body { get; } = nameof(Body).ToUpper();

        /// <summary>
        /// A customizable helper property that provides a default name for the head section
        /// </summary>
        public static string Head { get; } = nameof(Head).ToUpper();

        /// <summary>
        /// A customizable helper property that provides a default name for the tail section
        /// </summary>
        public static string Tail { get; } = nameof(Tail).ToUpper();

        /// <summary>
        /// A customizable helper property that provides a default set of names for the default
        /// master template sections
        /// </summary>
        public List<string> Sections { get; set; } = new List<string>{ Head, Body, Tail};

    }

}
