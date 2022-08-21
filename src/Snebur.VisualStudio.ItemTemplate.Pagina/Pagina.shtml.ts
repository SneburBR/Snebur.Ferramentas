
namespace $rootnamespace$
{
    export class $safeitemrootname$ extends Snebur.UI.Pagina
    {
        public constructor(controlePai: Snebur.UI.BaseControle) 
        {
            super(controlePai);
            this.EventoCarregado.AddHandler(this.Pagina_Carregada.bind(this));
        }

        public Pagina_Carregada(provedor: any, e: EventArgs) 
        {
            //pagina carregada
        }
    }
}
