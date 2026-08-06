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

using System.Linq;
using FluentAssertions;
using Xunit;

namespace MyOkapis.ObjectAccess.Tests
{

    public class AccessibleObjectBaseTests
    {

        [Fact]
        public void TestFieldCount()
        {
            var record = new Record { PropertyA = "A", PropertyB = "B" };
            record.FieldCount.Should().Be(2);
        }

        [Fact]
        public void TestFieldDescriptors()
        {
            var record = new Record { PropertyA = "A", PropertyB = "B" };
            record.FieldDescriptors.Select(f => f.Name).Should().BeEquivalentTo(new[] { "PropertyA", "PropertyB" });
        }

        [Fact]
        public void TestStaticFields_NoInstanceRequired()
        {
            Record.Fields.Select(f => f.Name).Should().BeEquivalentTo(new[] { "PropertyA", "PropertyB" });
        }

        [Fact]
        public void TestPolymorphicAccessViaNonGenericInterface()
        {
            IAccessibleObject accessible = new Record { PropertyA = "A", PropertyB = "B" };
            accessible.FieldCount.Should().Be(2);
        }

        [Fact]
        public void TestSharesCacheWithWrapperPattern()
        {
            // AccessibleObjectBase<T> delegates to AccessibleObject<T>'s own cache rather than
            // reflecting independently -- both patterns must see the same field metadata.
            var directFields = Record.Fields;
            var wrappedFields = AccessibleObject<Record>.Fields;

            directFields.Should().BeSameAs(wrappedFields);
        }

        public class Record : AccessibleObjectBase<Record>
        {
            public string? PropertyA { get; set; }
            public string? PropertyB { get; set; }
        }
    }
}
