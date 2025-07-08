data modify storage amethyst:internal type._check set value [0l]
function amethyst:internal/type/run_check
return run execute if score type._success amethyst matches 1 run data modify storage amethyst:internal type.out set value "long"