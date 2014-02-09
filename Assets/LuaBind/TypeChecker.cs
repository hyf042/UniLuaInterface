using System;
using System.Collections.Generic;
using System.Reflection;
using UniLua;

namespace LuaBind
{
    /*
     * Type checking and conversion functions.
     * 
     * Author: Fabio Mascarenhas
     * Version: 1.0
     */
    public class TypeChecker
    {
        private ObjectTranslator translator;
        private ILuaState luaState { get { return translator.luaState; } }

        Dictionary<long, ExtractValue> extractValues = new Dictionary<long, ExtractValue>();

        public TypeChecker(ObjectTranslator translator)
        {
            this.translator = translator;

            //extractValues.Add(typeof(object).TypeHandle.Value.ToInt64(), new ExtractValue(getAsObject));
            extractValues.Add(typeof(sbyte).TypeHandle.Value.ToInt64(), new ExtractValue(getAsSbyte));
            extractValues.Add(typeof(byte).TypeHandle.Value.ToInt64(), new ExtractValue(getAsByte));
            extractValues.Add(typeof(short).TypeHandle.Value.ToInt64(), new ExtractValue(getAsShort));
            extractValues.Add(typeof(ushort).TypeHandle.Value.ToInt64(), new ExtractValue(getAsUshort));
            extractValues.Add(typeof(int).TypeHandle.Value.ToInt64(), new ExtractValue(getAsInt));
            extractValues.Add(typeof(uint).TypeHandle.Value.ToInt64(), new ExtractValue(getAsUint));
            extractValues.Add(typeof(long).TypeHandle.Value.ToInt64(), new ExtractValue(getAsLong));
            extractValues.Add(typeof(ulong).TypeHandle.Value.ToInt64(), new ExtractValue(getAsUlong));
            extractValues.Add(typeof(double).TypeHandle.Value.ToInt64(), new ExtractValue(getAsDouble));
            extractValues.Add(typeof(char).TypeHandle.Value.ToInt64(), new ExtractValue(getAsChar));
            extractValues.Add(typeof(float).TypeHandle.Value.ToInt64(), new ExtractValue(getAsFloat));
            extractValues.Add(typeof(decimal).TypeHandle.Value.ToInt64(), new ExtractValue(getAsDecimal));
            extractValues.Add(typeof(bool).TypeHandle.Value.ToInt64(), new ExtractValue(getAsBoolean));
            extractValues.Add(typeof(string).TypeHandle.Value.ToInt64(), new ExtractValue(getAsString));
            extractValues.Add(typeof(LuaFunction).TypeHandle.Value.ToInt64(), new ExtractValue(getAsFunction));
            //extractValues.Add(typeof(LuaTable).TypeHandle.Value.ToInt64(), new ExtractValue(getAsTable));
            //extractValues.Add(typeof(LuaUserData).TypeHandle.Value.ToInt64(), new ExtractValue(getAsUserdata));

            //extractNetObject = new ExtractValue(getAsNetObject);
        }

        /*
         * Checks if the value at Lua stack index stackPos matches paramType,
         * returning a conversion function if it does and null otherwise.
         */
        internal ExtractValue getExtractor(IReflect paramType)
        {
            return getExtractor(paramType.UnderlyingSystemType);
        }
        internal ExtractValue getExtractor(Type paramType)
        {
            if (paramType.IsByRef) paramType = paramType.GetElementType();

            long runtimeHandleValue = paramType.TypeHandle.Value.ToInt64();

            if (extractValues.ContainsKey(runtimeHandleValue))
                return extractValues[runtimeHandleValue];
            else
                return null;
        }

        internal ExtractValue checkType( int stackPos, Type paramType)
        {
            LuaType luatype = luaState.Type(stackPos);

            if (paramType.IsByRef) paramType = paramType.GetElementType();

            Type underlyingType = Nullable.GetUnderlyingType(paramType);
            if (underlyingType != null)
            {
                paramType = underlyingType;     // Silently convert nullable types to their non null requics
            }

            long runtimeHandleValue = paramType.TypeHandle.Value.ToInt64();

            if (paramType.Equals(typeof(object)))
                return extractValues[runtimeHandleValue];

            //CP: Added support for generic parameters
            if (paramType.IsGenericParameter)
            {
                if (luatype == LuaType.LUA_TBOOLEAN)
                    return extractValues[typeof(bool).TypeHandle.Value.ToInt64()];
                else if (luatype == LuaType.LUA_TSTRING)
                    return extractValues[typeof(string).TypeHandle.Value.ToInt64()];
                else if (luatype == LuaType.LUA_TTABLE)
                    return extractValues[typeof(LuaTable).TypeHandle.Value.ToInt64()];
                else if (luatype == LuaType.LUA_TUSERDATA || luatype == LuaType.LUA_TLIGHTUSERDATA)
                    return extractValues[typeof(object).TypeHandle.Value.ToInt64()];
                else if (luatype == LuaType.LUA_TFUNCTION)
                    return extractValues[typeof(LuaFunction).TypeHandle.Value.ToInt64()];
                else if (luatype == LuaType.LUA_TNUMBER)
                    return extractValues[typeof(double).TypeHandle.Value.ToInt64()];
            }

            if (extractValues.ContainsKey(runtimeHandleValue))
            {
                ExtractValue extractor = extractValues[runtimeHandleValue];
                if (extractor(stackPos) != null)
                    return extractor;
            }
           
            return null;
        }

