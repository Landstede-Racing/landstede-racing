using System.Collections.Generic;
using UnityEngine;

public class ConsoleManager : MonoBehaviour
{
    public static ConsoleManager Instance { get; private set; }

    private List<ConsoleEntry> entries = new();
    private Dictionary<string, ConsoleCommand> commands = new();
    [SerializeField] private bool isOpened = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }


}