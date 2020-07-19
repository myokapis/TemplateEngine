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

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TemplateEngine.Loader;
using TemplateEngine.Web;
using ExampleWebSite.Models;
using ExampleWebSite.Services;

namespace ExampleWebSite.Presenters
{

    public class SimplePresenter : MasterPresenter
    {
        public SimplePresenter(ITemplateCache<IWebWriter> templateCache, IDataService dataService) : base(templateCache, dataService)
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
            await SetupWriters("Master.tpl", "Simple.tpl");

            // use a method in the master presenter to register a provider to add a drop down for selecting the item type
            await RegisterSelector();

            // use a method in the master presenter to write the html from the HEAD section of the content page into the <head> of the output
            WriteMasterSection(Head);

            WriteMasterSection(Body, (writer) =>
            {
                // use the common method to write the dropdown
                WriteSelector(writer, pageScope);

                // fill in the template section with data once per data item
                writer.SetMultiSectionFields("ITEMS", GetData(pageScope), new FieldDefinitions("Selected"));
            });

            // collect and render the final content
            return GetContent();
        }

        /// <summary>
        /// Returns a partial view containing items that match the filter in the page scope
        /// </summary>
        /// <param name="pageScope">A custom class for persisting data between pages</param>
        /// <returns></returns>
        public async Task<string> GetItems(PageScope pageScope)
        {
            // set up a writer for a specific section of the content page template
            var writer = await templateLoader.GetWriterAsync("Simple.tpl", "ITEMS");

            // fill in the template section with data once per data item
            writer.SetMultiSectionFields(GetData(pageScope), new FieldDefinitions("Selected"));

            // collect and render the final content
            return writer.GetContent();
        }

        /// <summary>
        /// Retuns a partial view containing items that match the selected items
        /// </summary>
        /// <param name="itemNames">A collection of item names on which to filter</param>
        /// <returns></returns>
        public async Task<string> SelectItems(IEnumerable<string> itemNames)
        {
            // get a collection of items that match the items names passed in from the calling web page
            var items = dataService.GetItems((i) => itemNames.Any(n => n == i.Name));

            // set up a writer for a specific section of the content page template
            var writer = await templateLoader.GetWriterAsync("Simple.tpl", "POPUP");

            // generate html for the items unless there are no selected items in which case append static html
            if (items.Count() > 0)
                // generate html for the items using the POPUP_ITEMS section of the content template
                writer.SetMultiSectionFields("POPUP_ITEMS", items);
            else
                // select a static section of the content template
                // NOTE: it's not necessary to call AppendSection() as long as the GetContent() call below is passed AppendAll = true
                writer.SelectSection("EMPTY_POPUP");

            // collect and render the final content while automatically appending each section
            return writer.GetContent(true);
        }

        /// <summary>
        /// A helper method to get a collection of data based on the values in the page scope
        /// </summary>
        /// <param name="pageScope">A custom class that contains information about the scope of the data</param>
        /// <returns>A collection of Items</returns>
        protected IEnumerable<Item> GetData(PageScope pageScope)
        {
            return dataService.GetItems((i) => (pageScope.ItemType == null) || (i.Type == pageScope.ItemType));
        }

    }

}
