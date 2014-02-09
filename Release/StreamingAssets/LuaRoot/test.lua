Bundle = luanet.import_type "Test.Bundle"
Debug = luanet.import_type "UnityEngine.Debug"
Math = luanet.import_type "System.Math"
String = luanet.import_type "System.String"

a = Bundle()
a:test(function(s) print(s) end)
print(String.Format("sqrt(2) is {0}", Math.Sqrt(2)))