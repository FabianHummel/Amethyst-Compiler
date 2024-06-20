# check if _tmp has index 0
execute unless data storage amethyst:internal _tmp[0] run data modify storage amethyst:internal _out append from storage amethyst:internal _tmp
data modify storage amethyst:internal _tmp set from storage amethyst:internal _in[0].0
data remove storage amethyst:internal _in[0]
function amethyst:internal/string/prettify/loop