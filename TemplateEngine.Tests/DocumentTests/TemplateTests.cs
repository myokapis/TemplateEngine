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
using System.Linq;
using System.Text;
using FluentAssertions;
using TemplateEngine.Document;
using TemplateEngine.Tests.Helpers;
using Xunit;

namespace TemplateEngine.Tests.DocumentTests
{

    public class TemplateTests
    {
        private readonly List<string> templateTexts = TemplateText.Items;

        [Fact]
        public void TestChildSectionNames()
        {
            var template = new Template(GetTemplateFileText());
            var templates = GetAllTemplates(template);

            var actual = new Dictionary<string, List<string>>();

            foreach (var tpl in templates)
            {
                actual.Add(tpl.SectionName, tpl.ChildSectionNames);
            }

            actual.Should().BeEquivalentTo(childSectionNames);
        }

        [Fact]
        public void TestCopy()
        {
            var template1 = new Template(GetTemplateFileText());
            var template2 = template1.Copy();

            template1.TemplateTypeId.Should().Be(template2.TemplateTypeId);
            template1.TemplateId.Should().NotBe(template2.TemplateId);
        }

        [Fact]
        public void TestDuplicateSectionName()
        {
            var templateText = string.Concat(new string[]
            {
                "<!-- @@SECTION1@@ -->some stuff<!-- @@SECTION1@@ -->",
                "<!-- @@SECTION2@@ -->",
                "    <!-- @@SECTION1@@ -->some stuff<!-- @@SECTION1@@ -->",
                "<!-- @@SECTION2@@ -->"
            });

            Action action = () => new Template(templateText);
            action.Should().Throw<ArgumentException>()
                .WithMessage("The CloseIndex of the section parameter has already been set.");
        }

        [Fact]
        public void TestFieldNames()
        {
            var template = new Template(GetTemplateFileText());
            var templates = GetAllTemplates(template);

            var actual = new Dictionary<string, List<string>>();

            foreach (var tpl in templates)
            {
                actual.Add(tpl.SectionName, tpl.FieldNames);
            }

            actual.Should().BeEquivalentTo(fieldNames);
        }

        [Fact]
        public void TestLiteralSection_AsMain()
        {
            var templateText = templateTexts[10];
            var template = new Template(templateText);

            template.ToString().Should().Be(templateText);
        }

        [Fact]
        public void TestLiteralSection_EmbeddedFields()
        {
            var templateText = templateTexts[11];
            var template = new Template(templateText);

            template.ToString().Should().Be(templateText);
        }

        [Fact]
        public void TestLiteralSection_EmbeddedSections()
        {
            var templateText = templateTexts[12];
            var template = new Template(templateText);

            template.ToString().Should().Be(templateText);
        }

        [Fact]
        public void TestMissingTag()
        {
            var templateText = string.Concat(new string[]
            {
                "<!-- @@SECTION1@@ -->",
                "text and more text",
                "<!-- @@SECTION2@@ -->",
                "some stuff",
                "<!-- @@SECTION1@@ -->",
                "some more stuff"
            });

            Action action = () => new Template(templateText);
            action.Should().Throw<InvalidOperationException>()
                .WithMessage("Section, SECTION2, is missing an opening or closing tag.");
        }


        [Fact]
        public void TestOverlappingSections()
        {
            var templateText = string.Concat(new string[]
            {
                "<!-- @@SECTION1@@ -->",
                "text and more text",
                "<!-- @@SECTION2@@ -->",
                "some stuff",
                "<!-- @@SECTION1@@ -->",
                "some more stuff",
                "<!-- @@SECTION2@@ -->"
            });

            Action action = () => new Template(templateText);
            action.Should().Throw<InvalidOperationException>()
                .WithMessage("Section, SECTION2, is improperly nested.");
        }

        [Fact]
        public void TestRawLength()
        {
            var template = new Template(GetTemplateFileText());
            var templates = GetAllTemplates(template);

            var actual = new Dictionary<string, int>();

            foreach (var tpl in templates)
            {
                actual.Add(tpl.SectionName, tpl.RawLength);
            }

            actual.Should().BeEquivalentTo(rawLength);
        }

        [Fact]
        public void TestSectionNames()
        {
            var template = new Template(GetTemplateFileText());
            var templates = GetAllTemplates(template);

            var actual = new List<string>();

            foreach (var tpl in templates)
            {
                actual.Add(tpl.SectionName);
            }

            actual.Should().BeEquivalentTo(sectionNames);
        }

