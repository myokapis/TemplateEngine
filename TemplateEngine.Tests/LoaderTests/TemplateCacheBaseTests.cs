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

using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using TemplateEngine.Loader;
using TemplateEngine.Tests.Helpers;
using TemplateEngine.Writer;
using Xunit;

namespace TemplateEngine.Tests.LoaderTests
{

    [Collection("TemplateTests")]
    public class TemplateCacheBaseTests
    {

        private readonly TemplateMocks mocks;

        public TemplateCacheBaseTests(TemplateMocks mocks)
        {
            this.mocks = mocks;
        }

        [Fact]
        public void TestGetTemplate()
        {
            // create a template cache using the mock cache and mock loader
            var cache = new TemplateCache<ITemplateWriter>(mocks.TemplateDirectory, mocks.MockTemplateFactory.Object,
                mocks.MockWriterFactory.Object, mocks.MockCache);

            mocks.Templates.Iterate((t, i) =>
            {
                var fileName = mocks.FileNames.ElementAt(i);
                var template = cache.GetTemplate(fileName);

                template.ToString().Should().Be(mocks.TemplateText.ElementAt(i));
                template.TemplateId.Should().NotBe(mocks.Templates.ElementAt(i).TemplateId);
            });
        }

        [Fact]
        public async Task TestGetTemplateAsync()
        {
            // create a template cache using the mock cache and mock loader
            var cache = new TemplateCache<ITemplateWriter>(mocks.TemplateDirectory, mocks.MockTemplateFactory.Object,
                mocks.MockWriterFactory.Object, mocks.MockCache);

            await mocks.Templates.IterateAsync(async (t, i) =>
            {
                var fileName = mocks.FileNames.ElementAt(i);
                var template = await cache.GetTemplateAsync(fileName);

                template.ToString().Should().Be(mocks.TemplateText.ElementAt(i));
                template.TemplateId.Should().NotBe(mocks.Templates.ElementAt(i).TemplateId);
            });
        }

        [Fact]
        public void TestIsTemplateCached()
        {
            // create a template cache using the mock cache and mock loader
            var mockAppCache = mocks.MockCache;
            var cache = new TemplateCache<ITemplateWriter>(mocks.TemplateDirectory, mocks.MockTemplateFactory.Object,
                mocks.MockWriterFactory.Object, mockAppCache);

            mocks.FileNames.Iterate((f, i) =>
            {
                var template = mocks.Templates.ElementAt(i);
                cache.IsTemplateCached(f).Should().BeFalse();
                mockAppCache.Add(f, template);
                cache.IsTemplateCached(f).Should().BeTrue();
            });
        }

    }

}
