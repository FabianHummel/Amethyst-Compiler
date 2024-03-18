data modify storage amethyst:internal _out set value 'I should never print'
tellraw @s {'storage':'amethyst:internal','nbt':'_out'}
