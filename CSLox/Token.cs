namespace CSLox;

public class Token(TokenType type, string lexeme, object? literal, int line)
{
    public TokenType Type { get; set; } = type;
    public string Lexeme { get; set; } = lexeme;
    public object Literal { get; set; } = literal;
    public int Line { get; set; } = line;

    public override string ToString()
    {
        return $"{Type} {Lexeme} {Literal}";
    }
}