function CreateCounter()
	local count = 0
	return function()
		count = count + 1
		return count
	end
end

c1 = CreateCounter()
print(c1())
print(c1())

c2 = CreateCounter()
print(c2())
print(c1())
print(c2())