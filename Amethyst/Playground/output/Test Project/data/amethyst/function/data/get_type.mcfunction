# Created using Amethyst v1.0.1.0 on 2025-10-18.
data remove storage amethyst:internal data.type.out
execute if function amethyst:internal/data/type/check_object run return 1
execute if function amethyst:internal/data/type/check_array run return 1
execute if function amethyst:internal/data/type/check_string run return 1
function amethyst:internal/data/type/check_numeric with storage amethyst:internal data.type/
