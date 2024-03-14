function amethyst:println

data modify storage amethyst:internal _out set from storage amethyst:constants c1
data modify storage amethyst:internal main.y set from storage amethyst:internal _out value
