namespace CSLox;

public abstract class Stmt {
    public interface IVisitor<TR> {
        TR VisitBlockStmt(Block stmt);
        TR VisitExpressionStmt(Expression stmt);
        TR VisitIfStmt(If stmt);
        TR VisitPrintStmt(Print stmt);
        TR VisitVarStmt(Var stmt);
    }

    public class Block(List<Stmt> statements) : Stmt {
        public override TR Accept<TR>(IVisitor<TR> visitor)
        {
            return visitor.VisitBlockStmt(this);
        }

        public List<Stmt> Statements { get; set; } = statements;
    }
    public class Expression(Expr expr) : Stmt {
        public override TR Accept<TR>(IVisitor<TR> visitor)
        {
            return visitor.VisitExpressionStmt(this);
        }

        public Expr Expr { get; set; } = expr;
    }
    public class If(Expr condition, Stmt thenBranch, Stmt? elseBranch) : Stmt {
        public override TR Accept<TR>(IVisitor<TR> visitor)
        {
            return visitor.VisitIfStmt(this);
        }

        public Expr Condition { get; set; } = condition;
        public Stmt ThenBranch { get; set; } = thenBranch;
        public Stmt? ElseBranch { get; set; } = elseBranch;
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
