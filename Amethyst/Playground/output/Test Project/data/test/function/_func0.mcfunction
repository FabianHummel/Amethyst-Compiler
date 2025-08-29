# Created using Amethyst v1.0.1.0 on 2025-08-29.
scoreboard players set 1 amethyst 10
scoreboard players set 2 amethyst 30
scoreboard players operation 2 amethyst += 1 amethyst
execute store result storage amethyst:internal data.stringify.in int 1 run scoreboard players get 2 amethyst
function amethyst:api/data/stringify
tellraw @a ["",{"text":"DEBUG: ","color":"dark_gray"},{"nbt":"data.stringify.out","storage":"amethyst:internal","interpret":true}]
