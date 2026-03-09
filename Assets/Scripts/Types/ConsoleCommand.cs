public abstract class ConsoleCommand
{
    public abstract string Command { get; }
    public abstract string Name { get; }
    public abstract string Description { get; }
    public abstract void Execute(string[] arguments);
}