-- Bundle = luanet.import_type "Test.Bundle"
-- Debug = luanet.import_type "UnityEngine.Debug"
-- Math = luanet.import_type "System.Math"
-- String = luanet.import_type "System.String"

-- a = Bundle()
-- a:test(function(s) print(s) end)
-- print(String.Format("sqrt(2) is {0}", Math.Sqrt(2)))

function Task()
	co = coroutine.create(function() 
		for i = 1, 1000 do
			print ("co", i)
			coroutine.yield()
		end
	end)
end

function Resume()
	coroutine.resume(co)
end