scoreboard players reset _ret amethyst
data storage remove amethyst:internal _ret
# "mcfunction comment. nice!"
data modify storage amethyst:internal _tmp set value []
scoreboard players set _out amethyst 1
data modify storage amethyst:internal _tmp append value 0
execute store result storage amethyst:internal _tmp[-1] int 1.0 run scoreboard players get _out amethyst
scoreboard players set _out amethyst 2
data modify storage amethyst:internal _tmp append value 0
execute store result storage amethyst:internal _tmp[-1] int 1.0 run scoreboard players get _out amethyst
scoreboard players set _out amethyst 3
data modify storage amethyst:internal _tmp append value 0
execute store result storage amethyst:internal _tmp[-1] int 1.0 run scoreboard players get _out amethyst
scoreboard players set _out amethyst 4
data modify storage amethyst:internal _tmp append value 0
execute store result storage amethyst:internal _tmp[-1] int 1.0 run scoreboard players get _out amethyst
data modify storage amethyst:internal _out set from storage amethyst:internal y
data modify storage amethyst:internal _tmp append from storage amethyst:internal _out
data modify storage amethyst:internal _out set from storage amethyst:internal _tmp
data modify storage amethyst:internal _ret set from storage amethyst:internal _out
execute unless data storage amethyst:internal _ret run function playground:main/_amethyst_arf
