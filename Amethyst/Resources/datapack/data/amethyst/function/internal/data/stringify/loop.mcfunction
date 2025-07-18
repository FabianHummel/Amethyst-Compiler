# Detect of which type the current data is
data modify storage amethyst:internal data.type.in set from storage amethyst:internal data.stringify._stack[-1]
function amethyst:api/data/type

# Depending on the resulting data type, create the JSON text components
execute if data storage amethyst:internal {data:{type:{out:"object"}}} run return run function amethyst:internal/data/stringify/visit_object
execute if data storage amethyst:internal {data:{type:{out:"array"}}} run return run function amethyst:internal/data/stringify/visit_array
data modify storage amethyst:internal data.stringify._cur set from storage amethyst:internal data.stringify._stack[-1]
execute if data storage amethyst:internal {data:{type:{out:"string"}}} run return run function amethyst:internal/data/stringify/visit_string with storage amethyst:internal data.stringify
execute if data storage amethyst:internal {data:{type:{out:"int"}}} run return run function amethyst:internal/data/stringify/visit_int with storage amethyst:internal data.stringify
execute if data storage amethyst:internal {data:{type:{out:"byte"}}} run return run function amethyst:internal/data/stringify/visit_byte with storage amethyst:internal data.stringify
execute if data storage amethyst:internal {data:{type:{out:"double"}}} run return run function amethyst:internal/data/stringify/visit_double with storage amethyst:internal data.stringify
execute if data storage amethyst:internal {data:{type:{out:"float"}}} run return run function amethyst:internal/data/stringify/visit_float with storage amethyst:internal data.stringify
execute if data storage amethyst:internal {data:{type:{out:"short"}}} run return run function amethyst:internal/data/stringify/visit_short with storage amethyst:internal data.stringify
execute if data storage amethyst:internal {data:{type:{out:"long"}}} run return run function amethyst:internal/data/stringify/visit_long with storage amethyst:internal data.stringify

tellraw @a [{"text":"Error whilst decoding data: Unexpected value '","color":"red"},{"nbt":"data.type.in","storage":"amethyst:internal"},"'."]