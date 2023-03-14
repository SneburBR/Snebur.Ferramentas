using Snebur.Dominio;
using System.Collections.Generic;

namespace Snebur.VisualStudio
{
    public class ProjetoViewModel
    {
        public object ProjetoVS { get; }
        public List<PropriedadeViewModel> PropriedadesViewModel { get; }
        public EnumTipoCsProj TipoCsProj { get; }

        public ProjetoViewModel(object projetoVS,
                                List< PropriedadeViewModel> propriedadesVM)

        {
            this.ProjetoVS = projetoVS;
            this.PropriedadesViewModel = propriedadesVM;
            this.TipoCsProj = TipoCsProjUtil.RetornarTipoCsProjet(projetoVS);
        }
 
    }

    public static class TipoCsProjUtil
    {
        public static EnumTipoCsProj RetornarTipoCsProjet(object projetoVS)
        {
            var nomeTipoProjeto = projetoVS.GetType().Name;
            switch (nomeTipoProjeto)
            {
                case "OAProject":
                    return EnumTipoCsProj.MicrosoftSdk;
                default:
                    
                    return EnumTipoCsProj.Tradicional;

                    //throw new System.Exception("Tipo de projeto não suportado");
                    
            }
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


