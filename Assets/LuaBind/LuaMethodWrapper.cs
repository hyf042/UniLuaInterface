namespace LuaBind
{
    using System;
    using System.IO;
    using System.Collections;
    using System.Reflection;
    using System.Collections.Generic;
    using System.Diagnostics;
    using UniLua;

    /*
     * Cached method
     */
    struct MethodCache
    {
        private MethodBase _cachedMethod;

        public MethodBase cachedMethod
        {
            get
            {
                return _cachedMethod;
            }
            set
            {
                _cachedMethod = value;
                MethodInfo mi = value as MethodInfo;
                if (mi != null)
                {
                    IsReturnVoid = string.Compare(mi.ReturnType.Name, "System.Void", true) == 0;
                }
            }
        }

        public bool IsReturnVoid;

        // List or arguments
        public object[] args;
        // Positions of out parameters
        public int[] outList;
        // Types of parameters
        //public MethodArgs[] argTypes;
    }

    /*
     * Argument extraction with type-conversion function
     */
    delegate object ExtractValue(int stackPos);

    /*
     * Wrapper class for methods/constructors accessed from Lua.
     * 
     * Author: Fabio Mascarenhas
     * Version: 1.0
     */
    class LuaMethodWrapper
    {
        private ObjectTranslator _Translator;
        private MethodBase _Method;
        private MethodCache _LastCalledMethod = new MethodCache();
        private string _MethodName;
        //private MemberInfo[] _Members;
        private IReflect _TargetType;
        //private ExtractValue _ExtractTarget;
        private object _Target;
        private BindingFlags _BindingType;

        private ILuaState luaState { get { return _Translator.luaState; } }

        /*
         * Constructs the wrapper for a known MethodBase instance
         */
        public LuaMethodWrapper(ObjectTranslator translator, object target, MethodBase method)
        {
            IReflect targetType = method.DeclaringType;
            _Translator = translator;
            _Target = target;
            _TargetType = targetType;
            //if (targetType != null)
            //    _ExtractTarget = translator.typeChecker.getExtractor(targetType);
            _Method = method;
            _MethodName = method.Name;

            if (method.IsStatic)
            { _BindingType = BindingFlags.Static; }
            else
            { _BindingType = BindingFlags.Instance; }
        }

        /*
         * Calls the method. Receives the arguments from the Lua stack
         * and returns values in it.
         */
        public int call(ILuaState luaState)
        {
            MethodBase methodToCall = _Method;
            object targetObject = _Target;
            bool failedCall = true;
            int nReturnValues = 0;

            if (!luaState.CheckStack(5))
                throw new Exception("Lua stack overflow");

            bool isStatic = (_BindingType & BindingFlags.Static) == BindingFlags.Static;

            if (methodToCall != null) // Method from MethodBase instance 
            {
                if (methodToCall.ContainsGenericParameters)
                {
                    bool isMethod = matchParameters(methodToCall, ref _LastCalledMethod);

                    if (methodToCall.IsGenericMethodDefinition)
                    {
                        //need to make a concrete type of the generic method definition
                        Type[] typeArgs = new Type[methodToCall.GetGenericArguments().Length];

                        //get each generic type's binding type (if multiply vars belong to same T, pick first one's Type)
                        int index = 0;
                        foreach (var paramInfo in methodToCall.GetParameters())
                        {
                            if (paramInfo.ParameterType.IsGenericParameter)
                            {
                                int pos = paramInfo.ParameterType.GenericParameterPosition;
                                if (typeArgs[pos] == null)
                                    typeArgs[pos] = _LastCalledMethod.args[index].GetType();
                            }
                            index++;
                            if (index > _LastCalledMethod.args.Length)
                                break;
                        }

                        MethodInfo concreteMethod = (methodToCall as MethodInfo).MakeGenericMethod(typeArgs);

                        _Translator.push(concreteMethod.Invoke(targetObject, _LastCalledMethod.args));
                        failedCall = false;
                    }
                    else if (methodToCall.ContainsGenericParameters)
                    {
                        _Translator.throwErrorInLua("unable to invoke method on generic class as the current method is an open generic method");
                        return 1;
                    }
                }
                else
                {
                    if (!methodToCall.IsStatic && !methodToCall.IsConstructor && targetObject == null)
                    {
                        //targetObject = _ExtractTarget(1);
                        //luaState.Remove(1); // Pops the receiver
                        //!FIXME: extract target object and invoke function
                    }

                    if (!matchParameters(methodToCall, ref _LastCalledMethod))
                    {
                        _Translator.throwErrorInLua("invalid arguments to method call");
                        return 1;
                    }
                }
            }

            if (failedCall)
            {
                if (!luaState.CheckStack(_LastCalledMethod.outList.Length + 6))
                    throw new Exception("Lua stack overflow");
                try
                {
                    if (isStatic)
                    {
                        _Translator.push(_LastCalledMethod.cachedMethod.Invoke(null, _LastCalledMethod.args));
                    }
                    else
                    {
                        if (_LastCalledMethod.cachedMethod.IsConstructor)
                            _Translator.push(((ConstructorInfo)_LastCalledMethod.cachedMethod).Invoke(_LastCalledMethod.args));
                        else
                            _Translator.push(_LastCalledMethod.cachedMethod.Invoke(targetObject, _LastCalledMethod.args));
                    }
                }
                catch (TargetInvocationException e)
                {
                    return _Translator.throwErrorInLua(e.GetBaseException());
                }
                catch (Exception e)
                {
                    return _Translator.throwErrorInLua(e);
                }
            }

            // Pushes out and ref return values
            for (int index = 0; index < _LastCalledMethod.outList.Length; index++)
            {
                nReturnValues++;
                //for(int i=0;i<lastCalledMethod.outList.Length;i++)
                _Translator.push(_LastCalledMethod.args[_LastCalledMethod.outList[index]]);
            }

            //by isSingle 2010-09-10 11:26:31 
            //Desc: 
            //  if not return void,we need add 1,
            //  or we will lost the function's return value 
            //  when call dotnet function like "int foo(arg1,out arg2,out arg3)" in lua code 
            if (!_LastCalledMethod.IsReturnVoid && nReturnValues > 0)
            {
                nReturnValues++;
            }

            return nReturnValues < 1 ? 1 : nReturnValues;
        }

        private bool matchParameters(MethodBase method, ref MethodCache methodCache)
        {
            ExtractValue extractValue = null;
            bool isParamMatched = true;
            
            int currentLuaParam = 1;
            int nLuaParams = luaState.GetTop();
            ArrayList paramList = new ArrayList();
            List<int> outList = new List<int>();
            //List<MethodArgs> argTypes = new List<MethodArgs>();

            ParameterInfo[] paramInfo = method.GetParameters();
            foreach (ParameterInfo currentNetParam in paramInfo)
            {
                if (!currentNetParam.IsIn && currentNetParam.IsOut)  // Skips out params
                {
                    outList.Add(paramList.Add(null));
                }
                else if (currentLuaParam > nLuaParams) // Adds optional parameters
                {
                    if (currentNetParam.IsOptional)
                    {
                        paramList.Add(currentNetParam.DefaultValue);
                    }
                    else
                    {
                        isParamMatched = false;
                        break;
                    }
                }
                else if (isTypeCorrect(currentLuaParam, currentNetParam, out extractValue))  // Type checking
                {
                    int index = paramList.Add(extractValue(currentLuaParam));

                    //MethodArgs methodArg = new MethodArgs();
                    //methodArg.index = index;
                    //argTypes.Add(methodArg);

                    if (currentNetParam.ParameterType.IsByRef)
                        outList.Add(index);
                    currentLuaParam++;
                }
                else if (currentNetParam.IsOptional)
                {
                    paramList.Add(currentNetParam.DefaultValue);
                }
                else  // No match
                {
                    isParamMatched = false;
                    break;
                }
            }
            if (currentLuaParam != nLuaParams + 1) // Number of parameters does not match
                isParamMatched = false;
            if (isParamMatched)
            {
                methodCache.args = paramList.ToArray();
                methodCache.cachedMethod = method;
                methodCache.outList = outList.ToArray();
                //methodCache.argTypes = argTypes.ToArray();
            }
            return isParamMatched;
        }
        private bool isTypeCorrect(int currentLuaParam, ParameterInfo currentNetParam, out ExtractValue extractValue)
        {
            try
            {
                extractValue = _Translator.typeChecker.checkType(currentLuaParam, currentNetParam.ParameterType);
                return extractValue != null;
            }
            catch
            {
                extractValue = null;
                return false;
            }
        }
    }

}
