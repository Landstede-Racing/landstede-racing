public class EnableResetCommand : ConsoleCommand
{
    public override string Command => "enableReset";

    public override string Name => "Enable Reset";

    public override string Description => "Enables reset button";

    public override void Execute(string[] arguments)
    {
        GameManager.Instance.SetResetButtonEnabled(true);
        ConsoleManager.Instance.AddConsoleEntry(new(ConsoleEntryType.SYSTEM, "Reset button enabled"));
    }
}