------------------------
-- File: Police Radio --
-- Version:	1.0		  --
-- Author: Ace Asin   --
------------------------

-- GitHub: https://github.com/AceAsin

-- Discord: https://discord.gg/U8vHS7y

--------------
-- Settings --
--------------

Radio_Police = true -- If you want to enable police radio by defaault. It's not compatible with ELS vehicles.

Active_Modifier = true -- If you want to require the active modifier keybind to be held pressed, in order for the rest of the keybinds to function.

--------------
-- Keybinds --
--------------

Keybind_Shift = 160 -- The active modifier that needs to be held pressed down in order to use the toggle keybind. The default keybind is set to 160 / LShiftKey.

Keybind_Toggle = 121 -- The keybind to toggle the police radio. The defualt keybind is set to 121 / F10.

--------------
--- Script ---
--------------

local Script = {}

function Shift()
	if (Key_Number_Just_Pressed(Keybind_Toggle) or Key_String_Just_Pressed(Keybind_Toggle)) then
		if (Radio_Police) then
			Radio_Police = false
		else
			Radio_Police = true
		end
	end
end

function Script.tick()
	local Identification = PLAYER.PLAYER_PED_ID()
	local Police = PED.IS_PED_IN_ANY_POLICE_VEHICLE(Identification)

	if (not ENTITY.DOES_ENTITY_EXIST(Identification)) then return end

	if (Active_Modifier) then
		if (Key_Number_Pressed(Keybind_Shift) or Key_String_Pressed(Keybind_Shift)) then
			Shift()
		end
	else
		Shift()
	end

	if (Police) then
		if (Radio_Police) then
			AUDIO.SET_MOBILE_RADIO_ENABLED_DURING_GAMEPLAY(true)
		else
			AUDIO.SET_MOBILE_RADIO_ENABLED_DURING_GAMEPLAY(false)
		end
	else
		AUDIO.SET_MOBILE_RADIO_ENABLED_DURING_GAMEPLAY(false)
	end
end

return Script