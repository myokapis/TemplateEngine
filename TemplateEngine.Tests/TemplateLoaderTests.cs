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
using System.IO;
using FluentAssertions;
using Xunit;

namespace TemplateEngine.Tests
{

    public class TemplateLoaderTests
    {

        [Fact]
        public void TestLoadTemplate()
        {
            var fileName = "template.txt";
            var fileData = "template data for test";
            string actual = null;

            UseTempFile(fileName, fileData, () =>
            {
                var loader = new TemplateLoader(Path.GetTempPath());
                actual = loader.LoadTemplate(fileName);
            });

            actual.Should().Be(fileData);
        }

        [Fact]
        public void TestTemplateDirectory()
        {
            var tempPath = Path.GetTempPath();
            var loader = new TemplateLoader(tempPath);
            loader.TemplateDirectory.Should().Be(tempPath);
        }

        private void UseTempFile(string fileName, string fileData, Action action)
        {
            var filePath = Path.Combine(Path.GetTempPath(), fileName);

            try
            {
                File.AppendAllText(filePath, fileData);
                action.Invoke();
            }
            finally
            {
                if (File.Exists(filePath)) File.Delete(filePath);
            }
        }

    }

}
