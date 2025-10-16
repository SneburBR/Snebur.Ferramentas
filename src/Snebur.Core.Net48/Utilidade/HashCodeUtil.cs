//using System;

//namespace Snebur.Utilidade
//{
//    public static class HashCodeUtil
//    {
//        public static int GetHashCodeFromString(string str)
//        {
//            var hash = 0;
//            if (str.Length == 0)
//            {
//                return hash;
//            }

//            for (var i = 0; i < str.Length; i++)
//            {
//                hash = ((hash << 5) - hash) + Convert.ToInt32(str[i]);
//                hash |= 0;
//            }
//            return hash;
//        }

//        public static long RetronarHashCode(params long[] ids)
//        {
//            var hash = 0L;
//            for (var i = 0; i < ids.Length; i++)
//            {
//                var id = ids[i];
//                hash = ((hash << 5) - hash) + id;
//                hash = hash & hash;
//            }
//            return hash;
//        }

//        public static int RetronarHashCode(params int[] ids)
//        {
//            var hash = 0;
//            for (var i = 0; i < ids.Length; i++)
//            {
//                var id = ids[i];
//                hash = ((hash << 5) - hash) + id;
//                hash = hash & hash;
//            }
//            return hash;
//        }

//        public static int HashDataHora()
//        {
//            var d = DateTime.Now;
//            return RetronarHashCode(d.Day, d.Month, d.Hour);
//        }
//    }
//}
