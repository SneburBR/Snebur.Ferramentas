using Snebur.Dominio;
using Snebur.Utilidade;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

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
            
            this.TipoCsProj = TipoCsProjUtil.RetornarTipoCsProjet(caminhoProjetoCsProj);
        }

    }

    public static class TipoCsProjUtil
    {
        public static EnumTipoCsProj RetornarTipoCsProjet(string caminhoProjetoCsProj)
        {
            using(var fs =   StreamUtil.OpenRead(caminhoProjetoCsProj))
            {
                var xml = new XmlDocument();
                xml.Load(fs);

                var atributoSdk = xml.GetElementsByTagName("Project")[0].Attributes["Sdk"];
                if (atributoSdk!= null && atributoSdk.Value == "Microsoft.NET.Sdk")
                {
                    return EnumTipoCsProj.MicrosoftSdk;
                }
                return EnumTipoCsProj.Tradicional;
            }
        }

        //public static EnumTipoCsProj RetornarTipoCsProjet(object projetoVS)
        //{
        //    var nomeTipoProjeto = projetoVS.GetType().Name;
        //    switch (nomeTipoProjeto)
        //    {
        //        case "OAProject":
        //            return EnumTipoCsProj.MicrosoftSdk;
        //        default:

        //            return EnumTipoCsProj.Tradicional;

        //            //throw new System.Exception("Tipo de projeto não suportado");

        //    }
        //}

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


