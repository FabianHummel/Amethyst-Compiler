# Convert any data structure to JSON text components including syntax highlighting.
#
# input: `string.stringify.in`
# output: `string.stringify.out`

data modify storage amethyst:internal data.stringify.out set value []
data modify storage amethyst:internal data.stringify._stack set value []
data modify storage amethyst:internal data.stringify._stack append from storage amethyst:internal data.stringify.in
function amethyst:internal/data/stringify/loop