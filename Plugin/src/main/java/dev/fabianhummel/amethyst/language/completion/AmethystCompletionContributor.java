package dev.fabianhummel.amethyst.language.completion;

import com.intellij.codeInsight.completion.*;
import dev.fabianhummel.amethyst.language.parser.AmethystParserDefinition;
import dev.fabianhummel.amethyst.psi.nodes.ExpressionPsiNode;

import static com.intellij.patterns.PlatformPatterns.psiElement;

public class AmethystCompletionContributor extends CompletionContributor {
    AmethystCompletionContributor() {
        extend(
                CompletionType.BASIC,
                psiElement().inside(ExpressionPsiNode.class),
                new ExpressionCompletionProvider()
        );

        extend(
                CompletionType.BASIC,
                psiElement().inside(psiElement(AmethystParserDefinition.FILE)),
                new KeywordCompletionProvider()
        );
    }
}
