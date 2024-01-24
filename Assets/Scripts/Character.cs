using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class Character : NetworkBehaviour
{
    public NetworkVariable<FixedString32Bytes> Name;

    private void Awake()
    {
        Name = new NetworkVariable<FixedString32Bytes>();
    }
}
