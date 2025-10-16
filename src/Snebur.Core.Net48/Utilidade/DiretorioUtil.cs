using Snebur.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Text;

namespace Snebur.Utilidade
{
    public partial class DiretorioUtil
    {
        public const int LIMITE_MAXIMO_CARACTERES_CAMINHO_PASTA = 239;

        private static HashSet<char> _caracteresInvalidos;
        public static HashSet<char> CaracteresInvalidos
        {
            get
            {
                if (_caracteresInvalidos == null)
                {
                    _caracteresInvalidos = new HashSet<char>();
                    _caracteresInvalidos.AddRange(Path.GetInvalidPathChars());
                    _caracteresInvalidos.AddRange(Path.GetInvalidFileNameChars());
                }
                return _caracteresInvalidos;
            }
        }

        public static string FormatarNomePasta(string caminho,
                                               bool isRemoverCaracterEspecial = false,
                                               bool isRemoverAcento = false)
        {
            string retorno;
            if (IsDireotrioRaiz(caminho))
            {
                retorno = TextoUtil.RemoverCaracteres(caminho, Path.GetInvalidPathChars());
            }
            else
            {
                retorno = TextoUtil.RemoverCaracteres(caminho, CaracteresInvalidos);
            }
            if (isRemoverCaracterEspecial)
            {
                retorno = TextoUtil.RemoverCaracteresEspecial(caminho);
            }
            if (isRemoverAcento)
            {
                retorno = TextoUtil.RemoverAcentos(caminho);
            }
            return retorno.Trim();
        }

        public static bool IsDireotrioRaiz(string caminho)
        {
            if (caminho == null)
            {
                return false;
            }
            var comprimento = caminho.Length;
            if ((comprimento >= 1 && (caminho[0] == Path.DirectorySeparatorChar || caminho[0] == Path.AltDirectorySeparatorChar)) ||
                                     (comprimento >= 2 && caminho[1] == Path.VolumeSeparatorChar))
            {
                return true;
            }
            return false;
        }

        public static string Combinar(params string[] caminhos)
        {
            try
            {
                return CaminhoUtil.Combine(caminhos);
            }
            catch (Exception ex)
            {
                LogUtil.ErroAsync(new Exception($"Caracteres invalidos ao combinhar caminhos  {String.Join(", ", caminhos)}", ex));
                return ComibarFormatado(caminhos);
            }
        }

        public static string ComibarFormatado(params string[] caminhos)
        {
            var caminhosFormatado = caminhos.Select(x => FormatarNomePasta(x)).ToArray();
            return CaminhoUtil.Combine(caminhosFormatado);
        }

        public static void CopiarDiretorio(string caminhoOrigem,
                                           string caminhoDestino,
                                           bool copiarSubDiretorios,
                                           Func<FileInfo, bool> funcaoIsCopiar)
        {
            CopiarDiretorio(caminhoOrigem,
                                          caminhoDestino,
                                          copiarSubDiretorios,
                                          null,
                                          funcaoIsCopiar);

        }
        /// <summary>
        /// Testar codigo de https://msdn.microsoft.com/en-us/library/bb762914(v=vs.110).aspx
        /// </summary>
        /// <param name="caminhoOrigem"></param>
        /// <param name="caminhoDestino"></param>
        /// <param name="copiarSubDiretorios"></param>
        /// 
        public static void CopiarDiretorio(string caminhoOrigem,
                                            string caminhoDestino,
                                            bool copiarSubDiretorios,
                                            string searchPattern = null,
                                            Func<FileInfo, bool> funcaoIsCopiar = null)
        {
            var diretorio = new DirectoryInfo(caminhoOrigem);

            if (!diretorio.Exists)
            {
                throw new Erro($"O caminho do origem não existe {caminhoOrigem}");
            }
            CriarDiretorio(caminhoDestino);
            var arquivos = String.IsNullOrWhiteSpace(searchPattern) ? diretorio.GetFiles() :
                                                                     diretorio.GetFiles(searchPattern);
            foreach (var arquivo in arquivos)
            {
                if (funcaoIsCopiar == null || funcaoIsCopiar.Invoke(arquivo))
                {
                    var caminhoArquivoDestino = CaminhoUtil.Combine(caminhoDestino, arquivo.Name);
                    arquivo.CopyTo(caminhoArquivoDestino, true);
                }
            }
            if (copiarSubDiretorios)
            {
                var subDiretorios = diretorio.GetDirectories();
                foreach (DirectoryInfo subDiretorio in subDiretorios)
                {
                    var caminhoArquivoDestino = CaminhoUtil.Combine(caminhoDestino, subDiretorio.Name);
                    CopiarDiretorio(subDiretorio.FullName, caminhoArquivoDestino, copiarSubDiretorios, searchPattern, funcaoIsCopiar);
                }
            }
        }

