/* ****************************************************************************
Copyright 2018-2020 Gene Graves

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

using System;

namespace TemplateEngine
{

    /// <summary>
    /// Provides information about the location and length of a subsection of a text document
    /// </summary>
    public readonly struct SectionInfo
    {

        /// <summary>
        /// Constructor based on the opening tag of a section
        /// </summary>
        /// <param name="sectionName">An identifier for the section</param>
        /// <param name="openIndex">Index of the first character of the section opening tag</param>
        /// <param name="openTag">Text of the opening tag</param>
        /// <param name="prefix">Whitespace preceding the opening tag</param>
        /// <param name="suffix">Whitespace following the opening tag</param>
        /// <param name="closeIndex">Optional index of the first character of the section closing tag</param>
        public SectionInfo(string sectionName, int openIndex, string openTag, string prefix, string suffix, int closeIndex = 0)
        {
            if (openIndex < 0) throw new ArgumentException("The OpenIndex must be greater than or equal to zero.");

            this.SectionName = sectionName;
            this.OpenIndex = openIndex;
            this.OpenTag = openTag;
            this.OpenTagLength = openTag.Length;
            this.CloseIndex = closeIndex;
            this.CloseTag = "";
            this.CloseTagLength = 0;
            this.OpenPrefix = prefix;
            this.OpenSuffix = suffix;
            this.ClosePrefix = "";
            this.CloseSuffix = "";
        }

        /// <summary>
        /// Constructor based on an existing section supplemented with information about the closing tag
        /// </summary>
        /// <param name="sectionInfo">Existing <cref="SectionInfo" /> object</param>
        /// <param name="closeIndex">Index of the first character of the section closing tag</param>
        /// <param name="closeTag">Text of the section closing tag</param>
        /// <param name="prefix">Whitespace preceding the closing tag</param>
        /// <param name="suffix">Whitespace following the closing tag</param>
        public SectionInfo(SectionInfo sectionInfo, int closeIndex, string closeTag, string prefix, string suffix)
        {
            if (sectionInfo.CloseIndex > 0) throw new ArgumentException("The CloseIndex of the section parameter has already been set.");
            if (sectionInfo.OpenIndex >= closeIndex) throw new ArgumentException("The CloseIndex must be greater than the OpenIndex of the section parameter.");
            if (string.IsNullOrWhiteSpace(closeTag)) throw new ArgumentException("The CloseTag cannot be null or empty.");

            this.SectionName = sectionInfo.SectionName;
            this.OpenIndex = sectionInfo.OpenIndex;
            this.OpenTag = sectionInfo.OpenTag;
            this.OpenTagLength = sectionInfo.OpenTagLength;
            this.CloseIndex = closeIndex;
            this.CloseTag = closeTag;
            this.CloseTagLength = closeTag.Length;
            this.OpenPrefix = sectionInfo.OpenPrefix;
            this.OpenSuffix = sectionInfo.OpenSuffix;
            this.ClosePrefix = prefix;
            this.CloseSuffix = suffix;
        }

        /// <summary>
        /// Text identifier for the section
        /// </summary>
        public string SectionName { get; }

        /// <summary>
        /// Index of the first character of the section opening tag
        /// </summary>
        public int OpenIndex { get; }

        /// <summary>
        /// Text of the section opening tag
        /// </summary>
        public string OpenTag { get; }

        /// <summary>
        /// Length of the opening tag
        /// </summary>
        public int OpenTagLength { get; }

        /// <summary>
        /// Index of the first character of the section closing tag
        /// </summary>
        public int CloseIndex { get; }

        /// <summary>
        /// Text of the section closing tag
        /// </summary>
        public string CloseTag { get; }

        /// <summary>
        /// Length of the section closing tag
        /// </summary>
        public int CloseTagLength { get; }

        /// <summary>
        /// Whitespace preceding the section opening tag
        /// </summary>
        public string OpenPrefix { get; }

        /// <summary>
        /// Whitespace following the section opening tag
        /// </summary>
        public string OpenSuffix { get; }

        /// <summary>
        /// Whitespace preceding the section closing tag
        /// </summary>
        public string ClosePrefix { get; }

        /// <summary>
        /// Whitespace following the section closing tag
        /// </summary>
        public string CloseSuffix { get; }

        /// <summary>
        /// Index of the end of the section closing tag
        /// </summary>
        public int CloseTagEndIndex
        {
            get
            {
                return this.CloseIndex + this.CloseTagLength + this.ClosePrefix.Length + this.CloseSuffix.Length;
            }
        }

        /// <summary>
        /// Method for verifying that the section represents a valid character range
        /// </summary>
        public bool IsValid
        {
            get
            {
                return (CloseIndex > OpenIndex);
            }
        }

        /// <summary>
        /// Index of the end of the section opening tag
        /// </summary>
        public int OpenTagEndIndex
        {
            get
            {
                return this.OpenIndex + this.OpenTagLength + this.OpenPrefix.Length + this.OpenSuffix.Length;
            }
        }

        /// <summary>
        /// Index range of the characters contained within the section
        /// </summary>
        public Tuple<int, int> SubstringRange
        {
            get
            {
                if (CloseIndex <= OpenIndex) throw new Exception("Section range is invalid.");
                return new Tuple<int, int>(OpenTagEndIndex, CloseIndex);
            }
        }

    }

}
