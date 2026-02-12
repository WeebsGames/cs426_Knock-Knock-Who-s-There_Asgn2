using Unity.Netcode;
using UnityEngine;

public class VaultTrigger : NetworkBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        PlayerNetworkState player = other.GetComponentInParent<PlayerNetworkState>();
        if (player == null) return;
        if (!player.IsOwner) return;

        Debug.Log($"[Vault] Local enter by clientId={player.OwnerClientId}. Requesting finish.");
        TryFinishServerRpc(player.NetworkObjectId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void TryFinishServerRpc(ulong playerObjectId, ServerRpcParams rpcParams = default)
    {
        if (!NetworkManager.SpawnManager.SpawnedObjects.TryGetValue(playerObjectId, out var netObj)) return;

        PlayerNetworkState player = netObj.GetComponent<PlayerNetworkState>();
        if (player == null) return;

        if (player.OwnerClientId != rpcParams.Receive.SenderClientId) return;
        Debug.Log($"[Vault] Server received finish attempt. sender={rpcParams.Receive.SenderClientId}, roomProgress={player.CurrentRoomIndex}");

        if (NetworkGameManager.Instance != null)
        {
            NetworkGameManager.Instance.TryFinishRace(player.OwnerClientId);
        }
    }
}
