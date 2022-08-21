using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snebur;
using EnvDTE;

namespace Snebur.VisualStudio.Projeto
{
    public abstract class BaseProjeto
    {
        public Project ProjetoVS { get; set; }

        public string NomeProjeto { get; set; }

        public string CaminhoProjeto { get; set; }

        public String CaminhoConfiguracao { get; set; }

        public bool Globalizar { get; set; }

        public BaseProjeto(Project projetoVS, string caminhoProjeto, string caminhoConfiguracao)
        {
            this.ProjetoVS = projetoVS;
            this.CaminhoProjeto = caminhoProjeto;
            this.CaminhoConfiguracao = caminhoConfiguracao;
            this.NomeProjeto = this.ProjetoVS.Name;

        }

        public abstract void Configurar();
    }
}
