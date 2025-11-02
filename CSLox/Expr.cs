namespace CSLox;

public abstract class Expr {
    public interface IVisitor<TR> {
        TR VisitAssignExpr(Assign expr);
        TR VisitBinaryExpr(Binary expr);
        TR VisitGroupingExpr(Grouping expr);
        TR VisitLiteralExpr(Literal expr);
        TR VisitLogicalExpr(Logical expr);
        TR VisitUnaryExpr(Unary expr);
        TR VisitVariableExpr(Variable expr);
    }

    public class Assign(Token name, Expr value) : Expr {
        public override TR Accept<TR>(IVisitor<TR> visitor)
        {
            return visitor.VisitAssignExpr(this);
        }

        public Token Name { get; set; } = name;
        public Expr Value { get; set; } = value;
    }
    public class Binary(Expr left, Token oper, Expr right) : Expr {
        public override TR Accept<TR>(IVisitor<TR> visitor)
        {
            return visitor.VisitBinaryExpr(this);
        }

        public Expr Left { get; set; } = left;
        public Token Oper { get; set; } = oper;
        public Expr Right { get; set; } = right;
    }
    public class Grouping(Expr expression) : Expr {
        public override TR Accept<TR>(IVisitor<TR> visitor)
        {
            return visitor.VisitGroupingExpr(this);
        }

        public Expr Expression { get; set; } = expression;
    }
    public class Literal(object? value) : Expr {
        public override TR Accept<TR>(IVisitor<TR> visitor)
        {
            return visitor.VisitLiteralExpr(this);
        }

        public object? Value { get; set; } = value;
    }
    public class Logical(Expr left, Token oper, Expr right) : Expr {
        public override TR Accept<TR>(IVisitor<TR> visitor)
        {
            return visitor.VisitLogicalExpr(this);
        }

        public Expr Left { get; set; } = left;
        public Token Oper { get; set; } = oper;
        public Expr Right { get; set; } = right;
    }
    public class Unary(Token oper, Expr right) : Expr {
        public override TR Accept<TR>(IVisitor<TR> visitor)
        {
            return visitor.VisitUnaryExpr(this);
        }

        public Token Oper { get; set; } = oper;
        public Expr Right { get; set; } = right;
    }
    public class Variable(Token name) : Expr {
        public override TR Accept<TR>(IVisitor<TR> visitor)
        {
            return visitor.VisitVariableExpr(this);
        }

        public Token Name { get; set; } = name;
    }

    public abstract TR Accept<TR>(IVisitor<TR> visitor);
}
