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
using TemplateEngine.Loader;
using TemplateEngine.Writer;
using TemplateEngine.Tests.Helpers;
using Xunit;

namespace TemplateEngine.Tests.LoaderTests
{

    [Collection("TemplateTests")]
    public class TemplateLoaderBaseTests
    {
        private readonly TemplateMocks mocks;

        public TemplateLoaderBaseTests(TemplateMocks mocks)
        {
            this.mocks = mocks;
        }

        [Fact]
        public void TestGetWriter()
        {
            var loader = new TemplateLoaderBase<ITemplateWriter>(mocks.TemplateDirectory, mocks.MockTemplateFactory.Object,
                mocks.MockWriterFactory.Object);

            TemplateMocks.FileNames.Iterate((f, i) =>
            {
                var writer = loader.GetWriter(f);
                var templateText = TemplateMocks.TemplateText.ElementAt(i);
                writer.GetContent().Should().Be(templateText);
            });
        }

        [Fact]
        public async Task TestGetWriterAsync()
        {
            var loader = new TemplateLoaderBase<ITemplateWriter>(mocks.TemplateDirectory, mocks.MockTemplateFactory.Object,
                mocks.MockWriterFactory.Object);

            await TemplateMocks.FileNames.IterateAsync(async (f, i) =>
            {
                var writer = await loader.GetWriterAsync(f);
                var templateText = TemplateMocks.TemplateText.ElementAt(i);
                writer.GetContent().Should().Be(templateText);
            });
        }

        [Fact]
        public void TestGetTemplate()
        {
            var loader = new TemplateLoaderBase<ITemplateWriter>(mocks.TemplateDirectory, mocks.MockTemplateFactory.Object,
                mocks.MockWriterFactory.Object);

            TemplateMocks.FileNames.Iterate((f, i) =>
            {
                var template = loader.GetTemplate(f);
                var templateText = TemplateMocks.TemplateText.ElementAt(i);
                template.ToString().Should().Be(templateText);
            });
        }

        [Fact]
        public async Task TestGetTemplateAsync()
        {
            var loader = new TemplateLoaderBase<ITemplateWriter>(mocks.TemplateDirectory, mocks.MockTemplateFactory.Object,
                mocks.MockWriterFactory.Object);

            await TemplateMocks.FileNames.IterateAsync(async (f, i) =>
            {
                var template = await loader.GetTemplateAsync(f);
                var templateText = TemplateMocks.TemplateText.ElementAt(i);
                template.ToString().Should().Be(templateText);
            });
        }

        [Fact]
        public void TestGetTemplateText()
        {
            var loader = new TemplateLoaderBase<ITemplateWriter>(mocks.TemplateDirectory, mocks.MockTemplateFactory.Object,
                mocks.MockWriterFactory.Object);

            TemplateMocks.FileNames.Iterate((f, i) =>
            {
                var templateText = loader.GetTemplateText(f);
                var expectedText = TemplateMocks.TemplateText.ElementAt(i);
                templateText.Should().Be(expectedText);
            });
        }

        [Fact]
        public async Task TestGetTemplateTextAsync()
        {
            var loader = new TemplateLoaderBase<ITemplateWriter>(mocks.TemplateDirectory, mocks.MockTemplateFactory.Object,
                mocks.MockWriterFactory.Object);

            await TemplateMocks.FileNames.IterateAsync(async (f, i) =>
            {
                var templateText = await loader.GetTemplateTextAsync(f);
                var expectedText = TemplateMocks.TemplateText.ElementAt(i);
                templateText.Should().Be(expectedText);
            });
        }

        [Fact]
        public void TestTemplateDirectory()
        {
            var loader = new TemplateLoaderBase<ITemplateWriter>(mocks.TemplateDirectory, mocks.MockTemplateFactory.Object,
                mocks.MockWriterFactory.Object);
            loader.TemplateDirectory.Should().Be(mocks.TemplateDirectory);
        }

    }

}
