using UnityEngine;
using Unity.Netcode;
using System.Collections;

public class PitController : NetworkBehaviour
{
    [SerializeField] private GameObject ownerGameObject;
    [SerializeField] private ulong ownerId = 0;
    [SerializeField] private bool stopInProgress;
    [SerializeField] private bool readyForNext = true;
    [SerializeField] private GameObject indicator;
    private NetworkVariable<ulong> m_OwnerId = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        indicator.SetActive(false);
        m_OwnerId.OnValueChanged += OwnerGameObjectChanged;

        EventService.CarEnteredPit += OnCarEnteredPit;
        EventService.CarExitedPit += OnCarExitedPit;
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        m_OwnerId.OnValueChanged -= OwnerGameObjectChanged;
    }

    public void SetOwnerGameObject(GameObject ownerGameObject)
    {
        VehicleController vehicleController = ownerGameObject.GetComponent<VehicleController>();
        if (vehicleController)
        {
            this.ownerGameObject = ownerGameObject;
            m_OwnerId.Value = vehicleController.OwnerClientId;
            ownerId = vehicleController.OwnerClientId;
        }
    }

    public GameObject GetOwnerGameObject()
    {
        return ownerGameObject;
    }

    private void OwnerGameObjectChanged(ulong previousValue, ulong newValue)
    {
        GameObject newGo = NetworkManager.SpawnManager.GetPlayerNetworkObject(newValue).gameObject;
        VehicleController vehicleController = newGo.GetComponentInChildren<VehicleController>();
        if (vehicleController)
        {
            ownerGameObject = vehicleController.gameObject;
            ownerId = newValue;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!IsClient && !IsHost) return;

        VehicleController vehicleController = other.transform.parent.GetComponent<VehicleController>();
        if (vehicleController && vehicleController.OwnerClientId == ownerId && !stopInProgress && readyForNext)
        {
            stopInProgress = true;
            readyForNext = false;
            ownerGameObject = vehicleController.gameObject;
            StartCoroutine(PitStopCoroutine());
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!IsClient && !IsHost) return;

        VehicleController vehicleController = other.transform.parent.GetComponent<VehicleController>();
        if (vehicleController && vehicleController.OwnerClientId == ownerId && stopInProgress)
        {
            stopInProgress = false;
            readyForNext = true;
            StopCoroutine(PitStopCoroutine());
        }
    }

    private IEnumerator PitStopCoroutine()
    {
        VehicleController vehicleController = ownerGameObject.GetComponent<VehicleController>();
        yield return new WaitWhile(() => vehicleController.GetSpeed() > 1);

        EventService.InvokePitStopStart();
        TireCompound newCompound = vehicleController.nextCompound;

        float stopLength = Random.Range(3, 5);
        yield return new WaitForSecondsRealtime(stopLength);

        vehicleController.SetTires(newCompound);
        EventService.InvokePitStopEnd();
        stopInProgress = false;

        yield return new WaitForSecondsRealtime(2);
        readyForNext = true;
    }

    private void OnCarEnteredPit(ulong clientId)
    {
        if (!IsClient || ownerId != clientId) return;
        indicator.SetActive(true);
    }

    private void OnCarExitedPit(ulong clientId)
    {
        if (!IsClient || ownerId != clientId) return;
        indicator.SetActive(false);
    }
}