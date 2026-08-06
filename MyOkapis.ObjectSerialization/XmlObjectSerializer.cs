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
using System.Xml;
using System.Xml.Serialization;
using MyOkapis.Formatting;
using MyOkapis.ObjectAccess;

namespace MyOkapis.ObjectSerialization
{

    /// <summary>
    /// Serializes an accessible object to an XML string, honoring FormatAttribute and
    /// SerializationAttribute masking, and System.Xml.Serialization's own XmlElement/XmlIgnore/
    /// XmlAttribute attributes.
    /// </summary>
    public class XmlObjectSerializer : SerializerBase<XmlSerializationOptions>, IXmlObjectSerializer
    {
        /// <summary>
        /// Constructs an instance of the XML object serializer.
        /// </summary>
        /// <param name="formatter">The formatter to be used when preparing field data.</param>
        public XmlObjectSerializer(IFormatter formatter) : base(formatter) { }

        /// <inheritdoc />
        public override string Serialize(IAccessibleObject? source, XmlSerializationOptions options)
        {
            if (source == null) return "";

            var xmlDocument = GetXmlDocument(source, options);

            // the document has a single node: the XML for the top-level source object
            var sourceNode = xmlDocument.FirstChild;
            if (sourceNode == null) return "";

            ProcessNode(sourceNode, source, options.ApplyMasking);

            return XmlDocumentToString(xmlDocument, options);
        }

        /// <inheritdoc />
        public override void Serialize(IAccessibleObject? source, StringBuilder builder, XmlSerializationOptions options)
        {
            if (source == null) return;

            builder.Append(Serialize(source, options));
        }

        /// <inheritdoc />
        protected override FieldContent ExtractContent(IAccessibleObject source, FieldDescriptor field)
        {
            var elementAttribute = field.GetAttribute<XmlElementAttribute>();

            Attribute? ignoreAttribute = (Attribute?)field.GetAttribute<XmlIgnoreAttribute>()
                ?? field.GetAttribute<XmlAttributeAttribute>();

            return new FieldContent()
            {
                FieldName = elementAttribute?.ElementName ?? field.Name,
                FormatAttribute = field.GetAttribute<IFormatAttribute>(),
                Ignore = ignoreAttribute != null,
                SerializationAttribute = field.GetAttribute<ISerializationAttribute>(),
                Value = field.PropertyInfo.GetValue(source)
            };
        }

        /// <summary>
        /// Serializes the source object using System.Xml.Serialization to produce the starting document.
        /// </summary>
        protected virtual XmlDocument GetXmlDocument(IAccessibleObject source, XmlSerializationOptions options)
        {
            var xmlDocument = new XmlDocument();
            var serializer = new XmlSerializer(source.GetType());
            var navigator = xmlDocument.CreateNavigator();

            if (navigator == null)
                throw new NullReferenceException("Unable to create an XML document.");

            using var xmlWriter = navigator.AppendChild();
            {
                if (options.RemoveNamespace)
                    serializer.Serialize(xmlWriter, source, new XmlSerializerNamespaces([XmlQualifiedName.Empty]));
                else
                    serializer.Serialize(xmlWriter, source);
            }

            return xmlDocument;
        }

        /// <summary>
        /// Recursively applies formatting/masking to the XML nodes produced for the source object.
        /// </summary>
        protected virtual void ProcessNode(XmlNode xmlNode, IAccessibleObject source, bool applyMasking)
        {
            foreach (var field in source.FieldDescriptors)
            {
                var fieldContent = ExtractContent(source, field);
                if (fieldContent.Ignore || fieldContent.Value == null) continue;

                var childNode = xmlNode[fieldContent.FieldName];
                if (childNode == null) continue;

                switch (fieldContent.Value)
                {
                    case IAccessibleObject nested:
                        ProcessNode(childNode, nested, applyMasking);
                        break;
                    case IEnumerable<IAccessibleObject> nestedItems:
                        ProcessNodes(nestedItems, childNode.ChildNodes, applyMasking);
                        break;
                    default:
                        if (fieldContent.FormatAttribute != null)
                        {
                            childNode.InnerText = FormatField(fieldContent, applyMasking);
                        }
                        else if (applyMasking && (fieldContent.SerializationAttribute?.Masked ?? false))
                        {
                            childNode.InnerText = MaskValue(childNode.InnerText, fieldContent);
                        }

                        break;
                }
            }
        }

        /// <summary>
        /// Applies ProcessNode to each element of an XML node list produced for a collection field.
        /// </summary>
        protected virtual void ProcessNodes(IEnumerable<IAccessibleObject> sources, XmlNodeList childNodes, bool applyMasking)
        {
            var sourcesWithIndexes = sources.Select((item, index) => (item, index));

            foreach (var (item, index) in sourcesWithIndexes)
            {
                ArgumentNullException.ThrowIfNull(item);
                var innerChild = childNodes[index];
                ArgumentNullException.ThrowIfNull(innerChild);
                ProcessNode(innerChild, item, applyMasking);
            }
        }

        /// <summary>
        /// Converts the working XML document to its final string representation.
        /// </summary>
        protected virtual string XmlDocumentToString(XmlDocument xmlDocument, XmlSerializationOptions options)
        {
            var xmlWriterSettings = new XmlWriterSettings
            {
                Indent = options.Indent,
                OmitXmlDeclaration = options.OmitXmlDeclaration
            };

            using var writer = new StringWriter();
            using (var xmlWriter = XmlWriter.Create(writer, xmlWriterSettings))
            {
                xmlDocument.WriteTo(xmlWriter);
            }

            return writer.ToString();
        }
    }

}
