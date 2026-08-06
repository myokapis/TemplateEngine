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

    public class XmlObjectSerializerTests
    {
        private readonly XmlObjectSerializer serializer = new(Formatter.Default);

        [Fact]
        public void TestSerialize_NullSource_ReturnsEmptyString()
        {
            serializer.Serialize(null, new XmlSerializationOptions()).Should().Be("");
        }

        [Fact]
        public void TestSerialize_ProducesElementsForEachField()
        {
            var person = new Person { Name = "Bob", Age = 30 };
            var result = serializer.Serialize(person, new XmlSerializationOptions());

            result.Should().Contain("<Name>Bob</Name>");
            result.Should().Contain("<Age>30</Age>");
        }

        [Fact]
        public void TestSerialize_AppliesMasking()
        {
            var person = new Person { Name = "Bob", Ssn = "123-45-6789" };
            var result = serializer.Serialize(person, new XmlSerializationOptions { ApplyMasking = true });

            result.Should().Contain("<Ssn>***********</Ssn>");
            result.Should().NotContain("123-45-6789");
        }

        [Fact]
        public void TestSerialize_MaskingOff_LeavesValueUnchanged()
        {
            var person = new Person { Name = "Bob", Ssn = "123-45-6789" };
            var result = serializer.Serialize(person, new XmlSerializationOptions { ApplyMasking = false });

            result.Should().Contain("<Ssn>123-45-6789</Ssn>");
        }

        [Fact]
        public void TestSerialize_DoesNotLeakAccessibleObjectBookkeeping()
        {
            var person = new Person { Name = "Bob", Age = 30 };
            var result = serializer.Serialize(person, new XmlSerializationOptions());

            result.Should().NotContain("FieldCount", "AccessibleObjectBase bookkeeping must not leak into serialized output");
            result.Should().NotContain("FieldDescriptors", "AccessibleObjectBase bookkeeping must not leak into serialized output");
        }

        [Fact]
        public void TestSerialize_OmitXmlDeclaration_ByDefault()
        {
            var person = new Person { Name = "Bob" };
            var result = serializer.Serialize(person, new XmlSerializationOptions());

            result.Should().NotStartWith("<?xml");
        }
    }
}
