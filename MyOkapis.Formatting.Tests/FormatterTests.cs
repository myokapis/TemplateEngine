/* ****************************************************************************
Copyright 2018-2026 Gene Graves

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

using FluentAssertions;
using Xunit;

namespace MyOkapis.Formatting.Tests
{

    public class FormatterTests
    {

        [Fact]
        public void TestFormat_NullValue_ReturnsEmptyString()
        {
            Formatter.Default.Format((object?)null, null).Should().Be("");
        }

        [Fact]
        public void TestFormat_NoAttribute_UsesPlainToString()
        {
            Formatter.Default.Format("hello", null).Should().Be("hello");
        }

        [Fact]
        public void TestFormat_StringWithCompositeFormatString()
        {
            var attr = new TestFormatAttribute { FormatString = "[{0}]" };
            Formatter.Default.Format("hello", attr).Should().Be("[hello]");
        }

        [Fact]
        public void TestFormat_MaxLength_Truncates()
        {
            var attr = new TestFormatAttribute { MaxLength = 3 };
            Formatter.Default.Format("hello", attr).Should().Be("hel");
        }

        [Fact]
        public void TestFormat_MinLength_LeftJustify_PadsRight()
        {
            var attr = new TestFormatAttribute { MinLength = 6, Justify = Justify.Left, PaddingChar = '.' };
            Formatter.Default.Format("hi", attr).Should().Be("hi....");
        }

        [Fact]
        public void TestFormat_MinLength_RightJustify_PadsLeft()
        {
            var attr = new TestFormatAttribute { MinLength = 6, Justify = Justify.Right, PaddingChar = '.' };
            Formatter.Default.Format("hi", attr).Should().Be("....hi");
        }

        [Fact]
        public void TestFormat_MinLength_CenterJustify_PadsBothSides()
        {
            var attr = new TestFormatAttribute { MinLength = 6, Justify = Justify.Center, PaddingChar = '.' };
            Formatter.Default.Format("hi", attr).Should().Be("..hi..");
        }

        [Fact]
        public void TestFormat_ValueAlreadyMeetsMinLength_NoPadding()
        {
            var attr = new TestFormatAttribute { MinLength = 3, PaddingChar = '.' };
            Formatter.Default.Format("hello", attr).Should().Be("hello");
        }

        [Fact]
        public void TestFormatAttribute_FixedLength_SetsMinAndMax()
        {
            var attr = new TestFormatAttribute { FixedLength = 5 };
            attr.MinLength.Should().Be(5);
            attr.MaxLength.Should().Be(5);
        }

        [Fact]
        public void TestFormatAttribute_MaxLengthCannotBeSetTwiceToDifferentValues()
        {
            var attr = new TestFormatAttribute { MaxLength = 5 };
            var act = () => attr.MaxLength = 10;
            act.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void TestFormatAttribute_MinLengthCannotExceedMaxLength()
        {
            var attr = new TestFormatAttribute { MaxLength = 5 };
            var act = () => attr.MinLength = 10;
            act.Should().Throw<InvalidOperationException>();
        }

        private sealed class TestFormatAttribute : FormatAttribute
        {
            public new string? FormatString { get => base.FormatString; init => base.FormatString = value; }
        }
    }

}
