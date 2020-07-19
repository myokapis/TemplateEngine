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
using System.Text;

namespace TemplateEngine.Tests.Models
{

    internal class FormatterTestInfo
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

}
