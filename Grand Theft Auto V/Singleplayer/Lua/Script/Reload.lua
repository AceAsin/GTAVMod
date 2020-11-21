local Reload = {}

function Reload.tick()
	if(get_key_pressed(162)) then
		if (get_key_pressed(82)) then
			loadAddIns()
			print("AddIns ReLoaded")
			wait(999)
		end
	end
end

return Reload