namespace Snebur.Dominio
{
    public interface IImagem : IMedia
    {
        string ChecksumArquivoLocal { get; set; }
        bool IsExisteMiniatura { get; set; }

        bool IsExistePequena { get; set; }

        bool IsExisteMedia { get; set; }

        bool IsExisteGrande { get; set; }

        long TotalBytesMiniatura { get; set; }

        long TotalBytesPequena { get; set; }

        long TotalBytesMedia { get; set; }

        long TotalBytesGrande { get; set; }

        //[Indexar]
        //bool ImagemProcessada { get; set; }

        //[Indexar]
        //bool ImagemTruncada { get; set; }

        //[ValidacaoTextoTamanho(255)]
        //IPerfilIcc PerfilIcc { get; set; }

        //bool IsPerfilsRGB { get; set; }

        bool IsIcone { get; set; }

        bool IsImagemProcessada { get; set; }

        Dimensao DimensaoImagemMiniatura { get; set; }

        Dimensao DimensaoImagemPequena { get; set; }

        Dimensao DimensaoImagemMedia { get; set; }

        Dimensao DimensaoImagemGrande { get; set; }

        Dimensao DimensaoImagemOrigem { get; set; }

        Dimensao DimensaoImagemLocal { get; set; }

        //bool RedimencionarAntesEnviar { get; set; }

        Dimensao DimensaoImagemImpressao { get; set; }

        EnumFormatoImagem FormatoImagem { get; set; }

    }
}