# MyOkapis.Formatting

Attribute-driven value formatting, decoupled from how the value was obtained. A format attribute
is pure metadata (`FormatString`, `FormatProvider`, `MinLength`, `MaxLength`, `Justify`,
`PaddingChar`); an `IFormatter` service does the actual conversion. There's no dependency between
the two — you can format a raw value with no reflection, no POCO, and no attribute at all.

## Installation

This package is published to GitHub Packages, not nuget.org. Add the feed once per machine:

```bash
dotnet nuget add source https://nuget.pkg.github.com/myokapis/index.json -n github -u <github-username> -p <personal-access-token> --store-password-in-clear-text
```

Then:

```bash
dotnet add package MyOkapis.Formatting
```

## Formatting a value directly

```csharp
using MyOkapis.Formatting;

var attribute = new FormatDateAttribute("yyyy-MM-dd");
Formatter.Default.Format(DateTime.Now, attribute);   // "2026-01-15"

// no attribute at all -- falls back to CultureInfo.CurrentCulture, matching value.ToString()
Formatter.Default.Format(42, null);                  // "42"
```

## Decorating a model property

```csharp
using MyOkapis.Formatting;

public class Invoice
{
    [FormatCurrency("C2")]
    public decimal Total { get; set; }

    [FormatDate("MM/dd/yyyy")]
    public DateTime Date { get; set; }
}
```

Any consumer that reflects the property (e.g.
[MyOkapis.ObjectAccess](https://github.com/myokapis/TemplateEngine/tree/master/MyOkapis.ObjectAccess)'s
`FieldDescriptor.GetAttribute<IFormatAttribute>()`) can retrieve the attribute and hand it to
`IFormatter` without knowing which concrete attribute type it is.

## Padding, justification, and truncation

`MinLength`/`MaxLength`/`Justify`/`PaddingChar` are applied automatically by `Formatter` after the
format-string conversion — useful for fixed-width text output (flat files, reports):

```csharp
public class FixedWidthField : FormatAttribute
{
    public FixedWidthField(int width)
    {
        FixedLength = width;   // MinLength/MaxLength/FixedLength use a regular setter
    }
}

// Justify/PaddingChar are init-only, set via object initializer syntax
var attr = new FixedWidthField(10) { Justify = Justify.Left, PaddingChar = ' ' };
Formatter.Default.Format("hi", attr);   // "hi        " (10 chars)
```

## Built-in format attributes

`FormatCurrencyAttribute`, `FormatDateAttribute`, `FormatIntegerAttribute`, `FormatNumberAttribute`,
`FormatPercentAttribute` — each accepts either a format string, a `NumberFormatInfo`/
`DateTimeFormatInfo`, or (for currency/number/percent) explicit decimal-places/negative-pattern/
separator parameters.

## Related packages

- [MyOkapis.ObjectAccess](https://github.com/myokapis/TemplateEngine/tree/master/MyOkapis.ObjectAccess) — cached reflection-based property access
- [MyOkapis.ObjectSerialization](https://github.com/myokapis/TemplateEngine/tree/master/MyOkapis.ObjectSerialization) — uses this package to format fields during serialization
- [MyOkapis.TemplateEngine](https://github.com/myokapis/TemplateEngine/tree/master/MyOkapis.TemplateEngine) — uses this package to format bound template fields

## License

Apache License, Version 2.0. See [LICENSE.md](https://github.com/myokapis/TemplateEngine/blob/master/LICENSE.md).
