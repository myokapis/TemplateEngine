/* ****************************************************************************
Copyright 2018-2022 Gene Graves

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

using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TemplateEngine.Document;
using TemplateEngine.Loader;
using TemplateEngine.Web;

namespace TemplateEngine.AspNetCore.Extensions
{

    /// <summary>
    /// Template Engine-specific xtension methods for IServiceCollection
    /// </summary>
    public static class IServiceCollectionExtensions
    {

        /// <summary>
        /// Automatically registers all presenters that inherit from MasterPresenterBase
        /// </summary>
        /// <param name="services">The DI collection of registered services in which presenters will be registered</param>
        /// <returns>The service collection</returns>
        public static IServiceCollection AddPresenters(this IServiceCollection services)
        {
            return services.AddPresenters<MasterPresenterBase>();
        }

        /// <summary>
        /// Automatically registers all presenters that inherit from TBase
        /// </summary>
        /// <typeparam name="TBase">The base type for presenters to be registered</typeparam>
        /// <param name="services">The DI collection of registered services in which presenters will be registered</param>
        /// <returns>The service collection</returns>
        public static IServiceCollection AddPresenters<TBase>(this IServiceCollection services)
        {

            // reflect all classes in assembly to find and add all that implement the base class or interface
            var baseType = typeof(TBase);
            var assembly = Assembly.GetEntryAssembly();

            // choose only classes and ignore the base class
            var subClasses = assembly.GetTypes()
                .Where(t => t != baseType && t.IsClass && baseType.IsAssignableFrom(t));
            
            foreach(var subClass in subClasses)
            {
                    services.AddTransient(subClass);
            }

            return services;
        }

        /// <summary>
        /// Creates a template cache based on the configuration and registers it
        /// </summary>
        /// <param name="services">The DI collection of registered services in which the cache will be registered</param>
        /// <returns>The service collection</returns>
        public static IServiceCollection AddTemplateCache(this IServiceCollection services)
        {
            services.TryAddSingleton<ITemplateCache<IWebWriter>>(serviceProvider =>
            {
                var settings = serviceProvider
                    .GetService<SettingsFactory>()
                    .Settings;

                return new WebTemplateCache(settings.TemplateDirectory);
            });

            return services;
        }

        /// <summary>
        /// Creates a template loader based on the configuration and registers it
        /// </summary>
        /// <param name="services">The DI collection of registered services in which the loader will be registered</param>
        /// <returns>The service collection</returns>
        public static IServiceCollection AddTemplateLoader(this IServiceCollection services)
        {
            services.TryAddSingleton<ITemplateLoader<IWebWriter>>(serviceProvider =>
            {
                var settings = serviceProvider
                    .GetService<SettingsFactory>()
                    .Settings;

                return new TemplateLoader<IWebWriter>(
                    settings.TemplateDirectory,
                    (text) => new Template(text),
                    (template) => new WebWriter(template));

            });

            return services;
        }

        /// <summary>
        /// Adds the necessary Template Engine classes to dependency injection
        /// </summary>
        /// <param name="services">The DI collection of registered services in which template engine classes will be registered</param>
        /// <param name="configuration">The configuration for this context</param>
        /// <param name="settings">Optional settings to be registered</param>
        /// <returns>The service collection</returns>
        public static IServiceCollection AddTemplateEngine(this IServiceCollection services,
            IConfiguration configuration = null, TemplateEngineSettings settings = null)
        {
            services.AddTemplateEngine<MasterPresenterBase>(configuration, settings);
            return services;
        }

        /// <summary>
        /// Adds the necessary Template Engine classes to dependency injection
        /// </summary>
        /// <typeparam name="T">The base presenter type for which all descendants will be registered</typeparam>
        /// <param name="services">The DI collection of registered services in which template engine classes will be registered</param>
        /// <param name="configuration">The configuration for this context</param>
        /// <param name="settings">Optional settings to be registered</param>
        /// <returns>The service collection</returns>
        public static IServiceCollection AddTemplateEngine<T>(this IServiceCollection services,
            IConfiguration configuration, TemplateEngineSettings settings = null)
        {
            var settingsInstance = settings;
            services.TryAddSingleton<SettingsFactory>();

            if (settings == null && configuration != null)
                settingsInstance = SettingsFactory.GetSettingsFromConfig(configuration);

            if (settingsInstance == null)
                services.AddTemplateSettings();
            else
                services.AddTemplateSettings(settingsInstance);

            if (settings?.UseCache ?? true)
                services.AddTemplateCache();
            else
                services.AddTemplateLoader();

            services.AddPresenters<T>();

            return services;
        }

        /// <summary>
        /// Adds Template Engine settings from convention to dependency injection
        /// </summary>
        /// <param name="services">The DI collection of registered services in which settings will be registered</param>
        /// <returns>The service collection</returns>
        public static IServiceCollection AddTemplateSettings(this IServiceCollection services)
        {
            services.TryAddSingleton<SettingsFactory>();
            services.TryAddSingleton(serviceProvider => serviceProvider.GetService<SettingsFactory>().Settings);
            return services;
        }

        /// <summary>
        /// Adds Template Engine settings from configuration to dependency injection
        /// </summary>
        /// <param name="services">The DI collection of registered services in which settings will be registered</param>
        /// <param name="settings">The settings to be registered</param>
        /// <returns>The service collection</returns>
        public static IServiceCollection AddTemplateSettings(this IServiceCollection services, TemplateEngineSettings settings)
        {
            services.TryAddSingleton(settings);
            return services;
        }

    }

}
