# Created using Amethyst v1.0.1.0 on 2025-07-09.
tellraw @a ["",{"text":"DEBUG: ","color":"dark_gray"},["[",{"text":"1","color":"gold"},", ",{"text":"2","color":"gold"},", ",{"text":"3","color":"gold"},", ",["{",{"text":"okey","color":"aqua"},": ",["\"",{"text":"nice","color":"green"},"\""],", ",{"text":"nice","color":"aqua"},": ",["[",{"text":"1","color":"gold"},", ",[{"text":"1","color":"gold"},{"text":"b","color":"red"}],", ",["\"",{"text":"Helo","color":"green"},"\""],", ","[]",", ","{}",", ",["\"",{"text":"nice","color":"green"},"\""],"]"],"}"],"]"]]
data modify storage amethyst: 1 set value [1,2,3,{keys:["okey","nice"],data:{"okey":"nice","nice":[1,1b,"Helo",[],{keys:[],data:{}},"nice"]}}]
data modify storage amethyst:internal string.stringify.in set from storage amethyst: 1
function amethyst:api/string/stringify
tellraw @a ["",{"text":"DEBUG: ","color":"dark_gray"},{"nbt":"string.stringify.out","storage":"amethyst:internal","interpret":true}]
