using Snebur.Dominio;

namespace Snebur.Comunicacao
{
    public class ResultadoExisteIdentificadoUsuario : BaseDominio
    {
        public string Nome { get; set; }

        public bool IsExiste { get; set; }
    }
}