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

using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using MyOkapis.TemplateEngine.Writer;
using Xunit;

namespace MyOkapis.TemplateEngine.Tests
{
    public class TrueValuesTests
    {

        [Fact]
        public void TestContains()
        {
            var data = new List<string> { "0", "Yes", "l", "Y", "yes", "y", "T", "t", "True", "true", "1", "~" };
            var actuals = data.Select(d => TrueValues.Default.Contains(d));
            var expected = new List<bool> { false, true, false, true, true, true, true, true, true, true, true, false };
            actuals.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void TestDefaultValues()
        {
            var expected = new List<string> { "Yes", "Y", "T", "True", "1" };
            TrueValues.Default.Values.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void TestCustomInstanceDoesNotAffectDefault()
        {
            // constructing a custom instance is the supported way to use different True tokens;
            // it must never mutate the shared Default instance
            var custom = new TrueValues(new[] { "Cat", "Y", "T", "True", "1" });

            custom.Values.Should().BeEquivalentTo(new List<string> { "Cat", "Y", "T", "True", "1" });
            custom.Contains("Cat").Should().BeTrue();
            custom.Contains("Yes").Should().BeFalse();

            TrueValues.Default.Values.Should().BeEquivalentTo(new List<string> { "Yes", "Y", "T", "True", "1" });
        }

    }
}
