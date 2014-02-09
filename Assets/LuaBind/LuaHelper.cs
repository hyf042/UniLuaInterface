using System;
using UniLua;

namespace LuaBind
{
    public class LuaHelper
    {
        public void NewMetaTable(ILuaState luaState, string name)
        {
            luaState.PushString(name);
            luaState.NewTable();
            luaState.RawSet(LuaDef.LUA_REGISTRYINDEX);
            GetMetaTable(luaState, name);
        }

        public void GetMetaTable(ILuaState luaState, string name)
        {
            luaState.PushString(name);
            luaState.RawGet(LuaDef.LUA_REGISTRYINDEX);
        }
    }
}