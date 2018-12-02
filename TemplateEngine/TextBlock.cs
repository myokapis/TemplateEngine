namespace TemplateEngine
{

    public readonly struct TextBlock
    {
        public TextBlock(TextBlockType type, string text, string referenceName = null, string tagText = null)
        {
            this.ReferenceName = referenceName;
            this.TagText = tagText;
            this.Text = text;
            this.Type = type;
        }

        public string ReferenceName { get; }
        public string TagText { get; }
        public string Text { get; }
        public TextBlockType Type { get; }
    }

}
