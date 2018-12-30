/* ****************************************************************************
Copyright 2018 Gene Graves

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

namespace TemplateEngine.Tests
{

    public class SectionOptionsTests
    {

        [Fact]
        public void TestSectionOptions_AppendDeselect()
        {
            var option = SectionOptions.AppendDeselect;
            option.Append.Should().Be(true);
            option.Deselect.Should().Be(true);
        }

        [Fact]
        public void TestSectionOptions_AppendOnly()
        {
            var option = SectionOptions.AppendOnly;
            option.Append.Should().Be(true);
            option.Deselect.Should().Be(false);
        }

        [Fact]
        public void TestSectionOptions_Set()
        {
            var option = SectionOptions.Set;
            option.Append.Should().Be(false);
            option.Deselect.Should().Be(false);
        }

    }

}
