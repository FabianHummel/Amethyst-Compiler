$data modify storage amethyst:internal string.stringify.out append value [{"text":"$(_cur)","color":"gold"},{"text":"b","color":"red"}]
data remove storage amethyst:internal string.stringify._stack[-1]
data remove storage amethyst:internal type.out