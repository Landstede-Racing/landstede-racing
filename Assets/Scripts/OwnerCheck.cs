using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class OwnerCheck : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        if(!IsOwner || IsServer) Destroy(gameObject);
    }
}
