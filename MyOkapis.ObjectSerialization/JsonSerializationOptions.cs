/* ****************************************************************************
Copyright 2018-2026 Gene Graves

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

using System.Text.Json;
using System.Text.Json.Serialization;

namespace MyOkapis.ObjectSerialization
{

    /// <summary>
    /// Options controlling JSON serialization.
    /// </summary>
    public class JsonSerializationOptions : SerializationOptions
    {
        /// <summary>
        /// Constructs the default JsonSerializationOptions.
        /// </summary>
        public JsonSerializationOptions() : this(NullHandlingOptions.SuppressNull) { }

        /// <summary>
        /// Constructs JsonSerializationOptions with the given null-handling behavior.
        /// </summary>
        /// <param name="nullHandling">Controls how null field values are represented.</param>
        public JsonSerializationOptions(NullHandlingOptions nullHandling)
        {
            NullHandling = nullHandling;

            SystemJsonSerializerOptions = new JsonSerializerOptions()
            {
                DefaultIgnoreCondition = nullHandling == NullHandlingOptions.SuppressNull
                    ? JsonIgnoreCondition.WhenWritingNull
                    : JsonIgnoreCondition.Never,
                PropertyNameCaseInsensitive = true,
                NumberHandling = JsonNumberHandling.AllowReadingFromString,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false
            };
        }

        /// <summary>
        /// Constructs JsonSerializationOptions from explicit System.Text.Json options.
        /// </summary>
        /// <param name="jsonSerializerOptions">The underlying System.Text.Json options to use.</param>
        /// <param name="nullHandling">Controls how null field values are represented.</param>
        /// <exception cref="ArgumentException">Thrown if the null-handling setting conflicts with the supplied JSON options.</exception>
        public JsonSerializationOptions(JsonSerializerOptions jsonSerializerOptions, NullHandlingOptions nullHandling)
        {
            if (jsonSerializerOptions.DefaultIgnoreCondition == JsonIgnoreCondition.WhenWritingNull
                && nullHandling != NullHandlingOptions.SuppressNull)
                throw new ArgumentException("Null handling conflicts with Json serialization options.");

            if (jsonSerializerOptions.DefaultIgnoreCondition == JsonIgnoreCondition.Never
                && nullHandling == NullHandlingOptions.SuppressNull)
                throw new ArgumentException("Null handling conflicts with Json serialization options.");

            SystemJsonSerializerOptions = jsonSerializerOptions;
            NullHandling = nullHandling;
        }

        /// <summary>
        /// When true, nested object properties are flattened into the top-level JSON object
        /// instead of being written as nested JSON objects.
        /// </summary>
        public bool Flatten { get; init; } = false;

        /// <summary>
        /// When true, each field's FormatAttribute (if any) is applied before serialization.
        /// </summary>
        public bool Format { get; init; } = false;

        /// <summary>
        /// Controls how null field values are represented.
        /// </summary>
        public NullHandlingOptions NullHandling { get; init; }

        /// <summary>
        /// The underlying System.Text.Json options used for serialization.
        /// </summary>
        public JsonSerializerOptions SystemJsonSerializerOptions { get; init; }
    }

}
