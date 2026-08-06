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

    public class JsonObjectSerializerTests
    {
        private readonly JsonObjectSerializer serializer = new(Formatter.Default);

        [Fact]
        public void TestSerialize_NullSource_ReturnsEmptyString()
        {
            serializer.Serialize(null, new JsonSerializationOptions()).Should().Be("");
        }

        [Fact]
        public void TestSerialize_SuppressNull_OmitsNullFields()
        {
            var person = new Person { Name = "Bob", Age = null, Ssn = null };
            var result = serializer.Serialize(person, new JsonSerializationOptions(NullHandlingOptions.SuppressNull));

            result.Should().Contain("\"name\":\"Bob\"");
            result.Should().NotContain("age");
            result.Should().NotContain("ssn");
        }

        [Fact]
        public void TestSerialize_PreserveNulls_KeepsNullFields()
        {
            var person = new Person { Name = "Bob", Age = null, Ssn = null };
            var result = serializer.Serialize(person, new JsonSerializationOptions(NullHandlingOptions.PreserveNulls));

            result.Should().Contain("\"age\":null");
        }

        [Fact]
        public void TestSerialize_AppliesMasking()
        {
            var person = new Person { Name = "Bob", Ssn = "123-45-6789" };
            var options = new JsonSerializationOptions(NullHandlingOptions.SuppressNull) { ApplyMasking = true };
            var result = serializer.Serialize(person, options);

            result.Should().Contain("\"ssn\":\"***********\"");
            result.Should().NotContain("123-45-6789");
        }

        [Fact]
        public void TestSerialize_MaskingOff_LeavesValueUnchanged()
        {
            var person = new Person { Name = "Bob", Ssn = "123-45-6789" };
            var options = new JsonSerializationOptions(NullHandlingOptions.SuppressNull) { ApplyMasking = false };
            var result = serializer.Serialize(person, options);

            result.Should().Contain("\"ssn\":\"123-45-6789\"");
        }

        [Fact]
        public void TestSerialize_Flattened_NullAsEmptyString()
        {
            // The flattened path serializes a plain Dictionary<string, object?>, so property
            // naming policy (camelCase) does not apply to its keys -- only to reflected class
            // properties, which is what the non-flattened path serializes instead.
            var person = new Person { Name = "Bob", Age = null };
            var options = new JsonSerializationOptions(NullHandlingOptions.NullAsEmptyString) { Flatten = true };
            var result = serializer.Serialize(person, options);

            result.Should().Contain("\"Age\":\"\"");
        }

        [Fact]
        public void TestSerialize_DoesNotLeakAccessibleObjectBookkeeping()
        {
            var person = new Person { Name = "Bob", Age = 30 };
            var result = serializer.Serialize(person, new JsonSerializationOptions(NullHandlingOptions.PreserveNulls));

            result.Should().NotContain("fieldCount", "AccessibleObjectBase bookkeeping must not leak into serialized output");
            result.Should().NotContain("fieldDescriptors", "AccessibleObjectBase bookkeeping must not leak into serialized output");
        }
    }
}
