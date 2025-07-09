# Key
data modify storage amethyst:internal string.stringify._key set from storage amethyst:internal string.stringify._stack[-1].keys[0]
function amethyst:internal/string/stringify/object/key with storage amethyst:internal string.stringify
# Value
function amethyst:internal/string/stringify/object/extract_value with storage amethyst:internal string.stringify
function amethyst:internal/string/stringify/loop
data remove storage amethyst:internal string.stringify._stack[-1].keys[0]
# Rerun for next element if exists
execute if data storage amethyst:internal string.stringify._stack[-1].keys[] run function amethyst:internal/string/stringify/object/next