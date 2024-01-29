using System;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.TextCore.Text;

public class Character : NetworkBehaviour
{
    public NetworkVariable<FixedString32Bytes> Name;
    public NetworkVariable<long> Experience;
    public NetworkVariable<CharacterClass> CharacterClass;

    [SerializeField] private NavMeshAgent _navMeshAgent;
    
    private PersistedCharacterData _data;

    private void OnValidate()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Awake()
    {
        // Initialize variables if we are not the server?
        if (!NetworkManager.Singleton.IsServer)
        {
            Name = new NetworkVariable<FixedString32Bytes>();
            Experience = new NetworkVariable<long>();
            CharacterClass = new NetworkVariable<CharacterClass>();
        }
    }

    private void Start()
    {
        gameObject.name = $"Character {Name.Value} {CharacterClass.Value} {Experience.Value}";
    }

    public void SetDestinationOnNavMesh(Vector3 navHitPosition)
    {
        _navMeshAgent.SetDestination(navHitPosition);
    }

    public void Bind(PersistedCharacterData persistedCharacterData)
    {
        _data = persistedCharacterData;
        Name = new NetworkVariable<FixedString32Bytes>(_data.Name);
        Experience = new NetworkVariable<long>(_data.Experience);

        Enum.TryParse<CharacterClass>(_data.Class, out var characterClass);
        CharacterClass = new NetworkVariable<CharacterClass>(characterClass);
    }
}
