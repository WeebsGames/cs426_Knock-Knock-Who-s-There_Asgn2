using UnityEngine;
using Unity.Netcode;

public class BumpObstacle : NetworkBehaviour
{
    public float pushDistance = 1.0f;
    public float cooldownSeconds = 0.25f;

    private float nextAllowedTime = 0f;

    private void OnCollisionEnter(Collision collision)
    {
        // TEMP: log to verify it runs
        Debug.Log("Obstacle hit: " + collision.collider.name + " tag=" + collision.collider.tag);

        if (!IsServer) return;
        if (Time.time < nextAllowedTime) return;

        if (!collision.collider.CompareTag("Player")) return;

        // Get the NetworkObject on the PLAYER ROOT
        NetworkObject playerNO = collision.collider.GetComponentInParent<NetworkObject>();
        if (playerNO == null) return;

        Vector3 pushDir = collision.GetContact(0).normal;
        pushDir.y = 0f;
        pushDir.Normalize();

        // flip direction
        pushDir = -pushDir;

        playerNO.transform.position += pushDir * pushDistance;

        nextAllowedTime = Time.time + cooldownSeconds;
    }
}
