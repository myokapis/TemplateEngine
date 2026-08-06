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

using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using MyOkapis.Formatting;
using MyOkapis.ObjectAccess;
using PropertyStore = System.Collections.Generic.Dictionary<string, object?>;

namespace MyOkapis.ObjectSerialization
{

    /// <summary>
    /// Serializes an accessible object to a JSON string, honoring FormatAttribute (optionally),
    /// SerializationAttribute masking, and System.Text.Json's own JsonPropertyName/JsonIgnore
    /// attributes.
    /// </summary>
    public class JsonObjectSerializer : SerializerBase<JsonSerializationOptions>, IJsonObjectSerializer
    {
        /// <summary>
        /// Constructs an instance of the JSON object serializer.
        /// </summary>
        /// <param name="formatter">The formatter to be used when preparing field data.</param>
        public JsonObjectSerializer(IFormatter formatter) : base(formatter) { }

        /// <inheritdoc />
        public override string Serialize(IAccessibleObject? source, JsonSerializationOptions options)
        {
            if (source == null) return "";

            return options.Flatten ? SerializeFlattened(source, options) : SerializeStandard(source, options);
        }

        /// <inheritdoc />
        public override void Serialize(IAccessibleObject? source, StringBuilder builder, JsonSerializationOptions options)
        {
            if (source == null) return;

            builder.Append(Serialize(source, options));
        }

        /// <inheritdoc />
        protected override FieldContent ExtractContent(IAccessibleObject source, FieldDescriptor field)
        {
            var jsonPropertyName = field.GetAttribute<JsonPropertyNameAttribute>();
            var jsonIgnore = field.GetAttribute<JsonIgnoreAttribute>();

            return new FieldContent()
            {
                FieldName = jsonPropertyName?.Name ?? field.Name,
                FormatAttribute = field.GetAttribute<IFormatAttribute>(),
                Ignore = jsonIgnore != null,
                SerializationAttribute = field.GetAttribute<ISerializationAttribute>(),
                Value = field.PropertyInfo.GetValue(source)
            };
        }

        /// <summary>
        /// Applies serialization and formatting options to produce a value suitable for the property store.
        /// </summary>
        /// <param name="fieldContent">A container for field metadata and the value of the field.</param>
        /// <param name="options">The serialization options.</param>
        /// <returns>A value based on the serialization and formatting options.</returns>
        /// <remarks>
        /// The caller is expected to handle the SuppressNull case since that case controls flow.
        /// This method handles the PreserveNulls and NullAsEmptyString cases since those determine a value.
        /// </remarks>
        protected virtual object? GetFormattedFieldValue(FieldContent fieldContent, JsonSerializationOptions options)
        {
            if (fieldContent.Value == null)
                return options.NullHandling == NullHandlingOptions.PreserveNulls ? null : "";

            if (options.Format && fieldContent.FormatAttribute != null)
                return FormatField(fieldContent, options.ApplyMasking);

            if (options.ApplyMasking && (fieldContent.SerializationAttribute?.Masked ?? false))
                return MaskValue(fieldContent.Value.ToString() ?? "", fieldContent);

            return fieldContent.Value;
        }

        /// <summary>
        /// Converts a JsonNode to its string representation using the configured system options.
        /// </summary>
        protected virtual string JsonNodeToString(JsonNode jsonNode, JsonSerializationOptions options) => jsonNode.ToJsonString(options.SystemJsonSerializerOptions);

        /// <summary>
        /// Recursively flattens a source object's fields into a single-level property store.
        /// </summary>
        protected virtual PropertyStore ProcessSource(PropertyStore propertyStore, IAccessibleObject source, JsonSerializationOptions options)
        {
            foreach (var field in source.FieldDescriptors)
            {
                var fieldContent = ExtractContent(source, field);
                if (fieldContent.Ignore) continue;

                // a field's value may be a nested accessible object, a collection of them, or a simple value
                switch (fieldContent.Value)
                {
                    case IAccessibleObject nested:
                        ProcessSource(propertyStore, nested, options);
                        break;
                    case IEnumerable<IAccessibleObject> nestedItems:
                        var itemArray = ProcessSources(nestedItems, options);
                        propertyStore.Add(fieldContent.FieldName, itemArray);
                        break;
                    default:
                        if (fieldContent.Value == null && options.NullHandling == NullHandlingOptions.SuppressNull)
                            break;

                        var formattedFieldValue = GetFormattedFieldValue(fieldContent, options);
                        propertyStore.Add(fieldContent.FieldName, formattedFieldValue);
                        break;
                }
            }

            return propertyStore;
        }

