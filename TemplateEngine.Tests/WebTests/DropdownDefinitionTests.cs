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
using TemplateEngine.Web;
using Xunit;

namespace TemplateEngine.Tests.WebTests
{
    public class DropdownDefinitionTests
    {

        [Fact]
        public void TestInitialization()
        {
            var fieldName = "Field1";
            var sectionName = "Section1";

            var data = new List<Option>
            {
                new Option{ Text = "", Value = "" },
                new Option{ Text = "", Value = "" },
                new Option{ Text = "", Value = "" }
            };

            var dropdownDefinition = new DropdownDefinition(sectionName, fieldName, data);

            dropdownDefinition.Data.Should().BeSameAs(data);
            dropdownDefinition.FieldName.Should().BeSameAs(fieldName);
            dropdownDefinition.SectionName.Should().BeSameAs(sectionName);
        }

    }

}
