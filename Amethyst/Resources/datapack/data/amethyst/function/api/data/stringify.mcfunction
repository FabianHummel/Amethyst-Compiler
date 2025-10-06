# Convert any data structure to JSON text components including syntax highlighting.

[no_mangle]
function stringify([no_mangle("data.stringify.in")] input: unknown): string
{
    /data modify storage amethyst:internal data.stringify.out set value []
    /data modify storage amethyst:internal data.stringify._stack set value []
    /data modify storage amethyst:internal data.stringify._stack append from storage amethyst:internal %input%
    /function amethyst:internal/data/stringify/loop
    
    return storage data.stringify.out
}