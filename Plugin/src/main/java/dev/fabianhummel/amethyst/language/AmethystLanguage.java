package dev.fabianhummel.amethyst.language;

import com.intellij.lang.Language;

public class AmethystLanguage extends Language {
    public static final AmethystLanguage INSTANCE = new AmethystLanguage();

    private AmethystLanguage() {
        super("Amethyst");
    }
}
