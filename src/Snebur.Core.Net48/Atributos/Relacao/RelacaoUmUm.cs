using System;
using System.Diagnostics;

namespace Snebur.Dominio.Atributos
{
    [AttributeUsage(AttributeTargets.Property)]
    public class RelacaoUmUmAttribute : RelacaoChaveEstrangeiraAttribute, IIgnorarAlerta
    {
        public bool IgnorarAlerta { get; set; }

        public EnumTipoExclusaoRelacao TipoExclusao { get; set; } = EnumTipoExclusaoRelacao.NaoDeletar;

        public RelacaoUmUmAttribute()
        {
            Trace.TraceWarning("RelacaoUmUmAttribute is not implementada");
        }
    }
}