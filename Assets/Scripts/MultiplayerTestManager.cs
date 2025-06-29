using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class MultiplayerTestManager : MonoBehaviour
{
    private Button _clientButton;
    private Button _hostButton;
    private VisualElement _rootVisualElement;
    private Button _serverButton;
    private Label _statusLabel;

    private void Update()
    {
        UpdateUI();
    }

    private void OnEnable()
    {
        var uiDocument = GetComponent<UIDocument>();
        _rootVisualElement = uiDocument.rootVisualElement;

        _hostButton = CreateButton("HostButton", "Host");
        _clientButton = CreateButton("ClientButton", "Client");
        _serverButton = CreateButton("ServerButton", "Server");
        _statusLabel = CreateLabel("StatusLabel", "Not Connected");

        _rootVisualElement.Clear();
        _rootVisualElement.Add(_hostButton);
        _rootVisualElement.Add(_clientButton);
        _rootVisualElement.Add(_serverButton);
        _rootVisualElement.Add(_statusLabel);

        _hostButton.clicked += OnHostButtonClicked;
        _clientButton.clicked += OnClientButtonClicked;
        _serverButton.clicked += OnServerButtonClicked;
    }

    private void OnDisable()
    {
        _hostButton.clicked -= OnHostButtonClicked;
        _clientButton.clicked -= OnClientButtonClicked;
        _serverButton.clicked -= OnServerButtonClicked;
    }

    private static void OnHostButtonClicked()
    {
        NetworkManager.Singleton.StartHost();
    }

    private static void OnClientButtonClicked()
    {
        NetworkManager.Singleton.StartClient();
    }

    private static void OnServerButtonClicked()
    {
        NetworkManager.Singleton.StartServer();
    }

    private static Button CreateButton(string name, string text)
    {
        var button = new Button
        {
            name = name,
            text = text,
            style =
            {
                width = 240,
                backgroundColor = Color.white,
                color = Color.black,
                unityFontStyleAndWeight = FontStyle.Bold
            }
        };
        return button;
    }

    private static Label CreateLabel(string name, string content)
    {
        var label = new Label
        {
            name = name,
            text = content,
            style =
            {
                color = Color.black,
                fontSize = 18
            }
        };
        return label;
    }

    private void UpdateUI()
    {
        if (!NetworkManager.Singleton)
        {
            SetStartButtons(false);
            SetStatusText("NetworkManager not found");
            return;
        }

        if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
        {
            SetStartButtons(true);
            SetStatusText("Not connected");
        }
        else
        {
            SetStartButtons(false);
            UpdateStatusLabels();
        }
    }

    private void SetStartButtons(bool state)
    {
        _hostButton.style.display = state ? DisplayStyle.Flex : DisplayStyle.None;
        _clientButton.style.display = state ? DisplayStyle.Flex : DisplayStyle.None;
        _serverButton.style.display = state ? DisplayStyle.Flex : DisplayStyle.None;
    }

    private void SetStatusText(string text)
    {
        _statusLabel.text = text;
    }

    private void UpdateStatusLabels()
    {
        var mode = NetworkManager.Singleton.IsHost ? "Host" : NetworkManager.Singleton.IsServer ? "Server" : "Client";
        var transport = "Transport: " + NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetType().Name;
        var modeText = "Mode: " + mode;
        SetStatusText($"{transport}\n{modeText}");
    }
}