# Created using Amethyst v1.0.1.0 on 2025-07-05.
scoreboard players set 1 amethyst 123
scoreboard players set 2 amethyst 1
scoreboard players operation 2 amethyst *= .1000 amethyst_const
scoreboard players operation 2 amethyst += 1 amethyst
data modify storage amethyst: 3 set value [0d]
execute store result storage amethyst: 3[0] double 1 run scoreboard players get 2 amethyst
data modify storage amethyst: 4 set value [0d]
execute store result storage amethyst: 4[0] double 1 run scoreboard players get 1 amethyst
