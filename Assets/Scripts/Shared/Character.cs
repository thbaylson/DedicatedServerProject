using System;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

namespace Shared
{
    public class Character : NetworkBehaviour
    {
        public static Character LocalCharacter { get; private set; }

        // Character, these first 3 NetworkVariables, and Data all seem to track the same data in three different ways. Why?
        public NetworkVariable<FixedString32Bytes> Name;
        public NetworkVariable<CharacterClass> CharacterClass;
        public NetworkVariable<long> Experience;
        public PersistedCharacterData Data { get; private set; }

        [SerializeField] private NavMeshAgent _navMeshAgent;

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
            if (IsOwner)
            {
                LocalCharacter = this;
            }
        }

        public void SetDestinationOnNavMesh(Vector3 navHitPosition)
        {
            _navMeshAgent.SetDestination(navHitPosition);
        }

        /// <summary>
        /// This is a mapper. It maps PersistedCharacterData to Character
        /// </summary>
        /// <param name="persistedCharacterData"></param>
        public void Bind(PersistedCharacterData persistedCharacterData)
        {
            Data = persistedCharacterData;
            Name = new NetworkVariable<FixedString32Bytes>(persistedCharacterData.Name);
            Experience = new NetworkVariable<long>(persistedCharacterData.Experience);

            Enum.TryParse<CharacterClass>(persistedCharacterData.Class, out var characterClass);
            CharacterClass = new NetworkVariable<CharacterClass>(characterClass);
        }

        public void AddExperience(long amountToAdd) => AddExperienceServerRpc(amountToAdd);

        [ServerRpc]
        private void AddExperienceServerRpc(long amountToAdd)
        {
            // Why are we tracking the same data across multiple objects? This is forcing us to take measures to keep them in sync.
            // PersistedCharacterData. *Important: As the name implies, this is the only object that persists.
            Data.Experience += amountToAdd;
            // NetworkVariable. What's the point of the NetworkVariables when we already have PersistedCharacterData?
            Experience.Value = Data.Experience;
        }
    }
}
