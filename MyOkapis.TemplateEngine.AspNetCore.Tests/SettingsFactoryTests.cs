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
using MyOkapis.TemplateEngine.AspNetCore.Tests.Helpers;
using Xunit;

namespace MyOkapis.TemplateEngine.AspNetCore.Tests
{

    public class SettingsFactoryTests
    {

        [Fact]
        public void TestGetDefaultSettings_UsesContentRootAndTemplatesConvention()
        {
            var environment = new FakeHostEnvironment { ContentRootPath = "/app/root" };

            var settings = SettingsFactory.GetDefaultSettings(environment);

            settings.TemplateDirectory.Should().Be(Path.Combine("/app/root", "Templates"));
            settings.UseCache.Should().BeTrue();
        }

        [Fact]
        public void TestGetSettingsFromConfig_ReturnsNull_WhenSectionMissing()
        {
            // This is the exact case that used to be broken -- an absent "TemplateEngine" section
            // must return null so SettingsFactory's constructor can fall back to convention,
            // rather than the caller crashing or silently getting garbage settings.
            var configuration = new ConfigurationBuilder().Build();

            var settings = SettingsFactory.GetSettingsFromConfig(configuration);

            settings.Should().BeNull();
        }

        [Fact]
        public void TestGetSettingsFromConfig_BindsSection_WhenPresent()
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["TemplateEngine:TemplateDirectory"] = "/configured/templates",
                    ["TemplateEngine:UseCache"] = "false"
                })
                .Build();

            var settings = SettingsFactory.GetSettingsFromConfig(configuration);

            settings.Should().NotBeNull();
            settings!.TemplateDirectory.Should().Be("/configured/templates");
            settings.UseCache.Should().BeFalse();
        }

        [Fact]
        public void TestConstructor_PrefersConfigurationOverConvention()
        {
            var environment = new FakeHostEnvironment { ContentRootPath = "/app/root" };
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["TemplateEngine:TemplateDirectory"] = "/configured/templates",
                })
                .Build();

            var factory = new SettingsFactory(environment, configuration);

            factory.Settings.TemplateDirectory.Should().Be("/configured/templates");
        }

        [Fact]
        public void TestConstructor_FallsBackToConventionWhenConfigMissing()
        {
            var environment = new FakeHostEnvironment { ContentRootPath = "/app/root" };
            var configuration = new ConfigurationBuilder().Build();

            var factory = new SettingsFactory(environment, configuration);

            factory.Settings.TemplateDirectory.Should().Be(Path.Combine("/app/root", "Templates"));
            factory.Settings.UseCache.Should().BeTrue();
        }
    }
}
