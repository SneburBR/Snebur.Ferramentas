using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Snebur.Utilidade;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Snebur.Serializacao
{
    internal abstract class JsonPropertiesResolver : DefaultContractResolver
    {
        protected abstract EnumTipoSerializacao TipoSerializacao { get; }
        protected override List<MemberInfo> GetSerializableMembers(Type objectType)
        {
            return JsonUtil.RetornarPropriedadesSerializavel(objectType, true, this.TipoSerializacao);
        }

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var jsonPropriedade = base.CreateProperty(member, memberSerialization);
            if (member is PropertyInfo propriedade)
            {
                if (propriedade.PropertyType != typeof(string))
                {
                    if (propriedade.PropertyType.GetInterface(nameof(ICollection)) != null)
                    {
                        jsonPropriedade.ShouldSerialize = (instancia) =>
                        {
                            try
                            {
                                var valorPropriedade = propriedade.GetValue(instancia);
                                if (valorPropriedade is ICollection colecao)
                                {
                                    return colecao.Count > 0;
                                }
                            }
                            catch
                            {
                            }
                            return false;
                        };
                        //jsonPropriedade.ShouldSerialize =
                        //    instance => (instance?.GetType().GetProperty(jsonPropriedade.PropertyName).GetValue(instance) as IEnumerable<object>)?.Count() > 0;
                    }
                }
            }
            return jsonPropriedade;
        }
    }

    internal class JsonPropertiesResolverJavascript : JsonPropertiesResolver
    {
        protected override EnumTipoSerializacao TipoSerializacao => EnumTipoSerializacao.Javascript;
    }

    internal class JsonPropertiesResolverDotNet : JsonPropertiesResolver
    {
        protected override EnumTipoSerializacao TipoSerializacao => EnumTipoSerializacao.DotNet;
    }
}
