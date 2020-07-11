/* ****************************************************************************
Copyright 2018-2020 Gene Graves

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
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using Moq;
using LazyCache.Mocks;
using LazyCache;
using Microsoft.Extensions.Caching.Memory;
using FluentAssertions.Specialized;

namespace TemplateEngine.Tests
{

    public class TemplateCacheTests
    {

        //[Fact]
        //public void TestExpiration()
        //{
        //    // get a mock template loader
        //    var fileName = "test_template.txt";
        //    var testTemplateData = "some template data";
        //    var mockLoader = GetTemplateLoader(fileName, testTemplateData);

        //    // create a template cache using the mock loader
        //    var expiresAfter = 1;
        //    var mockCache = new MockCachingService();
        //    mockCache.DefaultCachePolicy = new LazyCache.CacheDefaults { DefaultCacheDurationSeconds = expiresAfter };
        //    var cache = new TemplateCache(mockCache, mockLoader.Object);

        //    // ensure template is not in the cache
        //    mockCache.Remove(fileName);

        //    // request a template
        //    var tpl = cache.GetTemplate(fileName);

        //    // verify the template is cached
        //    cache.IsTemplateCached(fileName).Should().BeTrue();

        //    // wait for the cache to expire
        //    Thread.Sleep(expiresAfter * 1000);

        //    // verify the template is no longer cached
        //    cache.IsTemplateCached(fileName).Should().BeFalse();
        //}

        //[Fact]
        //public void TestGetTemplate()
        //{
        //    // get a mock template loader
        //    var fileName = "test_template.txt";
        //    var testTemplateData = "some template data";
        //    var mockLoader = GetTemplateLoader(fileName, testTemplateData);

        //    // setup a dictionary for 

        //    // create a mock cache
        //    var mockCache = new Mock<IAppCache>();
        //    mockCache.Setup(m => m.GetOrAdd(It.IsAny<string>(), It.IsAny<Func<ICacheEntry, ITemplate>>()))
        //        .Callback<string, Func<ICacheEntry, ITemplate>>((k, vf) => { if (!dic.ContainsKey(k)) dic.Add(k, testTemplateData); })
        //        .Returns(testTemplateData);

        //    // create a template cache using the mock cache and mock loader
        //    var cache = new TemplateCache(mockCache, mockLoader.Object);

        //    // ensure template is not in the cache
        //    dic.Remove(fileName);

        //    // request a template twice
        //    var tpl = cache.GetTemplate(fileName);
        //    tpl = cache.GetTemplate(fileName);

        //    // verify the template and that it was only loaded once
        //    tpl.ToString().Should().Be(testTemplateData);
        //    mockLoader.Verify(m => m.LoadTemplateText(fileName), Times.Once);
        //}

        //[Fact]
        //public async Task TestGetTemplateAsync()
        //{
        //    var dic = new Dictionary<string, ITemplate>();

        //    // get a mock template loader
        //    var fileName = "test_template.txt";
        //    var testTemplateData = "some template data";
        //    var mockLoader = GetTemplateLoader(fileName, testTemplateData);

        //    // get a mock cache
        //    var mockCache = new Mock<IAppCache>();
        //    mockCache.Setup(m => m.GetOrAddAsync(It.IsAny<string>(), It.IsAny<Func<Task<ITemplate>>>()))
        //        .Callback()

        //    // create a template cache using the mock loader

        //    var cache = new TemplateCache(mockCache, mockLoader.Object);

        //    // ensure template is not in the cache
        //    mockCache.Remove(fileName);

        //    // request a template twice
        //    await cache.GetTemplateAsync(fileName);
        //    var tpl = await cache.GetTemplateAsync(fileName);

        //    // verify the template and that it was only loaded once
        //    tpl.ToString().Should().Be(testTemplateData);
        //    mockLoader.Verify(m => m.LoadTemplateTextAsync(fileName), Times.Once);
        //}

        private static Mock<ITemplateLoader> GetTemplateLoader(string fileName, string fileData)
        {
            var moq = new Mock<ITemplateLoader>();

            moq.Setup(m => m.LoadTemplateText(It.Is<string>(i => i == fileName)))
                .Returns(fileData);

            moq.Setup(m => m.LoadTemplateTextAsync(It.Is<string>(i => i == fileName)))
                .ReturnsAsync(fileData);

            return moq;
        }

        //private class AppCacheMock : IAppCache
        //{
        //    private ConcurrentDictionary<string, ITemplate> dic = new ConcurrentDictionary<string, ITemplate>();

        //    public CacheDefaults DefaultCachePolicy { get; } = new CacheDefaults { DefaultCacheDurationSeconds = 1 };

        //    public T GetOrAdd<T>(string key, Func<T> addItemFactory)where T: ITemplate
        //    {
        //        return (T)dic.GetOrAdd(key, (k) => addItemFactory.Invoke());
        //    }

        //    public async Task<T> GetOrAddAsync<T>(string key, Func<Task<T>> addItemFactory)
        //    {
        //        var template = await addItemFactory.Invoke() as ITemplate;
        //        return (T)dic.GetOrAdd(key, (k) => template);
        //    }

        //    public void Remove(string key)
        //    {
        //        dic.TryRemove(key, out var value);
        //    }

        //    #region Explicit Interface Implementations

        //    ICacheProvider IAppCache.CacheProvider => throw new NotImplementedException();

        //    void IAppCache.Add<T>(string key, T item, MemoryCacheEntryOptions policy)
        //    {
        //        throw new NotImplementedException();
        //    }

        //    T IAppCache.Get<T>(string key)
        //    {
        //        throw new NotImplementedException();
        //    }

        //    Task<T> IAppCache.GetAsync<T>(string key)
        //    {
        //        throw new NotImplementedException();
        //    }

        //    T IAppCache.GetOrAdd<T>(string key, Func<ICacheEntry, T> addItemFactory)
        //    {
        //        throw new NotImplementedException();
        //    }

        //    Task<T> IAppCache.GetOrAddAsync<T>(string key, Func<ICacheEntry, Task<T>> addItemFactory)
        //    {
        //        throw new NotImplementedException();
        //    }

        //    #endregion

        //}

    }

}
