package dev.fabianhummel.amethyst.psi;

import dev.fabianhummel.amethyst.AmethystLexer;
import dev.fabianhummel.amethyst.AmethystParser;
import dev.fabianhummel.amethyst.language.AmethystLanguage;
import org.antlr.intellij.adaptor.lexer.PSIElementTypeFactory;
import org.antlr.intellij.adaptor.lexer.RuleIElementType;
import org.antlr.intellij.adaptor.lexer.TokenIElementType;

import java.util.List;

public class AmethystTypes {

    private static final List<RuleIElementType> _ruleIElementTypes = PSIElementTypeFactory.getRuleIElementTypes(AmethystLanguage.INSTANCE);
    private static final List<TokenIElementType> _tokenIElementTypes = PSIElementTypeFactory.getTokenIElementTypes(AmethystLanguage.INSTANCE);

    public static RuleIElementType EXPRESSION = _ruleIElementTypes.get(AmethystParser.RULE_expression);
    public static TokenIElementType IDENTIFIER = _tokenIElementTypes.get(AmethystLexer.IDENTIFIER);
}
