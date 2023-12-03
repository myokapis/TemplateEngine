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

using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using TemplateEngine.Document;
using TemplateEngine.Loader;
using TemplateEngine.Tests.Helpers;

namespace TemplateEngine.Tests.LoaderTests
{

    [Collection("TemplateTests")]
    public class CacheBaseTests
    {
        private readonly Cache cache;
        private readonly TemplateMocks mocks;

        public CacheBaseTests(TemplateMocks mocks)
        {
            cache = new Cache();
            this.mocks = mocks;
        }

        [Fact]
        public void TestAdd()
        {
            string key = "Add";
            var template = mocks.Templates.First().Copy();
            cache.Get<ITemplate>(key).Should().Be(null);
            cache.Add(key, template);
            cache.Get<ITemplate>(key).Should().BeSameAs(template);
        }

        [Fact]
        public void TestGetOrAdd_SimpleFunc()
        {
            string key = "GetOrAdd1";
            var template = mocks.Templates.First().Copy();
            cache.Get<ITemplate>(key).Should().Be(null);
            cache.GetOrAdd(key, () => template).Should().BeSameAs(template);
            cache.GetOrAdd(key, () => template.Copy()).Should().BeSameAs(template);
        }

        [Fact]
        public async Task TestGetOrAdd_ComplexFuncAsync()
        {
            string key = "GetOrAdd2";
            var template = mocks.Templates.First().Copy();
            cache.Get<ITemplate>(key).Should().Be(null);
            var result = await cache.GetOrAddAsync(key, (s) => Task.FromResult(template));
            result.Should().BeSameAs(template);
            result = await cache.GetOrAddAsync(key, (s) => Task.FromResult(template.Copy()));
            result.Should().BeSameAs(template);
        }

        [Fact]
        public void TestRemove()
        {
            string key = "Remove";
            var template = mocks.Templates.First().Copy();
            cache.Get<ITemplate>(key).Should().Be(null);
            cache.Add(key, template);
            cache.Remove("NonExistentKey");
            cache.Get<ITemplate>(key).Should().BeSameAs(template);
            cache.Remove(key);
            cache.Get<ITemplate>(key).Should().BeNull();
        }

    }
}
