using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
using Xunit;
using TemplateEngine;

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
            var parentWriter = new TemplateWriter(tpl);
            parentWriter.SelectSection("SECTION1");

            var t = new List<string>() { "A", "B", "C" };

            // append a date field and get the content
            parentWriter.SetField("Field1", t);
            parentWriter.AppendSection(true);
            parentWriter.AppendSection();
            var content = parentWriter.GetContent();

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

        }

        [Fact]
        public void TestSetSectionFields_EnumerableWithName()
        {

        }

        [Fact]
        public void TestSetSectionFields_EnumerableWithNameDefinitions()
        {

        }

        [Fact]
        public void TestSetSectionFields_EnumerableWithNameDefinitionsOptions()
        {

        }

        [Fact]
        public void TestSetSectionFields_EnumerableWithDefinitionsOptions()
        {

        }

        [Fact]
        public void TestSetSectionFields_POCOWithName()
        {

        }

        [Fact]
        public void TestSetSectionFields_POCOWithNameDefinitions()
        {

        }

        [Fact]
        public void TestSetSectionFields_POCOWithNameDefinitionsOptions()
        {

        }

        [Fact]
        public void TestSetSectionFields_POCOWithDefinitionsOptions()
        {

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
                "  <!-- @@OPTION_SECTION1@@ -->@@TEXT@@;@@VALUE@@;@@SELECTED@@<!-- @@OPTION_SECTION1@@ -->\r\n",
                "  <!-- @@OPTION_SECTION2@@ -->@@TEXT@@;@@VALUE@@;@@SELECTED@@<!-- @@OPTION_SECTION2@@ -->\r\n",
                "<!-- @@SECTION3@@ -->",
                "<!-- @@SECTION4@@ -->\r\n",
                "  Checkbox1: @@Checkbox1@@\r\n",
                "  Checkbox2: @@Checkbox2@@\r\n",
                "<!-- @@SECTION4@@ -->"
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
            }.Concat()
        };

        #endregion

    }

}
