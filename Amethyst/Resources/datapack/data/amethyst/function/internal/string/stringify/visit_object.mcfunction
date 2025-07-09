data modify storage amethyst:internal string.stringify.out append value "{"
# Iterate over all elements within the object and concatenate them to the output
execute if data storage amethyst:internal string.stringify._stack[-1].keys[] run function amethyst:internal/string/stringify/object/loop
data remove storage amethyst:internal string.stringify._stack[-1]
data modify storage amethyst:internal string.stringify.out append value "}"
data remove storage amethyst:internal type.out