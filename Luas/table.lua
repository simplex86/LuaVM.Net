local function func()
	local t = { "a", "b", "c" }
	t[2] = "B"
	t["d"] = "d"
	local r = t[3] .. t[2] .. t[1] .. t["d"] .. #t
end

func()