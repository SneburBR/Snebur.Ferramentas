using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NormalizarNomesArquivos
{
    internal class NormalizadorNomesArquivosApresentacao
    {
        private string Diretorio { get; }
        public List<string> Falhas { get; } = new List<string>();
        public List<string> Atualizados { get; } = new List<string>();

        public NormalizadorNomesArquivosApresentacao(string diretorio)
        {
            this.Diretorio = diretorio;
        }

        internal void Normalizar()
        {
            var caminhos = Directory.EnumerateFiles(this.Diretorio, "*.shtml", SearchOption.AllDirectories).ToList();
            foreach (var caminhoApresentacao in caminhos)
            {
                this.NormalizarArquivo(caminhoApresentacao);
            }
        }

        private void NormalizarArquivo(string caminhoApresentacao)
        {
            var fiApresentacao = new FileInfo(caminhoApresentacao);

            var arquivos = Directory.EnumerateFiles(fiApresentacao.Directory.FullName,
                                                    fiApresentacao.Name + ".*").
                                                    Select(x => new FileInfo(x)).ToList();

            var fiCodigo = arquivos.Where(x => x.Extension == ".ts").SingleOrDefault();
            var fiEstilo = arquivos.Where(x => x.Extension == ".scss").SingleOrDefault();

            if(fiCodigo == null)
            {
                this.Falhas.Add(caminhoApresentacao);
                return;
            }

            var (resultado, nomeArquivo) = this.RetornarNomeArquivo(fiApresentacao.Name,
                                                                    fiCodigo.NameWithoutExtension(),
                                                                    fiEstilo.NameWithoutExtension() ?? fiApresentacao.Name);

            switch (resultado)
            {
                case EnumResultado.TudoCerto:
                    //não faz nada
                    break;
                case EnumResultado.Alterar:
                    this.AtualizarNomes(nomeArquivo, fiApresentacao, fiCodigo, fiEstilo);
                    this.Atualizados.Add(caminhoApresentacao);
                    break;

                case EnumResultado.MelhorNomeNaoEncontrado:
                case EnumResultado.MaisDeUmMelhorNome:
                case EnumResultado.Falha:
                    this.Falhas.Add(caminhoApresentacao);
                    break;
                default:
                    throw new Exception("Resultado não suportado");
            }
        }

        private void AtualizarNomes(string nomeArquivo,
                                    FileInfo fiApresentacao,
                                    FileInfo fiCodigo,
                                    FileInfo fiEstilo)
        {
            var nomeApresentacao = nomeArquivo;
            var nomeEstilo = nomeArquivo + ".scss";
            var nomeCodigo = nomeArquivo + ".ts";

            if (fiApresentacao.Name != nomeApresentacao)
            {
                fiApresentacao.Rename(nomeApresentacao);
            }

            if (fiEstilo != null && fiEstilo.Name != nomeEstilo)
            {
                fiEstilo.Rename(nomeEstilo);
            }
            if (fiCodigo.Name != nomeCodigo)
            {
                fiCodigo.Rename(nomeCodigo);
            }
        }

        private (EnumResultado, string) RetornarNomeArquivo(string nomeApresentacao, string nomeCodigo, string nomeEstilo)
        {
            var nomes = new string[] { nomeApresentacao, nomeCodigo, nomeEstilo };
            if (nomes.Any(x => x.Length != nomeApresentacao.Length))
            {
                return this.RetornarNomeArquivo(nomeApresentacao, nomeApresentacao, nomeApresentacao);
            }
            var nomesDistindo = nomes.Distinct().ToList();
            var melhoresNome = nomes.Where(x => Char.IsUpper(x[0]) && x.Any(c => Char.IsLower(c))).Distinct().ToList();
            if (melhoresNome.Count == 1 && nomesDistindo.Count == 1)
            {
                return (EnumResultado.TudoCerto, null);
            }

            if (melhoresNome.Count == 0)
            {
                return (EnumResultado.MelhorNomeNaoEncontrado, null);
            }

            if (nomesDistindo.Count > 1)
            {
                if (melhoresNome.Count == 1)
                {
                    return (EnumResultado.Alterar, melhoresNome.Single());
                }
                return (EnumResultado.MaisDeUmMelhorNome, null);
            }
            return (EnumResultado.Falha, null);

        }

        enum EnumResultado
        {
            Alterar,
            MelhorNomeNaoEncontrado,
            TudoCerto,
            MaisDeUmMelhorNome,
            Falha
        }
    }

    internal static class FileInfoEntension
    {
        public static string NameWithoutExtension(this FileInfo fi)
        {
            if (fi == null)
            {
                return null;
            }
            return Path.GetFileNameWithoutExtension(fi.Name);
        }
        public static void Rename(this FileInfo fi, string novoNome)
        {
            if (!fi.Exists)
            {
                throw new FileNotFoundException(fi.FullName);
            }
            var destino = Path.Combine(fi.Directory.FullName, novoNome);
            fi.MoveTo(destino);
        }
    }
}