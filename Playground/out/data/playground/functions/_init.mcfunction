execute if function playground:_init/_if0 run return
execute if function playground:_init/_if1 run return
data modify storage amethyst:internal _out set value 'End'
tellraw @s {'storage':'amethyst:internal','nbt':'_out'}
