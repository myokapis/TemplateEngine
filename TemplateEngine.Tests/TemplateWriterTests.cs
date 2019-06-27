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
using System.IO;
using FluentAssertions;
using Xunit;

namespace TemplateEngine.Tests
{

    public class TemplateWriterTests
    {

        [Fact]
        public void TestAppendAll()
        {
            // create a template and writer for the test
            var tpl = new Template(this.templateTexts[4]);
            var writer = new TemplateWriter(tpl);

            // set field data in an inner section
            writer.SelectSection("SECTION1");
            writer.SelectSection("SECTION2");
            writer.SetField("Field1", "S2F1");
            writer.SetField("Field2", "S2F2");

            // append all sections out to section 1
            writer.AppendAll("SECTION1");

            // set field data in main section
            writer.SetField("Main1", "Data1");
            writer.SetField("Main2", "Data2");
            writer.AppendSection();

            var expected = string.Concat(
                "Main1: Data1\r\n",
                "  some text\r\n",
                "  Field1: S2F1\r\n",
                "  Field2: S2F2\r\n",
                "  post text section 2\r\n",
                "Main2: Data2");

            var actual = writer.GetContent();
            actual.Should().Be(expected);
        }

        [Fact]
        public void TestAppendAll_Main()
        {
            // create a template and writer for the test
            var tpl = new Template(this.templateTexts[4]);
            var writer = new TemplateWriter(tpl);

            // set field data in main section
            writer.SetField("Main1", "Data1");
            writer.SetField("Main2", "Data2");

            // set field data in an inner section
            writer.SelectSection("SECTION1");
            writer.SelectSection("SECTION2");
            writer.SetField("Field1", "S2F1");
            writer.SetField("Field2", "S2F2");

            // append all sections out to the end
            writer.AppendAll();

            var expected = string.Concat(
                "Main1: Data1\r\n",
                "  some text\r\n",
                "  Field1: S2F1\r\n",
                "  Field2: S2F2\r\n",
                "  post text section 2\r\n",
                "Main2: Data2");

            var actual = writer.GetContent();
            actual.Should().Be(expected);
        }

