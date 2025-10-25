namespace CSLox;

class Program
{
    private static bool hadError = false;
    private static bool hadRuntimeError = false;

    public static void Main(string[] args)
    {
        switch (args.Length)
        {
            case > 1:
                Console.WriteLine("Usage: cslox [script]");
                Environment.Exit(64);
                break;
            case 1:
                RunFile(args[0]);
                break;
            default:
                RunPrompt();
                break;
        }
    }

    public static void RunFile(string path)
    {
        var content = File.ReadAllText(path);
        Run(content);

        // Indicate an error in the exit code.
        if (hadError)
        {
            Environment.Exit(65);
        }

        if (hadRuntimeError)
        {
            Environment.Exit(70);
        }
    }

    public static void RunPrompt()
    {
        while (true)
        {
            Console.Write("> ");

            var line = Console.ReadLine();
            if (line == null)
            {
                break;
            }

            Run(line);
            hadError = false;
        }
    }

    public static void Run(string source)
    {
        var scanner = new Scanner(source);
        var tokens = scanner.ScanTokens();
        var parser = new Parser(tokens);
        var expression = parser.Parse();

        // Stop if there was a syntax error.
        if (hadError)
        {
            return;
        }

        Console.WriteLine(new AstPrinter().Print(expression));
    }

    public static void Error(int line, string message)
    {
        Report(line, "", message);
    }

    public static void Report(int line, string where, string message)
    {
        Console.WriteLine($"[line {line}] Error{where}: {message}");
        hadError = true;
    }

    public static void Error(Token token, string message)
    {
        if (token.Type == TokenType.Eof)
        {
            Report(token.Line, " at end", message);
        }
        else
        {
            Report(token.Line, $" at '{token.Lexeme}'", message);
        }
    }

    public static void RuntimeError(RuntimeError error)
    {
        Console.WriteLine($"{error.Message}\n[line {error.Token.Line}]");
        hadRuntimeError = true;
    }
}
