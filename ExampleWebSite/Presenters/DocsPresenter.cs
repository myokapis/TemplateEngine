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

using System.Linq;
using System.Threading.Tasks;
using TemplateEngine.Loader;
using TemplateEngine.Web;
using ExampleWebSite.Services;

namespace ExampleWebSite.Presenters
{

    public class DocsPresenter : MasterPresenter
    {

        public DocsPresenter(ITemplateCache<IWebWriter> templateCache, IDataService dataService) : base(templateCache, dataService) { }

        public async Task<string> Index()
        {
            await SetupWriters("Master.tpl", "Docs.tpl");
            SetupMasterPage();
            AutoRegisterProviders();
            WriteMasterSection(Head);

            WriteMasterSection(Body, (writer) =>
            {
                AutoRenderProviders(writer);
            });

            return masterWriter.GetContent(true);
        }

        /// <summary>
        /// Finds all sections in the main section of the content template where the section name
        /// matches a markup field in the body section of the content template. Any section with
        /// a corresponding markup field is registered as a field provider.
        /// </summary>
        private void AutoRegisterProviders()
        {
            // get all markup fields by convention
            var markupFields = contentWriter.Template.GetTemplate(Body)
                .FieldNames
                .Where(n => n == n.ToUpper());

            foreach (var fieldName in markupFields)
            {
                // if there is a corresponding section for the field then register a provider for that section
                if(contentWriter.ContainsSection(fieldName))
                    contentWriter.RegisterFieldProvider(Body, fieldName, contentWriter.GetWriter(fieldName));
            }
        }

        /// <summary>
        /// Finds all sections in the main section of the content template where the section name
        /// matches a markup field in the body section of the content template. Any section with
        /// a corresponding markup field is selected and appended.
        /// </summary>
        /// <param name="writer"></param>
        private void AutoRenderProviders(IWebWriter writer)
        {
            // get all markup fields by convention
            var markupFields = contentWriter.Template.GetTemplate(Body)
                .FieldNames
                .Where(n => n == n.ToUpper());

            foreach (var fieldName in markupFields)
            {
                writer.SelectProvider(fieldName);
                writer.AppendSection(true);
            }
        }

    }

}
