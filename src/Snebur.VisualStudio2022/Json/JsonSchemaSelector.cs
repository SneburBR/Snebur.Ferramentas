using Microsoft.WebTools.Languages.Json.Schema;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Snebur.VisualStudio.Json
{
    [Export(typeof(IJsonSchemaSelector))]
    internal class JsonSchemaSelector :IJsonSchemaSelector
    {
        public event EventHandler AvailableSchemasChanged { add { } remove { } }


        public static Dictionary<string, string> JsonSchemasMaths = new Dictionary<string, string>
        {
            {"publicacao.json", "json/publicacao-schema.json" },
            {"dominio.json", "json/dominio-schema.json" },
            {"version.json", "json/version-schema.json" },
            {"versions.json", "json/versions-schema.json" },
        };
         
        public Task<IEnumerable<string>> GetAvailableSchemasAsync()
        {
            return Task.FromResult(Enumerable.Empty<string>());
        }
        public string GetSchemaFor(string fileLocation)
        {
            var fileName = Path.GetFileName(fileLocation);
            if (JsonSchemasMaths.ContainsKey(fileName))
            {
                var diretorio = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                var pathSchema =  Path.Combine(diretorio, JsonSchemasMaths[fileName]);
                if (File.Exists(pathSchema))
                {
                    return pathSchema;
                }
            }
            return null;
        }
    }
}
