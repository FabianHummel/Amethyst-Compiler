# Get the data type of the passed data
#
# input: `type.in`
# output: `type.out`

data remove storage amethyst:internal data.type.out
execute if function amethyst:internal/data/type/check_object run return 1
execute if function amethyst:internal/data/type/check_array run return 1
execute if function amethyst:internal/data/type/check_string run return 1
function amethyst:internal/data/type/check_numeric with storage amethyst:internal data.type