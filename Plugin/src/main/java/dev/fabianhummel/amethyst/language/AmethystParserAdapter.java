package dev.fabianhummel.amethyst.language;

import com.intellij.psi.tree.IElementType;
import dev.fabianhummel.amethyst.AmethystParser;
import org.antlr.intellij.adaptor.parser.ANTLRParserAdaptor;
import org.antlr.v4.runtime.Parser;
import org.antlr.v4.runtime.tree.ParseTree;

public class AmethystParserAdapter extends ANTLRParserAdaptor {
    public AmethystParserAdapter() {
        super(AmethystLanguage.INSTANCE, new AmethystParser(null));
    }

    @Override
    protected ParseTree parse(Parser parser, IElementType root) {
        return ((AmethystParser) parser).file();
    }
}
