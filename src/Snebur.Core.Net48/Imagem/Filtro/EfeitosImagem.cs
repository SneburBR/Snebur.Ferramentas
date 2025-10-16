using Snebur.Dominio;
using Snebur.Imagens;
using System.Collections.Generic;

namespace Snebur.UI
{
    public static class EfeitosImagem
    {
        public static Dictionary<EnumEfeitoImagem, EfeitoImagem> Efeitos { get; } = new Dictionary<EnumEfeitoImagem, EfeitoImagem>
        {
            {EnumEfeitoImagem.Nenhum, Normal },
            {EnumEfeitoImagem.PretoBranco, PretoBranco },
            {EnumEfeitoImagem.Sepia, Sepia },
            {EnumEfeitoImagem.Cancum, Cancum },
            {EnumEfeitoImagem.Moscou, Moscou },
            {EnumEfeitoImagem.Dubai, Dubai },
            {EnumEfeitoImagem.Paris, Paris },
            {EnumEfeitoImagem.Chicago, Chicago },
            {EnumEfeitoImagem.Veneza, Veneza },
            {EnumEfeitoImagem.Cairo, Cairo },
            {EnumEfeitoImagem.Acapulco, Acapulco },
            {EnumEfeitoImagem.Fortaleza, Fortaleza },
            {EnumEfeitoImagem.Pequim, Pequim },
            {EnumEfeitoImagem.Atenas, Atenas },
            {EnumEfeitoImagem.Manaus, Dakar },
            {EnumEfeitoImagem.Rio, Rio },
            {EnumEfeitoImagem.Sydney, Sydney },
            {EnumEfeitoImagem.Vancouver, Vancouver },
            {EnumEfeitoImagem.SaoPaulo, SaoPaulo },
            {EnumEfeitoImagem.Jaipur, Jaipur },
            {EnumEfeitoImagem.Medellin, Medellin },
            {EnumEfeitoImagem.Londres, Londres }
        };

        private static EfeitoImagem _normal;
        private static EfeitoImagem _pretoBranco;
        private static EfeitoImagem _sepia;
        private static EfeitoImagem _cancum;
        private static EfeitoImagem _moscou;
        private static EfeitoImagem _dubai;
        private static EfeitoImagem _paris;
        private static EfeitoImagem _chicago;
        private static EfeitoImagem _veneza;
        private static EfeitoImagem _cairo;
        private static EfeitoImagem _acapulco;
        private static EfeitoImagem _fortaleza;
        private static EfeitoImagem _pequim;
        private static EfeitoImagem _atenas;
        private static EfeitoImagem _dakar;
        private static EfeitoImagem _rio;
        private static EfeitoImagem _sydney;
        private static EfeitoImagem _vancouver;
        private static EfeitoImagem _saoPaulo;
        private static EfeitoImagem _jaipur;
        private static EfeitoImagem _medellin;
        private static EfeitoImagem _londres;

        public static EfeitoImagem Normal
        {
            get
            {
                if (_normal == null)
                {
                    _normal = new EfeitoImagem(EnumEfeitoImagem.Nenhum, FiltroImagem.Empty);
                }
                return _normal;
            }
        }

        public static EfeitoImagem PretoBranco
        {
            get
            {
                if (_pretoBranco == null)
                {
                    var filtro = new FiltroImagem
                    {
                        PretoBranco = 100
                    };
                    _pretoBranco = new EfeitoImagem(EnumEfeitoImagem.PretoBranco, filtro);
                }
                return _pretoBranco;
            }
        }

