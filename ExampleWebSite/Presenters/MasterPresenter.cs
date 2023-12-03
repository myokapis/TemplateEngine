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
using System.Threading.Tasks;
using TemplateEngine.Loader;
using TemplateEngine.Web;
using ExampleWebSite.Models;
using ExampleWebSite.Services;

namespace ExampleWebSite.Presenters
{

    /// <summary>
    /// A base presenter class containing convenience methods for common code that is used in multiple presenters
    /// </summary>
    /// <remarks>If you find yourself writing similar code in more than one presenter, then consider 
    /// refactoring that code into the master presenter.</remarks>
    public class MasterPresenter : MasterPresenterBase
    {
        protected readonly IDataService dataService;

        public MasterPresenter(ITemplateCache<IWebWriter> templateCache, IDataService dataService) : base(templateCache)
        {
            this.dataService = dataService;
        }

        /// <summary>
        /// A convenience method that can be called in inheriting presenters. The method registers a writer 
        /// to provide the html for a drop down of item types.
        /// </summary>
        /// <returns>An empty task</returns>
        protected async Task RegisterSelector()
        {
            // get common template
            var tplCommon = await templateLoader.GetWriterAsync("Common.tpl");

            // register the SELECTOR section of the common template as the provider for the PAGE_SCOPE field in the body
            contentWriter.RegisterFieldProvider("BODY", "PAGE_SCOPE", tplCommon.GetWriter("SELECTOR"));
        }

        /// <summary>
        /// A convenience method that generates the html content for a drop down of item types.
        /// </summary>
        /// <param name="writer">A reference to the writer into which the drop down html will be written</param>
        /// <param name="pageScope">A custom class that contains information about the scope of the data</param>
        protected void WriteSelector(IWebWriter writer, PageScope pageScope)
        {
            // setup the option data for the dropdown
            var options = new List<Option> { new Option("", "0") };
            options.AddRange(dataService.GetItemTypes((i) => new Option(i.ToString(), ((int)i).ToString())));

            // get the registered provider that was bound to the PAGE_SCOPE field
            writer.SelectProvider("PAGE_SCOPE");

            // get the value that should be selected in the drop down when it is displayed
            var selectedValue = pageScope.ItemType == null ? 0 : (int)pageScope.ItemType;

            // set the dropdown in the registered provider
            writer.SetOptionFields("ITEM_TYPES", options, selectedValue.ToString());

            // append the content for this provider to the output
            writer.AppendSection(true);
        }

    }

}
