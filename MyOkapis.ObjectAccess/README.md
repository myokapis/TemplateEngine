# MyOkapis.ObjectAccess

Cached, reflection-based access to a type's properties and their custom attributes — the
foundation other MyOkapis packages (and your own code) use to discover a POCO's fields once and
reuse that metadata cheaply, rather than reflecting on every access.

Two usage patterns are supported:

- **Wrapper** (`AccessibleObject<T>`) — for types you don't own, or don't want coupled to this
  library. Works with any type, including third-party DTOs and EF entities.
- **Direct implementation** (`AccessibleObjectBase<T>`) — for types you own and want to expose
  their own field metadata directly, including polymorphic access across different accessible
  types and static metadata with no instance in hand.

Both patterns share the same cached field list per type, computed once via `Lazy<>`.

## Installation

This package is published to GitHub Packages, not nuget.org. Add the feed once per machine:

```bash
dotnet nuget add source https://nuget.pkg.github.com/myokapis/index.json -n github -u <github-username> -p <personal-access-token> --store-password-in-clear-text
```

Then:

```bash
dotnet add package MyOkapis.ObjectAccess
```

## Usage: wrapping a type you don't own

```csharp
using MyOkapis.ObjectAccess;

public class Customer
{
    public string Name { get; set; } = "";
    public int Age { get; set; }
}

var customer = new Customer { Name = "Ichabod", Age = 30 };
var accessible = new AccessibleObject<Customer>(customer);

accessible.Count;              // 2
accessible["Name"];            // "Ichabod"

foreach (var (field, value) in accessible.Values)
{
    Console.WriteLine($"{field.Name} = {value}");
}
```

## Usage: implementing directly on a type you own

```csharp
using MyOkapis.ObjectAccess;

public class Customer : AccessibleObjectBase<Customer>
{
    public string Name { get; set; } = "";
    public int Age { get; set; }
}

var customer = new Customer { Name = "Ichabod", Age = 30 };

customer.FieldCount;            // 2, no wrapper needed
Customer.Fields;                // field metadata with no instance at all

// polymorphic access across different accessible types
IAccessibleObject accessible = customer;
accessible.FieldDescriptors;
```

## Looking up custom attributes on a field

`FieldDescriptor.GetAttribute<T>()` looks up any attribute type (or interface implemented by an
attribute), not just ones this library knows about — this is what lets
[MyOkapis.Formatting](https://github.com/myokapis/TemplateEngine/tree/master/MyOkapis.Formatting)
and other packages layer their own attribute-driven behavior on top without `ObjectAccess` itself
needing to know they exist.

```csharp
var field = Customer.Fields.First(f => f.Name == "Name");
var someAttribute = field.GetAttribute<SomeCustomAttribute>();
```

## Related packages

- [MyOkapis.Formatting](https://github.com/myokapis/TemplateEngine/tree/master/MyOkapis.Formatting) — attribute-driven value formatting
- [MyOkapis.ObjectMapping](https://github.com/myokapis/TemplateEngine/tree/master/MyOkapis.ObjectMapping) — mapping a source object onto an accessible target
- [MyOkapis.ObjectSerialization](https://github.com/myokapis/TemplateEngine/tree/master/MyOkapis.ObjectSerialization) — serializing an accessible object to text/JSON/XML

## License

Apache License, Version 2.0. See [LICENSE.md](https://github.com/myokapis/TemplateEngine/blob/master/LICENSE.md).
