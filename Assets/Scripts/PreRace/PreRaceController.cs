using Unity.Netcode;
using UnityEngine;

public class PreRaceController : NetworkBehaviour
{
    [SerializeField] private PreRaceUI preRaceUI;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsOwner)
        {
            preRaceUI.Show();
        }
        else
        {
            preRaceUI.Hide();
        }
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        preRaceUI.Hide();
    }
}