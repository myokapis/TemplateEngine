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
using FluentAssertions;
using Xunit;
using TemplateEngine.Formatters;
using TemplateEngine.Tests.Helpers;
using TemplateEngine.Tests.Models;

namespace TemplateEngine.Tests.FormatterTests
{

    [Collection("FormatAttributeTests")]
    public class FormatNumberAttributeTests
    {

        public static IEnumerable<object[]> GetTestData => FormatterTestHelpers.GetTestData();

        [Theory]
        [MemberData(nameof(GetTestData))]
        public void TestFormatNumberAttribute_WithFormatString(object testData)
        {
            var data = (FormatterTestInfo)testData;

            if (data.FormatStringNumber != null)
            {
                FormatterTestHelpers.TestInCulture(data.Culture, () =>
                {
                    var attr = new FormatNumberAttribute(data.FormatStringNumber);
                    var actual = attr.FormatData(data.NumberValue);
                    actual.Should().Be(data.ExpectedNumberValue);
                });
            }
        }

        [Theory]
        [MemberData(nameof(GetTestData))]
        public void TestFormatNumberAttribute_WithFormatInfo(object testData)
        {
            var data = (FormatterTestInfo)testData;

            FormatterTestHelpers.TestInCulture(data.Culture, () =>
            {
                var attr = new FormatNumberAttribute(data.NumberFormatter);
                var actual = attr.FormatData(data.NumberValue);
                actual.Should().Be(data.ExpectedNumberValue);
            });
        }

        [Theory]
        [MemberData(nameof(GetTestData))]
        public void TestFormatNumberAttribute_WithParams(object testData)
        {
            var data = (FormatterTestInfo)testData;

            FormatterTestHelpers.TestInCulture(data.Culture, () =>
            {
                var attr = new FormatNumberAttribute(data.DecimalPlaces, data.NegativePattern,
                data.GroupSeparator, data.DecimalSeparator);

                var actual = attr.FormatData(data.NumberValue);
                actual.Should().Be(data.ExpectedNumberValue);
            });
        }

    }

}
