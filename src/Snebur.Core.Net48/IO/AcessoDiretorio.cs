using Snebur.Utilidade;
using System;

namespace Snebur.IO
{
    public class AcessoDiretorio : IAcessoDiretorio
    {
        public string Caminho { get; set; }

        public bool IsAutenticar { get; set; }

        public bool IsRede { get; set; }

        public string Dominio { get; set; }

        public string Usuario { get; set; }

        public string Senha { get; set; }

        public AcessoDiretorio()
        {
        }

        public AcessoDiretorio(bool isAutenticar,
                               string dominio,
                               string usuario,
                               string senha)
        {
            this.IsAutenticar = isAutenticar;
            this.Dominio = dominio;
            this.Usuario = usuario;
            this.Senha = senha;
        }
        public string NomeComputador
        {
            get
            {
                if (this.IsRede)
                {
                    return DiretorioUtil.RetornarNomeComputador(this.Caminho);
                }
                return String.Empty;
            }
        }
    }
}