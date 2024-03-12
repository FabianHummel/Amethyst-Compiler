using System.ComponentModel;

namespace Amethyst;

public partial interface AstNode
{
    
}

public static class Parser
{
    public static Token Expect(IList<Token> input, TokenType type, ref Token parent)
    {
        // get [Description] attribute from enum
        var fi = type.GetType().GetField(type.ToString())!;
        string description = type.ToString();
        if (fi.GetCustomAttributes(typeof(DescriptionAttribute), false) is DescriptionAttribute[] attributes && attributes.Any())
        {
            description = attributes.First().Description;
        }

        if (input.Count <= 0 || input[0].Type != type)
        {
            throw new SyntaxException("Expected " + description, parent);
        }
        parent.Line = input[0].Line;
        parent.Type = input[0].Type;
        parent.Lexeme = input[0].Lexeme;
        input.RemoveAt(0);
        return parent;
    }
    
    public static IEnumerable<AstNode> ParseBody(IList<Token> input)
    {
        try
        {
            var nodes = new List<AstNode>();
            while (input.Count > 0)
            {
                if (input[0].Type is TokenType.KWD_VARIABLE)
                {
                    nodes.Add(Variable.Consume(input, input[0]));
                }
                else if (input[0].Type is TokenType.IDENTIFIER)
                {
                    if (input.Count > 0)
                    {
                        if (input[1].Type is TokenType.PAREN_OPEN)
                        {
                            nodes.Add(FunctionCall.Consume(input, input[0]));
                        }
                        else if (input[1].Type is TokenType.OP_ASSIGN)
                        {
                            nodes.Add(Assignment.Consume(input, input[0]));
                        }
                    }
                }
                else if (input[0].Type is TokenType.KWD_FUNCTION or TokenType.KWD_TICKING or TokenType.KWD_INITIALIZING)
                {
                    nodes.Add(Function.Consume(input, input[0]));
                }
                else if (input[0].Type is TokenType.KWD_NAMESPACE)
                {
                    nodes.Add(Namespace.Consume(input, input[0]));
                }
                else
                {
                    throw new SyntaxException("Unexpected token '" + input[0].Lexeme + "'", input[0]);
                }
            }

            return nodes;
        }
        catch (SyntaxException e)
        {
            Console.Error.WriteLine(e.Message + " at line " + e.Expected.Line);
            return new List<AstNode>();
        }
    }
    
    public static AstNode? Consume(IList<Token> input)
    {
        if (input.Count == 0)
        {
            return null;
        }

        if (input.Count == 1)
        {
            var token = input[0];
            if (token.Type == TokenType.LITERAL_NUMBER)
            {
                return new Constant
                {
                    Type = DataType.Number,
                    Value = token.Lexeme
                };
            }
            if (token.Type == TokenType.LITERAL_STRING)
            {
                return new Constant
                {
                    Type = DataType.String,
                    Value = token.Lexeme
                };
            }

            if (token.Type == TokenType.IDENTIFIER)
            {
                return VariableReference.Consume(input, input[0]);
            }
        }
        
        if (input[0].Type is TokenType.PAREN_OPEN)
        {
            int parenCount = 1;
            int i = 1;
            while (parenCount > 0 && i < input.Count)
            {
                var token = input[i];
                if (token.Type is TokenType.PAREN_OPEN)
                {
                    parenCount++;
                }
                else if (token.Type is TokenType.PAREN_CLOSE)
                {
                    parenCount--;
                    if (parenCount == 0)
                    {
                        if (i == input.Count - 1)
                        {
                            input.RemoveAt(0);
                            input.RemoveAt(input.Count - 1);
                            return Consume(input);
                        }
                        break;
                    }
                }
                i++;
            }
        }
        
        if (input[0].Type is TokenType.IDENTIFIER)
        {
            if (input.Count >= 1 && input[1].Type is TokenType.PAREN_OPEN)
            {
                return FunctionCall.Consume(input, input[0]);
            }
        }

        var tokens = new List<Token>();
        loop:
        while (input.Count > 0 && input[0].Type is not TokenType.COMMA and not TokenType.SEMICOLON)
        {
            var token = input[0];
            if (token.Type is TokenType.PAREN_OPEN)
            {
                tokens.Add(token);
                input.RemoveAt(0);
                
                int parenCount = 1;
                while (parenCount > 0 && input.Count > 0)
                {
                    token = input[0];
                    if (token.Type is TokenType.PAREN_OPEN)
                    {
                        parenCount++;
                    }
                    else if (token.Type is TokenType.PAREN_CLOSE)
                    {
                        parenCount--;
                        if (parenCount == 0)
                        {
                            tokens.Add(token);
                            input.RemoveAt(0);
                            goto loop;
                        }
                    }
                    
                    tokens.Add(token);
                    input.RemoveAt(0);
                }
            }

            if (input.Any(t => t.Type == TokenType.OP_ADD))
            {
                if (token.Type is TokenType.OP_ADD)
                {
                    var left = Consume(tokens);
                    input.RemoveAt(0);
                    var right = Consume(input);
                    return new Operation
                    {
                        Left = left,
                        Right = right,
                        Op = ArithmeticOperator.OP_ADD
                    };
                }
            }
            else if (input.Any(t => t.Type == TokenType.OP_SUB))
            {
                if (token.Type is TokenType.OP_SUB)
                {
                    var left = Consume(tokens);
                    input.RemoveAt(0);
                    var right = Consume(input);
                    return new Operation
                    {
                        Left = left,
                        Right = right,
                        Op = ArithmeticOperator.OP_SUB
                    };
                }
            }
            else if (input.Any(t => t.Type == TokenType.OP_MUL))
            {
                if (token.Type is TokenType.OP_MUL)
                {
                    var left = Consume(tokens);
                    input.RemoveAt(0);
                    var right = Consume(input);
                    return new Operation
                    {
                        Left = left,
                        Right = right,
                        Op = ArithmeticOperator.OP_MUL
                    };
                }
            }
            else if (input.Any(t => t.Type == TokenType.OP_DIV))
            {
                if (token.Type is TokenType.OP_DIV)
                {
                    var left = Consume(tokens);
                    input.RemoveAt(0);
                    var right = Consume(input);
                    return new Operation
                    {
                        Left = left,
                        Right = right,
                        Op = ArithmeticOperator.OP_DIV
                    };
                }
            }
            else if (input.Any(t => t.Type == TokenType.OP_MOD))
            {
                if (token.Type is TokenType.OP_MOD)
                {
                    var left = Consume(tokens);
                    input.RemoveAt(0);
                    var right = Consume(input);
                    return new Operation
                    {
                        Left = left,
                        Right = right,
                        Op = ArithmeticOperator.OP_MOD
                    };
                }
            }
            
            tokens.Add(token);
            input.RemoveAt(0);
        }

        if (input.Count <= 0)
        {
            throw new SyntaxException("Expected expression", tokens[0]);
        }
        
        return Consume(tokens);
    }
}

