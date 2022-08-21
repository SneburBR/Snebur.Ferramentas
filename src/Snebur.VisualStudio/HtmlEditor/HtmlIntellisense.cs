using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snebur;
using Snebur.Utilidade;
using Snebur.Dominio;
using System.IO;

namespace Snebur.VisualStudio
{
    public partial class HtmlIntellisense : IDisposable
    {
        private const string INICIO = "<!--Snebur-->";
        private const string FIM = "<!--fim-Snebur-->";

        private const string INICIO_REFERENCIA = "<!--Snebur-referencia-->";
        private const string FIM_REFERENCIA = "<!--fim-Snebur-referencia-->";

        private const string PROCURAR_INCLUDE = "<xsd:include schemaLocation=\"CommonHTML5Types.xsd\" />";
        private const string PROCURAR_COMMON_ATRIBUTES = "<xsd:attributeGroup name=\"commonAttributeGroup\">";
        private const string PROCURAR_FLOW_CONTENT = "<xsd:group name=\"flowContent\">";

        private const string LINHA_REF_GRUPO_ATRIBUTOS = "<xsd:attributeGroup ref=\"GrupoSneburAtributos\" />";

        private HtmlIntellisense()
        {

        }

        public void Atualizar()
        {
            if (File.Exists(RepositorioSchemaHtml.CaminhoControlesSnebur) && 
                File.Exists(RepositorioSchemaHtml.CaminhoAtributosSnebur))
            {
                var linhas = this.RetornarConteudo();
                this.AtualizarConteudo(RepositorioSchemaHtml.CaminhoSchemaXHTML5, linhas.ToList());
                this.AtualizarConteudo(RepositorioSchemaHtml.CaminhoSchemaHTML5, linhas.ToList());
            }
        }

        private void AtualizarConteudo(string caminhoSchema, List<string> linhas)
        {
            this.ConferirBackupSchema(caminhoSchema);
            var linhasSchemaHtml = File.ReadAllLines(caminhoSchema, Encoding.Default).ToList();

            if (!linhasSchemaHtml.Any(x => x.Trim().StartsWith(INICIO)))
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

            var linhasReferencia = this.RetornarLinhasReferenciasControles();
            linhasReferencia.Insert(0, String.Format("\t\t\t{0}", INICIO_REFERENCIA));
            linhasReferencia.Add(String.Format("\t\t\t{0}", FIM_REFERENCIA));

            var posicaoReferencia = linhasSchemaHtml.IndexOf(linhasSchemaHtml.Where(x => x.Trim().StartsWith(PROCURAR_FLOW_CONTENT)).Single());
            linhasSchemaHtml.InsertRange(posicaoReferencia + 3, linhasReferencia);

            var conteudo = string.Join(System.Environment.NewLine, linhasSchemaHtml);
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

            var linhasReferencia = this.RetornarLinhasReferenciasControles();
            linhasSchemaHtml.RemoveRange(posicaoInicioReferencia + 1, (posicaoFimReferencia - posicaoInicioReferencia) - 1);
            linhasSchemaHtml.InsertRange(posicaoInicioReferencia + 1, linhasReferencia);

            var conteudo = string.Join(System.Environment.NewLine, linhasSchemaHtml);
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
            return linhas;
        }

        #region Atributos

        private List<string> RetornarLinhasConteudoAtributos()
        {
            var atributosVM = this.RetornarAtributosVM();
            var linhas = new List<string>();

            foreach (var atributoVM in atributosVM)
            {
                var linhasAtributo = this.RetornarConteudoAtributo(atributoVM.Item1, atributoVM.Item2);
                linhas.AddRange(linhasAtributo);

            }

            return linhas;
        }

        private List<Tuple<string, string>> RetornarAtributosVM()
        {
            var atributos = new List<Tuple<string, string>>();
            var procurar = "AtributoHtml(\"";
            var linhas = File.ReadAllLines(RepositorioSchemaHtml.CaminhoAtributosSnebur, Encoding.UTF8);
            var len = linhas.Length;
            for (var i = 0; i < len; i++)
            {
                var linha = linhas[i].Trim();
                if (linha.Contains(procurar))
                {
                    var inicio = linha.IndexOf(procurar) + procurar.Length;
                    var fim = linha.IndexOf(",");
                    var atributo = linha.Trim().Substring(inicio, fim - inicio);
                    atributo = atributo.Replace("\"", String.Empty);

                    if (atributo.StartsWith("sn"))
                    {
                        linha = linha.Substring(fim + 1).Trim();
                        fim = linha.IndexOf(");");
                        var tipo = linha.Substring(0, fim).Trim();
                        tipo = tipo.Replace("\"", String.Empty);
                        atributos.Add(new Tuple<string, string>(atributo.ToLower(), tipo));
                    }
                }
            }
            return atributos;
        }

        private List<string> RetornarConteudoAtributo(string atributo, string tipo)
        {
            var linhas = new List<string>();
            if (tipo.Contains(".") || tipo == nameof(Boolean))
            {

                linhas.Add(String.Format("<xsd:attribute name=\"{0}\" vs:standalone=\"noname\" >", atributo));
                linhas.Add(String.Format("\t<xsd:simpleType>"));
                linhas.Add(String.Format("\t\t<xsd:restriction base=\"xsd:NMTOKEN\">"));

                var linhasEnum = this.RetornarLinhasEnum(tipo);
                foreach (var linhaEnum in linhasEnum)
                {
                    linhas.Add(String.Format("\t\t\t<xsd:enumeration value=\"{0}\" />", linhaEnum));
                }

                linhas.Add(String.Format("\t\t</xsd:restriction>"));
                linhas.Add(String.Format("\t</xsd:simpleType>"));
                linhas.Add("</xsd:attribute>");

            }
            else
            {
                var descricaoTipo = (tipo == "Event") ? "vs:omtype=\"event\"" : String.Format("type=\"xsd:{0}\"", tipo.ToLower());
                linhas.Add(String.Format("<xsd:attribute name=\"{0}\" {1} />", atributo, descricaoTipo));
            }
            return linhas;
        }

        private List<string> RetornarLinhasEnum(string tipo)
        {
            var linhas = new List<string>();
            if (tipo == nameof(Boolean))
            {
                linhas.Add(true.ToString().ToLower());
                linhas.Add(false.ToString().ToLower());
            }
            else
            {
                var partes = tipo.Split('.');
                var nome = partes.Last();
                var _namespace = String.Join(".", partes.Take(partes.Length - 1));
                var tipoEnum = AjudanteAssembly.RetornarTipo(nome);
                return EnumUtil.RetornarNomesEnum(tipoEnum);
            }
            return linhas;
        }

        #endregion

        #region Controles Snebur

        private List<string> RetornarLinhasReferenciasControles()
        {
            var tags = this.RetornarTagsControles();
            var linhas = new List<string>();
            foreach (var tag in tags)
            {
                linhas.Add(String.Format("\t\t\t<xsd:element ref=\"{0}\" />", tag.ToLower()));
            }
            return linhas;
        }

        private List<string> RetornarLinhasConteudoControles()
        {
            var tags = this.RetornarTagsControles();
            var linhas = new List<string>();
            foreach (var tag in tags)
            {
                linhas.Add(String.Format("<xsd:element name=\"{0}\" type=\"simpleFlowContentElement\" />", tag.ToLower()));
            }
            return linhas;
        }
        private List<string> RetornarTagsControles()
        {
            var procurar = "$ElementosControle.Add(\"";
            var tags = new List<string>();
            var linhas = File.ReadAllLines(RepositorioSchemaHtml.CaminhoControlesSnebur, Encoding.UTF8);
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
