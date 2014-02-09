using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Linq;
using System.Text;
using UniLua;
using UnityEngine;

namespace LuaInterface
{
    public static class LuaDLL
    {
        static object tag = new object();

        
        public static ILuaState luaL_newstate()
        {
            return LuaAPI.NewState();
        }
        public static void luaL_openlibs(ILuaState luaState)
        {
            luaState.L_OpenLibs();
        }
        public static void lua_pushstring(ILuaState luaState, string s)
        {
            luaState.PushString(s);
        }
        public static void lua_settable(ILuaState luaState, int index)
        {
            luaState.SetTable(index);
        }
        public static void lua_newtable(ILuaState luaState)
        {
            luaState.NewTable();
        }
        public static void lua_setglobal(ILuaState luaState, string name)
        {
            luaState.SetGlobal(name);
        }
        public static void lua_getglobal(ILuaState luaState, string name)
        {
            luaState.GetGlobal(name);
        }
        public static void lua_pushvalue(ILuaState luaState, int index)
        {
            luaState.PushValue(index);
        }
        public static void lua_replace(ILuaState luaState, int index)
        {
            luaState.Replace(index);
        }
        public static void luaL_dostring(ILuaState luaState, string s)
        {
            luaState.L_DoString(s);
        }
        public static void lua_atpanic(ILuaState luaState, CSharpFunctionDelegate cb)
        {
            //Debug.LogWarning("not implement");
        }
        public static void lua_close(ILuaState luaState)
        {
            //Debug.LogWarning("not implement");
        }
        public static string lua_tostring(ILuaState luaState, int index)
        {
            return luaState.ToString(index);
        }
        public static bool lua_toboolean(ILuaState luaState, int index)
        {
            return luaState.ToBoolean(index);
        }
        public static double lua_tonumber(ILuaState luaState, int index)
        {
            return luaState.ToNumber(index);
        }
        public static void lua_settop(ILuaState luaState, int top)
        {
            luaState.SetTop(top);
        }
        public static int lua_gettop(ILuaState luaState)
        {
            return luaState.GetTop();
        }
        public static void lua_pushnil(ILuaState luaState)
        {
            luaState.PushNil();
        }
        public static void lua_pushnumber(ILuaState luaState, double n)
        {
            luaState.PushNumber(n);
        }
        public static void lua_pushboolean(ILuaState luaState, bool b)
        {
            luaState.PushBoolean(b);
        }
        public static bool lua_checkstack(ILuaState luaState, int size)
        {
            return luaState.CheckStack(size);
        }
        public static bool lua_isnil(ILuaState luaState, int index)
        {
            return luaState.IsNil(index);
        }
        public static bool lua_isnumber(ILuaState luaState, int index)
        {
            return luaState.Type(index) == LuaType.LUA_TNUMBER;
        }
        public static bool lua_isboolean(ILuaState luaState, int index)
        {
            return luaState.Type(index) == LuaType.LUA_TBOOLEAN;
        }
        public static bool lua_isstring(ILuaState luaState, int index)
        {
            return luaState.IsString(index);
        }
        public static LuaType lua_type(ILuaState luaState, int index)
        {
            return luaState.Type(index);
        }
        public static bool luaL_getmetafield(ILuaState luaState, int index, string method)
        {
            return luaState.L_GetMetaField(index, method);
        }
        //public static bool luaL_checkmetatable(ILuaState luaState, int index)
        //{
        //    //FIXME
        //    return true;
        //}
        public static void lua_insert(ILuaState luaState, int index)
        {
            luaState.Insert(index);
        }
        public static void lua_remove(ILuaState luaState, int index)
        {
            luaState.Remove(index);
        }
        public static int luaL_loadbuffer(ILuaState luaState, string chunk, string name)
        {
            return (int)luaState.L_LoadBuffer(chunk, name);
        }
        public static int luaL_loadfile(ILuaState luaState, string filename)
        {
            return (int)luaState.L_LoadFile(filename);
        }
        public static int lua_pcall(ILuaState luaState, int n1, int n2, int n3)
        {
            return (int)luaState.PCall(n1, n2, n3);
        }
        public static void lua_gettable(ILuaState luaState, int index)
        {
            luaState.GetTable(index);
        }
        public static int lua_next(ILuaState luaState, int index)
        {
            return luaState.Next(index)?1:0;
        }
        public static void lua_unref(ILuaState luaState, int reference)
        {
            luaState.L_Unref(LuaDef.LUA_REGISTRYINDEX, reference);
        }
        public static int lua_ref(ILuaState luaState, int t)
        {
            return luaState.L_Ref(LuaDef.LUA_REGISTRYINDEX);
        }
        public static void lua_getref(ILuaState luaState, int reference)
        {
            luaState.RawGetI(LuaDef.LUA_REGISTRYINDEX, reference);
        }
        public static void lua_rawget(ILuaState luaState, int index)
        {
            luaState.RawGet(index);
        }
        public static void lua_rawset(ILuaState luaState, int index)
        {
            luaState.RawSet(index);
        }
        public static int lua_equal(ILuaState luaState, int a, int b)
        {
            return luaState.RawEqual(a, b)?1:0;
        }
        public static string lua_typename(ILuaState luaState, LuaType type)
        {
            return luaState.TypeName(type);
        }
        public static void lua_pushstdcallcfunction(ILuaState luaState, CSharpFunctionDelegate cb)
        {
            luaState.PushCSharpFunction(cb);
        }
        public static void lua_rawseti(ILuaState luaState, int index, int n)
        {
            luaState.RawSetI(index, n);
        }
        public static void lua_rawgeti(ILuaState luaState, int index, int n)
        {
            luaState.RawGetI(index, n);
        }
        public static void lua_pushlightuserdata(ILuaState luaState, object o)
        {
            luaState.PushLightUserData(o);
        }
        public static void lua_error(ILuaState luaState)
        {
            luaState.Error();
        }
        public static int lua_getmetatable(ILuaState luaState, int index)
        {
            return luaState.GetMetaTable(index)?1:0;
        }
        public static int lua_setmetatable(ILuaState luaState, int index)
        {
            return luaState.SetMetaTable(index)?1:0;
        }
        public static int luaL_newmetatable(ILuaState luaState, string name)
        {
            luaState.GetField(LuaDef.LUA_REGISTRYINDEX, name);
            if (luaState.IsNil(-1)){
                luaState.Pop(1);
                luaState.NewTable();
                luaState.PushValue(-1);
                luaState.SetField(LuaDef.LUA_REGISTRYINDEX, name);
                //luaState.GetField(LuaDef.LUA_REGISTRYINDEX, name);
                return 1;
            }
            return 0;
        }
        public static void luaL_getmetatable(ILuaState luaState, string name)
        {
            luaState.GetField(LuaDef.LUA_REGISTRYINDEX, name);
        }
        public static bool luaL_checkmetatable(ILuaState luaState, int index)
        {
            bool retVal = false;

            if (luaState.GetMetaTable(index))
            {
                luaState.PushLightUserData(tag);
                luaState.RawGet(-2);
                retVal = !luaState.IsNil(-1);
                luaState.SetTop(-3);
            }

            return retVal;
        }
        public static void luaL_where(ILuaState luaState, int level)
        {
            luaState.L_Where(level);
        }
        public static void lua_pushglobal(ILuaState luaState)
        {
            luaState.PushGlobalTable();
        }
        public static void lua_replaceglobal(ILuaState luaState)
        {
            luaState.RawSetI(LuaDef.LUA_REGISTRYINDEX, LuaDef.LUA_RIDX_GLOBALS);
        }

