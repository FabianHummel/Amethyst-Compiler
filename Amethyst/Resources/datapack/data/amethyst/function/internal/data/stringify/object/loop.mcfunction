# Key
data modify storage amethyst:internal data.stringify._key set from storage amethyst:internal data.stringify._stack[-1].keys[0]
function amethyst:internal/data/stringify/object/key with storage amethyst:internal data.stringify
# Value
function amethyst:internal/data/stringify/object/extract_value with storage amethyst:internal data.stringify
function amethyst:internal/data/stringify/loop
data remove storage amethyst:internal data.stringify._stack[-1].keys[0]
# Rerun for next element if exists
execute if data storage amethyst:internal data.stringify._stack[-1].keys[] run function amethyst:internal/data/stringify/object/next