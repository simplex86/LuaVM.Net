--[[
	04.lua
]]

local function func(a, b)
	local sum = 0
	for i=a, b do
		sum = sum + i
	end
	return sum
end

func(1, 10)