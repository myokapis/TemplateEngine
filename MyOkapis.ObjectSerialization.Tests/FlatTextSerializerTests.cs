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
using MyOkapis.Formatting;
using Xunit;

namespace MyOkapis.ObjectSerialization.Tests
{

    public class FlatTextSerializerTests
    {
        private readonly FlatTextSerializer serializer = new(Formatter.Default);

        [Fact]
        public void TestSerialize_NullSource_ReturnsEmptyString()
        {
            serializer.Serialize(null, new SerializationOptions()).Should().Be("");
        }

        [Fact]
        public void TestSerialize_ConcatenatesFieldsWithTerminators()
        {
            var item = new LineItem { Code = "A1", Secret = "hidden" };
            serializer.Serialize(item, new SerializationOptions()).Should().Be("A1|hidden|");
        }

        [Fact]
        public void TestSerialize_AppliesMasking()
        {
            var item = new LineItem { Code = "A1", Secret = "hidden" };
            serializer.Serialize(item, new SerializationOptions { ApplyMasking = true }).Should().Be("A1|******|");
        }

        [Fact]
        public void TestSerialize_MaskingOff_LeavesValueUnchanged()
        {
            var item = new LineItem { Code = "A1", Secret = "hidden" };
            serializer.Serialize(item, new SerializationOptions { ApplyMasking = false }).Should().Be("A1|hidden|");
        }

        [Fact]
        public void TestSerialize_NestedAccessibleObject()
        {
            var invoice = new Invoice { Number = "INV1", PrimaryItem = new LineItem { Code = "A1", Secret = "x" } };
            serializer.Serialize(invoice, new SerializationOptions()).Should().Be("INV1;A1|x|");
        }

        [Fact]
        public void TestSerialize_NestedCollection()
        {
            var invoice = new Invoice
            {
                Number = "INV1",
                Items = new[]
                {
                    new LineItem { Code = "A1", Secret = "x" },
                    new LineItem { Code = "A2", Secret = "y" }
                }
            };

            serializer.Serialize(invoice, new SerializationOptions()).Should().Be("INV1;A1|x|A2|y|");
        }

        [Fact]
        public void TestSerialize_NullNestedFields_ContributeNothing()
        {
            var invoice = new Invoice { Number = "INV1" };
            serializer.Serialize(invoice, new SerializationOptions()).Should().Be("INV1;");
        }
    }
}
