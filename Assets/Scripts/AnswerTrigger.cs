using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR.WSA;

public class AnswerTrigger : MonoBehaviour
{

    public bool ans;
    public Transform checkpoint;
    public scoreManager scoreManager;

    private Renderer rend;
    private Color color;
    private void Start()
    {
        rend = GetComponent<Renderer>();
    }
    private void OnTriggerEnter(Collider other)
    {
        
        
        if (ans)
        {
            color.a = 0.5f;
            color.g = 255;
            rend.material.color = color;
            Debug.Log("sent message upwards");
            scoreManager.AddPoint(1);
        } else
        {
            color.a = 0.5f;
            color.r = 255;
            rend.material.color = color;
            other.transform.position = checkpoint.position;
            scoreManager.AddPoint(-1);
        }
        Invoke("resetColor", 2f);
    }

    private void resetColor()
    {
        color.a = 0f;
        rend.material.color = color;
    }
}
