data modify storage amethyst:internal data.stringify._stack append from storage amethyst:internal data.stringify._stack[-1][0]
function amethyst:internal/data/stringify/loop
data remove storage amethyst:internal data.stringify._stack[-1][0]
# Rerun for next element if exists
execute if data storage amethyst:internal data.stringify._stack[-1][] run function amethyst:internal/data/stringify/array/next