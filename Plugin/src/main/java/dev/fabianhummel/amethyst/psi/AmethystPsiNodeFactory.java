package dev.fabianhummel.amethyst.psi;

import com.intellij.lang.ASTNode;
import com.intellij.psi.tree.IElementType;
import dev.fabianhummel.amethyst.psi.nodes.ExpressionPsiNode;
import dev.fabianhummel.amethyst.psi.nodes.IdentifierPsiNode;
import org.antlr.intellij.adaptor.psi.ANTLRPsiNode;

import java.util.HashMap;
import java.util.Map;

public class AmethystPsiNodeFactory {
    private static final Map<IElementType, Class<? extends ANTLRPsiNode>> map = new HashMap<>();

    static {
        map.put(AmethystTypes.IDENTIFIER, IdentifierPsiNode.class);
        map.put(AmethystTypes.EXPRESSION, ExpressionPsiNode.class);
    }

    public static ANTLRPsiNode create(ASTNode astNode) {
        if (map.containsKey(astNode.getElementType())) {
            try {
                return map.get(astNode.getElementType())
                        .getConstructor(ASTNode.class)
                        .newInstance(astNode);
            } catch (ReflectiveOperationException ignored) {
            }
        }
        return new ANTLRPsiNode(astNode);
    }
}