        [Fact]
        public void TestAppendSection()
        {
            // create a template and writer for the test
            var tpl = new Template(this.templateTexts[1]);
            var writer = new TemplateWriter(tpl);

            // append sections using all three parameter options
            writer.SelectSection("SECTION1");
            writer.SelectSection("SECTION2");
            writer.AppendSection(false);
            writer.AppendSection(true);
            writer.AppendSection();
            writer.DeselectSection();
            writer.AppendSection();

            var expected = string.Concat("  some text\r\n",
                "  Field1: \r\n",
                "  Field2: \r\n",
                "  Field1: \r\n",
                "  Field2: \r\n",
                "  post text section 2\r\n");

            var actual = writer.GetContent();

            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void TestClear()
        {
            // create a template and writer for the test
            var tpl = new Template(this.templateTexts[1]);
            var parentWriter = new TemplateWriter(tpl);

            // set fields, clear, then get the content for the template
            var childWriter = parentWriter.GetWriter("SECTION2");
            childWriter.SetField("Field1", "Value1");
            childWriter.SetField("Field2", 33);
            childWriter.Clear();
            childWriter.SetField("Field1", "Value2");
            childWriter.SetField("Field2", 44);
            childWriter.AppendSection();
            var actual = childWriter.GetContent();

            actual.Should().Equals("  Field1: Value2\r\n  Field2: 44\r\n");
        }

        [Fact]
        public void TestDeselectMainSection()
        {
            // create a template and writer for the test
            var tpl = new Template(this.templateTexts[1]);
            var parentWriter = new TemplateWriter(tpl);

            // select and append a section
            parentWriter.SelectSection("SECTION3");
            parentWriter.AppendSection(true);

            // deselect one too many times
            Action action = () => parentWriter.DeselectSection();

            action.Should().Throw<InvalidOperationException>()
                .WithMessage("Cannot deselect the parent section");
        }

        [Fact]
        public void TestDeselectSection()
        {
            // create a template and writer for the test
            var tpl = new Template(this.templateTexts[1]);
            var parentWriter = new TemplateWriter(tpl);

            // select and deselect a section
            parentWriter.SelectSection("SECTION3");
            parentWriter.SelectedSectionName.Should().Be("SECTION3");
            parentWriter.DeselectSection();
            parentWriter.SelectedSectionName.Should().Be("@MAIN");
        }

        [Fact]
        public void TestGetContent()
        {
            // create a template and writer for the test
            var tpl = new Template(this.templateTexts[1]);
            var writer = new TemplateWriter(tpl);

            // append section 1 containing two instances of section 2
            writer.SelectSection("SECTION1");
            writer.SelectSection("SECTION2");
            writer.SetField("Field1", "Value1-1");
            writer.SetField("Field2", "Value2-1");
            writer.AppendSection();
            writer.SetField("Field1", "Value1-2");
            writer.SetField("Field2", "Value2-2");
            writer.AppendSection(true);
            writer.AppendSection(true);

            // append section 1 containing an instance of section 2
            writer.SelectSection("SECTION1");
            writer.SelectSection("SECTION2");
            writer.SetField("Field1", "Value1-3");
            writer.SetField("Field2", "Value2-3");
            writer.AppendSection(true);
            writer.AppendAll();

            var expected = string.Concat("  some text\r\n",
                "  Field1: Value1-1\r\n",
                "  Field2: Value2-1\r\n",
                "  Field1: Value1-2\r\n",
                "  Field2: Value2-2\r\n",
                "  post text section 2\r\n",
                "  some text\r\n",
                "  Field1: Value1-3\r\n",
                "  Field2: Value2-3\r\n",
                "  post text section 2\r\n");

            var actual = writer.GetContent();

            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void TestGetWriter()
        {
            // create a template and writer for the test
            var tpl = new Template(this.templateTexts[0]);
            var wrt = new TemplateWriter(tpl);

            // call the method under text and get the content for the template
            var wrt2 = wrt.GetWriter("SECTION2");
            wrt2.AppendSection();
            var actual = wrt2.GetContent();

            actual.Should().BeEquivalentTo("  section 2 text\r\n");
        }

        [Fact]
        public void TestRegisterFieldProvider_Child()
        {
            // create a template and main writer for the test
            var tpl1 = new Template(this.templateTexts[1]);
            var writer = new TemplateWriter(tpl1);

            // create another template and writer to serve as a provider
            var tpl2 = new Template(this.templateTexts[3]);
            var provider = new TemplateWriter(tpl2);

            // register the provider for a field in a child section of the main writer
            writer.RegisterFieldProvider("SECTION2", "Field2", provider);

            // get to the section containing the provider
            writer.SelectSection("SECTION1");
            writer.SelectSection("SECTION2");

            // select the provider and then select a child section within the provider
            writer.SelectProvider("Field2");
            writer.SelectSection("SECTION1");

            // set field data in the provider section
            writer.SetField("Field1", "Field1-1");
            writer.SetField("Field2", "Field2-1");
            writer.AppendSection();
            writer.SetField("Field2", "Field2-2");
            writer.SetField("Field6", "Field6-2");

            // append all the way out of the provider and the writer
            writer.AppendAll();

            var expected = string.Concat("  some text\r\n",
                "  Field1: \r\n",
                "  Field2: ",
                "-->Field1-1<--\r\n",
                "-->Field2-1<--\r\n",
                "--><--\r\n",
                "--><--\r\n",
                "--><--\r\n",
                "--><--\r\n",
                "--><--\r\n",
                "-->Field2-2<--\r\n",
                "--><--\r\n",
                "--><--\r\n",
                "--><--\r\n",
                "-->Field6-2<--\r\n",
                "\r\n",
                "  post text section 2\r\n");

            var actual = writer.GetContent();

            actual.Equals(expected);
        }

        [Fact]
        public void TestRegisterFieldProvider_Main()
        {
            // create a template and main writer for the test
            var tpl1 = new Template(this.templateTexts[4]);
            var writer = new TemplateWriter(tpl1);

            // create another template and writer to serve as a provider
            var tpl2 = new Template(this.templateTexts[3]);
            var provider = new TemplateWriter(tpl2);

            // register the provider for a field in the main section of the main writer
            writer.RegisterFieldProvider("Main2", provider);

            // select the provider and then select a child section within the provider
            writer.SelectProvider("Main2");
            writer.SelectSection("SECTION1");

            // set field data in the provider section
            writer.SetField("Field1", "Field1-1");
            writer.SetField("Field2", "Field2-1");
            writer.AppendSection();
            writer.SetField("Field2", "Field2-2");
            writer.SetField("Field6", "Field6-2");

            // append all the way out of the provider and the writer
            writer.AppendAll();

            var expected = new List<string>
            {
                "Main1: \r\n",
                "Main2: ",
                "-->Field1-1<--\r\n",
                "-->Field2-1<--\r\n",
                "--><--\r\n",
                "--><--\r\n",
                "--><--\r\n",
                "--><--\r\n",
                "--><--\r\n",
                "-->Field2-2<--\r\n",
                "--><--\r\n",
                "--><--\r\n",
                "--><--\r\n",
                "-->Field6-2<--\r\n"
            }.Concat();

            var actual = writer.GetContent();

            actual.Equals(expected);
        }

        [Fact]
        public void TestRepeatedNestedSections()
        {
            TemplateWriter.EnableTrace = true;

            // setup a template and writer
            var tpl = new Template(templateTexts[5]);
            var writer = new TemplateWriter(tpl);

            // populate field in main and get first nested section
            writer.SetField("Main1", "Value1");
            writer.SelectSection("SECTION1");

            // set up parameters for creating nested sections
            var i = 1;
            var sectionCount1 = 1;
            var sectionCount2 = 2;
            var sectionCount3 = 2;

            // populate repeated nested sections
            for (var counter1 = 1; counter1 <= sectionCount1; counter1++)
            {
                writer.SetField("Field1_1", i++);
                writer.SelectSection("SECTION2");

                for (var counter2 = 1; counter2 <= sectionCount2; counter2++)
                {
                    writer.SetField("Field2_1", i++);
                    writer.SelectSection("SECTION3");

                    for(var counter3 = 1; counter3 <= sectionCount3; counter3++)
                    {
                        writer.SetField("Field3_1", i++);
                        writer.AppendSection(counter3 == sectionCount3);
                    }

                    writer.SetField("Field2_2", i++);
                    writer.AppendSection(counter2 == sectionCount2);
                }
                
                writer.SetField("Field1_2", i++);
                writer.AppendSection(counter1 == sectionCount1);
            }


            // set another field in main section
            writer.SetField("Main2", "Value2");
            writer.AppendAll();
            var actual = writer.GetContent();

            using (var file = new StreamWriter(@"C:\Temp\trace.txt"))
            {
                TemplateWriter.TraceResults.ForEach(r => file.WriteLine(r));
            }
                
            var expected = new List<string>
            {
                "Main1: Value1\r\n",
                "  Field1.1: 1\r\n",

                "  Field2.1: 2\r\n",
                "  section 3 text\r\n",
                " Field3.1: 3\r\n",
                "  section 3 text\r\n",
                " Field3.1: 4\r\n",
                "  Field2.2: 5\r\n",
                "  post text section 2\r\n",

                "  Field2.1: 6\r\n",
                "  section 3 text\r\n",
                " Field3.1: 7\r\n",
                "  section 3 text\r\n",
                " Field3.1: 8\r\n",
                "  Field2.2: 9\r\n",
                "  post text section 2\r\n",

                "Main2: Value2"
            }.Concat();

            actual.Should().Be(expected);
        }

        [Fact]
        public void TestReset()
        {
            // create a template and writer for the test
            var tpl = new Template(this.templateTexts[1]);
            var parentWriter = new TemplateWriter(tpl);

            // set fields, append, reset, set fields, append, and check content
            var childWriter = parentWriter.GetWriter("SECTION2");
            childWriter.SetField("Field1", "Value1");
            childWriter.SetField("Field2", 33);
            childWriter.AppendSection();
            childWriter.SetField("Field1", "Value2");
            childWriter.SetField("Field2", 44);
            childWriter.AppendSection();
            childWriter.Reset();
            childWriter.SetField("Field1", "Value3");
            childWriter.SetField("Field2", 55);
            childWriter.AppendSection();
            var actual = childWriter.GetContent();

            actual.Should().BeEquivalentTo("  Field1: Value3\r\n  Field2: 55\r\n");
        }

        [Fact]
        public void TestSelectDeselect()
        {
            var template = new Template(templateTexts[1]);
            var writer = new TemplateWriter(template);

            writer.SelectSection("SECTION1");
            writer.SelectSection("SECTION2");
            writer.DeselectSection();
            writer.SelectSection("SECTION2");

            writer.SetField("Field1", "Field1Value");
            writer.SetField("Field2", "Field2Value");
            writer.AppendSection();
            writer.DeselectSection();
            writer.AppendSection();
            writer.DeselectSection();
            writer.AppendSection();

            var expected = string.Concat(
                "  some text\r\n",
                "  Field1: Field1Value\r\n",
                "  Field2: Field2Value\r\n",
                "  post text section 2\r\n");

            var actual = writer.GetContent();

            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void TestSelectSection()
        {
            // create a template and writer for the test
            var tpl = new Template(this.templateTexts[1]);
            var parentWriter = new TemplateWriter(tpl);

            // select a section and then a subsection
            parentWriter.SelectSection("SECTION1");
            parentWriter.SelectedSectionName.Should().Be("SECTION1");
            parentWriter.SelectSection("SECTION2");
            parentWriter.SelectedSectionName.Should().Be("SECTION2");
        }

        [Fact]
        public void TestSelectedSectionName()
        {
            // create a template and writer for the test
            var tpl = new Template(this.templateTexts[1]);
            var parentWriter = new TemplateWriter(tpl);

            parentWriter.SelectedSectionName.Should().Be("@MAIN");
            parentWriter.SelectSection("SECTION3");
            parentWriter.SelectedSectionName.Should().Be("SECTION3");
        }

        [Fact]
        public void TestSetFieldDateTime()
        {
            // create a template and writer for the test
            var tpl = new Template(this.templateTexts[3]);
            var parentWriter = new TemplateWriter(tpl);
            parentWriter.SelectSection("SECTION1");

            var dt = DateTime.Now;

            // append a date field and get the content
            parentWriter.SetField("Field2", dt);
            parentWriter.AppendSection(true);
            parentWriter.AppendSection();
            var content = parentWriter.GetContent();

            var expected = string.Concat(
                "--><--\r\n",
                $"-->{dt.ToString()}<--\r\n",
                "--><--\r\n",
                "--><--\r\n",
                "--><--\r\n",
                "--><--\r\n");

            content.Should().Be(expected);
        }

        [Fact]
        public void TestSetFieldDecimal()
        {
            // create a template and writer for the test
            var tpl = new Template(this.templateTexts[3]);
            var parentWriter = new TemplateWriter(tpl);
            parentWriter.SelectSection("SECTION1");

            var dec = 890.123M;

            // append a date field and get the content
            parentWriter.SetField("Field3", dec);
            parentWriter.AppendSection(true);
            parentWriter.AppendSection();
            var content = parentWriter.GetContent();

            var expected = string.Concat(
                "--><--\r\n",
                "--><--\r\n",
                "-->890.123<--\r\n",
                "--><--\r\n",
                "--><--\r\n",
                "--><--\r\n");

            content.Should().Be(expected);
        }

        [Fact]
        public void TestSetFieldDouble()
        {
            // create a template and writer for the test
            var tpl = new Template(this.templateTexts[3]);
            var parentWriter = new TemplateWriter(tpl);
            parentWriter.SelectSection("SECTION1");

            var dbl = 123.456;

            // append a date field and get the content
            parentWriter.SetField("Field4", dbl);
            parentWriter.AppendSection(true);
            parentWriter.AppendSection();
            var content = parentWriter.GetContent();

            var expected = string.Concat(
                "--><--\r\n",
                "--><--\r\n",
                "--><--\r\n",
                "-->123.456<--\r\n",
                "--><--\r\n",
                "--><--\r\n");

            content.Should().Be(expected);
        }

        [Fact]
        public void TestSetFieldInt()
        {
            // create a template and writer for the test
            var tpl = new Template(this.templateTexts[3]);
            var parentWriter = new TemplateWriter(tpl);
            parentWriter.SelectSection("SECTION1");

            var val = 76543;

            // append a date field and get the content
            parentWriter.SetField("Field5", val);
            parentWriter.AppendSection(true);
            parentWriter.AppendSection();
            var content = parentWriter.GetContent();

            var expected = string.Concat(
                "--><--\r\n",
                "--><--\r\n",
                "--><--\r\n",
                "--><--\r\n",
                "-->76543<--\r\n",
                "--><--\r\n");

            content.Should().Be(expected);
        }

        [Fact]
        public void TestSetFieldString()
        {
            // create a template and writer for the test
            var tpl = new Template(this.templateTexts[3]);
            var parentWriter = new TemplateWriter(tpl);
            parentWriter.SelectSection("SECTION1");

            var val = "This is the real deal";

            // append a date field and get the content
            parentWriter.SetField("Field6", val);
            parentWriter.AppendSection(true);
            parentWriter.AppendSection();
            var content = parentWriter.GetContent();

            var expected = string.Concat(
                "--><--\r\n",
                "--><--\r\n",
                "--><--\r\n",
                "--><--\r\n",
                "--><--\r\n",
                "-->This is the real deal<--\r\n");

            content.Should().Be(expected);
        }

        [Fact]
        public void TestSetFieldOfT()
        {
            // create a template and writer for the test
            var tpl = new Template(this.templateTexts[3]);
            var writer = new TemplateWriter(tpl);
            writer.SelectSection("SECTION1");

            var t = new List<string>() { "A", "B", "C" };

            // append a date field and get the content
            writer.SetField("Field1", t);
            writer.AppendSection(true);
            writer.AppendSection();
            var content = writer.GetContent();

            var expected = string.Concat(
                $"-->{t.ToString()}<--\r\n",
                "--><--\r\n",
                "--><--\r\n",
                "--><--\r\n",
                "--><--\r\n",
                "--><--\r\n");

            content.Should().Be(expected);
        }

        [Fact]
        public void TestSetOptionFields()
        {
            // create a template and writer for the test
            var tpl = new Template(this.templateTexts[2]);
            var writer = new TemplateWriter(tpl);
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

            content.Should().Be(expected);
        }

        [Fact]
        public void TestSetMultiSectionFields_WithName()
        {
            // setup a template and writer
            var tpl = new Template(templateTexts[3]);
            var writer = new TemplateWriter(tpl);

            // setup data
            var data = new List<Data>
            {
                new Data("value 1-1", "value 2-1", "value 3-1", "value 4-1", "value 5-1", "value 6-1"),
                new Data("value 1-2", "value 2-2", "value 3-2", "value 4-2", "value 5-2", "value 6-2"),
                new Data("value 1-3", "value 2-3", "value 3-3", "value 4-3", "value 5-3", "value 6-3"),
                new Data("value 1-4", "value 2-4", "value 3-4", "value 4-4", "value 5-4", "value 6-4"),
            };

            // set a section for each row of data
            writer.SetMultiSectionFields<Data>("SECTION1", data);
            writer.AppendAll();
            var content = writer.GetContent();

            var expected = new List<string>
            {
                "-->value 1-1<--\r\n",
                "-->value 2-1<--\r\n",
                "-->value 3-1<--\r\n",
                "-->value 4-1<--\r\n",
                "-->value 5-1<--\r\n",
                "-->value 6-1<--\r\n",
                "-->value 1-2<--\r\n",
                "-->value 2-2<--\r\n",
                "-->value 3-2<--\r\n",
                "-->value 4-2<--\r\n",
                "-->value 5-2<--\r\n",
                "-->value 6-2<--\r\n",
                "-->value 1-3<--\r\n",
                "-->value 2-3<--\r\n",
                "-->value 3-3<--\r\n",
                "-->value 4-3<--\r\n",
                "-->value 5-3<--\r\n",
                "-->value 6-3<--\r\n",
                "-->value 1-4<--\r\n",
                "-->value 2-4<--\r\n",
                "-->value 3-4<--\r\n",
                "-->value 4-4<--\r\n",
                "-->value 5-4<--\r\n",
                "-->value 6-4<--\r\n"
            }.Concat();

            content.Should().Be(expected);
        }

        [Fact]
        public void TestSetMultiSectionFields_WithNameDefinitions()
        {
            // setup a template and writer
            var tpl = new Template(templateTexts[2]);
            var writer = new TemplateWriter(tpl);

            // setup data
            var data = new List<Data>
            {
                new Data("1", "Val4", "value 3-1", "value 4-1", "true", "false"),
                new Data("2", "Val3", "value 3-2", "value 4-2", "0", "1"),
                new Data("3", "Val2", "value 3-3", "value 4-3", "T", "F"),
                new Data("4", "Val1", "value 3-4", "value 4-4", "f", "t"),
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
            writer.SetMultiSectionFields<Data>("SECTION3", data, definitions);
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

            content.Should().Be(expected);
        }

        [Fact]
        public void TestSetMultiSectionFields_WithDefinitions()
        {
            // setup a template and writer
            var tpl = new Template(templateTexts[2]);
            var writer = new TemplateWriter(tpl);

            // setup data
            var data = new List<Data>
            {
                new Data("1", "Val4", "value 3-1", "value 4-1", "true", "false"),
                new Data("2", "Val3", "value 3-2", "value 4-2", "0", "1"),
                new Data("3", "Val2", "value 3-3", "value 4-3", "T", "F"),
                new Data("4", "Val1", "value 3-4", "value 4-4", "f", "t"),
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
            writer.SetMultiSectionFields<Data>(data, definitions);
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

            content.Should().Be(expected);
        }

        [Fact]
        public void TestSetSectionFields_WithName()
        {
            // setup a template and writer
            var tpl = new Template(templateTexts[3]);
            var writer = new TemplateWriter(tpl);

            // setup data
            var data = new Data("value 1-1", "value 2-1", "value 3-1", "value 4-1", "value 5-1", "value 6-1");

            // set a section for the row of data
            writer.SetSectionFields<Data>("SECTION1", data);
            writer.AppendAll();
            var content = writer.GetContent();

            var expected = new List<string>
            {
                "-->value 1-1<--\r\n",
                "-->value 2-1<--\r\n",
                "-->value 3-1<--\r\n",
                "-->value 4-1<--\r\n",
                "-->value 5-1<--\r\n",
                "-->value 6-1<--\r\n"
            }.Concat();

            content.Should().Be(expected);
        }

        [Fact]
        public void TestSetSectionFields_WithNameDefinitions()
        {
            // setup a template and writer
            var tpl = new Template(templateTexts[2]);
            var writer = new TemplateWriter(tpl);

            // setup data
            var data = new Data("1", "Val4", "value 3-1", "value 4-1", "true", "false");

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
            writer.SetSectionFields<Data>("SECTION3", data, definitions);
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

            content.Should().Be(expected);
        }

        [Fact]
        public void TestSetSectionFields_WithNameDefinitionsOptions()
        {
            // setup a template and writer
            var tpl = new Template(templateTexts[2]);
            var writer = new TemplateWriter(tpl);

            // setup data
            var data = new List<Data>
            {
                new Data("1", "Val4", "value 3-1", "value 4-1", "true", "false"),
                new Data("2", "Val3", "value 3-2", "value 4-2", "0", "1"),
                new Data("3", "Val2", "value 3-3", "value 4-3", "T", "F"),
                new Data("4", "Val1", "value 3-4", "value 4-4", "f", "t"),
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
            writer.SetSectionFields<Data>("SECTION3", data[0], SectionOptions.AppendDeselect, definitions);
            writer.AppendAll();
            content.Add(writer.GetContent());
            writer.Clear();

            // set a section for the row of data using Append option
            writer.SetSectionFields<Data>("SECTION3", data[1], SectionOptions.AppendOnly, definitions);
            writer.DeselectSection();
            writer.AppendAll();
            content.Add(writer.GetContent());
            writer.Clear();

            // set a section for the row of data using Set option
            writer.SetSectionFields<Data>("SECTION3", data[2], SectionOptions.Set, definitions);
            writer.AppendSection();
            writer.DeselectSection();
            writer.AppendAll();
            content.Add(writer.GetContent());

            var expected = new List<string>
            {
                new List<string>
                {
                    "  section 3 text\r\n",
                    "  Field3: value 3-1\r\n",
                    "  Field4: value 4-1\r\n",
                    "  Text 1;1;selected='selected'Text 2;2;Text 3;3;Text 4;4;\r\n",
                    "  Name 1;Val1;Name 2;Val2;Name 3;Val3;Name 4;Val4;selected='selected'\r\n",
                    "  Checkbox1: checked='checked'\r\n",
                    "  Checkbox2: \r\n"
                }.Concat(),
                new List<string>
                {
                    "  section 3 text\r\n",
                    "  Field3: value 3-2\r\n",
                    "  Field4: value 4-2\r\n",
                    "  Text 1;1;Text 2;2;selected='selected'Text 3;3;Text 4;4;\r\n",
                    "  Name 1;Val1;Name 2;Val2;Name 3;Val3;selected='selected'Name 4;Val4;\r\n",
                    "  Checkbox1: \r\n",
                    "  Checkbox2: checked='checked'\r\n"
                }.Concat(),
                new List<string>
                {
                    "  section 3 text\r\n",
                    "  Field3: value 3-3\r\n",
                    "  Field4: value 4-3\r\n",
                    "  Text 1;1;Text 2;2;Text 3;3;selected='selected'Text 4;4;\r\n",
                    "  Name 1;Val1;Name 2;Val2;selected='selected'Name 3;Val3;Name 4;Val4;\r\n",
                    "  Checkbox1: checked='checked'\r\n",
                    "  Checkbox2: \r\n"
                }.Concat()
            };

            content.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void TestSetSectionFields_WithDefinitionsOptions()
        {
            // setup a template and writer
            var tpl = new Template(templateTexts[2]);
            var writer = new TemplateWriter(tpl);

            // setup data
            var data = new List<Data>
            {
                new Data("1", "Val4", "value 3-1", "value 4-1", "true", "false"),
                new Data("2", "Val3", "value 3-2", "value 4-2", "0", "1"),
                new Data("3", "Val2", "value 3-3", "value 4-3", "T", "F"),
                new Data("4", "Val1", "value 3-4", "value 4-4", "f", "t"),
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
            writer.SetSectionFields<Data>(data[0], SectionOptions.AppendDeselect, definitions);
            writer.AppendAll();
            content.Add(writer.GetContent());
            writer.Clear();

            // set a section for the row of data using Append option
            writer.SelectSection("SECTION3");
            writer.SetSectionFields<Data>(data[1], SectionOptions.AppendOnly, definitions);
            writer.DeselectSection();
            writer.AppendAll();
            content.Add(writer.GetContent());
            writer.Clear();

            // set a section for the row of data using Set option
            writer.SelectSection("SECTION3");
            writer.SetSectionFields<Data>(data[2], SectionOptions.Set, definitions);
            writer.AppendSection();
            writer.DeselectSection();
            writer.AppendAll();
            content.Add(writer.GetContent());

            var expected = new List<string>
            {
                new List<string>
                {
                    "  section 3 text\r\n",
                    "  Field3: value 3-1\r\n",
                    "  Field4: value 4-1\r\n",
                    "  Text 1;1;selected='selected'Text 2;2;Text 3;3;Text 4;4;\r\n",
                    "  Name 1;Val1;Name 2;Val2;Name 3;Val3;Name 4;Val4;selected='selected'\r\n",
                    "  Checkbox1: checked='checked'\r\n",
                    "  Checkbox2: \r\n"
                }.Concat(),
                new List<string>
                {
                    "  section 3 text\r\n",
                    "  Field3: value 3-2\r\n",
                    "  Field4: value 4-2\r\n",
                    "  Text 1;1;Text 2;2;selected='selected'Text 3;3;Text 4;4;\r\n",
                    "  Name 1;Val1;Name 2;Val2;Name 3;Val3;selected='selected'Name 4;Val4;\r\n",
                    "  Checkbox1: \r\n",
                    "  Checkbox2: checked='checked'\r\n"
                }.Concat(),
                new List<string>
                {
                    "  section 3 text\r\n",
                    "  Field3: value 3-3\r\n",
                    "  Field4: value 4-3\r\n",
                    "  Text 1;1;Text 2;2;Text 3;3;selected='selected'Text 4;4;\r\n",
                    "  Name 1;Val1;Name 2;Val2;selected='selected'Name 3;Val3;Name 4;Val4;\r\n",
                    "  Checkbox1: checked='checked'\r\n",
                    "  Checkbox2: \r\n"
                }.Concat()
            };

            content.Should().BeEquivalentTo(expected);
        }

        #region Helper Methods

        private List<string> templateTexts = new List<string>()
        {
            new List<string>
            {
                "<!-- @@SECTION1@@ -->\r\n",
                "  some text\r\n",
                "<!-- @@SECTION2@@ -->\r\n",
                "  section 2 text\r\n",
                "<!-- @@SECTION2@@ -->\r\n",
                "  post text section 2\r\n",
                "<!-- @@SECTION1@@ -->\r\n",
                "<!-- @@SECTION3@@ -->",
                "  section 3 text\r\n",
                "<!-- @@SECTION3@@ -->"
            }.Concat(),
            new List<string>
            {
                "<!-- @@SECTION1@@ -->\r\n",
                "  some text\r\n",
                "<!-- @@SECTION2@@ -->\r\n",
                "  Field1: @@Field1@@\r\n",
                "  Field2: @@Field2@@\r\n",
                "<!-- @@SECTION2@@ -->\r\n",
                "  post text section 2\r\n",
                "<!-- @@SECTION1@@ -->\r\n",
                "<!-- @@SECTION3@@ -->",
                "  section 3 text\r\n",
                "<!-- @@SECTION3@@ -->"
            }.Concat(),
            new List<string>
            {
                "<!-- @@SECTION1@@ -->\r\n",
                "  some text\r\n",
                "<!-- @@SECTION2@@ -->\r\n",
                "  Field1: @@Field1@@\r\n",
                "  Field2: @@Field2@@\r\n",
                "<!-- @@SECTION2@@ -->\r\n",
                "  post text section 2\r\n",
                "<!-- @@SECTION1@@ -->\r\n",
                "<!-- @@SECTION3@@ -->",
                "  section 3 text\r\n",
                "  Field3: @@Field3@@\r\n",
                "  Field4: @@Field4@@\r\n",
                "  <!-- @@OPTION_SECTION1@@ -->@@TEXT@@;@@VALUE@@;@@SELECTED@@<!-- @@OPTION_SECTION1@@ -->\r\n",
                "  <!-- @@OPTION_SECTION2@@ -->@@TEXT@@;@@VALUE@@;@@SELECTED@@<!-- @@OPTION_SECTION2@@ -->\r\n",
                "  Checkbox1: @@Field5@@\r\n",
                "  Checkbox2: @@Field6@@\r\n",
                "<!-- @@SECTION3@@ -->"
            }.Concat(),
            new List<string>
            {
                "<!-- @@SECTION1@@ -->\r\n",
                "-->@@Field1@@<--\r\n",
                "-->@@Field2@@<--\r\n",
                "-->@@Field3@@<--\r\n",
                "-->@@Field4@@<--\r\n",
                "-->@@Field5@@<--\r\n",
                "-->@@Field6@@<--\r\n",
                "<!-- @@SECTION1@@ -->\r\n"
            }.Concat(),
            new List<string>
            {
                "Main1: @@Main1@@\r\n",
                "<!-- @@SECTION1@@ -->\r\n",
                "  some text\r\n",
                "<!-- @@SECTION2@@ -->\r\n",
                "  Field1: @@Field1@@\r\n",
                "  Field2: @@Field2@@\r\n",
                "<!-- @@SECTION2@@ -->\r\n",
                "  post text section 2\r\n",
                "<!-- @@SECTION1@@ -->\r\n",
                "<!-- @@SECTION3@@ -->",
                "  section 3 text\r\n",
                "<!-- @@SECTION3@@ -->\r\n",
                "Main2: @@Main2@@"
            }.Concat(),
            new List<string>
            {
                "Main1: @@Main1@@\r\n",
                "<!-- @@SECTION1@@ -->\r\n",
                "  Field1.1: @@Field1_1@@\r\n",
                "<!-- @@SECTION2@@ -->\r\n",
                "  Field2.1: @@Field2_1@@\r\n",
                "<!-- @@SECTION3@@ -->",
                "  section 3 text\r\n",
                " Field3.1: @@Field3_1@@\r\n",
                "<!-- @@SECTION3@@ -->\r\n",
                "  Field2.2: @@Field2_2@@\r\n",
                "<!-- @@SECTION2@@ -->\r\n",
                "  post text section 2\r\n",
                "<!-- @@SECTION1@@ -->\r\n",
                "Main2: @@Main2@@"
            }.Concat()
        };

        #endregion

        private class Data
        {
            public Data(string field1, string field2, string field3, string field4, string field5, string field6)
            {
                this.Field1 = field1;
                this.Field2 = field2;
                this.Field3 = field3;
                this.Field4 = field4;
                this.Field5 = field5;
                this.Field6 = field6;
            }

            public string Field1 { get; set; }
            public string Field2 { get; set; }
            public string Field3 { get; set; }
            public string Field4 { get; set; }
            public string Field5 { get; set; }
            public string Field6 { get; set; }
        }

    }

}
