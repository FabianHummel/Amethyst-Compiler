#     
#     ______     __    __     ______    ______    __  __     __  __     ______     ______  
#    /\  __ \   /\ "-./  \   /\  ___\  /\__  _\  /\ \_\ \   /\ \_\ \   /\  ___\   /\__  _\ 
#    \ \  __ \  \ \ \-./\ \  \ \  __\  \/_/\ \/  \ \  __ \  \ \____ \  \ \___  \  \/_/\ \/ 
#     \ \_\ \_\  \ \_\ \ \_\  \ \_____\   \ \_\   \ \_\ \_\  \/\_____\  \/\_____\    \ \_\ 
#      \/_/\/_/   \/_/  \/_/   \/_____/    \/_/    \/_/\/_/   \/_____/   \/_____/     \/_/ 
#                                                                                          
#     -=-=-= COMPILER PLATFORM =-=-=-=-=-=-=-=-=-                                                   
#

scoreboard objectives add amethyst dummy
scoreboard objectives modify amethyst displayname {"text":"Amethyst internals","color":"light_purple"}
scoreboard players reset * amethyst

scoreboard objectives add amethyst_const dummy
scoreboard objectives modify amethyst_const displayname {"text":"Amethyst constants","color":"light_purple"}
scoreboard players reset * amethyst_const

data remove storage amethyst:internal _out
data remove storage amethyst:internal _argv

# ["",{"text":"Constant: ","color":"gray"},{"text":"100","color":"red","bold":true}]