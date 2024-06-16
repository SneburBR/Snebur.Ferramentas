using System;
using System.Linq;

namespace Snebur.Utilidade
{
    public class ZidUtil
    {
        public static long RetornarZid(long id)
        {
            var random = new Random();
            var soma = random.Next(10, 99);
            return RetornarZid(id, soma);
        }

        public static long RetornarZid(long id, int rnd)
        {
            var novaSequencia = String.Empty;
            if (rnd < 29)
            {
                foreach (char charId in id.ToString())
                {
                    if (charId == '0')
                    {
                        novaSequencia += '0';
                    }
                    if (charId == '1')
                    {
                        novaSequencia += '8';
                    }
                    if (charId == '2')
                    {
                        novaSequencia += '1';
                    }
                    if (charId == '3')
                    {
                        novaSequencia += '4';
                    }
                    if (charId == '4')
                    {
                        novaSequencia += '5';
                    }
                    if (charId == '5')
                    {
                        novaSequencia += '2';
                    }
                    if (charId == '6')
                    {
                        novaSequencia += '9';
                    }
                    if (charId == '7')
                    {
                        novaSequencia += '3';
                    }
                    if (charId == '8')
                    {
                        novaSequencia += '7';
                    }
                    if (charId == '9')
                    {
                        novaSequencia += '6';
                    }
                }
            }
            else if (rnd > 58)
            {
                foreach (char charId in id.ToString())
                {
                    if (charId == '0')
                    {
                        novaSequencia += '0';
                    }
                    if (charId == '1')
                    {
                        novaSequencia += '3';
                    }
                    if (charId == '2')
                    {
                        novaSequencia += '2';
                    }
                    if (charId == '3')
                    {
                        novaSequencia += '8';
                    }
                    if (charId == '4')
                    {
                        novaSequencia += '6';
                    }
                    if (charId == '5')
                    {
                        novaSequencia += '1';
                    }
                    if (charId == '6')
                    {
                        novaSequencia += '7';
                    }
                    if (charId == '7')
                    {
                        novaSequencia += '9';
                    }
                    if (charId == '8')
                    {
                        novaSequencia += '4';
                    }
                    if (charId == '9')
                    {
                        novaSequencia += '5';
                    }
                }
            }
            else
            {
                foreach (char charId in id.ToString())
                {
                    if (charId == '0')
                    {
                        novaSequencia += '0';
                    }
                    if (charId == '1')
                    {
                        novaSequencia += '5';
                    }
                    if (charId == '2')
                    {
                        novaSequencia += '7';
                    }
                    if (charId == '3')
                    {
                        novaSequencia += '6';
                    }
                    if (charId == '4')
                    {
                        novaSequencia += '3';
                    }
                    if (charId == '5')
                    {
                        novaSequencia += '8';
                    }
                    if (charId == '6')
                    {
                        novaSequencia += '1';
                    }
                    if (charId == '7')
                    {
                        novaSequencia += '2';
                    }
                    if (charId == '8')
                    {
                        novaSequencia += '9';
                    }
                    if (charId == '9')
                    {
                        novaSequencia += '4';
                    }
                }
            }

            var random = new Random();
            var total = long.Parse(novaSequencia) + rnd;
            var strSplit = rnd.ToString() + total.ToString().Insert(total.ToString().Length - 1, random.Next(1, 9).ToString()).Insert(total.ToString().Length + 1, random.Next(1, 9).ToString());

            return long.Parse(strSplit);
        }

        public static int RetornarId(long zid)
        {
            var novaSequencia = "";

            var strId = zid.ToString().Substring(2, zid.ToString().Length - 2);
            var subtracao = Convert.ToInt32(zid.ToString().Substring(0, 2));
            strId = strId.Remove(strId.Length - 1, 1);
            strId = strId.Remove(strId.Length - 2, 1);
            zid = long.Parse(strId) - subtracao;

            if (subtracao < 29)
            {
                foreach (char charId in zid.ToString())
                {
                    if (charId == '0')
                    {
                        novaSequencia += '0';
                    }
                    if (charId == '8')
                    {
                        novaSequencia += '1';
                    }
                    if (charId == '1')
                    {
                        novaSequencia += '2';
                    }
                    if (charId == '4')
                    {
                        novaSequencia += '3';
                    }
                    if (charId == '5')
                    {
                        novaSequencia += '4';
                    }
                    if (charId == '2')
                    {
                        novaSequencia += '5';
                    }
                    if (charId == '9')
                    {
                        novaSequencia += '6';
                    }
                    if (charId == '3')
                    {
                        novaSequencia += '7';
                    }
                    if (charId == '7')
                    {
                        novaSequencia += '8';
                    }
                    if (charId == '6')
                    {
                        novaSequencia += '9';
                    }
                }
            }
            else if (subtracao > 58)
            {
                foreach (char charId in zid.ToString())
                {
                    if (charId == '0')
                    {
                        novaSequencia += '0';
                    }
                    if (charId == '3')
                    {
                        novaSequencia += '1';
                    }
                    if (charId == '2')
                    {
                        novaSequencia += '2';
                    }
                    if (charId == '8')
                    {
                        novaSequencia += '3';
                    }
                    if (charId == '6')
                    {
                        novaSequencia += '4';
                    }
                    if (charId == '1')
                    {
                        novaSequencia += '5';
                    }
                    if (charId == '7')
                    {
                        novaSequencia += '6';
                    }
                    if (charId == '9')
                    {
                        novaSequencia += '7';
                    }
                    if (charId == '4')
                    {
                        novaSequencia += '8';
                    }
                    if (charId == '5')
                    {
                        novaSequencia += '9';
                    }
                }
            }
            else
            {
                foreach (char charId in zid.ToString())
                {
                    if (charId == '0')
                    {
                        novaSequencia += '0';
                    }
                    if (charId == '5')
                    {
                        novaSequencia += '1';
                    }
                    if (charId == '7')
                    {
                        novaSequencia += '2';
                    }
                    if (charId == '6')
                    {
                        novaSequencia += '3';
                    }
                    if (charId == '3')
                    {
                        novaSequencia += '4';
                    }
                    if (charId == '8')
                    {
                        novaSequencia += '5';
                    }
                    if (charId == '1')
                    {
                        novaSequencia += '6';
                    }
                    if (charId == '2')
                    {
                        novaSequencia += '7';
                    }
                    if (charId == '9')
                    {
                        novaSequencia += '8';
                    }
                    if (charId == '4')
                    {
                        novaSequencia += '9';
                    }
                }
            }

            return int.Parse(novaSequencia);
        }


        public static void Teste()
        {
            var ids = Enumerable.Range(1, 10000).ToList();
            var rnds = Enumerable.Range(10, 89).ToList();
            foreach (var id in ids)
            {
                foreach (var rnd in rnds)
                {
                    var zid = ZidUtil.RetornarZid(id, rnd);
                    var idVal = ZidUtil.RetornarId(zid);
                    if (idVal != id)
                    {
                        throw new Exception("falha");
                    }
                }
            }

            //ZidUtil.TestarZid(18, 109672);
            //ZidUtil.TestarZid(282, 6831306);
            //ZidUtil.TestarZid(231, 1516333);
            //ZidUtil.TestarZid(174, 7046466);
            //ZidUtil.TestarZid(59, 769216);
            //ZidUtil.TestarZid(2, 222334);
            //ZidUtil.TestarZid(3, 293653);
        }

        private static void TestarZid(int id, int zid)
        {
            var idVal = ZidUtil.RetornarId(zid);
            if (idVal != id)
            {
                throw new Exception("falha");
            }
        }
    }
}