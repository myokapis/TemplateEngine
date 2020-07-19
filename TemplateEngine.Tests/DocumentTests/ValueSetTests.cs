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
using TemplateEngine.Document;
using TemplateEngine.Writer;
using TemplateEngine.Tests.Helpers;
using FluentAssertions;
using Xunit;

namespace TemplateEngine.Tests.DocumentTests
{
    [Collection("TemplateTests")]
    public class ValueSetTests
    {
        private readonly TemplateMocks mocks;

        public ValueSetTests(TemplateMocks mocks)
        {
            this.mocks = mocks;
        }

        [Fact]
        public void TestClear()
        {
            var valueSet = new ValueSet
            {
                FieldValues = new Dictionary<string, string>
                {
                    { "One", "1" },
                    { "Two", "2" },
                    { "Three", "3" }
                },
                FieldWriters = new Dictionary<string, Writer.ITemplateWriter>
                {
                    { "One", mocks.MockWriters.First() }
                },
                SectionWriters = mocks.MockWriters.Skip(1).Take(2).ToDictionary(k => k.WriterId.ToString(), v => v)
            };

            valueSet.FieldValues.Count.Should().Be(3);
            valueSet.FieldWriters.Count.Should().Be(1);
            valueSet.SectionWriters.Count.Should().Be(2);

            valueSet.Clear();

            valueSet.FieldValues.Count.Should().Be(0);
            valueSet.FieldWriters.Count.Should().Be(0);
            valueSet.SectionWriters.Count.Should().Be(0);
        }

        [Fact]
        public void TestHasData_All()
        {
            var valueSet = new ValueSet
            {
                FieldValues = new Dictionary<string, string>
                {
                    { "One", "1" }
                },
                FieldWriters = new Dictionary<string, ITemplateWriter>
                {
                    { "One", mocks.MockWriters.First() }
                },
                SectionWriters = new Dictionary<string, ITemplateWriter>
                {
                    { "One", mocks.MockWriters.First() }
                }
            };

            valueSet.HasData.Should().BeTrue();
        }

        [Fact]
        public void TestHasData_FieldValues()
        {
            var valueSet = new ValueSet
            {
                FieldValues = new Dictionary<string, string>
                {
                    { "One", "1" }
                },
                FieldWriters = new Dictionary<string, ITemplateWriter>(),
                SectionWriters = new Dictionary<string, ITemplateWriter>()
            };

            valueSet.HasData.Should().BeTrue();
        }

        [Fact]
        public void TestHasData_FieldWriters()
        {
            var valueSet = new ValueSet
            {
                FieldValues = new Dictionary<string, string>(),
                FieldWriters = new Dictionary<string, ITemplateWriter>
                {
                    { "One", mocks.MockWriters.First() }
                },
                SectionWriters = new Dictionary<string, ITemplateWriter>()
            };

            valueSet.HasData.Should().BeTrue();
        }

        [Fact]
        public void TestHasData_None()
        {
            var valueSet = new ValueSet
            {
                FieldValues = new Dictionary<string, string>(),
                FieldWriters = new Dictionary<string, ITemplateWriter>(),
                SectionWriters = new Dictionary<string, ITemplateWriter>()
            };

            valueSet.HasData.Should().BeFalse();
        }

        [Fact]
        public void TestHasData_SectionWriters()
        {
            var valueSet = new ValueSet
            {
                FieldValues = new Dictionary<string, string>(),
                FieldWriters = new Dictionary<string, ITemplateWriter>(),
                SectionWriters = new Dictionary<string, ITemplateWriter>
                {
                    { "One", mocks.MockWriters.First() }
                }
            };

            valueSet.HasData.Should().BeTrue();
        }

        [Fact]
        public void TestHasData_Uninitialized()
        {
            var valueSet = new ValueSet();

            valueSet.HasData.Should().BeFalse();
        }

    }

}
