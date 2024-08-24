# check if _tmp has index 0
execute unless data storage amethyst: _tmp[0] run data modify storage amethyst: _out append from storage amethyst: _tmp
data modify storage amethyst: _tmp set from storage amethyst: _in[0].0
data remove storage amethyst: _in[0]
function amethyst:/string/prettify/loop