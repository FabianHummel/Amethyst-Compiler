package dev.fabianhummel.amethyst.language.highlight;

import com.intellij.lexer.Lexer;
import com.intellij.openapi.editor.DefaultLanguageHighlighterColors;
import com.intellij.openapi.editor.colors.TextAttributesKey;
import com.intellij.openapi.fileTypes.SyntaxHighlighterBase;
import com.intellij.psi.tree.IElementType;
import dev.fabianhummel.amethyst.AmethystLexer;
import dev.fabianhummel.amethyst.AmethystParser;
import dev.fabianhummel.amethyst.language.AmethystLanguage;
import dev.fabianhummel.amethyst.language.parser.AmethystLexerAdapter;
import org.antlr.intellij.adaptor.lexer.PSIElementTypeFactory;
import org.antlr.intellij.adaptor.lexer.TokenIElementType;
import org.jetbrains.annotations.NotNull;

import static com.intellij.openapi.editor.colors.TextAttributesKey.createTextAttributesKey;

public class AmethystSyntaxHighlighter extends SyntaxHighlighterBase {
    private static final TextAttributesKey[] EMPTY_KEYS = new TextAttributesKey[0];

    public static final TextAttributesKey DOT =
            createTextAttributesKey("AMETHYST_DOT", DefaultLanguageHighlighterColors.DOT);
    public static final TextAttributesKey COMMA =
            createTextAttributesKey("AMETHYST_COMMA", DefaultLanguageHighlighterColors.COMMA);
    public static final TextAttributesKey SEMICOLON =
            createTextAttributesKey("AMETHYST_SEMICOLON", DefaultLanguageHighlighterColors.SEMICOLON);
    public static final TextAttributesKey PARENTHESES =
            createTextAttributesKey("AMETHYST_PARENTHESES", DefaultLanguageHighlighterColors.PARENTHESES);
    public static final TextAttributesKey BRACES =
            createTextAttributesKey("AMETHYST_BRACES", DefaultLanguageHighlighterColors.BRACES);
    public static final TextAttributesKey BRACKETS =
            createTextAttributesKey("AMETHYST_BRACKETS", DefaultLanguageHighlighterColors.BRACKETS);
    public static final TextAttributesKey OPERATION =
            createTextAttributesKey("AMETHYST_OPERATION", DefaultLanguageHighlighterColors.OPERATION_SIGN);

    public static final TextAttributesKey PREPROCESSOR_DECLARATION =
            createTextAttributesKey("AMETHYST_PREPROCESSOR_DECLARATION", DefaultLanguageHighlighterColors.FUNCTION_DECLARATION);
    public static final TextAttributesKey PREPROCESSOR_STATEMENT =
            createTextAttributesKey("AMETHYST_PREPROCESSOR_STATEMENT", DefaultLanguageHighlighterColors.FUNCTION_DECLARATION);
    public static final TextAttributesKey PREPROCESSOR_BOOLEAN =
            createTextAttributesKey("AMETHYST_PREPROCESSOR_BOOLEAN", DefaultLanguageHighlighterColors.FUNCTION_DECLARATION);
    public static final TextAttributesKey PREPROCESSOR_DATATYPE =
            createTextAttributesKey("AMETHYST_PREPROCESSOR_DATATYPE", DefaultLanguageHighlighterColors.FUNCTION_DECLARATION);

    public static final TextAttributesKey DECLARATION =
            createTextAttributesKey("AMETHYST_DECLARATION", DefaultLanguageHighlighterColors.KEYWORD);
    public static final TextAttributesKey STATEMENT =
            createTextAttributesKey("AMETHYST_STATEMENT", DefaultLanguageHighlighterColors.KEYWORD);
    public static final TextAttributesKey BOOLEAN =
            createTextAttributesKey("AMETHYST_BOOLEAN", DefaultLanguageHighlighterColors.KEYWORD);
    public static final TextAttributesKey DATATYPE =
            createTextAttributesKey("AMETHYST_DATATYPE", DefaultLanguageHighlighterColors.KEYWORD);
    public static final TextAttributesKey SELECTOR =
            createTextAttributesKey("AMETHYST_SELECTOR", DefaultLanguageHighlighterColors.NUMBER);

