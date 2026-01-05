package dev.fabianhummel.amethyst.psi;

import com.intellij.lang.ASTNode;
import com.intellij.psi.PsiElement;
import com.intellij.psi.PsiNamedElement;
import com.intellij.util.IncorrectOperationException;
import dev.fabianhummel.amethyst.language.AmethystLanguage;
import dev.fabianhummel.amethyst.language.parser.AmethystParserDefinition;
import org.antlr.intellij.adaptor.psi.ANTLRPsiNode;
import org.antlr.intellij.adaptor.psi.Trees;
import org.jetbrains.annotations.NonNls;
import org.jetbrains.annotations.NotNull;

public class AmethystPsiNode extends ANTLRPsiNode implements PsiNamedElement {
    protected String name = null; // an override to input text ID if we rename via intellij

    public AmethystPsiNode(final @NotNull ASTNode node) {
        super(node);
    }

    @Override
    public String getName() {
        if (name != null) return name;
        return getText();
    }

    @Override
    public PsiElement setName(@NonNls @NotNull String name) throws IncorrectOperationException {
		/* From doc: Creating a fully correct AST node from scratch is
              quite difficult. Thus, surprisingly, the easiest way to
              get the replacement node is to create a dummy file in the
              custom language so that it would contain the necessary
              node in its parse tree, build the parse tree and
              extract the necessary node from it.
		 */
        System.out.println("rename "+this+" to "+name);
        PsiElement newNode = Trees.createLeafFromText(getProject(),
                AmethystLanguage.INSTANCE,
                getContext(),
                name, AmethystParserDefinition.ID);
        if (newNode != null) {
            this.replace(newNode);
            this.name = name;
            return this;
        }
        throw new IncorrectOperationException();
    }
}
