namespace Snebur.Dominio.Atributos
{
    public class RelacaoPaiExterna : NaoMapearAttribute, IIgnorarAlerta
    {
        public bool IgnorarAlerta => true;
        public string NomePropriedadeChaveEstrangeira { get; set; }
        public string NomeContextoDadosExterno { get; set; }

        public RelacaoPaiExterna(string nomeContextoDadosExtexto, 
                                 string nomePropriedadeChaveEstrangeira)  
        {
            this.NomePropriedadeChaveEstrangeira = nomePropriedadeChaveEstrangeira;
            this.NomeContextoDadosExterno = nomeContextoDadosExtexto;
        }
    }
}