    public static final TextAttributesKey COMMENT =
            createTextAttributesKey("AMETHYST_COMMENT", DefaultLanguageHighlighterColors.LINE_COMMENT);
    public static final TextAttributesKey COMMAND =
            createTextAttributesKey("AMETHYST_COMMAND", DefaultLanguageHighlighterColors.DOC_COMMENT);
    public static final TextAttributesKey STRING =
            createTextAttributesKey("AMETHYST_STRING", DefaultLanguageHighlighterColors.STRING);
    public static final TextAttributesKey RESOURCE =
            createTextAttributesKey("AMETHYST_RESOURCE", DefaultLanguageHighlighterColors.DOC_COMMENT);
    public static final TextAttributesKey NUMBER =
            createTextAttributesKey("AMETHYST_NUMBER", DefaultLanguageHighlighterColors.NUMBER);
    public static final TextAttributesKey IDENTIFIER =
            createTextAttributesKey("AMETHYST_IDENTIFIER", DefaultLanguageHighlighterColors.IDENTIFIER);

    static {
        PSIElementTypeFactory.defineLanguageIElementTypes(AmethystLanguage.INSTANCE,
                AmethystParser.tokenNames,
                AmethystParser.ruleNames);
    }

    @NotNull
    @Override
    public Lexer getHighlightingLexer() {
        return new AmethystLexerAdapter();
    }

