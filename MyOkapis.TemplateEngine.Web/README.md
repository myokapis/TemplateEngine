# MyOkapis.TemplateEngine.Web

HTML-specific extensions for
[MyOkapis.TemplateEngine](https://github.com/myokapis/TemplateEngine/tree/master/MyOkapis.TemplateEngine):
a `WebWriter` that HTML-encodes bound values and understands checkboxes and dropdowns, plus a
`MasterPresenterBase` for composing a master/content page layout.

## Installation

This package is published to GitHub Packages, not nuget.org. Add the feed once per machine:

```bash
dotnet nuget add source https://nuget.pkg.github.com/myokapis/index.json -n github -u <github-username> -p <personal-access-token> --store-password-in-clear-text
```

Then:

```bash
dotnet add package MyOkapis.TemplateEngine.Web
```

## Checkboxes and dropdowns

`FieldDefinitions` tells `WebWriter` which bound fields are checkboxes (rendered `checked` when the
value is a recognized "true" token) and which are dropdowns (rendered as `<option>` lists):

```csharp
using MyOkapis.TemplateEngine.Document;
using MyOkapis.TemplateEngine.Web;

var writer = new WebWriter(new Template(templateText));
var definitions = new FieldDefinitions("Selected");   // "Selected" is a checkbox field

writer.SetMultiSectionFields("ITEMS", items, definitions);
```

Dropdown fields are configured with a `DropdownDefinition` (field name, section name, and the
`Option` data to render) and passed the same way.

## Master/content pages

`MasterPresenterBase` composes a master template with a content template, letting the content
page provide sections (e.g. `HEAD`, `BODY`) that the master template renders into:

```csharp
using MyOkapis.TemplateEngine.Web;

public class MyPresenter : MasterPresenterBase
{
    public MyPresenter(ITemplateLoader<IWebWriter> templateLoader) : base(templateLoader) { }

    public async Task<string> Render()
    {
        await SetupWriters("Master.tpl", "Content.tpl");
        WriteMasterSections();
        return GetContent();
    }
}
```

## Related packages

- [MyOkapis.TemplateEngine](https://github.com/myokapis/TemplateEngine/tree/master/MyOkapis.TemplateEngine) — the core engine this package extends
- [MyOkapis.TemplateEngine.AspNetCore](https://github.com/myokapis/TemplateEngine/tree/master/MyOkapis.TemplateEngine.AspNetCore) — wires this package into ASP.NET Core dependency injection

## License

Apache License, Version 2.0. See [LICENSE.md](https://github.com/myokapis/TemplateEngine/blob/master/LICENSE.md).
