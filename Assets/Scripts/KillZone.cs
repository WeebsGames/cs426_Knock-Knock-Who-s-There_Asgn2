using UnityEngine;

public class KillZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        PlayerNetworkState player = other.GetComponentInParent<PlayerNetworkState>();
        if (player == null) return;

        // Owner sends request, server teleports to checkpoint.
        if (!player.IsOwner) return;

        Debug.Log($"[KillZone] Local fall detected. clientId={player.OwnerClientId}, roomProgress={player.CurrentRoomIndex}");
        player.RequestRespawnServerRpc();
    }
}
