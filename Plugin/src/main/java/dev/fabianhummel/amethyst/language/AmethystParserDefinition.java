package dev.fabianhummel.amethyst.language;

import com.intellij.lang.ASTNode;
import com.intellij.lang.ParserDefinition;
import com.intellij.lang.PsiParser;
import com.intellij.lexer.Lexer;
import com.intellij.openapi.project.Project;
import com.intellij.psi.FileViewProvider;
import com.intellij.psi.PsiElement;
import com.intellij.psi.PsiFile;
import com.intellij.psi.tree.IFileElementType;
import com.intellij.psi.tree.TokenSet;
import dev.fabianhummel.amethyst.AmethystLexer;
import dev.fabianhummel.amethyst.AmethystParser;
import dev.fabianhummel.amethyst.psi.AmethystPsiNode;
import dev.fabianhummel.amethyst.psi.AmethystTokenSets;
import dev.fabianhummel.amethyst.psi.AmethystFile;
import org.antlr.intellij.adaptor.lexer.PSIElementTypeFactory;
import org.antlr.intellij.adaptor.lexer.TokenIElementType;
import org.jetbrains.annotations.NotNull;

import java.util.List;

public class AmethystParserDefinition implements ParserDefinition {
    public static final IFileElementType FILE =
            new IFileElementType(AmethystLanguage.INSTANCE);

    public static TokenIElementType ID;

    static {
        PSIElementTypeFactory.defineLanguageIElementTypes(AmethystLanguage.INSTANCE,
                AmethystParser.tokenNames,
                AmethystParser.ruleNames);
        List<TokenIElementType> tokenIElementTypes =
                PSIElementTypeFactory.getTokenIElementTypes(AmethystLanguage.INSTANCE);
        ID = tokenIElementTypes.get(AmethystLexer.IDENTIFIER);
    }

    @Override
    public @NotNull Lexer createLexer(Project project) {
        return new AmethystLexerAdapter();
    }

    @Override
    public @NotNull PsiParser createParser(final Project project) {
        return new AmethystParserAdapter();
    }

    @NotNull
    @Override
    public TokenSet getCommentTokens() {
        return AmethystTokenSets.COMMENTS;
    }

    @Override
    public @NotNull TokenSet getWhitespaceTokens() {
        return AmethystTokenSets.SPACE;
    }

    @Override
    public @NotNull TokenSet getStringLiteralElements() {
        return AmethystTokenSets.STRINGS;
    }

    @Override
    public @NotNull IFileElementType getFileNodeType() {
        return FILE;
    }

    @Override
    public @NotNull PsiFile createFile(@NotNull FileViewProvider viewProvider) {
        return new AmethystFile(viewProvider);
    }

    @Override
    public @NotNull PsiElement createElement(ASTNode node) {
        return new AmethystPsiNode(node);
    }
}