        /// <summary>
        /// Flattens a collection of accessible objects into a collection of property stores.
        /// </summary>
        protected virtual IEnumerable<PropertyStore> ProcessSources(IEnumerable<IAccessibleObject> sources, JsonSerializationOptions options)
        {
            return sources
                .Select(source => ProcessSource(new PropertyStore(), source, options))
                .ToArray();
        }

        /// <summary>
        /// Recursively applies formatting/masking to a JSON node produced by the standard
        /// System.Text.Json serialization of the source object.
        /// </summary>
        protected virtual void ProcessNode(JsonNode jsonNode, IAccessibleObject source, JsonSerializationOptions options)
        {
            foreach (var field in source.FieldDescriptors)
            {
                var fieldContent = ExtractContent(source, field);
                if (fieldContent.Ignore) continue;

                var childNode = jsonNode[fieldContent.FieldName];
                if (childNode == null) continue;

                switch (fieldContent.Value)
                {
                    case IAccessibleObject nested:
                        ProcessNode(childNode, nested, options);
                        break;
                    case IEnumerable<IAccessibleObject> nestedItems:
                        ProcessNodes(childNode, nestedItems, options);
                        break;
                    default:
                        // SuppressNull does not apply here -- the node already omits null values,
                        // since it was generated using JSON options forced to match that setting.
                        var formattedFieldValue = GetFormattedFieldValue(fieldContent, options);
                        if (formattedFieldValue == fieldContent.Value) break;

                        var nodeValue = childNode.GetValue<object?>();
                        if (nodeValue != formattedFieldValue)
                            childNode.ReplaceWith(formattedFieldValue);

                        break;
                }
            }
        }

        /// <summary>
        /// Applies ProcessNode to each element of a JSON array node produced for a collection field.
        /// </summary>
        protected virtual void ProcessNodes(JsonNode node, IEnumerable<IAccessibleObject> sources, JsonSerializationOptions options)
        {
            var sourcesWithIndexes = sources.Select((item, index) => (item, index));

            foreach (var (item, index) in sourcesWithIndexes)
            {
                ArgumentNullException.ThrowIfNull(item);
                var innerChild = node[index];
                ArgumentNullException.ThrowIfNull(innerChild);
                ProcessNode(innerChild, item, options);
            }
        }

        /// <summary>
        /// Serializes with all nested object properties flattened into the top-level JSON object.
        /// </summary>
        /// <param name="source">The object to be serialized.</param>
        /// <param name="options">The options to apply when formatting and serializing the data.</param>
        /// <returns>A JSON string representing the input object in a flattened format.</returns>
        protected virtual string SerializeFlattened(IAccessibleObject source, JsonSerializationOptions options)
        {
            var propertyStore = new PropertyStore();
            ProcessSource(propertyStore, source, options);

            return JsonSerializer.Serialize(propertyStore, options.SystemJsonSerializerOptions);
        }

        /// <summary>
        /// Serializes in the standard way, preserving nested JSON objects.
        /// </summary>
        /// <param name="source">The object to be serialized.</param>
        /// <param name="options">The options to apply when formatting and serializing the data.</param>
        /// <returns>A JSON string representing the input object.</returns>
        protected virtual string SerializeStandard(IAccessibleObject source, JsonSerializationOptions options)
        {
            var jsonNode = JsonSerializer.SerializeToNode(source, source.GetType(), options.SystemJsonSerializerOptions);
            if (jsonNode == null) return "";

            ProcessNode(jsonNode, source, options);

            return JsonNodeToString(jsonNode, options);
        }
    }

}
