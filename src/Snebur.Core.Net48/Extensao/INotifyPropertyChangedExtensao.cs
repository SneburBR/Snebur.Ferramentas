//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Linq;
//using System.Runtime.CompilerServices;
//using System.Text;
//using System.Threading.Tasks;

//namespace Snebur.Extensao
//{
//    public static class INotifyPropertyChangedExtensao
//    {
//        public static void NotificarPropriedadeAlterada(object source, [CallerMemberName]  string nomePropriedade = "")
//        {
//            source.PropertyChanged?.Invoke(source, new PropertyChangedEventArgs(nomePropriedade));
//        }
//    }
//}
