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

using System.Reflection;

namespace MyOkapis.ObjectAccess
{

    /// <summary>
    /// Caches a property together with all of the custom attributes declared on it, so that
    /// attribute lookups do not require a fresh reflection call every time a field is accessed.
    /// </summary>
    public readonly struct FieldDescriptor
    {
        private readonly Attribute[] attributes;

        /// <summary>
        /// Constructs a descriptor for the given property, capturing its custom attributes.
        /// </summary>
        /// <param name="propertyInfo">The property being described</param>
        public FieldDescriptor(PropertyInfo propertyInfo)
        {
            PropertyInfo = propertyInfo;
            attributes = Attribute.GetCustomAttributes(propertyInfo, inherit: true);
        }

        /// <summary>
        /// Name of the described property
        /// </summary>
        public string Name => PropertyInfo.Name;

        /// <summary>
        /// The reflected property info for the property
        /// </summary>
        public PropertyInfo PropertyInfo { get; }

        /// <summary>
        /// Returns the first custom attribute on the property that is assignable to the requested
        /// type, or null if none is found. The requested type may be a concrete attribute class or
        /// an interface implemented by one.
        /// </summary>
        /// <typeparam name="TAttribute">The attribute type or interface to look up</typeparam>
        /// <returns>The matching attribute, or null if none was found</returns>
        public TAttribute? GetAttribute<TAttribute>()
        {
            foreach (var attribute in attributes)
            {
                if (attribute is TAttribute match) return match;
            }

            return default;
        }
    }

}
