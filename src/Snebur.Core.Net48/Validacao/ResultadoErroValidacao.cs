namespace Snebur.Dominio
{
    public class ErroValidacao : BaseDominio
    {
        public string NomeTipoEntidade { get; set; }

        public string NomePropriedade { get; set; }

        public string NomeTipoValidacao { get; set; }

        public string Mensagem { get; set; }
    }
}
