<br>
<img src="Assets/text.png" height="140" alt="Amethyst Compiler Logo" align="right" />

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
```

## Batteries Included

Amethyst is not just for writing datapack code, but is also great for managing all resource pack needs.

## Usage

The project structure is as simple as it gets, no need for complicated setups or configurations. Just write your Amethyst code in the `src` directory and run the compiler. The output will be a fully functional datapack. Source files end in `.amy`. Examples can be found in the `Examples` directory of this repository.

```text
amethyst.toml
src/
    *.amy
```

## Configuration

The `amethyst.toml` file is used to configure the compiler. It allows you to specify the output directory, the namespace, and other settings. Here's an example configuration file:

```toml
namespace = "my_datapack"
description = "My first Amethyst datapack"
pack_format = 18
output = "out"
```

## Contributing

Contributions are welcome! If you have any ideas, suggestions, or bug reports, feel free to open an issue or a pull request. A few things to keep in mind when changing the codebase:

- Language features are defined in the AstModel.txt file and use a proprietary syntax. This file is used to generate the parser and the AST. If you want to add a new feature, make sure to update the [AstModel](Amethyst/AstModel.txt) file and run the roslyn source generator to update the parser and the AST.
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
declaration    → funcDecl
               | varDecl 
               | statement ;
               
funcDecl       → ( "initializing" | "ticking" )* "function" function ;
               
varDecl        → "var" IDENTIFIER ( "=" expression )? ";" ;
```

### Statements
```text
statement      → exprStmt
               | forStmt
               | ifStmt
               | printStmt
               | returnStmt
               | whileStmt
               | block ;
               
exprStmt       → expression ";" ;

forStmt        → "for" "(" ( varDecl | exprStmt | ";" )
               expression? ";"
               expression? ")" statement ;

ifStmt         → "if" "(" expression ")" statement
               ( "else" statement )? ;

printStmt      → "print" expression ";" ;

returnStmt     → "return" expression? ";" ;

whileStmt      → "while" "(" expression ")" statement ;
               
block          → "{" declaration* "}" ;
```

### Expressions
```text
expression     → assignment ;

assignment     → IDENTIFIER "=" assignment
               | logic_or ;
              
logic_or       → logic_and ( "or" logic_and )* ;

logic_and      → equality ( "and" equality )* ;

equality       → comparison ( ( "!=" | "==" ) comparison )* ;

comparison     → term ( ( ">" | ">=" | "<" | "<=" ) term )* ;

term           → factor ( ( "-" | "+" ) factor )* ;

factor         → unary ( ( "/" | "*" ) unary )* ;

unary          → ( "!" | "-" ) unary | call ;
               
call           → primary ( "(" arguments? ")" )* ;
               
primary        → NUMBER | STRING | "true" | "false" | "null"
               | "(" expression ")"
               | IDENTIFIER ;
```

### Utility rules
```text
function       → IDENTIFIER "(" parameters? ")" block ;

parameters     → IDENTIFIER ( "," IDENTIFIER )* ;

arguments      → expression ( "," expression )* ;
```