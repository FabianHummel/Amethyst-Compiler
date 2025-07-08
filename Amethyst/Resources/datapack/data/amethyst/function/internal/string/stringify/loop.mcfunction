# Detect of which type the current data is
data modify storage amethyst:internal type.in set from storage amethyst:internal string.stringify._stack[-1]._
function amethyst:api/type

# Depending on the resulting data type, create the JSON text components
execute if data storage amethyst:internal {type:{out:"array"}} run function amethyst:internal/string/stringify/visit_array
execute if data storage amethyst:internal {type:{out:"object"}} run function amethyst:internal/string/stringify/visit_object
execute if data storage amethyst:internal {type:{out:"int"}} run function amethyst:internal/string/stringify/visit_int with storage amethyst:internal string.stringify._stack[-1]