    @NotNull
    @Override
    public TextAttributesKey @NotNull [] getTokenHighlights(IElementType tokenType) {
        if ( !(tokenType instanceof TokenIElementType amethystTokenType) ) {
            return EMPTY_KEYS;
        }
        int ttype = amethystTokenType.getANTLRTokenType();
        TextAttributesKey attrKey;
        switch (ttype) {

            case AmethystLexer.DOT:
                attrKey = DOT;
                break;

            case AmethystLexer.COMMA:
                attrKey = COMMA;
                break;

            case AmethystLexer.SEMICOLON:
                attrKey = SEMICOLON;
                break;

            case AmethystLexer.LPAREN:
            case AmethystLexer.RPAREN:
                attrKey = PARENTHESES;
                break;

            case AmethystLexer.LBRACE:
            case AmethystLexer.RBRACE:
                attrKey = BRACES;
                break;

            case AmethystLexer.LBRACKET:
            case AmethystLexer.RBRACKET:
                attrKey = BRACKETS;
                break;

            case AmethystLexer.EQUALS:
            case AmethystLexer.PLUS:
            case AmethystLexer.MINUS:
            case AmethystLexer.MULTIPLY:
            case AmethystLexer.DIVIDE:
            case AmethystLexer.MODULO:
            case AmethystLexer.NOT:
            case AmethystLexer.AND:
            case AmethystLexer.OR:
            case AmethystLexer.LESS:
            case AmethystLexer.LESSEQ:
            case AmethystLexer.GREATER:
            case AmethystLexer.GREATEREQ:
            case AmethystLexer.EQEQ:
            case AmethystLexer.NOTEQ:
            case AmethystLexer.PLUSPLUS:
            case AmethystLexer.MINUSMINUS:
            case AmethystLexer.PLUSEQ:
            case AmethystLexer.MINUSEQ:
            case AmethystLexer.MULTEQ:
            case AmethystLexer.DIVEQ:
            case AmethystLexer.MODEQ:
                attrKey = OPERATION;
                break;

            case AmethystLexer.PREPROCESSOR_FROM:
            case AmethystLexer.PREPROCESSOR_IMPORT:
            case AmethystLexer.PREPROCESSOR_VAR:
                attrKey = PREPROCESSOR_DECLARATION;
                break;

            case AmethystLexer.PREPROCESSOR_IF:
            case AmethystLexer.PREPROCESSOR_ELSE:
            case AmethystLexer.PREPROCESSOR_FOR:
            case AmethystLexer.PREPROCESSOR_RETURN:
            case AmethystLexer.PREPROCESSOR_BREAK:
            case AmethystLexer.PREPROCESSOR_YIELD:
            case AmethystLexer.PREPROCESSOR_DEBUG:
                attrKey = PREPROCESSOR_STATEMENT;
                break;

            case AmethystLexer.PREPROCESSOR_TRUE:
            case AmethystLexer.PREPROCESSOR_FALSE:
                attrKey = PREPROCESSOR_BOOLEAN;
                break;

            case AmethystLexer.PREPROCESSOR_BOOL:
            case AmethystLexer.PREPROCESSOR_INT:
            case AmethystLexer.PREPROCESSOR_DEC:
            case AmethystLexer.PREPROCESSOR_STRING:
            case AmethystLexer.PREPROCESSOR_RESOURCE:
                attrKey = PREPROCESSOR_DATATYPE;
                break;

            case AmethystLexer.FUNCTION:
                attrKey = DECLARATION;
                break;

            case AmethystLexer.VAR:
            case AmethystLexer.RECORD:
            case AmethystLexer.FOR:
            case AmethystLexer.WHILE:
            case AmethystLexer.FOREACH:
            case AmethystLexer.IF:
            case AmethystLexer.ELSE:
            case AmethystLexer.DEBUG:
            case AmethystLexer.COMMENT:
            case AmethystLexer.BREAK:
            case AmethystLexer.CONTINUE:
            case AmethystLexer.RETURN:
                attrKey = STATEMENT;
                break;

            case AmethystLexer.TRUE:
            case AmethystLexer.FALSE:
                attrKey = BOOLEAN;
                break;

            case AmethystLexer.BOOL:
            case AmethystLexer.INT:
            case AmethystLexer.DEC:
            case AmethystLexer.STRING:
            case AmethystLexer.ARRAY:
            case AmethystLexer.OBJECT:
            case AmethystLexer.ENTITY:
            case AmethystLexer.STORAGE:
            case AmethystLexer.STORAGE_NAMESPACE:
            case AmethystLexer.STORAGE_MEMBER:
            case AmethystLexer.SCOREBOARD:
            case AmethystLexer.SCOREBOARD_PLAYER:
            case AmethystLexer.SCOREBOARD_OBJECTIVE:
                attrKey = DATATYPE;
                break;

            case AmethystLexer.SELECTOR_SELF:
            case AmethystLexer.SELECTOR_RANDOM_PLAYER:
            case AmethystLexer.SELECTOR_ALL_PLAYERS:
            case AmethystLexer.SELECTOR_ALL_ENTITIES:
            case AmethystLexer.SELECTOR_NEAREST_PLAYER:
            case AmethystLexer.SELECTOR_NEAREST_ENTITY:
                attrKey = SELECTOR;
                break;

            case AmethystLexer.SL_COMMENT:
                attrKey = COMMENT;
                break;

            case AmethystLexer.COMMAND:
                attrKey = COMMAND;
                break;

            case AmethystLexer.STRING_LITERAL:
                attrKey = STRING;
                break;

            case AmethystLexer.RESOURCE_LITERAL:
                attrKey = RESOURCE;
                break;

            case AmethystLexer.INTEGER_LITERAL:
            case AmethystLexer.DECIMAL_LITERAL:
                attrKey = NUMBER;
                break;

            case AmethystLexer.IDENTIFIER:
                attrKey = IDENTIFIER;
                break;

            default:
                return EMPTY_KEYS;
        }
        return new TextAttributesKey[] {attrKey};
    }
}
