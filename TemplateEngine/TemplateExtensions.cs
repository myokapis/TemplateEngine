using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TemplateEngine.Obsolete
{

    public interface ITemplate
    {
        void reset();

        void setField(string key, string val);

        void setField(string key, int val);

        void setField(string key, double val);

        void setFieldGlobal(string key, string val);

        void setFieldGlobal(string key, int val);

        void setFieldGlobal(string key, double val);

        void setFieldFromFile(string key, string filename);

        void selectSection(string key);

        void deselectSection();

        void appendSection();

        void attachSection(string key);

        void setSection(string key, string data);

        void setSectionFromFile(string key, string filename);

        string getSection(string key);

        string getContent();

        void setOptionFields(string sectionName, IEnumerable<Option> data, string selectedValue = "");

        void setSectionFields<T>(string sectionName, IEnumerable<T> data);

        void setSectionFields<T>(string sectionName, IEnumerable<T> data, FieldDefinitions fieldDefinitions);

        void setSectionFields<T>(string sectionName, IEnumerable<T> data, SectionOptions sectionOptions, FieldDefinitions fieldDefinitions = null);

        void setSectionFields<T>(IEnumerable<T> data, SectionOptions sectionOptions, FieldDefinitions fieldDefinitions = null);

        void setSectionFields<T>(string sectionName, T data);

        void setSectionFields<T>(string sectionName, T data, FieldDefinitions fieldDefinitions);

        void setSectionFields<T>(string sectionName, T data, SectionOptions sectionOptions, FieldDefinitions fieldDefinitions = null);

        void setSectionFields<T>(T data, SectionOptions sectionOptions, FieldDefinitions fieldDefinitions = null);
    }

    public partial class Template : ITemplate
    {

        protected List<string> _trueValues = new List<string>() { "Yes", "Y", "T", "True" };

        #region "refactored constructors"

        // Create an empty initialized template
        public Template()
        {
            Initialize();
        }

        // Create using template file
        public Template(string filePath)
        {
            Initialize();
            StringBuilder sb = new StringBuilder(SECTIONTAG_HEAD);

            using (StreamReader sr = new StreamReader(filePath, System.Text.Encoding.Default))
            {
                sb.Append(sr.ReadToEnd());
            }

            sb.Append(SECTIONTAG_TAIL);
            construct(sb.ToString(), SECTIONTAG_HEAD_LEN, sb.Length - SECTIONTAG_TAIL_LEN);
        }

        #endregion

        //#region "static methods"

        //public static string TemplatePath { get; set; }

        //#endregion

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


        public void setOptionFields(string sectionName, IEnumerable<Option> data, string selectedValue = "")
        {
            tpl.selectSection(sectionName.ToUpper());

            foreach (Option option in data)
            {
                tpl.setField("TEXT", option.Text);
                tpl.setField("VALUE", option.Value);
                tpl.setField("SELECTED", (option.Value == selectedValue) ? "selected='selected'" : "");
                tpl.appendSection();
            }

            tpl.deselectSection();
        }

        // NOTE: this must be called with the type specified for IEnumerable<T>. Ex: setSectionFields<MyClass>(data ...
        public void setSectionFields<T>(string sectionName, IEnumerable<T> data)
        {
            selectSection(sectionName);
            setSectionFields<T>(data, SectionOptions.AppendDeselect, null);
        }

        // NOTE: this must be called with the type specified for IEnumerable<T>. Ex: setSectionFields<MyClass>(data ...
        public void setSectionFields<T>(string sectionName, IEnumerable<T> data, FieldDefinitions fieldDefinitions)
        {
            selectSection(sectionName);
            setSectionFields<T>(data, SectionOptions.AppendDeselect, fieldDefinitions);
        }

        // NOTE: this must be called with the type specified for IEnumerable<T>. Ex: setSectionFields<MyClass>(data ...
        public void setSectionFields<T>(string sectionName, IEnumerable<T> data, SectionOptions sectionOptions, FieldDefinitions fieldDefinitions = null)
        {
            selectSection(sectionName);
            setSectionFields<T>(data, sectionOptions, fieldDefinitions);
        }

        // NOTE: this must be called with the type specified for IEnumerable<T>. Ex: setSectionFields<MyClass>(data ...
        public void setSectionFields<T>(IEnumerable<T> data, SectionOptions sectionOptions, FieldDefinitions fieldDefinitions = null)
        {
            FieldDefinitions definitions = (fieldDefinitions != null) ? fieldDefinitions : new FieldDefinitions();

            foreach (T row in data)
            {
                ViewModelAccessor<T> accessor = new ViewModelAccessor<T>(row);

                foreach (KeyValuePair<string, string> kvp in accessor.FieldValues)
                {
                    if (definitions.Checkboxes.Contains(kvp.Key))
                    {
                        string checkedValue = (_trueValues.Contains(kvp.Value)) ? "checked='checked'" : "";
                        setField(kvp.Key, checkedValue);
                    }
                    else if (definitions.DropdownFieldNames.Contains(kvp.Key))
                    {
                        DropdownDefinition definition = definitions.Dropdowns.Where<DropdownDefinition>(d => d.FieldName == kvp.Key).FirstOrDefault<DropdownDefinition>();
                        setOptionFields(definition.SectionName, definition.Data, kvp.Value);
                    }
                    else
                    {
                        setField(kvp.Key, kvp.Value);
                    }
                }

                if (sectionOptions.Append) appendSection();

            }

            if (sectionOptions.Deselect) deselectSection();

        }

        public void setSectionFields<T>(string sectionName, T data)
        {
            selectSection(sectionName);
            setSectionFields(data, SectionOptions.AppendDeselect, null);
        }

        public void setSectionFields<T>(string sectionName, T data, FieldDefinitions fieldDefinitions)
        {
            selectSection(sectionName);
            setSectionFields(data, SectionOptions.AppendDeselect, fieldDefinitions);
        }

        public void setSectionFields<T>(string sectionName, T data, SectionOptions sectionOptions, FieldDefinitions fieldDefinitions = null)
        {
            selectSection(sectionName);
            setSectionFields(data, sectionOptions, fieldDefinitions);
        }

        public void setSectionFields<T>(T data, SectionOptions sectionOptions, FieldDefinitions fieldDefinitions = null)
        {

            FieldDefinitions definitions = (fieldDefinitions != null) ? fieldDefinitions : new FieldDefinitions();

            ViewModelAccessor<T> accessor = new ViewModelAccessor<T>(data);

            foreach (KeyValuePair<string, string> kvp in accessor.FieldValues)
            {
                if (definitions.Checkboxes.Contains(kvp.Key))
                {
                    string checkedValue = (_trueValues.Contains(kvp.Value)) ? "checked='checked'" : "";
                    setField(kvp.Key, checkedValue);
                }
                else if (definitions.DropdownFieldNames.Contains(kvp.Key))
                {
                    DropdownDefinition definition = definitions.Dropdowns.Where<DropdownDefinition>(d => d.FieldName == kvp.Key).FirstOrDefault<DropdownDefinition>();
                    setOptionFields(definition.SectionName, definition.Data, kvp.Value);
                }
                else
                {
                    setField(kvp.Key, kvp.Value);
                }

            }

            if (sectionOptions.Append) appendSection();
            if (sectionOptions.Deselect) deselectSection();
        }

        #endregion

        #region "protected methods"

        //protected string GetFilePath(string FileName)
        //{
        //    return (string.IsNullOrEmpty(TemplatePath)) ? FileName : Path.Combine(TemplatePath, FileName);
        //}

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
