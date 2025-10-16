using System;
using System.IO;

namespace Snebur.VisualStudio
{
    public class ProjetoViewModel
    {
        public string CaminhoProjetoCsProj { get; }
        public object ProjetoVS { get; }
        public EnumTipoCsProj TipoCsProj { get; }

        public ProjetoViewModel(string caminhoProjetoCsProj,
                                object projetoVS )

        {
            if(Path.GetExtension(caminhoProjetoCsProj) != ".csproj")
            {
                throw new  Exception($"Extensão do projeto não suportado {caminhoProjetoCsProj}");
            }

            if (!File.Exists(caminhoProjetoCsProj))
            {
                throw new FileNotFoundException(caminhoProjetoCsProj);
            }

            this.CaminhoProjetoCsProj = caminhoProjetoCsProj;
            this.ProjetoVS = projetoVS;
            
            this.TipoCsProj = ProjetoUtil.RetornarTipoCsProjet(caminhoProjetoCsProj);
        }

    }

    public enum EnumTipoCsProj
    {
        Tradicional,
        MicrosoftSdk
    }

    public class PropriedadeViewModel
    {
        public PropriedadeViewModel(string name, object value)
        {
            this.Name = name;
            this.Value = value;
        }

        public string Name { get; set; }
        public object Value { get; set; }
    }

}

