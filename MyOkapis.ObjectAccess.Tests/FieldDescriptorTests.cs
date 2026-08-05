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

using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace MyOkapis.ObjectAccess.Tests
{

    public class FieldDescriptorTests
    {

        [Fact]
        public void TestGetAttribute_ConcreteType()
        {
            var field = AccessibleObject<Model>.Fields.First(f => f.Name == nameof(Model.Marked));
            var attribute = field.GetAttribute<MarkerAttribute>();

            attribute.Should().NotBeNull();
            attribute!.Note.Should().Be("hello");
        }

        [Fact]
        public void TestGetAttribute_ByInterface()
        {
            var field = AccessibleObject<Model>.Fields.First(f => f.Name == nameof(Model.Marked));
            var attribute = field.GetAttribute<IMarker>();

            attribute.Should().NotBeNull();
            attribute!.Note.Should().Be("hello");
        }

        [Fact]
        public void TestGetAttribute_NotPresent()
        {
            var field = AccessibleObject<Model>.Fields.First(f => f.Name == nameof(Model.Unmarked));
            var attribute = field.GetAttribute<MarkerAttribute>();

            attribute.Should().BeNull();
        }

        [Fact]
        public void TestNotMappedPropertyIsExcluded()
        {
            AccessibleObject<Model>.Fields.Select(f => f.Name).Should().NotContain(nameof(Model.Hidden));
        }

        public interface IMarker
        {
            string Note { get; }
        }

        [AttributeUsage(AttributeTargets.Property)]
        public class MarkerAttribute : Attribute, IMarker
        {
            public MarkerAttribute(string note)
            {
                Note = note;
            }

            public string Note { get; }
        }

        public class Model
        {
            [Marker("hello")]
            public string? Marked { get; set; }

            public string? Unmarked { get; set; }

            [NotMapped]
            public string? Hidden { get; set; }
        }
    }
}
