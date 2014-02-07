/*
 * Time         : 2/6/2014
 * Author       : hyf042
 * 
 * Copyright (c) 2013 GameMaster
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UniLua;

namespace LuaBind
{
    public class ObjectTranslator
    {
        internal Lua interpreter;
        public TypeChecker typeChecker { get; private set; }
        public ILuaState luaState { get { return interpreter.luaState; } }
        // object to object #
        public readonly Dictionary<object, int> objectsBackMap = new Dictionary<object, int>();

        public ObjectTranslator(Lua interpreter)
        {
            this.interpreter = interpreter;

            typeChecker = new TypeChecker(this);
        }

        internal object getObject(int index)
        {
            switch (luaState.Type(index))
            {
                case LuaType.LUA_TNUMBER:
                    return luaState.ToNumber(index);
                case LuaType.LUA_TSTRING:
                    return luaState.ToString(index);
                case LuaType.LUA_TBOOLEAN:
                    return luaState.ToBoolean(index);
                //case LuaType.LUA_TTABLE:
                //    return getTable(luaState, index);
                case LuaType.LUA_TFUNCTION:
                    return getFunction(index);
                //case LuaType.LUA_TUSERDATA:
                //    return getUserData(luaState, index);                    
                default:
                    return null;
            }
        }
        /*
		 * Gets the function in the index positon of the Lua stack.
		 */
        internal LuaFunction getFunction(int index)
        {
            luaState.PushValue(index);
            return new LuaFunction(luaState.L_Ref(LuaDef.LUA_REGISTRYINDEX), interpreter);
        }
        //internal LuaTable getTable(ILuaState luaState,int index) 
        //{
        //    luaState.PushValue(index);
        //    return new LuaTable(luaState.L_Ref(LuaDef.LUA_REGISTRYINDEX),interpreter);
        //}

        /*
		 * Pushes the object into the Lua stack according to its type.
		 */
        internal void push(object o)
        {
            if (o == null)
            {
                luaState.PushNil();
            }
            else if (o is sbyte || o is byte || o is short || o is ushort ||
                o is int || o is uint || o is long || o is float ||
                o is ulong || o is decimal || o is double)
            {
                double d = Convert.ToDouble(o);
                luaState.PushNumber(d);
            }
            else if (o is char)
            {
                double d = (char)o;
                luaState.PushNumber(d);
            }
            else if (o is string)
            {
                string str = (string)o;
                luaState.PushString(str);
            }
            else if (o is bool)
            {
                bool b = (bool)o;
                luaState.PushBoolean(b);
            }
            else if (o is CSharpFunctionDelegate)
            {
                pushFunction((CSharpFunctionDelegate)o);
            }
            else if (o is LuaFunction)
            {
                ((LuaFunction)o).push();
            }
            //else if (IsILua(o))
            //{
            //    (((ILuaGeneratedType)o).__luaInterface_getLuaTable()).push(luaState);
            //}
            //else if (o is LuaTable)
            //{
            //    ((LuaTable)o).push(luaState);
            //}
            //else
            //{
            //    pushObject(luaState, o, "luaNet_metatable");
            //}
        }
        /*
		 * Pushes a delegate into the stack
		 */
        internal void pushFunction(CSharpFunctionDelegate func)
        {
            luaState.PushCSharpFunction(func);
        }

        internal object[] popValues(int oldTop)
        {
            int newTop = luaState.GetTop();
            if (oldTop == newTop)
            {
                return null;
            }
            else
            {
                ArrayList returnValues = new ArrayList();
                for (int i = oldTop + 1; i <= newTop; i++)
                {
                    returnValues.Add(getObject(i));
                }
                luaState.SetTop(oldTop);
                return returnValues.ToArray();
            }
        }

        /*
		 * Passes errors (argument e) to the Lua interpreter
		 */
        internal int throwErrorInLua(object e)
        {
            if (e == null)
                return 0;

            // We use this to remove anything pushed by luaL_where
            int oldTop = luaState.GetTop();

            // Stack frame #1 is our C# wrapper, so not very interesting to the user
            // Stack frame #2 must be the lua code that called us, so that's what we want to use
            luaState.L_Where(1);
            object[] curlev = popValues(oldTop);

            // Determine the position in the script where the exception was triggered
            string errLocation = "";
            if (curlev.Length > 0)
                errLocation = curlev[0].ToString();

            string message = e as string;
            if (message != null)
            {
                // Wrap Lua error (just a string) and store the error location
                e = new LuaScriptException(message, errLocation);
            }
            else
            {
                Exception ex = e as Exception;
                if (ex != null)
                {
                    // Wrap generic .NET exception as an InnerException and store the error location
                    e = new LuaScriptException(ex, errLocation);
                }
            }

            push(e.ToString());
            luaState.Error();
            luaState.PushNil();

            return 1;
        }
    }
}
