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

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TemplateEngine
{

    public static class Extensions
    {

        public static string Concat(this IEnumerable<string> collection, string separator = "")
        {
            return string.Join(separator, collection);
        }

        public static void Iterate<T>(this IEnumerable<T> items, Action<T, int> action)
        {
            var i = 0;

            foreach (var item in items)
            {
                action(item, i);
                i++;
            }
        }

        public static async Task IterateAsync<T>(this IEnumerable<T> items, Func<T, int, Task> action)
        {
            var i = 0;

            foreach (var item in items)
            {
                await action(item, i);
                i++;
            }
        }

    }

}
