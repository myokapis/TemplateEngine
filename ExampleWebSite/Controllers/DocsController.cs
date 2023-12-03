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

using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ExampleWebSite.Presenters;
using ExampleWebSite.Services;

namespace ExampleWebSite.Controllers
{

    public class DocsController : ControllerBase
    {
        private readonly DocsPresenter presenter;

        public DocsController(DocsPresenter presenter, IDataService dataService) : base(dataService)
        {
            this.presenter = presenter;
        }

        /// <summary>
        /// Generates a view for the entire page
        /// </summary>
        /// <returns>A task containing html from a view</returns>
        [ResponseCache(Duration = 0)]
        public async Task<ContentResult> Index()
        {
            return Html(await presenter.Index());
        }

    }

}
