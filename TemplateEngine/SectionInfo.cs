using System;

namespace TemplateEngine
{

    public readonly struct SectionInfo
    {

        public SectionInfo(string sectionName, int openIndex, string openTag, int closeIndex = 0, string closeTag = null)
        {
            if (openIndex < 0) throw new ArgumentException("The OpenIndex must be greater than or equal to zero.");

            this.SectionName = sectionName;
            this.OpenIndex = openIndex;
            this.OpenTag = openTag;
            this.OpenTagLength = (openTag == null) ? 0 : openTag.Length;
            this.CloseIndex = closeIndex;
            this.CloseTag = closeTag;
            this.CloseTagLength = (closeTag == null) ? 0 : closeTag.Length;
        }

        public SectionInfo(SectionInfo sectionInfo, int closeIndex, string closeTag)
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
        }

        public string SectionName { get; }
        public int OpenIndex { get; }
        public string OpenTag { get; }
        public int OpenTagLength { get; }
        public int CloseIndex { get; }
        public string CloseTag { get; }
        public int CloseTagLength { get; }

        public int CloseTagEndIndex
        {
            get
            {
                return this.CloseIndex + this.CloseTagLength;
            }
        }

        public bool IsValid
        {
            get
            {
                return (CloseIndex > OpenIndex);
            }
        }

        public int OpenTagEndIndex
        {
            get
            {
                return this.OpenIndex + this.OpenTagLength;
            }
        }

        public Tuple<int, int> SubstringRange
        {
            get
            {
                if (CloseIndex <= OpenIndex) throw new Exception("Section range is invalid.");
                return new Tuple<int, int>(OpenIndex + OpenTagLength, CloseIndex);
            }
        }

    }

}
