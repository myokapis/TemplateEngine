﻿/* ****************************************************************************
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
using System.Linq;

namespace TemplateEngine
{

    public static class TrueValues
    {

        // TODO: provide a way for developers to set and get values that are localized
        static TrueValues()
        {
            Values = new List<string> { "Yes", "Y", "T", "True", "1" };
        }

        public static bool Contains(string value)
        {
            return Values.Any(v => string.Equals(value, v, StringComparison.CurrentCultureIgnoreCase));
        }

        public static List<string> Values { get; set; }

    }

}
