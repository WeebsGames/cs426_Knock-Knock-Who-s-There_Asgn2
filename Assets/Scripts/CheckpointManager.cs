using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    [Header("Checkpoint Setup")]
    public Transform startCheckpoint;
    public Transform[] roomCheckpoints = new Transform[6];

    public void TeleportPlayerToStart(PlayerNetworkState player)
    {
        if (player == null || startCheckpoint == null) return;
        Debug.Log($"[Checkpoint] Teleport to START for clientId={player.OwnerClientId}");
        player.TeleportTo(startCheckpoint.position);
    }

    public void TeleportPlayerToRoom(PlayerNetworkState player, int roomIndex)
    {
        if (player == null) return;
        if (roomIndex < 0 || roomIndex >= roomCheckpoints.Length) return;
        if (roomCheckpoints[roomIndex] == null) return;

        Debug.Log($"[Checkpoint] Teleport to ROOM checkpoint. clientId={player.OwnerClientId}, roomIndex={roomIndex}");
        player.TeleportTo(roomCheckpoints[roomIndex].position);
    }

    public void TeleportToLastCheckpoint(PlayerNetworkState player)
    {
        if (player == null) return;

        // currentRoomIndex = next room the player needs to clear.
        // 0 means they are still at start.
        int nextRoom = player.CurrentRoomIndex;

        if (nextRoom <= 0)
        {
            Debug.Log($"[Checkpoint] Last checkpoint is START. clientId={player.OwnerClientId}");
            TeleportPlayerToStart(player);
            return;
        }

        int checkpointIndex = Mathf.Clamp(nextRoom, 0, roomCheckpoints.Length - 1);
        Debug.Log($"[Checkpoint] Last checkpoint room for clientId={player.OwnerClientId} is roomIndex={checkpointIndex}");
        TeleportPlayerToRoom(player, checkpointIndex);
    }
}
