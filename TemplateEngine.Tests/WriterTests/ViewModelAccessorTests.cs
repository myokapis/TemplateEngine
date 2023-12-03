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

using System.Collections.Generic;
using FluentAssertions;
using TemplateEngine.Writer;
using Xunit;

namespace TemplateEngine.Tests
{

    public class ViewModelAccessorTests
    {

        [Fact]
        public void TestCount()
        {
            Model1 model1 = new() { PropertyA = "ValueA", PropertyB = "ValueB" };
            Model2 model2 = new() { PropertyA = "ValueA", PropertyB = "ValueB", PropertyC = "ValueC" };

            ViewModelAccessor<Model1> accessor1 = new(model1);
            ViewModelAccessor<Model2> accessor2 = new(model2);

            accessor1.Count.Should().Be(2);
            accessor2.Count.Should().Be(3);
        }

        [Fact]
        public void TestCreateObject()
        {
            Model1 model1 = new() { PropertyA = "ValueA", PropertyB = "ValueB" };
            Model2 model2 = new() { PropertyA = "Value1", PropertyB = "Value2", PropertyC = "Value3" };

            ViewModelAccessor<Model1> accessor1 = new(model1);
            ViewModelAccessor<Model2> accessor2 = new(model2);

            accessor1.Should().BeOfType<ViewModelAccessor<Model1>>();
            accessor2.Should().BeOfType<ViewModelAccessor<Model2>>();
        }

        [Fact]
        public void TestEnumerator()
        {
            Model1 model1 = new() { PropertyA = "ValueA", PropertyB = "ValueB" };
            Model2 model2 = new() { PropertyA = "Value1", PropertyB = "Value2", PropertyC = "Value3" };

            ViewModelAccessor<Model1> accessor1 = new(model1);
            ViewModelAccessor<Model2> accessor2 = new(model2);

            Dictionary<string, string> dictionary1 = new();
            Dictionary<string, string> dictionary2 = new();
            Dictionary<string, string> expectedValues1 = new() { { "PropertyA", model1.PropertyA }, { "PropertyB", model1.PropertyB } };
            Dictionary<string, string> expectedValues2 = new() { { "PropertyA", model2.PropertyA }, { "PropertyB", model2.PropertyB }, { "PropertyC", model2.PropertyC } };

            foreach (KeyValuePair<string, string> kvp in accessor1.FieldValues)
            {
                dictionary1.Add(kvp.Key, kvp.Value);
            }

            foreach (KeyValuePair<string, string> kvp in accessor2.FieldValues)
            {
                dictionary2.Add(kvp.Key, kvp.Value);
            }

            expectedValues1.Should().BeEquivalentTo(dictionary1);
            expectedValues2.Should().BeEquivalentTo(dictionary2);
        }

        [Fact]
        public void TestIndexer_ByIndex()
        {
            Model2 model2 = new() { PropertyA = "Value1", PropertyB = "Value2", PropertyC = "Value3" };

            ViewModelAccessor<Model2> accessor2 = new(model2);

            model2.PropertyA.Should().BeEquivalentTo(accessor2[0]);
            model2.PropertyB.Should().BeEquivalentTo(accessor2[1]);
            model2.PropertyC.Should().BeSameAs(accessor2[2]);
        }

        [Fact]
        public void TestIndexer_ByFieldName()
        {
            Model2 model2 = new() { PropertyA = "Value1", PropertyB = "Value2", PropertyC = "Value3" };

            ViewModelAccessor<Model2> accessor2 = new(model2);

            model2.PropertyA.Should().BeEquivalentTo(accessor2["PropertyA"]);
            model2.PropertyB.Should().BeEquivalentTo(accessor2["PropertyB"]);
            model2.PropertyC.Should().BeEquivalentTo(accessor2["PropertyC"]);
        }

        [Fact]
        public void TestModel()
        {
            Model1 model1 = new() { PropertyA = "ValueA", PropertyB = "ValueB" };

            ViewModelAccessor<Model1> accessor1 = new(model1);

            model1.PropertyA.Should().BeEquivalentTo(accessor1.Model.PropertyA);
            model1.PropertyB.Should().BeEquivalentTo(accessor1.Model.PropertyB);
            model1.Should().BeSameAs(accessor1.Model);
        }

        [Fact]
        public void TestStruct()
        {
            var model3 = new Model3() { PropertyA = "ValueA", PropertyB = "ValueB" };

            ViewModelAccessor<Model3> accessor1 = new(model3);

            model3.PropertyA.Should().BeEquivalentTo(accessor1.Model.PropertyA);
            model3.PropertyB.Should().BeEquivalentTo(accessor1.Model.PropertyB);
            model3.Should().BeEquivalentTo(accessor1.Model);
        }

        #region "internal objects"

        public class Model1
        {
            public string? PropertyA { get; set; }
            public string? PropertyB { get; set; }
        }

        public class Model2
        {
            public string? PropertyA { get; set; }
            public string? PropertyB { get; set; }
            public string? PropertyC { get; set; }
        }

        public struct Model3
        {
            public string PropertyA { get; set; }
            public string PropertyB { get; set; }
            public string PropertyC { get; set; }
        }

        #endregion
    }
}
