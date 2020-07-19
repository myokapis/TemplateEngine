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

namespace TemplateEngine.AspNetCore
{

    /// <summary>
    /// Options for configuring Template Engine
    /// </summary>
    public class TemplateEngineSettings
    {
        
        /// <summary>
        /// The full path to the directory from which templates can be read
        /// </summary>
        public string TemplateDirectory { get; set; }

        /// <summary>
        /// Enables or disables caching in the template loader
        /// </summary>
        public bool UseCache { get; set; } = true;

        /// <summary>
        /// A default templates directory name to support configuration by convention
        /// </summary>
        internal static string DefaultDirectoryName { get; } = nameof(Templates);

        /// <summary>
        /// A helper property representing the default directory name
        /// </summary>
        private static object Templates { get; }
    }

}
