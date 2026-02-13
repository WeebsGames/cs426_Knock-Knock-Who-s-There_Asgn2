using UnityEngine;
using Unity.Netcode;

public class BumpObstacle : NetworkBehaviour
{
    [Header("Bump")]
    public float pushDistance = 1.0f;
    public float cooldownSeconds = 0.25f;

    private float _nextAllowedTime = 0f;

    private void OnCollisionEnter(Collision collision)
    {
        if (!IsServer) return;                 // server/host decides
        if (Time.time < _nextAllowedTime) return;

        if (!collision.collider.CompareTag("Player")) return;

        var playerNO = collision.collider.GetComponentInParent<NetworkObject>();
        if (playerNO == null) return;

        // Push away from obstacle using the collision normal
        Vector3 pushDir = collision.GetContact(0).normal;
        pushDir.y = 0f;
        if (pushDir.sqrMagnitude < 0.0001f) return;
        pushDir.Normalize();

        playerNO.transform.position += pushDir * pushDistance;

        _nextAllowedTime = Time.time + cooldownSeconds;
    }
}
