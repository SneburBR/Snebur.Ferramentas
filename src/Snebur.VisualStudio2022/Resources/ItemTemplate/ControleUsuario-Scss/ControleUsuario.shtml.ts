
namespace $rootnamespace$
{
    export class $safeitemrootname$ extends Snebur.UI.ControleUsuario
    {
        public constructor(controlePai: Snebur.UI.BaseControle, refElemento: HTMLElement | string) 
        {
            super(controlePai, refElemento);
            this.EventoCarregado.AddHandler(this.Controle_Carregado, this);
        }

        private Controle_Carregado(provedor: any, e: EventArgs) 
        {
            //controle carregada
        }
	}
}
