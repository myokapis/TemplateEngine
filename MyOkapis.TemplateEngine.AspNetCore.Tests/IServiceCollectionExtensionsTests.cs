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
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MyOkapis.TemplateEngine.AspNetCore.Extensions;
using MyOkapis.TemplateEngine.AspNetCore.Tests.Helpers;
using MyOkapis.TemplateEngine.Loader;
using MyOkapis.TemplateEngine.Web;
using Xunit;

namespace MyOkapis.TemplateEngine.AspNetCore.Tests
{

    public class IServiceCollectionExtensionsTests : IDisposable
    {
        // TemplateLoaderBase validates that the template directory actually exists on disk, so
        // these tests need real directories, not arbitrary path strings.
        private readonly List<string> tempDirectories = new();

        private string CreateTempDirectory()
        {
            var path = Path.Combine(Path.GetTempPath(), "aspnetcore-tests-" + Guid.NewGuid());
            Directory.CreateDirectory(path);
            tempDirectories.Add(path);
            return path;
        }

        public void Dispose()
        {
            foreach (var path in tempDirectories)
            {
                if (Directory.Exists(path)) Directory.Delete(path, recursive: true);
            }
        }

        private static ServiceCollection ServicesWithEnvironmentAndConfig(
            string contentRootPath,
            IConfiguration? configuration = null)
        {
            var services = new ServiceCollection();
            services.AddSingleton<IHostEnvironment>(new FakeHostEnvironment { ContentRootPath = contentRootPath });
            services.AddSingleton(configuration ?? new ConfigurationBuilder().Build());
            return services;
        }

        [Fact]
        public void TestAddTemplateSettings_NoArgs_DerivesFromSettingsFactory()
        {
            var contentRoot = CreateTempDirectory();
            var services = ServicesWithEnvironmentAndConfig(contentRoot);
            services.AddTemplateSettings();

            var provider = services.BuildServiceProvider();
            var settings = provider.GetRequiredService<TemplateEngineSettings>();
            var factory = provider.GetRequiredService<SettingsFactory>();

            settings.Should().BeSameAs(factory.Settings);
            settings.TemplateDirectory.Should().Be(Path.Combine(contentRoot, "Templates"));
        }

        [Fact]
        public void TestAddTemplateSettings_WithExplicitInstance_RegistersThatExactInstance()
        {
            var services = new ServiceCollection();
            var explicitSettings = new TemplateEngineSettings { TemplateDirectory = "/explicit" };

            services.AddTemplateSettings(explicitSettings);

            var provider = services.BuildServiceProvider();
            provider.GetRequiredService<TemplateEngineSettings>().Should().BeSameAs(explicitSettings);
        }

        [Fact]
        public void TestAddTemplateCache_RegistersCacheUsingRegisteredSettings()
        {
            var contentRoot = CreateTempDirectory();
            Directory.CreateDirectory(Path.Combine(contentRoot, "Templates"));
            var services = ServicesWithEnvironmentAndConfig(contentRoot);
            services.AddTemplateSettings();
            services.AddTemplateCache();

            var provider = services.BuildServiceProvider();
            var cache = provider.GetRequiredService<ITemplateCache<IWebWriter>>();

            cache.TemplateDirectory.Should().Be(Path.Combine(contentRoot, "Templates"));
        }

        [Fact]
        public void TestAddTemplateLoader_RegistersLoaderUsingRegisteredSettings()
        {
            var contentRoot = CreateTempDirectory();
            Directory.CreateDirectory(Path.Combine(contentRoot, "Templates"));
            var services = ServicesWithEnvironmentAndConfig(contentRoot);
            services.AddTemplateSettings();
            services.AddTemplateLoader();

            var provider = services.BuildServiceProvider();
            var loader = provider.GetRequiredService<ITemplateLoader<IWebWriter>>();

            loader.TemplateDirectory.Should().Be(Path.Combine(contentRoot, "Templates"));
        }

        [Fact]
        public void TestAddPresenters_ZeroArg_FindsSubclassInCallingAssembly()
        {
            // The zero-arg overload uses Assembly.GetCallingAssembly(), which correctly resolves to
            // *this* test assembly since the call happens directly here -- unlike the old
            // GetEntryAssembly()-based implementation, which could never find a presenter defined in
            // this assembly under a test host (see IMPROVEMENTS.md).
            var services = new ServiceCollection();

            services.AddPresenters<ConcretePresenter, PresenterTestBase>();

            var provider = services.BuildServiceProvider();
            provider.GetService<ConcretePresenter>().Should().NotBeNull();
        }

