namespace Amethyst;

public interface AstNode
{
    string ToCode(AstContext context);
}

internal interface AstNode<out T> : AstNode
{
    static abstract T Consume(IList<(TokenType type, string info)> input);
}

public class Variable : AstNode<Variable>
{
    public enum VariableType
    {
        Number,
        String,
        Boolean,
        Object,
        Array
    }
    
    public string Name { get; private set; }
    public VariableType Type { get; private set; }
    public string Value { get; private set; }
    
    public static Variable Consume(IList<(TokenType type, string info)> input)
    {
        if (input[0].type != TokenType.KWD_VARIABLE)
        {
            throw new Exception("Expected 'var'");
        }
        input.RemoveAt(0);
        
        var name = Identifier.Consume(input);
        
        if (input[0].type != TokenType.OP_ASSIGN)
        {
            throw new Exception("Expected '='");
        }
        input.RemoveAt(0);
        
        var value = input[0].info;
        input.RemoveAt(0);
        
        return new Variable
        {
            Name = name,
            Type = VariableType.String, // TODO: infer type (if function call -> use return type)
            Value = value
        };
    }

    public string ToCode(AstContext context)
    {
        throw new NotImplementedException();
    }
}

public class Function : AstNode<Function>
{
    public string Name { get; private set; }
    public FunctionDecorators Decorators { get; private set; }
    public IEnumerable<string> Arguments { get; private set; }
    public IEnumerable<AstNode> Body { get; private set; }
    // TODO: return type
    
    public static Function Consume(IList<(TokenType type, string info)> input)
    {
        var decorators = FunctionDecorators.Consume(input);
        if (input[0].type != TokenType.KWD_FUNCTION)
        {
            throw new Exception("Expected 'function'");
        }
        input.RemoveAt(0);
        
        var name = Identifier.Consume(input);
        var arguments = Amethyst.Arguments.Consume(input);
        var body = Amethyst.Body.Consume(input);
        
        return new Function
        {
            Name = name,
            Decorators = decorators,
            Arguments = arguments,
            Body = body
        };
    }

    public string ToCode(AstContext context)
    {
        throw new NotImplementedException();
    }
}
    
public class FunctionDecorators : AstNode<FunctionDecorators>
{
    public bool IsTicking { get; private set; }
    public bool IsInitializing { get; private set; }
    
    public static FunctionDecorators Consume(IList<(TokenType type, string info)> input)
    {
        var decorators = new FunctionDecorators();
        while (input[0].type != TokenType.KWD_FUNCTION)
        {
            var token = input[0];
            if (token.type == TokenType.KWD_TICKING)
            {
                decorators.IsTicking = true;
            }
            else if (token.type == TokenType.KWD_INITIALIZING)
            {
                decorators.IsInitializing = true;
            }
            else
            {
                throw new Exception("Expected function or decorator");
            }
            input.RemoveAt(0);
        }
        return decorators;
    }

    public string ToCode(AstContext context)
    {
        throw new NotImplementedException();
    }
}

public abstract class Identifier : AstNode<string>
{
    public static string Consume(IList<(TokenType type, string info)> input)
    { 
        if (input[0].type != TokenType.IDENTIFIER)
        {
            throw new Exception("Expected identifier");
        }
        var identifier = input[0].info;
        input.RemoveAt(0);
        return identifier;
    }

    public string ToCode(AstContext context)
    {
        throw new NotImplementedException();
    }
}

public abstract class Arguments : AstNode<IEnumerable<string>>
{
    public static IEnumerable<string> Consume(IList<(TokenType type, string info)> input)
    {
        var arguments = new List<string>();
        if (input[0].type != TokenType.PAREN_OPEN)
        {
            throw new Exception("Expected (");
        }
        input.RemoveAt(0);
        
        while (input[0].type != TokenType.PAREN_CLOSE)
        {
            if (input[0].type != TokenType.IDENTIFIER)
            {
                throw new Exception("Expected identifier");
            }
            arguments.Add(input[0].info);
            input.RemoveAt(0);
            
            if (input[0].type == TokenType.COMMA)
            {
                input.RemoveAt(0);
            }
        }
        
        if (input[0].type != TokenType.PAREN_CLOSE)
        {
            throw new Exception("Expected )");
        }
        input.RemoveAt(0);
        
        return arguments;
    }

    public string ToCode(AstContext context)
    {
        throw new NotImplementedException();
    }
}
    
public abstract class Body : AstNode<IEnumerable<AstNode>>
{
    public static IEnumerable<AstNode> Consume(IList<(TokenType type, string info)> input)
    {
        if (input[0].type != TokenType.BRACE_OPEN)
        {
            throw new Exception("Expected {");
        }
        input.RemoveAt(0);
        
        var body = new List<(TokenType, string)>();
        
        // find matching brace
        int braceCount = 1;
        while (braceCount > 0 && input.Count > 0)
        {
            var token = input[0];
            if (token.type == TokenType.BRACE_OPEN)
            {
                braceCount++;
            }
            else if (token.type == TokenType.BRACE_CLOSE)
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
        
        if (input[0].type != TokenType.BRACE_CLOSE)
        {
            throw new Exception("Expected }");
        }
        input.RemoveAt(0);

        return Parser.Parse(body);
    }

    public string ToCode(AstContext context)
    {
        throw new NotImplementedException();
    }
}

public class Namespace : AstNode<Namespace>
{
    public string Name { get; private set; }
    public IEnumerable<AstNode> Body { get; private set; }
    
    public static Namespace Consume(IList<(TokenType type, string info)> input)
    {
        if (input[0].type != TokenType.KWD_NAMESPACE)
        {
            throw new Exception("Expected 'namespace'");
        }
        input.RemoveAt(0);
        
        var name = Identifier.Consume(input);
        var body = Amethyst.Body.Consume(input);
        
        return new Namespace
        {
            Name = name,
            Body = body
        };
    }

    public string ToCode(AstContext context)
    {
        throw new NotImplementedException();
    }
}