        public static void CopiarTodosArquivo(string diretorioOrigem, string diretorioDestino, bool ignorarErro = false, bool forcar = false)
        {
            var arquivos = Directory.GetFiles(diretorioOrigem);
            CriarDiretorio(diretorioDestino);
            foreach (var arquivo in arquivos)
            {
                var caminhoRelativo = arquivo.Substring(diretorioOrigem.Length + 1);
                var caminhoDestino = CaminhoUtil.Combine(diretorioDestino, caminhoRelativo);
                ArquivoUtil.DeletarArquivo(caminhoDestino, ignorarErro, forcar);
                ArquivoUtil.CopiarArquivo(arquivo, caminhoDestino, true);
            }
        }

        public static void CriarDiretorio(string caminho)
        {
            if (!Directory.Exists(caminho))
            {
                // caso existe um arquivo sem extensão
                if (File.Exists(caminho))
                {
                    File.Move(caminho, caminho + $".file.{DateTime.Now:yyyy-MM-dd-HH-mm.ss.ffff}");
                }

                try
                {
                    Directory.CreateDirectory(caminho);
                }
                catch (Exception erro)
                {
                    throw new Erro($"Não foi possível criar a pasta {caminho}", erro);
                }
            }
        }

        public static void ExcluirDiretorio(string caminho)
        {
            ExcluirDiretorio(caminho, true, false);
        }

        public static void ExcluirDiretorio(string caminho,
                                            bool isExcluirTodosArquivos,
                                            bool ignorarErro,
                                            bool isForcar = false)
        {
            if (isExcluirTodosArquivos)
            {
                ExcluirTodosArquivo(caminho,
                                                  true,
                                                  ignorarErro,
                                                  isForcar);
            }
            ExcluirDiretorioInterno(caminho, true, ignorarErro);
        }

        private static bool ExcluirDiretorioInterno(string caminho, bool recursivo, bool ignorarErro)
        {
            if (File.Exists(caminho))
            {
                if (!ignorarErro)
                {
                    throw new Erro("Não é posssivel excluir o diretorio {0} por ele é um arquivo no disco");
                }
            }

            if (Directory.Exists(caminho))
            {
                try
                {
                    var di = new DirectoryInfo(caminho);
                    di.Delete(recursivo);
                    //Directory.Delete(caminho);
                    return true;
                }
                catch (Exception erro)
                {
                    if (!ignorarErro)
                    {
                        throw new Erro($"Não foi possível criar a pasta {caminho}", erro);
                    }
                }
            }
            return false;
        }

        public static bool IsDiretorioFilho(string diretorioPai, string diretorioFilho)
        {
            return IsDiretorioFilho(new DirectoryInfo(diretorioPai), new DirectoryInfo(diretorioFilho));
        }

        public static bool MoverDiretorio(string diretorioOrigem,
                                          string diretorioDestino,
                                          bool isSobreEscrever,
                                          bool isIgnorarErro)
        {
            try
            {
                CriarDiretorio(Path.GetDirectoryName(diretorioDestino));
                if (isSobreEscrever)
                {
                    ExcluirDiretorio(diretorioDestino, true, true);
                }
                if (Directory.Exists(diretorioDestino))
                {
                    CopiarTodosArquivo(diretorioOrigem,
                                                     diretorioDestino,
                                                     false,
                                                     true);
                    ExcluirDiretorio(diretorioDestino,
                                                   true,
                                                   true,
                                                   true);
                }
                else
                {
                    Directory.Move(diretorioOrigem, diretorioDestino);
                }
                return true;
            }
            catch (Exception)
            {
                if (!isIgnorarErro)
                {
                    throw;
                }
            }
            return false;
        }

