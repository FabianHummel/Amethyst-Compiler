
#     
#     ______     __    __     ______    ______    __  __     __  __     ______     ______  
#    /\  __ \   /\ "-./  \   /\  ___\  /\__  _\  /\ \_\ \   /\ \_\ \   /\  ___\   /\__  _\ 
#    \ \  __ \  \ \ \-./\ \  \ \  __\  \/_/\ \/  \ \  __ \  \ \____ \  \ \___  \  \/_/\ \/ 
#     \ \_\ \_\  \ \_\ \ \_\  \ \_____\   \ \_\   \ \_\ \_\  \/\_____\  \/\_____\    \ \_\ 
#      \/_/\/_/   \/_/  \/_/   \/_____/    \/_/    \/_/\/_/   \/_____/   \/_____/     \/_/ 
#                                                                                          
#     -=-=-= COMPILER FRAMEWORK =-=-=-=-=-=-=-=-=-                                                   
#

# Amethyst
scoreboard objectives add amethyst dummy {"text":"Amethyst internals","color":"dark_purple"}
scoreboard players reset * amethyst

scoreboard objectives add amethyst_const dummy {"text":"Amethyst constants","color":"dark_purple"}
scoreboard players reset * amethyst_const

data merge storage amethyst:internal {_out:{}}
data merge storage amethyst:constants {}

# User definitions
scoreboard players set c0 amethyst_const 300
scoreboard players operation _out amethyst = c0 amethyst_const
function amethyst:nice

data modify storage amethyst:constants c1 set value 'Nice!169'
