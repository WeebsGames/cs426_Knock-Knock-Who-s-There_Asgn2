using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
// adding namespaces
using Unity.Netcode;
// because we are using the NetworkBehaviour class
// NewtorkBehaviour class is a part of the Unity.Netcode namespace
// extension of MonoBehaviour that has functions related to multiplayer
public class PlayerMovement : NetworkBehaviour
{
    public float speed = 2f;
    public float force = 70f;
    public float maxSpeed = 10.0f;
    public float sensitivity = 1.0f;
    // create a list of colors
    public List<Color> colors = new List<Color>();
    Vector3 dir = new Vector3(0,0,0);
    bool grounded = true;

    // getting the reference to the prefab
    [SerializeField]
    private GameObject spawnedPrefab;
    // save the instantiated prefab
    private GameObject instantiatedPrefab;

    public GameObject cannon;
    public GameObject bullet;

    // reference to the camera audio listener
    [SerializeField] private AudioListener audioListener;
    // reference to the camera
    [SerializeField] private Camera playerCamera;

    Rigidbody rb;
    Transform t;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        t = GetComponent<Transform>();

        rb.freezeRotation = true;
        Cursor.lockState = CursorLockMode.Locked;
    }
    // Update is called once per frame
    void Update()
    {
        // check if the player is the owner of the object
        // makes sure the script is only executed on the owners 
        // not on the other prefabs 
        if (!IsOwner) return;

        Vector3 moveDirection = new Vector3(0, 0, 0);

        if (Keyboard.current.wKey.isPressed)
        {
            dir += t.forward;
        }
        if (Keyboard.current.sKey.isPressed)
        {
            dir -= t.forward;
        }
        if (Keyboard.current.dKey.isPressed)
        {
            dir += t.right;
        }
        if (Keyboard.current.aKey.isPressed)
        {
            dir -= t.right;
        }
        // Time.deltaTime represents the time that passed since the last frame
        //the multiplication below ensures that GameObject moves constant speed every frame
        //translate directional input to movement
        if(rb.linearVelocity.magnitude < maxSpeed)
        {
            rb.linearVelocity += dir * speed * Time.deltaTime;
        }
        //reset directional input vector
        dir = Vector3.zero;

        if (Keyboard.current.spaceKey.wasPressedThisFrame && grounded){
            rb.AddForce(t.up * force);
            grounded = false;
        }

        //horizontal movement with mouse
        Vector3 mouseDelta = UnityEngine.InputSystem.Mouse.current.delta.ReadValue();
        if (mouseDelta.x < 0) {
            // Mouse moved left
            // Debug.Log("mouse move lef by " + mouseDelta.x);
            t.rotation *= Quaternion.Euler(0f, mouseDelta.x * sensitivity, 0f);
        } else if (mouseDelta.x > 0) {
            // Mouse moved right
            // Debug.Log("mouse move rig by" + mouseDelta.x);
            t.rotation *= Quaternion.Euler(0f, mouseDelta.x * sensitivity, 0f);
        }

        // if I is pressed spawn the object 
        // if J is pressed destroy the object
        if (Input.GetKeyDown(KeyCode.I))
        {
            //instantiate the object
            instantiatedPrefab = Instantiate(spawnedPrefab);
            // spawn it on the scene
            instantiatedPrefab.GetComponent<NetworkObject>().Spawn(true);
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            //despawn the object
            instantiatedPrefab.GetComponent<NetworkObject>().Despawn(true);
            // destroy the object
            Destroy(instantiatedPrefab);
        }

        if (Input.GetButtonDown("Fire1"))
        {
            // call the BulletSpawningServerRpc method
            // as client can not spawn objects
            cannon.SendMessage("RotateCannon");
        }
    }

    void ShootBullet(RaycastHit hit)
    {
        BulletSpawningServerRpc(cannon.transform.position, cannon.transform.rotation);
    }

    void OnCollisionEnter(Collision collision)
    {
        // Debug.Log("collision detected with tag: " + collision.transform.tag);
        if(collision.transform.tag == "ground")
        {
            grounded = true;
        }
    }

    // void OnCollisionExit(Collision collision)
    // {
    //     if(collision.transform.tag == "ground")
    //     {
    //         grounded = false;   
    //     }
    // }

    // this method is called when the object is spawned
    // we will change the color of the objects
    public override void OnNetworkSpawn()
    {
        GetComponent<MeshRenderer>().material.color = colors[(int)OwnerClientId];

        // check if the player is the owner of the object
        if (!IsOwner) return;
        // if the player is the owner of the object
        // enable the camera and the audio listener
        audioListener.enabled = true;
        playerCamera.enabled = true;
    }

    // need to add the [ServerRPC] attribute
    [ServerRpc]
    // method name must end with ServerRPC
    private void BulletSpawningServerRpc(Vector3 position, Quaternion rotation)
    {
        // call the BulletSpawningClientRpc method to locally create the bullet on all clients
        BulletSpawningClientRpc(position, rotation);
    }

    [ClientRpc]
    private void BulletSpawningClientRpc(Vector3 position, Quaternion rotation)
    {
        GameObject newBullet = Instantiate(bullet, position, rotation);
        newBullet.GetComponent<Rigidbody>().linearVelocity += Vector3.up * 2;
        newBullet.GetComponent<Rigidbody>().AddForce(newBullet.transform.forward * 1500);
        // newBullet.GetComponent<NetworkObject>().Spawn(true);
    }
}