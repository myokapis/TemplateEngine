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
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ExampleWebSite.Models;
using ExampleWebSite.Presenters;
using ExampleWebSite.Services;

namespace ExampleWebSite.Controllers
{

    public class ComplexController : ControllerBase
    {
        private readonly ComplexPresenter presenter;

        public ComplexController(ComplexPresenter presenter, IDataService dataService) : base(dataService)
        {
            this.presenter = presenter;
        }

        /// <summary>
        /// Generates a view for the entire page
        /// </summary>
        /// <param name="pageScope">A custom class for persisting data between pages</param>
        /// <returns>A task containing html from a view</returns>
        public async Task<ActionResult> Index(PageScope pageScope = null)
        {
            return Html(await presenter.Index(GetPageScope(pageScope)));
        }

        /// <summary>
        /// Generates a partial view in response to a change in the page scope
        /// </summary>
        /// <param name="pageScope">A custom class for persisting data between pages</param>
        /// <returns>A task containing html from a partial view</returns>
        public async Task<ActionResult> ChangePageScope(PageScope pageScope)
        {
            var data = new
            {
                html = await presenter.GetItems(GetPageScope(pageScope))
            };

            return Json(data);
        }

        /// <summary>
        /// Generates a partial view for a set of selected item names
        /// </summary>
        /// <param name="itemNames">The collection of items names to include in the partial view</param>
        /// <returns>A task containing html from a partial view</returns>
        public async Task<ActionResult> Select(IEnumerable<string> itemNames)
        {
            var data = new
            {
                html = await presenter.SelectItems(itemNames)
            };

            return Json(data);
        }

    }

}
