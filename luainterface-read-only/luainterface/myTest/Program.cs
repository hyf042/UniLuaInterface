using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
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
            Console.WriteLine("test1");
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            Lua lua = new Lua();
            lua.DoFile("test.lua");
            /*object obj = null;
            GCHandle handle = GCHandle.Alloc(obj);
            Console.WriteLine(GCHandle.ToIntPtr(handle));*/
        }
    }
}
