using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TemplateEngine
{
  public class FieldDefinitions
  {
    private List<string> _checkboxes = new List<string>();
    private List<DropdownDefinition> _dropdowns = new List<DropdownDefinition>();
    private List<string> _dropdownFieldNames = new List<string>();

    public FieldDefinitions() { }

    public FieldDefinitions(List<string> checkboxes, List<DropdownDefinition> dropdowns)
    {
      _checkboxes = (checkboxes == null) ? new List<string>() : checkboxes;
      _dropdowns = (dropdowns == null) ? new List<DropdownDefinition>() : dropdowns;
      _dropdownFieldNames = dropdowns.Select<DropdownDefinition, string>(d => d.FieldName).ToList<string>();
    }

    public List<string> Checkboxes
    {
      get
      {
        return _checkboxes;
      }
    }

    public List<DropdownDefinition> Dropdowns
    {
      get
      {
        return _dropdowns;
      }
    }

    public List<string> DropdownFieldNames
    {
      get
      {
        return _dropdownFieldNames;
      }
    }

    public void SetCheckboxes(params string[] fieldNames)
    {
        _checkboxes = fieldNames.ToList<string>();
    }

    public void SetDropdowns(params DropdownDefinition[] dropdowns)
    {
      _dropdowns = dropdowns.ToList<DropdownDefinition>();
      _dropdownFieldNames = dropdowns.Select<DropdownDefinition, string>(d => d.FieldName).ToList<string>();
    }

  }

  public struct DropdownDefinition
  {
    public List<Option> Data { get; set; }
    public string FieldName { get; set; }
    public string SectionName { get; set; }
  }

  public class Option
  {
    public string Text { get; set; }
    public string Value { get; set; }
  }
}
