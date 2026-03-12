using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConsoleController : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private ScrollRect scrollView;
    [SerializeField] private GameObject scrollContent;
    [SerializeField] private GameObject entryPrefab;

    void Awake()
    {
        inputField.onSubmit.AddListener((value) => HandleInput(value));
        // TODO: Automatically focus input field
    }

    public void UpdateEntries()
    {
        for (int i = 0; i < scrollContent.transform.childCount; i++)
        {
            var child = scrollContent.transform.GetChild(i);
            Destroy(child.gameObject);
        }

        var entries = ConsoleManager.Instance.GetConsoleEntries();
        entries.ForEach((entry) =>
        {
            var go = Instantiate(entryPrefab, scrollContent.transform);
            go.GetComponent<TMP_Text>().SetText($"[{entry.DateTime:HH:mm:ss}] {entry.Type} | {entry.Content}");
        });
    }

    public void HandleInput(string input)
    {
        ConsoleManager.Instance.ParseConsoleInput(input);
    }
}