using System;

namespace Snebur.Dominio.Atributos
{
    /// <summary>
    /// Não herdade NotMappedAttribute, uma vez que no EntityFramework tudo da sua entidades especializadas não serão mapeadas
    /// </summary>
    [IgnorarAtributoTS]
    [AttributeUsage(AttributeTargets.Class)]
    public class NaoCriarTabelaEntidadeAttribute : Attribute
    {
    }
}