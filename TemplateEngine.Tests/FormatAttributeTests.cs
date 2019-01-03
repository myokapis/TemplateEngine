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

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using TemplateEngine.Formats;

[assembly: CollectionBehavior(DisableTestParallelization = true)]
namespace TemplateEngine.Tests
{

    public class FormatAttributeTests
    {

        #region Currency Formatter Tests

        [Theory]
        [MemberData(nameof(GetTestData))]
        public void TestFormatCurrencyAttribute_WithFormatString(object testData)
        {
            var data = testData as TestData;

            if (data.FormatStringCurrency != null)
            {
                TestInCulture(data.Culture, () =>
                {
                    var attr = new FormatCurrencyAttribute(data.FormatStringCurrency);
                    var actual = attr.FormatData(data.NumberValue);
                    actual.Should().Be(data.ExpectedCurrencyValue);
                });
            }
        }

        [Theory]
        [MemberData(nameof(GetTestData))]
        public void TestFormatCurrencyAttribute_WithFormatInfo(object testData)
        {
            var data = testData as TestData;

            TestInCulture(data.Culture, () =>
            {
                var attr = new FormatCurrencyAttribute(data.NumberFormatter);
                var actual = attr.FormatData(data.NumberValue);
                actual.Should().Be(data.ExpectedCurrencyValue);
            });
        }

        [Theory]
        [MemberData(nameof(GetTestData))]
        public void TestFormatCurrencyAttribute_WithParams(object testData)
        {
            var data = testData as TestData;

            TestInCulture(data.Culture, () =>
            {
                var attr = new FormatCurrencyAttribute(data.DecimalPlaces, data.NegativePattern,
                data.GroupSeparator, data.DecimalSeparator);

                var actual = attr.FormatData(data.NumberValue);
                actual.Should().Be(data.ExpectedCurrencyValue);
            });
        }

        #endregion

        #region Date Formatter Tests

        [Theory]
        [MemberData(nameof(GetTestData))]
        public void TestFormatDateAttribute_WithFormatString(object testData)
        {
            var data = testData as TestData;

            TestInCulture(data.Culture, () =>
            {
                var attr = new FormatDateAttribute(data.FormatStringDate);
                var actual = attr.FormatData(data.DateValue);
                actual.Should().Be(data.ExpectedDateValue);
            });
        }

        [Theory]
        [MemberData(nameof(GetTestData))]
        public void TestFormatDateAttribute_WithFormatInfo(object testData)
        {
            var data = testData as TestData;

            if (data.DateFormatter != null)
            {
                TestInCulture(data.Culture, () =>
                {
                    var attr = new FormatDateAttribute(data.DateFormatter);
                    var actual = attr.FormatData(data.DateValue);
                    actual.Should().Be(data.ExpectedDateValue);
                });
            }
        }

        #endregion

        #region Integer Formatter Tests

        [Theory]
        [MemberData(nameof(GetTestData))]
        public void TestFormatIntegerAttribute_WithFormatString(object testData)
        {
            var data = testData as TestData;

            TestInCulture(data.Culture, () =>
            {
                var attr = new FormatIntegerAttribute(data.FormatStringInteger);
                var actual = attr.FormatData(data.NumberValue);
                actual.Should().Be(data.ExpectedIntegerValue);
            });
        }

        [Theory]
        [MemberData(nameof(GetTestData))]
        public void TestFormatIntegerAttribute_WithFormatInfo(object testData)
        {
            var data = testData as TestData;

            TestInCulture(data.Culture, () =>
            {
                var attr = new FormatIntegerAttribute(data.NumberFormatter);
                var actual = attr.FormatData(data.NumberValue);
                actual.Should().Be(data.ExpectedIntegerValue);
            });
        }

        [Theory]
        [MemberData(nameof(GetTestData))]
        public void TestFormatIntegerAttribute_WithParams(object testData)
        {
            var data = testData as TestData;

            TestInCulture(data.Culture, () =>
            {
                var attr = new FormatIntegerAttribute(data.NegativePattern, data.GroupSeparator);
                var actual = attr.FormatData(data.NumberValue);
                actual.Should().Be(data.ExpectedIntegerValue);
            });
        }

        #endregion

        #region Number Formatter Tests

