using System;
using System.Collections.Generic;

namespace TemplateEngine
{

    public interface ITemplateWriter
    {

        void AppendAll(string sectionName = null);

        void AppendSection(bool deselect = false);

        void Clear();

        void DeselectSection();

        string GetContent(bool appendAll = false);

        ITemplateWriter GetWriter(string sectionName);

        string HashCode { get; }

        bool IsRootSelected { get; }

        bool RegisterFieldProvider(string fieldName, ITemplateWriter writer);

        bool RegisterFieldProvider(string sectionName, string fieldName, ITemplateWriter writer);

        void Reset();

        string SectionName { get; }

        void SelectProvider(string fieldName);

        void SelectSection(string sectionName);

        string SelectedSectionName { get; }

        void SetField(string key, string val);

        void SetField<T>(string key, T val);

        void SetOptionFields(string sectionName, IEnumerable<Option> data, string selectedValue = "");

        void SetSectionFields<T>(string sectionName, T data) where T : IEnumerable<T>;

        void SetSectionFields<T>(string sectionName, T data, FieldDefinitions fieldDefinitions) where T : IEnumerable<T>;

        void SetSectionFields<T>(string sectionName, T data, SectionOptions sectionOptions,
            FieldDefinitions fieldDefinitions = null) where T : IEnumerable<T>;

        void SetSectionFields<T>(T data, SectionOptions sectionOptions, FieldDefinitions fieldDefinitions = null) where T : IEnumerable<T>;

        void SetSectionFields<T>(T data, string sectionName);

        void SetSectionFields<T>(T data, string sectionName, FieldDefinitions fieldDefinitions);

        void SetSectionFields<T>(SectionOptions sectionOptions, T data, string sectionName, FieldDefinitions fieldDefinitions = null);

        void SetSectionFields<T>(SectionOptions sectionOptions, T data, FieldDefinitions fieldDefinitions = null);

        Guid TemplateId { get; }

    }

}
