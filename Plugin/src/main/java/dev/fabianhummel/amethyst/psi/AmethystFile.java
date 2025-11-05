package dev.fabianhummel.amethyst.psi;

import com.intellij.extapi.psi.PsiFileBase;
import com.intellij.openapi.fileTypes.FileType;
import com.intellij.psi.FileViewProvider;
import com.intellij.psi.PsiElement;
import dev.fabianhummel.amethyst.icons.AmethystIcons;
import dev.fabianhummel.amethyst.language.AmethystFileType;
import dev.fabianhummel.amethyst.language.AmethystLanguage;
import org.jetbrains.annotations.NotNull;
import org.jetbrains.annotations.Nullable;

import javax.swing.*;

public class AmethystFile extends PsiFileBase {
    public AmethystFile(@NotNull FileViewProvider viewProvider) {
        super(viewProvider, AmethystLanguage.INSTANCE);
    }

    @NotNull
    @Override
    public FileType getFileType() {
        return AmethystFileType.INSTANCE;
    }

    @Override
    public String toString() {
        return "Amethyst File";
    }

    @Override
    public @Nullable Icon getIcon(int flags) {
        return AmethystIcons.FILE;
    }
}