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

using System;
using System.Collections.Generic;
using System.Text;
using LazyCache;
using TemplateEngine.Document;

namespace TemplateEngine.Tests.Helpers
{

    public class TestAppCache : CachingService
    {
        protected Dictionary<string, ITemplate> dic = new Dictionary<string, ITemplate>();

        public virtual void Add<T>(string key, T item)
        {
            dic.TryAdd(key, (ITemplate)item);
        }

        public override T Get<T>(string key)
        {
            dic.TryGetValue(key, out var template);
            return (T)template;
        }

        public virtual T GetOrAdd<T>(string key, Func<string, T> factory)
        {
            if (!dic.TryGetValue(key, out var template))
            {
                template = (ITemplate)factory.Invoke(key);
                dic.Add(key, template);
            }

            return (T)template;
        }
    }

}
