using Unity.Netcode;
using UnityEngine;

public class DoorTrigger : NetworkBehaviour
{
    public int roomIndex;   // 0..5
    public int answerIndex; // 0..3

    private void OnTriggerEnter(Collider other)
    {
        PlayerNetworkState player = other.GetComponentInParent<PlayerNetworkState>();
        if (player == null) return;

        // Only the local owner asks the server to process this door.
        if (!player.IsOwner) return;

        Debug.Log($"[DoorTrigger] Local enter. playerClientId={player.OwnerClientId}, room={roomIndex}, answer={answerIndex}");
        SubmitDoorChoiceServerRpc(player.NetworkObjectId, roomIndex, answerIndex);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SubmitDoorChoiceServerRpc(
        ulong playerObjectId,
        int submittedRoomIndex,
        int submittedAnswerIndex,
        ServerRpcParams rpcParams = default)
    {
        if (!NetworkManager.SpawnManager.SpawnedObjects.TryGetValue(playerObjectId, out var netObj)) return;

        PlayerNetworkState player = netObj.GetComponent<PlayerNetworkState>();
        if (player == null) return;

        // Basic anti-cheat check: sender must own this player object.
        if (player.OwnerClientId != rpcParams.Receive.SenderClientId) return;

        Debug.Log($"[DoorTrigger] Server received door choice. sender={rpcParams.Receive.SenderClientId}, room={submittedRoomIndex}, answer={submittedAnswerIndex}");

        if (NetworkGameManager.Instance != null)
        {
            NetworkGameManager.Instance.HandleDoorChoice(
                player.OwnerClientId,
                submittedRoomIndex,
                submittedAnswerIndex);
        }
    }
}
