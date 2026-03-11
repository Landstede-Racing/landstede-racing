using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ConsoleManager : MonoBehaviour
{
    public static ConsoleManager Instance { get; private set; }

    private List<ConsoleEntry> entries = new();
    private Dictionary<string, ConsoleCommand> commands = new();
    [SerializeField] private bool isOpened = false;
    [SerializeField] private GameObject consolePrefab;
    [SerializeField] private GameObject consoleObject;

    private void Awake()
    {
        commands.Add("enableReset", new EnableResetCommand());

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void ToggleConsole()
    {
        if(isOpened)
        {
            Destroy(consoleObject);
            consoleObject = null;
        } else
        {
            consoleObject = Instantiate(consolePrefab);
        }

        isOpened = !isOpened;
    }

    public void ParseConsoleInput(string input)
    {
        entries.Add(new(ConsoleEntryType.USER, input));
        string[] inputParts = input.Split("");
        
        ConsoleCommand command = commands[inputParts[0]];
        if(command == null)
        {
            entries.Add(new(ConsoleEntryType.SYSTEM, $"Command {command} not found"));
            return;
        }

        command.Execute(inputParts.Skip(1).ToArray());
    }

    public void AddConsoleEntry(ConsoleEntry entry)
    {
        entries.Add(entry);
        if(isOpened && consoleObject != null)
        {
            consoleObject.GetComponent<ConsoleController>().UpdateEntries();
        }
    }

    public bool IsOpened()
    {
        return isOpened;
    }

    public List<ConsoleEntry> GetConsoleEntries()
    {
        return entries;
    }

    public Dictionary<string, ConsoleCommand> GetConsoleCommands()
    {
        return commands;
    }
}