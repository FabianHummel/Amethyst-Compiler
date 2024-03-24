data modify storage amethyst:internal _out set value 'Else'
tellraw @s {'storage':'amethyst:internal','nbt':'_out'}
execute if function playground:_init/_if0/_else/_if0 run return
