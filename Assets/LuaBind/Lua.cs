/*
 * Time         : 2/5/2014
 * Author       : hyf042
 * 
 * Copyright (c) 2013 GameMaster
 */
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UniLua;

namespace LuaBind
{
    public class Lua
    {
        public delegate void LuaCallbackDelegate(Lua lua);
        ObjectTranslator translator;
        
        public Lua()
        {
            luaState = LuaAPI.NewState();
            luaState.L_OpenLibs();

            translator = new ObjectTranslator(this);
            executing = false;
        }

        #region Globals auto-complete
        private readonly List<string> globals = new List<string>();
        private bool globalsSorted;

        /// <summary>
        /// An alphabetically sorted list of all globals (objects, methods, etc.) externally added to this Lua instance
        /// </summary>
        /// <remarks>Members of globals are also listed. The formatting is optimized for text input auto-completion.</remarks>
        public IEnumerable<string> Globals
        {
            get
            {
                // Only sort list when necessary
                if (!globalsSorted)
                {
                    globals.Sort();
                    globalsSorted = true;
                }

                return globals;
            }
        }

        /// <summary>
        /// Adds an entry to <see cref="globals"/> (recursivley handles 2 levels of members)
        /// </summary>
        /// <param name="path">The index accessor path ot the entry</param>
        /// <param name="type">The type of the entry</param>
        /// <param name="recursionCounter">How deep have we gone with recursion?</param>
        private void registerGlobal(string path, Type type, int recursionCounter)
        {
            // If the type is a global method, list it directly
            if (type == typeof(CSharpFunctionDelegate))
            {
                // Format for easy method invocation
                globals.Add(path + "(");
            }
            // If the type is a class or an interface and recursion hasn't been running too long, list the members
            else if ((type.IsClass || type.IsInterface) && type != typeof(string) && recursionCounter < 2)
            {
                #region Methods
                foreach (MethodInfo method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance))
                {
                    if (
                        // Check that the LuaHideAttribute and LuaGlobalAttribute were not applied
                        (method.GetCustomAttributes(typeof(LuaHideAttribute), false).Length == 0) &&
                        (method.GetCustomAttributes(typeof(LuaGlobalAttribute), false).Length == 0) &&
                        // Exclude some generic .NET methods that wouldn't be very usefull in Lua
                        method.Name != "GetType" && method.Name != "GetHashCode" && method.Name != "Equals" &&
                        method.Name != "ToString" && method.Name != "Clone" && method.Name != "Dispose" &&
                        method.Name != "GetEnumerator" && method.Name != "CopyTo" &&
                        !method.Name.StartsWith("get_", StringComparison.Ordinal) &&
                        !method.Name.StartsWith("set_", StringComparison.Ordinal) &&
                        !method.Name.StartsWith("add_", StringComparison.Ordinal) &&
                        !method.Name.StartsWith("remove_", StringComparison.Ordinal))
                    {
                        // Format for easy method invocation
                        string command = path + ":" + method.Name + "(";
                        if (method.GetParameters().Length == 0) command += ")";
                        globals.Add(command);
                    }
                }
                #endregion

                #region Fields
                foreach (FieldInfo field in type.GetFields(BindingFlags.Public | BindingFlags.Instance))
                {
                    if (
                        // Check that the LuaHideAttribute and LuaGlobalAttribute were not applied
                        (field.GetCustomAttributes(typeof(LuaHideAttribute), false).Length == 0) &&
                        (field.GetCustomAttributes(typeof(LuaGlobalAttribute), false).Length == 0))
                    {
                        // Go into recursion for members
                        registerGlobal(path + "." + field.Name, field.FieldType, recursionCounter + 1);
                    }
                }
                #endregion

                #region Properties
                foreach (PropertyInfo property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    if (
                        // Check that the LuaHideAttribute and LuaGlobalAttribute were not applied
                        (property.GetCustomAttributes(typeof(LuaHideAttribute), false).Length == 0) &&
                        (property.GetCustomAttributes(typeof(LuaGlobalAttribute), false).Length == 0)
                        // Exclude some generic .NET properties that wouldn't be very usefull in Lua
                        && property.Name != "Item")
                    {
                        // Go into recursion for members
                        registerGlobal(path + "." + property.Name, property.PropertyType, recursionCounter + 1);
                    }
                }
                #endregion
            }
            // Otherwise simply add the element to the list
            else globals.Add(path);

            // List will need to be sorted on next access
            globalsSorted = false;
        }
        #endregion

        #region Properties
        public ILuaState luaState { get; private set; }
        public bool executing { get; private set; }
        #endregion

