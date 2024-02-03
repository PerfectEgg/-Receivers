using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Script.LazySingleton
{
    // 간단하게 만든 LazySingleton<T>
    public abstract class LazySingleton<T> where T : LazySingleton<T>, new()
    {
        private static readonly Lazy<T> instance = new Lazy<T>(() => new T());

        protected LazySingleton()
        {

        }

        public static T Instance => instance.Value;
    }
}
