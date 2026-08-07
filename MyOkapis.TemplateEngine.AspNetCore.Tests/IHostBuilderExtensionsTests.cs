/* ****************************************************************************
Copyright 2018-2026 Gene Graves

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

using System.IO;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MyOkapis.TemplateEngine.AspNetCore.Extensions;
using MyOkapis.TemplateEngine.Loader;
using MyOkapis.TemplateEngine.Web;
using Xunit;

namespace MyOkapis.TemplateEngine.AspNetCore.Tests
{

    public class IHostBuilderExtensionsTests : IDisposable
    {
        private readonly List<string> tempDirectories = new();

        private string CreateContentRootWithTemplatesFolder()
        {
            var contentRoot = Path.Combine(Path.GetTempPath(), "aspnetcore-tests-" + Guid.NewGuid());
            Directory.CreateDirectory(Path.Combine(contentRoot, "Templates"));
            tempDirectories.Add(contentRoot);
            return contentRoot;
        }

        public void Dispose()
        {
            foreach (var path in tempDirectories)
            {
                if (Directory.Exists(path)) Directory.Delete(path, recursive: true);
            }
        }

        [Fact]
        public void TestUseTemplateEngine_RegistersSettingsFromConvention()
        {
            var contentRoot = CreateContentRootWithTemplatesFolder();

            using var host = new HostBuilder()
                .UseContentRoot(contentRoot)
                .UseTemplateEngine()
                .Build();

            var settings = host.Services.GetRequiredService<TemplateEngineSettings>();

            settings.TemplateDirectory.Should().Be(Path.Combine(contentRoot, "Templates"));
            settings.UseCache.Should().BeTrue();
        }

        [Fact]
        public void TestUseTemplateEngine_RegistersBothCacheAndLoader_WhenCachingEnabled()
        {
            var contentRoot = CreateContentRootWithTemplatesFolder();

            using var host = new HostBuilder()
                .UseContentRoot(contentRoot)
                .UseTemplateEngine()
                .Build();

            host.Services.GetService<ITemplateCache<IWebWriter>>().Should().NotBeNull();
            host.Services.GetService<ITemplateLoader<IWebWriter>>().Should().NotBeNull();
        }

        [Fact]
        public void TestUseTemplateEngine_TemplateLoaderUsesTheSameSettingsAsTemplateCache()
        {
            var contentRoot = CreateContentRootWithTemplatesFolder();

            using var host = new HostBuilder()
                .UseContentRoot(contentRoot)
                .UseTemplateEngine()
                .Build();

            var cache = host.Services.GetRequiredService<ITemplateCache<IWebWriter>>();
            var loader = host.Services.GetRequiredService<ITemplateLoader<IWebWriter>>();

            cache.TemplateDirectory.Should().Be(loader.TemplateDirectory);
        }
    }
}