        #region Interface
        /// <summary>
        /// Excutes a Lua file and returns all the chunk's return
		/// values in an array
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public object[] DoFile(string filename)
        {
            int oldTop = luaState.GetTop();
            if (luaState.L_LoadFile(filename) == ThreadStatus.LUA_OK)
            {
                executing = true;
                try
                {
                    if (luaState.PCall(0, -1, 0) == 0)
                        return translator.popValues(oldTop);
                    else
                        ThrowException(oldTop);
                }
                finally
                { executing = false; }
            }
            else
                ThrowException(oldTop);

            return null;
        }
        /// <summary>
        /// Executes a Lua chnk and returns all the chunk's return values in an array.
        /// chunkName is just for debug and error output
        /// </summary>
        /// <param name="chunk">Chunk to execute</param>
        /// <param name="chunkName">Name to associate with the chunk</param>
        /// <returns></returns>
        public object[] DoString(string chunk, string chunkName = "chunk")
        {
            int oldTop = luaState.GetTop();
            if (luaState.L_LoadBuffer(chunk, chunkName) == ThreadStatus.LUA_OK)
            {
                executing = true;
                try
                {
                    if (luaState.PCall(0, -1, 0) == 0)
                        return translator.popValues(oldTop);
                    else
                        ThrowException(oldTop);
                }
                finally
                { executing = false; }
            }
            else
                ThrowException(oldTop);

            return null;
        }

        public LuaFunction LoadString(string chunk, string chunkName = "chunk")
        {
            int oldTop = luaState.GetTop();

            executing = true;
            try
            {
                if (luaState.L_LoadBuffer(chunk, chunkName) != 0)
                    ThrowException(oldTop);
            }
            finally
            { executing = false; }

            LuaFunction result = translator.getFunction(-1);
            translator.popValues(oldTop);

            return result;
        }
        public LuaFunction LoadFile(string filename)
        {
            int oldTop = luaState.GetTop();

            executing = true;
            try
            {
                if (luaState.L_LoadFile(filename) != 0)
                    ThrowException(oldTop);
            }
            finally
            { executing = false; }

            LuaFunction result = translator.getFunction(-1);
            translator.popValues(oldTop);

            return result;
        }
        #endregion

        #region Access Variables
        /*
		 * Indexer for global variables from the LuaInterpreter
		 * Supports navigation of tables by using . operator
		 */
        public object this[string fullPath]
        {
            get
            {
                object returnValue = null;
                int oldTop = luaState.GetTop();
                string[] path = fullPath.Split(new char[] { '.' });
                luaState.GetGlobal(path[0]);
                returnValue = translator.getObject(-1);
                if (path.Length > 1)
                {
                    string[] remainingPath = new string[path.Length - 1];
                    Array.Copy(path, 1, remainingPath, 0, path.Length - 1);
                    returnValue = getObject(remainingPath);
                }
                luaState.SetTop(oldTop);
                return returnValue;
            }
            set
            {
                int oldTop = luaState.GetTop();
                string[] path = fullPath.Split(new char[] { '.' });
                if (path.Length == 1)
                {
                    translator.push(value);
                    luaState.SetGlobal(fullPath);
                }
                else
                {
                    luaState.GetGlobal(path[0]);
                    string[] remainingPath = new string[path.Length - 1];
                    Array.Copy(path, 1, remainingPath, 0, path.Length - 1);
                    setObject(remainingPath, value);
                }
                luaState.SetTop(oldTop);

                // Globals auto-complete
                if (value == null)
                {
                    // Remove now obsolete entries
                    globals.Remove(fullPath);
                }
                else
                {
                    // Add new entries
                    if (!globals.Contains(fullPath))
                        registerGlobal(fullPath, value.GetType(), 0);
                }
            }
        }
        
        public double GetNumber(string fullpath)
        {
            return (double)this[fullpath];
        }
        public bool GetBoolean(string fullpath)
        {
            return (bool)this[fullpath];
        }
        public string GetString(string fullpath)
        {
            return (string)this[fullpath];
        }
        
        
        public void SetVar<T>(string fullpath, T val)
        {
            this[fullpath] = val;
        }

        

        internal void dispose(int reference)
        {
            if (luaState != null) //Fix submitted by Qingrui Li
                luaState.L_Unref(LuaDef.LUA_REGISTRYINDEX, reference);
        }
        internal bool compareRef(int ref1, int ref2)
        {
            int top = luaState.GetTop();
            luaState.RawGetI(LuaDef.LUA_REGISTRYINDEX, ref1);
            luaState.RawGetI(LuaDef.LUA_REGISTRYINDEX, ref2);
            bool equal = luaState.RawEqual(-1, -2);
            luaState.SetTop(top);
            return equal;
        }

