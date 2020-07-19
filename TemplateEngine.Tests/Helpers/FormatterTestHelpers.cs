/* ****************************************************************************
Copyright 2018-2022 Gene Graves

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
using TemplateEngine.Tests.Models;

namespace TemplateEngine.Tests.Helpers
{

    internal static class FormatterTestHelpers
    {

        private static readonly CultureInfo[] cultures = new CultureInfo[]
        {
            new CultureInfo("en-US"),
            new CultureInfo("en-CA"),
            new CultureInfo("es-ES"),
            new CultureInfo("de-DE")
        };

        private static readonly FormatterTestInfo[] testData = new FormatterTestInfo[]
        {
            new FormatterTestInfo()
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
            new FormatterTestInfo()
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
            new FormatterTestInfo()
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
            new FormatterTestInfo()
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
            new FormatterTestInfo()
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

            if (decimalSeparator != null)
            {
                f.CurrencyDecimalSeparator = decimalSeparator;
                f.NumberDecimalSeparator = decimalSeparator;
                f.PercentDecimalSeparator = decimalSeparator;
            }

            if (groupSeparator != null)
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
            foreach (var data in testData)
            {
                yield return new object[] { data };
            }
        }

        public static void TestInCulture(CultureInfo culture, Action action)
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

    }

}
