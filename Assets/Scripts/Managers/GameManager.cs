using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private bool EnableResetButton = false;

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

    public void SetResetButtonEnabled(bool resetButtonEnabled)
    {
        EnableResetButton = resetButtonEnabled;
    }

    public bool ResetButtonEnabled()
    {
        return EnableResetButton;
    }
}