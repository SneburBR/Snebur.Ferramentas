namespace NormalizarNomesArquivos
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var diretorio = @"E:\Github\Zyoncore\Sigi\src\Zyoncore.Sigi.FotoAlbum.TS";
            var normalizador = new NormalizadorNomesArquivosApresentacao(diretorio);
            normalizador.Normalizar();
        }
    }
}
