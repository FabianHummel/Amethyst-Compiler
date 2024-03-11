namespace Amethyst;

public interface AstNode<out T>
{
    static abstract T Consume(List<(TokenType type, string info)> input);
}

public class Function : AstNode<Function>
{
    public string Name { get; private set; }
    public FunctionDecorators Decorators { get; private set; }
    public FunctionArguments Arguments { get; private set; }
    public FunctionBody Body { get; private set; }
    
    public static Function Consume(List<(TokenType type, string info)> input)
    {
        
        var decorators = FunctionDecorators.Consume(input);
        if (input[0].type != TokenType.KWD_FUNCTION)
        {
            throw new Exception("Expected 'function'");
        }
        input.RemoveAt(0);
        var name = FunctionIdentifier.Consume(input);
        var arguments = FunctionArguments.Consume(input);
        var body = FunctionBody.Consume(input);
        
        return new Function
        {
            Name = name,
            Decorators = decorators,
            Arguments = arguments,
            Body = body
        };
    }
}
    
public class FunctionDecorators : AstNode<FunctionDecorators>
{
    public bool IsTicking { get; private set; }
    public bool IsInitializing { get; private set; }
    
    public static FunctionDecorators Consume(List<(TokenType type, string info)> input)
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
}

public abstract class FunctionIdentifier : AstNode<string>
{
    public static string Consume(List<(TokenType type, string info)> input)
    { 
        if (input[0].type != TokenType.IDENTIFIER)
        {
            throw new Exception("Expected identifier");
        }
        var identifier = input[0].info;
        input.RemoveAt(0);
        return identifier;
    }
}

public class FunctionArguments : AstNode<FunctionArguments>
{
    public List<string> Arguments { get; private set; }
    
    public static FunctionArguments Consume(List<(TokenType type, string info)> input)
    {
        if (input[0].type != TokenType.PAREN_OPEN)
        {
            throw new Exception("Expected (");
        }
        input.RemoveAt(0);
        
        var arguments = new List<string>();
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

        return new FunctionArguments
        {
            Arguments = arguments
        };
    }
}
    
public class FunctionBody : AstNode<FunctionBody>
{
    public List<(TokenType type, string info)> Body { get; private set; }
    
    public static FunctionBody Consume(List<(TokenType type, string info)> input)
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

        return new FunctionBody
        {
            Body = body
        };
    }
}