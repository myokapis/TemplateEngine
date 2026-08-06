# MyOkapis.ObjectSerialization

Serializes an accessible object's fields to flat/delimited text, JSON, or XML, with optional field
masking and terminators. Builds on
[MyOkapis.ObjectAccess](https://github.com/myokapis/TemplateEngine/tree/master/MyOkapis.ObjectAccess)
for field discovery and
[MyOkapis.Formatting](https://github.com/myokapis/TemplateEngine/tree/master/MyOkapis.Formatting)
for value formatting — width, justification, and truncation are already handled by `Formatter`, so
the serializers here only add masking and structural concerns.

## Installation

This package is published to GitHub Packages, not nuget.org. Add the feed once per machine:

```bash
dotnet nuget add source https://nuget.pkg.github.com/myokapis/index.json -n github -u <github-username> -p <personal-access-token> --store-password-in-clear-text
```

Then:

```bash
dotnet add package MyOkapis.ObjectSerialization
```

## Flat/delimited text

Useful for fixed-width records, delimited rows, or log lines. Each field's `SerializationAttribute`
controls its terminator and whether sensitive values are masked:

```csharp
using MyOkapis.Formatting;
using MyOkapis.ObjectAccess;
using MyOkapis.ObjectSerialization;

public class LineItem : AccessibleObjectBase<LineItem>
{
    [SerializationAttribute(Terminator = "|")]
    public string Code { get; set; } = "";

    [SerializationAttribute(Terminator = "|", Masked = true, MaskChar = '*')]
    public string AccountNumber { get; set; } = "";
}

var serializer = new FlatTextSerializer(Formatter.Default);
var item = new LineItem { Code = "A1", AccountNumber = "12345" };

serializer.Serialize(item, new SerializationOptions());                        // "A1|12345|"
serializer.Serialize(item, new SerializationOptions { ApplyMasking = true });  // "A1|*****|"
```

Nested `IAccessibleObject` fields (and `IEnumerable<IAccessibleObject>` collections of them) are
serialized inline automatically.

## JSON

```csharp
using MyOkapis.ObjectSerialization;

var jsonSerializer = new JsonObjectSerializer(Formatter.Default);

// default options: SuppressNull, camelCase property names
jsonSerializer.Serialize(item, new JsonSerializationOptions());

// preserve nulls instead of omitting them
jsonSerializer.Serialize(item, new JsonSerializationOptions(NullHandlingOptions.PreserveNulls));

// mask sensitive fields
var options = new JsonSerializationOptions { ApplyMasking = true };
jsonSerializer.Serialize(item, options);
```

`JsonSerializationOptions.Flatten` collapses nested objects into the top-level JSON object instead
of nesting them — note that in flattened mode, field names are *not* camelCased, since the
underlying serialization target is a plain dictionary rather than a reflected class.

## XML

```csharp
using MyOkapis.ObjectSerialization;

var xmlSerializer = new XmlObjectSerializer(Formatter.Default);
xmlSerializer.Serialize(item, new XmlSerializationOptions());
```

Respects `System.Xml.Serialization`'s own `XmlElement`/`XmlIgnore`/`XmlAttribute` attributes for
field naming and exclusion, the same way `JsonObjectSerializer` respects `JsonPropertyName`/
`JsonIgnore`.

## Related packages

- [MyOkapis.ObjectAccess](https://github.com/myokapis/TemplateEngine/tree/master/MyOkapis.ObjectAccess) — the accessible-object concept this package serializes
- [MyOkapis.Formatting](https://github.com/myokapis/TemplateEngine/tree/master/MyOkapis.Formatting) — value formatting used by all three serializers
- [MyOkapis.ObjectMapping](https://github.com/myokapis/TemplateEngine/tree/master/MyOkapis.ObjectMapping) — mapping a source object into something this package can serialize

## License

Apache License, Version 2.0. See [LICENSE.md](https://github.com/myokapis/TemplateEngine/blob/master/LICENSE.md).