        /*
         * The following functions return the value in the Lua stack
         * index stackPos as the desired type if it can, or null
         * otherwise.
         */
        private object getAsSbyte( int stackPos)
        {
            sbyte retVal = (sbyte)luaState.ToNumber(stackPos);
            if (retVal == 0 && !(luaState.Type(stackPos)==LuaType.LUA_TNUMBER)) return null;
            return retVal;
        }
        private object getAsByte( int stackPos)
        {
            byte retVal = (byte)luaState.ToNumber( stackPos);
            if (retVal == 0 && !(luaState.Type(stackPos)==LuaType.LUA_TNUMBER)) return null;
            return retVal;
        }
        private object getAsShort( int stackPos)
        {
            short retVal = (short)luaState.ToNumber( stackPos);
            if (retVal == 0 && !(luaState.Type(stackPos)==LuaType.LUA_TNUMBER)) return null;
            return retVal;
        }
        private object getAsUshort( int stackPos)
        {
            ushort retVal = (ushort)luaState.ToNumber( stackPos);
            if (retVal == 0 && !(luaState.Type(stackPos)==LuaType.LUA_TNUMBER)) return null;
            return retVal;
        }
        private object getAsInt( int stackPos)
        {
            int retVal = (int)luaState.ToNumber( stackPos);
            if (retVal == 0 && !(luaState.Type(stackPos)==LuaType.LUA_TNUMBER)) return null;
            return retVal;
        }
        private object getAsUint( int stackPos)
        {
            uint retVal = (uint)luaState.ToNumber( stackPos);
            if (retVal == 0 && !(luaState.Type(stackPos)==LuaType.LUA_TNUMBER)) return null;
            return retVal;
        }
        private object getAsLong( int stackPos)
        {
            long retVal = (long)luaState.ToNumber( stackPos);
            if (retVal == 0 && !(luaState.Type(stackPos)==LuaType.LUA_TNUMBER)) return null;
            return retVal;
        }
        private object getAsUlong( int stackPos)
        {
            ulong retVal = (ulong)luaState.ToNumber( stackPos);
            if (retVal == 0 && !(luaState.Type(stackPos)==LuaType.LUA_TNUMBER)) return null;
            return retVal;
        }
        private object getAsDouble( int stackPos)
        {
            double retVal = luaState.ToNumber( stackPos);
            if (retVal == 0 && !(luaState.Type(stackPos)==LuaType.LUA_TNUMBER)) return null;
            return retVal;
        }
        private object getAsChar( int stackPos)
        {
            char retVal = (char)luaState.ToNumber( stackPos);
            if (retVal == 0 && !(luaState.Type(stackPos)==LuaType.LUA_TNUMBER)) return null;
            return retVal;
        }
        private object getAsFloat( int stackPos)
        {
            float retVal = (float)luaState.ToNumber( stackPos);
            if (retVal == 0 && !(luaState.Type(stackPos)==LuaType.LUA_TNUMBER)) return null;
            return retVal;
        }
        private object getAsDecimal( int stackPos)
        {
            decimal retVal = (decimal)luaState.ToNumber( stackPos);
            if (retVal == 0 && !(luaState.Type(stackPos)==LuaType.LUA_TNUMBER)) return null;
            return retVal;
        }
        private object getAsBoolean( int stackPos)
        {
            bool retVal = luaState.ToBoolean(stackPos);
            if (luaState.Type(stackPos) != LuaType.LUA_TBOOLEAN) return null;
            return retVal;
        }
        private object getAsString( int stackPos)
        {
            string retVal = luaState.ToString(stackPos);
            if (retVal == "" && !luaState.IsString(stackPos)) return null;
            return retVal;
        }
        private object getAsFunction( int stackPos)
        {
            if (luaState.IsFunction(stackPos))
                return translator.getFunction(stackPos);
            return null;
        }
    }
}
