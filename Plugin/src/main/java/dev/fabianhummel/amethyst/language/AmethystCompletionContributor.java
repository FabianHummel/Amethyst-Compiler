package dev.fabianhummel.amethyst.language;

import com.intellij.codeInsight.completion.*;
import dev.fabianhummel.amethyst.AmethystParser;
import dev.fabianhummel.amethyst.language.completions.ExpressionCompletionProvider;
import dev.fabianhummel.amethyst.psi.AmethystPsiNode;
import org.antlr.intellij.adaptor.lexer.PSIElementTypeFactory;

import static com.intellij.patterns.PlatformPatterns.psiElement;

public class AmethystCompletionContributor extends CompletionContributor {
    AmethystCompletionContributor() {
        var expressionToken = PSIElementTypeFactory
                .getRuleIElementTypes(AmethystLanguage.INSTANCE)
                .get(AmethystParser.RULE_expression);

        var pattern = psiElement(AmethystPsiNode.class).withElementType(expressionToken);

        extend(CompletionType.BASIC, psiElement().withParent(pattern), new ExpressionCompletionProvider());
    }
}
