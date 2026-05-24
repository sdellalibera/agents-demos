namespace AgentsDemos;

public static class ConsoleHelpers
{
    public static void Pause(string label)
    {
        Console.WriteLine();
        Console.Write($"-- press Enter to {label} --");
        Console.ReadLine();
    }
}
