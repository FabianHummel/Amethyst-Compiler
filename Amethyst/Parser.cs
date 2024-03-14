using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace Amethyst;

public partial interface AstNode
{
    
}

public static class Parser
{
    public static Token Expect(IList<Token> input, TokenType type, ref Token parent)
    {
        if (input.Count <= 0 || input[0].Type != type)
        {
            var fi = type.GetType().GetField(type.ToString())!;
            string description = type.ToString();
            if (fi.GetCustomAttributes(typeof(DescriptionAttribute), false) is DescriptionAttribute[] attributes && attributes.Any())
            {
                description = attributes.First().Description;
            }
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
                if (Assignment.TryConsume(input, input[0], out var assignment))
                {
                    nodes.Add(assignment);
                }
                else if (FunctionCall.TryConsume(input, input[0], out var functionCall))
                {
                    nodes.Add(functionCall);
                }
                else if (Variable.TryConsume(input, input[0], out var variable))
                {
                    nodes.Add(variable);
                }
                else if (Function.TryConsume(input, input[0], out var function))
                {
                    nodes.Add(function);
                }
                else if (Namespace.TryConsume(input, input[0], out var @namespace))
                {
                    nodes.Add(@namespace);
                }
                else
                {
                    throw new SyntaxException("Unexpected token", input[0]);
                }
            }

            return nodes;
        }
        catch (SyntaxException e)
        {
            Console.Error.WriteLine(e.Message + (e.Expected != null ? " at line " + e.Expected.Line : ""));
            return new List<AstNode>();
        }
    }

    /**
     * Input is ONLY the expression (f.e. until excl. ; or , or }), not the entire input
     */
    public static AstNode? Expression(IList<Token> input)
    {
        if (input.Count == 0)
        {
            return null;
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
                            return Expression(input);
                        }

                        break;
                    }
                }

                i++;
            }
        }
        
        if (input.Count <= 0)
        {
            throw new SyntaxException("Unexpected end of expression");
        }
        
        if (Operation.TryConsume(input, input[0], out var operation))
        {
            return operation;
        }
        
        if (FunctionCall.TryConsume(input, input[0], out var functionCall))
        {
            return functionCall;
        }

        if (VariableReference.TryConsume(input, input[0], out var variable))
        {
            return variable;
        }

        if (Constant.TryConsume(input, input[0], out var constant))
        {
            return constant;
        }

        if (input.Count > 0)
        {
            throw new SyntaxException("Unexpected end of expression", input[0]);
        }
        
        return null;
    }
}

internal interface AstNode<T> : AstNode
{
    protected static abstract bool TryConsume(IList<Token> input, Token parent, [NotNullWhen(true)] out T? result);
}

public partial class Variable : AstNode<Variable>
{
    public required string Name { get; init; }
    public required DataType Type { get; init; }
    public required AstNode Value { get; init; }
    
    public static bool TryConsume(IList<Token> input, Token parent, [NotNullWhen(true)] out Variable? result)
    {
        result = null;
        
        if (input.Count < 1 || input[0].Type != TokenType.KWD_VARIABLE)
        {
            return false;
        }
        
        Parser.Expect(input, TokenType.KWD_VARIABLE, ref parent);
        if (!Identifier.TryConsume(input, parent, out var name))
        {
            return false;
        }
        
        Parser.Expect(input, TokenType.OP_ASSIGN, ref parent);
        
        var tokens = new List<Token>();
        while (input.Count > 0 && input[0].Type != TokenType.SEMICOLON)
        {
            tokens.Add(input[0]);
            input.RemoveAt(0);
        }
        
        var node = Parser.Expression(tokens);
        if (node == null)
        {
            throw new SyntaxException("Expected expression", parent);
        }
        
        Parser.Expect(input, TokenType.SEMICOLON, ref parent);
        
        result = new Variable
        {
            Name = name,
            Type = DataType.String, // TODO: infer type
            Value = node
        }; 
        return true;
    }
}

public partial class Assignment : AstNode<Assignment>
{
    public required string Name { get; init; }
    public required DataType Type { get; init; }
    public required AstNode Value { get; init; }
    
