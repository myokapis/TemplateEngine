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

using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace TemplateEngine.Tests
{
    public class TrueValuesTests
    {

        [Fact]
        public void TestContains()
        {
            var data = new List<string> { "0", "Yes", "l", "Y", "yes", "y", "T", "t", "True", "true", "1", "~" };
            var actuals = data.Select(d => TrueValues.Contains(d));
            var expected = new List<bool> { false, true, false, true, true, true, true, true, true, true, true, false};
            actuals.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void TestDefaultValues()
        {
            var expected = new List<string> { "Yes", "Y", "T", "True", "1" };
            TrueValues.Values.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void TestValues()
        {
            // change the list of true values
            TrueValues.Values.Remove("Yes");
            TrueValues.Values.Add("Cat");
            var expected = new List<string> { "Cat", "Y", "T", "True", "1" };
            TrueValues.Values.Should().BeEquivalentTo(expected);
        }

    }
}
