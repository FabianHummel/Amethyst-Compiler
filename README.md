<br>
<img src="Assets/logo.png" alt="Amethyst Compiler Logo" align="right" />

# The Amethyst Compiler Platform <br>
> A modern, refreshing way of writing Minecraft datapacks

## Introduction

Datapacks are Minecraft's way of scripting custom logic and content into the game. They are written in Mojang's own language MCFunction (or MCF in short) which happens to be a very verbose and limited language. Amethyst tries to solve this problem by providing a modern, refreshing way of writing Minecraft datapacks through high-level syntax and perfect integration with the game.

## Features

Like many modern programming languages, Amethyst abstracts away the low-level details of MCF, and instead provides readable and expressive syntax. To get a glimpse of what an Amethyst code snipped could look like, here's a simple example:

```amethyst
initializing function init() {
    
}

ticking function tick() {
    
}

namespace math {
    
}
```

Note the keywords `initializing` and `ticking` which are used to define special attributes of the function. `ticking` automatically adds the function to the `tick.json` function tag and `initializing` to the `load.json`. This syntax perfectly integrates with the game and allows for a more natural way of writing datapacks.

## Preprocessing

A key feature of any modern language is preprocessing of source files. For example, if you simply want to repeat a command with a compile-time constant output, you can opt out of the runtime environment through a `preprocess` group. All variables and expressions are then evaluated at compile time. Runtime variables from an outer scope simply get replaced with their intermediary location (either a scoreboard player's name or an nbt path to a storage value)

```amethyst
tellraw @a "It's raining diamonds!";
    
preprocess {
    for (var x = 0; x < 5; x++) {
        schedule x seconds {
            give @a diamond;
        }
    }
}
```

<details>
  <summary>Resulting MCFunction code</summary>

```mcfunction
# my_datapack:_amethyst_init

tellraw @a "It's raining diamonds!"
schedule function "my_datapack:_amethyst_init/anon_schedule_1" 0s
schedule function "my_datapack:_amethyst_init/anon_schedule_1" 1s
schedule function "my_datapack:_amethyst_init/anon_schedule_1" 2s
schedule function "my_datapack:_amethyst_init/anon_schedule_1" 3s
schedule function "my_datapack:_amethyst_init/anon_schedule_1" 4s
schedule function "my_datapack:_amethyst_init/anon_schedule_1" 5s
```
```mcfunction
# my_datapack:init/anon_schedule_1

give @a diamond
```
    
</details>


## Inlining

What really gives Amethyst superpowers is that you can inline any MCF code in any location you like. This is useful for several reasons:
- Some features of the compiler have not yet been implemented -> simply write the MCF code in the place you like and it will be included in the output; but be aware that you have to adhere to some standard definitions of the compiler.
- Quickly outline code that is cumbersome to write via Amethyst otherwise.

Inline code is enabled either via a standard minecraft console command with a `/` prefix or an `inline` group. Everything after a `/` or inside an inline group is considered **plaintext**, with no dynamic variables.

```amethyst
var name = "Amethyst";                  # Silly example that would generate way too bloated code.
tellraw @a "Hello from " + name;        # Better use constants, inlining or preprocessing in this case.

/tellraw @a "Hello from MCFunction"

inline {
    tellraw @a "My feet are magic"
    execute as @a at @s run setblock @s ~ ~-1 ~ minecraft:gold
}
```

<details>
<summary>Resulting MCFunction code</summary>

```mcfunction
# my_datapack:_amethyst_init

data modify storage amethyst:constants c1 set value "Hello from "
data modify storage amethyst:interal vname set value "Amethyst"
data modify storage amethyst:interal _out set from storage amethyst:constants c1
data modify storage amethyst:interal _tmp set from storage amethyst:interal vname
function amethyst:internal/string_concat with storage amethyst:interal
tellraw @a {"storage":"amethyst:internal","nbt":"_out"}
tellraw @a "Hello from MCFunction"
tellraw @a "My feet are magic"
execute as @a at @s run setblock @s ~ ~-1 ~ minecraft:gold
```

</details>

## Batteries Included

Amethyst is not just for writing datapack code, but is also great for managing all resource pack needs. A resource pack can be defined in the same project as the datapack and gets rid of the tedious process of managing two separate folders at once. Names and values can also include preprocessed variables to supercharge resource pack development, although they need to be wrapped inside a `resource` group _(acts like an ordinary `preprocess` group)_ to be able to create preprocess variables. 

```amethyst
resource {
    for (var i = 0; i < 5; i++) {
        resource item model "item_" + x {
            "layer0": "item/my_item_" + x
        }
    }
    
    block model "my_block" {
        "parent": "block/cube_all",
        "textures": {
            "all": "block/my_block"
        }
    }
}

resource sound "my_sound" {
    "sounds": [
        {
            "name": "my_sound",
            "stream": "true",
        }
    ]
}
```

Other from defining resources directly inside Amethyst code, you can also place your resource pack inside the `res` directory of your project. The compiler will automatically merge this template with the processed amethyst code and place the output in the configured location.

## Usage

The project structure is as simple as it gets, no need for complicated setups or configurations. Just write your Amethyst code in the `src` directory and run the compiler. The output will be a fully functional datapack. Source files end in `.amy`. Examples can be found in the `Examples` directory of this repository.

```text
amethyst.toml
res/
    pack.mcmeta
    assets/
        <any normal resource pack content>
src/
    *.amy
```

## Configuration

The `amethyst.toml` file is used to configure the compiler. It allows you to specify the output directory, the namespace, and other settings. Here's an example configuration file:

```toml
minecraft_root = "<path to .minecraft>"             # Root directory of the minecraft instance
                                                    # Amethyst tries to find this folder automatically by searching the default location
                                                    # (adjust if using a launcher, otherwise remove this property)

[Datapack]
namespace = "my_datapack"                           # Root namespace of the datapack
description = "My first Amethyst datapack"          # Datapack description
pack_format = 18                                    # Any number, 'release' or 'snapshot'
output = "out"                                      # Datapack output directory (preferably a world's datapacks folder). 
                                                    #   Default: '.minecraft > datapacks' if minecraft_root is specified

[Resourcepack]
namespace = "my_resourcepack"                       # Root namespace of the resourcepack
description = "My first Amethyst resourcepack"      # Resourcepack description
pack_format = 18                                    # Any number, 'release' or 'snapshot'
output = "out"                                      # Datapack output directory (preferably a world's datapacks folder).
                                                    #   Default: '.minecraft > resourcepacks' if minecraft_root is specified
```

## Contributing

Contributions are welcome! If you have any ideas, suggestions, or bug reports, feel free to open an issue or a pull request. A few things to keep in mind when changing the codebase:

- Language features are defined in the [AstModel](Amethyst/AstModel.toml) file and uses TOML syntax. This file is used to generate the parser and the AST. If you want to add a new feature, make sure to update the [AstModel](Amethyst/AstModel.toml) file and run the roslyn source generator to update the parser and the AST.
- Tokenization happens in the [Tokenizer](Amethyst/Tokenizer.cs) class. If you want to add a new token, add it to the `TokenType` enum and don't forget to update the keyword list and / or the `ScanToken` method.
- Parsing happens in the [Parser](Amethyst/Parser.cs) class. If you want to add a new language feature, think about how it will be represented in the AST by adjusting the [AST reference](#reference) below and then update methods in the class to handle the new feature.
- Code Generation happens in the [Compiler](Amethyst/Compiler.cs) class. Changes in the AST will force you to update the `Compile` method to convert the AST to valid MCF code. Note that values represented through variables (scoreboard or storage) or function calls are always placed into an intermediate location `_out` before being used in the final command. This is to avoid complex and repetitive code with nested commands at the cost of a few extra commands (and performance 😔).

## Reference

Below is the AST reference for Amethyst. It is a work in progress and will be updated as the language evolves. The schema is inspired by [Crafting Interpreters](https://craftinginterpreters.com/).

```text
program        → declaration* EOF ;
```

### Declarations
```text
declaration    → nsDecl 
               | funcDecl
               | varDecl 
               | statement ;

nsDecl         → "namespace" STRING block ;

funcDecl       → ( "initializing" | "ticking" )* "function" function ;

varDecl        → "var" IDENTIFIER ( "=" expression )? ";" ;
```

### Statements
```text
statement      → exprStmt
               | forStmt
               | ifStmt
               | printStmt
               | commentStmt
               | outStmt
               | whileStmt
               | block ;

exprStmt       → expression ";" ;

forStmt        → "for" "(" ( varDecl | exprStmt | ";" ) expression? ";" expression? ")" statement ;

ifStmt         → "if" "(" expression ")" statement ( "else" statement )? ;

printStmt      → "print" expression ";" ;

commentStmt    → "comment" STRING ";" ;

outStmt        → "out" expression ";" ;

whileStmt      → "while" "(" expression ")" statement ;

block          → "{" declaration* "}" ;
```

### Expressions
```text
expression     → assignment ;

assignment     → IDENTIFIER "=" assignment
               | logic_or ;

mapping        → expression ":" expression ;

logic_or       → logic_and ( "or" logic_and )* ;

logic_and      → equality ( "and" equality )* ;

equality       → comparison ( ( "!=" | "==" ) comparison )* ;

comparison     → term ( ( ">" | ">=" | "<" | "<=" ) term )* ;

term           → factor ( ( "-" | "+" ) factor )* ;

factor         → unary ( ( "/" | "*" ) unary )* ;

unary          → ( "!" | "-" ) unary | call ;

call           → primary ( "(" arguments? ")" )* ;

primary        → NUMBER | STRING | "true" | "false" | "null"
               | "(" expression ")" | object | array
               | IDENTIFIER ;
```

### Utility rules
```text
function       → IDENTIFIER "(" parameters? ")" block ;

parameters     → IDENTIFIER ( "," IDENTIFIER )* ;

arguments      → expression ( "," expression )* ;

object         → "{" ( mapping ( "," mapping )* )? "}" ;

array          → "[" ( expression ( "," expression )* )? "]" ;
```