using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TemplateEngine
{

    public interface ITemplateCache
    {
        ITemplate GetTemplate(string fileName);
    }

    public class TemplateCache : ITemplateCache
    {

        protected string templatePath;
        protected int expiresAfterSeconds; 
        protected ConcurrentDictionary<string, CachedTemplate> templates = new ConcurrentDictionary<string, CachedTemplate>();

        public TemplateCache(string templatePath, int expiresAfterMinutes = 0)
        {
            if (string.IsNullOrWhiteSpace(templatePath)) throw new ArgumentException("Invalid template path.");
            if(!Directory.Exists(templatePath)) throw new ArgumentException("Template path does not exist or is inaccessible.");
            if (expiresAfterMinutes < 0) throw new ArgumentException("Expiration minutes must be greater than zero.");

            this.templatePath = templatePath;
            this.expiresAfterSeconds = expiresAfterMinutes;
        }

        public ITemplate GetTemplate(string fileName)
        {
            return CreateTemplate(fileName);

            //// lookup the cached template
            //var cachedTemplate = this.templates.GetOrAdd(fileName, (key) => CreateCachedTemplate(key));
            
            //// if it is new, use it
            //if (cachedTemplate.IsNew)
            //{
            //    cachedTemplate.IsNew = false;
            //    return cachedTemplate.Template;
            //}

            //// if the cached template is expired, replace it and use the new template
            //if (this.expiresAfterSeconds == 0 || cachedTemplate.Expires <= DateTime.UtcNow)
            //{
            //    cachedTemplate = CreateCachedTemplate(fileName);
            //    cachedTemplate.IsNew = false;
            //    this.templates[fileName] = cachedTemplate;
            //    return cachedTemplate.Template;
            //}

            //// otherwise return a copy of the template
            //return new Template(cachedTemplate.Template);
        }

        protected CachedTemplate CreateCachedTemplate(string fileName)
        {
            var template = CreateTemplate(fileName);
            return new CachedTemplate(template, this.expiresAfterSeconds);
        }

        protected Template CreateTemplate(string fileName)
        {
            var filePath = GetFilePath(fileName);
            return new Template(filePath);
        }

        protected string GetFilePath(string FileName)
        {
            return Path.Combine(this.templatePath, FileName);
        }

        protected struct CachedTemplate
        {
            public CachedTemplate(Template template, int expiresAfterSeconds)
            {
                this.Template = template;
                this.Expires = DateTime.UtcNow.AddSeconds(expiresAfterSeconds);
                this.IsNew = true;
            }

            public DateTime Expires { get; set; }
            public bool IsNew { get; set; }
            public Template Template { get; set; }
            
        }

    }

}
