namespace CSLox;

public abstract class Stmt {
    public interface IVisitor<TR> {
        TR VisitExpressionStmt(Expression stmt);
        TR VisitPrintStmt(Print stmt);
        TR VisitVarStmt(Var stmt);
    }

    public class Expression(Expr expr) : Stmt {
        public override TR Accept<TR>(IVisitor<TR> visitor)
        {
            return visitor.VisitExpressionStmt(this);
        }

        public Expr Expr { get; set; } = expr;
    }
    public class Print(Expr expr) : Stmt {
        public override TR Accept<TR>(IVisitor<TR> visitor)
        {
            return visitor.VisitPrintStmt(this);
        }

        public Expr Expr { get; set; } = expr;
    }
    public class Var(Token name, Expr? initializer) : Stmt {
        public override TR Accept<TR>(IVisitor<TR> visitor)
        {
            return visitor.VisitVarStmt(this);
        }

        public Token Name { get; set; } = name;
        public Expr? Initializer { get; set; } = initializer;
    }

    public abstract TR Accept<TR>(IVisitor<TR> visitor);
}
