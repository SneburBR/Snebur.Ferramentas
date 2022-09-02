
namespace $rootnamespace$
{
	export class $safeitemrootname$ extends Snebur.UI.Janela
	{
		public constructor(controlePai: Snebur.UI.BaseControle) 
		{
			super(controlePai);
            this.EventoCarregado.AddHandler(this.Janela_Carregada, this);
		}	

        private Janela_Carregada(provedor: any, e: EventArgs) 
		{
			//pagina carregada
		}
	}
}
