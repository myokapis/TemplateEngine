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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace TemplateEngine.Document
{

    // TODO: look at Span<> and MemorySpan<> to see if they could be useful

    /// <summary>
    /// Parses a text document into sections and fillable fields
    /// </summary>
    public class Template : ITemplate
    {
        private List<string> fieldNames = new List<string>();

        /// <summary>
        /// Collection of parsed text blocks
        /// </summary>
        protected List<TextBlock> textBlocks = new List<TextBlock>();

        /// <summary>
        /// Collection of child templates referenced by section name
        /// </summary>
        protected Dictionary<string, Template> templates = new Dictionary<string, Template>();

        /// <summary>
        /// Raw length of the string from which the template was parsed
        /// </summary>
        protected int rawLength;

        private static readonly Regex regexField = new Regex(@"(?<!<!--\s)@@[A-Za-z0-9_]+@@(?!\s-->)");
        private static readonly Regex regexSectionName = new Regex(@"[(@|\*)><\s\r\n!-]");
        private static readonly Regex regexEOL = new Regex(@"[\r\n]");
        private static readonly Regex regexSectionText = new Regex(@"(?<prefix>[ \t]*)?(?<body><!-- (@@|\*\*)[A-Z0-9_]+(@@|\*\*) -->)(?<suffix>[ \t]*\r?\n)?");

        #region Constructors

        //internal Template()
        //{

        //}

        /// <summary>
        /// Constructs a template from raw text
        /// </summary>
        /// <param name="text">Raw text to parse</param>
        public Template(in string text)
        {
            SectionName = "@MAIN";
            ParseTemplateFromText(text);
            TemplateTypeId = Guid.NewGuid();
            TemplateId = Guid.NewGuid();
            IsSingleLine = false;
            IsEmpty = CheckForEmpty();
        }

        /// <summary>
        /// Constructs a template for a subsection of a raw text document
        /// </summary>
        /// <param name="text">Raw text document</param>
        /// <param name="sectionName">Name of the subsection to parse</param>
        /// <param name="sectionList">Names of the sections in the document</param>
        /// <param name="sectionLookup">Dictionary of sections in the document keyed by section name</param>
        protected Template(in string text, string sectionName, in List<string> sectionList,
            in Dictionary<string, SectionInfo> sectionLookup)
        {
            SectionName = sectionName;
            ParseSections(text, sectionName, sectionList, sectionLookup);
            TemplateTypeId = Guid.NewGuid();
            TemplateId = Guid.NewGuid();
            IsSingleLine = CheckForSingleLine();
            IsEmpty = CheckForEmpty();
        }

        /// <summary>
        /// Creates a template from another template
        /// </summary>
        /// <param name="template">Template to clone</param>
        protected Template(in Template template)
        {
            textBlocks = new List<TextBlock>(template.textBlocks);
            templates = new Dictionary<string, Template>(template.templates);
            SectionName = template.SectionName;
            TemplateTypeId = template.TemplateTypeId;
            TemplateId = Guid.NewGuid();
            fieldNames = template.fieldNames;
            rawLength = template.rawLength;
            IsSingleLine = template.IsSingleLine;
            IsEmpty = template.IsEmpty;
        }

        #endregion

        /// <summary>
        /// Names of all of the child sections within this template
        /// </summary>
        public List<string> ChildSectionNames => templates.Keys.ToList();

        /// <summary>
        /// Names of all of the fields within this template
        /// </summary>
        public List<string> FieldNames => fieldNames.ToList();

        /// <summary>
        /// Flag indicating if the template is an empty template
        /// </summary>
        public bool IsEmpty { get; private set; }

        /// <summary>
        /// Flag indicating if the section represented by this template begins and ends on a single line
        /// </summary>
        public bool IsSingleLine { get; private set; }

        /// <summary>
        /// Length of the raw text used to construct this template
        /// </summary>
        public int RawLength => rawLength;

        /// <summary>
        /// Name of the section represented by this template
        /// </summary>
        public string SectionName { get; private set; }

        /// <summary>
        /// A unique identifier for this instance
        /// </summary>
        public Guid TemplateId { get; protected set; }

        /// <summary>
        /// An identifier shared by all templates constructed from a particular template
        /// </summary>
        public Guid TemplateTypeId { get; protected set; }

        /// <summary>
        /// A collection of text block contained within this template
        /// </summary>
        public IEnumerable<TextBlock> TextBlocks
        {
            get
            {
                foreach (var textBlock in textBlocks)
                {
                    yield return textBlock;
                }
            }
        }

        /// <summary>
        /// Creates a copy of this template
        /// </summary>
        /// <returns>A copy of the current template</returns>
        public ITemplate Copy()
        {
            return new Template(this);
        }

        /// <summary>
        /// Gets a copy of a template based on this template or a template representing a subsection of this template
        /// </summary>
        /// <param name="sectionName">Section name identifying the template (null returns a copy of this template)</param>
        /// <returns>A copy of the requested template</returns>
        public ITemplate GetTemplate(string sectionName)
        {
            if (string.IsNullOrWhiteSpace(sectionName)) Copy();

            if (templates.TryGetValue(sectionName, out var template)) return template;

            foreach (var section in templates.Values)
            {
                if (section.GetTemplate(sectionName) != null) return section.Copy();
            }

            throw new ArgumentException($"Section, {sectionName}, was not found in the current template.");
        }

        /// <summary>
        /// Renders the template definition as text
        /// </summary>
        /// <returns>Template text</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();

            foreach (var textBlock in textBlocks)
            {
                switch (textBlock.Type)
                {
                    case TextBlockType.Prefix:
                    case TextBlockType.Suffix:
                    case TextBlockType.Field:
                    case TextBlockType.SectionTag:
                        sb.Append(textBlock.TagText);
                        break;
                    case TextBlockType.Section:
                        var template = templates[textBlock.ReferenceName];
                        sb.Append(template.ToString());
                        break;
                    default:
                        sb.Append(textBlock.Text);
                        break;
                }
            }

            return sb.ToString();
        }

        #region Protected Methods

        /// <summary>
        /// True if all text type text blocks contain no text
        /// </summary>
        /// <returns></returns>
        protected bool CheckForEmpty()
        {
            return !TextBlocks.Any(b => b.Type == TextBlockType.Text && b.Text.Length > 0);
        }

        /// <summary>
        /// Checks if this section exists in a single line of the template
        /// </summary>
        /// <returns>True if this section exists in a single line of the template</returns>
        protected bool CheckForSingleLine()
        {
            if (ChildSectionNames.Count > 0) return false;

            var suffixTags = new List<string>();

            foreach (var textBlock in TextBlocks)
            {
                // skip non-text blocks
                if ((textBlock.Type & (TextBlockType.Field | TextBlockType.Section | TextBlockType.SectionTag)) > 0) continue;

                // skip the closing tag suffix
                if (textBlock.Type == TextBlockType.Suffix)
                {
                    if (suffixTags.Contains(textBlock.ReferenceName)) continue;
                    suffixTags.Add(textBlock.ReferenceName);
                }

                // not single line if the text or tag text contains an EOL char
                if (regexEOL.IsMatch(textBlock.Text)) return false;
                if (regexEOL.IsMatch(textBlock.TagText)) return false;
            }

            return true;
        }

        /// <summary>
        /// Parses a template section to find all subsections
        /// </summary>
        /// <param name="parentSection">A <see cref="SectionInfo"/> for the section to be parsed</param>
        /// <param name="allSections">A collection of <see cref="SectionInfo"/> representing all
        /// sections in the template</param>
        /// <returns></returns>
        protected List<SectionInfo> GetSubSections(in SectionInfo parentSection, in IEnumerable<SectionInfo> allSections)
        {
            var subSections = new List<SectionInfo>();

            //// don't parse a section if it is a literal section
            //if (parentSection.IsLiteral)
            //    return subSections;

            (int parentStart, int parentEnd) = parentSection.SubstringRange;
            var parentSectionName = parentSection.SectionName;

            // get a list of sections that fall within the parent section
            var innerSections = allSections.Where(s => s.OpenIndex >= parentStart && s.CloseTagEndIndex <= parentEnd
                && s.SectionName != parentSectionName)
                .OrderBy(s => s.OpenIndex);

            foreach (var section in innerSections)
            {
                // if section is not a child of one of the previous subsections then add it
                if (!subSections.Any(ss => section.OpenIndex >= ss.OpenTagEndIndex && section.CloseTagEndIndex <= ss.CloseIndex))
                    subSections.Add(section);
            }

            return subSections;
        }

        ///// <summary>
        ///// Sets initial values for properties and members
        ///// </summary>
        ///// <param name="sectionName"></param>
        //protected void Init(string sectionName)
        //{
        //    fieldNames = new List<string>();
        //    textBlocks = new List<TextBlock>();
        //    templates = new Dictionary<string, Template>();
        //    SectionName = sectionName;
        //}

        /// <summary>
        /// Parses a section of text from a template
        /// </summary>
        /// <param name="innerText">The text to be parsed</param>
        protected void ParseInnerText(in string innerText)
        {
            // bail out if there is no text to parse
            if (innerText.Length == 0) return;

            // set the text start point and beginning index values
            int startPoint = 0;

            // split the text by fields
            var matches = regexField.Matches(innerText);

            // iterate the matches to find the start and end of each field
            foreach (Match match in matches)
            {

                // get the text before the field and add it
                if (match.Index > startPoint)
                {
                    var text = innerText.Substring(startPoint, match.Index - startPoint);
                    textBlocks.Add(new TextBlock(TextBlockType.Text, text));
                }

                // get the field name from the tag text
                var fieldName = match.Value.Replace("@", "");

                // add the field to the list of template fields if it isn't already on the list
                if (!fieldNames.Contains(fieldName)) fieldNames.Add(fieldName);

                // add the field placeholder
                textBlocks.Add(new TextBlock(TextBlockType.Field, "", fieldName, match.Value));

                // reset the start point to after the end of the field
                startPoint = match.Index + match.Length;
            }

            // add the text after the last field
            if (startPoint < innerText.Length)
            {
                var text = innerText.Substring(startPoint, innerText.Length - startPoint);
                textBlocks.Add(new TextBlock(TextBlockType.Text, text));
            }

        }

        /// <summary>
        /// Parses a section with respect to its subsections
        /// </summary>
        /// <param name="allText">The full text used to generate the template</param>
        /// <param name="sectionName">The section being parsed</param>
        /// <param name="sectionList">A collection of section names</param>
        /// <param name="sectionLookup">A collection of <see cref="SectionInfo"/> representing
        /// all sections in the template</param>
        protected void ParseSections(in string allText, string sectionName, in List<string> sectionList,
            in Dictionary<string, SectionInfo> sectionLookup)
        {

            // find the current section and get its subsections
            var section = sectionLookup[sectionName];
            var subSections = GetSubSections(section, sectionLookup.Values);

            // get the range for the current section
            (int startIndex, int endIndex) = section.SubstringRange;
            int textStartIndex = startIndex;

            // save the raw section length
            rawLength = endIndex - startIndex;

            // process each subsection
            foreach (var subSection in subSections)
            {

                // parse any unparsed text prior to this subsection
                if (subSection.OpenIndex > textStartIndex)
                    ParseInnerText(allText.Substring(textStartIndex, subSection.OpenIndex - textStartIndex));

                // create and add the subsection template
                var template = new Template(allText, subSection.SectionName, sectionList, sectionLookup);
                templates.Add(subSection.SectionName, template);

                // add the subsection open tag as a placeholder
                textBlocks.Add(new TextBlock(TextBlockType.Prefix, "", subSection.SectionName, subSection.OpenPrefix));
                textBlocks.Add(new TextBlock(TextBlockType.SectionTag, "", subSection.SectionName, subSection.OpenTag));
                textBlocks.Add(new TextBlock(TextBlockType.Suffix, "", subSection.SectionName, subSection.OpenSuffix));

                // add the section as a placeholder
                if (subSection.IsLiteral)
                {
                    // add literal section containing the actual text
                    var textLength = subSection.CloseIndex - subSection.OpenTagEndIndex;
                    textBlocks.Add(new TextBlock(TextBlockType.Literal, allText.Substring(subSection.OpenTagEndIndex, textLength), subSection.SectionName));
                }
                else
                {
                    // add normal section as a placeholder
                    textBlocks.Add(new TextBlock(TextBlockType.Section, "", subSection.SectionName));
                }

                // add the subsection close tag as a placeholder
                textBlocks.Add(new TextBlock(TextBlockType.Prefix, "", subSection.SectionName, subSection.ClosePrefix));
                textBlocks.Add(new TextBlock(TextBlockType.SectionTag, "", subSection.SectionName, subSection.CloseTag));
                textBlocks.Add(new TextBlock(TextBlockType.Suffix, "", subSection.SectionName, subSection.CloseSuffix));

                // set the new start point
                textStartIndex = subSection.CloseTagEndIndex;
            }

            // parse any unparsed text following the last subsection
            if (textStartIndex < endIndex) ParseInnerText(allText.Substring(textStartIndex, endIndex - textStartIndex));

        }

        /// <summary>
        /// Parses text into sections and subsections
        /// </summary>
        /// <param name="text">The text to be parsed</param>
        protected void ParseTemplateFromText(string text)
        {
            string? literalSectionName = null;
            rawLength = text.Length;
            var matches = regexSectionText.Matches(text);

            // add the default parent section
            Dictionary<string, SectionInfo> sectionLookup = new Dictionary<string, SectionInfo>()
            {
                { "@MAIN", new SectionInfo("@MAIN", 0, "", "", "", false, text.Length) }
            };

            // iterate the matches to find the start and end of each section
            foreach (Match match in matches)
            {
                var sectionName = regexSectionName.Replace(match.Value, "");
                var prefix = match.Groups["prefix"]?.Value ?? "";
                var suffix = match.Groups["suffix"]?.Value ?? "";
                var tag = match.Groups["body"].Value;

                if (sectionLookup.TryGetValue(sectionName, out var sectionInfo))
                {
                    if ((literalSectionName == null) || (literalSectionName == sectionName))
                    {
                        // section has already been created so update the section based on the closing tag
                        var newSectionInfo = new SectionInfo(sectionInfo, match.Index, tag, prefix, suffix);
                        sectionLookup[sectionName] = newSectionInfo;

                        // if the section being closed is a literal section then update the open section flag
                        if (sectionInfo.IsLiteral)
                            literalSectionName = null;
                    }
                }
                else
                {
                    // the section was not found so check if the new section is a literal section
                    var isLiteral = match.Value.Contains("**");

                    if(literalSectionName == null)
                    {
                        //create a new section from the opening tag
                        sectionInfo = new SectionInfo(sectionName, match.Index, tag, prefix, suffix, isLiteral);
                        sectionLookup[sectionName] = sectionInfo;

                        // set a flag to process all subsequent text as literal text until the current literal section is closed
                        if (isLiteral)
                            literalSectionName = sectionName;
                    }

                }

            }

            ValidateSections(sectionLookup.Values);

            ParseSections(text, "@MAIN", sectionLookup.Keys.ToList(), sectionLookup);

        }

        /// <summary>
        /// Applies rules to ensure all section definitions are valid
        /// </summary>
        /// <param name="sectionInfos">The collection of <see cref="SectionInfo"/> to be validated</param>
        protected void ValidateSections(IEnumerable<SectionInfo> sectionInfos)
        {

            var sections = sectionInfos.Select(s => new
            {
                s.SectionName,
                s.OpenIndex,
                s.OpenTagEndIndex,
                s.CloseIndex,
                s.CloseTagEndIndex
            });

            // check for overlapping sections
            var overlap = sections.FirstOrDefault(s1 => sections.Any(s2 => s1.OpenIndex >= s2.OpenTagEndIndex
                && s1.CloseIndex >= s2.CloseTagEndIndex && s1.OpenIndex <= s2.CloseIndex
                && s1.SectionName != s2.SectionName));

            if (overlap != null)
                throw new InvalidOperationException($"Section, {overlap?.SectionName}, is improperly nested.");

            // check for sections without an opening or closing tag
            var oneTag = sections.FirstOrDefault(s => s.CloseIndex == 0);

            if (oneTag != null)
                throw new InvalidOperationException($"Section, {oneTag?.SectionName}, is missing an opening or closing tag.");

            // check for sections with invalid ranges
            var sectionName = sectionInfos.FirstOrDefault(s => !s.IsValid).SectionName;

            if (!string.IsNullOrWhiteSpace(sectionName))
                throw new InvalidOperationException($"Section, {sectionName}, has an invalid section range.");
        }

        #endregion

        /// <summary>
        /// Calculates an MD5 hash code from a string
        /// </summary>
        /// <param name="text">The string to be hashed</param>
        /// <returns>An MD5 hash code</returns>
        protected static string CalculateHashCode(in string text)
        {
            using (var md5 = MD5.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(text);
                return Encoding.UTF8.GetString(md5.ComputeHash(bytes));
            }
        }

    }

}
