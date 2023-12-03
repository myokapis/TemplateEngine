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

namespace ExampleWebSite.Models
{

    /// <summary>
    /// A custom class that contains information about the scope of the data on a web page.
    /// In the example web pages, the selected value in the Filter Options drop down is
    /// retained when navigating between the example pages.
    /// </summary>
    /// <remarks>While PageScope is not part of TemplateEngine, I personally find it useful
    /// for maintaining state between pages.</remarks>
    public class PageScope
    {
        public ItemType? ItemType { get; set; }
    }

}