        [Fact]
        public void TestAddPresenters_WithAssemblyMarker_FindsSubclass()
        {
            var services = new ServiceCollection();

            services.AddPresenters<PresenterTestBase, PresenterTestBase>();

            var provider = services.BuildServiceProvider();
            provider.GetService<ConcretePresenter>().Should().NotBeNull();
        }

        [Fact]
        public void TestAddPresenters_ExplicitAssemblyAndMultipleBaseTypes()
        {
            var services = new ServiceCollection();

            services.AddPresenters(typeof(PresenterTestBase).Assembly, typeof(PresenterTestBase), typeof(OtherPresenterTestBase));

            var provider = services.BuildServiceProvider();
            provider.GetService<ConcretePresenter>().Should().NotBeNull();
            provider.GetService<OtherConcretePresenter>().Should().NotBeNull();
        }

        [Fact]
        public void TestAddPresenters_ExcludesAbstractSubclasses()
        {
            var services = new ServiceCollection();

            services.AddPresenters(typeof(PresenterTestBase).Assembly, typeof(PresenterTestBase));

            var provider = services.BuildServiceProvider();
            provider.GetService<AbstractPresenterTestBase>().Should().BeNull();
        }

        [Fact]
        public void TestAddPresenters_RegistersAsScoped()
        {
            // A scoped registration returns the same instance for two resolutions within one scope,
            // and a different instance in a different scope -- distinguishing it from Transient
            // (always different) and Singleton (always the same, even across scopes).
            var services = new ServiceCollection();
            services.AddPresenters<ConcretePresenter, PresenterTestBase>();
            var provider = services.BuildServiceProvider();

            using var scopeA = provider.CreateScope();
            var first = scopeA.ServiceProvider.GetRequiredService<ConcretePresenter>();
            var second = scopeA.ServiceProvider.GetRequiredService<ConcretePresenter>();
            first.Should().BeSameAs(second);

            using var scopeB = provider.CreateScope();
            var third = scopeB.ServiceProvider.GetRequiredService<ConcretePresenter>();
            third.Should().NotBeSameAs(first);
        }

        [Fact]
        public void TestAddTemplateEngine_DefaultsToUsingCache()
        {
            var contentRoot = CreateTempDirectory();
            Directory.CreateDirectory(Path.Combine(contentRoot, "Templates"));
            var services = ServicesWithEnvironmentAndConfig(contentRoot);
            services.AddTemplateEngine<PresenterTestBase>(configuration: null);

            var provider = services.BuildServiceProvider();
            provider.GetService<ITemplateCache<IWebWriter>>().Should().NotBeNull();
        }

        [Fact]
        public void TestAddTemplateEngine_UseCacheFalse_RegistersLoaderNotCache()
        {
            var contentRoot = CreateTempDirectory();
            var services = ServicesWithEnvironmentAndConfig(contentRoot);
            services.AddTemplateEngine<PresenterTestBase>(configuration: null,
                settings: new TemplateEngineSettings { TemplateDirectory = contentRoot, UseCache = false });

            var provider = services.BuildServiceProvider();
            provider.GetService<ITemplateLoader<IWebWriter>>().Should().NotBeNull();
            provider.GetService<ITemplateCache<IWebWriter>>().Should().BeNull();
        }

        [Fact]
        public void TestAddTemplateEngine_ExplicitSettingsAreUsedByTheTemplateLoader()
        {
            // AddTemplateEngine(configuration, settings) lets a caller pass an explicit
            // TemplateEngineSettings instance. That instance must actually be what the template
            // cache/loader use -- not just something registered for unrelated direct injection
            // while the loader silently falls back to config/convention instead.
            var defaultRoot = CreateTempDirectory(); // deliberately NOT where explicitDir points
            var explicitDir = CreateTempDirectory();
            var services = ServicesWithEnvironmentAndConfig(defaultRoot);
            var explicitSettings = new TemplateEngineSettings { TemplateDirectory = explicitDir, UseCache = false };

            services.AddTemplateEngine<PresenterTestBase>(configuration: null, settings: explicitSettings);

            var provider = services.BuildServiceProvider();
            var loader = provider.GetRequiredService<ITemplateLoader<IWebWriter>>();

            loader.TemplateDirectory.Should().Be(explicitDir);
        }

        public class PresenterTestBase { }
        public class ConcretePresenter : PresenterTestBase { }
        public abstract class AbstractPresenterTestBase : PresenterTestBase { }

        public class OtherPresenterTestBase { }
        public class OtherConcretePresenter : OtherPresenterTestBase { }
    }
}
