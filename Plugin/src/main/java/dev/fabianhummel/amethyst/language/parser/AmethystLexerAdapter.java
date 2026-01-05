package dev.fabianhummel.amethyst.language.parser;

import dev.fabianhummel.amethyst.AmethystLexer;
import dev.fabianhummel.amethyst.language.AmethystLanguage;
import org.antlr.intellij.adaptor.lexer.ANTLRLexerAdaptor;

public class AmethystLexerAdapter extends ANTLRLexerAdaptor {
    public AmethystLexerAdapter() {
        super(AmethystLanguage.INSTANCE, new AmethystLexer(null));
    }
}
