using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TemplateEngine
{

    public class ValueSet
    {
        public Dictionary<string, string> FieldValues { get; set; }
        public Dictionary<string, ITemplateWriter> FieldWriters { get; set; }
        public Dictionary<string, ITemplateWriter> SectionWriters { get; set; }

        public void Clear()
        {
            FieldValues?.Clear();
            FieldWriters?.Clear();
            SectionWriters?.Clear();
        }

        //public void Initialize(List<string> fieldNames, Dictionary<string, ITemplateWriter> fieldWriters,
        //    Dictionary<string, ITemplateWriter> sectionWriters)
        //{
        //    Clear();

        //    fieldNames?.ForEach(n => FieldValues.Add(n, null));
            
        //    if(fieldWriters != null)
        //    {
        //        foreach(var kvp in fieldWriters)
        //        {
        //            FieldWriters.Add(kvp.Key, kvp.Value);
        //        }
        //    }

        //    if (sectionWriters != null)
        //    {
        //        foreach (var kvp in sectionWriters)
        //        {
        //            SectionWriters.Add(kvp.Key, kvp.Value);
        //        }
        //    }
        //}
    }

}
