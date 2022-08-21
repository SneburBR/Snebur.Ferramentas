using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snebur;
using System.Reflection;
using Snebur.VisualStudio.Utilidade;
using Snebur.Utilidade;

namespace Snebur.VisualStudio.Projeto.Dominio.Estrutura
{
    public class EstruturaPropriedadeMensagemValidacao : EstruturaPropriedadeEstatica
    {
   

        public EstruturaPropriedadeMensagemValidacao(PropertyInfo propriedade) : base(propriedade)
        {

        }

        public override List<string> RetornarLinhasTypeScript(string tabInicial)
        {
            var linha = string.Format("{0}public static {1} : string = \"{2}\"; ", tabInicial, this.RetornarNomePropriedadeIdentificadorMensagemValidacao(),   this.Propriedade.Name);
            return new List<string> { linha };
        }

        private string RetornarNomePropriedadeIdentificadorMensagemValidacao()
        {
            var nomePropriedade = this.Propriedade.Name;
            var partes = new List<string>();
            partes.Add("IDENTIFICADOR");
            var len = nomePropriedade.Length;
            var posicaoUltimaDivisao = 0;
            var fim = 0;
            for (var i = 0; i < len; i++)
            {
                var caracter = nomePropriedade[i];
                if (Char.IsUpper(caracter))
                {
                     fim = i - posicaoUltimaDivisao;
                    var parte = nomePropriedade.Substring(posicaoUltimaDivisao, fim);
                    if (!String.IsNullOrEmpty(parte))
                    {
                        partes.Add(parte);
                    }
                    

                    posicaoUltimaDivisao = i;
                }
            }
            fim = nomePropriedade.Length - posicaoUltimaDivisao;
            var ultimaParte = nomePropriedade.Substring(posicaoUltimaDivisao, fim);
            if (!String.IsNullOrEmpty(ultimaParte))
            {
                partes.Add(ultimaParte);
            }
            return String.Join("_", partes.Select(x => x.ToUpper()));
        }


    }
}
