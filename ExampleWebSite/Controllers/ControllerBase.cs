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

using Microsoft.AspNetCore.Mvc;
using ExampleWebSite.Models;
using ExampleWebSite.Services;

namespace ExampleWebSite.Controllers
{

    /// <summary>
    /// A base controller containing helper methods
    /// </summary>
    public class ControllerBase : Controller
    {
        protected IDataService dataService;

        public ControllerBase(IDataService dataService)
        {
            this.dataService = dataService;
        }

        /// <summary>
        /// A helper method for returning view content as html
        /// </summary>
        /// <param name="content">The html view content as a string</param>
        /// <returns>An content result with a content type header set to "text/html"</returns>
        protected ContentResult Html(string content)
        {
            return Content(content, "text/html");
        }

        /// <summary>
        /// A helper method to ensure that a valid page scope is available
        /// </summary>
        /// <param name="pageScope">A custom class for persisting data between pages</param>
        /// <returns>The page scope from the calling method if it is valid otherwise a default page scope</returns>
        protected PageScope GetPageScope(PageScope pageScope)
        {
            return ModelState.IsValid ? pageScope : dataService.GetPageScope((i) => new PageScope { ItemType = i});
        }

    }

}
