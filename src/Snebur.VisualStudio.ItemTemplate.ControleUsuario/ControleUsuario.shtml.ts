
namespace $rootnamespace$
{
    export class $safeitemrootname$ extends Snebur.UI.ControleUsuario
    {
        public constructor(controlePai: Snebur.UI.BaseControle, elemento: HTMLElement);
        public constructor(controlePai: Snebur.UI.BaseControle, idElemento: string);
        public constructor(controlePai: Snebur.UI.BaseControle, refElemento: any) 
        {
            super(controlePai, refElemento);
            this.EventoCarregado.AddHandler(this.Controle_Carregado.bind(this));
        }

        public Controle_Carregado(provedor: any, e: EventArgs) 
        {
            //controle carregada
        }
	}
}
