using System;

public class ConsoleEntry
{
    public readonly DateTime DateTime;
    public readonly ConsoleEntryType Type;
    public readonly string Content;
    public ConsoleEntry(ConsoleEntryType Type, string Content)
    {
        DateTime = DateTime.Now;
        this.Type = Type;
        this.Content = Content;
    }
}

public enum ConsoleEntryType
{
    USER,
    SYSTEM
}