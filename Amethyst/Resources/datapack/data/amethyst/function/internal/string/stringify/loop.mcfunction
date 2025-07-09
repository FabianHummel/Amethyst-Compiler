# Detect of which type the current data is
data modify storage amethyst:internal type.in set from storage amethyst:internal string.stringify._stack[-1]
function amethyst:api/type

# Depending on the resulting data type, create the JSON text components
execute if data storage amethyst:internal {type:{out:"object"}} run return run function amethyst:internal/string/stringify/visit_object
execute if data storage amethyst:internal {type:{out:"array"}} run return run function amethyst:internal/string/stringify/visit_array
data modify storage amethyst:internal string.stringify._cur set from storage amethyst:internal string.stringify._stack[-1]
execute if data storage amethyst:internal {type:{out:"string"}} run return run function amethyst:internal/string/stringify/visit_string with storage amethyst:internal string.stringify
execute if data storage amethyst:internal {type:{out:"int"}} run return run function amethyst:internal/string/stringify/visit_int with storage amethyst:internal string.stringify
execute if data storage amethyst:internal {type:{out:"byte"}} run return run function amethyst:internal/string/stringify/visit_byte with storage amethyst:internal string.stringify
execute if data storage amethyst:internal {type:{out:"double"}} run return run function amethyst:internal/string/stringify/visit_double with storage amethyst:internal string.stringify
execute if data storage amethyst:internal {type:{out:"float"}} run return run function amethyst:internal/string/stringify/visit_float with storage amethyst:internal string.stringify
execute if data storage amethyst:internal {type:{out:"short"}} run return run function amethyst:internal/string/stringify/visit_short with storage amethyst:internal string.stringify
execute if data storage amethyst:internal {type:{out:"long"}} run return run function amethyst:internal/string/stringify/visit_long with storage amethyst:internal string.stringify

tellraw @a [{"text":"Error whilst decoding data: Unexpected value '","color":"red"},{"nbt":"type.in","storage":"amethyst:internal"},"'."]