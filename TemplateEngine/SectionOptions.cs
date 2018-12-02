using System;

namespace TemplateEngine
{

    public class SectionOptions
    {
        private static readonly SectionOptions appendSelect;
        private static readonly SectionOptions appendOnly;
        private static readonly SectionOptions setOnly;

        static SectionOptions()
        {
            appendSelect = new SectionOptions(true, true);
            appendOnly = new SectionOptions(true, false);
            setOnly = new SectionOptions(false, false);
        }

        public static SectionOptions AppendDeselect { get; } = appendSelect;

        public static SectionOptions AppendOnly { get; } = appendOnly;

        public static SectionOptions Set { get; } = setOnly;

        protected SectionOptions(bool append, bool deselect)
        {
            this.Append = append;
            this.Deselect = deselect;
        }

        public bool Append { get; }
        public bool Deselect { get; }
    }

}
