using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace TemplateEngine
{

    // TODO: look at Span<> and MemorySpan<> as a way to avoid any string allocation
    public class Template : ITemplate
    {
        private List<string> fieldNames;
        protected List<TextBlock> textBlocks;
        protected Dictionary<string, Template> templates;
        protected int rawLength;

        // TODO: consider making the regex a property having a default value to allow users to set their own regex
        //       testing would be a pain unless I restricted the setting to just include the allowed characters such as [A-Za-z0-9_]
        private Regex fieldRegex = new Regex(@"(?<!<!--\s)@@[A-Za-z0-9_]+@@(?!\s-->)");
        private Regex sectionNameRegex = new Regex(@"[@><\s\r\n!-]");
        
        #region Constructors

        public Template(in string text)
        {
            Init("@MAIN");
            ParseTemplateFromText(text);
            this.TemplateTypeId = Guid.NewGuid();
            this.TemplateId = Guid.NewGuid();
            //BuildFieldIndexes();
        }

        protected Template(in string text, string sectionName, in List<string> sectionList,
            in Dictionary<string, SectionInfo> sectionLookup)
        {
            Init(sectionName);
            ParseSections(text, sectionName, sectionList, sectionLookup);
            this.TemplateTypeId = Guid.NewGuid();
            this.TemplateId = Guid.NewGuid();
            //BuildFieldIndexes();
        }

        protected Template(in Template template)
        {
            //this.fields = new Dictionary<string, List<int>>(template.fields);
            //this.sectionTags = new Dictionary<string, (string OpeningTag, string ClosingTag)>(template.sectionTags);
            this.textBlocks = new List<TextBlock>(template.textBlocks);
            this.templates = new Dictionary<string, Template>(template.templates);
            this.SectionName = template.SectionName;
            this.TemplateTypeId = template.TemplateTypeId;
            this.TemplateId = Guid.NewGuid();
            //this.fieldIndexes = template.fieldIndexes;
            this.fieldNames = template.fieldNames;
            this.rawLength = template.rawLength;
            //this.sectionIndexes = template.sectionIndexes;
        }

        #endregion

        public List<string> ChildSectionNames => this.templates.Keys.ToList();

        public List<string> FieldNames => fieldNames.ToList();

        public int RawLength => this.rawLength;

        public string SectionName { get; private set; }

        public Guid TemplateId { get; protected set; }

        public Guid TemplateTypeId { get; protected set; }

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

        public ITemplate Copy()
        {
            return new Template(this);
        }

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

        public override string ToString()
        {
            var sb = new StringBuilder();

            foreach(var textBlock in this.textBlocks)
            {
                switch(textBlock.Type)
                {
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
            var matches = this.fieldRegex.Matches(innerText);

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
            this.rawLength = endIndex - startIndex + 1;

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
                this.textBlocks.Add(new TextBlock(TextBlockType.SectionTag, null, subSection.SectionName, subSection.OpenTag));

                // add the section as a placeholder
                this.textBlocks.Add(new TextBlock(TextBlockType.Section, "", subSection.SectionName));

                // add the subsection close tag as a placeholder
                this.textBlocks.Add(new TextBlock(TextBlockType.SectionTag, null, subSection.SectionName, subSection.CloseTag));

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
                { "@MAIN", new SectionInfo("@MAIN", 0, null, text.Length, null) }
            };

            this.rawLength = text.Length;

            // split the text by sections
            var regex = new Regex(@"[ \t]?<!-- @@[A-Z0-9_]+@@ -->[ \t\r]?\n?");
            var matches = regex.Matches(text);

            // iterate the matches to find the start and end of each section
            foreach (Match match in matches)
            {
                var sectionName = sectionNameRegex.Replace(match.Value, "");
                SectionInfo sectionInfo;

                if (sectionLookup.TryGetValue(sectionName, out sectionInfo))
                {
                    // if the section has already been created then update the section based on the closing tag
                    var newSectionInfo = new SectionInfo(sectionInfo, match.Index, match.Value);
                    sectionLookup[sectionName] = newSectionInfo;
                }
                else
                {
                    // the section was not found so create a new section from the opening tag
                    sectionInfo = new SectionInfo(sectionName, match.Index, match.Value);
                    sectionLookup[sectionName] = sectionInfo;
                    sectionList.Add(sectionName);
                }
            }

            ValidateSections(sectionLookup.Values);

            ParseSections(text, "@MAIN", sectionList, sectionLookup);

        }

        public void ValidateSections(IEnumerable<SectionInfo> sectionInfos)
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
