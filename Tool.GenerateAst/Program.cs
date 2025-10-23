namespace Tool.GenerateAst;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length != 1)
        {
            Console.WriteLine("Usage: GenerateAst <output_directory>");
            Environment.Exit(64);
        }

        var outputDir = args[0];
        DefineAst(outputDir, "Expr", [
            "Binary   : Expr left, Token oper, Expr right",
            "Grouping : Expr expression",
            "Literal  : object value",
            "Unary    : Token oper, Expr right"
        ]);
    }

    private static void DefineAst(string outputDir, string baseName, List<string> types)
    {
        var path = Path.Combine(outputDir, $"{baseName}.cs");
        using var writer = new StreamWriter(path);

        writer.WriteLine("namespace CSLox;");
        writer.WriteLine();
        writer.WriteLine($"public abstract class {baseName} {{");

        DefineVisitor(writer, baseName, types);

        writer.WriteLine();

        foreach (var type in types)
        {
            var className = type.Split(":")[0].Trim();
            var fields = type.Split(":")[1].Trim();

            DefineType(writer, baseName, className, fields);
        }

        writer.WriteLine();
        writer.WriteLine("    public abstract TR Accept<TR>(IVisitor<TR> visitor);");

        writer.WriteLine("}");
    }

    private static void DefineType(StreamWriter writer, string baseName, string className, string fieldList)
    {
        writer.WriteLine($"    public class {className}({fieldList}) : {baseName} {{");

        // Visitor.
        writer.WriteLine("        public override TR Accept<TR>(IVisitor<TR> visitor)");
        writer.WriteLine("        {");
        writer.WriteLine($"            return visitor.Visit{className}{baseName}(this);");
        writer.WriteLine("        }");

        writer.WriteLine();

        // Fields.
        var fields = fieldList.Split(", ");
        foreach (var field in fields)
        {
            var type = field.Split(" ")[0];
            var name = field.Split(" ")[1];
            writer.WriteLine(
                $"        public {type} {char.ToUpperInvariant(name[0])}{name[1..]} {{ get; set; }} = {name};");
        }

        writer.WriteLine("    }");
    }

    private static void DefineVisitor(StreamWriter writer, string baseName, List<string> types)
    {
        writer.WriteLine("    public interface IVisitor<TR> {");

        foreach (var type in types)
        {
            var typeName = type.Split(":")[0].Trim();
            writer.WriteLine($"        TR Visit{typeName}{baseName}({typeName} {baseName.ToLowerInvariant()});");
        }

        writer.WriteLine("    }");
    }
}
