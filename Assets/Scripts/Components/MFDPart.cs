using UnityEngine;

public class MFDPart : MonoBehaviour
{
    [SerializeField] private bool shouldUpdate = true;

    public bool ShouldUpdate()
    {
        return shouldUpdate;
    }
}