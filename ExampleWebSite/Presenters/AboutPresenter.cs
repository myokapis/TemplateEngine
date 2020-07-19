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
using ExampleWebSite.Services;

namespace ExampleWebSite.Presenters
{

    public class AboutPresenter : MasterPresenter
    {

        public AboutPresenter(ITemplateCache<IWebWriter> templateCache, IDataService dataService) : base(templateCache, dataService)
        {
        }

        /// <summary>
        /// The main method for rendering the page
        /// </summary>
        /// <param name="pageScope">A custom class for persisting data between pages</param>
        /// <returns>The HTML content for the entire page</returns>
        public async Task<string> Index()
        {
            // set up a writer by specifying the master page and the content page templates
            await SetupWriters("Master.tpl", "About.tpl");

            // use a method in the master presenter to write the html for the head, body, and tail using the
            //    corresponding sections in the content template.
            // this helper method is a quick way to render a static view
            WriteMasterSections();

            // collect and render the final content
            return GetContent();
        }

    }

}
