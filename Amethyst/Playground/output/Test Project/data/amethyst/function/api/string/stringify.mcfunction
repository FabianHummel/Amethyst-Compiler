# Convert any data structure to JSON text components including syntax highlighting.
#
# input: `string.stringify.in`
# output: `string.stringify.out`

data modify storage amethyst:internal string.stringify.out set value ""
data modify storage amethyst:internal string.stringify._stack set value [{_:0}]
data modify storage amethyst:internal string.stringify._stack[-1]._ set from storage amethyst:internal string.stringify.in
function amethyst:internal/string/stringify/loop