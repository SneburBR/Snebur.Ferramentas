using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Snebur.VisualStudio
{
    public class ArquivoTSDominio : BaseArquivoTypeScript
    { 
        public int PrioridadeDominio { get; set; }
        public string Versao { get; set; }
        public bool Globalizar { get; set; }
        public List<string> DominiosDepedentes { get; set; }

        public ArquivoTSDominio(  FileInfo arquivo, int prioridadeProjeto) : base(arquivo, prioridadeProjeto)
        {
            this.PrioridadeDominio = this.RetornarPrioridadeDominio();
        }
         
        protected override EnumTipoArquivoTypeScript RetornarTipoArquivoTypeScript()
        {
             if (this.Arquivo.Name.EndsWith(".Atributos.ts"))
            {
                return EnumTipoArquivoTypeScript.DominioAtributos;
            }

            if (this.Arquivo.Name.EndsWith(".Classes.ts"))
            {
                return EnumTipoArquivoTypeScript.DominioClasses;
            }

            if (this.Arquivo.Name.EndsWith(".Enums.ts"))
            {
                return EnumTipoArquivoTypeScript.DominioEnums;
            }

            if (this.Arquivo.Name.EndsWith(".Reflexao.ts"))
            {
                return EnumTipoArquivoTypeScript.DominioReflexao;
            }

            if (this.Arquivo.Name.EndsWith(".Interfaces.ts"))
            {
                return EnumTipoArquivoTypeScript.DominioInterfaces;
            }

            if (this.Arquivo.Name.EndsWith(".Constantes.ts"))
            {
                return EnumTipoArquivoTypeScript.DominioConstantes;
            }

            throw new NotSupportedException("");
        }

        protected override string RetornarNamespace()
        {
            return this.RetornarValorAtributo("Namespace");
        }

        protected override string RetornarCaminhoClasseBase()
        {
            return null;
        }

        private int RetornarPrioridadeDominio()
        {
            return Convert.ToInt32(this.RetornarValorAtributo("PrioridadeDominio"));
        }

        private string RetornarValorAtributo(string nomeAtributo)
        {
            var procurar = String.Format("//@{0}:", nomeAtributo);
            var linhasAtributo = this.Linhas.Where(x => x.StartsWith(procurar)).ToList();
            if (linhasAtributo.Count == 0)
            {
                throw new Exception(String.Format(" A linha do namespace não foi encontrado no Arquivo do dominio {0}", this.Arquivo.FullName));
            }

            if (linhasAtributo.Count > 1)
            {
                throw new Exception(String.Format(" Existe mais de uma linha de namespace  no Arquivo do dominio {0}", this.Arquivo.FullName));
            }
            var linhaAtributo = linhasAtributo.Single();
            var valorAtributo = linhaAtributo.Substring(procurar.Length).Trim();
            return valorAtributo;
        }
 
    }
}