        public static bool IsDiretorioFilho(DirectoryInfo diretorioPai, DirectoryInfo diretorioFilho)
        {
            var diretorioAtual = diretorioPai;
            while (diretorioAtual != null)
            {
                if (ArquivoUtil.CaminhoIgual(diretorioAtual.FullName, diretorioFilho.FullName))
                {
                    return true;
                }
                diretorioAtual = diretorioAtual.Parent;
            }
            return false;
        }

        public static void ExcluirTodosArquivo(string caminhoDiretorio)
        {
            ExcluirTodosArquivo(caminhoDiretorio, false);
        }

        public static void ExcluirTodosArquivo(string caminhoDiretorio,
                                               bool incluirSubDiretorios)
        {
            ExcluirTodosArquivo(caminhoDiretorio,
                                              incluirSubDiretorios,
                                              false,
                                              false);
        }

        public static void ExcluirTodosArquivo(string caminhoDiretorio,
                                               bool isIncluirSubDiretorios,
                                               bool isIgnorarErro,
                                               bool isForcar = false,
                                               string[] ignorarArquvos = null)
        {
            if (Directory.Exists(caminhoDiretorio))
            {
                var arquivos = Directory.GetFiles(caminhoDiretorio).ToList();
                foreach (var arquivo in arquivos)
                {
                    if (ignorarArquvos?.Contains(Path.GetFileName(arquivo), StringComparison.InvariantCultureIgnoreCase) == true)
                    {
                        continue;
                    }
                    ArquivoUtil.DeletarArquivo(arquivo,
                                              isIgnorarErro,
                                              isForcar);
                }
                if (isIncluirSubDiretorios)
                {
                    var subsDiretorio = Directory.GetDirectories(caminhoDiretorio);
                    foreach (var subDiretorio in subsDiretorio)
                    {
                        ExcluirTodosArquivo(subDiretorio, isIncluirSubDiretorios, isIgnorarErro);
                        ExcluirDiretorio(subDiretorio);
                    }
                }
            }
        }

        public static EnumStatusDiretorio RetornarStatusDiretorio(string caminhoDiretorio, long espacoMinimo)
        {
            if (!Directory.Exists(caminhoDiretorio))
            {
                return EnumStatusDiretorio.DiretorioNaoExiste;
            }
            var espacoLivre = RetornarEspacoLivre(caminhoDiretorio);
            if (espacoLivre > 0 && espacoLivre < espacoMinimo)
            {
                return EnumStatusDiretorio.EspacoInsuficiente;
            }
            if (!PossuiPermissaoGravao(caminhoDiretorio))
            {
                return EnumStatusDiretorio.SemPermissaoGravacao;
            }
            return EnumStatusDiretorio.TudoCerto;
        }

        public static long RetornarEspacoLivre(string caminhoDiretorio)
        {
            var isRede = caminhoDiretorio.StartsWith("\\\\");
            if (!isRede)
            {
                var raiz = new DirectoryInfo(caminhoDiretorio);
                var disco = new DriveInfo(raiz.Root.FullName);

                try
                {
                    return disco.AvailableFreeSpace;
                }
                catch
                {
                    //Não é possivel saber o espaço livre no disco
                }
            }
            return -1;
        }

        public static bool PossuiPermissaoGravao(string caminhoDiretorio)
        {
            var isRede = IsRede(caminhoDiretorio);
            if (isRede)
            {
                return TestarPermissaoGravao(caminhoDiretorio);
            }
            else
            {
                return DiretorioPossuiPermissaoGravao(caminhoDiretorio);
            }
        }
        /// <summary>
        /// Se diretorio é um caminho compartilhado na rede
        /// </summary>
        /// <param name="caminhoDiretorio"></param>
        /// <returns></returns>
        public static bool IsRede(string caminhoDiretorio)
        {
            if (String.IsNullOrEmpty(caminhoDiretorio))
            {
                return false;
            }
            if (caminhoDiretorio.StartsWith("\\\\"))
            {
                return true;
            }
            if (IsDireotrioRaiz(caminhoDiretorio))
            {
                var raiz = new DirectoryInfo(caminhoDiretorio);
                var disco = new DriveInfo(raiz.Root.FullName);

                try
                {
                    return (!(disco.DriveType == DriveType.Fixed));
                }
                catch
                {
                    return false;
                }
            }
            return false;
        }