        public static EfeitoImagem Sepia
        {
            get
            {
                if (_sepia == null)
                {
                    var filtro = new FiltroImagem
                    {
                        Sepia = 100
                    };
                    _sepia = new EfeitoImagem(EnumEfeitoImagem.Sepia, filtro);
                }
                return _sepia;
            }
        }
        //1977
        public static EfeitoImagem Cancum
        {
            get
            {
                if (_cancum == null)
                {
                    var sobrePosicao = new SobrePosicaoSolida
                    {
                        Cor = "rgba(243,106,188,1)",
                        Mixagem = EnumMixagem.SoftLight,
                    };

                    var filtro = new FiltroImagem
                    {
                        Contraste = 110,
                        Brilho = 110,
                        Saturacao = 130
                    };

                    _cancum = new EfeitoImagem(EnumEfeitoImagem.Cancum, filtro, sobrePosicao);
                }
                return _cancum;
            }
        }
        //Aden
        public static EfeitoImagem Moscou
        {
            get
            {
                if (_moscou == null)
                {
                    var sobreposicao = new SobrePosicaoGradienteLinear
                    {
                        Cor1 = "rgba(66, 10, 14, 1)",
                        Cor2 = "rgba(255, 255, 255, 1)",
                        LimiteCor1 = 0,
                        LimiteCor2 = 100,
                        Direcao = EnumDirecaoGradiente.ToRight,
                        Mixagem = EnumMixagem.SoftLight
                    };

                    var filtro = new FiltroImagem
                    {
                        Contraste = 90,
                        Brilho = 120,
                        Saturacao = 85,
                        Matriz = 20,
                    };

                    _moscou = new EfeitoImagem(EnumEfeitoImagem.Moscou, filtro, sobreposicao);
                }
                return _moscou;
            }
        }
        //Amaro
        public static EfeitoImagem Dubai
        {
            get
            {
                if (_dubai == null)
                {
                    var filtro = new FiltroImagem
                    {
                        Contraste = 90,
                        Brilho = 110,
                        Saturacao = 150,
                        Matriz = 350
                    };
                    _dubai = new EfeitoImagem(EnumEfeitoImagem.Dubai, filtro);
                }
                return _dubai;
            }
        }
        //Brannan
        public static EfeitoImagem Paris
        {
            get
            {
                if (_paris == null)
                {
                    var sobrePosicao = new SobrePosicaoSolida
                    {
                        Cor = "rgba(161, 44, 199, 1)",
                        Mixagem = EnumMixagem.SoftLight
                    };

                    var filtro = new FiltroImagem
                    {
                        Contraste = 140,
                        Sepia = 50
                    };
                    _paris = new EfeitoImagem(EnumEfeitoImagem.Paris, filtro, sobrePosicao);
                }
                return _paris;
            }
        }
        //Brooklyn
        public static EfeitoImagem Chicago
        {
            get
            {
                if (_chicago == null)
                {
                    var sobrePosicao = new SobrePosicaoGradienteLinear
                    {
                        Cor1 = "rgba(168, 223, 193, 1)",
                        Cor2 = "rgba(183, 196, 200, 1)",
                        LimiteCor1 = 0,
                        LimiteCor2 = 100,
                        Direcao = EnumDirecaoGradiente.ToRight,
                        Mixagem = EnumMixagem.SoftLight
                    };

                    var filtro = new FiltroImagem
                    {
                        Contraste = 90,
                        Brilho = 110
                    };

                    _chicago = new EfeitoImagem(EnumEfeitoImagem.Chicago, filtro, sobrePosicao);
                }
                return _chicago;
            }
        }
        //Clarendon
        public static EfeitoImagem Veneza
        {
            get
            {
                if (_veneza == null)
                {
                    var sobrePosicao = new SobrePosicaoSolida
                    {
                        Cor = "rgba(127,187,227,1)",
                        Mixagem = EnumMixagem.SoftLight
                    };

                    var filtro = new FiltroImagem
                    {
                        Contraste = 120,
                        Saturacao = 125
                    };

                    _veneza = new EfeitoImagem(EnumEfeitoImagem.Veneza, filtro, sobrePosicao);
                }
                return _veneza;
            }
        }
        //Earlybird
        public static EfeitoImagem Cairo
        {
            get
            {
                if (_cairo == null)
                {
                    var sobrePosicao = new SobrePosicaoGradienteLinear
                    {
                        Cor1 = "rgba(208,186,142,1)",
                        Cor2 = "rgba(29,2,16,1)",
                        LimiteCor1 = 0,
                        LimiteCor2 = 100,
                        Direcao = EnumDirecaoGradiente.ToRight,
                        Mixagem = EnumMixagem.SoftLight,
                    };

                    var filtro = new FiltroImagem
                    {
                        Contraste = 90,
                        Sepia = 20
                    };

                    _cairo = new EfeitoImagem(EnumEfeitoImagem.Cairo, filtro, sobrePosicao);
                }
                return _cairo;
            }
        }
        //Gingham
        public static EfeitoImagem Acapulco
        {
            get
            {
                if (_acapulco == null)
                {
                    var sobreposicao = new SobrePosicaoGradienteLinear
                    {
                        Cor1 = "rgba(66, 10, 14, 1)",
                        Cor2 = "rgba(255,255,255,1)",
                        LimiteCor1 = 0,
                        LimiteCor2 = 100,
                        Direcao = EnumDirecaoGradiente.ToRight,
                        Mixagem = EnumMixagem.SoftLight
                    };

                    var filtro = new FiltroImagem
                    {
                        Brilho = 105,
                        Matriz = 350
                    };

                    _acapulco = new EfeitoImagem(EnumEfeitoImagem.Acapulco, filtro, sobreposicao);
                }
                return _acapulco;
            }
        }
        //Hudson
        public static EfeitoImagem Fortaleza
        {
            get
            {
                if (_fortaleza == null)
                {
                    var sobrePosicao = new SobrePosicaoGradienteLinear
                    {
                        Cor1 = "rgba(255,177,166,1)",
                        Cor2 = "rgba(52,33,52,1)",
                        LimiteCor1 = 0,
                        LimiteCor2 = 100,
                        Direcao = EnumDirecaoGradiente.ToRight,
                        Mixagem = EnumMixagem.SoftLight
                    };

                    var filtro = new FiltroImagem
                    {
                        Contraste = 90,
                        Brilho = 120,
                        Saturacao = 110
                    };

                    _fortaleza = new EfeitoImagem(EnumEfeitoImagem.Fortaleza, filtro, sobrePosicao);
                }
                return _fortaleza;
            }
        }
        //InkWell
        public static EfeitoImagem Pequim
        {
            get
            {
                if (_pequim == null)
                {
                    var filtro = new FiltroImagem
                    {
                        Brilho = 110,
                        Contraste = 110,
                        Sepia = 30
                    };

                    _pequim = new EfeitoImagem(EnumEfeitoImagem.Pequim, filtro/*, sobrePosicao*/);
                }
                return _pequim;
            }
        }
        //lofi
        public static EfeitoImagem Atenas
        {
            get
            {
                if (_atenas == null)
                {
                    var sobrePosicao = new SobrePosicaoGradienteLinear
                    {
                        Cor1 = "rgba(255,255,255,1)",
                        Cor2 = "rgba(34,34,34,1)",
                        LimiteCor1 = 0,
                        LimiteCor2 = 100,
                        Direcao = EnumDirecaoGradiente.ToRight,
                        Mixagem = EnumMixagem.SoftLight
                    };

                    var filtro = new FiltroImagem
                    {
                        Contraste = 150,
                        Saturacao = 110
                    };

                    _atenas = new EfeitoImagem(EnumEfeitoImagem.Atenas, filtro, sobrePosicao);
                }
                return _atenas;
            }
        }
        //Maven
        public static EfeitoImagem Dakar
        {
            get
            {
                if (_dakar == null)
                {
                    var sobrePosicao = new SobrePosicaoSolida
                    {
                        Cor = "rgba(3,230,26,1)",
                        Mixagem = EnumMixagem.SoftLight
                    };

                    var filtro = new FiltroImagem
                    {
                        Contraste = 95,
                        Brilho = 95,
                        Saturacao = 150,
                        Sepia = 25
                    };

                    _dakar = new EfeitoImagem(EnumEfeitoImagem.Manaus, filtro, sobrePosicao);
                }
                return _dakar;
            }
        }
        //Perpetua
        public static EfeitoImagem Rio
        {
            get
            {
                if (_rio == null)
                {
                    var sobreposicao = new SobrePosicaoGradienteLinear
                    {
                        Cor1 = "rgba(0, 91,154,1)",
                        Cor2 = "rgba(255,255,255,1)",
                        LimiteCor1 = 0,
                        LimiteCor2 = 100,
                        Direcao = EnumDirecaoGradiente.ToBottom,
                        Mixagem = EnumMixagem.SoftLight
                    };
                    _rio = new EfeitoImagem(EnumEfeitoImagem.Rio, FiltroImagem.Empty, sobreposicao);
                }
                return _rio;
            }
        }
        //Reyes
        public static EfeitoImagem Sydney
        {
            get
            {
                if (_sydney == null)
                {
                    var sobrePosicao = new SobrePosicaoSolida
                    {
                        Cor = "rgba(173, 205, 239, 1)",
                        Mixagem = EnumMixagem.SoftLight
                    };

                    var filtro = new FiltroImagem
                    {
                        Contraste = 85,
                        Brilho = 110,
                        Saturacao = 75,
                        Sepia = 22
                    };

                    _sydney = new EfeitoImagem(EnumEfeitoImagem.Sydney, filtro, sobrePosicao);
                }
                return _sydney;
            }
        }
        //Stinson
        public static EfeitoImagem Vancouver
        {
            get
            {
                if (_vancouver == null)
                {
                    var sobrePosicao = new SobrePosicaoSolida
                    {
                        Cor = "rgba(240, 149, 128, 1)",
                        Mixagem = EnumMixagem.SoftLight
                    };

                    var filtro = new FiltroImagem
                    {
                        Contraste = 75,
                        Brilho = 115,
                        Saturacao = 85
                    };

                    _vancouver = new EfeitoImagem(EnumEfeitoImagem.Vancouver, filtro, sobrePosicao);
                }
                return _vancouver;
            }
        }
        //Toaster
        public static EfeitoImagem SaoPaulo
        {
            get
            {
                if (_saoPaulo == null)
                {
                    var sobreposicao = new SobrePosicaoGradienteLinear
                    {
                        Cor1 = "rgba(15, 78, 128, 1)",
                        Cor2 = "rgba(59, 0, 59, 1)",
                        LimiteCor1 = 0,
                        LimiteCor2 = 100,
                        Direcao = EnumDirecaoGradiente.ToRight,
                        Mixagem = EnumMixagem.SoftLight
                    };

                    var filtro = new FiltroImagem
                    {
                        Contraste = 150,
                        Brilho = 90
                    };

                    _saoPaulo = new EfeitoImagem(EnumEfeitoImagem.SaoPaulo, filtro, sobreposicao);
                }
                return _saoPaulo;
            }
        }
        //Walden
        public static EfeitoImagem Jaipur
        {
            get
            {
                if (_jaipur == null)
                {
                    var sobrePosicao = new SobrePosicaoSolida
                    {
                        Cor = "rgba(204, 68, 0, 1)",
                        Mixagem = EnumMixagem.SoftLight
                    };

                    var filtro = new FiltroImagem
                    {
                        Brilho = 110,
                        Saturacao = 160,
                        Sepia = 30,
                        Matriz = 350
                    };

                    _jaipur = new EfeitoImagem(EnumEfeitoImagem.Jaipur, filtro, sobrePosicao);
                }
                return _jaipur;
            }
        }
        //Valencia
        public static EfeitoImagem Medellin
        {
            get
            {
                if (_medellin == null)
                {
                    var sobrePosicao = new SobrePosicaoSolida
                    {
                        Cor = "rgba(58, 3, 57, 1)",
                        Mixagem = EnumMixagem.SoftLight
                    };

                    var filtro = new FiltroImagem
                    {
                        Contraste = 108,
                        Brilho = 108,
                        Sepia = 8
                    };

                    _medellin = new EfeitoImagem(EnumEfeitoImagem.Medellin, filtro, sobrePosicao);
                }
                return _medellin;
            }
        }
        //XPro2
        public static EfeitoImagem Londres
        {
            get
            {
                if (_londres == null)
                {
                    var sobreposicao = new SobrePosicaoGradienteLinear
                    {
                        Cor1 = "rgba(224, 231, 230, 1)",
                        Cor2 = "rgba(43, 42, 161, 1)",
                        LimiteCor1 = 0,
                        LimiteCor2 = 100,
                        Direcao = EnumDirecaoGradiente.ToRight,
                        Mixagem = EnumMixagem.SoftLight
                    };

                    var filtro = new FiltroImagem
                    {
                        Sepia = 30
                    };

                    _londres = new EfeitoImagem(EnumEfeitoImagem.Londres, filtro, sobreposicao);
                }
                return _londres;
            }
        }
    }
}