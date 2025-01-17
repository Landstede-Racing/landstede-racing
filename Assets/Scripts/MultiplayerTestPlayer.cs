using Unity.Netcode;
using UnityEngine;

public class MultiplayerTestPlayer : NetworkBehaviour
{
    public NetworkVariable<Vector3> position = new NetworkVariable<Vector3>();

    public override void OnNetworkSpawn()
    {
        position.OnValueChanged += OnStateChanged;

        if (IsOwner)
        {
            Move();
        }
    }

    public override void OnNetworkDespawn()
    {
        position.OnValueChanged -= OnStateChanged;
    }

    public void OnStateChanged(Vector3 previous, Vector3 current)
    {
// note: `Position.Value` will be equal to `current` here
        if (position.Value != previous)
        {
            transform.position = position.Value;
        }
    }

    public void Move()
    {
        SubmitPositionRequestServerRpc();
    }

    [Rpc(SendTo.Server)]
    void SubmitPositionRequestServerRpc(RpcParams rpcParams = default)
    {
        var randomPosition = GetRandomPositionOnPlane();
        transform.position = randomPosition;
        position.Value = randomPosition;
    }

    static Vector3 GetRandomPositionOnPlane()
    {
        return new Vector3(Random.Range(-3f, 3f), 1f, Random.Range(-3f, 3f));
    }
}