    public static bool TryConsume(IList<Token> input, Token parent, [NotNullWhen(true)] out Assignment? result)
    {
        result = null;
        if (input.Count < 3 || input[0].Type != TokenType.IDENTIFIER || input[1].Type != TokenType.OP_ASSIGN)
        {
            return false;
        }
        
        if (!Identifier.TryConsume(input, parent, out var name))
        { 
            return false;
        }
        Parser.Expect(input, TokenType.OP_ASSIGN, ref parent);
        
        var tokens = new List<Token>();
        while (input.Count > 0 && input[0].Type != TokenType.SEMICOLON)
        {
            tokens.Add(input[0]);
            input.RemoveAt(0);
        }
        
        var node = Parser.Expression(tokens);
        if (node == null)
        {
            throw new SyntaxException("Expected expression", parent);
        }
        
        Parser.Expect(input, TokenType.SEMICOLON, ref parent);

        result = new Assignment
        {
            Name = name,
            Type = DataType.String, // TODO: infer type (if function call -> use return type)
            Value = node
        };
        return true;
    }
}

public partial class Function : AstNode<Function>
{
    public required string Name { get; init; }
    public required FunctionDecorators Decorators { get; init; }
    public required IEnumerable<string> Arguments { get; init; }
    public required IEnumerable<AstNode> Body { get; init; }
    // TODO: return type
    
    public static bool TryConsume(IList<Token> input, Token parent, [NotNullWhen(true)] out Function? result)
    {
        result = null;
        if (input[0].Type is not TokenType.KWD_TICKING and not TokenType.KWD_INITIALIZING and not TokenType.KWD_FUNCTION)
        {
            return false;
        }
        
        if (!FunctionDecorators.TryConsume(input, parent, out var decorators))
        {
            return false;
        }
        Parser.Expect(input, TokenType.KWD_FUNCTION, ref parent);

        if (!Identifier.TryConsume(input, parent, out var name))
        {
            return false;
        }

        if (!Amethyst.Arguments.TryConsume(input, parent, out var arguments))
        {
            return false;
        }

        if (!Amethyst.Body.TryConsume(input, parent, out var body))
        {
            return false;
        }
        
        result = new Function
        {
            Name = name,
            Decorators = decorators,
            Arguments = arguments,
            Body = body
        };
        return true;
    }
}
    
public partial class FunctionDecorators : AstNode<FunctionDecorators>
{
    public bool IsTicking { get; private set; }
    public bool IsInitializing { get; private set; }
    
    public static bool TryConsume(IList<Token> input, Token parent, [NotNullWhen(true)] out FunctionDecorators? result)
    {
        result = new FunctionDecorators();
        while (input[0].Type != TokenType.KWD_FUNCTION)
        {
            var token = input[0];
            if (token.Type == TokenType.KWD_TICKING)
            {
                result.IsTicking = true;
            }
            else if (token.Type == TokenType.KWD_INITIALIZING)
            {
                result.IsInitializing = true;
            }
            else
            {
                throw new SyntaxException("Expected function or decorator", input[0]);
            }
            input.RemoveAt(0);
        }
        return true;
    }
}

public abstract partial class Identifier : AstNode<string>
{
    public static bool TryConsume(IList<Token> input, Token parent, [NotNullWhen(true)] out string? result)
    {
        result = Parser.Expect(input, TokenType.IDENTIFIER, ref parent).Lexeme;
        return true;
    }
}

public abstract partial class Arguments : AstNode<IEnumerable<string>>
{
    public static bool TryConsume(IList<Token> input, Token parent, [NotNullWhen(true)] out IEnumerable<string>? result)
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
        result = arguments;
        return true;
    }
}

public abstract partial class Parameters : AstNode<IEnumerable<AstNode>>
{
    public static bool TryConsume(IList<Token> input, Token parent, [NotNullWhen(true)] out IEnumerable<AstNode>? result)
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
                var node = Parser.Expression(currentParameter);
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
                    var node = Parser.Expression(currentParameter);
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
        result = parameters;
        return true;
    }
}

public abstract partial class Body : AstNode<IEnumerable<AstNode>>
{
    public static bool TryConsume(IList<Token> input, Token parent, [NotNullWhen(true)] out IEnumerable<AstNode>? result)
    {
        Parser.Expect(input, TokenType.BRACE_OPEN, ref parent);
        var tokens = new List<Token>();

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

            tokens.Add(token);
            input.RemoveAt(0);
        }

        Parser.Expect(input, TokenType.BRACE_CLOSE, ref parent);
        result = Parser.ParseBody(tokens);
        return true;
    }
}

public partial class Namespace : AstNode<Namespace>
{
    public required string Name { get; init; }
    public required IEnumerable<AstNode> Body { get; init; }
    
