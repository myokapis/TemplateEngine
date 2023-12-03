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

// TODO: consider separating the writer tests from the field setter and multifield setter tests

using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using TemplateEngine.Document;
using TemplateEngine.Tests.Helpers;
using TemplateEngine.Tests.Models;
using TemplateEngine.Writer;
using Xunit;

namespace TemplateEngine.Tests
{

    public class TemplateWriterTests
    {
        private readonly List<string> templateTexts = TemplateText.Items;

        [Fact]
        public void TestAppendAll()
        {
            // create a template and writer for the test
            var tpl = new Template(templateTexts[4]);
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
            var tpl = new Template(templateTexts[4]);
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
            var tpl = new Template(templateTexts[1]);
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
            var tpl = new Template(templateTexts[1]);
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

            actual.Should().Be("  Field1: Value2\r\n  Field2: 44\r\n");
        }

        [Fact]
        public void TestContainsSection()
        {
            // create a template and writer for the test
            var tpl = new Template(templateTexts[1]);
            var writer = new TemplateWriter(tpl);

            writer.ContainsSection("SECTION1").Should().BeTrue();
            writer.ContainsSection("SECTION2").Should().BeFalse();
            writer.ContainsSection("SECTION3").Should().BeTrue();
        }

        [Fact]
        public void TestCopy()
        {
            // create a template and writer for the test
            var tpl = new Template(templateTexts[1]);
            var writer = new TemplateWriter(tpl);
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
        public void TestDocumentation_BindingDataExample1()
        {
            var template = new Template(templateTexts[9]);
            var writer = new TemplateWriter(template);

            var data = new MyModel { FirstName = "Ichabod", LastName = "Crane" };

            // the writer selects the section, binds the data, then deselects the section
            writer.SetSectionFields("ROW", data);

            writer.AppendAll();
            var actuals = writer.GetContent();

            var expected = new List<string>
            {
                "<h1>Example</h1>",
                "",
                "<table>",
                "  <thead>",
                "    <tr>",
                "      <th>First Name</th>",
                "      <th>Last Name</th>",
                "    </tr>",
                "  </thead>",
                "  <tbody>",
                "    <tr>",
                "      <td>Ichabod</td>",
                "      <td>Crane</td>",
                "    </tr>",
                "  </tbody>",
                "</table>"
            }.Concat("\r\n");

            actuals.Should().Be(expected);
        }

        [Fact]
        public void TestDocumentation_BindingDataExample2()
        {
            var template = new Template(templateTexts[9]);
            var writer = new TemplateWriter(template);

            // create a collection of data
            var data = new List<MyModel>
            {
                new MyModel { FirstName = "Ichabod", LastName = "Crane" },
                new MyModel { FirstName = "Alex", LastName = "Rodriguez" }
            };

            // the writer selects the ROW section, binds data and appends the section once for each object in the collection, and then deselects the section
            writer.SetMultiSectionFields("ROW", data);

            writer.AppendAll();
            var actuals = writer.GetContent();

            var expected = new List<string>
            {
                "<h1>Example</h1>",
                "",
                "<table>",
                "  <thead>",
                "    <tr>",
                "      <th>First Name</th>",
                "      <th>Last Name</th>",
                "    </tr>",
                "  </thead>",
                "  <tbody>",
                "    <tr>",
                "      <td>Ichabod</td>",
                "      <td>Crane</td>",
                "    </tr>",
                "    <tr>",
                "      <td>Alex</td>",
                "      <td>Rodriguez</td>",
                "    </tr>",
                "  </tbody>",
                "</table>"
            }.Concat("\r\n");

            actuals.Should().Be(expected);
        }

        [Fact]
        public void TestDocumentation_SectionsExample()
        {
            var template = new Template(templateTexts[9]);
            var writer = new TemplateWriter(template);

            // select a section so that it can be worked with
            writer.SelectSection("ROW");

            // bind data to the section fields and append the content
            writer.SetField("FirstName", "Ichabod");
            writer.SetField("LastName", "Crane");
            writer.AppendSection();

            // bind data to the section fields and append the content
            writer.SetField("FirstName", "Alex");
            writer.SetField("LastName", "Rodriguez");
            writer.AppendSection(true);
            writer.AppendSection();

            var actuals = writer.GetContent();

            var expected = new List<string>
            {
                "<h1>Example</h1>",
                "",
                "<table>",
                "  <thead>",
                "    <tr>",
                "      <th>First Name</th>",
                "      <th>Last Name</th>",
                "    </tr>",
                "  </thead>",
                "  <tbody>",
                "    <tr>",
                "      <td>Ichabod</td>",
                "      <td>Crane</td>",
                "    </tr>",
                "    <tr>",
                "      <td>Alex</td>",
                "      <td>Rodriguez</td>",
                "    </tr>",
                "  </tbody>",
                "</table>"
            }.Concat("\r\n");

            actuals.Should().Be(expected);
        }

        [Fact]
        public void TestDeselectSection()
        {
            // create a template and writer for the test
            var tpl = new Template(templateTexts[1]);
            var parentWriter = new TemplateWriter(tpl);

            // select and deselect a section
            parentWriter.SelectSection("SECTION3");
            parentWriter.SelectedSectionName.Should().Be("SECTION3");
            parentWriter.DeselectSection();
            parentWriter.SelectedSectionName.Should().Be("@MAIN");
        }

        [Fact]
        public void TestFieldProviderAsOnlyField()
        {
            // create a template and main writer for the test
            var tpl1 = new Template(templateTexts[7]);
            var writer = new TemplateWriter(tpl1);

            // create a template and writer to serve as a provider and register it with the main writer
            var tpl2 = new Template(templateTexts[8]);
            var contentWriter = new TemplateWriter(tpl2);
            var provider = contentWriter.GetWriter("HEAD");
            writer.RegisterFieldProvider("HEAD", provider);

            // select the provider and append it
            writer.SelectProvider("HEAD");
            writer.AppendSection(true);

            // append all the way out
            writer.AppendAll();

            var expected = new List<string>
            {
                "<html>",
                "<head>",
                "    <title>Test</title>",
                "    <link rel=\"stylesheet\" href=\"Content/Import.css\" />",
                "<script type=\"text/javascript\" src=\"Scripts/App/import.js\"></script>",
                "</head>",
                "<body>",
                "  Some test info",
                "</body>",
                "</html>"
            }.Concat();

            var actual = writer.GetContent();

            actual.Should().Be(expected);
        }

        [Fact]
        public void TestGetContent()
        {
            // create a template and writer for the test
            var tpl = new Template(templateTexts[1]);
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
        public void TestGetContent_NotFromMain()
        {
            // create a template and writer for the test
            var tpl = new Template(templateTexts[4]);
            var writer = new TemplateWriter(tpl);

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
        public void TestGetWriter()
        {
            // create a template and writer for the test
            var tpl = new Template(templateTexts[5]);
            var wrt = new TemplateWriter(tpl);

            // get a new standalone writer for a specific section
            var wrt2 = wrt.GetWriter("SECTION2");

            //set fields in the current section
            wrt2.SetField("Field2_1", "Value2_1");
            wrt2.SetField("Field2_2", "Value2_2");

            // select a subsection and populate its fields
            wrt2.SelectSection("SECTION3");
            wrt2.SetField("Field3_1", "Value3_1");
            wrt2.AppendAll();

            var expected = new List<string>
            {
                "  Field2.1: Value2_1\r\n",
                "  section 3 text\r\n",
                " Field3.1: Value3_1\r\n",
                "  Field2.2: Value2_2\r\n",
            }.Concat();

            var actual = wrt2.GetContent();

            actual.Should().BeEquivalentTo(expected);
        }

        //[Fact]
        //public void TestIsProvider()
        //{
        //    // create a template and main writer for the test
        //    var tpl1 = new Template(templateTexts[4]);
        //    var writer = new TemplateWriter(tpl1);

        //    // create another template and writer to serve as a provider
        //    var tpl2 = new Template(templateTexts[3]);
        //    var provider = new TemplateWriter(tpl2);

        //    // register the provider for a field in the main section of the main writer
        //    writer.RegisterFieldProvider("Main2", provider);

        //    // select the provider and verify that it is a registered provider
        //    writer.SelectProvider("Main2");
        //    writer.CurrentWriter.IsProvider.Should().BeTrue();

        //    // select a child section within the provider and verify that it is not a provider
        //    writer.SelectSection("SECTION1");
        //    writer.CurrentWriter.IsProvider.Should().BeFalse();
        //}

        [Fact]
        public void TestLiteralTextBlock_Main()
        {
            // create a template and main writer for a document containing a literal block
            var tpl = new Template(templateTexts[10]);
            var writer = new TemplateWriter(tpl);
            var actual = writer.GetContent(true);

            var expected = templateTexts[10].Replace("<!-- **LITERAL1** -->", "").TrimStart();
            actual.Should().Be(expected);
        }

        [Fact]
        public void TestLiteralTextBlock_Nested()
        {
            // create a template and main writer for a document containing a literal block
            var tpl = new Template(templateTexts[11]);
            var writer = new TemplateWriter(tpl);
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

            var expected = sb.ToString();
            actual.Should().Be(expected);
        }

        [Fact]
        public void TestNestedFieldProviders()
        {
            // create a template and main writer for the test
            var tpl = new Template(templateTexts[6]);
            var writer = new TemplateWriter(tpl);

            // create a writer to serve as a provider and register it with the main writer
            var provider1 = new TemplateWriter(tpl);
            writer.RegisterFieldProvider("Field1", provider1);

            // set values in the main writer
            writer.SetField("SectionName", "Main Writer");

            // select the first provider and set values in it
            writer.SelectProvider("Field1");
            writer.SetField("SectionName", "Provider 1");

            // create a writer to serve as a provider and register it with the first provider
            var provider2 = new TemplateWriter(tpl);
            writer.CurrentWriter.RegisterFieldProvider("Field1", provider2);

            // select the second provider and set values in it
            writer.SelectProvider("Field1");
            writer.SetField("SectionName", "Provider 2");
            writer.SetField("Field1", "Actual field value");

            // append all the way out of the providers and the writer
            writer.AppendAll();

            var expected = new List<string>
            {
                "Start of section Main Writer",
                "Start of section Provider 1",
                "Start of section Provider 2",
                "Actual field value",
                "End of section Provider 2",
                "End of section Provider 1",
                "End of section Main Writer"
            }.Concat();

            var actual = writer.GetContent();

            actual.Should().Be(expected);
        }

        [Fact]
        public void TestRegisterFieldProvider_Child()
        {
            // create a template and main writer for the test
            var tpl1 = new Template(templateTexts[1]);
            var writer = new TemplateWriter(tpl1);

            // create another template and writer to serve as a provider
            var tpl2 = new Template(templateTexts[3]);
            var provider = new TemplateWriter(tpl2);

            // register the provider for a field in a child section of the main writer
            writer.RegisterFieldProvider("SECTION2", "Field2", provider);

            // get to the section containing the provider
            writer.SelectSection("SECTION1");
            writer.SelectSection("SECTION2");

            //writer.RegisterFieldProvider("Field2", provider);

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

            actual.Should().Be(expected);
        }

        [Fact]
        public void TestRegisterFieldProvider_Main()
        {
            // create a template and main writer for the test
            var tpl1 = new Template(templateTexts[4]);
            var writer = new TemplateWriter(tpl1);

            // create another template and writer to serve as a provider
            var tpl2 = new Template(templateTexts[3]);
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

            actual.Should().Be(expected);
        }

        [Fact]
        public void TestRepeatedNestedSections()
        {

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

                    for (var counter3 = 1; counter3 <= sectionCount3; counter3++)
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
            var tpl = new Template(templateTexts[1]);
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
        public void TestSectionName()
        {
            // setup a template and writer
            var tpl = new Template(templateTexts[5]);
            var writer = new TemplateWriter(tpl);

            // verify the initial sectino is main
            writer.CurrentWriter.SectionName.Should().Be("@MAIN");

            // get first nested section
            writer.SelectSection("SECTION1");
            writer.CurrentWriter.SectionName.Should().Be("SECTION1");
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
            var tpl = new Template(templateTexts[1]);
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
            var tpl = new Template(templateTexts[1]);
            var parentWriter = new TemplateWriter(tpl);

            parentWriter.SelectedSectionName.Should().Be("@MAIN");
            parentWriter.SelectSection("SECTION3");
            parentWriter.SelectedSectionName.Should().Be("SECTION3");
        }

        [Fact]
        public void TestSetFieldDateTime()
        {
            // create a template and writer for the test
            var tpl = new Template(templateTexts[3]);
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
                $"-->{dt}<--\r\n",
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
            var tpl = new Template(templateTexts[3]);
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
            var tpl = new Template(templateTexts[3]);
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
            var tpl = new Template(templateTexts[3]);
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
            var tpl = new Template(templateTexts[3]);
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
            var tpl = new Template(templateTexts[3]);
            var writer = new TemplateWriter(tpl);
            writer.SelectSection("SECTION1");

            var t = new List<string>() { "A", "B", "C" };

            // append a date field and get the content
            writer.SetField("Field1", t);
            writer.AppendSection(true);
            writer.AppendSection();

            var content = writer.GetContent();

            var expected = string.Concat(
                $"-->{t}<--\r\n",
                "--><--\r\n",
                "--><--\r\n",
                "--><--\r\n",
                "--><--\r\n",
                "--><--\r\n");

            content.Should().Be(expected);
        }

        [Fact]
        public void TestSetMultiSectionFields_WithName()
        {
            // setup a template and writer
            var tpl = new Template(templateTexts[3]);
            var writer = new TemplateWriter(tpl);

            // setup data
            var data = new List<MyData>
            {
                new MyData("value 1-1", "value 2-1", "value 3-1", "value 4-1", "value 5-1", "value 6-1"),
                new MyData("value 1-2", "value 2-2", "value 3-2", "value 4-2", "value 5-2", "value 6-2"),
                new MyData("value 1-3", "value 2-3", "value 3-3", "value 4-3", "value 5-3", "value 6-3"),
                new MyData("value 1-4", "value 2-4", "value 3-4", "value 4-4", "value 5-4", "value 6-4"),
            };

            // set a section for each row of data
            writer.SetMultiSectionFields<MyData>("SECTION1", data);
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
        public void TestSetMultiSectionFields_WithStandaloneWriter()
        {
            // setup a template and get a standalone writer
            var tpl = new Template(templateTexts[1]);
            var writer = new TemplateWriter(tpl).GetWriter("SECTION2");

            // setup data
            var data = new List<MyData>
            {
                new MyData("1", "Val4", "value 3-1", "value 4-1", "true", "false"),
                new MyData("2", "Val3", "value 3-2", "value 4-2", "0", "1"),
                new MyData("3", "Val2", "value 3-3", "value 4-3", "T", "F"),
                new MyData("4", "Val1", "value 3-4", "value 4-4", "f", "t"),
            };

            // set a section for each row of data
            writer.SetMultiSectionFields(data);
            var content = writer.GetContent();

            var expected = new List<string>
            {
            "  Field1: 1\r\n",
            "  Field2: Val4\r\n",
            "  Field1: 2\r\n",
            "  Field2: Val3\r\n",
            "  Field1: 3\r\n",
            "  Field2: Val2\r\n",
            "  Field1: 4\r\n",
            "  Field2: Val1\r\n"
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
            var data = new MyData("value 1-1", "value 2-1", "value 3-1", "value 4-1", "value 5-1", "value 6-1");

            // set a section for the row of data
            writer.SetSectionFields("SECTION1", data);
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
        public void TestSetSectionFields_WithNameAppendDeselect()
        {
            // setup a template and writer
            var tpl = new Template(templateTexts[3]);
            var writer = new TemplateWriter(tpl);

            var data = new MyData();
            var expected = new List<string>
            {
                "-->Value1<--\r\n",
                "-->Value2<--\r\n",
                "-->Value3<--\r\n",
                "-->Value4<--\r\n",
                "-->Value5<--\r\n",
                "-->Value6<--\r\n"
            }.Concat();

            // select a section, append data, then deselect the section
            writer.SetSectionFields("SECTION1", data, SectionOptions.AppendDeselect);
            writer.SelectedSectionName.Should().Be("@MAIN");

            // verify content
            var actual = writer.GetContent(true);
            actual.Should().Be(expected);
        }

        [Fact]
        public void TestSetSectionFields_WithNameAppendOnly()
        {
            // setup a template and writer
            var tpl = new Template(templateTexts[3]);
            var writer = new TemplateWriter(tpl);

            var data = new MyData();
            var expected = new List<string>
            {
                "-->Value1<--\r\n",
                "-->Value2<--\r\n",
                "-->Value3<--\r\n",
                "-->Value4<--\r\n",
                "-->Value5<--\r\n",
                "-->Value6<--\r\n"
            }.Concat();

            // select a section, append data, then manually deselect section
            writer.SetSectionFields("SECTION1", data, SectionOptions.AppendOnly);
            writer.SelectedSectionName.Should().Be("SECTION1");
            writer.DeselectSection();

            // verify content
            var actual = writer.GetContent(true);
            actual.Should().Be(expected);
        }

        [Fact]
        public void TestSetSectionFields_WithNameSet()
        {
            // setup a template and writer
            var tpl = new Template(templateTexts[3]);
            var writer = new TemplateWriter(tpl);

            var data = new MyData();
            var expected = new List<string>
            {
                "-->Value1<--\r\n",
                "-->Value2<--\r\n",
                "-->Value3<--\r\n",
                "-->Value4<--\r\n",
                "-->Value5<--\r\n",
                "-->Value6<--\r\n"
            }.Concat();

            // select a section and set data without appending the section
            writer.SetSectionFields("SECTION1", data, SectionOptions.Set);
            writer.SelectedSectionName.Should().Be("SECTION1");

            // verify no content because nothing was appended
            var actual = writer.GetContent();
            actual.Should().Be("");

            // clear data from the writer and start at the main section again
            writer.Reset();
            writer.DeselectSection();

            // select a section and set data without appending the section
            writer.SetSectionFields("SECTION1", data, SectionOptions.Set);
            writer.SelectedSectionName.Should().Be("SECTION1");

            // verify content with section appended and deselected
            actual = writer.GetContent(true);
            actual.Should().Be(expected);
        }

        [Fact]
        public void TestSetSectionFields_WithAppendDeselect()
        {
            // setup a template and writer
            var tpl = new Template(templateTexts[3]);
            var writer = new TemplateWriter(tpl);

            var data = new MyData();
            var expected = new List<string>
            {
                "-->Value1<--\r\n",
                "-->Value2<--\r\n",
                "-->Value3<--\r\n",
                "-->Value4<--\r\n",
                "-->Value5<--\r\n",
                "-->Value6<--\r\n"
            }.Concat();

            // select a section, append data, then deselect the section
            writer.SelectSection("SECTION1");
            writer.SetSectionFields(data, SectionOptions.AppendDeselect);
            writer.SelectedSectionName.Should().Be("@MAIN");

            // verify content
            var actual = writer.GetContent(true);
            actual.Should().Be(expected);
        }

        [Fact]
        public void TestSetSectionFields_WithAppendOnly()
        {
            // setup a template and writer
            var tpl = new Template(templateTexts[3]);
            var writer = new TemplateWriter(tpl);

            var data = new MyData();
            var expected = new List<string>
            {
                "-->Value1<--\r\n",
                "-->Value2<--\r\n",
                "-->Value3<--\r\n",
                "-->Value4<--\r\n",
                "-->Value5<--\r\n",
                "-->Value6<--\r\n"
            }.Concat();

            // select a section, append data, then manually deselect section
            writer.SelectSection("SECTION1");
            writer.SetSectionFields(data, SectionOptions.AppendOnly);
            writer.SelectedSectionName.Should().Be("SECTION1");
            writer.DeselectSection();

            // verify content
            var actual = writer.GetContent(true);
            actual.Should().Be(expected);
        }

        [Fact]
        public void TestSetSectionFields_WithSet()
        {
            // setup a template and writer
            var tpl = new Template(templateTexts[3]);
            var writer = new TemplateWriter(tpl);

            var data = new MyData();
            var expected = new List<string>
            {
                "-->Value1<--\r\n",
                "-->Value2<--\r\n",
                "-->Value3<--\r\n",
                "-->Value4<--\r\n",
                "-->Value5<--\r\n",
                "-->Value6<--\r\n"
            }.Concat();

            // select a section and set data without appending the section
            writer.SelectSection("SECTION1");
            writer.SetSectionFields(data, SectionOptions.Set);
            writer.SelectedSectionName.Should().Be("SECTION1");

            // verify no content because nothing was appended
            var actual = writer.GetContent();
            actual.Should().Be("");

            // clear data from the writer and start at the main section again
            writer.Reset();
            writer.DeselectSection();

            // select a section and set data without appending the section
            writer.SelectSection("SECTION1");
            writer.SetSectionFields(data, SectionOptions.Set);
            writer.SelectedSectionName.Should().Be("SECTION1");

            // verify content with section appended and deselected
            actual = writer.GetContent(true);
            actual.Should().Be(expected);
        }

        [Fact]
        public void TestTemplate()
        {
            // setup a template and writer
            var tpl = new Template(templateTexts[3]);
            var writer = new TemplateWriter(tpl);

            writer.Template.Should().BeSameAs(tpl);
        }

        [Fact]
        public void TestTemplateId()
        {
            // setup a template and writer
            var tpl = new Template(templateTexts[3]);
            var writer = new TemplateWriter(tpl);

            writer.TemplateId.Should().Be(tpl.TemplateId);
        }

    }

}
