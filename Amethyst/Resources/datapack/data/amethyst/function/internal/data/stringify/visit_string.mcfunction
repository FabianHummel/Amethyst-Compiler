$data modify storage amethyst:internal data.stringify.out append value ["\"",{"text":"$(_cur)","color":"green"},"\""]
data remove storage amethyst:internal data.stringify._stack[-1]
data remove storage amethyst:internal data.type.out