using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Snebur.VisualStudio.Utilidade
{

    public class JsonUtil
    {

        private static readonly JsonSerializerSettings ConfiguracoesSerializarJavascript = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.None,
            DateFormatHandling = DateFormatHandling.MicrosoftDateFormat,
            ReferenceLoopHandling = ReferenceLoopHandling.Serialize

        };

        public static string Serializar(object objeto)
        {
            var formatacaoJson = Formatting.Indented;
            var configuracaoSerializacao = JsonUtil.ConfiguracoesSerializarJavascript;
            var json = JsonConvert.SerializeObject(objeto, formatacaoJson, configuracaoSerializacao);
            return json;
        }

        public static T Deserializar<T>(string json)
        {
            var configuracaoSerializacao = JsonUtil.ConfiguracoesSerializarJavascript;
            var objeto = (T)JsonConvert.DeserializeObject(json, typeof(T), configuracaoSerializacao);
            return objeto;
        }


    }

}
