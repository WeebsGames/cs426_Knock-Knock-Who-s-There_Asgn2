using UnityEngine;

public class Firewall : MonoBehaviour
{
    
    public float force = 100;    

    private Rigidbody rb;
    private ParticleSystem part;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        part = GetComponentInChildren<ParticleSystem>();
        part.Stop();
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            rb.AddForce(collision.transform.forward * force);
            part.Play();
        }
    }
}
