### How to set up the recommended configuration:

1. Place `server.jar` into `Tests > Server`. Of course, you can also supply other servers like Spigot, Paper, etc. I haven't tested, but it should work.

2. Place `eula.txt` into `Tests > Server` set `eula=true`.

3. Check if the path to the `server.jar` relative to `Tests > Server` is correct in the `appsettings.json` file. If followed the steps above, the value should be `"server.jar"`. Alternatively, you can also create a `appsettings.Development.json` file to use values that are not tracked by VCS.

4. Run the unit tests and make sure all tests pass. Keep in mind that the code runs on an actual Minecraft server, so it could take a while.

> [!IMPORTANT]
> Don't modify the `server.properties.template` file, it's used to automatically generate the `server.properties` file.

> [!TIP]
> Pro-tip for contributing to unit tests: Amethyst projects can be reused by adding variables `{{xyz}}` everywhere within the codebase, which will be expanded upon compilation. This allows multiple test cases with different data to be generated from a single project.