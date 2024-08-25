# Created using Amethyst v1.0.1.0 on 2024-08-25.
data modify storage amethyst: 0 set value [{_:19},{_:6},{_:10},{_:"nice"},{_:8},{_:10}]
function test:_load/_array0
data modify storage amethyst: 1 set value "nice"
scoreboard players set 2 amethyst 432
