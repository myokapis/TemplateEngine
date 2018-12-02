using System;
using System.Collections.Generic;
using System.Text;

namespace TemplateEngine
{

    public interface ITemplate
    {
        List<string> ChildSectionNames { get; }

        List<string> FieldNames { get; }

        int RawLength { get; }

        string SectionName { get; }

        Guid TemplateTypeId { get; }

        Guid TemplateId { get; }

        IEnumerable<TextBlock> TextBlocks { get; }

        ITemplate Copy();

        ITemplate GetTemplate(string name);

        string ToString();

    }

}
