# Created using Amethyst v1.0.1.0 on 2025-07-05.
execute unless score 1 amethyst matches 0 run return 1
scoreboard players set 2 amethyst 0
execute unless score 2 amethyst matches 0 run return 1
return fail
