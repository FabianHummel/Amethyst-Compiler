# Created using Amethyst v1.0.1.0 on 2025-10-16.
data modify storage amethyst:internal data.stringify.out set value []
data modify storage amethyst:internal data.stringify._stack set value []
data modify storage amethyst:internal data.stringify._stack append from storage amethyst:internal data.stringify.in
function amethyst:internal/data/stringify/loop
