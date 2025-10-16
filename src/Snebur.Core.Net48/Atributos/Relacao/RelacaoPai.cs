using System;

namespace Snebur.Dominio.Atributos
{
    [AttributeUsage(AttributeTargets.Property)]
    public class RelacaoPaiAttribute : RelacaoChaveEstrangeiraAttribute, IIgnorarAlerta
    {
        public bool IgnorarAlerta { get; set; }

        public EnumTipoExclusaoRelacao TipoExclusao { get; set; } = EnumTipoExclusaoRelacao.NaoDeletar;

        public RelacaoPaiAttribute(EnumTipoExclusaoRelacao tipoExclusao = EnumTipoExclusaoRelacao.NaoDeletar)
        {
            this.TipoExclusao = tipoExclusao;
        }
    }

  
}