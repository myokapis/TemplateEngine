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
using TemplateEngine.Document;
using TemplateEngine.Loader;

namespace TemplateEngine.Tests.Helpers
{

    public class TestAppCache : ICache
    {
        protected Dictionary<string, ITemplate> dic = new();

        public virtual void Add<T>(string key, T item) where T : ITemplate
        {
            dic.TryAdd(key, item);
        }

        public T? Get<T>(string key) where T : ITemplate
        {
            dic.TryGetValue(key, out var template);

            return template == null ? default : (T)template;
        }

        public T GetOrAdd<T>(string key, Func<string, T> factory) where T : ITemplate
        {
            if (!dic.TryGetValue(key, out var template))
            {
                template = factory.Invoke(key);
                dic.Add(key, template);
            }

            return (T)template;
        }

        public T GetOrAdd<T>(string key, Func<T> factory) where T : ITemplate
        {
            if (!dic.TryGetValue(key, out var template))
            {
                template = factory.Invoke();
                dic.Add(key, template);
            }

            return (T)template;
        }

        public async Task<T> GetOrAddAsync<T>(string key, Func<string, Task<T>> factory) where T : ITemplate
        {
            if (!dic.TryGetValue(key, out var template))
            {
                template = await factory.Invoke(key);
                dic.Add(key, template);
            }

            return (T)template;
        }

        public void Remove(string key)
        {
            throw new NotImplementedException();
        }
    }

}
