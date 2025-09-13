# Created using Amethyst v1.0.1.0 on 2025-09-13.
scoreboard players set 1 amethyst 0
execute store success score 2 amethyst run function test:_func0/_and0
execute store result storage amethyst:internal data.stringify.in byte 1 run scoreboard players get 2 amethyst
function amethyst:api/data/stringify
tellraw @a ["",{"text":"DEBUG: ","color":"dark_gray"},{"nbt":"data.stringify.out","storage":"amethyst:internal","interpret":true}]
