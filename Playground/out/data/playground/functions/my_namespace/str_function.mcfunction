scoreboard players reset _ret amethyst
data storage remove amethyst:internal _ret
data modify storage amethyst:internal _out set value 'Hello, world!'
data modify storage amethyst:internal _ret set from storage amethyst:internal _out
execute unless data storage amethyst:internal _ret run function playground:my_namespace/str_function/_amethyst_arf