        public static object luanet_gettag()
        {
            return tag;
        }
        
        //public static object luanet_tonetobject(ILuaState luaState, int index)
        //{
        //    if (luaState.Type(index) == LuaType.LUA_TUSERDATA)
        //    {
        //        if (luaState.)
        //    }
        //}
        public static int luanet_rawnetobj(ILuaState luaState, int index)
        {
            object obj = luaState.ToUserData(index);
            if (obj == null)
                return -1;
            return (int)obj;
        }
        public static int luanet_checkudata(ILuaState luaState, int ud, string name)
        {
            object obj = luaState.ToUserData(ud);

            if (obj != null)
            {
                if (luaState.GetMetaTable(ud))
                {
                    bool isEqual;

                    luaState.GetField(LuaDef.LUA_REGISTRYINDEX, name);

                    isEqual = luaState.RawEqual(-1, -2);

                    luaState.SetTop(-(2) - 1);

                    if (isEqual)
                    {
                        return (int)obj;
                    }
                }
            }

            return -1;
        }
        public static void luanet_newudata(ILuaState luaState, int val)
        {
            luaState.PushUserData(val);
        }
        public static int luanet_tonetobject(ILuaState luaState, int index)
        {
            object udata;

            if (luaState.Type(index) == LuaType.LUA_TUSERDATA || luaState.Type(index) == LuaType.LUA_TLIGHTUSERDATA)
            {
                if (luaL_checkmetatable(luaState, index))
                {
                    udata = luaState.ToUserData(index);
                    if (udata != null)
                        return (int)udata;
                }

                int udatai = luanet_checkudata(luaState, index, "luaNet_class");
                if (udatai != -1)
                    return (int)udatai;
                udatai = luanet_checkudata(luaState, index, "luaNet_searchbase");
                if (udatai != -1)
                    return (int)udatai;
                udatai = luanet_checkudata(luaState, index, "luaNet_function");
                if (udatai != -1)
                    return (int)udatai;
            }
            return -1;
        }
    }
}
