using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using RazorLight;

namespace EmailClient.Helpers
{
    public class RazorParser
    {
        private Assembly _assembly;
        private readonly IMemoryCache memoryCache;
        private static object __lockObj = new object();

        public RazorParser(Assembly assembly, IMemoryCache memoryCache)
        {
            this._assembly = assembly;
            this.memoryCache = memoryCache;
        }

        public string Parse<T>(string templateName, string template, T model)
        {
            return ParseAsync(templateName, template, model).GetAwaiter().GetResult();
        }

        public string UsingTemplateFromEmbedded<T>(string path, T model)
        {
            string templateName = GetTemplateName(path);
            string template = memoryCache.Get<string>(templateName);

            if(string.IsNullOrEmpty(template))
            {
                lock (__lockObj)
                {
                    // Your code...
                    template = memoryCache.GetOrCreate(templateName, f =>
                    {
                        string resource = EmbeddedResourceHelper.GetResourceAsString(_assembly, GenerateFileAssemblyPath(path, _assembly));
                        return resource;
                    });
                }
            }

            var result = Parse(templateName, template, model);
            return result;
        }

        async Task<string> ParseAsync<T>(string templateName, string template, T model)
        {
            var project = new InMemoryRazorLightProject();
            var engine = new EngineFactory().Create(project);
            
            return await engine.CompileRenderAsync<T>(templateName, template, model);
        }

        string GenerateFileAssemblyPath(string template, Assembly assembly)
        {
            string assemblyName = assembly.GetName().Name;
            return string.Format("{0}.{1}.{2}", assemblyName, template, "cshtml");
        }

        private string GetTemplateName(string path)
        {
            return string.IsNullOrEmpty(path) ? "" : path.Replace(".", "");
        }
    }
}
