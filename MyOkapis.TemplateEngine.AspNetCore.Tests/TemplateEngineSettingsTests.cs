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

using FluentAssertions;
using Xunit;

namespace MyOkapis.TemplateEngine.AspNetCore.Tests
{

    public class TemplateEngineSettingsTests
    {

        [Fact]
        public void TestDefaults()
        {
            var settings = new TemplateEngineSettings();

            settings.TemplateDirectory.Should().Be("Templates");
            settings.UseCache.Should().BeTrue();
        }
    }
}
