namespace Snebur.Dominio.Atributos
{
    public class TabelaSistemaAttribute : TabelaAttribute
    {
        public const string SCHEMA_SISTEMA = "sistema";
        public TabelaSistemaAttribute(string nomeTabela) : base(nomeTabela, SCHEMA_SISTEMA)
        {

        }
    }
}