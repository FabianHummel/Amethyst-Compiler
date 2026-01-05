package dev.fabianhummel.amethyst.icons;

import com.intellij.ide.FileIconProvider;
import com.intellij.openapi.project.Project;
import com.intellij.openapi.vfs.VirtualFile;
import org.jetbrains.annotations.NotNull;
import org.jetbrains.annotations.Nullable;

import javax.swing.*;

public class AmethystFileIconProvider implements FileIconProvider {

    @Override
    public @Nullable Icon getIcon(@NotNull VirtualFile file, int flags, @Nullable Project project) {
        if (project == null || !project.isOpen()) {
            return null;
        }

        if (file.getName().equals("amethyst.toml") && file.getParent().getPath().equals(project.getBasePath())) {
            return AmethystIcons.PROJECT;
        }

        return null;
    }
}
