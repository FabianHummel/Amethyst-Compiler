package dev.fabianhummel.amethyst.psi;

import com.intellij.psi.tree.TokenSet;
import dev.fabianhummel.amethyst.AmethystLexer;
import dev.fabianhummel.amethyst.language.AmethystLanguage;
import org.antlr.intellij.adaptor.lexer.PSIElementTypeFactory;

public interface AmethystTokenSets {

    TokenSet COMMENTS = PSIElementTypeFactory.createTokenSet(
            AmethystLanguage.INSTANCE,
            AmethystLexer.COMMENT);

    TokenSet SPACE = PSIElementTypeFactory.createTokenSet(
            AmethystLanguage.INSTANCE,
            AmethystLexer.WS);

    TokenSet STRINGS = PSIElementTypeFactory.createTokenSet(
            AmethystLanguage.INSTANCE,
            AmethystLexer.STRING);
}
