------------------------
-- File: Mobile Radio --
-- Version:	1.0		  --
-- Author:	Ace Asin  --
------------------------

-- GitHub: https://github.com/AceAsin

-- Discord: https://discord.gg/U8vHS7y

--------------
-- Settings --
--------------

Radio_Mobile = true -- If you want to enable mobile radio by default. It's not compatible with ELS vehicles.

Active_Modifier = true -- If you want to require the active modifier keybind to be held pressed, in order for the rest of the keybinds to function.

--------------
-- Keybinds --
--------------

Keybind_Shift = 160 -- The active modifier that needs to be held pressed down in order to use the toggle keybind. The default keybind is set to 160 / LShiftKey.

Keybind_Toggle = 120 -- The keybind to toggle the police radio. The defualt keybind is set to 120 / F9.

--------------
--- Script ---
--------------

local Script = {}

function Shift()
	if (Key_Number_Just_Pressed(Keybind_Toggle) or Key_String_Just_Pressed(Keybind_Toggle)) then
		if (Radio_Mobile) then
			Radio_Mobile = false
		else
			Radio_Mobile = true
		end
	end
end

function Script.tick()
	local Identification = PLAYER.PLAYER_PED_ID()
	local Foot = PED.IS_PED_ON_FOOT(Identification)

	if (not ENTITY.DOES_ENTITY_EXIST(Identification)) then return end

	if (Active_Modifier) then
		if (Key_Number_Pressed(Keybind_Shift) or Key_String_Pressed(Keybind_Shift)) then
			Shift()
		end
	else
		Shift()
	end

	if (Foot) then
		if (Radio_Mobile) then
			AUDIO.SET_MOBILE_RADIO_ENABLED_DURING_GAMEPLAY(true)
		else
			AUDIO.SET_MOBILE_RADIO_ENABLED_DURING_GAMEPLAY(false)
		end
	else
		AUDIO.SET_MOBILE_RADIO_ENABLED_DURING_GAMEPLAY(false)
	end
end

return Script