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
using TemplateEngine.Loader;
using TemplateEngine.Web;
using ExampleWebSite.Models;
using ExampleWebSite.Services;

namespace ExampleWebSite.Presenters
{

    public class HomePresenter : MasterPresenter
    {

        public HomePresenter(ITemplateCache<IWebWriter> templateCache, IDataService dataService) : base(templateCache, dataService)
        {
        }

        /// <summary>
        /// The main method for rendering the page
        /// </summary>
        /// <param name="pageScope">A custom class for persisting data between pages</param>
        /// <returns>The HTML content for the entire page</returns>
        public async Task<string> Index(PageScope pageScope)
        {
            // set up a writer by specifying the master page and the content page templates
            await SetupWriters("Master.tpl", "Home.tpl");

            // use a method in the master presenter to register a provider to add a drop down for selecting the item type
            await RegisterSelector();

            // use a method in the master presenter to write the html from the HEAD section of the content page into the <head> of the output
            WriteMasterSection(Head);

            // use a method in the master presenter to write the generated html from the BODY section of the content page into the body of the output
            WriteMasterSection(Body, (writer) =>
            {
                // use the common method to write the dropdown
                WriteSelector(writer, pageScope);
            });

            // collect and render the final content
            return GetContent();
        }

    }

}
