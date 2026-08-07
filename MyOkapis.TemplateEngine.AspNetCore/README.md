# MyOkapis.TemplateEngine.AspNetCore

Dependency injection extensions for using
[MyOkapis.TemplateEngine](https://github.com/myokapis/TemplateEngine/tree/master/MyOkapis.TemplateEngine)
in ASP.NET Core applications: registers the template cache/loader, settings, and automatically
discovers and registers presenter classes.

## Installation

This package is published to GitHub Packages, not nuget.org. Add the feed once per machine:

```bash
dotnet nuget add source https://nuget.pkg.github.com/myokapis/index.json -n github -u <github-username> -p <personal-access-token> --store-password-in-clear-text
```

Then:

```bash
dotnet add package MyOkapis.TemplateEngine.AspNetCore
```

## Usage with WebApplicationBuilder (.NET 6+ minimal hosting)

```csharp
using MyOkapis.TemplateEngine.AspNetCore.Extensions;

var builder = WebApplication.CreateBuilder(args);

// registers presenters descending from MasterPresenterBase, the template cache,
// and settings resolved from configuration (falling back to convention)
builder.UseTemplateEngine();

var app = builder.Build();
```

## Configuration

By convention, templates are expected in a `Templates` directory under the content root. Override
via an `appsettings.json` `TemplateEngine` section:

```json
{
  "TemplateEngine": {
    "TemplateDirectory": "Views/Templates",
    "UseCache": true
  }
}
```

## Registering only what you need

```csharp
using MyOkapis.TemplateEngine.AspNetCore.Extensions;

builder.Services.AddTemplateSettings();
builder.Services.AddTemplateCache();     // or AddTemplateLoader() to skip caching

// AddPresenters needs to know which assembly to search -- it can't reliably guess. Pass any type
// declared in your own application assembly as a marker (Program works well for this).
builder.Services.AddPresenters<Program, MyPresenterBase>();
```

## Presenter discovery

`AddPresenters`/`AddTemplateEngine`/`UseTemplateEngine` need to know which assembly to search for
presenter classes. Since there's no reliable way to infer "the consumer's application assembly"
automatically (an earlier version tried `Assembly.GetEntryAssembly()`, which only works when
presenters are compiled directly into the actual process entry point — it silently finds nothing
under a test host, IIS in-process hosting, or any setup where the entry assembly differs from
where your presenters live), you have two options:

```csharp
// Zero-arg overloads use Assembly.GetCallingAssembly() -- correct as long as you call
// AddTemplateEngine/UseTemplateEngine/AddPresenters directly from your own application code
builder.UseTemplateEngine();

// Or be explicit with a marker type from your own assembly, which also works if you're
// calling through a shared configuration helper in a different assembly
builder.UseTemplateEngine<Program>();                    // MasterPresenterBase subclasses
builder.UseTemplateEngine<Program, MyPresenterBase>();    // MyPresenterBase subclasses
```

## Related packages

- [MyOkapis.TemplateEngine](https://github.com/myokapis/TemplateEngine/tree/master/MyOkapis.TemplateEngine) — the core engine
- [MyOkapis.TemplateEngine.Web](https://github.com/myokapis/TemplateEngine/tree/master/MyOkapis.TemplateEngine.Web) — the HTML writer and presenter base class this package registers

## License

Apache License, Version 2.0. See [LICENSE.md](https://github.com/myokapis/TemplateEngine/blob/master/LICENSE.md).
