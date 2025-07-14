# Created using Amethyst v1.0.1.0 on 2025-07-14.
data modify storage amethyst: 1 set value "okey"
data modify storage amethyst: 2 set value {keys:["okey"],data:{"okey":"nice"}}
function test:_load/_index0 with storage amethyst:
data modify storage amethyst:internal data.stringify.in set from storage amethyst: 2
function amethyst:api/data/stringify
tellraw @a ["",{"text":"DEBUG: ","color":"dark_gray"},{"nbt":"data.stringify.out","storage":"amethyst:internal","interpret":true}]
