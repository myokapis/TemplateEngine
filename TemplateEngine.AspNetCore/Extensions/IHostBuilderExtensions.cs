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

using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using TemplateEngine.Web;

namespace TemplateEngine.AspNetCore.Extensions
{

    /// <summary>
    /// Template Engine-specific extension methods for IHostBuilder
    /// </summary>
    /// <remarks>
    /// For .Net 6, IHostBuilder is no longer the recommended builder for web applications.
    /// It is strongly recommended that you use the WebApplicationBuilder and its
    /// TemplateEngine extensions instead of IHostBuilder and its extensions.
    /// </remarks>
    public static class IHostBuilderExtensions
    {

        /// <summary>
        /// Sets up configuration and dependency injection for using Template Engine in ASP.Net Core
        /// </summary>
        /// <param name="builder">The host builder setting up the host in which Template Engine will be used</param>
        /// <returns>A copy of the host builder</returns>
        /// <remarks>Registers presenter that inherit from MasterPresenterBase</remarks>
        public static IHostBuilder UseTemplateEngine(this IHostBuilder builder)
        {
            return builder.UseTemplateEngine<MasterPresenterBase>();
        }

        /// <summary>
        /// Sets up configuration and dependency injection for using Template Engine in ASP.Net Core
        /// </summary>
        /// <typeparam name="T">The base type for presenters to be automatically registered</typeparam>
        /// <param name="builder">The host builder setting up the host in which Template Engine will be used</param>
        /// <returns>A copy of the host builder</returns>
        /// <remarks>If caching is configured, then both the template loader and template cache are
        /// set up in DI. This allows presenters to make use of either object for acquiring a template.</remarks>
        public static IHostBuilder UseTemplateEngine<T>(this IHostBuilder builder)
        {
            return builder.ConfigureServices((context, services) =>
            {
                var settings = SettingsFactory.GetSettingsFromConfig(context.Configuration) ??
                    SettingsFactory.GetDefaultSettings(context.HostingEnvironment);

                services.AddTemplateSettings(settings);
                services.TryAddSingleton<SettingsFactory>();

                if (settings.UseCache)
                    services.AddTemplateCache();

                services.AddTemplateLoader();
                services.AddPresenters<T>();
            });
        }

    }

}
