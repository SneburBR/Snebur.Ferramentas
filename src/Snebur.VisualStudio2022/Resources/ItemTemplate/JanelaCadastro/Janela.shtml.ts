
namespace $rootnamespace$
{
	export class $safeitemrootname$ extends Snebur.UI.BaseJanelaCadastro
	{
		public constructor(controlePai: Snebur.UI.BaseControle, entidade: entidades.TIPO_ENTIDADE) 
		{
			super(controlePai, entidade);
            this.EventoCarregado.AddHandler(this.Janela_Carregada, this);
		}	

        private Janela_Carregada(provedor: any, e: EventArgs) 
		{
			//pagina carregada
		}
	}
}
