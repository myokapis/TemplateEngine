/* ****************************************************************************
Copyright 2018-2023 Gene Graves

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
using System.Text;
using System.Threading.Tasks;
using TemplateEngine.Loader;
using TemplateEngine.Web;

namespace TemplateEngine.Tests.Helpers
{

    public class MasterPresenter : MasterPresenterBase
    {
        public MasterPresenter(ITemplateLoader<IWebWriter> templateLoader) : base(templateLoader) { }

        public string GetContent_Error()
        {
            return GetContent();
        }

        public async Task<string> GetContent_Okay()
        {
            await SetupWriters("Master.tpl", "Content.tpl");
            WriteMasterSections();
            return GetContent().Replace("\r\n", "");
        }

        public async Task<string> SetupContentWriter()
        {
            await SetupContentWriter("Content.tpl");
            contentWriter!.SelectSection("HEAD");
            contentWriter.AppendAll();
            return contentWriter!.GetContent().Replace("\r\n", "");
        }

        public string SetupMasterPage_Error()
        {
            SetupMasterPage();
            WriteMasterSections();
            return GetContent().Replace("\r\n", "");
        }

        public async Task<string> SetupMasterPage_Okay()
        {
            await SetupWriters("Master.tpl", "Content.tpl", false);
            SetupMasterPage();
            WriteMasterSections();
            return GetContent().Replace("\r\n", "");
        }

        public async Task<string> SetupMasterPage_Sections()
        {
            await SetupWriters("Master.tpl", "Content.tpl", false);
            SetupMasterPage(Head, Body, Tail);
            WriteMasterSections();
            return GetContent().Replace("\r\n", "");
        }

        public string SetupMasterPage_Sections_Error()
        {
            SetupMasterPage(Head, Body, Tail);
            WriteMasterSections();
            return GetContent().Replace("\r\n", "");
        }

        public string SetupMasterPage_Writers_Error()
        {
            var sectionWriter = (IWebWriter)contentWriter!.GetWriter(Body);
            SetupMasterPage(null, sectionWriter, null);
            WriteMasterSections();
            return GetContent().Replace("\r\n", "");
        }

        public async Task<string> WriteMasterSectionAsync_Error()
        {
            SetupMasterPage();

            await WriteMasterSectionAsync(Body, async (writer) =>
            {
                writer.AppendSection();
                await Task.FromResult(true);
            });

            return GetContent().Replace("\r\n", "");
        }

        public async Task<string> WriteMasterSectionAsync_Okay()
        {
            await SetupWriters("Master.tpl", "Content.tpl");
            SetupMasterPage();

            await WriteMasterSectionAsync(Body, async(writer) =>
            {
                writer.AppendSection();
                await Task.FromResult(true);
            });

            return GetContent().Replace("\r\n", "");
        }

        public string WriteMasterSection_Error()
        {
            SetupMasterPage();
            WriteMasterSection(Body);

            return GetContent().Replace("\r\n", "");
        }

        public async Task<string> WriteMasterSection_Okay()
        {
            await SetupWriters("Master.tpl", "Content.tpl");
            SetupMasterPage();
            WriteMasterSection(Body);

            return GetContent().Replace("\r\n", "");
        }

        public string WriteMasterSection_PageBuilder_Error()
        {
            SetupMasterPage();

            WriteMasterSection(Body, (writer) =>
            {
                writer.AppendSection();
            });

            return GetContent().Replace("\r\n", "");
        }

        public async Task<string> WriteMasterSection_PageBuilder_Okay()
        {
            await SetupWriters("Master.tpl", "Content.tpl");
            SetupMasterPage();

            WriteMasterSection(Body, (writer) =>
            {
                writer.AppendSection();
            });

            return GetContent().Replace("\r\n", "");
        }

        public string WriteMasterSections_Error()
        {
            SetupMasterPage();
            WriteMasterSections();
            return GetContent().Replace("\r\n", "");
        }

    }

}
