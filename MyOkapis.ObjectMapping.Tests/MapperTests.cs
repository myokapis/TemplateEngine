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
using MyOkapis.ObjectAccess;
using Xunit;

namespace MyOkapis.ObjectMapping.Tests
{

    public class MapperTests
    {

        [Fact]
        public void TestMap_ReturnsStronglyTypedTarget()
        {
            var mapper = new PersonMapper();
            var target = mapper.Map(new PersonSource { Name = "Ichabod", Age = 30 });

            target.Should().BeOfType<PersonTarget>();
            target.Name.Should().Be("Ichabod");
            target.Age.Should().Be(30);
        }

        [Fact]
        public void TestMap_ViaInterface_ReturnsIAccessibleObject()
        {
            IMapper<PersonSource> mapper = new PersonMapper();
            IAccessibleObject target = mapper.Map(new PersonSource { Name = "Ichabod", Age = 30 });

            target.Should().BeAssignableTo<IAccessibleObject>();
            target.FieldCount.Should().Be(2);
        }

        [Fact]
        public void TestSetFields_SetsMatchingFieldsOnly()
        {
            var mapper = new PersonMapper();
            var target = new PersonTarget { Name = "Original", Age = 1 };

            mapper.SetFields(target, new Dictionary<string, object?> { { "Name", "Updated" } });

            target.Name.Should().Be("Updated");
            target.Age.Should().Be(1);
        }

        [Fact]
        public void TestSetFields_ViaNonGenericInterface()
        {
            IMapper mapper = new PersonMapper();
            var target = new PersonTarget { Name = "Original", Age = 1 };

            mapper.SetFields(target, new Dictionary<string, object?> { { "Age", 99 } });

            target.Age.Should().Be(99);
        }

        [Fact]
        public void TestCompositeMapper_DelegatesToChildMapper()
        {
            var mapper = new HouseholdMapper(mapperType => mapperType == typeof(OwnerMapper) ? new OwnerMapper() : null);
            var target = mapper.Map(new HouseholdSource { OwnerName = "Ichabod", OwnerAge = 30 });

            target.Owner.Should().NotBeNull();
            target.Owner!.Name.Should().Be("Ichabod");
            target.Owner.Age.Should().Be(30);
        }

        [Fact]
        public void TestCompositeMapper_ThrowsWhenChildMapperNotResolved()
        {
            var mapper = new HouseholdMapper(mapperType => null);
            var act = () => mapper.Map(new HouseholdSource { OwnerName = "Ichabod", OwnerAge = 30 });

            act.Should().Throw<InvalidOperationException>();
        }

        public class PersonSource
        {
            public string Name { get; set; } = "";
            public int Age { get; set; }
        }

        public class PersonTarget : AccessibleObjectBase<PersonTarget>
        {
            public string Name { get; set; } = "";
            public int Age { get; set; }
        }

        public class PersonMapper : Mapper<PersonSource, PersonTarget>
        {
            public override PersonTarget Map(PersonSource source)
            {
                return new PersonTarget { Name = source.Name, Age = source.Age };
            }
        }

        public class HouseholdSource
        {
            public string OwnerName { get; set; } = "";
            public int OwnerAge { get; set; }
        }

        public class HouseholdTarget : AccessibleObjectBase<HouseholdTarget>
        {
            public PersonTarget? Owner { get; set; }
        }

        public class HouseholdMapper : CompositeMapper<HouseholdSource, HouseholdTarget>
        {
            public HouseholdMapper(MapperFactory<HouseholdSource> mapperFactory) : base(mapperFactory) { }

            public override HouseholdTarget Map(HouseholdSource source)
            {
                var owner = (PersonTarget)MapChild<OwnerMapper>(source);
                return new HouseholdTarget { Owner = owner };
            }
        }

        public class OwnerMapper : Mapper<HouseholdSource, PersonTarget>
        {
            public override PersonTarget Map(HouseholdSource source)
            {
                return new PersonTarget { Name = source.OwnerName, Age = source.OwnerAge };
            }
        }
    }
}
