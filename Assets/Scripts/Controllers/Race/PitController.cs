using UnityEngine;
using Unity.Netcode;
using System.Collections;

public class PitController : NetworkBehaviour
{
    private readonly float PIT_STOP_SPEED_THRESHOLD = 1;
    private readonly float POST_PIT_WAIT_TIME = 2;

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
        EventService.CarEnteredPit -= OnCarEnteredPit;
        EventService.CarExitedPit -= OnCarExitedPit;
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
        yield return new WaitWhile(() => vehicleController.GetSpeed() > PIT_STOP_SPEED_THRESHOLD);

        EventService.InvokePitStopStart();
        float minStop = 3;
        float maxStop = 5;
        TireCompound newCompound = vehicleController.nextCompound;

        if (vehicleController.replaceWing)
        {
            minStop += 3;
            maxStop += 6;
        }

        float stopLength = Random.Range(minStop, maxStop);
        yield return new WaitForSecondsRealtime(stopLength);

        vehicleController.SetTires(newCompound);

        if (vehicleController.replaceWing)
        {
            DamageablePart leftFrontWing = vehicleController.GetDamageablePart(Locations.FrontLeftWing);
            DamageablePart rightFrontWing = vehicleController.GetDamageablePart(Locations.FrontRightWing);

            leftFrontWing.RepairPart();
            rightFrontWing.RepairPart();
        }

        EventService.InvokePitStopEnd();
        stopInProgress = false;

        yield return new WaitForSecondsRealtime(POST_PIT_WAIT_TIME);
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