        internal object rawGetObject(int reference, string field)
        {
            int oldTop = luaState.GetTop();
            luaState.RawGetI(LuaDef.LUA_REGISTRYINDEX, reference);
            luaState.PushString(field);
            luaState.RawGet(-2);
            object obj = translator.getObject(-1);
            luaState.SetTop(oldTop);
            return obj;
        }
        internal object getObject(string[] remainingPath)
        {
            object returnValue = null;
            for (int i = 0; i < remainingPath.Length; i++)
            {
                luaState.PushString(remainingPath[i]);
                luaState.GetTable(-2);
                returnValue = translator.getObject(-1);
                if (returnValue == null) break;
            }
            return returnValue;
        }
        internal void setObject(string[] remainingPath, object val)
        {
            for (int i = 0; i < remainingPath.Length - 1; i++)
            {
                luaState.PushString(remainingPath[i]);
                luaState.GetTable(-2);
            }
            luaState.PushString(remainingPath[remainingPath.Length - 1]);
            translator.push(val);
            luaState.SetTable(-3);
        }
        internal object getObject(int reference, string field)
        {
            int oldTop = luaState.GetTop();
            luaState.RawGetI(LuaDef.LUA_REGISTRYINDEX, reference);
            object returnValue = getObject(field.Split(new char[] { '.' }));
            luaState.SetTop(oldTop);
            return returnValue;
        }
        internal void setObject(int reference, string field, object val)
        {
            int oldTop = luaState.GetTop();
            luaState.RawGetI(LuaDef.LUA_REGISTRYINDEX, reference);
            setObject(field.Split(new char[] { '.' }), val);
            luaState.SetTop(oldTop);
        }
        #endregion

        #region Access Function
        public LuaFunction GetFunction(string fullpath)
        {
            return (LuaFunction)this[fullpath];
        }
        public object[] Invoke(string fullpath, params object[] args)
        {
            return GetFunction(fullpath).Call(args);
        }
        /*
		 * Registers an object's method as a Lua function (global or table field)
		 * The method may have any signature
		 */
        public LuaFunction RegisterFunction(string path, object target, string name)
        {
            return RegisterFunction(path, target, target.GetType().GetMethod(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance));
        }
        public LuaFunction RegisterFunction(string path, Type targetType, string name)
        {
            return RegisterFunction(path, null, targetType.GetMethod(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static));
        }
        public LuaFunction RegisterFunction(string path, object target, MethodBase function)
        {
            int oldTop = luaState.GetTop();

            LuaMethodWrapper wrapper = new LuaMethodWrapper(translator, target, function);
            translator.push(new CSharpFunctionDelegate(wrapper.call));

            this[path] = translator.getObject(-1);
            LuaFunction func = GetFunction(path);

            luaState.SetTop(oldTop);

            return func;
        }
        #endregion

        #region Internal
        void ThrowException(int oldTop)
        {
            object err = translator.getObject(-1);
            luaState.SetTop(oldTop);

            if (err == null) err = "Unknown Lua Error";
            throw new Exception( err.ToString() );
        }
        public string DumpStack()
        {
            StringBuilder builder = new StringBuilder();
            int top = luaState.GetTop();

            for (int i = 1; i <= top; i++)
            {
                var t = luaState.Type(i);
                switch (t)
                {
                    case LuaType.LUA_TSTRING:
                        builder.Append(luaState.ToString(i));
                        break;
                    case LuaType.LUA_TNUMBER:
                        builder.Append(luaState.ToNumber(i));
                        break;
                    case LuaType.LUA_TBOOLEAN:
                        builder.Append(luaState.ToBoolean(i));
                        break;
                }
                builder.Append("\t");
            }

            return builder.ToString();
        }

        internal void pushCSFunction(CSharpFunctionDelegate function)
        {
            translator.pushFunction(function);
        }
        /*
		 * Calls the object as a function with the provided arguments,
		 * returning the function's returned values inside an array
		 */
		internal object[] callFunction(object function,object[] args) 
		{
            int nArgs = 0;
            int oldTop = luaState.GetTop();
            if (!luaState.CheckStack(args.Length + 6))
                throw new Exception("Lua stack overflow");

            translator.push(function);
            if (args != null)
            {
                nArgs = args.Length;
                for (int i = 0; i < args.Length; i++)
                {
                    translator.push(args[i]);
                }
            }
            executing = true;
            try
            {
                if (luaState.PCall(nArgs, -1, 0) != 0)
                    ThrowException(oldTop);
            }
            finally { executing = false; }

            return translator.popValues(oldTop);
		}
        #endregion

        #region Helper
        public void RegisterFunctions(Object pTarget)
        {
            // Get the target type
            Type pTrgType = pTarget.GetType();

            // ... and simply iterate through all it's methods
            foreach (MethodInfo mInfo in pTrgType.GetMethods(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                // ... then through all this method's attributes
                foreach (Attribute attr in Attribute.GetCustomAttributes(mInfo))
                {
                    // and if they happen to be one of our AttrLuaFunc attributes
                    if (attr.GetType() == typeof(LuaGlobalAttribute))
                    {
                        LuaGlobalAttribute pAttr = (LuaGlobalAttribute)attr;

                        // And tell the VM to register it.
                        RegisterFunction(pAttr.Name, pTarget, mInfo);
                    }
                }
            }
        }
        #endregion
    }
}
