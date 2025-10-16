using Snebur.Dominio;
using System;

namespace Snebur.Utilidade
{
    public static class ImagemUtilEx
    {
        public static EnumTamanhoImagem[] TamanhosImagemApresentacao { get; } = new EnumTamanhoImagem[] { EnumTamanhoImagem.Miniatura,
                                                                                                          EnumTamanhoImagem.Pequena,
                                                                                                          EnumTamanhoImagem.Media,
                                                                                                          EnumTamanhoImagem.Grande };

        public static bool IsExisteImagem(IImagem imagem, EnumTamanhoImagem tamanhoImagem)
        {
            switch (tamanhoImagem)
            {
                case EnumTamanhoImagem.Miniatura:
                    return imagem.IsExisteMiniatura;
                case EnumTamanhoImagem.Pequena:
                    return imagem.IsExistePequena;
                case EnumTamanhoImagem.Media:
                    return imagem.IsExisteMedia;
                case EnumTamanhoImagem.Grande:
                    return imagem.IsExisteGrande;
                case EnumTamanhoImagem.Impressao:
                    return imagem.IsExisteArquivo;
                default:
                    throw new Erro("Tamanho da imagem não é suportado");
            }
        }
    }
}