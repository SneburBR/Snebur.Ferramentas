//futuramente vamos normalizar os Error, passando um parametro da mapeamento do arquivo e linha
//detalhes aqui https://www.html5rocks.com/en/tutorials/developertools/sourcemaps/

using Snebur.Utilidade;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Snebur.VisualStudio
{
    public static class ArquivoControleUtil
    {
        public static FileInfo RetornarArquivoShtml(FileInfo arquivo)
        {
            var caminhoShtml = RetornarCaminhoShtml(arquivo);
            return new FileInfo(caminhoShtml);
        }

        public static int RetornarOrdenacao(FileInfo arquivo)
        {
            var name = arquivo.Name;
            if (name.EndsWith(ConstantesProjeto.EXTENSAO_CONTROLE_SHTML, StringComparison.InvariantCultureIgnoreCase))
            {
                return 0;
            }
            if (name.EndsWith(ConstantesProjeto.EXTENSAO_CONTROLE_SHTML_SCSS, StringComparison.InvariantCultureIgnoreCase))
            {
                return 1;
            }
            if (name.EndsWith(ConstantesProjeto.EXTENSAO_CONTROLE_SHTML_TYPESCRIPT, StringComparison.InvariantCultureIgnoreCase))
            {
                return 2;
            }
            return 3;
        }

        public static string RetornarCaminhoShtml(FileInfo arquivo)
        {
            if (!IsArquivoControle(arquivo))
            {
                throw new Exception("Arquivo não suportado " + arquivo.FullName);
            }

            if (arquivo.Extension.Equals(ConstantesProjeto.EXTENSAO_CONTROLE_SHTML, StringComparison.InvariantCultureIgnoreCase))
            {
                return arquivo.FullName;
            }
            return Path.Combine(arquivo.Directory.FullName, Path.GetFileNameWithoutExtension(arquivo.FullName));
        }

        public static IEnumerable<FileInfo> RetornarArquivosControle(IEnumerable<FileInfo> arquivos)
        {
            return arquivos.Where(fi => ArquivoControleUtil.IsArquivoControle(fi));
        }

        public static bool IsArquivoControle(FileInfo arquivo)
        {
            return ConstantesProjeto.ExtensoesControlesSnebur.Any(e => arquivo.Name.EndsWith(e, StringComparison.CurrentCultureIgnoreCase));
        }
    }


}