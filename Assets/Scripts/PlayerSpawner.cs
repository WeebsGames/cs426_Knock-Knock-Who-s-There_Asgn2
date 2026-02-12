using UnityEngine;
using Unity.Netcode;

//this was vibe coded.

// Simple server-side spawner for Netcode for GameObjects.
// Usage:
// - Add this script to an object in the scene (e.g., "GameManager").
// - Populate the `spawnPoints` array in the inspector.
// - Assign the `playerPrefab` (NetworkObject-based prefab) in the inspector.
// - Make sure the NetworkManager does NOT also auto-create player objects (clear its Player Prefab or disable auto-creation) to avoid duplicates.
public class PlayerSpawner : MonoBehaviour
{
    public Transform[] spawnPoints;
    public GameObject playerPrefab; // networked prefab assigned in inspector

    private void Start()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        }
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        if (!NetworkManager.Singleton.IsServer) return;
        if (playerPrefab == null)
        {
            Debug.LogError("PlayerSpawner: playerPrefab is not assigned.");
            return;
        }

        Vector3 pos = Vector3.zero;
        Quaternion rot = Quaternion.identity;

        if (spawnPoints != null && spawnPoints.Length > 0)
        {
            int index = (int)(clientId % (ulong)spawnPoints.Length);
            Transform chosen = spawnPoints[index];
            pos = chosen.position;
            rot = chosen.rotation;
        }

        GameObject playerInstance = Instantiate(playerPrefab, pos, rot);
        NetworkObject netObj = playerInstance.GetComponent<NetworkObject>();
        if (netObj == null)
        {
            Debug.LogError("PlayerSpawner: playerPrefab does not contain a NetworkObject component.");
            Destroy(playerInstance);
            return;
        }

        netObj.SpawnAsPlayerObject(clientId);
    }
}
