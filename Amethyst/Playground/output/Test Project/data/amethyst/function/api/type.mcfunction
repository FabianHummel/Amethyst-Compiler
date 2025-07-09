# Get the data type of the passed data
#
# input: `type.in`
# output: `type.out`

data remove storage amethyst:internal type.out
execute if function amethyst:internal/type/check_object run return 1
execute if function amethyst:internal/type/check_array run return 1
execute if function amethyst:internal/type/check_string run return 1
function amethyst:internal/type/check_numeric with storage amethyst:internal type