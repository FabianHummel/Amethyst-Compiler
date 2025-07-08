# Key
data modify storage amethyst:internal string.stringify._key set from storage amethyst:internal string.stringify._stack[-1]._.keys[0]
function amethyst:internal/string/stringify/object/key with storage amethyst:internal string.stringify

# Value
data modify storage amethyst:internal string.stringify._stack append value {_:0}
function amethyst:internal/string/stringify/object/extract_value with storage amethyst:internal string.stringify
function amethyst:internal/string/stringify/loop
data remove storage amethyst:internal string.stringify._stack[-1]._.keys[0]

data modify storage amethyst:internal string.stringify._str set value ","
function amethyst:internal/string/stringify/concat with storage amethyst:internal string.stringify

# Rerun for next element if exists
execute if data storage amethyst:internal string.stringify._stack[-1]._.keys[] run function amethyst:internal/string/stringify/object/next