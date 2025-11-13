using Snebur.Dominio.Atributos;
using Snebur.Utilidade;
using Snebur.VisualStudio.Reflexao;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Snebur.VisualStudio
{
    public partial class HtmlIntelliSense : IDisposable
    {
        private const string INICIO = "<!--Snebur-->";
        private const string FIM = "<!--fim-Snebur-->";

        private const string INICIO_REFERENCIA = "<!--Snebur-referencia-->";
        private const string FIM_REFERENCIA = "<!--fim-Snebur-referencia-->";

        private const string PROCURAR_INCLUDE = "<xsd:include schemaLocation=\"CommonHTML5Types.xsd\" />";
        private const string PROCURAR_COMMON_ATRIBUTES = "<xsd:attributeGroup name=\"commonAttributeGroup\">";
        private const string PROCURAR_FLOW_CONTENT = "<xsd:group name=\"flowContent\">";

        private const string LINHA_REF_GRUPO_ATRIBUTOS = "<xsd:attributeGroup ref=\"GrupoSneburAtributos\" />";
        public const string PREFIXO_ZS_PROPRIEDADE = "vs:customattrprefix=\"sn-prop-\"";
        public const string PREFIXO_DATA = "vs:customattrprefix=\"data-\"";


        private HtmlIntelliSense()
        {

        }

        public void Atualizar()
        {
            if (File.Exists(CaminhosUtil.CaminhoControlesTypescript) &&
                File.Exists(CaminhosUtil.CaminhoAtributosTypescript))
            {
                var linhas = this.RetornarConteudo();

                DeletarArquivoBinario(CaminhosUtil.CaminhoSchemaXHTML5);
                DeletarArquivoBinario(CaminhosUtil.CaminhoSchemaHTML5);

                this.AtualizarConteudo(CaminhosUtil.CaminhoSchemaXHTML5, linhas.ToList());
                this.AtualizarConteudo(CaminhosUtil.CaminhoSchemaHTML5, linhas.ToList());

            }
        }

        private void DeletarArquivoBinario(string caminhoSchemaXHTML5)
        {
            var caminho = Path.ChangeExtension(caminhoSchemaXHTML5, ".bin");
            if (File.Exists(caminho))
            {
                LogVSUtil.Alerta($" Deletando binário do schema '{Path.GetFileName(caminho)}' foi deletado para forçar a recompilação do IntelliSense.");
                try
                {
                    ArquivoUtil.DeletarArquivo(caminho);

                }
                catch(Exception ex)
                {
                    LogVSUtil.LogErro($" Erro ao deletar o binário do schema '{Path.GetFileName(caminho)}'. Tente fechar o Visual Studio e excluir manualmente o arquivo.", ex);
                }
            }
        }

        private void AtualizarConteudo(string caminhoSchema, List<string> linhas)
        {
            this.ConferirBackupSchema(caminhoSchema);
            var conteudo = File.ReadAllText(caminhoSchema, Encoding.Default);

            var linhasSchemaHtml = conteudo.ToLines();

            if (!linhasSchemaHtml.Any(x => x.Trim().Contains(INICIO)))
            {
                this.AdicionarNovoConteudo(caminhoSchema, linhasSchemaHtml, linhas);
            }
            else
            {
                this.AtualizarConteudoAtual(caminhoSchema, linhasSchemaHtml, linhas);
            }
        }

        private void AdicionarNovoConteudo(string caminhoSchema, List<string> linhasSchemaHtml, List<string> linhas)
        {
            linhas.Insert(0, String.Format("\t{0}", INICIO));
            linhas.Add(String.Format("\t{0}", FIM));
            linhas.Insert(0, String.Empty);

            var posicaoInserirConteudo = linhasSchemaHtml.IndexOf(linhasSchemaHtml.Where(x => x.Trim().StartsWith(PROCURAR_INCLUDE)).Single());
            if (!(posicaoInserirConteudo > 0))
            {
                throw new Erro("Não foi encontrado a linha do include");
            }

            linhasSchemaHtml.InsertRange(posicaoInserirConteudo + 1, linhas);

            var posicaoCommonAttributes = linhasSchemaHtml.IndexOf(linhasSchemaHtml.Where(x => x.Trim().StartsWith(PROCURAR_COMMON_ATRIBUTES)).Single());
            linhasSchemaHtml.Insert(posicaoCommonAttributes + 1, String.Format("\t\t{0}", LINHA_REF_GRUPO_ATRIBUTOS));

            var linhasReferencia = this.RetornarLinhasReferencias();

            linhasReferencia.Insert(0, String.Format("\t\t\t{0}", INICIO_REFERENCIA));
            linhasReferencia.Add(String.Format("\t\t\t{0}", FIM_REFERENCIA));

            var posicaoReferencia = linhasSchemaHtml.IndexOf(linhasSchemaHtml.Where(x => x.Trim().StartsWith(PROCURAR_FLOW_CONTENT)).Single());
            linhasSchemaHtml.InsertRange(posicaoReferencia + 3, linhasReferencia);

            var conteudo = String.Join(System.Environment.NewLine, linhasSchemaHtml);
            conteudo = conteudo.Replace(PREFIXO_DATA, PREFIXO_ZS_PROPRIEDADE);
            ArquivoUtil.SalvarTexto(caminhoSchema, conteudo, Encoding.Default);
        }

        private void AtualizarConteudoAtual(string caminhoSchema, List<string> linhasSchemaHtml, List<string> linhas)
        {
            var posicaoInicio = linhasSchemaHtml.IndexOf(linhasSchemaHtml.Where(x => x.Trim().StartsWith(INICIO)).Single());
            var posicaoFim = linhasSchemaHtml.IndexOf(linhasSchemaHtml.Where(x => x.Trim().StartsWith(FIM)).Single());

            linhasSchemaHtml.RemoveRange(posicaoInicio + 1, (posicaoFim - posicaoInicio) - 1);
            linhasSchemaHtml.InsertRange(posicaoInicio + 1, linhas);

            var posicaoInicioReferencia = linhasSchemaHtml.IndexOf(linhasSchemaHtml.Where(x => x.Trim().StartsWith(INICIO_REFERENCIA)).Single());
            var posicaoFimReferencia = linhasSchemaHtml.IndexOf(linhasSchemaHtml.Where(x => x.Trim().StartsWith(FIM_REFERENCIA)).Single());

            var linhasReferencia = this.RetornarLinhasReferencias();
            linhasSchemaHtml.RemoveRange(posicaoInicioReferencia + 1, (posicaoFimReferencia - posicaoInicioReferencia) - 1);
            linhasSchemaHtml.InsertRange(posicaoInicioReferencia + 1, linhasReferencia);

            var conteudo = String.Join(System.Environment.NewLine, linhasSchemaHtml);
            conteudo = conteudo.Replace(PREFIXO_DATA, PREFIXO_ZS_PROPRIEDADE);
            ArquivoUtil.SalvarTexto(caminhoSchema, conteudo, Encoding.Default);
        }

        private List<string> RetornarConteudo()
        {
            var linhas = new List<string>();
            linhas.Add(String.Format("\t<xsd:attributeGroup name=\"GrupoSneburAtributos\">"));
            foreach (var linha in this.RetornarLinhasConteudoAtributos())
            {
                linhas.Add(String.Format("\t\t{0}", linha));
            }
            linhas.Add(String.Format("\t</xsd:attributeGroup>"));
            linhas.Add(String.Empty);

            foreach (var linha in this.RetornarLinhasConteudoControles())
            {
                linhas.Add(String.Format("\t{0}", linha));
            }

            foreach (var linha in this.RetornarLinhasConteudoComponentesApresentacao())
            {
                linhas.Add(String.Format("\t{0}", linha));
            }

            return linhas;
        }

        #region Atributos

        private List<string> RetornarLinhasConteudoAtributos()
        {
            var atributosVM = HtmlIntelliSenseUtils.GetSneburAttributes();
            var linhas = new List<string>();

            foreach (var atributoVM in atributosVM)
            {
                var linhasAtributo = this.RetornarConteudoAtributo(atributoVM.Name, atributoVM.TypeValye);
                linhas.AddRange(linhasAtributo);
            }
            return linhas;
        }

       
        private List<string> RetornarConteudoAtributo(string nomeAtributp, string tipo)
        {
            var linhas = new List<string>();
            if (tipo.Contains(".") || tipo == nameof(Boolean))
            {

                //linhas.Add(String.Format("<xsd:attribute name=\"{0}\" vs:standalone=\"noname\" >", nomeAtributp));
                linhas.Add(String.Format("<xsd:attribute name=\"{0}\"  >", nomeAtributp));

                linhas.Add("\t<xsd:simpleType>");
                linhas.Add("\t\t<xsd:union>");

                //var nomeTipoEnum = nomeAtributp + "_enum";

                linhas.Add($"\t\t\t<xsd:simpleType>");

                linhas.Add(String.Format("\t\t\t\t<xsd:restriction base=\"xsd:string\">"));
                //linhas.Add(String.Format("\t\t\t\t<xsd:restriction base=\"xsd:NMTOKEN\">"));

                var valoresEnum = this.RetornarValoresEnum(tipo);
                foreach (var valorEnum in valoresEnum)
                {
                    linhas.Add(String.Format("\t\t\t\t\t<xsd:enumeration value=\"{0}\" />", valorEnum));
                }

                linhas.Add("\t\t\t\t</xsd:restriction>");
                linhas.Add("\t\t\t</xsd:simpleType>");

                linhas.Add($"\t\t\t<xsd:simpleType>");
                linhas.Add("\t\t\t\t<xsd:restriction base=\"xsd:string\">");

                linhas.Add("\t\t\t\t</xsd:restriction>");
                linhas.Add("\t\t\t</xsd:simpleType>");
                linhas.Add("\t\t</xsd:union>");
                linhas.Add("\t</xsd:simpleType>");
                linhas.Add("</xsd:attribute>");

            }
            else
            {
                //var descricaoTipo = (tipo == "Event") ? "vs:omtype=\"event\"" : String.Format("type=\"xsd:{0}\"", tipo.ToLower());
                var descricaoTipo = this.RetornarTipo(tipo);
                linhas.Add(String.Format("<xsd:attribute name=\"{0}\" {1} />", nomeAtributp, descricaoTipo));
            }
            return linhas;
        }

        private List<string> RetornarValoresEnum(string tipo)
        {
            var valores = new List<string>();
            if (tipo == nameof(Boolean))
            {
                valores.Add(true.ToString().ToLower());
                valores.Add(false.ToString().ToLower());
            }
            else
            {
                var partes = tipo.Split('.');
                var nome = partes.Last();
                var _namespace = String.Join(".", partes.Take(partes.Length - 1));
                var tipoEnum = AjudanteAssembly.RetornarTipo(nome);
                return this.RetornarNomesEnum(tipoEnum);
            }
            return valores;
        }

        private List<string> RetornarNomesEnum(Type tipoEnum)
        {
            var retorno = new List<string>();
            var valoresEnum = EnumUtil.RetornarValoresEnum(tipoEnum);

            foreach (var valorEnum in valoresEnum)
            {
                var membroInfo = tipoEnum.GetField(valorEnum.ToString());
                var atriutosRotuloVS = PropriedadeUtil.RetornarAtributos(membroInfo, AjudanteAssembly.TipoAtributoRotuloVSIntelliSense, true);
                if (atriutosRotuloVS.Count > 0)
                {
                    foreach (var atributoVS in atriutosRotuloVS)
                    {
                        var descricao = ReflexaoUtil.RetornarValorPropriedade<RotuloVSIntelliSenseAttribute>(x => x.Rotulo, atributoVS);
                        retorno.Add(ConverterUtil.ParaString(descricao));
                    }
                }
                else
                {
                    retorno.Add(membroInfo.Name);
                }
            }
            return retorno;
        }

        private string RetornarTipo(string tipo)
        {
            if ((tipo == "Event"))
            {
                return "vs:omtype=\"event\"";
            }
            var tipoSimples = this.RetornarTipoSimples(tipo);
            return String.Format("type=\"xsd:{0}\"", tipoSimples);
        }
        private string RetornarTipoSimples(string tipo)
        {
            switch (tipo)
            {
                case "Number":

                    return "float";

                default:

                    return "string";
            }
        }

        #endregion

        private List<string> RetornarLinhasReferencias()
        {
            var linhasReferencias = new List<string>();
            var linhasReferenciaControles = this.RetornarLinhasReferenciasControles();
            var linhasReferenciaCompomentesApresentacao = this.RetornarLinhasReferenciasComponentesApresentacao();

            linhasReferencias.AddRange(linhasReferenciaControles);
            linhasReferencias.AddRange(linhasReferenciaCompomentesApresentacao);

            return linhasReferencias;
        }

        #region Controles Snebur

        private List<string> RetornarLinhasReferenciasControles()
        {
            var tags = HtmlIntelliSenseUtils.GetSneburTagElements();
            var linhas = new List<string>();
            foreach (var tag in tags)
            {
                linhas.Add(String.Format("\t\t\t<xsd:element ref=\"{0}\" />", tag.TagName.ToLower()));
            }
            return linhas;
        }

        private List<string> RetornarLinhasConteudoControles()
        {
            var tags = HtmlIntelliSenseUtils.GetSneburTagElements();
            var linhas = new List<string>();
            foreach (var tag in tags)
            {
                linhas.Add(String.Format("<xsd:element name=\"{0}\" type=\"simpleFlowContentElement\" />", tag.TagName.ToLower()));
            }
            return linhas;
        }

       

        #endregion

        #region Elementos Apresentacao

        private List<string> RetornarLinhasReferenciasComponentesApresentacao()
        {
            var tags = this.RetornarTagsComponentesApresentacao();
            var linhas = new List<string>();
            foreach (var tag in tags)
            {
                linhas.Add(String.Format("\t\t\t<xsd:element ref=\"{0}\" />", tag.ToLower()));
            }
            return linhas;
        }

        private List<string> RetornarLinhasConteudoComponentesApresentacao()
        {
            var tags = this.RetornarTagsComponentesApresentacao();
            var linhas = new List<string>();
            foreach (var tag in tags)
            {
                linhas.Add(String.Format("<xsd:element name=\"{0}\" type=\"simpleFlowContentElement\" />", tag.ToLower()));
            }
            return linhas;
        }

        private List<string> RetornarTagsComponentesApresentacao()
        {
            var procurar = "$ComponentesApresentacao.Add(\"";
            var tags = new List<string>();
            var linhas = File.ReadAllLines(CaminhosUtil.CaminhoComponentesApresentacaoTypescript, Encoding.UTF8);
            var len = linhas.Length;
            for (var i = 0; i < len; i++)
            {
                var linha = linhas[i].Trim();
                if (linha.StartsWith(procurar))
                {
                    var fim = linha.IndexOf(",");
                    var tag = linha.Trim().Substring(procurar.Length, fim - procurar.Length);
                    tag = tag.Replace("\"", String.Empty);
                    tags.Add(tag);
                }
            }
            return tags;
        }

        #endregion

        #region Métodos privados

        private void ConferirBackupSchema(string caminho)
        {
            var fi = new FileInfo(caminho);
            var fiBkp = new FileInfo(Path.Combine(fi.Directory.FullName, "bkp", fi.Name));

            if (!fiBkp.Exists)
            {
                DiretorioUtil.CriarDiretorio(fiBkp.Directory.FullName);
                fi.CopyTo(fiBkp.FullName);
            }
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {

        }

        #endregion
    }
}
