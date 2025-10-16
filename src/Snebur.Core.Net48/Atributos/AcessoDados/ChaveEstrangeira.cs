using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Snebur.Dominio.Atributos
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ChaveEstrangeiraAttribute : ForeignKeyAttribute, IChaveEstrangeiraAttribute
    {
        public string NomePropriedade { get; set; }

        public ChaveEstrangeiraAttribute(string nomePropriedade) : base(nomePropriedade)
        {
            this.NomePropriedade = nomePropriedade;
        }
    }

    public interface IChaveEstrangeiraAttribute
    {
        string NomePropriedade { get; }
        [IgnorarPropriedade]
        string Name { get; }
    }
}