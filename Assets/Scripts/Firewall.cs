using UnityEngine;

public class Firewall : MonoBehaviour
{
    
    public float force = 100;    

    private Rigidbody rb;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            rb.AddForce(collision.transform.forward * force);
        }
    }
}
