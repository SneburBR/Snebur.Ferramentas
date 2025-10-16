using Snebur.Dominio.Atributos;
using Snebur.Utilidade;
using System.Collections.Generic;
using System.Linq;

namespace System.Reflection
{
    public static class ReflexaoExtension
    {
        public static IEnumerable<Type> GetAccessibleTypes(this Assembly assembly)
        {
            try
            {
                //
#if NET40
                return assembly.GetTypes();
#else
                return assembly.DefinedTypes.Select(t => t.AsType());
#endif

            }
            catch (ReflectionTypeLoadException ex)
            {
                return ex.Types.Where(t => t != null);
            }
        }

        public static object TryGetValueOrDefault(this PropertyInfo propriedade, object obj)
        {
            try
            {
                return propriedade.GetValue(obj);
            }
            catch
            {
                return default;
            }
        }

        public static bool TrySetValue(this PropertyInfo propriedade,
                                       object obj,
                                       object value,
                                       bool isLogErro = false)
        {
            try
            {
                if (propriedade.CanRead && propriedade.CanWrite)
                {
                    propriedade.SetValue(obj, value);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                if (isLogErro)
                {
                    var mensagem = $"TrySetValue falha  Propriedade {propriedade.Name} ({propriedade.PropertyType.Name}) " +
                                   $"obj:  {obj?.GetType().Name} {obj?.ToString() ?? "null"}, " +
                                   $"value {value.GetType().Name} {value?.ToString() ?? "null"} ";

                    LogUtil.ErroAsync(new Exception(mensagem, ex));

                }
                return false;
            }
        }

        public static bool IsTipoIguaOuHerda(this Type origem, Type tipo)
        {
            return origem == tipo || origem.IsSubclassOf(tipo);
            //return metodo.GetBaseDefinition().DeclaringType != metodo.DeclaringType;
        }

        public static bool IsOverride(this MethodInfo metodo)
        {
            return metodo.GetBaseDefinition().DeclaringType != metodo.DeclaringType;
        }

        public static bool IsOverride(this PropertyInfo propriedade)
        {
            var getMethod = propriedade.GetGetMethod();
            var setMethod = propriedade.GetSetMethod();
            var getIsOverride = (getMethod?.IsOverride() ?? false);
            var setIsOverride = (setMethod?.IsOverride() ?? false);
            return getIsOverride || setIsOverride;
        }
        /// <summary>
        /// Paliativo, para contornar  a migração do entity framework
        /// </summary>
        /// <param name="propriedade"></param>
        /// <returns></returns>
        public static IChaveEstrangeiraAttribute RetornarAtributoChaveEstrangeira(this PropertyInfo propriedade, bool isIgnorarErro = false)
        {
            var atributoChaveEstrangeira = propriedade.GetCustomAttribute<ChaveEstrangeiraAttribute>();
            if (atributoChaveEstrangeira != null)
            {
                return atributoChaveEstrangeira;
            }
            var atributoChaveEstrangeiraRelacaoUmUm = propriedade.GetCustomAttribute<ChaveEstrangeiraRelacaoUmUmAttribute>();
            if (atributoChaveEstrangeiraRelacaoUmUm != null)
            {
                return atributoChaveEstrangeiraRelacaoUmUm;
            }
            if (isIgnorarErro)
            {
                return null;
            }
            throw new Erro(String.Format("Não foi encontrado um chave estrangeira para a propriedade {0} em {1} ", propriedade.Name, propriedade.DeclaringType.Name));
        }
    }
}