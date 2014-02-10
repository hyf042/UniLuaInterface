using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniLuaInterface;

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
    Lua lua;

    void Awake()
    {
        var time = DateTime.Now;
        lua = new Lua();
        Debug.Log("hello");
        //lua.DoString("Bundle = luanet.import_type \"Test.Bundle\";a = Bundle();a:test(function(s) return s; end)");
        //UniLuaInterface.LuaTable table = (UniLuaInterface.LuaTable)(lua.DoFile("framework/main.lua")[0]);
        //((UniLuaInterface.LuaFunction)table["awake"]).Call();

        lua.DoFile("test.lua");
        lua.CallFunction("Task");
        Debug.Log("use " + (DateTime.Now - time).ToString());
    }

    void OnGUI()
    {
        GUI.Label(new Rect(0, 0, 100, 50), log_text);
        if (GUI.Button(new Rect(0, 100, 100, 50), "Next"))
            lua.CallFunction("Resume");
    }
}

