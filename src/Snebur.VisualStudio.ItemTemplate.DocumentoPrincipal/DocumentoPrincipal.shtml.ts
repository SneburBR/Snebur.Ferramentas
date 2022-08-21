
namespace $rootnamespace$
{
	export class $safeitemrootname$ extends Snebur.UI.DocumentoPrincipal
	{
        public constructor() 
		{
			super();
            this.EventoCarregado.AddHandler(this.Documento_Carregado.bind(this));
		}	

        public Documento_Carregado(provedor: any, e: EventArgs) 
		{
			//pagina carregada
		}
	}
}
