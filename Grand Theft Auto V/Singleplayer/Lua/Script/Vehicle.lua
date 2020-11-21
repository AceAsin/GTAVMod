-----------------------
-- File: Vehicle     --
-- Version:	1.0		 --
-- Author:	Ace Asin --
-----------------------

-- GitHub: https://github.com/AceAsin

-- Discord: https://discord.gg/U8vHS7y

--------------
-- Settings --
--------------

Radio_Favorite = 19 -- The index number or name string, references below.
Radio_Self = 20 -- The index number or name string, references below.
Radio_Off = 255 -- The index number or name string, references below.

Radio_Default = Radio_Off -- The default radio station for when you enter a vehicle. If you use the keybinds, then the default radio station is overwritten for normals vehicles. Police vehicles are not overwritten and will always use the default radio station that was set.

Radio_Force = false -- Whether to force the default radio station, when you first get into a vehicle.
Radio_Loud = true -- If you want to set the car radio to loud, when entering a vehicle.
Radio_Mobile = false -- If you want to enable mobile radio by default.
Radio_Police = true -- If you want to enable police radio by defaault. It's not compatible with ELS vehicles, so use the keybinds to change the radio station.

Vehicle_Interval = 1000 -- Time in milliseconds to hold down the F key, in order to keep the vehicle engine running and not close the door. I don't recommend to change this by much, since it needs to be in a certain timespan.

Vehicle_Engine = true -- If you want to keep the vehicle engine running, when you exit out of the vehicle, as well as the radio and lights, hold down F.
Vehicle_Door = true -- If you want to keep the vehicle door open, right after you exit a vehicle, hold down F.

Active_Modifier = true -- If you want to require the active modifier keybind to be held pressed, in order for the rest of the keybinds to function.

--------------
-- Keybinds --
--------------

Keybind_Control = 162 -- The active modifier that needs to be held pressed down in order to use the normal keybinds. The default keybind is set to 162 / LControlKey.

Keybind_Favorite = 120 -- The keybind to change to your favorite radio station. The defualt keybind is set to 120 / F9.
Keybind_Self = 121 -- The keybind to change to the user self radio station. The default keybind is set to 121 / F10.
Keybind_Off = 122 -- The keybind to turn the radio station off. The default keybind is set to 122 / F11.
Keybind_Engine = 123 -- The keybind to cutoff the engine. The default keybind is set to 123 / F12.

-- https://docs.microsoft.com/en-us/windows/win32/inputdev/virtual-key-codes

--------------
-- Controls --
--------------

Control_Input = 0 -- 0 | 1 | 2

Control_A = 63 -- It's used to cancel the cutoff switch when you move left on a vehicle. Only change if you have a custom keybind set for 63 / A.
Control_D = 64 -- It's used to cancel the cutoff switch when you move right on a vehicle. Only change if you have a custom keybind set for 64 / D.
Control_W = 71 -- It's used to cancel the cutoff switch when you move up a vehicle. Only change if you have a custom keybind set for 71 / W.
Control_S = 72 -- It's used to cancel the cutoff switch when you move down on a vehicle. Only change if you have a custom keybind set for 72 / S.
Control_F = 75 -- It's used for when you exit out of a vehicle, to keep the engine running or door open, when you hold down the key. Only change if you have a custom keybind set for 75 / F.

-- https://docs.fivem.net/docs/game-references/controls

-----------
-- Radio --
-----------

-- 0. RADIO_01_CLASS_ROCK | Los Santos Rock Radio
-- 1. RADIO_02_POP | Non Stop Pop FM
-- 2. RADIO_03_HIPHOP_NEW | Radio Los Santos
-- 3. RADIO_04_PUNK | Channel X
-- 4. RADIO_05_TALK_01 | West Coast Talk Radio
-- 5. RADIO_06_COUNTRY | Rebel Radio
-- 6. RADIO_07_DANCE_01 | Soulwax FM
-- 7. RADIO_08_MEXICAN | East Los FM
-- 8. RADIO_09_HIPHOP_OLD | West Coast Classics
-- 9. RADIO_12_REGGAE | The Blue Ark
-- 10. RADIO_13_JAZZ | World Wide FM
-- 11. RADIO_14_DANCE_02 | FlyLo FM
-- 12. RADIO_15_MOTOWN | The Lowdown 91.1
-- 13. RADIO_20_THELAB | The Lab
-- 14. RADIO_16_SILVERLAKE | Radio Mirror Park
-- 15. RADIO_17_FUNK | Space 103.2
-- 16. RADIO_18_90S_ROCK | Vinewood Boulevard Radio
-- 17. RADIO_21_DLC_XM17 | Blonded Los Santos 97.8 FM
-- 18. RADIO_22_DLC_BATTLE_MIX1_RADIO | Los Santos Underground Radio
-- 19. RADIO_23_DLC_XM19_RADIO | iFruit Radio
-- 20. RADIO_19_USER | Self Radio

