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
using MyOkapis.Formatting;
using MyOkapis.ObjectAccess;

namespace MyOkapis.ObjectSerialization
{

    /// <summary>
    /// Serializes an accessible object's fields into a single flat, delimited string. Nested
    /// accessible objects (and collections of them) are recursively serialized inline.
    /// </summary>
    public class FlatTextSerializer : SerializerBase<SerializationOptions>, IFlatTextSerializer
    {
        /// <summary>
        /// Constructs an instance of the flat text serializer.
        /// </summary>
        /// <param name="formatter">The formatter to be used when preparing field data.</param>
        public FlatTextSerializer(IFormatter formatter) : base(formatter) { }

        /// <inheritdoc />
        public override string Serialize(IAccessibleObject? source, SerializationOptions options)
        {
            if (source == null) return "";

            var builder = new StringBuilder();
            Serialize(source, builder, options);
            return builder.ToString();
        }

        /// <inheritdoc />
        public override void Serialize(IAccessibleObject? source, StringBuilder builder, SerializationOptions options)
        {
            if (source == null) return;

            foreach (var field in source.FieldDescriptors)
            {
                var fieldContent = ExtractContent(source, field);
                var startLength = builder.Length;

                // a field's value may be a nested accessible object, a collection of them, or a simple value
                if (fieldContent.Value is IAccessibleObject nested)
                {
                    Serialize(nested, builder, options);
                }
                else if (fieldContent.Value is IEnumerable<IAccessibleObject> nestedItems)
                {
                    foreach (var item in nestedItems)
                    {
                        Serialize(item, builder, options);
                    }
                }
                else
                {
                    builder.Append(FormatField(fieldContent, options.ApplyMasking));
                }

                var terminator = fieldContent.SerializationAttribute?.Terminator;
                var terminate = (fieldContent.SerializationAttribute?.AlwaysTerminate ?? false) || builder.Length > startLength;

                if (terminator != null && terminate) builder.Append(terminator);
            }
        }
    }

}
