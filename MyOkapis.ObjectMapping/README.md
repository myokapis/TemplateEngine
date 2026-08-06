# MyOkapis.ObjectMapping

Maps an arbitrary source object's data onto an accessible target object, with support for
delegating nested object graphs to other mappers. Depends only on
[MyOkapis.ObjectAccess](https://github.com/myokapis/TemplateEngine/tree/master/MyOkapis.ObjectAccess) —
mapping data *into* an object is kept decoupled from formatting or serializing it back *out*.

## Installation

This package is published to GitHub Packages, not nuget.org. Add the feed once per machine:

```bash
dotnet nuget add source https://nuget.pkg.github.com/myokapis/index.json -n github -u <github-username> -p <personal-access-token> --store-password-in-clear-text
```

Then:

```bash
dotnet add package MyOkapis.ObjectMapping
```

## Basic mapping

The target type implements `IAccessibleObject` (directly via `AccessibleObjectBase<T>`, or via the
`AccessibleObject<T>` wrapper from `MyOkapis.ObjectAccess`):

```csharp
using MyOkapis.ObjectAccess;
using MyOkapis.ObjectMapping;

public class CustomerRequest
{
    public string Name { get; set; } = "";
    public int Age { get; set; }
}

public class Customer : AccessibleObjectBase<Customer>
{
    public string Name { get; set; } = "";
    public int Age { get; set; }
}

public class CustomerMapper : Mapper<CustomerRequest, Customer>
{
    public override Customer Map(CustomerRequest source)
    {
        return new Customer { Name = source.Name, Age = source.Age };
    }
}

var mapper = new CustomerMapper();
Customer customer = mapper.Map(new CustomerRequest { Name = "Ichabod", Age = 30 });
```

`Map` returns the concrete target type directly. For polymorphic scenarios (e.g. resolving one of
several mapper types at runtime), the non-generic `IMapper<TSource>.Map` returns `IAccessibleObject`.

## Setting fields from a dictionary

```csharp
var target = new Customer { Name = "Original", Age = 1 };
mapper.SetFields(target, new Dictionary<string, object?> { { "Name", "Updated" } });
// target.Name == "Updated", target.Age unchanged -- only matching keys are set
```

## Delegating nested object graphs

`CompositeMapper<TSource, TTarget>` resolves child mappers through a `MapperFactory<TSource>`
delegate — plug in your DI container, or a simple type-switch:

```csharp
public class OrderSource
{
    public string OwnerName { get; set; } = "";
    public int OwnerAge { get; set; }
}

public class Order : AccessibleObjectBase<Order>
{
    public Customer? Owner { get; set; }
}

public class OwnerMapper : Mapper<OrderSource, Customer>
{
    public override Customer Map(OrderSource source) =>
        new() { Name = source.OwnerName, Age = source.OwnerAge };
}

public class OrderMapper : CompositeMapper<OrderSource, Order>
{
    public OrderMapper(MapperFactory<OrderSource> mapperFactory) : base(mapperFactory) { }

    public override Order Map(OrderSource source)
    {
        var owner = (Customer)MapChild<OwnerMapper>(source);
        return new Order { Owner = owner };
    }
}

// factory could just as easily resolve from a DI container
var orderMapper = new OrderMapper(mapperType => mapperType == typeof(OwnerMapper) ? new OwnerMapper() : null);
```

## Related packages

- [MyOkapis.ObjectAccess](https://github.com/myokapis/TemplateEngine/tree/master/MyOkapis.ObjectAccess) — the accessible-object concept this package maps onto
- [MyOkapis.ObjectSerialization](https://github.com/myokapis/TemplateEngine/tree/master/MyOkapis.ObjectSerialization) — serializing a mapped object to text/JSON/XML

## License

Apache License, Version 2.0. See [LICENSE.md](https://github.com/myokapis/TemplateEngine/blob/master/LICENSE.md).