-- 21. RADIO_11_TALK_02
-- 22. HIDDEN_RADIO_AMBIENT_TV_BRIGHT
-- 23. HIDDEN_RADIO_AMBIENT_TV
-- 24. HIDDEN_RADIO_01_CLASS_ROCK
-- 25. HIDDEN_RADIO_ADVERTS
-- 26. HIDDEN_RADIO_CASINO
-- 27. HIDDEN_RADIO_ARCADE_POP
-- 28. HIDDEN_RADIO_ARCADE_WWFM
-- 29. HIDDEN_RADIO_ARCADE_MIRROR_PARK
-- 30. HIDDEN_RADIO_ARCADE_EDM
-- 31. HIDDEN_RADIO_ARCADE_DANCE
-- 32. HIDDEN_RADIO_CASINO_PENTHOUSE_P
-- 33. HIDDEN_RADIO_02_POP
-- 34. HIDDEN_RADIO_03_HIPHOP_NEW
-- 35. HIDDEN_RADIO_04_PUNK
-- 36. HIDDEN_RADIO_06_COUNTRY
-- 37. HIDDEN_RADIO_07_DANCE_01
-- 38. HIDDEN_RADIO_09_HIPHOP_OLD
-- 39. HIDDEN_RADIO_12_REGGAE
-- 40. HIDDEN_RADIO_15_MOTOWN
-- 41. HIDDEN_RADIO_16_SILVERLAKE
-- 42. HIDDEN_RADIO_17_FUNK
-- 43. RADIO_22_DLC_BATTLE_MIX1_CLUB
-- 44. HIDDEN_RADIO_STRIP_CLUB
-- 45. DLC_BATTLE_MIX1_CLUB_PRIV
-- 46. HIDDEN_RADIO_BIKER_CLASSIC_ROCK
-- 47. RADIO_23_DLC_BATTLE_MIX2_CLUB
-- 48. DLC_BATTLE_MIX2_CLUB_PRIV
-- 49. HIDDEN_RADIO_BIKER_MODERN_ROCK
-- 50. RADIO_24_DLC_BATTLE_MIX3_CLUB
-- 51. RADIO_26_DLC_BATTLE_CLUB_WARMUP
-- 52. RADIO_25_DLC_BATTLE_MIX4_CLUB
-- 53. HIDDEN_RADIO_BIKER_PUNK
-- 54. DLC_BATTLE_MIX4_CLUB_PRIV
-- 55. DLC_BATTLE_MIX3_CLUB_PRIV
-- 56. HIDDEN_RADIO_BIKER_HIP_HOP

-- 255. OFF | Radio Off

-------------
-- Vehicle --
-------------

-- 0 = Exit the vechicle and close the door normal.
-- 1 = Exit the vehicle and close the door normal.
-- 16 = Exit the vehicle by teleporting right outside.
-- 64 = Exit the vehicle and close the door normal.
-- 256 = Exit the vehicle and keep door open.
-- 320 = Exit the vehicle and close the door normal.
-- 512 = Exit the vehicle and close the door normal.
-- 4160 = Throw yourself out of the vehicle. If the vehicle is still it will accelerate or reverse a little, depending on what was last pressed key.
-- 131072 = Exit the vehicle and close the door normal.
-- 262144 = Move to passenger seat first, then exit normal.

--------------
--- Script ---
--------------

local Script = {}

local Version = '1.0'

local Boolean = false
local Toggle = false

local Input = Radio_Default
local Interval = Vehicle_Interval

local Engine = 0
local Door = 0

function Parse(Identification)
		if (type(Input) == 'number') then
			local Radio = AUDIO.GET_PLAYER_RADIO_STATION_INDEX()

			if Radio ~= Input and Input ~= 255 then
				AUDIO.SET_RADIO_TO_STATION_INDEX(Input)
			elseif Radio ~= Input and Input == 255 then
				AUDIO.SET_RADIO_TO_STATION_NAME('OFF')
			end
		elseif (type(Input) == 'string') then
			local Radio = AUDIO.GET_PLAYER_RADIO_STATION_NAME()

			if Radio == nil then
				Radio = 'OFF'
			end

			if Radio ~= Input then
				AUDIO.SET_RADIO_TO_STATION_NAME(Input)
			end
		end
end

