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

using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace TemplateEngine.AspNetCore
{

    /// <summary>
    /// A factory class for obtaining Template Engine settings
    /// </summary>
    /// <remarks>Configuration takes precedence over convention</remarks>
    public class SettingsFactory
    {

        /// <summary>
        /// Instantiates the factory using the host environment and configuration
        /// </summary>
        /// <param name="environment">The host environment for this context</param>
        /// <param name="configuration">The configuration settings for this context</param>
        public SettingsFactory(IHostEnvironment environment, IConfiguration configuration)
        {
            Settings = GetSettingsFromConfig(configuration) ?? GetDefaultSettings(environment);
        }

        /// <summary>
        /// A read-only property for obtaining the settings from the factory
        /// </summary>
        public TemplateEngineSettings Settings { get; }

        /// <summary>
        /// A helper method for creating settings for the host environment
        /// </summary>
        /// <param name="environment">The host environment for this context</param>
        /// <returns>Template Engine settings based on convention</returns>
        internal static TemplateEngineSettings GetDefaultSettings(IHostEnvironment environment)
        {
            return new TemplateEngineSettings
            {
                TemplateDirectory = Path.Combine(environment.ContentRootPath, TemplateEngineSettings.DefaultDirectoryName)
            };
        }

        /// <summary>
        /// A helper method for getting the settings from the configuration
        /// </summary>
        /// <param name="configuration">The configuration for this context</param>
        /// <returns>Template Engine settings based on configuration</returns>
        internal static TemplateEngineSettings GetSettingsFromConfig(IConfiguration configuration)
        {
            // look for the settings in the config
            var configSection = configuration.GetSection(nameof(TemplateEngine));
            return configSection.Get<TemplateEngineSettings>();
        }

    }

}
