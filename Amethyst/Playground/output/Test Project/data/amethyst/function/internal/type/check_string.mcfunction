data modify storage amethyst:internal type._check set string storage amethyst:internal type.in
execute store success score type._success amethyst run data modify storage amethyst:internal type._check set from storage amethyst:internal type.in
execute if score type._success amethyst matches 0 run return run data modify storage amethyst:internal type.out set value "string"