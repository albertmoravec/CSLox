namespace CSLox;

class Program
{
    private static bool hadError = false;

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

        foreach (var token in tokens)
        {
            Console.WriteLine(token);
        }
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
}