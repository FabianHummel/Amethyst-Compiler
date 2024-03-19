scoreboard players reset _ret amethyst
data storage remove amethyst:internal _ret
function playground:my_namespace/str_function
data modify storage amethyst:internal _out set from storage amethyst:internal _ret
tellraw @s {'storage':'amethyst:internal','nbt':'_out'}
