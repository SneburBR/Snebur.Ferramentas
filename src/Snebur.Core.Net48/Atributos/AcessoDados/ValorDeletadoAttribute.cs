using System;

namespace Snebur.Dominio.Atributos
{

    [IgnorarAtributoTS]
    [IgnorarTSReflexao]
    public class ValorDeletadoAttribute : Attribute
    {
        public object Valor { get; set; }
        public ValorDeletadoAttribute(object valor)
        {
            this.Valor = valor;
        }
    }
    //public class ValorDeletadoAttribute : Attribute
    //{

    //}
}