    public static bool TryConsume(IList<Token> input, Token parent, [NotNullWhen(true)] out Namespace? result)
    {
        result = null;
        if (input[0].Type is not TokenType.KWD_NAMESPACE)
        {
            return false;
        }
        
        Parser.Expect(input, TokenType.KWD_NAMESPACE, ref parent);
        if (!Identifier.TryConsume(input, parent, out var name))
        {
            return false;
        }

        if(!Amethyst.Body.TryConsume(input, parent, out var body))
        {
            return false;
        }
        
        result = new Namespace
        {
            Name = name,
            Body = body
        };
        return true;
    }
}

public partial class FunctionCall : AstNode<FunctionCall>
{
    public required string Name { get; init; }
    public required IEnumerable<AstNode> Parameters { get; init; }

    public static bool TryConsume(IList<Token> input, Token parent, [NotNullWhen(true)] out FunctionCall? result)
    {
        result = null;
        if (input.Count < 2 || input[0].Type != TokenType.IDENTIFIER || input[1].Type != TokenType.PAREN_OPEN)
        {
            return false;
        }
        
        if(!Identifier.TryConsume(input, parent, out var name))
        {
            return false;    
        }
        
        if(!Amethyst.Parameters.TryConsume(input, parent, out var parameters))
        {
            return false;    
        }
        
        result = new FunctionCall
        {
            Name = name,
            Parameters = parameters
        };
        return true;
    }
}

public partial class VariableReference : AstNode<VariableReference>
{
    public required string Name { get; init; }

    public static bool TryConsume(IList<Token> input, Token parent, [NotNullWhen(true)] out VariableReference? result)
    {
        result = null;
        if (input.Count < 1 || input[0].Type != TokenType.IDENTIFIER)
        {
            return false;
        }
        
        if (!Identifier.TryConsume(input, parent, out var name))
        {
            return false;
        }
        
        // TODO: check for dots to access object properties
        
        result = new VariableReference
        {
            Name = name
        };
        return true;
    }
}

public partial class Constant : AstNode<Constant>
{
    public required DataType Type { get; init; }
    public required string Value { get; init; }
    
    public static bool TryConsume(IList<Token> input, Token parent, [NotNullWhen(true)] out Constant? result)
    {
        result = null;
        if (input.Count < 1)
        {
            return false;
        }
        
        if (input[0].Type == TokenType.LITERAL_STRING)
        {
            result = new Constant
            {
                Type = DataType.String,
                Value = input[0].Lexeme
            };
        }
        else if (input[0].Type == TokenType.LITERAL_NUMBER)
        {
            result = new Constant
            {
                Type = DataType.Number,
                Value = input[0].Lexeme
            };
        }
        return result != null;
    }
}

public partial class Operation : AstNode<Operation>
{
    public required AstNode Left { get; set; }
    public required AstNode Right { get; set; }
    public required ArithmeticOperator Op { get; init; }
    
    private static readonly IReadOnlySet<Tuple<ArithmeticOperator, TokenType>> OPERATIONS = new HashSet<Tuple<ArithmeticOperator, TokenType>>
    {
        new(ArithmeticOperator.OP_ADD, TokenType.OP_ADD),
        new(ArithmeticOperator.OP_SUB, TokenType.OP_SUB),
        new(ArithmeticOperator.OP_MUL, TokenType.OP_MUL),
        new(ArithmeticOperator.OP_DIV, TokenType.OP_DIV),
        new(ArithmeticOperator.OP_MOD, TokenType.OP_MOD),
    };
    
    public static bool TryConsume(IList<Token> input, Token parent, out Operation? result)
    {
        result = null;
        var tokens = new List<Token>();
        loop:
        while (input.Count > 0)
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
            
            bool found = false;
            foreach (var (arithmeticOperator, type) in OPERATIONS)
            {
                if (input.Any(t => t.Type == type)) // if there is an operator in the expression, split it there
                {
                    found = true;
                    if (input[0].Type == type)
                    {
                        if (Parser.Expression(tokens) is not { } left)
                        {
                            return false;
                        }
                        input.RemoveAt(0);
                        if (Parser.Expression(input) is not { } right)
                        {
                            return false;
                        }
                        result = new Operation
                        {
                            Left = left,
                            Right = right,
                            Op = arithmeticOperator,
                        };
                        return true;
                    }
                    break;
                }
            }
            
            if (!found)
            {
                break;
            }
            
            tokens.Add(token);
            input.RemoveAt(0);
        }
        return false;
    }
}