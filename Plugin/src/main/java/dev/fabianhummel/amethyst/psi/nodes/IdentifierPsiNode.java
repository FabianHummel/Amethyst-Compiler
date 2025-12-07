package dev.fabianhummel.amethyst.psi.nodes;

import org.antlr.intellij.adaptor.psi.ANTLRPsiLeafNode;
import org.antlr.intellij.adaptor.psi.ANTLRPsiNode;

public class IdentifierPsiNode extends ANTLRPsiNode {
    public IdentifierPsiNode(ANTLRPsiLeafNode node) {
        super(node);
    }
}
