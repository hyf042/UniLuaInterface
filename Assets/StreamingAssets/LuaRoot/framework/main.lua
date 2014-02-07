local InputControl  = require "logic.input_control"
local SceneMgr      = require "logic.scene_mgr"

local function awake()
    print("---- awake ----")
end

local function start()
    print("---- start ----")
end

local function update()
end

local function late_update()
end

local function fixed_update()
end

return {
    awake           = awake,
    start           = start,
    update          = update,
    late_update     = late_update,
    fixed_update    = fixed_update,
}
