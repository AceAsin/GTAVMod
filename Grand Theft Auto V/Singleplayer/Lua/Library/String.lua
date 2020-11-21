local Keybind = {[65535] = false, [65536] = false, [131072] = false, [262144] = false, [-65536] = false}

for i = 0, 254 do
	Keybind[i] = false
end

function Key_String_Just_Pressed(Key)
	if get_key_pressed(Keys[Key]) then
		if Keybind[Keys[Key]] == false then
			Keybind[Keys[Key]] = true
			return true
		end
	elseif Keybind[Keys[Key]] == true then
		Keybind[Keys[Key]] = false
	end
end

function Key_String_Just_Released(Key)
	if get_key_pressed(Keys[Key]) then
		if Keybind[Keys[Key]] == false then
			Keybind[Keys[Key]] = true
		end
	elseif Keybind[Keys[Key]] == true then
		Keybind[Keys[Key]] = false
		return true
	end
end

function Key_String_Pressed(Key)
	if get_key_pressed(Keys[Key]) then
		if Keybind[Keys[Key]] == false then
			Keybind[Keys[Key]] = true
		else
			return true
		end
	elseif Keybind[Keys[Key]] == true then
		Keybind[Keys[Key]] = false
	end
end

function Key_String_Released(Key)
	if get_key_pressed(Keys[Key]) then
		if Keybind[Keys[Key]] == false then
			Keybind[Keys[Key]] = true
		end
	elseif Keybind[Keys[Key]] == true then
		Keybind[Keys[Key]] = false
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