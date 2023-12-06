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

//using System;
//using Microsoft.AspNetCore.Hosting;
//using Microsoft.Extensions.Hosting;
using System;
using ExampleWebSite.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using TemplateEngine.AspNetCore.Extensions;

// setup a bootstrap logger to capture output until the real logger is configured
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    var services = builder.Services;
    services.AddControllers();
    services.AddTemplateEngine();
    services.AddSingleton<IDataService, DataService>();

    builder.Host.UseSerilog((context, serilogConfig) => serilogConfig.ReadFrom.Configuration(context.Configuration));

    var app = builder.Build();

    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
        app.UseHsts();
    }

    app.UseStaticFiles();
    app.UseRouting();
    app.UseHttpsRedirection();
    app.UseSerilogRequestLogging();

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    app.Run();
        
    return 0;
}
catch (Exception ex)
{
    Log.Fatal(ex, "Example app failed to start.");
    return ex.HResult;
}
finally
{
    Log.CloseAndFlush();
}
