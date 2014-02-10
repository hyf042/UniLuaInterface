using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniLua;

namespace Test
{
    public class Bundle
    {
        public delegate string TestDelegate(string s);
        public int a;
        public int Mac { get { return a + 1; } }
        public void test(TestDelegate cb)
        {
            LuaScriptController.log_text = cb("test1"); ;
        }
    }
}

public class LuaScriptController : MonoBehaviour
{
    public static string log_text = "";

    void Awake()
    {
        var time = DateTime.Now;
        var lua = new UniLuaInterface.Lua();
        Debug.Log("hello");
        lua.DoString("Bundle = luanet.import_type \"Test.Bundle\";a = Bundle();a:test(function(s) return s; end)");
        UniLuaInterface.LuaTable table = (UniLuaInterface.LuaTable)(lua.DoFile("framework/main.lua")[0]);
        ((UniLuaInterface.LuaFunction)table["awake"]).Call();
        Debug.Log("use " + (DateTime.Now - time).ToString());
    }

    void OnGUI()
    {
        GUI.Label(new Rect(0, 0, 100, 50), log_text);
    }
}

