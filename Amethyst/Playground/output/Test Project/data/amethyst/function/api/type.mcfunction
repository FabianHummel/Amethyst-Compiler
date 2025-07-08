# Get the data type of the passed data
#
# input: `type.in`
# output: `type.out`

execute if function amethyst:internal/type/check_object run return 1
execute if function amethyst:internal/type/check_array run return 1
execute if function amethyst:internal/type/check_string run return 1
execute if function amethyst:internal/type/check_int run return 1
execute if function amethyst:internal/type/check_byte run return 1
execute if function amethyst:internal/type/check_double run return 1
execute if function amethyst:internal/type/check_float run return 1
execute if function amethyst:internal/type/check_short run return 1
execute if function amethyst:internal/type/check_long run return 1
