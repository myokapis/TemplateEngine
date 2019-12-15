/* ****************************************************************************
Copyright 2018-2020 Gene Graves

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
using System.Threading;
using FluentAssertions;
using Xunit;
using Moq;

namespace TemplateEngine.Tests
{

    public class TemplateCacheTests
    {

        [Fact]
        public void TestClearCache()
        {
            // get a mock template loader
            var fileName = "test_template.txt";
            var testTemplateData = "some template data";
            var mockLoader = GetTemplateLoader(fileName, testTemplateData);

            // create a template cache using the mock loader and request a template
            var expiresAfter = 1;
            var cache = new TemplateCache(mockLoader.Object, expiresAfter, expiresAfter);
            var tpl = cache.GetTemplate(fileName);

            // verify the template is cached
            cache.IsTemplateCached(fileName).Should().BeTrue();

            // clear the cache
            cache.ClearCache();

            // verify the template is no longer cached
            cache.IsTemplateCached(fileName).Should().BeFalse();
        }

        [Fact]
        public void TestExpiration()
        {
            // get a mock template loader
            var fileName = "test_template.txt";
            var testTemplateData = "some template data";
            var mockLoader = GetTemplateLoader(fileName, testTemplateData);

            // create a template cache using the mock loader and request a template
            var expiresAfter = 1;
            var cache = new TemplateCache(mockLoader.Object, expiresAfter, expiresAfter);
            var tpl = cache.GetTemplate(fileName);

            // verify the template is cached
            cache.IsTemplateCached(fileName).Should().BeTrue();

            // wait for the cache to expire
            Thread.Sleep(expiresAfter * 1000);

            // verify the template is no longer cached
            cache.IsTemplateCached(fileName).Should().BeFalse();
        }

        [Fact]
        public void TestGetTemplate()
        {
            // get a mock template loader
            var fileName = "test_template.txt";
            var testTemplateData = "some template data";
            var mockLoader = GetTemplateLoader(fileName, testTemplateData);

            // create a template cache using the mock loader
            var expiresAfter = 1;
            var cache = new TemplateCache(mockLoader.Object, expiresAfter);

            // request a template twice
            var tpl = cache.GetTemplate(fileName);
            tpl = cache.GetTemplate(fileName);

            // verify the template and that it was only loaded once
            tpl.ToString().Should().Be(testTemplateData);
            mockLoader.Verify(m => m.LoadTemplate(fileName), Times.Once);
        }

        [Fact]
        public void TestNew_ExpirationError()
        {
            // get a mock template loader
            var mockLoader = GetTemplateLoader("", "");

            // verify the constructor error for an invalid expiration interval
            var exception = Record.Exception(() => new TemplateCache(mockLoader.Object, -2, 2));
            exception.Should().BeOfType<ArgumentException>();
            exception.Message.Should().Be("Expiration seconds must be greater than zero.");
        }

        [Fact]
        public void TestNew_FlushIntervalError()
        {
            // get a mock template loader
            var mockLoader = GetTemplateLoader("", "");

            // verify the constructor error for an invalid flush interval
            var exception = Record.Exception(() => new TemplateCache(mockLoader.Object, 2, -2));
            exception.Should().BeOfType<ArgumentException>();
            exception.Message.Should().Be("Flush interval must be greater than zero.");

        }

        private static Mock<ITemplateLoader> GetTemplateLoader(string fileName, string fileData)
        {
            var moq = new Mock<ITemplateLoader>();

            moq.Setup(m => m.LoadTemplate(It.Is<string>(i => i == fileName)))
                .Returns(fileData);

            return moq;
        }
    }

}
