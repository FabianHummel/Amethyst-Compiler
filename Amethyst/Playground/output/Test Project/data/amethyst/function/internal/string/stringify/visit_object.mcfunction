# Opening bracelet
data modify storage amethyst:internal string.stringify._str set value "[\"{\","
function amethyst:internal/string/stringify/concat with storage amethyst:internal string.stringify

# Iterate over all elements within the object and concatenate them to the output
execute if data storage amethyst:internal string.stringify._stack[-1]._.keys[] run function amethyst:internal/string/stringify/object/init

# Closing bracelet
data modify storage amethyst:internal string.stringify._str set value "\"}\"]"
function amethyst:internal/string/stringify/concat with storage amethyst:internal string.stringify

data remove storage amethyst:internal type.out