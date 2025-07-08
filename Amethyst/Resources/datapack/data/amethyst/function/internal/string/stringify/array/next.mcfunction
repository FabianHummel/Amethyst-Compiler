# Separator
data modify storage amethyst:internal string.stringify._str set value "\", \","
function amethyst:internal/string/stringify/concat with storage amethyst:internal string.stringify

function amethyst:internal/string/stringify/array/loop