        public static bool DiretorioPossuiPermissaoGravao(string caminhoDiretorio)
        {
            try
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    var isPermiteGravacao = false;
                    var isGravacaoNegada = false;
                    var accessControlList = new DirectoryInfo(caminhoDiretorio).GetAccessControl();
                    //Directory.GetAccessControl(caminhoDiretorio);

                    if (accessControlList == null)
                    {
                        return false;
                    }

                    var accessRules = accessControlList.GetAccessRules(true, true,
                                                  typeof(System.Security.Principal.SecurityIdentifier));
                    if (accessRules == null)
                    {
                        return false;
                    }

                    foreach (FileSystemAccessRule rule in accessRules)
                    {
                        if ((FileSystemRights.Write & rule.FileSystemRights) != FileSystemRights.Write)
                        {
                            continue;
                        }
                        if (rule.AccessControlType == AccessControlType.Allow)
                        {
                            isPermiteGravacao = true;
                        }
                        else if (rule.AccessControlType == AccessControlType.Deny)
                        {
                            isGravacaoNegada = true;
                        }
                    }
                    return isPermiteGravacao && !isGravacaoNegada;
                }

                return TestarPermissaoGravao(caminhoDiretorio);
            }
            catch
            {
                return TestarPermissaoGravao(caminhoDiretorio);
            }
        }

        public static DirectoryInfo RetornarDiretorioPaiExisteArquivo(string caminho, string nomeArquivo)
        {
            return RetornarDiretorioPai(caminho, (x) => x.GetFiles().Any(f => f.Name.Equals(nomeArquivo, StringComparison.InvariantCultureIgnoreCase)));
        }

        public static DirectoryInfo RetornarDiretorioPai(string caminho, Func<DirectoryInfo, bool> funcaoCondicao, bool ignorarErro = true)
        {
            var diretorio = new DirectoryInfo(caminho);
            return RetornarDiretorioPai(diretorio, funcaoCondicao, ignorarErro);
        }

        public static DirectoryInfo RetornarDiretorioPai(DirectoryInfo diretorio, Func<DirectoryInfo, bool> condicao, bool ignorarErro = true)
        {
            while (diretorio.Parent != null && diretorio.Exists)
            {
                if (condicao.Invoke(diretorio))
                {
                    return diretorio;
                }
                diretorio = diretorio.Parent;
            }
            if (!ignorarErro)
            {
                throw new Erro("O diretório não foi encontrado");
            }
            return null;
        }

        private static bool TestarPermissaoGravao(string caminhoDiretorio)
        {
            try
            {
                var caminhoTemp = CaminhoUtil.Combine(caminhoDiretorio);
                CriarDiretorio(caminhoTemp);
                var caminhoArquivoTemp = CaminhoUtil.Combine(caminhoTemp, $"{Guid.NewGuid()}.txt");
                File.WriteAllText(caminhoArquivoTemp, Guid.NewGuid().ToString(), Encoding.UTF8);
                var atributos = File.GetAttributes(caminhoArquivoTemp) | FileAttributes.Hidden;
                File.SetAttributes(caminhoArquivoTemp, atributos);
                ArquivoUtil.DeletarArquivo(caminhoArquivoTemp, true);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static string RetornarNomeComputador(string caminho)
        {
            if (IsRede(caminho))
            {
                NormalizarCaminho(caminho);
                while (caminho.StartsWith("\\"))
                {
                    caminho = caminho.Substring(1);
                }
                return caminho.Split('\\')[0];
            }
            return String.Empty;
        }

        private static string NormalizarCaminho(string caminho)
        {
            return caminho.Replace("/", "\\");
        }

        public static string RetornarDiretorioPai(IEnumerable<string> caminhos,
                                                  bool ignorarErro = false)
        {
            //find the common prefix
            caminhos = caminhos.Select(path => NormalizarCaminho(path)).ToArray();
            string prefix = caminhos.First();
            foreach (string path in caminhos.Skip(1))
            {
                while (!path.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                {
                    prefix = prefix.Substring(0, prefix.Length - 1);
                }
            }

            if (String.IsNullOrEmpty(prefix))
            {
                if (ignorarErro)
                {
                    return prefix;
                }
                throw new Erro("Não foi possivel encontrar o diretorio pai");
            }
            return prefix;
        }
 
    }
}