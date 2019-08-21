/* ****************************************************************************
Copyright 2018 Gene Graves

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

   http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
**************************************************************************** */

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

        ITemplateWriter GetWriter(string sectionName); //, bool makeRoot = false);

        bool IsProvider { get; }
        //bool IsRootSelected { get; }

        ITemplateWriter RegisterFieldProvider(string fieldName, ITemplateWriter writer);

        ITemplateWriter RegisterFieldProvider(string sectionName, string fieldName, ITemplateWriter writer);

        void Reset();

        string SectionName { get; }

        void SelectProvider(string fieldName);

        void SelectSection(string sectionName);

        string SelectedSectionName { get; }

        void SetField(string key, string val);

        void SetField<T>(string key, T val);

        void SetOptionFields(string sectionName, IEnumerable<Option> data, string selectedValue = "");

        void SetMultiSectionFields<T>(string sectionName, IEnumerable<T> data);

        void SetMultiSectionFields<T>(string sectionName, IEnumerable<T> data, FieldDefinitions fieldDefinitions);

        void SetMultiSectionFields<T>(IEnumerable<T> data, FieldDefinitions fieldDefinitions = null);

        void SetSectionFields<T>(string sectionName, T data);

        void SetSectionFields<T>(string sectionName, T data, FieldDefinitions fieldDefinitions);

        void SetSectionFields<T>(string sectionName, T data, SectionOptions sectionOptions, FieldDefinitions fieldDefinitions = null);

        void SetSectionFields<T>(T data, SectionOptions sectionOptions, FieldDefinitions fieldDefinitions = null);

        Guid TemplateId { get; }

    }

}
