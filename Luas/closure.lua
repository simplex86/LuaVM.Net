function counter()
	local count = 0
	return function()
		count = count + 1
		return count
	end
end

a = counter()
print(a())
print(a())

b = counter()
print(b())
print(b())
print(b())