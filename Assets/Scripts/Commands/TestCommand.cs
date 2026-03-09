public class TestCommand : ConsoleCommand
{
    public override string Command => "test";

    public override string Name => "Test";

    public override string Description => "Veri epic test command.";

    public override void Execute(string[] arguments)
    {
        ConsoleManager.Instance.AddConsoleEntry(new(ConsoleEntryType.SYSTEM, "Veri test"));
        CustomLogger.Log("Yay :D");
    }
}