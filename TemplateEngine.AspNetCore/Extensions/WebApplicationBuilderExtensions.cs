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

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TemplateEngine.Web;

namespace TemplateEngine.AspNetCore.Extensions
{

    /// <summary>
    /// Template Engine-specific extension methods for WebApplicationBuilder
    /// </summary>
    public static class WebApplicationBuilderExtensions
    {
        /// <summary>
        /// Sets up configuration and dependency injection for using Template Engine in ASP.Net Core
        /// </summary>
        /// <param name="builder">The builder setting up the host in which Template Engine will be used</param>
        /// <returns>A copy of the builder</returns>
        /// <remarks>Registers presenters that inherit from MasterPresenterBase</remarks>
        public static WebApplicationBuilder UseTemplateEngine(this WebApplicationBuilder builder)
        {
            builder.UseTemplateEngine<MasterPresenterBase>();

            return builder;
        }

        /// <summary>
        /// Sets up configuration and dependency injection for using Template Engine in ASP.Net Core
        /// </summary>
        /// <typeparam name="T">The base type for presenters to be automatically registered</typeparam>
        /// <param name="builder">The builder setting up the host in which Template Engine will be used</param>
        /// <returns>A copy of the builder</returns>
        /// <remarks>If caching is configured, then both the template loader and template cache are
        /// set up in DI. This allows presenters to make use of either object for acquiring a template.</remarks>
        public static WebApplicationBuilder UseTemplateEngine<T>(this WebApplicationBuilder builder)
        {
            var services = builder.Services;
            
            var settings = SettingsFactory.GetSettingsFromConfig(builder.Configuration) ??
                SettingsFactory.GetDefaultSettings(builder.Environment);

            services.AddTemplateSettings(settings);
            services.TryAddSingleton<SettingsFactory>();

            if (settings.UseCache)
                services.AddTemplateCache();

            services.AddTemplateLoader();
            services.AddPresenters<T>();

            return builder;
        }
    }
}
