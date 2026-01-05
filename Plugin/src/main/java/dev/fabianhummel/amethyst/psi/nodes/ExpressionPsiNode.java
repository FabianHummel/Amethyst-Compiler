package dev.fabianhummel.amethyst.psi.nodes;

import com.intellij.lang.ASTNode;
import dev.fabianhummel.amethyst.psi.AmethystPsiNode;
import org.jetbrains.annotations.NotNull;

public class ExpressionPsiNode extends AmethystPsiNode {
    public ExpressionPsiNode(@NotNull ASTNode node) {
        super(node);
    }
}
