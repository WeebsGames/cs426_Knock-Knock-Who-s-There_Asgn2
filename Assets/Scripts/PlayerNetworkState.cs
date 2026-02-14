using Unity.Netcode;
using UnityEngine;

public class PlayerNetworkState : NetworkBehaviour
{
    private NetworkVariable<PlayerRole> role =
        new(PlayerRole.None, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private NetworkVariable<int> currentRoomIndex =
        new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private NetworkVariable<int> score =
        new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private NetworkVariable<int> lastChosenRoomIndex =
        new(-1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private NetworkVariable<int> lastChosenAnswerIndex =
        new(-1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public PlayerRole Role => role.Value;
    public int CurrentRoomIndex => currentRoomIndex.Value;
    public int Score => score.Value;
    public int LastChosenRoomIndex => lastChosenRoomIndex.Value;
    public int LastChosenAnswerIndex => lastChosenAnswerIndex.Value;

    public override void OnNetworkSpawn()
    {
        Debug.Log($"[PlayerState] Spawned. ownerClientId={OwnerClientId}, isServer={IsServer}, isOwner={IsOwner}");
        if (IsServer && NetworkGameManager.Instance != null)
        {
            NetworkGameManager.Instance.RegisterPlayer(this);
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsServer && NetworkGameManager.Instance != null)
        {
            NetworkGameManager.Instance.UnregisterPlayer(this);
        }
    }

    public void SetRole(PlayerRole newRole)
    {
        if (!IsServer) return;
        role.Value = newRole;
        Debug.Log($"[PlayerState] Role set. clientId={OwnerClientId}, role={newRole}");
    }

    public void SetCurrentRoom(int roomIndex)
    {
        if (!IsServer) return;
        currentRoomIndex.Value = Mathf.Clamp(roomIndex, 0, 6);
        Debug.Log($"[PlayerState] Room progress set. clientId={OwnerClientId}, currentRoomIndex={currentRoomIndex.Value}");
    }

    public void SetScore(int newScore)
    {
        if (!IsServer) return;
        score.Value = newScore;
        Debug.Log($"[PlayerState] Score set. clientId={OwnerClientId}, score={score.Value}");
    }

    public void AddScore(int delta)
    {
        if (!IsServer) return;
        score.Value += delta;
        Debug.Log($"[PlayerState] Score changed. clientId={OwnerClientId}, delta={delta}, newScore={score.Value}");
    }

    public void SetLastDoorChoice(int roomIndex, int answerIndex)
    {
        if (!IsServer) return;
        lastChosenRoomIndex.Value = roomIndex;
        lastChosenAnswerIndex.Value = answerIndex;
        Debug.Log($"[PlayerState] Last door choice set. clientId={OwnerClientId}, room={roomIndex}, answer={answerIndex}");
    }

    public void ClearLastDoorChoice()
    {
        if (!IsServer) return;
        lastChosenRoomIndex.Value = -1;
        lastChosenAnswerIndex.Value = -1;
    }

    // Simple teleport helper used by server-side game logic.
    public void TeleportTo(Vector3 worldPosition)
    {
        if (!IsServer) return;
        Debug.Log($"[PlayerState] Teleport. clientId={OwnerClientId}, to={worldPosition}");

        CharacterController cc = GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        transform.position = worldPosition;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        if (cc != null) cc.enabled = true;

        // Player prefab uses client-authority transform, so owner client must also
        // apply the teleport locally for reliable knockback behavior.
        var target = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new[] { OwnerClientId }
            }
        };
        TeleportOwnerClientRpc(worldPosition, target);
    }

    [ServerRpc]
    public void RequestRespawnServerRpc(ServerRpcParams rpcParams = default)
    {
        if (rpcParams.Receive.SenderClientId != OwnerClientId) return;
        Debug.Log($"[PlayerState] Respawn requested. clientId={OwnerClientId}");

        CheckpointManager checkpoints = FindObjectOfType<CheckpointManager>();
        if (checkpoints != null)
        {
            checkpoints.TeleportToLastCheckpoint(this);
        }
    }

    [ClientRpc]
    private void TeleportOwnerClientRpc(Vector3 worldPosition, ClientRpcParams clientRpcParams = default)
    {
        CharacterController cc = GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        transform.position = worldPosition;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        if (cc != null) cc.enabled = true;
        Debug.Log($"[PlayerState] Owner client teleport applied. clientId={OwnerClientId}, to={worldPosition}");
    }
}
