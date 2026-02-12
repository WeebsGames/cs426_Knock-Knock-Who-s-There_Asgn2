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
    public Transform[] spawnPoints; //array of spawnpoint locations
    public GameObject playerPrefab; // networked prefab assigned in inspector

    private void Start()
    {
        if (NetworkManager.Singleton != null) //adds a client to the server
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        }
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton != null) //removes a client from the server
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        if (!NetworkManager.Singleton.IsServer) return; //prevents the server from executing this code
        if (playerPrefab == null)
        {
            Debug.LogError("PlayerSpawner: playerPrefab is not assigned.");
            return;
        }

        Vector3 pos = Vector3.zero;
        Quaternion rot = Quaternion.identity;


        //chooses a spawnpoint based on the player's id and spawnpoint array length
        if (spawnPoints != null && spawnPoints.Length > 0) //checks if anything is in the spawnpoint arr
        {
            int index = (int)(clientId % (ulong)spawnPoints.Length); //assigns an index based off the client's id
            Transform chosen = spawnPoints[index]; 
            pos = chosen.position;
            rot = chosen.rotation;
        }

        //creates a new player object at the spawnpoint's location
        GameObject playerInstance = Instantiate(playerPrefab, pos, rot);
        NetworkObject netObj = playerInstance.GetComponent<NetworkObject>();
        if (netObj == null)
        {
            Debug.LogError("PlayerSpawner: playerPrefab does not contain a NetworkObject component.");
            Destroy(playerInstance);
            return;
        }
        //spawns the player on the server
        netObj.SpawnAsPlayerObject(clientId);
    }
}
