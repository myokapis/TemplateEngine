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

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using TemplateEngine.Tests.Helpers;
using Xunit;

namespace TemplateEngine.Tests.WebTests
{

    [Collection("TemplateTests")]
    public class MasterPresenterBaseTests
    {
        private readonly TemplateMocks mocks;

        public MasterPresenterBaseTests(TemplateMocks mocks)
        {
            this.mocks = mocks;
        }

        /* ***** NOTES ************************************************************
        These tests remove new line characters to simplify comparisons.
        Layout details are tested in other test classes. The purpose of
        these Presenter tests is to verify the inherited protected methods
        are working correctly. Checking that master and content templates
        are loaded and merged correctly with appropriate content is sufficient.
        ************************************************************************* */

        [Fact]
        public async Task TestGetContent()
        {
            var expectedContent = GetMasterContent(true, true, true);
            var presenter = TemplateMocks.GetMasterPresenter();
            var actualContent = await presenter.GetContent_Okay();

            Assert.Equal(expectedContent, actualContent);
        }

        [Fact]
        public void TestGetContent_Error()
        {
            var presenter = TemplateMocks.GetMasterPresenter();

            Assert.Throws<ApplicationException>(() => presenter.GetContent_Error())
                .Message.Should().Be(messages["NoMasterWriter"]);
        }

        [Fact]
        public void TestSections()
        {
            List<string> expectedValues = new() { "HEAD", "BODY", "TAIL" };
            var presenter = TemplateMocks.GetMasterPresenter();

            Assert.Equal(expectedValues, presenter.Sections);
        }

        [Fact]
        public async Task TestSetupContentWriter()
        {
            var expectedContent = GetSectionContent("Head");
            var presenter = TemplateMocks.GetMasterPresenter();
            var actualContent = await presenter.SetupContentWriter();

            Assert.Equal(expectedContent, actualContent);
        }

        [Fact]
        public async Task TestSetupMasterPage()
        {
            var expectedContent = GetMasterContent(true, true, true);
            var presenter = TemplateMocks.GetMasterPresenter();
            var actualContent = await presenter.SetupMasterPage_Okay();

            Assert.Equal(expectedContent, actualContent);
        }

        [Fact]
        public void TestSetupMasterPage_Error()
        {
            var expectedContent = GetMasterContent(true, true, true);
            var presenter = TemplateMocks.GetMasterPresenter();

            Assert.Throws<ApplicationException>(() => presenter.SetupMasterPage_Error())
                .Message.Should().Be(messages["NoContentWriter"]);
        }

        [Fact]
        public async Task TestSetupMasterPage_Sections()
        {
            var expectedContent = GetMasterContent(true, true, true);
            var presenter = TemplateMocks.GetMasterPresenter();
            var actualContent = await presenter.SetupMasterPage_Sections();

            Assert.Equal(expectedContent, actualContent);
        }

        [Fact]
        public void TestSetupMasterPage_Sections_Error()
        {
            var presenter = TemplateMocks.GetMasterPresenter();

            Assert.Throws<ApplicationException>(() => presenter.SetupMasterPage_Sections_Error())
                .Message.Should().Be(messages["NoContentWriter"]);
        }

        [Fact]
        public void TestSetupMasterPage_Writers_Error()
        {
            var presenter = TemplateMocks.GetMasterPresenter();

            Assert.Throws<ApplicationException>(() => presenter.SetupMasterPage_Sections_Error())
                .Message.Should().Be(messages["NoContentWriter"]);
        }

        [Fact]
        public async Task TestWriteMasterSectionAsync()
        {
            var expectedContent = GetMasterContent(false, true, false)
                .Replace("    <p>Test body</p>", "    <p>Test body</p>    <p>Test body</p>");
            var presenter = TemplateMocks.GetMasterPresenter();
            var actualContent = await presenter.WriteMasterSectionAsync_Okay();

            Assert.Equal(expectedContent, actualContent);
        }

        [Fact]
        public async Task TestWriteMasterSectionAsync_Error()
        {
            var presenter = TemplateMocks.GetMasterPresenter();

            (await Assert.ThrowsAsync<ApplicationException>(
                async () => await presenter.WriteMasterSectionAsync_Error()))
                .Message.Should().Be(messages["NoContentWriter"]);
        }

