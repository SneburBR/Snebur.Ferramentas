using Snebur.Dominio.Atributos;

namespace Snebur.UI
{
    [IgnorarEnumTS]
    public enum EnumFormatacao
    {
        Nenhuma,

        Absoluto,
         
        Bytes,// "bytes",

        CaixaAlta, // "caixaalta",

        CaixaBaixa, // "caixabaixa"
         
        Centimetro, // "centimetro",

        Cep, // "cep",

        Cpf, // "cpf",

        Cnpj, // "cnpj",

        CpfCnpj, // "cpfcnpj",
         
        Data, // "data",
         
        DataHora, // "datahora",

        DataSemantica,

        DataHoraSemantica, // "datahorasemantico",

        DataSemanticaHora, // "datahorasemantico",
         
        Decimal, // "decimal",

        Decimal1, // "decimal1",

        Decimal3, // "decimal3",

        Dias, // "dias",
        DiasUteis, //"diasuteis",

        Dimensao,

        DimensaoCm,

        DimensaoPixels,

        DoisPontosFinal,

        EntreParentes,

        Grau, // "grau",

        Grau1, // "grau1",

        Grau2, // "grau2",

        Hora, // "hora",

        HoraDescricao, // "datahoradescricao",

        HoraDescricaoMin, // "datahoradescricaomin",

        Inteiro, // "inteiro",
         
        Margem,

        MelhorDecimal, // "melhordecimal",

        Moeda, // "moeda",

        MoedaIgnorarSemValor,

        MoedaComSinal, // "moedacomsinal"
         
        NaoQuebrar, // "naoquebrar",

        Nome,

        OcultarEmail,

        Prazo, // "prazo",

        Peso, //"peso",

        PesoKg, //"pesokg",

        Pixel, // "pixel",
         
        Porcentagem, // "porcentagem",

        Porcentagem1, // "porcentagem1",

        Porcentagem2, // "porcentagem2",
         
        PortentagemPositivoNegativo, //"porcentagempositivonegativo",

        Portentagem1PositivoNegativo, //"porcentagem1positivonegativo",

        PositivoNegativo, //"positivonegativo",

        PositivoNegativoDecimal,//"positivonegativodecimal"

        PrimeiraLetraMaiuscula, // "primeiraletramaiuscula",

        Proteger, // "proteger",

        Senha, // "senha",

        SimNao, // "simnao",

        QuebrarLinhasHtml, //"quebrarlinhahtml",

        Tempo, // "tempo",

        TempoSemantico, // "temposemantico",
         
        TamanhoArquivo, // "tamanhoarquivo",

        Telefone, // "telefone",

        Titulo, // "titulo",

        Trim, // "trim",
          
    }
}
