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
using System.Text;
using System.Web;
using FluentAssertions;
using TemplateEngine.Document;
using TemplateEngine.Tests.Helpers;
using TemplateEngine.Tests.Models;
using TemplateEngine.Web;
using TemplateEngine.Writer;
using Xunit;

namespace TemplateEngine.Tests
{

    public class WebWriterTests
    {
        private readonly List<string> templateTexts = TemplateText.Items;

        [Fact]
        public void TestCopy()
        {
            // create a template and writer for the test
            var tpl = new Template(templateTexts[1]);
            var writer = new WebWriter(tpl);
            var writerCopyMain = writer.Copy(true);
            var writerCopy = writer.Copy();

            // verify the writer copies use the same template as the source writer
            writerCopyMain.TemplateId.Should().Be(writer.TemplateId);
            writerCopy.TemplateId.Should().Be(writer.TemplateId);

            // verify the writer copies are different writers than the source writer
            writerCopyMain.WriterId.Should().NotBe(writer.WriterId);
            writerCopy.WriterId.Should().NotBe(writer.WriterId);

            // TODO: figure out how to test if writer is main or not
        }

        [Fact]
        public void TestDocumentation_OptionsExample()
        {
            var templateText = new List<string>
            {
                "<select>",
                "  <!-- @@OPTIONS_DEMO@@ --><option value=\"@@VALUE@@\" @@SELECTED@@>@@TEXT@@</option><!-- @@OPTIONS_DEMO@@ -->",
                "</select>"
            }.Concat("\r\n");

            var template = new Template(templateText);
            var writer = new WebWriter(template);

            // create a collection of options
            var data = new List<Option>
            {
                new Option { Text = "Small", Value = "S" },
                new Option { Text = "Medium", Value = "M" },
                new Option { Text = "Large", Value = "L" }
            };

            // writer selects the OPTIONS_DEMO section, binds data and appends the section once for each item in the data collection, then deselects the section
            // the optional third argument specifies the value of the option that should be initially selected
            writer.SetOptionFields("OPTIONS_DEMO", data, "M");

            writer.AppendAll();
            var actuals = writer.GetContent();

            var expected = new List<string>
            {
                "<select>\r\n",
                "  <option value=\"S\" >Small</option>",
                "<option value=\"M\" ",
                HttpUtility.HtmlEncode("selected='selected'"),
                ">Medium</option>",
                "<option value=\"L\" >Large</option>\r\n",
                "</select>"
            }.Concat();

            actuals.Should().Be(expected);
        }

