package dev.fabianhummel.amethyst.language.highlight;

import com.intellij.openapi.editor.colors.TextAttributesKey;
import com.intellij.openapi.fileTypes.SyntaxHighlighter;
import com.intellij.openapi.options.colors.AttributesDescriptor;
import com.intellij.openapi.options.colors.ColorDescriptor;
import com.intellij.openapi.options.colors.ColorSettingsPage;
import com.intellij.openapi.util.NlsContexts;
import dev.fabianhummel.amethyst.icons.AmethystIcons;
import org.jetbrains.annotations.NonNls;
import org.jetbrains.annotations.NotNull;
import org.jetbrains.annotations.Nullable;

import javax.swing.*;
import java.util.Map;

public class AmethystColorSettingsPage implements ColorSettingsPage {
    private static final AttributesDescriptor[] DESCRIPTORS = new AttributesDescriptor[] {
              new AttributesDescriptor("Dot", AmethystSyntaxHighlighter.DOT),
              new AttributesDescriptor("Comma", AmethystSyntaxHighlighter.COMMA),
              new AttributesDescriptor("Semicolon", AmethystSyntaxHighlighter.SEMICOLON),
              new AttributesDescriptor("Parentheses", AmethystSyntaxHighlighter.PARENTHESES),
              new AttributesDescriptor("Braces", AmethystSyntaxHighlighter.BRACES),
              new AttributesDescriptor("Brackets", AmethystSyntaxHighlighter.BRACKETS),
              new AttributesDescriptor("Operation", AmethystSyntaxHighlighter.OPERATION),
              new AttributesDescriptor("Preprocessor declaration", AmethystSyntaxHighlighter.PREPROCESSOR_DECLARATION),
              new AttributesDescriptor("Preprocessor statement", AmethystSyntaxHighlighter.PREPROCESSOR_STATEMENT),
              new AttributesDescriptor("Preprocessor boolean literal", AmethystSyntaxHighlighter.PREPROCESSOR_BOOLEAN),
              new AttributesDescriptor("Preprocessor datatype", AmethystSyntaxHighlighter.PREPROCESSOR_DATATYPE),
              new AttributesDescriptor("Declaration", AmethystSyntaxHighlighter.DECLARATION),
              new AttributesDescriptor("Statement", AmethystSyntaxHighlighter.STATEMENT),
              new AttributesDescriptor("Boolean literal", AmethystSyntaxHighlighter.BOOLEAN),
              new AttributesDescriptor("Datatype", AmethystSyntaxHighlighter.DATATYPE),
              new AttributesDescriptor("Selector", AmethystSyntaxHighlighter.SELECTOR),
              new AttributesDescriptor("Comment", AmethystSyntaxHighlighter.COMMENT),
              new AttributesDescriptor("Command", AmethystSyntaxHighlighter.COMMAND),
              new AttributesDescriptor("String literal", AmethystSyntaxHighlighter.STRING),
              new AttributesDescriptor("Resource literal", AmethystSyntaxHighlighter.RESOURCE),
              new AttributesDescriptor("Number", AmethystSyntaxHighlighter.NUMBER),
              new AttributesDescriptor("Identifier", AmethystSyntaxHighlighter.IDENTIFIER),
    };

    @Override
    public @Nullable Icon getIcon() {
        return AmethystIcons.FILE;
    }

    @Override
    public @NotNull SyntaxHighlighter getHighlighter() {
        return new AmethystSyntaxHighlighter();
    }

    @Override
    public @NonNls @NotNull String getDemoText() {
        return """
            FROM `namespace:path/to/resource` IMPORT symbol1;
            
            [unit_test]
            function my_function(param1: int, raw_location: scoreboard $test my_objective) -> dec(3) {
                /execute as @a at @s run say Hello World!
                var result = call_other_function(123.456, true);
                # what a nice comment
                var x = "Hello " + "World".length + 1;
                if (x.length >= 6) {
                    return true + @s.SelectedItem.Count + 12.459;
                }
                return 0;
            }
            
            RESOURCE `amethyst_scepter` {
                "texture": `minecraft:items/wooden_shovel`,
                "model": `custom:models/amethyst_scepter`
            }
            """;
    }

    @Override
    public @Nullable Map<String, TextAttributesKey> getAdditionalHighlightingTagToDescriptorMap() {
        return null;
    }

    @Override
    public AttributesDescriptor @NotNull [] getAttributeDescriptors() {
        return DESCRIPTORS;
    }

    @Override
    public ColorDescriptor @NotNull [] getColorDescriptors() {
        return ColorDescriptor.EMPTY_ARRAY;
    }

    @Override
    public @NotNull @NlsContexts.ConfigurableName String getDisplayName() {
        return "Amethyst";
    }
}
