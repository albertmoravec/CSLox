namespace CSLox;

public class Scanner(string source)
{
    private static Dictionary<string, TokenType> keywords = new()
    {
        { "and", TokenType.And },
        { "class", TokenType.Class },
        { "else", TokenType.Else },
        { "false", TokenType.False },
        { "for", TokenType.For },
        { "fun", TokenType.Fun },
        { "if", TokenType.If },
        { "nil", TokenType.Nil },
        { "or", TokenType.Or },
        { "print", TokenType.Print },
        { "return", TokenType.Return },
        { "super", TokenType.Super },
        { "this", TokenType.This },
        { "true", TokenType.True },
        { "var", TokenType.Var },
        { "while", TokenType.Var }
    };

    private List<Token> tokens = [];

    private int start = 0;
    private int current = 0;
    private int line = 1;

    public List<Token> ScanTokens()
    {
        while (!IsAtEnd())
        {
            start = current;
            ScanToken();
        }

        tokens.Add(new Token(TokenType.Eof, "", null, line));
        return tokens;
    }

    private void ScanToken()
    {
        var c = Advance();
        switch (c)
        {
            case '(': AddToken(TokenType.LeftParen); break;
            case ')': AddToken(TokenType.RightParen); break;
            case '{': AddToken(TokenType.LeftBrace); break;
            case '}': AddToken(TokenType.RightBrace); break;
            case ',': AddToken(TokenType.Comma); break;
            case '.': AddToken(TokenType.Dot); break;
            case '-': AddToken(TokenType.Minus); break;
            case '+': AddToken(TokenType.Plus); break;
            case ';': AddToken(TokenType.Semicolon); break;
            case '*': AddToken(TokenType.Star); break;
            case '!': AddToken(Match('=') ? TokenType.BangEqual : TokenType.Bang); break;
            case '=': AddToken(Match('=') ? TokenType.EqualEqual : TokenType.Equal); break;
            case '<': AddToken(Match('=') ? TokenType.LessEqual : TokenType.Less); break;
            case '>': AddToken(Match('=') ? TokenType.GreaterEqual : TokenType.Greater); break;
            case '/':
                if (Match('/'))
                {
                    while (Peek() != '\n' && !IsAtEnd()) Advance();
                }
                else
                {
                    AddToken(TokenType.Slash);
                }

                break;
            case ' ' or '\r' or '\t':
                // Ignore whitespace.
                break;
            case '\n':
                line += 1;
                break;
            case '"': String(); break;
            default:
                if (IsDigit(c))
                {
                    Number();
                }
                else if (IsAlpha(c))
                {
                    Identifier();
                }
                else
                {
                    Program.Error(line, "Unexpected character.");
                }

                break;
        }
    }

    private char Advance()
    {
        var character = source[current];
        current += 1;

        return character;
    }

    private bool Match(char expected)
    {
        if (IsAtEnd())
        {
            return false;
        }

        if (source[current] != expected)
        {
            return false;
        }

        current += 1;
        return true;
    }

    private char Peek()
    {
        if (IsAtEnd())
        {
            return '\0';
        }

        return source[current];
    }

    private char PeekNext()
    {
        if (current + 1 >= source.Length)
        {
            return '\0';
        }

        return source[current + 1];
    }

    private void String()
    {
        while (Peek() != '"' && !IsAtEnd())
        {
            if (Peek() == '\n')
            {
                line += 1;
            }

            Advance();
        }

        if (IsAtEnd())
        {
            Program.Error(line, "Unterminated string.");
            return;
        }

        // Read the closing "
        Advance();

        // Trim the surrounding quotes.
        var value = source[(start + 1)..(current - 1)];
        AddToken(TokenType.String, value);
    }

    private void Number()
    {
        while (IsDigit(Peek()))
        {
            Advance();
        }

        // Look for a fractional part
        if (Peek() is '.' && IsDigit(PeekNext()))
        {
            // Consume the .
            Advance();

            // Consume the rest of the numbers.
            while (IsDigit(Peek()))
            {
                Advance();
            }
        }

        AddToken(TokenType.Number, double.Parse(source[start..current]));
    }

    private void Identifier()
    {
        while (IsAlphaNumeric(Peek()))
        {
            Advance();
        }

        var text = source[start..current];
        var tokenType = keywords.GetValueOrDefault(text, TokenType.Identifier);
        
        AddToken(tokenType);
    }

    private void AddToken(TokenType type, object? literal = null)
    {
        var text = source[start..current];
        tokens.Add(new Token(type, text, literal, line));
    }

    private bool IsAtEnd()
    {
        return current >= source.Length;
    }

    private static bool IsDigit(char c)
    {
        return c is >= '0' and <= '9';
    }

    private static bool IsAlpha(char c)
    {
        return c is (>= 'a' and <= 'z') or (>= 'A' and <= 'Z') or '_';
    }

    private static bool IsAlphaNumeric(char c)
    {
        return IsDigit(c) || IsAlpha(c);
    }
}