        [Fact]
        public void TestGetContent_NotFromMain()
        {
            // create a template and writer for the test
            var tpl = new Template(templateTexts[4]);
            var writer = new WebWriter(tpl);

            // set fields in main
            writer.SetField("Main1", "Text1");
            writer.SetField("Main2", "Text2");

            // select section 3 and append all sections
            writer.SelectSection("SECTION3");
            writer.AppendAll();

            // return to section 3
            writer.SelectSection("SECTION3");

            // get content without returning to main
            var actual = writer.GetContent();

            var expected = string.Concat("Main1: Text1\r\n",
                "  section 3 text\r\n",
                "Main2: Text2");

            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void TestSetMultiSectionFields_WithNameDefinitions()
        {
            // setup a template and writer
            var tpl = new Template(templateTexts[2]);
            var writer = new WebWriter(tpl);

            // setup data
            var data = new List<MyData>
            {
                new MyData("1", "Val4", "value 3-1", "value 4-1", "true", "false"),
                new MyData("2", "Val3", "value 3-2", "value 4-2", "0", "1"),
                new MyData("3", "Val2", "value 3-3", "value 4-3", "T", "F"),
                new MyData("4", "Val1", "value 3-4", "value 4-4", "f", "t"),
            };

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

            // setup field definitions
            var definitions = new FieldDefinitions(new List<string> { "Field5", "Field6" }, dd);

            // set a section for each row of data
            writer.SetMultiSectionFields("SECTION3", data, definitions);
            writer.AppendAll();
            var content = writer.GetContent();

            var expected = new List<string>
            {
                "  section 3 text\r\n",
                "  Field3: value 3-1\r\n",
                "  Field4: value 4-1\r\n",
                "  Text 1;1;selected='selected'Text 2;2;Text 3;3;Text 4;4;\r\n",
                "  Name 1;Val1;Name 2;Val2;Name 3;Val3;Name 4;Val4;selected='selected'\r\n",
                "  Checkbox1: checked='checked'\r\n",
                "  Checkbox2: \r\n",
                "  section 3 text\r\n",
                "  Field3: value 3-2\r\n",
                "  Field4: value 4-2\r\n",
                "  Text 1;1;Text 2;2;selected='selected'Text 3;3;Text 4;4;\r\n",
                "  Name 1;Val1;Name 2;Val2;Name 3;Val3;selected='selected'Name 4;Val4;\r\n",
                "  Checkbox1: \r\n",
                "  Checkbox2: checked='checked'\r\n",
                "  section 3 text\r\n",
                "  Field3: value 3-3\r\n",
                "  Field4: value 4-3\r\n",
                "  Text 1;1;Text 2;2;Text 3;3;selected='selected'Text 4;4;\r\n",
                "  Name 1;Val1;Name 2;Val2;selected='selected'Name 3;Val3;Name 4;Val4;\r\n",
                "  Checkbox1: checked='checked'\r\n",
                "  Checkbox2: \r\n",
                "  section 3 text\r\n",
                "  Field3: value 3-4\r\n",
                "  Field4: value 4-4\r\n",
                "  Text 1;1;Text 2;2;Text 3;3;Text 4;4;selected='selected'\r\n",
                "  Name 1;Val1;selected='selected'Name 2;Val2;Name 3;Val3;Name 4;Val4;\r\n",
                "  Checkbox1: \r\n",
                "  Checkbox2: checked='checked'\r\n"
            }.Concat();

            content.Should().Be(HttpUtility.HtmlEncode(expected));
        }

        [Fact]
        public void TestLiteralTextBlock_Main()
        {
            // create a template and main writer for a document containing a literal block
            var tpl = new Template(templateTexts[10]);
            var writer = new WebWriter(tpl);
            var actual = writer.GetContent(true);

            var expected = HttpUtility.HtmlEncode(templateTexts[10].Replace("<!-- **LITERAL1** -->", "").TrimStart());
            actual.Should().Be(expected);
        }

        [Fact]
        public void TestLiteralTextBlock_Nested()
        {
            // create a template and main writer for a document containing a literal block
            var tpl = new Template(templateTexts[11]);
            var writer = new WebWriter(tpl);
            writer.SetField("Field1", "Value1");
            writer.SetField("Field2", "Value2");
            writer.SetField("Field3", "Value3");
            writer.SetField("Field4", "Value4");
            var actual = writer.GetContent(true);

            var sb = new StringBuilder(templateTexts[11]);
            sb.Replace("<!-- **LITERAL1** -->\r\n", "");
            sb.Replace("<!-- **LITERAL1** -->", "");
            sb.Replace("@@Field1@@", "Value1");
            sb.Replace("@@Field4@@", "Value4");

            var expected = HttpUtility.HtmlEncode(sb.ToString());
            actual.Should().Be(expected);
        }

        [Fact]
        public void TestSetMultiSectionFields_WithDefinitions()
        {
            // setup a template and writer
            var tpl = new Template(templateTexts[2]);
            var writer = new WebWriter(tpl);

            // setup data
            var data = new List<MyData>
            {
                new MyData("1", "Val4", "value 3-1", "value 4-1", "true", "false"),
                new MyData("2", "Val3", "value 3-2", "value 4-2", "0", "1"),
                new MyData("3", "Val2", "value 3-3", "value 4-3", "T", "F"),
                new MyData("4", "Val1", "value 3-4", "value 4-4", "f", "t"),
            };

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

            // setup field definitions
            var definitions = new FieldDefinitions(new List<string> { "Field5", "Field6" }, dd);

            // set a section for each row of data
            writer.SelectSection("SECTION3");
            writer.SetMultiSectionFields(data, definitions);
            writer.DeselectSection();
            writer.AppendAll();
            var content = writer.GetContent();

            var expected = new List<string>
            {
                "  section 3 text\r\n",
                "  Field3: value 3-1\r\n",
                "  Field4: value 4-1\r\n",
                "  Text 1;1;selected='selected'Text 2;2;Text 3;3;Text 4;4;\r\n",
                "  Name 1;Val1;Name 2;Val2;Name 3;Val3;Name 4;Val4;selected='selected'\r\n",
                "  Checkbox1: checked='checked'\r\n",
                "  Checkbox2: \r\n",
                "  section 3 text\r\n",
                "  Field3: value 3-2\r\n",
                "  Field4: value 4-2\r\n",
                "  Text 1;1;Text 2;2;selected='selected'Text 3;3;Text 4;4;\r\n",
                "  Name 1;Val1;Name 2;Val2;Name 3;Val3;selected='selected'Name 4;Val4;\r\n",
                "  Checkbox1: \r\n",
                "  Checkbox2: checked='checked'\r\n",
                "  section 3 text\r\n",
                "  Field3: value 3-3\r\n",
                "  Field4: value 4-3\r\n",
                "  Text 1;1;Text 2;2;Text 3;3;selected='selected'Text 4;4;\r\n",
                "  Name 1;Val1;Name 2;Val2;selected='selected'Name 3;Val3;Name 4;Val4;\r\n",
                "  Checkbox1: checked='checked'\r\n",
                "  Checkbox2: \r\n",
                "  section 3 text\r\n",
                "  Field3: value 3-4\r\n",
                "  Field4: value 4-4\r\n",
                "  Text 1;1;Text 2;2;Text 3;3;Text 4;4;selected='selected'\r\n",
                "  Name 1;Val1;selected='selected'Name 2;Val2;Name 3;Val3;Name 4;Val4;\r\n",
                "  Checkbox1: \r\n",
                "  Checkbox2: checked='checked'\r\n"
            }.Concat();

            content.Should().Be(HttpUtility.HtmlEncode(expected));
        }

        [Fact]
        public void TestSetOptionFields()
        {
            // create a template and writer for the test
            var tpl = new Template(templateTexts[2]);
            var writer = new WebWriter(tpl);
            writer.SelectSection("SECTION3");

            // setup option data
            var data = new List<Option>
            {
                new Option() { Text = "Text 1", Value = "1" },
                new Option() { Text = "Text 2", Value = "2" },
                new Option() { Text = "Text 3", Value = "3" },
                new Option() { Text = "Text 4", Value = "4" }
            };

            // set option field in the template and get the content
            writer.SetOptionFields("OPTION_SECTION1", data, "3");
            writer.AppendAll();
            var content = writer.GetContent();

            var expected = new List<string>
            {
                "  section 3 text\r\n",
                "  Field3: \r\n",
                "  Field4: \r\n",
                "  Text 1;1;Text 2;2;Text 3;3;selected='selected'Text 4;4;\r\n",
                "  Checkbox1: \r\n",
                "  Checkbox2: \r\n"
            }.Concat();

            content.Should().Be(HttpUtility.HtmlEncode(expected));
        }

        [Fact]
        public void TestSetSectionFields_WithDefinitionsOptions()
        {
            // setup a template and writer
            var tpl = new Template(templateTexts[2]);
            var writer = new WebWriter(tpl);

            // setup data
            var data = new List<MyData>
            {
                new MyData("1", "Val4", "value 3-1", "value 4-1", "true", "false"),
                new MyData("2", "Val3", "value 3-2", "value 4-2", "0", "1"),
                new MyData("3", "Val2", "value 3-3", "value 4-3", "T", "F"),
                new MyData("4", "Val1", "value 3-4", "value 4-4", "f", "t"),
            };

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

            // setup field definitions
            var definitions = new FieldDefinitions(new List<string> { "Field5", "Field6" }, dd);

            var content = new List<string>();

            // set a section for the row of data using AppendDeselect option
            writer.SelectSection("SECTION3");
            writer.SetSectionFields<MyData>(data[0], SectionOptions.AppendDeselect, definitions);
            writer.AppendAll();
            content.Add(writer.GetContent());
            writer.Clear();

            // set a section for the row of data using Append option
            writer.SelectSection("SECTION3");
            writer.SetSectionFields<MyData>(data[1], SectionOptions.AppendOnly, definitions);
            writer.DeselectSection();
            writer.AppendAll();
            content.Add(writer.GetContent());
            writer.Clear();

            // set a section for the row of data using Set option
            writer.SelectSection("SECTION3");
            writer.SetSectionFields<MyData>(data[2], SectionOptions.Set, definitions);
            writer.AppendSection();
            writer.DeselectSection();
            writer.AppendAll();
            content.Add(writer.GetContent());

            var expected = new List<string>
            {
                HttpUtility.HtmlEncode(new List<string>
                {
                    "  section 3 text\r\n",
                    "  Field3: value 3-1\r\n",
                    "  Field4: value 4-1\r\n",
                    "  Text 1;1;selected='selected'Text 2;2;Text 3;3;Text 4;4;\r\n",
                    "  Name 1;Val1;Name 2;Val2;Name 3;Val3;Name 4;Val4;selected='selected'\r\n",
                    "  Checkbox1: checked='checked'\r\n",
                    "  Checkbox2: \r\n"
                }.Concat()),
                HttpUtility.HtmlEncode(new List<string>
                {
                    "  section 3 text\r\n",
                    "  Field3: value 3-2\r\n",
                    "  Field4: value 4-2\r\n",
                    "  Text 1;1;Text 2;2;selected='selected'Text 3;3;Text 4;4;\r\n",
                    "  Name 1;Val1;Name 2;Val2;Name 3;Val3;selected='selected'Name 4;Val4;\r\n",
                    "  Checkbox1: \r\n",
                    "  Checkbox2: checked='checked'\r\n"
                }.Concat()),
                HttpUtility.HtmlEncode(new List<string>
                {
                    "  section 3 text\r\n",
                    "  Field3: value 3-3\r\n",
                    "  Field4: value 4-3\r\n",
                    "  Text 1;1;Text 2;2;Text 3;3;selected='selected'Text 4;4;\r\n",
                    "  Name 1;Val1;Name 2;Val2;selected='selected'Name 3;Val3;Name 4;Val4;\r\n",
                    "  Checkbox1: checked='checked'\r\n",
                    "  Checkbox2: \r\n"
                }.Concat())
            };

            content.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void TestSetSectionFields_WithNameDefinitions()
        {
            // setup a template and writer
            var tpl = new Template(templateTexts[2]);
            var writer = new WebWriter(tpl);

            // setup data
            var data = new MyData("1", "Val4", "value 3-1", "value 4-1", "true", "false");

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

            // setup field definitions
            var definitions = new FieldDefinitions(new List<string> { "Field5", "Field6" }, dd);

            // set a section for the row of data
            writer.SetSectionFields<MyData>("SECTION3", data, definitions);
            writer.AppendAll();
            var content = writer.GetContent();

            var expected = new List<string>
            {
                "  section 3 text\r\n",
                "  Field3: value 3-1\r\n",
                "  Field4: value 4-1\r\n",
                "  Text 1;1;selected='selected'Text 2;2;Text 3;3;Text 4;4;\r\n",
                "  Name 1;Val1;Name 2;Val2;Name 3;Val3;Name 4;Val4;selected='selected'\r\n",
                "  Checkbox1: checked='checked'\r\n",
                "  Checkbox2: \r\n"
            }.Concat();

            content.Should().Be(HttpUtility.HtmlEncode(expected));
        }

        [Fact]
        public void TestSetSectionFields_WithNameDefinitionsOptions()
        {
            // setup a template and writer
            var tpl = new Template(templateTexts[2]);
            var writer = new WebWriter(tpl);

            // setup data
            var data = new List<MyData>
            {
                new MyData("1", "Val4", "value 3-1", "value 4-1", "true", "false"),
                new MyData("2", "Val3", "value 3-2", "value 4-2", "0", "1"),
                new MyData("3", "Val2", "value 3-3", "value 4-3", "T", "F"),
                new MyData("4", "Val1", "value 3-4", "value 4-4", "f", "t"),
            };

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

            // setup field definitions
            var definitions = new FieldDefinitions(new List<string> { "Field5", "Field6" }, dd);

            var content = new List<string>();

            // set a section for the row of data using AppendDeselect option
            writer.SetSectionFields<MyData>("SECTION3", data[0], SectionOptions.AppendDeselect, definitions);
            writer.AppendAll();
            content.Add(writer.GetContent());
            writer.Clear();

            // set a section for the row of data using Append option
            writer.SetSectionFields<MyData>("SECTION3", data[1], SectionOptions.AppendOnly, definitions);
            writer.DeselectSection();
            writer.AppendAll();
            content.Add(writer.GetContent());
            writer.Clear();

            // set a section for the row of data using Set option
            writer.SetSectionFields<MyData>("SECTION3", data[2], SectionOptions.Set, definitions);
            writer.AppendSection();
            writer.DeselectSection();
            writer.AppendAll();
            content.Add(writer.GetContent());

            var expected = new List<string>
            {
                HttpUtility.HtmlEncode(new List<string>
                {
                    "  section 3 text\r\n",
                    "  Field3: value 3-1\r\n",
                    "  Field4: value 4-1\r\n",
                    "  Text 1;1;selected='selected'Text 2;2;Text 3;3;Text 4;4;\r\n",
                    "  Name 1;Val1;Name 2;Val2;Name 3;Val3;Name 4;Val4;selected='selected'\r\n",
                    "  Checkbox1: checked='checked'\r\n",
                    "  Checkbox2: \r\n"
                }.Concat()),
                HttpUtility.HtmlEncode(new List<string>
                {
                    "  section 3 text\r\n",
                    "  Field3: value 3-2\r\n",
                    "  Field4: value 4-2\r\n",
                    "  Text 1;1;Text 2;2;selected='selected'Text 3;3;Text 4;4;\r\n",
                    "  Name 1;Val1;Name 2;Val2;Name 3;Val3;selected='selected'Name 4;Val4;\r\n",
                    "  Checkbox1: \r\n",
                    "  Checkbox2: checked='checked'\r\n"
                }.Concat()),
                HttpUtility.HtmlEncode(new List<string>
                {
                    "  section 3 text\r\n",
                    "  Field3: value 3-3\r\n",
                    "  Field4: value 4-3\r\n",
                    "  Text 1;1;Text 2;2;Text 3;3;selected='selected'Text 4;4;\r\n",
                    "  Name 1;Val1;Name 2;Val2;selected='selected'Name 3;Val3;Name 4;Val4;\r\n",
                    "  Checkbox1: checked='checked'\r\n",
                    "  Checkbox2: \r\n"
                }.Concat())
            };

            content.Should().BeEquivalentTo(expected);
        }

    }

}
