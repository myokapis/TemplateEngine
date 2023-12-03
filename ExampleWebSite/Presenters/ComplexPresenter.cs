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

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TemplateEngine.Loader;
using TemplateEngine.Web;
using TemplateEngine.Writer;
using ExampleWebSite.Models;
using ExampleWebSite.Services;

namespace ExampleWebSite.Presenters
{

    public class ComplexPresenter : MasterPresenter
    {

        public ComplexPresenter(ITemplateCache<IWebWriter> templateCache, IDataService dataService) : base(templateCache, dataService)
        {
        }

        /// <summary>
        /// The main method for rendering the page
        /// </summary>
        /// <param name="pageScope">A custom class for persisting data between pages</param>
        /// <returns>A task containing the html for the entire page</returns>
        public async Task<string> Index(PageScope pageScope)
        {
            // set up a writer by specifying the master page and the content page templates
            await SetupWriters("Master.tpl", "Complex.tpl");

            // use a method in the master presenter to register a provider to add a drop down for selecting the item type
            await RegisterSelector();

            // use a method in the master presenter to write the html from the HEAD section of the content page into the <head> of the output
            WriteMasterSection(Head);

            // use a method in the master presenter to write the generated html from the BODY section of the content page into the body of the output
            WriteMasterSection(Body, (writer) =>
            {
                // use the common method to write the dropdown
                WriteSelector(writer, pageScope);

                // select the section of the template where the html for the items will be written
                writer.SelectSection("ITEM_BODY");

                // pass the writer and data to a common method that generates the html for each item
                SetItems(writer, GetData(pageScope));
            });

            // collect and render the final content
            return GetContent();
        }

        /// <summary>
        /// A method to generate a partial view of a set of items
        /// </summary>
        /// <param name="pageScope">A custom class for persisting data between pages</param>
        /// <returns>A task containing the html for the partial view</returns>
        public async Task<string> GetItems(PageScope pageScope)
        {
            // set up a writer for a specific section of the content page template
            var writer = await templateLoader.GetWriterAsync("Complex.tpl", "ITEM_BODY");

            // pass the writer and data to a common method that generates the html for each item
            SetItems(writer, GetData(pageScope));

            // collect and render the final content while automatically appending each section
            return writer.GetContent(true);
        }

        /// <summary>
        /// Populates a partial view using a collection of selected items
        /// </summary>
        /// <param name="itemNames">The names of the selected items</param>
        /// <returns>A task containing the partial view html</returns>
        public async Task<string> SelectItems(IEnumerable<string> itemNames)
        {
            // get a collection of items that match the items names passed in from the calling web page
            var items = dataService.GetItems((i) => itemNames.Any(n => n == i.Name));

            // set up a writer for a specific section of the content page template
            var writer = await templateLoader.GetWriterAsync("Complex.tpl", "POPUP");

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
        /// <param name="pageScope">A custom class for persisting data between pages</param>
        /// <returns>A collection of Items</returns>
        protected IEnumerable<Item> GetData(PageScope pageScope)
        {
            return dataService.GetItems((i) => (pageScope.ItemType == null) || (i.Type == pageScope.ItemType));
        }

        /// <summary>
        /// A helper method to generating the html content for a collection of items
        /// </summary>
        /// <param name="writer">The writer to receive the html content</param>
        /// <param name="items">The collection of Items for which data is to be rendered</param>
        protected void SetItems(IWebWriter writer, IEnumerable<Item> items)
        {
            foreach (var item in items)
            {
                // select a section to be a container
                writer.SelectSection("ITEMS");

                // determine which subsection to append based on the item type
                var sectionName = item.Type == ItemType.Animal ? "ITEM_ANIMAL" :
                    item.Type == ItemType.Mineral ? "ITEM_MINERAL" : "ITEM_VEGGIE";

                // fill in the template section with data once per data item and append the section to the container
                writer.SetSectionFields(sectionName, item, SectionOptions.AppendDeselect, new FieldDefinitions("Selected"));

                // append the container section
                writer.AppendSection(true);
            }
        }

    }

}
