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

using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MyOkapis.TemplateEngine.Loader;
using MyOkapis.TemplateEngine.Web;

namespace MyOkapis.TemplateEngine.AspNetCore.Extensions
{

    /// <summary>
    /// Template Engine-specific extension methods for IServiceCollection
    /// </summary>
    public static class IServiceCollectionExtensions
    {

        /// <summary>
        /// Automatically registers all presenters that inherit from MasterPresenterBase, found in
        /// the calling assembly.
        /// </summary>
        /// <param name="services">The DI collection of registered services in which presenters will be registered</param>
        /// <returns>The service collection</returns>
        public static IServiceCollection AddPresenters(this IServiceCollection services)
        {
            return services.AddPresenters(Assembly.GetCallingAssembly(), typeof(MasterPresenterBase));
        }

        /// <summary>
        /// Automatically registers all presenters that inherit from MasterPresenterBase, found in
        /// the assembly containing <typeparamref name="TAssemblyMarker"/>.
        /// </summary>
        /// <typeparam name="TAssemblyMarker">Any type declared in the assembly to be searched for presenters.</typeparam>
        /// <param name="services">The DI collection of registered services in which presenters will be registered</param>
        /// <returns>The service collection</returns>
        public static IServiceCollection AddPresenters<TAssemblyMarker>(this IServiceCollection services)
        {
            return services.AddPresenters<TAssemblyMarker, MasterPresenterBase>();
        }

        /// <summary>
        /// Automatically registers all presenters that inherit from <typeparamref name="TBase"/>,
        /// found in the assembly containing <typeparamref name="TAssemblyMarker"/>.
        /// </summary>
        /// <typeparam name="TAssemblyMarker">Any type declared in the assembly to be searched for presenters.</typeparam>
        /// <typeparam name="TBase">The base type for presenters to be registered</typeparam>
        /// <param name="services">The DI collection of registered services in which presenters will be registered</param>
        /// <returns>The service collection</returns>
        public static IServiceCollection AddPresenters<TAssemblyMarker, TBase>(this IServiceCollection services)
        {
            return services.AddPresenters(typeof(TAssemblyMarker).Assembly, typeof(TBase));
        }

        /// <summary>
        /// Automatically registers all non-abstract subclasses of any of the given base types,
        /// found in the given assembly, as scoped services.
        /// </summary>
        /// <remarks>
        /// The assembly must be provided explicitly (or resolved from a marker type) rather than
        /// inferred via <see cref="Assembly.GetEntryAssembly"/>, which only returns useful results
        /// when the presenter types are compiled directly into the application's actual entry
        /// assembly -- it returns something else entirely (e.g. the test host, under a unit test
        /// runner) in every other hosting scenario, silently registering nothing.
        /// </remarks>
        /// <param name="services">The DI collection of registered services in which presenters will be registered</param>
        /// <param name="assembly">The assembly to search for presenter subclasses</param>
        /// <param name="baseTypes">The base types used in the search</param>
        /// <returns>The service collection</returns>
        public static IServiceCollection AddPresenters(this IServiceCollection services, Assembly assembly, params Type[] baseTypes)
        {
            ArgumentNullException.ThrowIfNull(assembly);

            // choose only non-abstract classes that descend from (but aren't themselves) one of the base types
            var subClasses = assembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract
                    && baseTypes.Any(baseType => t != baseType && baseType.IsAssignableFrom(t)));

            foreach (var subClass in subClasses)
            {
                services.AddScoped(subClass);
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
                // Resolve TemplateEngineSettings directly rather than via SettingsFactory.Settings:
                // SettingsFactory independently recomputes settings from IConfiguration/IHostEnvironment,
                // which ignores an explicit TemplateEngineSettings instance passed to AddTemplateEngine.
                // TemplateEngineSettings is always registered first (see AddTemplateSettings), and
                // reflects whichever settings -- explicit or convention-derived -- were actually chosen.
                var settings = serviceProvider.GetService<TemplateEngineSettings>()!;

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
                // See AddTemplateCache: resolve TemplateEngineSettings directly, not via
                // SettingsFactory.Settings, so an explicit settings instance is honored.
                var settings = serviceProvider.GetService<TemplateEngineSettings>()!;

                return new WebLoader(settings.TemplateDirectory);

            });

            return services;
        }

        /// <summary>
        /// Adds the necessary Template Engine classes to dependency injection. Presenters descending
        /// from MasterPresenterBase are discovered from the calling assembly.
        /// </summary>
        /// <param name="services">The DI collection of registered services in which template engine classes will be registered</param>
        /// <param name="configuration">The configuration for this context</param>
        /// <param name="settings">Optional settings to be registered</param>
        /// <returns>The service collection</returns>
        public static IServiceCollection AddTemplateEngine(this IServiceCollection services,
            IConfiguration? configuration = null, TemplateEngineSettings? settings = null)
        {
            return services.AddTemplateEngineCore(Assembly.GetCallingAssembly(), typeof(MasterPresenterBase), configuration, settings);
        }

        /// <summary>
        /// Adds the necessary Template Engine classes to dependency injection. Presenters descending
        /// from MasterPresenterBase are discovered from the assembly containing <typeparamref name="TAssemblyMarker"/>.
        /// </summary>
        /// <typeparam name="TAssemblyMarker">Any type declared in the assembly to be searched for presenters.</typeparam>
        /// <param name="services">The DI collection of registered services in which template engine classes will be registered</param>
        /// <param name="configuration">The configuration for this context</param>
        /// <param name="settings">Optional settings to be registered</param>
        /// <returns>The service collection</returns>
        public static IServiceCollection AddTemplateEngine<TAssemblyMarker>(this IServiceCollection services,
            IConfiguration? configuration = null, TemplateEngineSettings? settings = null)
        {
            return services.AddTemplateEngine<TAssemblyMarker, MasterPresenterBase>(configuration, settings);
        }

        /// <summary>
        /// Adds the necessary Template Engine classes to dependency injection. Presenters descending
        /// from <typeparamref name="TBase"/> are discovered from the assembly containing
        /// <typeparamref name="TAssemblyMarker"/>.
        /// </summary>
        /// <typeparam name="TAssemblyMarker">Any type declared in the assembly to be searched for presenters.</typeparam>
        /// <typeparam name="TBase">The base presenter type for which all descendants will be registered</typeparam>
        /// <param name="services">The DI collection of registered services in which template engine classes will be registered</param>
        /// <param name="configuration">The configuration for this context</param>
        /// <param name="settings">Optional settings to be registered</param>
        /// <returns>The service collection</returns>
        public static IServiceCollection AddTemplateEngine<TAssemblyMarker, TBase>(this IServiceCollection services,
            IConfiguration? configuration, TemplateEngineSettings? settings = null)
        {
            return services.AddTemplateEngineCore(typeof(TAssemblyMarker).Assembly, typeof(TBase), configuration, settings);
        }

        private static IServiceCollection AddTemplateEngineCore(this IServiceCollection services,
            Assembly presenterAssembly, Type presenterBaseType, IConfiguration? configuration, TemplateEngineSettings? settings)
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

            services.AddPresenters(presenterAssembly, presenterBaseType);

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
            services.TryAddSingleton(serviceProvider => serviceProvider.GetService<SettingsFactory>()!.Settings);
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
