package dev.fabianhummel.amethyst.language;

import com.intellij.openapi.fileTypes.LanguageFileType;
import dev.fabianhummel.amethyst.icons.AmethystIcons;
import org.jetbrains.annotations.NotNull;

import javax.swing.*;

public final class AmethystFileType extends LanguageFileType {

    public static final AmethystFileType INSTANCE = new AmethystFileType();

    private AmethystFileType() {
        super(AmethystLanguage.INSTANCE);
    }

    @NotNull
    @Override
    public String getName() {
        return "Amethyst File";
    }

    @NotNull
    @Override
    public String getDescription() {
        return "Amethyst language file";
    }

    @NotNull
    @Override
    public String getDefaultExtension() {
        return "amy";
    }

    @Override
    public Icon getIcon() {
        return AmethystIcons.FILE;
    }
}