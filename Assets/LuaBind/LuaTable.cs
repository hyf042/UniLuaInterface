/*
 * Time         : 2/6/2014
 * Author       : hyf042
 * 
 * Copyright (c) 2013 GameMaster
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using UniLua;

namespace LuaBind
{
    /*
	 * Wrapper class for Lua tables
	 *
	 * Author: Fabio Mascarenhas
	 * Version: 1.0
	 */
    //public class LuaTable : LuaBase
    //{
    //    public LuaTable(int reference, Lua interpreter)
    //    {
    //        _Reference = reference;
    //        _Interpreter = interpreter;
    //    }

        
    //    /*
    //     * Indexer for string fields of the table
    //     */
    //    public object this[string field]
    //    {
    //        get
    //        {
    //            return _Interpreter.getObject(_Reference, field);
    //        }
    //        set
    //        {
    //            _Interpreter.setObject(_Reference, field, value);
    //        }
    //    }
    //    /*
    //     * Indexer for numeric fields of the table
    //     */
    //    public object this[object field]
    //    {
    //        get
    //        {
    //            return _Interpreter.getObject(_Reference, field);
    //        }
    //        set
    //        {
    //            _Interpreter.setObject(_Reference, field, value);
    //        }
    //    }


    //    public System.Collections.IDictionaryEnumerator GetEnumerator()
    //    {
    //        return _Interpreter.GetTableDict(this).GetEnumerator();
    //    }

    //    public ICollection Keys
    //    {
    //        get { return _Interpreter.GetTableDict(this).Keys; }
    //    }

    //    public ICollection Values
    //    {
    //        get { return _Interpreter.GetTableDict(this).Values; }
    //    }

    //    /*
    //     * Gets an string fields of a table ignoring its metatable,
    //     * if it exists
    //     */
    //    internal object rawget(string field)
    //    {
    //        return _Interpreter.rawGetObject(_Reference, field);
    //    }

    //    internal object rawgetFunction(string field)
    //    {
    //        object obj = _Interpreter.rawGetObject(_Reference, field);

    //        if (obj is CSharpFunctionDelegate)
    //            return new LuaFunction((CSharpFunctionDelegate)obj, _Interpreter);
    //        else
    //            return obj;
    //    }

    //    /*
    //     * Pushes this table into the Lua stack
    //     */
    //    internal void push()
    //    {
    //        luaState.RawGetI(LuaDef.LUA_REGISTRYINDEX, _Reference);
    //    }
    //    public override string ToString()
    //    {
    //        return "table";
    //    }
    //}
}
