data modify storage amethyst:internal string.stringify._stack append from storage amethyst:internal string.stringify._stack[-1][0]
function amethyst:internal/string/stringify/loop
data remove storage amethyst:internal string.stringify._stack[-1][0]
# Rerun for next element if exists
execute if data storage amethyst:internal string.stringify._stack[-1][] run function amethyst:internal/string/stringify/array/next