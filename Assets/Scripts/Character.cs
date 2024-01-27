using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

public class Character : NetworkBehaviour
{
    public NetworkVariable<FixedString32Bytes> Name;

    [SerializeField] private NavMeshAgent _navMeshAgent;

    private void OnValidate()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Awake()
    {
        Name = new NetworkVariable<FixedString32Bytes>();
    }

    public void SetDestinationOnNavMesh(Vector3 navHitPosition)
    {
        _navMeshAgent.SetDestination(navHitPosition);
    }
}