        [Theory]
        [MemberData(nameof(GetTestData))]
        public void TestFormatNumberAttribute_WithFormatString(object testData)
        {
            var data = testData as TestData;

            if(data.FormatStringNumber != null)
            {
                TestInCulture(data.Culture, () =>
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
            var data = testData as TestData;

            TestInCulture(data.Culture, () =>
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
            var data = testData as TestData;
            
            TestInCulture(data.Culture, () =>
            {
                var attr = new FormatNumberAttribute(data.DecimalPlaces, data.NegativePattern,
                data.GroupSeparator, data.DecimalSeparator);

                var actual = attr.FormatData(data.NumberValue);
                actual.Should().Be(data.ExpectedNumberValue);
            });
        }

        #endregion

        #region Percent Formatter Tests

        [Theory]
        [MemberData(nameof(GetTestData))]
        public void TestFormatPercentAttribute_WithFormatString(object testData)
        {
            var data = testData as TestData;

            if (data.FormatStringNumber != null)
            {
                TestInCulture(data.Culture, () =>
                {
                    var attr = new FormatPercentAttribute(data.FormatStringPercent);
                    var actual = attr.FormatData(data.NumberValue);
                    actual.Should().Be(data.ExpectedPercentValue);
                });
            }
        }

        [Theory]
        [MemberData(nameof(GetTestData))]
        public void TestFormatPercentAttribute_WithFormatInfo(object testData)
        {
            var data = testData as TestData;

            TestInCulture(data.Culture, () =>
            {
                var attr = new FormatPercentAttribute(data.NumberFormatter);
                var actual = attr.FormatData(data.NumberValue);
                actual.Should().Be(data.ExpectedPercentValue);
            });
        }

        [Theory]
        [MemberData(nameof(GetTestData))]
        public void TestFormatPercentAttribute_WithParams(object testData)
        {
            var data = testData as TestData;

            TestInCulture(data.Culture, () =>
            {
                var attr = new FormatPercentAttribute(data.DecimalPlaces, data.NegativePattern,
                data.GroupSeparator, data.DecimalSeparator);

                var actual = attr.FormatData(data.NumberValue);
                actual.Should().Be(data.ExpectedPercentValue);
            });
        }


        #endregion

        #region Helpers

        private static CultureInfo[] cultures = new CultureInfo[]
        {
            new CultureInfo("en-US"),
            new CultureInfo("en-CA"),
            new CultureInfo("es-ES"),
            new CultureInfo("de-DE")
        };

        private static TestData[] testData = new TestData[]
        {
            new TestData()
            {
                Culture = cultures[0],
                DateFormatter = GetDateFormatter(cultures[0]),
                DateValue = new DateTime(2025, 11, 13, 23, 47, 33),
                DecimalPlaces = 2,
                DecimalSeparator = null,
                ExpectedCurrencyValue = "-$123,456.79",
                ExpectedDateValue = "2025-11-13 11:47:33 PM",
                ExpectedIntegerValue = "-123,457",
                ExpectedNumberValue = "-123,456.79",
                ExpectedPercentValue = "-12,345,678.90%",
                FormatStringCurrency = "$###,###,##0.00",
                FormatStringDate = "yyyy-MM-dd hh:mm:ss tt",
                FormatStringInteger = "###,##0",
                FormatStringNumber = "###,##0.00",
                FormatStringPercent = "###,###,##0.00%",
                GroupSeparator = null,
                NegativePattern = 1,
                NumberFormatter = GetNumberFormatter(cultures[0]),
                NumberValue = -123456.789,
            },
            new TestData()
            {
                Culture = cultures[0],
                DateValue = new DateTime(2025, 11, 13, 23, 47, 33),
                DecimalPlaces = 3,
                DecimalSeparator = "x",
                ExpectedCurrencyValue = "-$123y456x789",
                ExpectedDateValue = "2025.11.13 11:47:33 PM",
                ExpectedIntegerValue = "-123y457",
                ExpectedNumberValue = "-123y456x789",
                ExpectedPercentValue = "-12y345y678x900%",
                FormatStringDate = "yyyy.MM.dd hh:mm:ss tt",
                FormatStringInteger = "###y##0",
                FormatStringPercent = "###y###y##0x000%",
                GroupSeparator = "y",
                NegativePattern = 1,
                NumberFormatter = GetNumberFormatter(cultures[0], 3, "x", "y", 1),
                NumberValue = -123456.789
            },
            new TestData()
            {
                Culture = cultures[2],
                DateFormatter = GetDateFormatter(cultures[2]),
                DateValue = null,
                DecimalPlaces = 2,
                DecimalSeparator = ".",
                ExpectedCurrencyValue = "",
                ExpectedDateValue = "",
                ExpectedIntegerValue = "",
                ExpectedNumberValue = "",
                ExpectedPercentValue = "",
                FormatStringCurrency = "C",
                FormatStringDate = "G",
                FormatStringInteger = "N0",
                FormatStringNumber = "N",
                FormatStringPercent = "P",
                GroupSeparator = ",",
                NegativePattern = 1,
                NumberFormatter = GetNumberFormatter(cultures[2], 2, ".", ",", 1),
                NumberValue = null
            },
            new TestData()
            {
                Culture = cultures[3],
                DateFormatter = GetDateFormatter(cultures[3]),
                DateValue = new DateTime(2025, 11, 13, 23, 47, 33),
                DecimalPlaces = 4,
                DecimalSeparator = null,
                ExpectedCurrencyValue = "-€23.456,7890",
                ExpectedDateValue = "13.11.2025 23:47:33",
                ExpectedIntegerValue = "-23.457",
                ExpectedNumberValue = "-23.456,7890",
                ExpectedPercentValue = "-2.345.678,9000%",
                FormatStringCurrency = "€###,##0.0000",
                FormatStringDate = "dd.MM.yyyy HH:mm:ss",
                FormatStringInteger = "###'.'##0",
                FormatStringNumber = "###,##0.0000",
                FormatStringPercent = "###,###,##0.0000%",
                GroupSeparator = ".",
                NegativePattern = 1,
                NumberFormatter = GetNumberFormatter(cultures[3], 4),
                NumberValue = -23456.789
            },
            new TestData()
            {
                Culture = cultures[3],
                DateValue = new DateTime(2025, 11, 13, 23, 47, 33),
                DecimalPlaces = 2,
                DecimalSeparator = ".",
                ExpectedCurrencyValue = "-€123,456.79",
                ExpectedDateValue = "13/11/2025 23:47:33",
                ExpectedIntegerValue = "-123,457",
                ExpectedNumberValue = "-123,456.79",
                ExpectedPercentValue = "-12,345,678.90%",
                FormatStringDate = "dd'/'MM'/'yyyy HH:mm:ss",
                FormatStringInteger = "###','##0",
                FormatStringPercent = "###','###','##0'.'00%",
                GroupSeparator = ",",
                NegativePattern = 1,
                NumberFormatter = GetNumberFormatter(cultures[3], 2, ".", ",", 1),
                NumberValue = -123456.789
            }
        };

        private static DateTimeFormatInfo GetDateFormatter(CultureInfo culture)
        {
            return (DateTimeFormatInfo)culture.DateTimeFormat.Clone();
        }

        private static NumberFormatInfo GetNumberFormatter(CultureInfo culture, int decimalPlaces = 2,
            string decimalSeparator = null, string groupSeparator = null, int negativePattern = 1)
        {
            var f = (NumberFormatInfo)culture.NumberFormat.Clone();

            f.CurrencyDecimalDigits = decimalPlaces;
            f.NumberDecimalDigits = decimalPlaces;
            f.PercentDecimalDigits = decimalPlaces;

            if(decimalSeparator != null)
            {
                f.CurrencyDecimalSeparator = decimalSeparator;
                f.NumberDecimalSeparator = decimalSeparator;
                f.PercentDecimalSeparator = decimalSeparator;
            }

            if(groupSeparator != null)
            {
                f.CurrencyGroupSeparator = groupSeparator;
                f.NumberGroupSeparator = groupSeparator;
                f.PercentGroupSeparator = groupSeparator;
            }

            f.CurrencyNegativePattern = negativePattern;
            f.NumberNegativePattern = negativePattern;
            f.PercentNegativePattern = negativePattern;

            return f;
        }

        public static IEnumerable<object[]> GetTestData()
        {
            foreach(var data in testData)
            {
                yield return new object[] { data };
            }
        }

        private void TestInCulture(CultureInfo culture, Action action)
        {
            var currentCulture = CultureInfo.CurrentCulture;

            try
            {
                CultureInfo.CurrentCulture = culture;
                action.Invoke();
            }
            finally
            {
                CultureInfo.CurrentCulture = currentCulture;
            }
        }

        public class TestData
        {
            public CultureInfo Culture { get; set; }
            public DateTimeFormatInfo DateFormatter { get; set; }
            public object DateValue { get; set; }
            public int DecimalPlaces { get; set; }
            public string DecimalSeparator { get; set; }
            public string ExpectedCurrencyValue { get; set; }
            public string ExpectedDateValue { get; set; }
            public string ExpectedIntegerValue { get; set; }
            public string ExpectedNumberValue { get; set; }
            public string ExpectedPercentValue { get; set; }
            public string FormatStringCurrency { get; set; }
            public string FormatStringDate { get; set; }
            public string FormatStringInteger { get; set; }
            public string FormatStringNumber { get; set; }
            public string FormatStringPercent { get; set; }
            public string GroupSeparator { get; set; }
            public int NegativePattern { get; set; }
            public NumberFormatInfo NumberFormatter { get; set; }
            public object NumberValue { get; set; }
        }

        #endregion

    }

}
