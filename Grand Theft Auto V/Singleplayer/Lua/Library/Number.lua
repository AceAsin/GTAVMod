local Keybind = {[65535] = false, [65536] = false, [131072] = false, [262144] = false, [-65536] = false}

for i = 0, 254 do
	Keybind[i] = false
end

function Key_Number_Just_Pressed(Key)
	if get_key_pressed(Key) then
		if Keybind[Key] == false then
			Keybind[Key] = true
			return true
		end
	elseif Keybind[Key] == true then
		Keybind[Key] = false
	end
end

function Key_Number_Just_Released(Key)
	if get_key_pressed(Key) then
		if Keybind[Key] == false then
			Keybind[Key] = true
		end
	elseif Keybind[Key] == true then
		Keybind[Key] = false
		return true
	end
end

function Key_Number_Pressed(Key)
	if get_key_pressed(Key) then
		if Keybind[Key] == false then
			Keybind[Key] = true
		else
			return true
		end
	elseif Keybind[Key] == true then
		Keybind[Key] = false
	end
end

function Key_Number_Released(Key)
	if get_key_pressed(Key) then
		if Keybind[Key] == false then
			Keybind[Key] = true
		end
	elseif Keybind[Key] == true then
		Keybind[Key] = false
	else
		return true
	end
end

local Key = {}

--[[ function Key.init()
end

function Key.tick()
end

function Key.unload()
end ]]

return Key