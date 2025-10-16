using System;

namespace Snebur.Utilidade
{
    public class SnRandom
    {
        public int Seed { get; private set; }

        public SnRandom()
        {
            var now = DateTime.Now;
            this.Seed = (now.Hour * 100) + now.Millisecond;
        }
        public SnRandom(int seed)
        {
            this.Seed = seed;
        }
        public int Next(int maximo)
        {
            return this.Next(0, maximo);
        }
        public int Next(int minimo, int maximo)
        {
            if (minimo < 0 || maximo < 0 || minimo > maximo)
            {
                throw new ErroOperacaoInvalida("Argumentos invalido, requerido numeros positivo e minimo > maximo");
            }

            this.Seed += 1;
            var sin = Math.Sin(this.Seed) * (maximo - minimo);
            var resultado = Math.Abs(Convert.ToInt32(Math.Floor(sin)));

            return resultado > minimo ? Math.Min(resultado, maximo) :
                                        Math.Max(resultado, minimo);
        }
    }
}
