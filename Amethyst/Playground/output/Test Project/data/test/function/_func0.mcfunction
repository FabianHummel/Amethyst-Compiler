# Created using Amethyst v1.0.1.0 on 2025-09-12.
scoreboard players set 1 amethyst 33
execute store result storage amethyst:internal data.stringify.in int 1 run scoreboard players get 1 amethyst
function amethyst:api/data/stringify
tellraw @a ["",{"text":"DEBUG: ","color":"dark_gray"},{"nbt":"data.stringify.out","storage":"amethyst:internal","interpret":true}]
