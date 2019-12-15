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
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace TemplateEngine
{

    // TODO: look at Span<> and MemorySpan<> as a way to avoid any string allocation

    /// <summary>
    /// Parses a text document into sections and fillable fields
    /// </summary>
    public class Template : ITemplate
    {
        private List<string> fieldNames;
        protected List<TextBlock> textBlocks;
        protected Dictionary<string, Template> templates;
        protected int rawLength;

        // TODO: consider making the regex a property having a default value to allow users to set their own regex
        //       testing would be a pain unless I restricted the setting to just include the allowed characters such as [A-Za-z0-9_]
        //       would need to test for alternate languages/cultures
        private Regex regexField = new Regex(@"(?<!<!--\s)@@[A-Za-z0-9_]+@@(?!\s-->)");
        private Regex regexSectionName = new Regex(@"[@><\s\r\n!-]");
        private static readonly Regex regexEOL = new Regex(@"[\r\n]");

        #region Constructors

        /// <summary>
        /// Constructs a template from raw text
        /// </summary>
        /// <param name="text">Raw text to parse</param>
        public Template(in string text)
        {
            Init("@MAIN");
            ParseTemplateFromText(text);
            this.TemplateTypeId = Guid.NewGuid();
            this.TemplateId = Guid.NewGuid();
            this.IsSingleLine = false;
            this.IsEmpty = CheckForEmpty();
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
            Init(sectionName);
            ParseSections(text, sectionName, sectionList, sectionLookup);
            this.TemplateTypeId = Guid.NewGuid();
            this.TemplateId = Guid.NewGuid();
            this.IsSingleLine = CheckForSingleLine();
            this.IsEmpty = CheckForEmpty();
        }

        /// <summary>
        /// Creates a template from another template
        /// </summary>
        /// <param name="template">Template to clone</param>
        protected Template(in Template template)
        {
            this.textBlocks = new List<TextBlock>(template.textBlocks);
            this.templates = new Dictionary<string, Template>(template.templates);
            this.SectionName = template.SectionName;
            this.TemplateTypeId = template.TemplateTypeId;
            this.TemplateId = Guid.NewGuid();
            this.fieldNames = template.fieldNames;
            this.rawLength = template.rawLength;
            this.IsSingleLine = template.IsSingleLine;
            this.IsEmpty = template.IsEmpty;
        }

        #endregion

        /// <summary>
        /// Names of all of the child sections within this template
        /// </summary>
        public List<string> ChildSectionNames => this.templates.Keys.ToList();

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
        public int RawLength => this.rawLength;

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
                foreach (var textBlock in this.textBlocks)
                {
                    yield return textBlock;
                }
            }
        }

        /// <summary>
        /// Creates a copy of this template
        /// </summary>
        /// <returns></returns>
        public ITemplate Copy()
        {
            return new Template(this);
        }

        /// <summary>
        /// Gets a copy of a template based on this template or a template representing a subsection of this template
        /// </summary>
        /// <param name="name">Section name identifying the template (null returns a copy of this template)</param>
        /// <returns>A copy of the requested template</returns>
        public ITemplate GetTemplate(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) this.Copy();

            if (this.templates.TryGetValue(name, out var template)) return template;

            foreach (var section in this.templates.Values)
            {
                if (section.GetTemplate(name) != null) return section.Copy();
            }

            return null;
        }

        /// <summary>
        /// Renders the template definition as text
        /// </summary>
        /// <returns>Template text</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();

            foreach(var textBlock in this.textBlocks)
            {
                switch(textBlock.Type)
                {
                    case TextBlockType.Prefix:
                    case TextBlockType.Suffix:
                    case TextBlockType.Field:
                    case TextBlockType.SectionTag:
                        sb.Append(textBlock.TagText);
                        break;
                    case TextBlockType.Section:
                        var template = this.templates[textBlock.ReferenceName];
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

        protected bool CheckForEmpty()
        {
            return !this.TextBlocks.Any(b => b.Type == TextBlockType.Text && b.Text.Length > 0);
        }

        protected bool CheckForSingleLine()
        {
            if (this.ChildSectionNames.Count > 0) return false;

            var suffixTags = new List<string>();

            foreach (var textBlock in this.TextBlocks)
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
                if (regexEOL.IsMatch(textBlock.Text ?? "")) return false;
                if (regexEOL.IsMatch(textBlock.TagText ?? "")) return false;
            }

            return true;
        }

        protected List<SectionInfo> GetSubSections(in SectionInfo parentSection, in IEnumerable<SectionInfo> allSections)
        {
            (int parentStart, int parentEnd) = parentSection.SubstringRange;
            var parentSectionName = parentSection.SectionName;

            // get a list of sections that fall within the parent section
            var innerSections = allSections.Where(s => s.OpenIndex >= parentStart && s.CloseTagEndIndex <= parentEnd
                && s.SectionName != parentSectionName)
                .OrderBy(s => s.OpenIndex);

            var subSections = new List<SectionInfo>();
            
            foreach(var section in innerSections)
            {
                // if section is not a child of one of the previous subsections then add it
                if (!subSections.Any(ss => section.OpenIndex >= ss.OpenTagEndIndex && section.CloseTagEndIndex <= ss.CloseIndex))
                    subSections.Add(section);
            }

            return subSections;
        }

        protected void Init(string sectionName)
        {
            this.fieldNames = new List<string>();
            this.textBlocks = new List<TextBlock>();
            this.templates = new Dictionary<string, Template>();
            this.SectionName = sectionName;
        }

        protected void ParseInnerText(in string innerText)
        {
            // bail out if there is no text to parse
            if (innerText.Length == 0) return;

            // set the text start point and beginning index values
            int startPoint = 0;

            // split the text by fields
            var matches = this.regexField.Matches(innerText);

            // iterate the matches to find the start and end of each field
            foreach (Match match in matches)
            {

                // get the text before the field and add it
                if (match.Index > startPoint)
                {
                    var text = innerText.Substring(startPoint, match.Index - startPoint);
                    this.textBlocks.Add(new TextBlock(TextBlockType.Text, text));
                }

                // get the field name from the tag text
                var fieldName = match.Value.Replace("@", "");

                // add the field to the list of template fields if it isn't already on the list
                if (!this.fieldNames.Contains(fieldName)) this.fieldNames.Add(fieldName);

                // add the field placeholder
                this.textBlocks.Add(new TextBlock(TextBlockType.Field, null, fieldName, match.Value));

                // reset the start point to after the end of the field
                 startPoint = match.Index + match.Length;
            }

            // add the text after the last field
            if(startPoint < innerText.Length)
            {
                var text = innerText.Substring(startPoint, innerText.Length - startPoint);
                this.textBlocks.Add(new TextBlock(TextBlockType.Text, text));
            }

        }

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
            this.rawLength = endIndex - startIndex; // + 1;

            // process each subsection
            foreach(var subSection in subSections)
            {

                // parse any unparsed text prior to this subsection
                if(subSection.OpenIndex > textStartIndex)
                    ParseInnerText(allText.Substring(textStartIndex, subSection.OpenIndex - textStartIndex));

                // create and add the subsection template
                var template = new Template(allText, subSection.SectionName, sectionList, sectionLookup);
                this.templates.Add(subSection.SectionName, template);

                // add the subsection open tag as a placeholder
                this.textBlocks.Add(new TextBlock(TextBlockType.Prefix, null, subSection.SectionName, subSection.OpenPrefix));
                this.textBlocks.Add(new TextBlock(TextBlockType.SectionTag, null, subSection.SectionName, subSection.OpenTag));
                this.textBlocks.Add(new TextBlock(TextBlockType.Suffix, null, subSection.SectionName, subSection.OpenSuffix));

                // add the section as a placeholder
                this.textBlocks.Add(new TextBlock(TextBlockType.Section, "", subSection.SectionName));

                // add the subsection close tag as a placeholder
                this.textBlocks.Add(new TextBlock(TextBlockType.Prefix, null, subSection.SectionName, subSection.ClosePrefix));
                this.textBlocks.Add(new TextBlock(TextBlockType.SectionTag, null, subSection.SectionName, subSection.CloseTag));
                this.textBlocks.Add(new TextBlock(TextBlockType.Suffix, null, subSection.SectionName, subSection.CloseSuffix));

                // set the new start point
                textStartIndex = subSection.CloseTagEndIndex;
            }

            // parse any unparsed text following the last subsection
            if(textStartIndex < endIndex) ParseInnerText(allText.Substring(textStartIndex, endIndex - textStartIndex));

        }

        protected void ParseTemplateFromText(string text)
        {
            List<string> sectionList = new List<string>() { "@MAIN" };

            Dictionary<string, SectionInfo> sectionLookup = new Dictionary<string, SectionInfo>()
            {
                { "@MAIN", new SectionInfo("@MAIN", 0, "", "", "", text.Length) }
            };

            this.rawLength = text.Length;
            var regex = new Regex(@"(?<prefix>[ \t]*)?(?<body><!-- @@[A-Z0-9_]+@@ -->)(?<suffix>[ \t]*\r?\n)?");
            var matches = regex.Matches(text);

            // iterate the matches to find the start and end of each section
            foreach (Match match in matches)
            {
                var sectionName = regexSectionName.Replace(match.Value, "");
                var prefix = match.Groups["prefix"]?.Value ?? "";
                var suffix = match.Groups["suffix"]?.Value ?? "";
                var tag = match.Groups["body"].Value;
                SectionInfo sectionInfo;

                if (sectionLookup.TryGetValue(sectionName, out sectionInfo))
                {
                    // if the section has already been created then update the section based on the closing tag
                    var newSectionInfo = new SectionInfo(sectionInfo, match.Index, tag, prefix, suffix);
                    sectionLookup[sectionName] = newSectionInfo;
                }
                else
                {
                    // the section was not found so create a new section from the opening tag
                    sectionInfo = new SectionInfo(sectionName, match.Index, tag, prefix, suffix);
                    sectionLookup[sectionName] = sectionInfo;
                    sectionList.Add(sectionName);
                }
            }

            ValidateSections(sectionLookup.Values);

            ParseSections(text, "@MAIN", sectionList, sectionLookup);

        }

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
