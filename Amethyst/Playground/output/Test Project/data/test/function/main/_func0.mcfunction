# Created using Amethyst v1.0.1.0 on 2025-10-16.
data modify storage amethyst: 1 set value [1,2,3]
execute store result storage amethyst:internal data.stringify.in int 1 run scoreboard players get amethyst: 1
function amethyst:internal/data/stringify/run
tellraw @a ["",{"text":"DEBUG: ","color":"dark_gray"},{"nbt":"data.stringify.out","storage":"amethyst:internal","interpret":true}]
function amethyst:test/_func0
