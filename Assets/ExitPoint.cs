using Shared;
using Unity.Netcode;
using UnityEngine;

public class ExitPoint : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (NetworkManager.Singleton.IsServer)
        {
            if(other.TryGetComponent(out Character serverCharacter))
            {
                var player = NetworkManager.Singleton.ConnectedClients[serverCharacter.OwnerClientId].PlayerObject.GetComponent<Player>();
                player.LeaveServer();
            }
        }
    }
}