function Control(Identification, Vehicle)
	if (Key_Number_Just_Pressed(Keybind_Favorite) or Key_String_Just_Pressed(Keybind_Favorite)) then
		Input = Radio_Favorite
		Parse(Identification)
	end

	if (Key_Number_Just_Pressed(Keybind_Self) or Key_String_Just_Pressed(Keybind_Self)) then
		Input = Radio_Self
		Parse(Identification)
	end

	if (Key_Number_Just_Pressed(Keybind_Off) or Key_String_Just_Pressed(Keybind_Off)) then
		Input = Radio_Off
		Parse(Identification)
	end

	if (Key_Number_Just_Pressed(Keybind_Engine) or Key_String_Just_Pressed(Keybind_Engine)) then
		if (Vehicle ~= 0 and VEHICLE.GET_PED_IN_VEHICLE_SEAT(Vehicle, -1) == Identification) then
			Toggle = true
		end
	end
end

function Script.tick()
	local Identification = PLAYER.PLAYER_PED_ID()
	local Vehicle = PED.GET_VEHICLE_PED_IS_USING(Identification)
	local Driver	= (VEHICLE.GET_PED_IN_VEHICLE_SEAT(Vehicle, -1) == Identification)
	local Police = PED.IS_PED_IN_ANY_POLICE_VEHICLE(Identification)
	local Foot = PED.IS_PED_ON_FOOT(Identification)

	if (not ENTITY.DOES_ENTITY_EXIST(Identification)) then return end

	if (Toggle and CONTROLS.IS_CONTROL_PRESSED(Control_Input, Control_A) or Toggle and CONTROLS.IS_CONTROL_PRESSED(Control_Input, Control_D) or Toggle and CONTROLS.IS_CONTROL_PRESSED(Control_Input, Control_S) or Toggle and CONTROLS.IS_CONTROL_PRESSED(Control_Input, Control_W)) then
		Toggle = false
	end

	if (Toggle) then
		VEHICLE.SET_VEHICLE_ENGINE_ON(Vehicle, false, true)
	end

	if (Radio_Loud and Vehicle ~= 0) then
		AUDIO.SET_VEHICLE_RADIO_LOUD(Vehicle, true)
	end

	if (Active_Modifier) then
		if (Key_Number_Pressed(Keybind_Control) or Key_String_Pressed(Keybind_Control)) then
			Control(Identification, Vehicle)
		end
	else
		Control(Identification, Vehicle)
	end

	if (Police) then
		if (Radio_Police) then
			AUDIO.SET_MOBILE_RADIO_ENABLED_DURING_GAMEPLAY(true)
		else
			AUDIO.SET_MOBILE_RADIO_ENABLED_DURING_GAMEPLAY(false)
		end
	end

	if (Foot) then
		if (Radio_Mobile) then
			AUDIO.SET_MOBILE_RADIO_ENABLED_DURING_GAMEPLAY(true)
		else
			AUDIO.SET_MOBILE_RADIO_ENABLED_DURING_GAMEPLAY(false)
		end
	end

	if (Vehicle ~= 0 and Driver) then
		if (CONTROLS.IS_CONTROL_PRESSED(Control_Input, Control_F)) then
			if (Vehicle_Engine and not Toggle) then
				if (Engine > Interval / 100) then
					VEHICLE.SET_VEHICLE_ENGINE_ON(Vehicle, true, true)
				end
				Engine = Engine + 1
			end

			if (Vehicle_Door) then
				if (Door > Interval / 100) then
					AI.TASK_LEAVE_VEHICLE(Identification, Vehicle, 256)
				end
				Door = Door + 1
			end
		end
	else
		Engine = 0
		Door = 0
	end

	if (PED.IS_PED_IN_ANY_VEHICLE(Identification, false)) then
		if (Radio_Force == true) then Input = Radio_Default end

		if (type(Input) == 'number') then
			local Radio = AUDIO.GET_PLAYER_RADIO_STATION_INDEX()

			if not Boolean and Radio ~= Input and Input ~= 255 then
				AUDIO.SET_RADIO_TO_STATION_INDEX(Input)
				Boolean = true
			elseif not Boolean and Radio ~= Input and Input == 255 then
				AUDIO.SET_RADIO_TO_STATION_NAME('OFF')
				Boolean = true
			end
		elseif (type(Input) == 'string') then
			local Radio = AUDIO.GET_PLAYER_RADIO_STATION_NAME()

			if Radio == nil then
				Radio = 'OFF'
			end

			if not Boolean and Radio ~= Input then
				AUDIO.SET_RADIO_TO_STATION_NAME(Input)
				Boolean = true
			end
		end
	else
		Boolean = false
	end
end

return Script