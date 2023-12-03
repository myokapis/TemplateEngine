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
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Moq;
using TemplateEngine.Document;
using TemplateEngine.Loader;
using TemplateEngine.Web;
using TemplateEngine.Writer;
using Xunit;

namespace TemplateEngine.Tests.Helpers
{

    public class TemplateMocks : IDisposable
    {

        #region Private Static Variables

        private static readonly string[] contentTemplate;

        private static readonly List<string> fileNames = new()
        {
            "test_template1.txt",
            "test_template2.txt",
            "test_template3.txt"
        };

        private static readonly string[] masterTemplate;

        private static readonly string templateDirectory = Path.GetTempPath();

        private static readonly List<string> templateText = new()
        {
            "some template data 1",
            "some template data 2",
            "some template data 3"
        };

        #endregion

        #region Public Methods & Properties

        static TemplateMocks()
        {
            ResourceDirectory = GetResourceDirectory();
            contentTemplate = GetTemplateContent("Content.tpl");
            masterTemplate = GetTemplateContent("Master.tpl");
        }

        public TemplateMocks()
        {
            Templates = templateText.Select(t => new Template(t) as ITemplate).ToList();
            CreateTemplateFiles();
            MockTemplateFactory = GetMockTemplateFactory();
            MockWriters = GetMockWriters();
            MockTemplateLoader = GetMockTemplateLoader();
            MockWriterFactory = GetMockWriterFactory();
            MockCache = new TestAppCache();
        }

        public static List<string> ContentTemplate => new(contentTemplate);

        public static List<string> FileNames => fileNames;

        public static List<string> MasterTemplate => new(masterTemplate);

        public TestAppCache MockCache { get; }

        public Mock<Func<string, ITemplate>> MockTemplateFactory { get; }

        public Mock<ITemplateLoader<IWebWriter>> MockTemplateLoader { get; }

        public Mock<Func<ITemplate, IWebWriter>> MockWriterFactory { get; }

        public List<IWebWriter> MockWriters { get; }

        public static DirectoryInfo ResourceDirectory { get; }

        public string TemplateDirectory { get; } = templateDirectory;

        public List<ITemplate> Templates { get; }

        public static List<string> TemplateText => templateText;

        #endregion

        #region Private Methods & Properties

        private static void CreateTemplateFiles()
        {
            templateText.Iterate((t, i) =>
            {
                var path = Path.Combine(templateDirectory, fileNames.ElementAt(i));
                File.WriteAllText(path, t);
            });
        }



        public static MasterPresenter GetMasterPresenter()
        {
            WebLoader templateLoader = new(ResourceDirectory.FullName);
            return new MasterPresenter(templateLoader);
        }

        private Mock<Func<string, ITemplate>> GetMockTemplateFactory()
        {
            Mock<Func<string, ITemplate>> mock = new ();

            templateText.Iterate((t, i) =>
            {
                mock.Setup(m => m.Invoke(t)).Returns(Templates.ElementAt(i));
            });

            return mock;
        }

        private Mock<ITemplateLoader<IWebWriter>> GetMockTemplateLoader()
        {
            Mock<ITemplateLoader<IWebWriter>> mock = new();

            FileNames.Iterate((f, i) =>
            {
                mock.Setup(m => m.GetWriterAsync(f)).Returns(Task.FromResult(MockWriters.ElementAt(i)));
            });

            return mock;
        }

        private Mock<Func<ITemplate, IWebWriter>> GetMockWriterFactory()
        {
            var mock = new Mock<Func<ITemplate, IWebWriter>>();

            Templates.Iterate((t, i) =>
            {
                mock.Setup(m => m.Invoke(t)).Returns(MockWriters.ElementAt(i));
            });

            return mock;
        }

        private static List<IWebWriter> GetMockWriters()
        {
            var mocks = new List<IWebWriter>();

            TemplateText.Iterate((t, i) =>
            {
                var mock = new Mock<IWebWriter>();
                mock.Setup(m => m.GetContent(It.IsAny<bool>())).Returns(t);
                mock.Setup(m => m.WriterId).Returns(new Guid($"00000000-0000-0000-0000-{i.ToString().PadLeft(12, '0')}"));
                mocks.Add(mock.Object);
            });

            return mocks;
        }

        private static DirectoryInfo GetResourceDirectory([CallerFilePath] string path = "")
        {
            string directoryPath = Path.GetDirectoryName(path) ?? ".";
            string resourcePath = Path.GetFullPath(@"..\Resources", directoryPath);
            Console.WriteLine(resourcePath);
            return new DirectoryInfo(resourcePath);
        }

        private static string[] GetTemplateContent(string fileName)
        {
            FileInfo fileInfo = ResourceDirectory.GetFiles(fileName).First();
            return File.ReadAllLines(fileInfo.FullName);
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
