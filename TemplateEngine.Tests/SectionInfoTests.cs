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

using System;
using FluentAssertions;
using Xunit;

namespace TemplateEngine.Tests
{

    public class SectionInfoTests
    {

        [Fact]
        public void TestSectionInfo_Close()
        {
            var sectionInfo = new SectionInfo("Section1", 13, "OpenTag1", "prefix", "suffix");
            sectionInfo = new SectionInfo(sectionInfo, 69, "CloseTag1", "closePrefix", "closeSuffix");

            var expected = new
            {
                SectionName = "Section1",
                OpenIndex = 13,
                OpenTag = "OpenTag1",
                OpenTagLength = 8,
                CloseIndex = 69,
                CloseTag = "CloseTag1",
                CloseTagLength = 9,
                OpenPrefix = "prefix",
                OpenSuffix = "suffix",
                ClosePrefix = "closePrefix",
                CloseSuffix = "closeSuffix",
                CloseTagEndIndex = 100,
                IsValid = true,
                OpenTagEndIndex = 33,
                SubstringRange = new Tuple<int, int>(33, 69)
            };

            var actual = new
            {
                sectionInfo.SectionName,
                sectionInfo.OpenIndex,
                sectionInfo.OpenTag,
                sectionInfo.OpenTagLength,
                sectionInfo.CloseIndex,
                sectionInfo.CloseTag,
                sectionInfo.CloseTagLength,
                sectionInfo.OpenPrefix,
                sectionInfo.OpenSuffix,
                sectionInfo.ClosePrefix,
                sectionInfo.CloseSuffix,
                sectionInfo.CloseTagEndIndex,
                sectionInfo.IsValid,
                sectionInfo.OpenTagEndIndex,
                sectionInfo.SubstringRange
            };

            actual.Should().Be(expected);
        }

        [Fact]
        public void TestSectionInfo_CloseAlreadySet()
        {
            var sectionInfo = new SectionInfo("Section1", 13, "OpenTag1", "prefix", "suffix");
            sectionInfo = new SectionInfo(sectionInfo, 69, "CloseTag1", "closePrefix", "closeSuffix");

            var exception = Record.Exception(() => new SectionInfo(sectionInfo, 99, "CloseTag2", "closePrefix2", "closeSuffix2"));
            exception.Should().BeOfType<ArgumentException>();
            exception.Message.Should().Be("The CloseIndex of the section parameter has already been set.");
        }

        [Fact]
        public void TestSectionInfo_CloseIndexError()
        {
            var sectionInfo = new SectionInfo("Section1", 13, "OpenTag1", "prefix", "suffix");
            var exception = Record.Exception(() => new SectionInfo(sectionInfo, 12, "CloseTag2", "closePrefix2", "closeSuffix2"));

            exception.Should().BeOfType<ArgumentException>();
            exception.Message.Should().Be("The CloseIndex must be greater than the OpenIndex of the section parameter.");
        }

        [Fact]
        public void TestSectionInfo_CloseTagError()
        {
            var sectionInfo = new SectionInfo("Section1", 13, "OpenTag1", "prefix", "suffix");

            var exception = Record.Exception(() => new SectionInfo(sectionInfo, 99, "", "closePrefix2", "closeSuffix2"));
            exception.Should().BeOfType<ArgumentException>();
            exception.Message.Should().Be("The CloseTag cannot be null or empty.");

            exception = Record.Exception(() => new SectionInfo(sectionInfo, 99, null, "closePrefix2", "closeSuffix2"));
            exception.Should().BeOfType<ArgumentException>();
            exception.Message.Should().Be("The CloseTag cannot be null or empty.");
        }

        [Fact]
        public void TestSectionInfo_Open()
        {
            var sectionInfo = new SectionInfo("Section1", 13, "OpenTag1", "prefix", "suffix", 69);

            var expected = new
            {
                SectionName = "Section1",
                OpenIndex = 13,
                OpenTag = "OpenTag1",
                OpenTagLength = 8,
                CloseIndex = 69,
                CloseTag = "",
                CloseTagLength = 0,
                OpenPrefix = "prefix",
                OpenSuffix = "suffix",
                ClosePrefix = "",
                CloseSuffix = "",
                CloseTagEndIndex = 69,
                IsValid = true,
                OpenTagEndIndex = 33,
                SubstringRange = new Tuple<int, int>(33, 69)
            };

            var actual = new
            {
                sectionInfo.SectionName,
                sectionInfo.OpenIndex,
                sectionInfo.OpenTag,
                sectionInfo.OpenTagLength,
                sectionInfo.CloseIndex,
                sectionInfo.CloseTag,
                sectionInfo.CloseTagLength,
                sectionInfo.OpenPrefix,
                sectionInfo.OpenSuffix,
                sectionInfo.ClosePrefix,
                sectionInfo.CloseSuffix,
                sectionInfo.CloseTagEndIndex,
                sectionInfo.IsValid,
                sectionInfo.OpenTagEndIndex,
                sectionInfo.SubstringRange
            };

            actual.Should().Be(expected);
        }

        [Fact]
        public void TestSectionInfo_OpenNewError()
        {
            var exception = Record.Exception(() => new SectionInfo("Section1", -1, "OpenTag1", "prefix", "suffix", 69));

            exception.Should().BeOfType<ArgumentException>();
            exception.Message.Should().Be("The OpenIndex must be greater than or equal to zero.");
        }

        [Fact]
        public void TestSectionInfo_OpenSubstringRangeError()
        {
            var sectionInfo = new SectionInfo("Section1", 13, "OpenTag1", "prefix", "suffix", 12);
            var exception = Record.Exception(() => sectionInfo.SubstringRange);

            exception.Should().BeOfType<Exception>();
            exception.Message.Should().Be("Section range is invalid.");
        }

    }

}
