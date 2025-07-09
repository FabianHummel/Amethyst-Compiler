# Convert any data structure to JSON text components including syntax highlighting.
#
# input: `string.stringify.in`
# output: `string.stringify.out`

data modify storage amethyst:internal string.stringify.out set value []
data modify storage amethyst:internal string.stringify._stack set value []
data modify storage amethyst:internal string.stringify._stack append from storage amethyst:internal string.stringify.in
function amethyst:internal/string/stringify/loop