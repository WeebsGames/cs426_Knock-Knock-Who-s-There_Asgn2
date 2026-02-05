using Unity.Services.Lobbies.Models;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class CannonAim : MonoBehaviour
{
    //weapon_id 1
    Transform t;
    RaycastHit hit;
    void Start()
    {
        t = GetComponent<Transform>();
        // cannon = GetComponent<GameObject>();
    }

    //rotate the cannon towards whatever the camera is looking at
    //if the camera isn't looking at an object, match the camera's rotation

    void RotateCannon()
    {
        // Debug.Log("Broadcast Recieved");
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0));

        if(Physics.Raycast(ray, out hit))
        {
            Debug.DrawRay(ray.origin, hit.transform.rotation.eulerAngles, Color.blue, 100f);
            if(hit.rigidbody != null || hit.collider != null)
            {
                t.LookAt(hit.point);
            }
            else
            {
                // Debug.Log("reset cannon position");
                t.transform.localEulerAngles = new Vector3(0,0,0);
            }
            // t.transform.localEulerAngles -= new Vector3(0.25f,0,0);
        }
        else
        {
            // Debug.Log("reset cannon position");
            t.transform.localEulerAngles = new Vector3(0,0,0);
        }
        
        if(Mouse.current.rightButton.isPressed){
            t.transform.localEulerAngles = new Vector3(0,0,0);
        }

        Debug.DrawLine(Camera.main.transform.position, hit.point, Color.blue, 5.0f);
        Debug.DrawRay(t.transform.position, t.transform.TransformDirection(Vector3.forward)*10, Color.red, 5.0f);
        // Debug.Log("Broadcasting ShootBullet");
        SendMessageUpwards("ShootBullet", hit);
    }
}
