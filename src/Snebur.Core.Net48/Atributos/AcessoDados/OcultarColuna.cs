using System;
using System.ComponentModel.DataAnnotations;

namespace Snebur.Dominio.Atributos
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class OcultarColunaAttribute : ScaffoldColumnAttribute
    {
        public OcultarColunaAttribute() : base(false)
        {
        }
    }
}