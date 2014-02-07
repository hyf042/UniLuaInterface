using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LuaInterface;

namespace myTest
{
    public class Bundle
    {
        public int a;
        public void test()
        {
            Console.WriteLine(a);
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            Lua lua = new Lua();
            var bundle = new Bundle();
            lua["a"] = new Bundle();
            lua.DoString("print(a)");
        }
    }
}
