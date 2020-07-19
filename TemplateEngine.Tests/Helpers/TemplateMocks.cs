/* ****************************************************************************
Copyright 2018-2022 Gene Graves

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
using System.IO;
using System.Linq;
using Moq;
using TemplateEngine.Document;
using TemplateEngine.Writer;

namespace TemplateEngine.Tests.Helpers
{

    public class TemplateMocks : IDisposable
    {

        #region Static Methods & Properties

        private static readonly List<string> fileNames = new List<string>
        {
            "test_template1.txt",
            "test_template2.txt",
            "test_template3.txt"
        };

        private static readonly string templateDirectory = Path.GetTempPath();

        private static readonly List<string> templateText = new List<string>
        {
            "some template data 1",
            "some template data 2",
            "some template data 3"
        };

        #endregion

        #region Public Methods & Properties

        public TemplateMocks()
        {
            Templates = templateText.Select(t => new Template(t) as ITemplate).ToList();
            CreateTemplateFiles();
            MockTemplateFactory = GetMockTemplateFactory();
            MockWriters = GetMockWriters();
            MockWriterFactory = GetMockWriterFactory();
            MockCache = new TestAppCache();
        }

        public List<string> FileNames => fileNames;

        public TestAppCache MockCache { get; }

        public Mock<Func<string, ITemplate>> MockTemplateFactory { get; }

        public Mock<Func<ITemplate, ITemplateWriter>> MockWriterFactory { get; }

        public List<ITemplateWriter> MockWriters { get; }

        public string TemplateDirectory { get; } = templateDirectory;

        public List<ITemplate> Templates { get; }

        public List<string> TemplateText => templateText;

        #endregion

        #region Private Methods & Properties

        //private readonly Dictionary<string, ITemplate> cachedTemplates = new Dictionary<string, ITemplate>();

        private void CreateTemplateFiles()
        {
            templateText.Iterate((t, i) =>
            {
                var path = Path.Combine(templateDirectory, fileNames.ElementAt(i));
                File.WriteAllText(path, t);
            });
        }

        private Mock<Func<string, ITemplate>> GetMockTemplateFactory()
        {
            var mock = new Mock<Func<string, ITemplate>>();

            templateText.Iterate((t, i) =>
            {
                mock.Setup(m => m.Invoke(t)).Returns(Templates.ElementAt(i));
            });

            return mock;
        }

        private Mock<Func<ITemplate, ITemplateWriter>> GetMockWriterFactory()
        {
            var mock = new Mock<Func<ITemplate, ITemplateWriter>>();

            Templates.Iterate((t, i) =>
            {
                mock.Setup(m => m.Invoke(t)).Returns(MockWriters.ElementAt(i));
            });

            return mock;
        }

        private List<ITemplateWriter> GetMockWriters()
        {
            var mocks = new List<ITemplateWriter>();

            TemplateText.Iterate((t, i) =>
            {
                var mock = new Mock<ITemplateWriter>();
                mock.Setup(m => m.GetContent(It.IsAny<bool>())).Returns(t);
                mock.Setup(m => m.WriterId).Returns(new Guid($"00000000-0000-0000-0000-{i.ToString().PadLeft(12, '0')}"));
                mocks.Add(mock.Object);
            });

            return mocks;
        }

        #endregion

        #region IDisposable

        private bool disposedValue;

        ~TemplateMocks()
        {
            Dispose(disposing: false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                fileNames.ForEach(f =>
                {
                    var path = Path.Combine(templateDirectory, f);
                    if (File.Exists(path)) File.Delete(path);
                });

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion

    }

}
