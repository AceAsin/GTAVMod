; THIS ARCHIVE REDISTRIBUTION IS NOT ALLOWED, USE THE FOLLOWING LINK INSTEAD
; http://www.dev-c.com/gtav/scripthookv/


							SCRIPT HOOK V	

v1.0.2060.1
							
Description:
Script hook is the library that allows to use GTA V script native
functions in custom *.asi plugins.

You are allowed to use this software only if you agree to the terms
written below:
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR
THE USE OR OTHER DEALINGS IN THE SOFTWARE

Multiplayer:
Script hook closes the game when player goes Online, this is done because
the game reports installed mods list to R* while being in Online mode.

Installation:
1. Copy ScriptHookV.dll to the game's main folder, i.e. where GTA5.exe
   is located.
2. In order to load asi plugins you need to have asi loader installed,
   you can download it separately or use the latest version that comes
   with this distrib (dinput8.dll).
   You must delete old dsound.dll asiloader if you have one installed.
3. This distrib also includes a sample asi plugin - native trainer,
   if you need a trainer then copy NativeTrainer.asi as well.
   
Changelog:
v1.0.2060.1
- added support of the latest patch
v1.0.2060.0
- added support of the latest patch
v1.0.1868.4
- added support of the egs version
v1.0.1868.1
- added support of the latest patch
v1.0.1868.0
- added support of the latest patch
v1.0.1737.6
- added support of the latest patch
v1.0.1737.0
- added support of the latest patch
v1.0.1604.1
- added support of the latest patch
v1.0.1604.0
- added support of the latest patch
v1.0.1493.1
- added support of the latest patch
v1.0.1493.0
- added support of the latest patch
v1.0.1365.1
- added support of the latest patch
v1.0.1290.1
- added support of the latest patch
v1.0.1180.2
- added support of the latest patch
v1.0.1103.2
- added support of the latest patch
v1.0.1032.1
- added support of the latest patch
v1.0.1011.1
- added support of the latest patch
v1.0.944.2
- added support of the latest patch
v1.0.877.1
- added support of the latest patch
v1.0.791.2
- added support of the latest patch
v1.0.757.4
- added support of the latest patch
v1.0.757.2
- added support of the latest patch
v1.0.678.1
- added support of the latest patch
v1.0.617.1a
- added ability to access pickup pool
- added ability to get base object pointer using script handle
v1.0.617.1
- added support of the latest patch
v1.0.573.1a
- fixed an issue with object spawning limit with the latest patch
v1.0.573.1
- added support of the latest patch
v1.0.505.2a
- fixed an issue with starting more than 20 scripts with the latest patch
v1.0.505.2
- added support of the latest patch
v1.0.463.1
- added support of the latest patch
v1.0.393.4a
- added ability to create more objects
- added ability to access entity pools
v1.0.393.4
- added support of the latest patch
v1.0.393.2
- added support of the latest patch
v1.0.372.2a
- added directx hook
- added ability to access script globals
v1.0.372.2
- added support of the latest patch
v1.0.350.2b
- fixed thread start issue while using more than 20 scripts
   
Supported game versions:
1.0.335.2, 1.0.350.1/2, 1.0.372.2, 1.0.393.2/4, 1.0.463.1,
1.0.505.2, 1.0.573.1, 1.0.617.1, 1.0.678.1, 1.0.757.2/4,
1.0.791.2, 1.0.877.1, 1.0.944.2, 1.0.1011.1, 1.0.1032.1,
1.0.1103.2, 1.0.1180.2, 1.0.1290.1, 1.0.1365.1, 1.0.1493.0/1,
1.0.1604.0/1, 1.0.1737.0/6, 1.0.1868.0/1/4, 1.0.2060.0/1
   
							NATIVE TRAINER
			
Description:			
The trainer comes with ScriptHookV distrib and shows an example of what
can be done using scripts. 

Changelog:
v1.0.1011.1
- added new dlc vehicles
v1.0.944.2
- added new dlc vehicles
v1.0.877.1
- added new dlc vehicles and weapons
v1.0.791.2
- added new dlc vehicles
v1.0.757.2
- added new dlc vehicles
v1.0.678.1
- added new dlc vehicles and weapons
v1.0.617.1
- added new dlc vehicles
v1.0.573.1
- added new dlc vehicles, peds and weapons
v1.0.505.2
- added new dlc vehicles, peds and weapons
v1.0.393.2
- added new dlc vehicles and weapons
v1.0.372.2a
- added strong wheels vehicle option
v1.0.372.2
- added new dlc cars and weapon
v1.0.350.2b
- added car seat belt option
- added force weather set option
- corrected car boost behaviour on car stuck
v1.0.350.2
- broken models are removed from skin changer
- fixed infinite loading on death/arrest while being in another skin
- fixed an issue when trainer was closing after showing up

Controls: 
F4					activate
NUM2/8/4/6			navigate thru the menus and lists (numlock must be on)
NUM5 				select
NUM0/BACKSPACE/F4 	back
NUM9/3 				use vehicle boost when active
NUM+ 				use vehicle rockets when active

- player
     - skin changer -- select any char you want, including animals
     - teleport -- teleport anywhere, has location set and ability
                   to perform proper map marker teleportation
     - invincible
     - fix player
     - add cash
     - wanted up/down/never
     - police ignored
     - unlimited ability
     - noiseless
     - fast swim/run
     - super jump
 - vehicle
     - speed boost -- use NUM9/3
     - car spawner
	 - strong wheels
	 - seat belt	 
     - wrap in spawned
     - paint random
     - fix
     - invincible
 - weapon
     - get all weapon
     - vehicle rockets -- use NUM+ in vehicle
     - no reload
     - fire ammo
     - explosive ammo
     - explosive melee
 - world
     - toggle random trains/boats/etc
 - time
     - hour forward/backward
     - pause
     - sync with system
 - weather
     - set wind
     - select weather
 - misc
     - change radio track
     - hide hud

Usefull links:
http://dev-c.com
http://gtaforums.com/topic/788343-script-hook-v/

							(C) Alexander Blade
								12 Sep 2020