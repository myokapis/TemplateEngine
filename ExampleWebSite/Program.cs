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

using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using TemplateEngine.AspNetCore.Extensions;

namespace ExampleWebSite
{
    public class Program
    {
        public static int Main(string[] args)
        {
            // setup a bootstrap logger to capture output until the real logger is configured
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateBootstrapLogger();

            try
            {
                CreateHostBuilder(args).Build().Run();
                return 0;
            }
            catch(Exception ex)
            {
                Log.Fatal(ex, "Example app failed to start.");
                return ex.HResult;
            }
            finally
            {
                Log.CloseAndFlush();
            }
            
        }

        /// <summary>
        /// Workhorse method to create a default host builder, register the Serilog logger
        /// for dependency injection, and register Template Engine components for dependency
        /// injection.
        /// </summary>
        /// <param name="args">The command line arguments</param>
        /// <returns>An IHostBuilder instance</returns>
        /// <remarks>The Template Engine components that are registered are 1) TemplateLoader,
        /// 2) TemplateCache 3) TemplateEngineSettings 4) All classes that descend from the
        /// MasterPresenterBase class. Additionally, the template directory is set to the path
        /// provided in the TemplateEngineSettings configuration section or to the default path
        /// if no usable configuration is found.</remarks>
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog((context, serilogConfig) => serilogConfig.ReadFrom.Configuration(context.Configuration))
                .UseTemplateEngine()
                .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>());
    }
}
