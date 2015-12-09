using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TemplateEngine
{

  public partial class Template
  {

    #region "refactored constructors"

    // Create an empty initialized template
    public Template()
    {
      Initialize();
    }

    // Create using template file
    public Template(string FileName)
    {
      Initialize();
      StringBuilder sb = new StringBuilder(SECTIONTAG_HEAD);

      using (StreamReader re = new StreamReader(GetFilePath(FileName), System.Text.Encoding.Default))
      {
        sb.Append(re.ReadToEnd());
      }

      sb.Append(SECTIONTAG_TAIL);
      construct(sb.ToString(), SECTIONTAG_HEAD_LEN, sb.Length - SECTIONTAG_TAIL_LEN);
    }

    #endregion

    #region "static methods"

    public static string TemplatePath { get; set; }

    #endregion

    #region "public methods"

    //// TODO: decide whether to keep or to obsolete this method
    //public void setSectionFields(string sectionName, IDictionary<string, string> fieldData, bool append=true, bool deselect=true)
    //{
    //  selectSection(sectionName);

    //  foreach (KeyValuePair<string, string> kvp in fieldData)
    //  {
    //    setField(kvp.Key, kvp.Value);
    //  }

    //  if (append)
    //  {
    //    appendSection();
    //    if (deselect) deselectSection();
    //  }
    //}




    public void setSectionFields<T>(IEnumerable<T> data)
    {
      foreach (T row in data)
      {
        ViewModelAccessor<T> accessor = new ViewModelAccessor<T>(row);
        foreach (KeyValuePair<string, string> kvp in accessor.FieldValues)
        {
          setField(kvp.Key, kvp.Value);
        }

        appendSection();
      }
    }

    #endregion

    #region "protected methods"

    protected string GetFilePath(string FileName)
    {
      return (string.IsNullOrEmpty(TemplatePath)) ? FileName : Path.Combine(TemplatePath, FileName);
    }

    protected void Initialize()
    {
      head = new node();
      addedHead = new node();
      addedTail = addedHead;
      firstSection = new section();
      tpl = this;
      fields = new Object[MAX_FIELDS];
      sections = new Object[MAX_FIELDS];
    }

    #endregion

  }

}
