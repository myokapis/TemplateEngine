using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;

namespace TemplateEngine
{
  public class ViewModelAccessor<T>
  {
    private static List<PropertyInfo> _fieldProperties = null;
    private static object[] _indexes = new object[] { };
    private T _model;

    public ViewModelAccessor(T Model)
    {
      _model = Model;
    }

    public int Count
    {
      get
      {
        return FieldProperties.Count;
      }
    }

    public IEnumerable<KeyValuePair<string, string>> FieldValues
    {
      get
      {
        foreach (PropertyInfo pi in FieldProperties)
        {
          yield return new KeyValuePair<string, string>(pi.Name, pi.GetValue(_model, _indexes).ToString());
        }
      }
    }

    public T Model
    {
      get
      {
        return _model;
      }
    }

    // return the value of the field at the specified index
    public string this[int fieldIndex]
    {
      get
      {
        return FieldProperties.ElementAt(fieldIndex).GetValue(_model, _indexes).ToString();
      }
    }

    // return the field value for the specified field name
    public string this[string fieldName]
    {
      get
      {
        PropertyInfo pi = FieldProperties.Where(p => p.Name == fieldName).FirstOrDefault();
        return pi.GetValue(_model, _indexes).ToString();
      }
    }

    #region "protected methods"

    protected static List<PropertyInfo> FieldProperties
    {
      get
      {
        if (_fieldProperties == null) GetFieldProperties();
        return _fieldProperties;
      }
    }

    protected static void GetFieldProperties()
    {
      _fieldProperties = new List<PropertyInfo>();

      foreach (PropertyInfo pi in typeof(T).GetProperties())
      {
        if (!Attribute.IsDefined(pi, typeof(NotMappedAttribute))) _fieldProperties.Add(pi);
      }
    }

    #endregion

  }
}
