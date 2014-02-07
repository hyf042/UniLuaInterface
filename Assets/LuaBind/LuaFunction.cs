using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UniLua;

namespace LuaBind
{
    public class LuaFunction : LuaBase
    {
        internal CSharpFunctionDelegate function;
        //internal int reference;

        public LuaFunction(int reference, Lua interpreter) : base(reference, interpreter)
        {
            this.function = null;
        }

        public LuaFunction(CSharpFunctionDelegate function, Lua interpreter)
            : base(0, interpreter)
        {
            this.function = function;
        }

        /*
         * Calls the function and returns its return values inside
         * an array
         */
        public object[] Call(params object[] args)
        {
            return _Interpreter.callFunction(this, args);
        }
        /*
         * Pushes the function into the Lua stack
         */
        internal void push()
        {
            if (_Reference != 0)
                _Interpreter.luaState.RawGetI(LuaDef.LUA_REGISTRYINDEX, _Reference);
            else
                _Interpreter.pushCSFunction(function);
        }
        public override string ToString()
        {
            return "function";
        }
        public override bool Equals(object o)
        {
            if (o is LuaFunction)
            {
                LuaFunction l = (LuaFunction)o;
                if (this._Reference != 0 && l._Reference != 0)
                    return _Interpreter.compareRef(l._Reference, this._Reference);
                else
                    return this.function == l.function;
            }
            else return false;
        }
        public override int GetHashCode()
        {
            if (_Reference != 0)
                return _Reference;
            else
                return function.GetHashCode();
        }
    }
}
