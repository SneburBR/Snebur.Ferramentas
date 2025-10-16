using System;

namespace Snebur.Dominio.Atributos
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ValorEnumStringAttribute : BaseAtributoDominio
    {
        public string Valor { get; }

        public ValorEnumStringAttribute(string valor)
        {
            this.Valor = valor;
        }
    }
}