        [Fact]
        public void TestTextBlocks()
        {
            var templateText = string.Join("\r\n", new string[]
            {
                "Line Before",
                "<!-- @@SECTION1@@ -->",
                "text and more text",
                "@@Field1@@",
                "<!-- @@SECTION2@@ -->",
                "some more stuff @@Field2@@ and a field",
                "<!-- @@SECTION2@@ -->",
                "<!-- @@SECTION1@@ -->",
                "\r\nsome stuff with a @@Field0@@ in it",
                "text after"
            });

            var template = new Template(templateText);
            var templates = GetAllTemplates(template);

            var expected = textBlocks.ToDictionary(t => t.Key, t => t.Value.Select(b => new
            {
                b.Type,
                b.Text,
                b.TagText,
                b.ReferenceName
            }));

            var actual = templates.ToDictionary(t => t.SectionName, t => t.TextBlocks.Select(b => new
            {
                b.Type,
                b.Text,
                b.TagText,
                b.ReferenceName
            }));

            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void TestToString()
        {
            var templateText = GetTemplateFileText();
            var template = new Template(templateText);
            template.ToString().Should().BeEquivalentTo(templateText);
        }

        #region Helpers

        private List<ITemplate> GetAllChildTemplates(ITemplate template)
        {
            var templates = new List<ITemplate>();

            foreach (var childSectionName in template.ChildSectionNames)
            {
                var tpl = template.GetTemplate(childSectionName);
                templates.Add(tpl);
                templates.AddRange(GetAllChildTemplates(tpl));
            }

            return templates;
        }

        private List<ITemplate> GetAllTemplates(ITemplate template)
        {
            var templates = new List<ITemplate>() { template };
            templates.AddRange(GetAllChildTemplates(template));
            return templates;
        }

        private string GetTemplateFileText()
        {
            var bytes = Properties.Resources.Transactions;
            return Encoding.UTF8.GetString(bytes);
        }

        private readonly Dictionary<string, List<string>> childSectionNames = new Dictionary<string, List<string>>()
        {
            { "@MAIN", new List<string>() { "LICENSE", "HEAD", "BODY" } },
            { "LICENSE", new List<string>() },
            { "HEAD", new List<string>() },
            { "BODY", new List<string>() { "ROW", "EDITOR" } },
            { "ROW", new List<string>() },
            { "EDITOR", new List<string>() { "TRANSACTION_TYPE", "EDITOR_ROWS" } },
            { "TRANSACTION_TYPE", new List<string>() },
            { "EDITOR_ROWS", new List<string>() { "BUDGET_LINES" } },
            { "BUDGET_LINES", new List<string>() }
        };

        private readonly Dictionary<string, List<string>> fieldNames = new Dictionary<string, List<string>>()
        {
            { "@MAIN", new List<string>() { "MainField" } },
            { "LICENSE", new List<string>() },
            { "HEAD", new List<string>() },
            { "BODY", new List<string>() { "SELECTOR" } },
            { "ROW", new List<string>() { "TransactionId", "Class", "TransactionDate", "CheckNo", "TransactionDesc", "Amount" } },
            { "EDITOR", new List<string>() },
            { "TRANSACTION_TYPE", new List<string>() { "VALUE", "SELECTED", "TEXT" } },
            { "EDITOR_ROWS", new List<string>() { "Amount" } },
            { "BUDGET_LINES", new List<string>() { "VALUE", "SELECTED", "TEXT" } }
        };

        private readonly Dictionary<string, int> rawLength = new Dictionary<string, int>()
        {
            { "@MAIN", 3661 },
            { "LICENSE", 742 },
            { "HEAD", 131 },
            { "BODY", 2654 },
            { "ROW", 325 },
            { "EDITOR", 1601 },
            { "TRANSACTION_TYPE", 56 },
            { "EDITOR_ROWS", 271 },
            { "BUDGET_LINES", 56 }
        };

        private readonly List<string> sectionNames = new List<string>()
        {
            "@MAIN", "LICENSE", "HEAD", "BODY", "ROW", "EDITOR", "TRANSACTION_TYPE", "EDITOR_ROWS", "BUDGET_LINES"
        };

        private readonly Dictionary<string, List<TextBlock>> textBlocks = new Dictionary<string, List<TextBlock>>()
        {
            { "@MAIN", new List<TextBlock>()
                {
                    new TextBlock(TextBlockType.Text, "Line Before\r\n"),
                    new TextBlock(TextBlockType.Prefix, null, "SECTION1", ""),
                    new TextBlock(TextBlockType.SectionTag, null, "SECTION1", "<!-- @@SECTION1@@ -->"),
                    new TextBlock(TextBlockType.Suffix, null, "SECTION1", "\r\n"),
                    new TextBlock(TextBlockType.Section, "", "SECTION1"),
                    new TextBlock(TextBlockType.Prefix, null, "SECTION1", ""),
                    new TextBlock(TextBlockType.SectionTag, null, "SECTION1", "<!-- @@SECTION1@@ -->"),
                    new TextBlock(TextBlockType.Suffix, null, "SECTION1", "\r\n"),
                    new TextBlock(TextBlockType.Text, "\r\nsome stuff with a "),
                    new TextBlock(TextBlockType.Field, null, "Field0", "@@Field0@@"),
                    new TextBlock(TextBlockType.Text, " in it\r\ntext after")
                }
            },
            { "SECTION1", new List<TextBlock>()
                {
                    new TextBlock(TextBlockType.Text, "text and more text\r\n"),
                    new TextBlock(TextBlockType.Field, null, "Field1", "@@Field1@@"),
                    new TextBlock(TextBlockType.Text, "\r\n"),
                    new TextBlock(TextBlockType.Prefix, null, "SECTION2", ""),
                    new TextBlock(TextBlockType.SectionTag, null, "SECTION2", "<!-- @@SECTION2@@ -->"),
                    new TextBlock(TextBlockType.Suffix, null, "SECTION2", "\r\n"),
                    new TextBlock(TextBlockType.Section, "", "SECTION2"),
                    new TextBlock(TextBlockType.Prefix, null, "SECTION2", ""),
                    new TextBlock(TextBlockType.SectionTag, null, "SECTION2", "<!-- @@SECTION2@@ -->"),
                    new TextBlock(TextBlockType.Suffix, null, "SECTION2", "\r\n"),
                }
            },
            { "SECTION2", new List<TextBlock>()
                {
                    new TextBlock(TextBlockType.Text, "some more stuff "),
                    new TextBlock(TextBlockType.Field, null, "Field2", "@@Field2@@"),
                    new TextBlock(TextBlockType.Text, " and a field\r\n")
                }
            }
        };

        #endregion

    }

}
