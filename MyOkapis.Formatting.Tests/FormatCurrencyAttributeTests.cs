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

using FluentAssertions;
using Xunit;
using MyOkapis.Formatting.Tests.Helpers;
using MyOkapis.Formatting.Tests.Models;

namespace MyOkapis.Formatting.Tests
{

    [Collection("FormatAttributeTests")]
    public class FormatCurrencyAttributeTests
    {

        public static IEnumerable<object[]> GetTestData => FormatterTestHelpers.GetTestData();

        [Theory]
        [MemberData(nameof(GetTestData))]
        public void TestFormatCurrencyAttribute_WithFormatString(object testData)
        {
            var data = (FormatterTestInfo)testData;

            if (data.FormatStringCurrency != null)
            {
                FormatterTestHelpers.TestInCulture(data.Culture, () =>
                {
                    var attr = new FormatCurrencyAttribute(data.FormatStringCurrency);
                    var actual = Formatter.Default.Format(data.NumberValue, attr);
                    actual.Should().Be(data.ExpectedCurrencyValue);
                });
            }
        }

        [Theory]
        [MemberData(nameof(GetTestData))]
        public void TestFormatCurrencyAttribute_WithFormatInfo(object testData)
        {
            var data = (FormatterTestInfo)testData;

            FormatterTestHelpers.TestInCulture(data.Culture, () =>
            {
                var attr = new FormatCurrencyAttribute(data.NumberFormatter);
                var actual = Formatter.Default.Format(data.NumberValue, attr);
                actual.Should().Be(data.ExpectedCurrencyValue);
            });
        }

        [Theory]
        [MemberData(nameof(GetTestData))]
        public void TestFormatCurrencyAttribute_WithParams(object testData)
        {
            var data = (FormatterTestInfo)testData;

            FormatterTestHelpers.TestInCulture(data.Culture, () =>
            {
                var attr = new FormatCurrencyAttribute(data.DecimalPlaces, data.NegativePattern,
                data.GroupSeparator, data.DecimalSeparator);

                var actual = Formatter.Default.Format(data.NumberValue, attr);
                actual.Should().Be(data.ExpectedCurrencyValue);
            });
        }

    }

}