internal interface AstNode<out T> : AstNode
{
    protected static abstract T Consume(IList<Token> input, Token parent);
}

public partial class Variable : AstNode<Variable>
{
    public required string Name { get; init; }
    public required DataType Type { get; init; }
    public required AstNode Value { get; init; }
    
    public static Variable Consume(IList<Token> input, Token parent)
    {
        Parser.Expect(input, TokenType.KWD_VARIABLE, ref parent);
        var name = Identifier.Consume(input, parent);
        Parser.Expect(input, TokenType.OP_ASSIGN, ref parent);
        
        var node = Parser.Consume(input);
        if (node == null)
        {
            throw new SyntaxException("Expected expression", parent);
        }
        Parser.Expect(input, TokenType.SEMICOLON, ref parent);

        return new Variable
        {
            Name = name,
            Type = DataType.String, // TODO: infer type (if function call -> use return type)
            Value = node
        };
    }
}

public partial class Assignment : AstNode<Assignment>
{
    public required string Name { get; init; }
    public required DataType Type { get; init; }
    public required AstNode Value { get; init; }
    
    public static Assignment Consume(IList<Token> input, Token parent)
    {
        var name = Identifier.Consume(input, parent);
        Parser.Expect(input, TokenType.OP_ASSIGN, ref parent);
        
        var node = Parser.Consume(input);
        if (node == null)
        {
            throw new SyntaxException("Expected expression", parent);
        }
        Parser.Expect(input, TokenType.SEMICOLON, ref parent);

        return new Assignment
        {
            Name = name,
            Type = DataType.String, // TODO: infer type (if function call -> use return type)
            Value = node
        };
    }
}

public partial class Function : AstNode<Function>
{
    public required string Name { get; init; }
    public required FunctionDecorators Decorators { get; init; }
    public required IEnumerable<string> Arguments { get; init; }
    public required IEnumerable<AstNode> Body { get; init; }
    // TODO: return type
    
    public static Function Consume(IList<Token> input, Token parent)
    {
        var decorators = FunctionDecorators.Consume(input, parent);
        Parser.Expect(input, TokenType.KWD_FUNCTION, ref parent);
        
        var name = Identifier.Consume(input, parent);
        var arguments = Amethyst.Arguments.Consume(input, parent);
        var body = Amethyst.Body.Consume(input, parent);
        
        return new Function
        {
            Name = name,
            Decorators = decorators,
            Arguments = arguments,
            Body = body
        };
    }
}
    
public partial class FunctionDecorators : AstNode<FunctionDecorators>
{
    public bool IsTicking { get; private set; }
    public bool IsInitializing { get; private set; }
    
