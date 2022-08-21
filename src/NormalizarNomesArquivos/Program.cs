namespace NormalizarNomesArquivos
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var diretorio = @"E:\OneDrive\GitHub\SneburBR\Snebur.Pandeco";
            var normalizador = new NormalizadorNomesArquivosApresentacao(diretorio);
            normalizador.Normalizar();
        }
    }
}
