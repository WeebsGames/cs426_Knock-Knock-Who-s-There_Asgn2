using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public enum GamePhase
{
    Setup = 0,
    Race = 1,
    Finished = 2
}

public enum PlayerRole
{
    None = 0,
    Thief = 1,
    Defender = 2,
    Spectator = 3
}

public class NetworkGameManager : NetworkBehaviour
{
    private const int TotalRooms = 10;

    public static NetworkGameManager Instance { get; private set; }

    [SerializeField] private CheckpointManager checkpointManager;

    private NetworkVariable<GamePhase> phase =
        new(GamePhase.Setup, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private NetworkVariable<int> selectedQuestionSet =
        new(-1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private NetworkVariable<ulong> winnerClientId =
        new(ulong.MaxValue, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private readonly Dictionary<ulong, PlayerRole> playerRoles = new();
    private readonly Dictionary<ulong, PlayerNetworkState> playerStates = new();
    private readonly List<ulong> joinOrder = new();

    private readonly HashSet<int> thiefTrapRooms = new();
    private bool thiefReady;
    private bool defenderReady;

    public GamePhase CurrentPhase => phase.Value;
    public int SelectedQuestionSetIndex => selectedQuestionSet.Value;
    public ulong WinnerClientId => winnerClientId.Value;

    private void Awake()
    {
        Instance = this;
        Debug.Log("[NGM] Awake. Game manager created.");
    }

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;
        Debug.Log("[NGM] Server OnNetworkSpawn. Phase = Setup.");

        if (checkpointManager == null)
        {
            checkpointManager = FindObjectOfType<CheckpointManager>();
        }

        NetworkManager.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.OnClientDisconnectCallback += OnClientDisconnected;

        foreach (ulong clientId in NetworkManager.ConnectedClientsIds)
        {
            OnClientConnected(clientId);
        }
    }

    public override void OnNetworkDespawn()
    {
        if (!IsServer || NetworkManager == null) return;

        NetworkManager.OnClientConnectedCallback -= OnClientConnected;
        NetworkManager.OnClientDisconnectCallback -= OnClientDisconnected;
    }

    private void OnClientConnected(ulong clientId)
    {
        if (!joinOrder.Contains(clientId))
        {
            joinOrder.Add(clientId);
        }

        Debug.Log($"[NGM] Client connected. clientId={clientId}, joinIndex={joinOrder.IndexOf(clientId)}");
        RebuildRoles();
    }

    private void OnClientDisconnected(ulong clientId)
    {
        Debug.Log($"[NGM] Client disconnected. clientId={clientId}");
        playerRoles.Remove(clientId);
        playerStates.Remove(clientId);
        joinOrder.Remove(clientId);
        RebuildRoles();
    }

    private void AssignRoleIfPossible(ulong clientId)
    {
        if (playerRoles.ContainsKey(clientId)) return;

        int index = joinOrder.IndexOf(clientId);
        PlayerRole role = PlayerRole.Spectator;

        if (index == 0) role = PlayerRole.Thief;
        else if (index == 1) role = PlayerRole.Defender;

        playerRoles[clientId] = role;
        Debug.Log($"[NGM] Role assigned. clientId={clientId}, role={role}");

        if (playerStates.TryGetValue(clientId, out var state))
        {
            state.SetRole(role);
        }
    }

    private void RebuildRoles()
    {
        playerRoles.Clear();

        for (int i = 0; i < joinOrder.Count; i++)
        {
            ulong clientId = joinOrder[i];
            PlayerRole role = PlayerRole.Spectator;

            if (i == 0) role = PlayerRole.Thief;
            else if (i == 1) role = PlayerRole.Defender;

            playerRoles[clientId] = role;
            Debug.Log($"[NGM] Role assigned. clientId={clientId}, role={role}");

            if (playerStates.TryGetValue(clientId, out var state))
            {
                state.SetRole(role);
            }
        }
    }

    public void RegisterPlayer(PlayerNetworkState state)
    {
        if (!IsServer || state == null) return;

        ulong clientId = state.OwnerClientId;
        playerStates[clientId] = state;

        if (!joinOrder.Contains(clientId))
        {
            joinOrder.Add(clientId);
        }

        RebuildRoles();

        if (playerRoles.TryGetValue(clientId, out var role))
        {
            state.SetRole(role);
        }

        Debug.Log($"[NGM] Player registered. clientId={clientId}, role={GetRole(clientId)}");

        state.SetCurrentRoom(0);
        state.SetScore(0);
        state.ClearLastDoorChoice();

        if (checkpointManager != null)
        {
            checkpointManager.TeleportPlayerToStart(state);
        }
    }

    public void UnregisterPlayer(PlayerNetworkState state)
    {
        if (!IsServer || state == null) return;
        playerStates.Remove(state.OwnerClientId);
    }

    public PlayerRole GetRole(ulong clientId)
    {
        return playerRoles.TryGetValue(clientId, out var role) ? role : PlayerRole.None;
    }

    public void SelectTrapRoom(int roomIndex)
    {
        SelectTrapRoomServerRpc(roomIndex);
    }

    public void SelectQuestionSet(int setIndex)
    {
        SelectQuestionSetServerRpc(setIndex);
    }

    public void ConfirmReady()
    {
        ConfirmReadyServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void SelectTrapRoomServerRpc(int roomIndex, ServerRpcParams rpcParams = default)
    {
        if (phase.Value != GamePhase.Setup) return;
        if (roomIndex < 0 || roomIndex >= TotalRooms) return;

        ulong sender = rpcParams.Receive.SenderClientId;
        PlayerRole role = GetRole(sender);
        if (role != PlayerRole.Thief)
        {
            Debug.Log($"[NGM] Trap select rejected. clientId={sender}, role={role}, room={roomIndex}");
            return;
        }

        if (thiefTrapRooms.Contains(roomIndex))
        {
            thiefTrapRooms.Remove(roomIndex);
            Debug.Log($"[NGM] Trap room removed by Thief. room={roomIndex}");
        }
        else if (thiefTrapRooms.Count < 3)
        {
            thiefTrapRooms.Add(roomIndex);
            Debug.Log($"[NGM] Trap room added by Thief. room={roomIndex}");
        }
        else
        {
            Debug.Log($"[NGM] Trap room ignored. Already 3 selected. attemptedRoom={roomIndex}");
        }

        thiefReady = false;
        Debug.Log($"[NGM] Trap rooms now: [{GetTrapRoomsString()}]");
    }

    [ServerRpc(RequireOwnership = false)]
    private void SelectQuestionSetServerRpc(int setIndex, ServerRpcParams rpcParams = default)
    {
        if (phase.Value != GamePhase.Setup) return;
        if (setIndex < 0 || setIndex > 2) return;

        ulong sender = rpcParams.Receive.SenderClientId;
        PlayerRole role = GetRole(sender);
        if (role != PlayerRole.Defender)
        {
            Debug.Log($"[NGM] Question set rejected. clientId={sender}, role={role}, set={setIndex}");
            return;
        }

        selectedQuestionSet.Value = setIndex;
        defenderReady = false;
        Debug.Log($"[NGM] Question set selected by Defender. setIndex={setIndex} ({GetQuestionSetName(setIndex)})");
    }

    [ServerRpc(RequireOwnership = false)]
    private void ConfirmReadyServerRpc(ServerRpcParams rpcParams = default)
    {
        if (phase.Value != GamePhase.Setup) return;

        ulong sender = rpcParams.Receive.SenderClientId;
        PlayerRole role = GetRole(sender);

        if (role == PlayerRole.Thief)
        {
            thiefReady = thiefTrapRooms.Count == 3;
            Debug.Log($"[NGM] Thief pressed ready. ready={thiefReady}, selectedTrapRooms={thiefTrapRooms.Count}");
        }
        else if (role == PlayerRole.Defender)
        {
            defenderReady = selectedQuestionSet.Value >= 0;
            Debug.Log($"[NGM] Defender pressed ready. ready={defenderReady}, selectedQuestionSet={selectedQuestionSet.Value}");
        }
        else
        {
            Debug.Log($"[NGM] Ready ignored for role={role}, clientId={sender}");
        }

        Debug.Log($"[NGM] Ready status. thiefReady={thiefReady}, defenderReady={defenderReady}");
        TryStartRace();
    }

    private void TryStartRace()
    {
        if (!IsServer) return;
        if (!thiefReady || !defenderReady)
        {
            Debug.Log($"[NGM] Race not started. Waiting ready. thiefReady={thiefReady}, defenderReady={defenderReady}");
            return;
        }
        if (thiefTrapRooms.Count != 3)
        {
            Debug.Log($"[NGM] Race not started. Need exactly 3 trap rooms. current={thiefTrapRooms.Count}");
            return;
        }
        if (selectedQuestionSet.Value < 0)
        {
            Debug.Log("[NGM] Race not started. Defender has not selected a question set.");
            return;
        }

        // Trap behavior:
        // For selected trap rooms, every wrong door is trap.
        // Correct door is always safe.
        Debug.Log($"[NGM] Trap behavior active: selected trap rooms punish all wrong doors.");

        foreach (var kvp in playerStates)
        {
            PlayerNetworkState player = kvp.Value;
            player.SetScore(0);
            player.SetCurrentRoom(0);
            player.ClearLastDoorChoice();

            if (checkpointManager != null)
            {
                checkpointManager.TeleportPlayerToStart(player);
            }
        }

        winnerClientId.Value = ulong.MaxValue;
        phase.Value = GamePhase.Race;
        Debug.Log($"[NGM] Race started. questionSet={selectedQuestionSet.Value} ({GetQuestionSetName(selectedQuestionSet.Value)}), trapRooms=[{GetTrapRoomsString()}]");
    }

    public void HandleDoorChoice(ulong clientId, int roomIndex, int answerIndex)
    {
        if (!IsServer || phase.Value != GamePhase.Race) return;
        if (roomIndex < 0 || roomIndex >= TotalRooms) return;
        if (answerIndex < 0 || answerIndex > 3) return;
        if (!playerStates.TryGetValue(clientId, out var player)) return;

        if (player.CurrentRoomIndex != roomIndex)
        {
            // Enforce in-order rooms.
            Debug.Log($"[NGM] Door ignored (wrong room order). clientId={clientId}, expectedRoom={player.CurrentRoomIndex}, submittedRoom={roomIndex}");
            return;
        }

        List<QuestionData> questions = QuestionBank.GetQuestionSet(selectedQuestionSet.Value);
        QuestionData q = questions[roomIndex];

        bool isTrapRoom = thiefTrapRooms.Contains(roomIndex);
        bool isCorrect = answerIndex == q.correctIndex;
        bool isTrapDoor = isTrapRoom && !isCorrect;
        Debug.Log($"[NGM] Door choice. clientId={clientId}, room={roomIndex}, answer={answerIndex}, correct={q.correctIndex}, trapRoom={isTrapRoom}, isTrapHit={isTrapDoor}");

        if (isTrapDoor)
        {
            player.AddScore(-5);
            if (checkpointManager != null)
            {
                checkpointManager.TeleportPlayerToStart(player);
            }
            Debug.Log($"[NGM] Trap hit. clientId={clientId}, score={player.Score}, roomProgress={player.CurrentRoomIndex}, action=teleport_to_start");
            return;
        }

        if (isCorrect)
        {
            player.AddScore(1);
            player.SetCurrentRoom(roomIndex + 1);

            // Move player to next room checkpoint (except after final room answer).
            if (roomIndex < TotalRooms - 1 && checkpointManager != null)
            {
                checkpointManager.TeleportPlayerToRoom(player, roomIndex + 1);
            }
            Debug.Log($"[NGM] Correct answer. clientId={clientId}, newScore={player.Score}, newRoomProgress={player.CurrentRoomIndex}");
        }
        else
        {
            player.AddScore(-2);
            Debug.Log($"[NGM] Wrong normal door. clientId={clientId}, newScore={player.Score}, roomProgress={player.CurrentRoomIndex}");
        }
    }

    public void TryFinishRace(ulong clientId)
    {
        if (!IsServer || phase.Value != GamePhase.Race) return;
        if (!playerStates.TryGetValue(clientId, out var player)) return;

        // Must complete all rooms first.
        if (player.CurrentRoomIndex < TotalRooms)
        {
            Debug.Log($"[NGM] Finish ignored. clientId={clientId}, currentRoomIndex={player.CurrentRoomIndex} (needs {TotalRooms}).");
            return;
        }

        phase.Value = GamePhase.Finished;
        winnerClientId.Value = clientId;
        Debug.Log($"[NGM] Race finished. WINNER clientId={clientId}, finalScore={player.Score}");
    }

    private string GetTrapRoomsString()
    {
        List<int> rooms = new(thiefTrapRooms);
        rooms.Sort();
        return string.Join(", ", rooms);
    }

    private string GetQuestionSetName(int setIndex)
    {
        return setIndex switch
        {
            0 => "A",
            1 => "B",
            2 => "C",
            _ => "Unknown"
        };
    }
}
