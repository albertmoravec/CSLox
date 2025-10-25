namespace CSLox;

public class RuntimeError(Token token, string message) : ApplicationException(message)
{
    public Token Token { get; set; } = token;
}
