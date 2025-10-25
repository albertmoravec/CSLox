using System.Text.RegularExpressions;

namespace CSLox;

public class Parser(List<Token> tokens)
{
    private class ParseError : ApplicationException;

    private int current = 0;

    public Expr? Parse()
    {
        try
        {
            return Expression();
        }
        catch (ParseError e)
        {
            return null;
        }
    }

    private Expr Expression()
    {
        return Equality();
    }

    private Expr Equality()
    {
        var expression = Comparison();

        while (Match(TokenType.BangEqual, TokenType.EqualEqual))
        {
            var @operator = Previous();
            var right = Comparison();
            expression = new Expr.Binary(expression, @operator, right);
        }

        return expression;
    }

    private Expr Comparison()
    {
        var expression = Term();

        while (Match(TokenType.Greater, TokenType.GreaterEqual, TokenType.Less, TokenType.LessEqual))
        {
            var @operator = Previous();
            var right = Term();
            expression = new Expr.Binary(expression, @operator, right);
        }

        return expression;
    }

    private Expr Term()
    {
        var expression = Factor();

        while (Match(TokenType.Minus, TokenType.Plus))
        {
            var @operator = Previous();
            var right = Factor();
            expression = new Expr.Binary(expression, @operator, right);
        }

        return expression;
    }

    private Expr Factor()
    {
        var expression = Unary();

        while (Match(TokenType.Slash, TokenType.Star))
        {
            var @operator = Previous();
            var right = Unary();
            expression = new Expr.Binary(expression, @operator, right);
        }

        return expression;
    }

    private Expr Unary()
    {
        if (Match(TokenType.Bang, TokenType.Minus))
        {
            var @operator = Previous();
            var right = Unary();
            return new Expr.Unary(@operator, right);
        }

        return Primary();
    }

    private Expr Primary()
    {
        if (Match(TokenType.False))
        {
            return new Expr.Literal(false);
        }

        if (Match(TokenType.True))
        {
            return new Expr.Literal(false);
        }

        if (Match(TokenType.Nil))
        {
            return new Expr.Literal(null);
        }

        if (Match(TokenType.Number, TokenType.String))
        {
            return new Expr.Literal(Previous().Literal);
        }

        if (Match(TokenType.LeftParen))
        {
            var expression = Expression();
            Consume(TokenType.RightParen, "Expect ')' after expression.");

            return new Expr.Grouping(expression);
        }

        throw Error(Peek(), "Expect expression.");
    }

    private Token Advance()
    {
        if (!IsAtEnd())
        {
            current++;
        }

        return Previous();
    }

    private Token Consume(TokenType type, string message)
    {
        if (Check(type))
        {
            return Advance();
        }

        throw Error(Peek(), message);
    }

    private bool Match(params TokenType[] types)
    {
        foreach (var tokenType in types)
        {
            if (Check(tokenType))
            {
                Advance();
                return true;
            }
        }

        return false;
    }

    private bool Check(TokenType tokenType)
    {
        if (IsAtEnd())
        {
            return false;
        }

        return Peek().Type == tokenType;
    }

    private Token Peek()
    {
        return tokens[current];
    }

    private Token Previous()
    {
        return tokens[current - 1];
    }

    private ParseError Error(Token token, string message)
    {
        Program.Error(token, message);
        return new ParseError();
    }

    private void Synchronize()
    {
        Advance();

        while (!IsAtEnd())
        {
            if (Previous().Type == TokenType.Semicolon)
            {
                return;
            }

            switch (Peek().Type)
            {
                case TokenType.Class or TokenType.Fun or TokenType.Var or TokenType.For or TokenType.If
                    or TokenType.While or TokenType.Print or TokenType.Return:
                    return;
            }

            Advance();
        }
    }

    private bool IsAtEnd()
    {
        return Peek().Type == TokenType.Eof;
    }
}