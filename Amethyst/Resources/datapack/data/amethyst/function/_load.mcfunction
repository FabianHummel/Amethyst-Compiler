#     
#     ______     __    __     ______    ______    __  __     __  __     ______     ______  
#    /\  __ \   /\ "-./  \   /\  ___\  /\__  _\  /\ \_\ \   /\ \_\ \   /\  ___\   /\__  _\ 
#    \ \  __ \  \ \ \-./\ \  \ \  __\  \/_/\ \/  \ \  __ \  \ \____ \  \ \___  \  \/_/\ \/ 
#     \ \_\ \_\  \ \_\ \ \_\  \ \_____\   \ \_\   \ \_\ \_\  \/\_____\  \/\_____\    \ \_\ 
#      \/_/\/_/   \/_/  \/_/   \/_____/    \/_/    \/_/\/_/   \/_____/   \/_____/     \/_/ 
#                                                                                          
#     -=-=-= MCFUNCTION TRANSPILER =-=-=-=-=-=-=-=-=-                                                   
#

scoreboard objectives add amethyst dummy
scoreboard objectives modify amethyst displayname {"text":"Amethyst internals","color":"light_purple"}
scoreboard players reset * amethyst

scoreboard objectives add amethyst_const dummy
scoreboard objectives modify amethyst_const displayname {"text":"Amethyst constants","color":"light_purple"}
scoreboard players reset * amethyst_const

scoreboard objectives add amethyst_test dummy
scoreboard objectives modify amethyst_test displayname {"text":"Amethyst unit tests","color":"light_purple"}
scoreboard players reset * amethyst_test

scoreboard objectives add amethyst_record_initializers dummy
scoreboard objectives modify amethyst_record_initializers displayname {"text":"Amethyst record initializers","color":"light_purple"}
# Don't reset this one, it's used to keep track of the initializers, even after restarting the server
# scoreboard players reset * amethyst_record_initializers

# ["",{"text":"Constant: ","color":"gray"},{"text":"100","color":"red","bold":true}]