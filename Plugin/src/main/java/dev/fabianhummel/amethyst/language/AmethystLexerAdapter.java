package dev.fabianhummel.amethyst.language;

import dev.fabianhummel.amethyst.AmethystLexer;
import org.antlr.intellij.adaptor.lexer.ANTLRLexerAdaptor;

public class AmethystLexerAdapter extends ANTLRLexerAdaptor {
    public AmethystLexerAdapter() {
        super(AmethystLanguage.INSTANCE, new AmethystLexer(null));
    }
}
