# MyOkapis.TemplateEngine

Parses text templates and populates template fields with formatted data. Developed for rendering
HTML, but usable for any text document where data needs to be merged into markup — sections,
nested sections, and repeating markup (tables, lists) are all first-class concepts.

This package is the core engine only: template parsing, the writer, and file/cache loading. HTML-
specific helpers (checkboxes, dropdowns, presenter base classes) live in
[MyOkapis.TemplateEngine.Web](https://github.com/myokapis/TemplateEngine/tree/master/MyOkapis.TemplateEngine.Web).
For the full walkthrough (template markup syntax, field providers, partial templates, options,
debugging tips), see the
[project README](https://github.com/myokapis/TemplateEngine/blob/master/README.md) — this package
README is a quick reference.

## Installation

This package is published to GitHub Packages, not nuget.org. Add the feed once per machine:

```bash
dotnet nuget add source https://nuget.pkg.github.com/myokapis/index.json -n github -u <github-username> -p <personal-access-token> --store-password-in-clear-text
```

Then:

```bash
dotnet add package MyOkapis.TemplateEngine
```

## Template markup

```html
<h1>Example</h1>
<table>
  <tbody>
    <!-- @@ROW@@ -->
    <tr>
      <td>@@FirstName@@</td>
      <td>@@LastName@@</td>
    </tr>
    <!-- @@ROW@@ -->
  </tbody>
</table>
```

`@@Field@@` marks a replaceable field. A pair of `<!-- @@SECTION@@ -->` comments marks the
beginning and end of a section, which can be selected, bound with data, and appended zero or more
times (or nested, for hierarchical documents).

## Basic usage

```csharp
using MyOkapis.TemplateEngine.Document;
using MyOkapis.TemplateEngine.Writer;

var template = new Template(templateText);
var writer = new TemplateWriter(template);

var people = new List<Person>
{
    new() { FirstName = "Ichabod", LastName = "Crane" },
    new() { FirstName = "Alex", LastName = "Rodriguez" }
};

// binds each object's fields to the section by name convention and appends once per item
writer.SetMultiSectionFields("ROW", people);

string html = writer.GetContent();
```

## Formatting bound values

Decorate model properties with attributes from
[MyOkapis.Formatting](https://github.com/myokapis/TemplateEngine/tree/master/MyOkapis.Formatting)
to control how they're rendered:

```csharp
using MyOkapis.Formatting;

public class Invoice
{
    [FormatCurrency("C2")]
    public decimal Total { get; set; }
}
```

## Caching templates

```csharp
using MyOkapis.TemplateEngine.Loader;

var cache = new TemplateCache(templateDirectory);
var writer = await cache.GetWriterAsync("Invoice.tpl");
```

## Related packages

- [MyOkapis.TemplateEngine.Web](https://github.com/myokapis/TemplateEngine/tree/master/MyOkapis.TemplateEngine.Web) — HTML-specific writer extensions
- [MyOkapis.TemplateEngine.AspNetCore](https://github.com/myokapis/TemplateEngine/tree/master/MyOkapis.TemplateEngine.AspNetCore) — ASP.NET Core dependency injection
- [MyOkapis.ObjectAccess](https://github.com/myokapis/TemplateEngine/tree/master/MyOkapis.ObjectAccess) / [MyOkapis.Formatting](https://github.com/myokapis/TemplateEngine/tree/master/MyOkapis.Formatting) — the field-binding and formatting foundation this package builds on

## License

Apache License, Version 2.0. See [LICENSE.md](https://github.com/myokapis/TemplateEngine/blob/master/LICENSE.md).
