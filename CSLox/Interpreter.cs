using System.Globalization;

namespace CSLox;

public class Interpreter : Expr.IVisitor<object?>, Stmt.IVisitor<object?>
{
    private Environment environment = new();

    public void Interpret(List<Stmt> statements)
    {
        try
        {
            foreach (var statement in statements)
            {
                Execute(statement);
            }
        }
        catch (RuntimeError e)
        {
            Program.RuntimeError(e);
        }
    }

    public object? VisitAssignExpr(Expr.Assign expr)
    {
        var value = Evaluate(expr.Value);
        environment.Assign(expr.Name, value);
        return value;
    }

    public object? VisitBinaryExpr(Expr.Binary expr)
    {
        var left = Evaluate(expr.Left);
        var right = Evaluate(expr.Right);

        switch (expr.Oper.Type)
        {
            case TokenType.Greater:
                CheckNumberOperands(expr.Oper, left, right);
                return (double)left > (double)right;
            case TokenType.GreaterEqual:
                CheckNumberOperands(expr.Oper, left, right);
                return (double)left >= (double)right;
            case TokenType.Less:
                CheckNumberOperands(expr.Oper, left, right);
                return (double)left < (double)right;
            case TokenType.LessEqual:
                CheckNumberOperands(expr.Oper, left, right);
                return (double)left <= (double)right;
            case TokenType.BangEqual:
                return !IsEqual(left, right);
            case TokenType.EqualEqual:
                return IsEqual(left, right);
            case TokenType.Minus:
                CheckNumberOperands(expr.Oper, left, right);
                return (double)left - (double)right;
            case TokenType.Slash:
                CheckNumberOperands(expr.Oper, left, right);
                return (double)left / (double)right;
            case TokenType.Star:
                CheckNumberOperands(expr.Oper, left, right);
                return (double)left * (double)right;
            case TokenType.Plus when left is string leftStr && right is string rightStr:
                return leftStr + rightStr;
            case TokenType.Plus when left is double leftNum && right is double rightNum:
                return leftNum + rightNum;
            case TokenType.Plus:
                // Neither Plus handler matched.
                throw new RuntimeError(expr.Oper, "Operands must be two numbers or two strings.");
        }

        // Unreachable.
        return null;
    }

    public object? VisitGroupingExpr(Expr.Grouping expr)
    {
        return Evaluate(expr.Expression);
    }

    public object? VisitLiteralExpr(Expr.Literal expr)
    {
        return expr.Value;
    }

    public object? VisitUnaryExpr(Expr.Unary expr)
    {
        var right = Evaluate(expr.Right);

        switch (expr.Oper.Type)
        {
            case TokenType.Bang:
                return !IsTruthy(right);
            case TokenType.Minus:
                CheckNumberOperand(expr.Oper, right);
                return -(double)right;
        }

        // Unreachable.
        return null;
    }

    private void CheckNumberOperand(Token @operator, object? operand)
    {
        if (operand is double)
        {
            return;
        }

        throw new RuntimeError(@operator, "Operand must be a number.");
    }

    private void CheckNumberOperands(Token @operator, object? left, object? right)
    {
        if (left is double && right is double)
        {
            return;
        }

        throw new RuntimeError(@operator, "Operands must be numbers.");
    }


    private bool IsTruthy(object? obj)
    {
        if (obj == null)
        {
            return false;
        }

        if (obj is bool val)
        {
            return val;
        }

        return true;
    }

    private bool IsEqual(object? a, object? b)
    {
        if (a == null && b == null)
        {
            return true;
        }

        if (a == null)
        {
            return false;
        }

        return a.Equals(b);
    }

    private string Stringify(object? obj)
    {
        if (obj == null)
        {
            return "nil";
        }

        if (obj is double objNum)
        {
            var text = objNum.ToString(CultureInfo.InvariantCulture);
            if (text.EndsWith(".0"))
            {
                text = text[..^2];
            }

            return text;
        }

        return obj.ToString();
    }

    public object? VisitExpressionStmt(Stmt.Expression stmt)
    {
        Evaluate(stmt.Expr);
        return null;
    }

    public object? VisitPrintStmt(Stmt.Print stmt)
    {
        var value = Evaluate(stmt.Expr);
        Console.WriteLine(Stringify(value));

        return null;
    }

    public object? VisitVarStmt(Stmt.Var stmt)
    {
        object? value = null;
        if (stmt.Initializer != null)
        {
            value = Evaluate(stmt.Initializer);
        }

        environment.Define(stmt.Name.Lexeme, value);
        return null;
    }

    public object? VisitVariableExpr(Expr.Variable expr)
    {
        return environment.Get(expr.Name);
    }

    public object? VisitBlockStmt(Stmt.Block stmt)
    {
        ExecuteBlock(stmt.Statements, new Environment(environment));
        return null;
    }

    private object? Evaluate(Expr expr)
    {
        return expr.Accept(this);
    }

    private void Execute(Stmt statement)
    {
        statement.Accept(this);
    }

    private void ExecuteBlock(List<Stmt> statements, Environment blockEnvironment)
    {
        var previous = environment;

        try
        {
            environment = blockEnvironment;

            foreach (var statement in statements)
            {
                Execute(statement);
            }
        }
        finally
        {
            environment = previous;
        }
    }
}