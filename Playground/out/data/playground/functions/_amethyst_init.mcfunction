data modify storage amethyst:internal v0 set value []
scoreboard players set _out amethyst 1
data modify storage amethyst:internal v0 append value 0
execute store result storage amethyst:internal v0[-1] int 1.0 run scoreboard players get _out amethyst
scoreboard players set _out amethyst 2
data modify storage amethyst:internal v0 append value 0
execute store result storage amethyst:internal v0[-1] int 1.0 run scoreboard players get _out amethyst
data modify storage amethyst:internal _out set value 'hello'
data modify storage amethyst:internal v0 append from storage amethyst:internal _out
data modify storage amethyst:internal _out set from storage amethyst:internal v0
data modify storage amethyst:internal v0 append from storage amethyst:internal _out
scoreboard players set _out amethyst 5
data modify storage amethyst:internal v0 append value 0
execute store result storage amethyst:internal v0[-1] int 1.0 run scoreboard players get _out amethyst
data modify storage amethyst:internal _out set from storage amethyst:internal v0
data modify storage amethyst:internal v0 append from storage amethyst:internal _out
data modify storage amethyst:internal _out set from storage amethyst:internal v0
data remove storage amethyst:internal v0
data modify storage amethyst:internal v0 set from storage amethyst:internal _out
function playground:main
