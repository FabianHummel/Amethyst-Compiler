$data modify storage amethyst:internal string.stringify._str set value "{\"text\":\"$(_)\",\"color\":\"gold\"}"
function amethyst:internal/string/stringify/concat with storage amethyst:internal string.stringify
data remove storage amethyst:internal string.stringify._stack[-1]

data remove storage amethyst:internal type.out