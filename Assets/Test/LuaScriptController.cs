using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniLua;

//class LibFoo
//{
//    public const string LIB_NAME = "libfoo"; // 库的名称, 可以是任意字符串
//    int abc;
//    public LibFoo(int v = 0)
//    {
//        abc = v;
//    }

//    public int OpenLib(ILuaState lua) // 库的初始化函数
//    {
//        var define = new NameFuncPair[]
//        {
//            new NameFuncPair("add", Add),
//            new NameFuncPair("sub", Sub),
//        };

//        lua.L_NewLib(define);
//        return 1;
//    }

//    public int Add(ILuaState lua)
//    {
//        var a = lua.L_CheckNumber(1); // 第一个参数
//        var b = lua.L_CheckNumber(2); // 第二个参数
//        var c = a + b + abc; // 执行加法操作
//        lua.PushNumber(c); // 将返回值入栈
//        return 1; // 有一个返回值
//    }

//    public int Sub(ILuaState lua)
//    {
//        var a = lua.L_CheckNumber(1); // 第一个参数
//        var b = lua.L_CheckNumber(2); // 第二个参数
//        var c = a - b; // 执行减法操作
//        lua.PushNumber(c); // 将返回值入栈
//        return 1; // 有一个返回值
//    }
//}


//public class LuaScriptController : MonoBehaviour {
//    public	string		LuaScriptFile = "framework/main.lua";

//    private ILuaState 	Lua;
//    private int			AwakeRef;
//    private int			StartRef;
//    private int			UpdateRef;
//    private int			LateUpdateRef;
//    private int			FixedUpdateRef;

//    void Awake() {
//        LuaBind.LuaVM luaVM = new LuaBind.LuaVM();
//        luaVM.DumpStack();

//        Debug.Log("LuaScriptController Awake");

//        if( Lua == null )
//        {
//            Lua = LuaAPI.NewState();
//            Lua.L_OpenLibs();
//            //LibFoo foo = new LibFoo(1);
//            //Lua.L_RequireF("lib1"
//            //    , foo.OpenLib
//            //    , true);
//            //LibFoo foo2 = new LibFoo(2);
//            //Lua.L_RequireF("lib2"
//            //    , foo2.OpenLib
//            //    , true);

//            var status = Lua.L_DoFile( LuaScriptFile );
//            if( status != ThreadStatus.LUA_OK )
//            {
//                throw new Exception( Lua.ToString(-1) );
//            }

//            if( ! Lua.IsTable(-1) )
//            {
//                throw new Exception(
//                    "framework main's return value is not a table" );
//            }

//            AwakeRef 		= StoreMethod( "awake" );
//            StartRef 		= StoreMethod( "start" );
//            UpdateRef 		= StoreMethod( "update" );
//            LateUpdateRef 	= StoreMethod( "late_update" );
//            FixedUpdateRef 	= StoreMethod( "fixed_update" );

//            Lua.Pop(1);
//            Debug.Log("Lua Init Done");
//        }

//        CallMethod( AwakeRef );
//    }

//    IEnumerator Start() {
//        CallMethod( StartRef );

//        // -- sample code for loading binary Asset Bundles --------------------
//        String s = "file:///"+Application.streamingAssetsPath+"/testx.unity3d";
//        WWW www = new WWW(s);
//        yield return www;
//        if(www.assetBundle.mainAsset != null) {
//            TextAsset cc = (TextAsset)www.assetBundle.mainAsset;

//            var status = Lua.L_LoadBytes(cc.bytes, "test");
//            if( status != ThreadStatus.LUA_OK )
//            {
//                throw new Exception( Lua.ToString(-1) );
//            }
//            status = Lua.PCall( 0, 0, 0);
//            if( status != ThreadStatus.LUA_OK )
//            {
//                throw new Exception( Lua.ToString(-1) );
//            }
//            Debug.Log("---- call done ----");
//        }
//    }

//    void Update() {
//        CallMethod( UpdateRef );
//    }

//    void LateUpdate() {
//        CallMethod( LateUpdateRef );
//    }

//    void FixedUpdate() {
//        CallMethod( FixedUpdateRef );
//    }

//    private int StoreMethod( string name )
//    {
//        Lua.GetField( -1, name );
//        if( !Lua.IsFunction( -1 ) )
//        {
//            throw new Exception( string.Format(
//                "method {0} not found!", name ) );
//        }
//        return Lua.L_Ref( LuaDef.LUA_REGISTRYINDEX );
//    }

//    private void CallMethod( int funcRef )
//    {
//        Lua.RawGetI( LuaDef.LUA_REGISTRYINDEX, funcRef );

//        // insert `traceback' function
//        var b = Lua.GetTop();
//        Lua.PushCSharpFunction( Traceback );
//        Lua.Insert(b);

//        var status = Lua.PCall( 0, 0, b );
//        if( status != ThreadStatus.LUA_OK )
//        {
//            Debug.LogError( Lua.ToString(-1) );
//        }

//        // remove `traceback' function
//        Lua.Remove(b);
//    }

//    private static int Traceback(ILuaState lua) {
//        var msg = lua.ToString(1);
//        if(msg != null) {
//            lua.L_Traceback(lua, msg, 1);
//        }
//        // is there an error object?
//        else if(!lua.IsNoneOrNil(1)) {
//            // try its `tostring' metamethod
//            if(!lua.L_CallMeta(1, "__tostring")) {
//                lua.PushString("(no error message)");
//            }
//        }
//        return 1;
//    }
//}

namespace Test
{
    public class Bundle
    {
        public delegate void TestDelegate(string s);
        public int a;
        public int Mac { get { return a + 1; } }
        public void test(TestDelegate cb)
        {
            cb("test1");
        }
    }
}

public class LuaScriptController : MonoBehaviour
{
    private LuaBind.Lua lua;

    void Awake()
    {
        /*lua = new LuaBind.Lua();

        //lua["test"] = (CSharpFunctionDelegate)((v) => { Debug.Log("test"); return 0; });
        //lua.CallFunction("test");
        lua.RegisterFunctions(this);
        object[] ret = lua.DoString("test(1,\"hehe\")");
        int length = ret == null ? 0 : ret.Length;
        Debug.Log(length);*/
        var time = DateTime.Now;
        var lua = new LuaInterface.Lua();
        lua.DoFile("test.lua");
        Debug.Log("use " + (DateTime.Now - time).ToString());
    }

    [LuaBind.LuaGlobal("test")]
    static int test<T1,T2>(T1 a, T2 b)
    {
        Debug.Log("世界真奇妙！");
        Debug.Log(a);
        Debug.Log(b);
        return 5;
    }
}

