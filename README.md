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

The project structure is as simple as it gets, no need for complicated setups or configurations. Just write your Amethyst code in the `src` directory and run the compiler. The output will be a fully functional datapack. Examples can be found in the `Examples` directory of this repository.

```text
amethyst.toml
src/
    *.amy
```

## Configuration

The `amethyst.toml` file is used to configure the compiler. It allows you to specify the output directory, the namespace, and other settings. Here's an example of a `amethyst.toml` file:

```toml
namespace = "my_datapack"
description = "My first Amethyst datapack"
pack_format = 18
output = "out"
```