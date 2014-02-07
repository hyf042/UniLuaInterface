luanet.load_assembly("myTest")
bundle_type = luanet.import_type("myTest.Bundle");
obj = {a = 1}
print (bundle_type)
luanet.make_object(obj, bundle_type);
--obj:test();