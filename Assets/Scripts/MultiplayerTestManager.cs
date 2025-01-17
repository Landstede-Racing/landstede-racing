using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class MultiplayerTestManager : MonoBehaviour
{
    private VisualElement _rootVisualElement;
    private Button _hostButton;
    private Button _clientButton;
    private Button _serverButton;
    private Button _moveButton;
    private Label _statusLabel;

    private void OnEnable()
    {
        var uiDocument = GetComponent<UIDocument>();
        _rootVisualElement = uiDocument.rootVisualElement;

        _hostButton = CreateButton("HostButton", "Host");
        _clientButton = CreateButton("ClientButton", "Client");
        _serverButton = CreateButton("ServerButton", "Server");
        _moveButton = CreateButton("MoveButton", "Move");
        _statusLabel = CreateLabel("StatusLabel", "Not Connected");

        _rootVisualElement.Clear();
        _rootVisualElement.Add(_hostButton);
        _rootVisualElement.Add(_clientButton);
        _rootVisualElement.Add(_serverButton);
        _rootVisualElement.Add(_moveButton);
        _rootVisualElement.Add(_statusLabel);

        _hostButton.clicked += OnHostButtonClicked;
        _clientButton.clicked += OnClientButtonClicked;
        _serverButton.clicked += OnServerButtonClicked;
        _moveButton.clicked += SubmitNewPosition;
    }

    void Update()
    {
        UpdateUI();
    }

    private void OnDisable()
    {
        _hostButton.clicked -= OnHostButtonClicked;
        _clientButton.clicked -= OnClientButtonClicked;
        _serverButton.clicked -= OnServerButtonClicked;
        _moveButton.clicked -= SubmitNewPosition;
    }

    private static void OnHostButtonClicked() => NetworkManager.Singleton.StartHost();

    private static void OnClientButtonClicked() => NetworkManager.Singleton.StartClient();

    private static void OnServerButtonClicked() => NetworkManager.Singleton.StartServer();

    // Disclaimer: This is not the recommended way to create and stylize the UI elements, it is only utilized for the sake of simplicity.
    // The recommended way is to use UXML and USS. Please see this link for more information: https://docs.unity3d.com/Manual/UIE-USS.html
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
            SetMoveButton(false);
            SetStatusText("NetworkManager not found");
            return;
        }

        if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
        {
            SetStartButtons(true);
            SetMoveButton(false);
            SetStatusText("Not connected");
        }
        else
        {
            SetStartButtons(false);
            SetMoveButton(true);
            UpdateStatusLabels();
        }
    }

    private void SetStartButtons(bool state)
    {
        _hostButton.style.display = state ? DisplayStyle.Flex : DisplayStyle.None;
        _clientButton.style.display = state ? DisplayStyle.Flex : DisplayStyle.None;
        _serverButton.style.display = state ? DisplayStyle.Flex : DisplayStyle.None;
    }

    private void SetMoveButton(bool state)
    {
        _moveButton.style.display = state ? DisplayStyle.Flex : DisplayStyle.None;
        if (state)
        {
            _moveButton.text = NetworkManager.Singleton.IsServer ? "Move" : "Request Position Change";
        }
    }

    private void SetStatusText(string text) => _statusLabel.text = text;

    private void UpdateStatusLabels()
    {
        var mode = NetworkManager.Singleton.IsHost ? "Host" : NetworkManager.Singleton.IsServer ? "Server" : "Client";
        string transport = "Transport: " + NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetType().Name;
        string modeText = "Mode: " + mode;
        SetStatusText($"{transport}\n{modeText}");
    }

    void SubmitNewPosition()
    {
        if (NetworkManager.Singleton.IsServer && !NetworkManager.Singleton.IsClient)
        {
            foreach (ulong uid in NetworkManager.Singleton.ConnectedClientsIds)
            {
                var playerObject = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(uid);
                var player = playerObject.GetComponent<MultiplayerTestPlayer>();
                player.Move();
            }
        }
        else if (NetworkManager.Singleton.IsClient)
        {
            var playerObject = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject();
            var player = playerObject.GetComponent<MultiplayerTestPlayer>();
            player.Move();
        }
    }
}