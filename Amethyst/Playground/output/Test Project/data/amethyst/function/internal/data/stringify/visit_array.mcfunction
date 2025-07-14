data modify storage amethyst:internal data.stringify.out append value "["
# Iterate over all elements within the array and concatenate them to the output
execute if data storage amethyst:internal data.stringify._stack[-1][] run function amethyst:internal/data/stringify/array/loop
data remove storage amethyst:internal data.stringify._stack[-1]
data modify storage amethyst:internal data.stringify.out append value "]"
data remove storage amethyst:internal data.type.out