    public static FunctionDecorators Consume(IList<Token> input, Token parent)
    {
        var decorators = new FunctionDecorators();
        while (input[0].Type != TokenType.KWD_FUNCTION)
        {
            var token = input[0];
            if (token.Type == TokenType.KWD_TICKING)
            {
                decorators.IsTicking = true;
            }
            else if (token.Type == TokenType.KWD_INITIALIZING)
            {
                decorators.IsInitializing = true;
            }
            else
            {
                throw new SyntaxException("Expected function or decorator", input[0]);
            }
            input.RemoveAt(0);
        }
        return decorators;
    }
}

public abstract partial class Identifier : AstNode<string>
{
    public static string Consume(IList<Token> input, Token parent)
    {
        return Parser.Expect(input, TokenType.IDENTIFIER, ref parent).Lexeme;
    }
}

public abstract partial class Arguments : AstNode<IEnumerable<string>>
{
    public static IEnumerable<string> Consume(IList<Token> input, Token parent)
    {
        var arguments = new List<string>();
        Parser.Expect(input, TokenType.PAREN_OPEN, ref parent);
        
        while (input[0].Type != TokenType.PAREN_CLOSE)
        {
            var token = Parser.Expect(input, TokenType.IDENTIFIER, ref parent);
            arguments.Add(token.Lexeme);
            if (input[0].Type == TokenType.COMMA)
            {
                input.RemoveAt(0);
            }
        }
        
        Parser.Expect(input, TokenType.PAREN_CLOSE, ref parent);
        return arguments;
    }
}

public abstract partial class Parameters : AstNode<IEnumerable<AstNode>>
{
    public static IEnumerable<AstNode> Consume(IList<Token> input, Token parent)
    {
        var parameters = new List<AstNode>();
        Parser.Expect(input, TokenType.PAREN_OPEN, ref parent);
        
        var currentParameter = new List<Token>();
        int braceCount = 1;
        while (braceCount > 0)
        {
            var token = input[0];
            if (token.Type is TokenType.COMMA)
            {
                var node = Parser.Consume(currentParameter);
                if (node != null)
                {
                    parameters.Add(node);
                }
                currentParameter = new List<Token>();
                input.RemoveAt(0);
                continue;
            }
            
            if (token.Type == TokenType.PAREN_OPEN)
            {
                braceCount++;
            }
            else if (token.Type == TokenType.PAREN_CLOSE)
            {
                braceCount--;
                if (braceCount == 0)
                {
                    var node = Parser.Consume(currentParameter);
                    if (node != null)
                    {
                        parameters.Add(node);
                    }
                    break;
                }
            }
            currentParameter.Add(token);
            input.RemoveAt(0);
        }
        
        Parser.Expect(input, TokenType.PAREN_CLOSE, ref parent);
        return parameters;
    }
}

public abstract partial class Body : AstNode<IEnumerable<AstNode>>
{
    public static IEnumerable<AstNode> Consume(IList<Token> input, Token parent)
    {
        Parser.Expect(input, TokenType.BRACE_OPEN, ref parent);
        var body = new List<Token>();

        // find matching brace
        int braceCount = 1;
        while (braceCount > 0 && input.Count > 0)
        {
            var token = input[0];
            if (token.Type == TokenType.BRACE_OPEN)
            {
                braceCount++;
            }
            else if (token.Type == TokenType.BRACE_CLOSE)
            {
                braceCount--;
                if (braceCount == 0)
                {
                    break;
                }
            }

            body.Add(token);
            input.RemoveAt(0);
        }

        Parser.Expect(input, TokenType.BRACE_CLOSE, ref parent);

        return Parser.ParseBody(body);
    }
}

public partial class Namespace : AstNode<Namespace>
{
    public required string Name { get; init; }
    public required IEnumerable<AstNode> Body { get; init; }
    
    public static Namespace Consume(IList<Token> input, Token parent)
    {
        Parser.Expect(input, TokenType.KWD_NAMESPACE, ref parent);
        
        var name = Identifier.Consume(input, parent);
        var body = Amethyst.Body.Consume(input, parent);
        
        return new Namespace
        {
            Name = name,
            Body = body
        };
    }
}

public partial class FunctionCall : AstNode<FunctionCall>
{
    public required string Name { get; init; }
    public required IEnumerable<AstNode> Parameters { get; init; }

    public static FunctionCall Consume(IList<Token> input, Token parent)
    {
        var name = Identifier.Consume(input, parent);
        var parameters = Amethyst.Parameters.Consume(input, parent);

        return new FunctionCall
        {
            Name = name,
            Parameters = parameters
        };
    }
}

public partial class VariableReference : AstNode<VariableReference>
{
    public required string Name { get; init; }

    public static VariableReference Consume(IList<Token> input, Token parent)
    {
        var name = Identifier.Consume(input, parent);
        
        // TODO: check for dots to access object properties
        
        return new VariableReference
        {
            Name = name
        };
    }
}

public partial class Constant : AstNode
{
    public required DataType Type { get; init; }
    public required string Value { get; init; }
}

public partial class Operation : AstNode
{
    public required AstNode Left { get; init; }
    public required AstNode Right { get; init; }
    public required ArithmeticOperator Op { get; init; }
}