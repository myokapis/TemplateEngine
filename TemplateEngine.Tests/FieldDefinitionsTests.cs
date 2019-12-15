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
using FluentAssertions;
using Xunit;

namespace TemplateEngine.Tests
{

    public class FieldDefinitionsTests
    {

        [Fact]
        public void TestDefaultConstructor()
        {
            var fd = new FieldDefinitions();
            fd.Checkboxes.Should().BeEquivalentTo(new string[] { });
            fd.DropdownFieldNames.Should().BeEquivalentTo(new List<string>() );
            fd.Dropdowns.Should().BeEquivalentTo(new List<DropdownDefinition>());
        }

        [Fact]
        public void TestAlternateConstructor()
        {
            // setup checkboxes
            var cb = new List<string> { "Checkbox 1", "Checkbox 2", "Checkbox3" };

            // setup dropdowns
            var dd = new List<DropdownDefinition>
            {
                new DropdownDefinition("OPTION_SECTION1", "Field1", new List<Option>()
                {
                    new Option() { Text = "Text 1", Value = "1" },
                    new Option() { Text = "Text 2", Value = "2" },
                    new Option() { Text = "Text 3", Value = "3" },
                    new Option() { Text = "Text 4", Value = "4" }
                }),
                new DropdownDefinition("OPTION_SECTION2", "Field2", new List<Option>()
                {
                    new Option() { Text = "Name 1", Value = "Val1" },
                    new Option() { Text = "Name 2", Value = "Val2" },
                    new Option() { Text = "Name 3", Value = "Val3" },
                    new Option() { Text = "Name 4", Value = "Val4" }
                })
            };

            var fd = new FieldDefinitions(cb, dd);

            fd.Checkboxes.Should().BeEquivalentTo(cb);
            fd.DropdownFieldNames.Should().BeEquivalentTo(new List<string> { "Field1", "Field2" });
            fd.Dropdowns.Should().BeEquivalentTo(dd);
        }

        [Fact]
        public void TestSetCheckboxes()
        {
            // setup checkboxes
            var cb = new List<string> { "Checkbox 1", "Checkbox 2", "Checkbox3" };

            var fd = new FieldDefinitions();
            fd.SetCheckboxes("Checkbox 1", "Checkbox 2", "Checkbox3");

            fd.Checkboxes.Should().BeEquivalentTo(cb);
            fd.DropdownFieldNames.Should().BeEquivalentTo(new List<string> { });
            fd.Dropdowns.Should().BeEquivalentTo(new List<DropdownDefinition>());
        }

        [Fact]
        public void TestSetDropDowns()
        {

            // setup dropdowns
            var dd = new DropdownDefinition[]
            {
                new DropdownDefinition("OPTION_SECTION1", "Field1", new List<Option>()
                {
                    new Option() { Text = "Text 1", Value = "1" },
                    new Option() { Text = "Text 2", Value = "2" },
                    new Option() { Text = "Text 3", Value = "3" },
                    new Option() { Text = "Text 4", Value = "4" }
                }),
                new DropdownDefinition("OPTION_SECTION2", "Field2", new List<Option>()
                {
                    new Option() { Text = "Name 1", Value = "Val1" },
                    new Option() { Text = "Name 2", Value = "Val2" },
                    new Option() { Text = "Name 3", Value = "Val3" },
                    new Option() { Text = "Name 4", Value = "Val4" }
                })
            };

            var fd = new FieldDefinitions();
            fd.SetDropdowns(dd);

            fd.Checkboxes.Should().BeEquivalentTo(new string[] { });
            fd.DropdownFieldNames.Should().BeEquivalentTo(new List<string> { "Field1", "Field2" });
            fd.Dropdowns.Should().BeEquivalentTo(dd);
        }

    }

}
