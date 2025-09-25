# Created using Amethyst v1.0.1.0 on 2025-09-25.
scoreboard players set 1 amethyst 404
scoreboard players set 2 amethyst 241
scoreboard players operation 3 amethyst = 1 amethyst
scoreboard players operation 3 amethyst *= .10 amethyst_const
scoreboard players operation 3 amethyst += 2 amethyst
execute store result storage amethyst:internal data.stringify.in double 0.01 run scoreboard players get 3 amethyst
function amethyst:api/data/stringify
tellraw @a ["",{"text":"DEBUG: ","color":"dark_gray"},{"nbt":"data.stringify.out","storage":"amethyst:internal","interpret":true}]