        [Fact]
        public async Task TestWriteMasterSection()
        {
            var expectedContent = GetMasterContent(false, true, false);
            var presenter = TemplateMocks.GetMasterPresenter();
            var actualContent = await presenter.WriteMasterSection_Okay();

            Assert.Equal(expectedContent, actualContent);
        }

        [Fact]
        public void TestWriteMasterSection_Error()
        {
            var presenter = TemplateMocks.GetMasterPresenter();

            Assert.Throws<ApplicationException>(() => presenter.WriteMasterSection_Error())
                .Message.Should().Be(messages["NoContentWriter"]);
        }

        [Fact]
        public async Task TestWriteMasterSection_PageBuilder()
        {
            var expectedContent = GetMasterContent(false, true, false)
                .Replace("    <p>Test body</p>", "    <p>Test body</p>    <p>Test body</p>");
            
            var presenter = TemplateMocks.GetMasterPresenter();
            var actualContent = await presenter.WriteMasterSection_PageBuilder_Okay();

            Assert.Equal(expectedContent, actualContent);
        }

        [Fact]
        public void TestWriteMasterSection_PageBuilder_Error()
        {
            var presenter = TemplateMocks.GetMasterPresenter();

            Assert.Throws<ApplicationException>(() => presenter.WriteMasterSection_PageBuilder_Error())
                .Message.Should().Be(messages["NoContentWriter"]);
        }

        [Fact]
        public void TestWriteMasterSections_Error()
        {
            var presenter = TemplateMocks.GetMasterPresenter();

            Assert.Throws<ApplicationException>(() => presenter.WriteMasterSections_Error())
                .Message.Should().Be(messages["NoContentWriter"]);
        }

        private static Dictionary<string, string> messages = new()
        {
            { "NoMasterWriter", "No master writer has been loaded for this presenter." },
            { "NoContentWriter", "No content writer has been loaded for this presenter." }
        };

        private static Dictionary<string, int> sectionIndices = new()
        {
            { "Head", 1 },
            { "Body", 5 },
            { "Tail", 9 }
        };

        private static string GetMasterContent(bool useHead = false, bool useBody = false, bool useTail = false)
        {
            NestedContent nestedContent = GetNestedContent(useHead, useBody, useTail);
            return GetMasterContent(nestedContent);
        }

        private static string GetMasterContent(NestedContent nestedContent)
        {
            List<string> masterContent = TemplateMocks.MasterTemplate;

            SetContent(masterContent, "Tail", nestedContent.Tail);
            SetContent(masterContent, "Body", nestedContent.Body);
            SetContent(masterContent, "Head", nestedContent.Head);

            return string.Join("", masterContent).Replace("\r\n", "");
        }

        private static NestedContent GetNestedContent(bool useHead = false, bool useBody = false, bool useTail = false)
        {
            List<string> baseContent = TemplateMocks.ContentTemplate;

            return new NestedContent
            {
                Head = useHead ? baseContent[sectionIndices["Head"]]: null,
                Body = useBody ? baseContent[sectionIndices["Body"]] : null,
                Tail = useTail ? baseContent[sectionIndices["Tail"]] : null
            };
        }

        private static string GetSectionContent(string sectionName)
        {
            List<string> baseContent = TemplateMocks.ContentTemplate;
            return baseContent[sectionIndices[sectionName]];
        }

        /// <summary>
        /// Sets the section content to the passed in content if the passed in content is not null.
        /// Otherwise it removes the section from the content.
        /// </summary>
        /// <param name="masterContent">The master content</param>
        /// <param name="sectionContent">The content to merge into a section of the master content</param>
        /// <param name="index">The line index of the section to which content should be merged</param>
        private static void SetContent(List<string> masterContent, string sectionName, string? sectionContent)
        {
            string sectionMarker = $"@@{sectionName.ToUpper()}@@";
            int sectionIndex = masterContent.IndexOf(sectionMarker);

            if (sectionContent == null)
            {
                masterContent.RemoveAt(sectionIndex);
            }
            else
            {
                masterContent[sectionIndex] = sectionContent;
            }
        }

        internal class NestedContent
        {
            internal string? Body { get; set; }
            internal string? Head { get; set; }
            internal string? Tail { get; set; }
        }
    }

}
