using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Snebur;
using Snebur.VisualStudio.Reflexao;
using Snebur.VisualStudio.Utilidade;
using Snebur.Utilidade;

namespace Snebur.VisualStudio.Projeto.Dominio
{
    public partial class AjudanteReflexao
    {
        private const string PREFIXO_LISTA_TIPO_PRIMARIO = "ListaTipoPrimario_";
        private const string PREFIXO_LISTA_TIPO_ENUM = "ListaTipoEnum_";
        private const string PREFIXO_LISTA_TIPO_BASEDOMINIO = "ListaTipoBaseDominio_";
        private const string PREFIXO_LISTA_TIPO_ENTIDADE = "ListaTipoEntidade_";

        private static string RetornarCaminhoTipo_SEM_PONTO(Type tipo)
        {
            return TipoUtil.RetornarCaminhoTipo(tipo).Replace(".", "_");
        }

        public static string RetornarDeclaracaoTipo(Type tipo)
        {
            if (tipo.IsEnum)
            {
                return AjudanteReflexao.RetornarDeclaracaoTipoEnum(tipo);
            }

            if (ReflexaoUtil.TipoRetornaTipoPrimario(tipo))
            {
                var tipoPrimarioEnum = ReflexaoUtil.RetornarTipoPrimarioEnum(tipo);
                return AjudanteReflexao.RetornarDeclaracaoTipoPrimario(tipoPrimarioEnum);
            }

            if (TipoUtil.TipoIgualOuSubTipo(tipo, AjudanteAssembly.TipoBaseDominio))
            {
                return AjudanteReflexao.RetornarDeclaracaoTipoBaseDominio(tipo);
            }

            if (ReflexaoUtil.TipoRetornaColecao(tipo))
            {
                return AjudanteReflexao.RetornarDeclaracaoTipoColecao(tipo);
            }

            throw new NotSupportedException(String.Format("Tipo não suportado {0} ", tipo.Name));

        }

        private static string RetornarDeclaracaoTipoColecao(Type tipo)
        {
            var tipoDefinicao = tipo.GetGenericTypeDefinition();

            if (tipo.GetGenericArguments().Count() != 1)
            {
                if (tipo.GetGenericTypeDefinition() == typeof(Dictionary<,>))
                {

                    var tipoChave = tipo.GetGenericArguments().First();
                    var tipoItemValor = tipo.GetGenericArguments().Last();
                    if (tipoChave != typeof(string))
                    {
                        throw new ErroNaoSuportado("O tipo da chave do dicionario não é suportado");
                    }
                    var declaracaoTipoItemValorDicionario = AjudanteReflexao.RetornarDeclaracaoTipo(tipoItemValor);
                    var declaracaoTipoDicionario = string.Format(" new Snebur.Reflexao.TipoDicionario({0})", declaracaoTipoItemValorDicionario);
                    return declaracaoTipoDicionario;


                }
                else
                {
                    throw new NotSupportedException(String.Format("Tipo coleção não suportado {0} ", tipo.Name));
                }

            }

            var tipoItemLista = tipo.GetGenericArguments().Single();
            if (tipoDefinicao == typeof(List<>))
            {
           
                return AjudanteReflexao.RetornarDeclaracaoTipoList(tipoItemLista);
            }

            if (tipoDefinicao.Name.Contains("ListaEntidade"))
            {
                if (!(TipoUtil.TipoIgualOuSubTipo(tipoItemLista, AjudanteAssembly.TipoEntidade) ||
                     TipoUtil.TipoIgualOuSubTipo(tipoItemLista, AjudanteAssembly.TipoInterfaceIEntidade) ||
                     TipoUtil.TipoImplementaInterface(tipoItemLista, AjudanteAssembly.TipoInterfaceIEntidade)))
                {
                    throw new NotSupportedException(String.Format("Tipo não suportado {0} ", tipo.Name));
                }
                return AjudanteReflexao.RetornarDeclaracaoListaTipoEntidade(tipoItemLista);
            }
            throw new NotSupportedException(String.Format("Tipo não suportado {0} ", tipo.Name));
        }

        private static string RetornarDeclaracaoTipoList(Type tipo)
        {
            if (tipo.IsEnum)
            {
                return AjudanteReflexao.RetornarDeclaracaoListaTipoEnum(tipo);
            }

            if (ReflexaoUtil.TipoRetornaTipoPrimario(tipo))
            {
                var tipoPrimarioEnum = ReflexaoUtil.RetornarTipoPrimarioEnum(tipo);
                return AjudanteReflexao.RetornarDeclaracaoListaTipoPrimario(tipoPrimarioEnum);
            }

            if (TipoUtil.TipoIgualOuSubTipo(tipo, AjudanteAssembly.TipoBaseDominio))
            {
                return AjudanteReflexao.RetornarDeclaracaoListaTipoBaseDominio(tipo);
            }

            throw new NotSupportedException(String.Format("Tipo não suportado {0} ", tipo.Name));

        }

        private static string RetornarAssemblyQualifiedName(Type tipo)
        {
            var q = tipo.AssemblyQualifiedName;
            var posicao = q.IndexOf(", Version");
            if (posicao > 0)
            {
                return q.Substring(0, posicao).Trim();
            }
            return q;
        }

    }
}
