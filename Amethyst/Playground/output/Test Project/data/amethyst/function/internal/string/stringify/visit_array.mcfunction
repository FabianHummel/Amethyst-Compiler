# Opening bracket
data modify storage amethyst:internal string.stringify._str set value "[\"[\","
function amethyst:internal/string/stringify/concat with storage amethyst:internal string.stringify

# Iterate over all elements within the array and concatenate them to the output
execute if data storage amethyst:internal string.stringify._stack[-1]._[] run function amethyst:internal/string/stringify/array/init

# Closing bracket
data modify storage amethyst:internal string.stringify._str set value "\"]\"]"
function amethyst:internal/string/stringify/concat with storage amethyst:internal string.stringify

data remove storage amethyst:internal type.out